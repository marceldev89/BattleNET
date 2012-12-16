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
        public int packetsDone = 0;
        public int packetsTodo = 0;
    }

    public class BattlEyeClient
    {
        private Socket _socket;

        private DateTime _commandSend;
        private DateTime _responseReceived;

        private EBattlEyeDisconnectionType _disconnectionType;

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

        private EBattlEyeCommandResult SendLoginPacket(string command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Helpers.String2Bytes(Helpers.Hex2Ascii("FF00") + command)).Aggregate<byte, string>(null,
                                                                                                            (current, b)
                                                                                                            =>
                                                                                                            current +
                                                                                                            b.ToString(
                                                                                                                "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF00") + command;
                _socket.Send(Helpers.String2Bytes(packet));

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

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Helpers.String2Bytes(Helpers.Hex2Ascii("FF02") + command)).Aggregate<byte, string>(null,
                                                                                                            (current, b)
                                                                                                            =>
                                                                                                            current +
                                                                                                            b.ToString(
                                                                                                                "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF02") + command;
                _socket.Send(Helpers.String2Bytes(packet));

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

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Helpers.String2Bytes(Helpers.Hex2Ascii("FF01") + Helpers.Bytes2String(new byte[] { (byte)_packetNumber }) + command)).Aggregate<byte, string>(null,
                                                                                                            (current, b)
                                                                                                            =>
                                                                                                            current +
                                                                                                            b.ToString(
                                                                                                                "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF01") + Helpers.Bytes2String(new byte[] { (byte)_packetNumber }) + command;
                _socket.Send(Helpers.String2Bytes(packet));
                _commandSend = DateTime.Now;

                if (log)
                {
                    _packetLog.Add(_packetNumber, packet);
                    _packetNumber++;
                }
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }

        [System.Obsolete("Marked for removal, please use BattlEyeClient.SendCommandPacket(string command)")]
        public EBattlEyeCommandResult SendCommandPacket(EBattlEyeCommand command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash =
                    crc32.ComputeHash(
                        Helpers.String2Bytes(Helpers.Hex2Ascii("FF01") + Helpers.Bytes2String(new byte[] { (byte)_packetNumber }) +
                                                  Helpers.StringValueOf(command))).Aggregate<byte, string>(
                                                      null,
                                                      (current, b)
                                                      =>
                                                      current +
                                                      b.ToString(
                                                          "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF01") + Helpers.Bytes2String(new byte[] { (byte)_packetNumber }) +
                         Helpers.StringValueOf(command);
                _socket.Send(Helpers.String2Bytes(packet));
                _commandSend = DateTime.Now;
                _packetLog.Add(_packetNumber, packet);
                _packetNumber++;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }

        [System.Obsolete("Marked for removal, please use BattlEyeClient.SendCommandPacket(string command)")]
        public EBattlEyeCommandResult SendCommandPacket(EBattlEyeCommand command, string parameters)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash =
                    crc32.ComputeHash(
                        Helpers.String2Bytes(Helpers.Hex2Ascii("FF01") + Helpers.Bytes2String(new byte[] { (byte)_packetNumber }) +
                                                  Helpers.StringValueOf(command) + parameters)).Aggregate
                        <byte, string>(null,
                                       (current, b)
                                       =>
                                       current +
                                       b.ToString(
                                           "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF01") + Helpers.Bytes2String(new byte[] { (byte)_packetNumber }) +
                         Helpers.StringValueOf(command) + parameters;
                _socket.Send(Helpers.String2Bytes(packet));
                _commandSend = DateTime.Now;
                _packetLog.Add(_packetNumber, packet);
                _packetNumber++;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }

        public bool IsConnected()
        {
            return _socket != null && _socket.Connected;
        }

        public EBattlEyeConnectionResult Connect()
        {
            try
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

                OnMessageReceived(string.Format("Connecting to {0}:{1}... ", _loginCredentials.Host,
                                                _loginCredentials.Port));

                try
                {                    
                    _socket.Connect(remoteEP);

                    if (SendLoginPacket(_loginCredentials.Password) == EBattlEyeCommandResult.Error)
                        return EBattlEyeConnectionResult.ConnectionFailed;

                    var bytesReceived = new Byte[4096];
                    int bytes = 0;

                    try
                    {
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
                    catch (Exception)
                    {
                        Disconnect(EBattlEyeDisconnectionType.ConnectionFailed);
                    }
                }
                catch (Exception)
                {
                    return EBattlEyeConnectionResult.ConnectionFailed;
                }
            }
            catch (Exception)
            {
                return EBattlEyeConnectionResult.ParseError;
            }

            return EBattlEyeConnectionResult.Success;
        }

        public void Disconnect()
        {
            _keepRunning = false;
            _disconnectionType = EBattlEyeDisconnectionType.Manual;

            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }

            OnDisconnect(_loginCredentials, _disconnectionType);
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

        private void Disconnect(EBattlEyeDisconnectionType disconnectionType)
        {
            _keepRunning = false;
            _disconnectionType = disconnectionType;

            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }

            OnDisconnect(_loginCredentials, _disconnectionType);
        }

        private void Receive()
        {
            StateObject state = new StateObject();
            state.workSocket = _socket;

            _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

            _disconnectionType = EBattlEyeDisconnectionType.ConnectionLost;

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

                    if (_packetLog.Count > 0)
                    {
                        int key = _packetLog.First().Key;
                        SendCommandPacket(_packetLog[key]);
                        _packetLog.Remove(key);
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
                        OnDisconnect(_loginCredentials, _disconnectionType);
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

                            if (state.packetsDone < state.packetsTodo)
                            {
                                state.sb.Append(Helpers.Bytes2String(state.buffer, 12, bytesRead - 12));
                                state.packetsDone++;
                            }

                            if (state.packetsDone == state.packetsTodo)
                            {
                                OnMessageReceived(state.sb.ToString());
                                state.sb = new StringBuilder();
                                state.packetsDone = 0;
                                state.packetsTodo = 0;
                            }
                        }
                        else
                        {
                            // Temporary fix to avoid infinite loops with multi-packet server messages
                            state.sb = new StringBuilder();
                            state.packetsDone = 0;
                            state.packetsTodo = 0;

                            OnMessageReceived(Helpers.Bytes2String(state.buffer, 9, bytesRead - 9));
                        }
                    }

                    _packetLog.Remove(state.buffer[8]);
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