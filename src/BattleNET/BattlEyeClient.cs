using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BattleNET
{
    public class BattlEyeClient : IBattleNET
    {
        private Socket _socket;

        private DateTime _commandSend;
        private DateTime _responseReceived;

        private EBattlEyeDisconnectionType _disconnectionType;

        private bool _ranBefore;
        private bool _keepRunning;
        private bool _reconnectOnPacketLoss;

        private Thread _doWork;
        private Thread _keepAlive;

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
                string hash = crc32.ComputeHash(Encoding.GetEncoding(1252).GetBytes(Helpers.Hex2Ascii("FF00") + command)).Aggregate<byte, string>(null,
                                                                                                            (current, b)
                                                                                                            =>
                                                                                                            current +
                                                                                                            b.ToString(
                                                                                                                "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF00") + command;
                _socket.Send(Encoding.GetEncoding(1252).GetBytes(packet));

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
                string hash = crc32.ComputeHash(Encoding.GetEncoding(1252).GetBytes(Helpers.Hex2Ascii("FF02") + command)).Aggregate<byte, string>(null,
                                                                                                            (current, b)
                                                                                                            =>
                                                                                                            current +
                                                                                                            b.ToString(
                                                                                                                "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF02") + command;
                _socket.Send(Encoding.GetEncoding(1252).GetBytes(packet));

                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }

        public EBattlEyeCommandResult SendCommandPacket(string command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Encoding.GetEncoding(1252).GetBytes(Helpers.Hex2Ascii("FF01") + Encoding.GetEncoding(1252).GetString(new byte[] { 0 }) + command)).Aggregate<byte, string>(null,
                                                                                                            (current, b)
                                                                                                            =>
                                                                                                            current +
                                                                                                            b.ToString(
                                                                                                                "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF01") + Encoding.GetEncoding(1252).GetString(new byte[] { 0 }) + command;
                _socket.Send(Encoding.GetEncoding(1252).GetBytes(packet));

                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }

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
                        Encoding.GetEncoding(1252).GetBytes(Helpers.Hex2Ascii("FF01") + Encoding.GetEncoding(1252).GetString(new byte[] { 0 }) +
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
                packet = header + Helpers.Hex2Ascii("FF01") + Encoding.GetEncoding(1252).GetString(new byte[] { 0 }) +
                         Helpers.StringValueOf(command);
                _socket.Send(Encoding.GetEncoding(1252).GetBytes(packet));
                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Success;
        }


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
                        Encoding.GetEncoding(1252).GetBytes(Helpers.Hex2Ascii("FF01") + Encoding.GetEncoding(1252).GetString(new byte[] { 0 }) +
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
                packet = header + Helpers.Hex2Ascii("FF01") + Encoding.GetEncoding(1252).GetString(new byte[] { 0 }) +
                         Helpers.StringValueOf(command) + parameters;
                _socket.Send(Encoding.GetEncoding(1252).GetBytes(packet));
                _commandSend = DateTime.Now;
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

                _keepRunning = true;
                IPAddress ipAddress = IPAddress.Parse(_loginCredentials.Host);
                EndPoint remoteEP = new IPEndPoint(ipAddress, _loginCredentials.Port);

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.ReceiveBufferSize = Int32.MaxValue;

                OnMessageReceived(string.Format("Connecting to {0}:{1}... ", _loginCredentials.Host,
                                                _loginCredentials.Port));

                try
                {                    
                    _socket.Connect(remoteEP);

                    OnMessageReceived("Connected!");

                    OnMessageReceived("Logging in... ");

                    if (SendLoginPacket(_loginCredentials.Password) == EBattlEyeCommandResult.Error)
                        return EBattlEyeConnectionResult.ConnectionFailed;

                    if (!_ranBefore)
                    {
                        _keepAlive = new Thread(KeepAlive);
                        _keepAlive.Start();
                    }

                    _doWork = new Thread(DoWork);
                    _doWork.Start();

                    _ranBefore = true;
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
        }

        private void DoWork()
        {
            var bytesReceived = new Byte[4096];
            int bytes = 0;

            string buffer = null;
            int bufferCount = 0;
            int packetCount = 0;
            _disconnectionType = EBattlEyeDisconnectionType.ConnectionLost;

            while (_socket.Connected && _keepRunning)
            {
                try
                {
                    bytes = _socket.Receive(bytesReceived, bytesReceived.Length, 0);

                    if (bytesReceived[7] == 0x00)
                    {
                        if (bytesReceived[8] == 0x01)
                        {
                            OnMessageReceived("Logged in!");
                        }
                        else
                        {
                            Disconnect(EBattlEyeDisconnectionType.LoginFailed);
                        }
                    }
                    else if (bytesReceived[7] == 0x02)
                    {
                        SendAcknowledgePacket(Encoding.GetEncoding(1252).GetString(new[] { bytesReceived[8] }));
                        OnMessageReceived(Encoding.GetEncoding(1252).GetString(bytesReceived, 9, bytes - 9));
                    }
                    else if (bytesReceived[7] == 0x01)
                    {
                        if (bytes > 9)
                        {
                            if (bytesReceived[7] == 0x01 && bytesReceived[9] == 0x00)
                            {
                                if (bytesReceived[11] == 0)
                                {
                                    packetCount = bytesReceived[10];
                                }

                                if (bufferCount < packetCount)
                                {
                                    buffer += Encoding.GetEncoding(1252).GetString(bytesReceived, 12, bytes - 12);
                                    bufferCount++;
                                }

                                if (bufferCount == packetCount)
                                {
                                    OnMessageReceived(buffer);
                                    buffer = null;
                                    bufferCount = 0;
                                    packetCount = 0;
                                }
                            }
                            else
                            {
                                OnMessageReceived(Encoding.GetEncoding(1252).GetString(bytesReceived, 9, bytes - 9));
                            }
                        }
                    }

                    _responseReceived = DateTime.Now;
                    bytesReceived = new Byte[4096];
                }
                catch (Exception)
                {
                    if (_keepRunning)
                    {
                        Disconnect(EBattlEyeDisconnectionType.SocketException);
                    }
                }
            }

            if (!_socket.Connected)
                OnDisconnect(_loginCredentials, _disconnectionType);
        }

        private void KeepAlive()
        {
            while (_socket.Connected && _keepRunning)
            {
                TimeSpan timeoutClient = DateTime.Now - _commandSend;
                TimeSpan timeoutServer = DateTime.Now - _responseReceived;

                if (timeoutClient.TotalSeconds >= 15)
                {
                    SendCommandPacket(null);
                }

                if (timeoutServer.TotalSeconds >= 45)
                {
                    Disconnect(EBattlEyeDisconnectionType.ConnectionLost);

                    if (_reconnectOnPacketLoss)
                    {
                        while (_doWork.IsAlive) { Thread.Sleep(250); }
                        Connect();
                    }
                }

                Thread.Sleep(500);
            }
        }

        public event BattlEyeMessageEventHandler MessageReceivedEvent;
        public event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}