using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace BattleNET
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 4096;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
        public int packetsTodo = 0;
    }

    public class BattlEyeClient
    {
        private Socket _socket;

        private DateTime _commandSend;
        private DateTime _responseReceived;

        private EBattlEyeDisconnectionType? _disconnectionType;

        private bool _keepRunning;
        private bool _reconnectOnPacketLoss;

        private int _packetNumber;
        private SortedDictionary<int, string> _packetLog;

        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public int CommandQueue
        {
            get 
            {
                return _packetLog.Count;
            }
        }

        private void OnMessageReceived(string message)
        {
            if (MessageReceivedEvent != null)
                MessageReceivedEvent(new BattlEyeMessageEventArgs(message));
        }

        private void OnDisconnect(BattlEyeLoginCredentials loginDetails, EBattlEyeDisconnectionType disconnectionType)
        {
            if (DisconnectEvent != null)
                DisconnectEvent(new BattlEyeDisconnectEventArgs(loginDetails, disconnectionType));
        }

        private BattlEyeLoginCredentials _loginCredentials;

        public BattlEyeClient(BattlEyeLoginCredentials loginCredentials)
        {
            _loginCredentials = loginCredentials;
        }

        public EBattlEyeConnectionResult Connect()
        {
            _commandSend = DateTime.Now;
            _responseReceived = DateTime.Now;

            _packetNumber = 0;
            _packetLog = new SortedDictionary<int, string>();

            _keepRunning = true;
            IPAddress ipAddress = IPAddress.Parse(_loginCredentials.Host);
            EndPoint remoteEP = new IPEndPoint(ipAddress, _loginCredentials.Port);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.ReceiveBufferSize = UInt16.MaxValue;
            _socket.ReceiveTimeout = 5000;

            OnMessageReceived(string.Format("Connecting to {0}:{1}... ", _loginCredentials.Host, _loginCredentials.Port));

            try
            {
                _socket.Connect(remoteEP);

                if (SendLoginPacket(_loginCredentials.Password) == EBattlEyeCommandResult.Error)
                    return EBattlEyeConnectionResult.ConnectionFailed;

                var bytesReceived = new Byte[4096];
                int bytes = 0;

                bytes = _socket.Receive(bytesReceived, bytesReceived.Length, 0);

                if (bytesReceived[7] == 0x00)
                {
                    if (bytesReceived[8] == 0x01)
                    {
                        OnMessageReceived("Connected!");

                        Receive();
                    }
                    else
                    {
                        Disconnect(EBattlEyeDisconnectionType.LoginFailed);
                    }
                }
            }
            catch
            {
                if (_disconnectionType == EBattlEyeDisconnectionType.ConnectionLost)
                {
                    Disconnect(EBattlEyeDisconnectionType.ConnectionLost);
                    Connect();
                }
                else
                {
                    Disconnect(EBattlEyeDisconnectionType.ConnectionFailed);
                    return EBattlEyeConnectionResult.ConnectionFailed;
                }
            }

            return EBattlEyeConnectionResult.Success;
        }

        private EBattlEyeCommandResult SendLoginPacket(string command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                byte[] packet = ConstructPacket(0, 0, command);
                _socket.Send(packet);

                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }

        private EBattlEyeCommandResult SendAcknowledgePacket(string command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                byte[] packet = ConstructPacket(2, 0, command);
                _socket.Send(packet);

                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }

        public EBattlEyeCommandResult SendCommandPacket(string command, bool log = true)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                byte[] packet = ConstructPacket(1, _packetNumber, command);

                _socket.Send(packet);
                _commandSend = DateTime.Now;

                if (log)
                {
                    _packetLog.Add(_packetNumber, command);
                    _packetNumber++;
                }
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }

        public EBattlEyeCommandResult SendCommandPacket(EBattlEyeCommand command, string parameters = "")
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                byte[] packet = ConstructPacket(1, _packetNumber, Helpers.StringValueOf(command) + parameters);

                _socket.Send(packet);

                _commandSend = DateTime.Now;

                _packetLog.Add(_packetNumber, Helpers.StringValueOf(command) + parameters);
                _packetNumber++;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }

        private byte[] ConstructPacket(int packetType, int sequenceNumber, string command)
        {
            string type;

            switch (packetType)
            {
                case 0:
                    type = Helpers.Hex2Ascii("FF00");
                    break;
                case 1:
                    type = Helpers.Hex2Ascii("FF01");
                    break;
                case 2:
                    type = Helpers.Hex2Ascii("FF02");
                    break;
                default:
                    return new byte[] { };
            }

            string count = Helpers.Bytes2String(new byte[] { (byte)sequenceNumber });

            byte[] byteArray = new CRC32().ComputeHash(Helpers.String2Bytes(type + ((packetType != 1) ? "" : count) + command));

            string hash = new string(Helpers.Hex2Ascii(BitConverter.ToString(byteArray).Replace("-", "")).ToCharArray().Reverse().ToArray());

            string packet = "BE" + hash + type + ((packetType != 1) ? "" : count) + command;

            return Helpers.String2Bytes(packet);
        }

        public bool IsConnected()
        {
            return _socket != null && _socket.Connected;
        }

        public void Disconnect()
        {
            _keepRunning = false;

            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }

            OnDisconnect(_loginCredentials, EBattlEyeDisconnectionType.Manual);
        }

        private void Disconnect(EBattlEyeDisconnectionType disconnectionType)
        {
            if (disconnectionType == EBattlEyeDisconnectionType.ConnectionLost)
                _disconnectionType = EBattlEyeDisconnectionType.ConnectionLost;

            _keepRunning = false;

            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }

            OnDisconnect(_loginCredentials, disconnectionType);
        }

        public bool ReconnectOnPacketLoss(bool newSetting)
        {
            _reconnectOnPacketLoss = newSetting;
            return _reconnectOnPacketLoss;
        }

        public bool IsReconnectingOnPacketLoss
        {
            get { return _reconnectOnPacketLoss; }
        }

        private void Receive()
        {
            StateObject state = new StateObject();
            state.workSocket = _socket;

            _disconnectionType = null;

            _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

            new Thread(delegate() {
                while (_socket.Connected && _keepRunning)
                {
                    TimeSpan timeoutClient = DateTime.Now - _commandSend;
                    TimeSpan timeoutServer = DateTime.Now - _responseReceived;

                    if (timeoutClient.TotalSeconds >= 5)
                    {
                        if (timeoutServer.TotalSeconds >= 20)
                        {
                            Disconnect(EBattlEyeDisconnectionType.ConnectionLost);
                            _keepRunning = true;
                        }
                        else
                        {
                            if (_packetLog.Count == 0)
                            {
                                SendCommandPacket(null, false);
                            }
                        }
                    }

                    if (_packetLog.Count > 0 && _socket.Available == 0)
                    {
                        try
                        {
                            int key = _packetLog.First().Key;
                            string value = _packetLog[key];
                            SendCommandPacket(value, false);
                            _packetLog.Remove(key);
                        }
                        catch
                        {
                            // Prevent possible crash when packet is received at the same moment it's trying to resend it.
                        }
                    }

                    Thread.Sleep(500);
                }

                if (!_socket.Connected)
                {
                    if (_reconnectOnPacketLoss && _keepRunning)
                    {
                        Connect();
                    }
                    else if (!_keepRunning)
                    {
                         //let the thread finish without further action
                    }
                    else
                    {
                        OnDisconnect(_loginCredentials, EBattlEyeDisconnectionType.ConnectionLost);
                    }
                }
            }).Start();
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if (state.buffer[7] == 0x02)
                {
                    SendAcknowledgePacket(Helpers.Bytes2String(new[] { state.buffer[8] }));
                    OnMessageReceived(Helpers.Bytes2String(state.buffer, 9, bytesRead - 9));
                }
                else if (state.buffer[7] == 0x01)
                {
                    if (bytesRead > 9)
                    {
                        if (state.buffer[7] == 0x01 && state.buffer[9] == 0x00)
                        {
                            if (state.buffer[11] == 0)
                            {
                                state.packetsTodo = state.buffer[10];
                            }

                            if (state.packetsTodo > 0)
                            {
                                state.sb.Append(Helpers.Bytes2String(state.buffer, 12, bytesRead - 12));
                                state.packetsTodo--;
                            }

                            if (state.packetsTodo == 0)
                            {
                                OnMessageReceived(state.sb.ToString());
                                state.sb = new StringBuilder();
                                state.packetsTodo = 0;
                            }
                        }
                        else
                        {
                            // Temporary fix to avoid infinite loops with multi-packet server messages
                            state.sb = new StringBuilder();
                            state.packetsTodo = 0;

                            OnMessageReceived(Helpers.Bytes2String(state.buffer, 9, bytesRead - 9));
                        }
                    }

                    if (_packetLog.ContainsKey(state.buffer[8]))
                    {
                        _packetLog.Remove(state.buffer[8]);
                    }
                }

                _responseReceived = DateTime.Now;

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch
            {
                // do nothing
            }
        }

        public event BattlEyeMessageEventHandler MessageReceivedEvent;
        public event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}