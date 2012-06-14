using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BattleNET
{
    public class BattleNETClient : IBattleNET
    {
        public bool KeepRunning;

        private Socket _socket;

        private DateTime commandSend;
      
        private void OnMessageReceived(string message)
        {
            if (MessageReceivedEvent != null)
                MessageReceivedEvent(new BattlEyeMessageEventArgs(message));
        }

        private void OnDisconnect(BattleEyeLoginCredentials loginDetails)
        {
            if (DisconnectEvent != null)
                DisconnectEvent(new BattlEyeDisconnectEventArgs(loginDetails));
        }

        private BattleEyeLoginCredentials _loginCredentials;

        public BattleNETClient(BattleEyeLoginCredentials loginCredentials)
        {
            _loginCredentials = loginCredentials;
        }

        public EBattlEyeCommandResult SendCommand(string command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;
                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Encoding.Default.GetBytes(command)).Aggregate<byte, string>(null,
                                                                                                             (current, b)
                                                                                                             =>
                                                                                                             current +
                                                                                                             b.ToString(
                                                                                                                 "X2"));
                hash = Helpers.HexToAscii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + command;
                _socket.Send(Encoding.Default.GetBytes(packet));

                commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }
            return EBattlEyeCommandResult.Succes;
        }
        
        public EBattlEyeCommandResult SendCommand(EBattlEyeCommand command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;
                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Encoding.Default.GetBytes(Helpers.HexToAscii("FF01") + Encoding.Default.GetString(new byte[] { 0 }) + Helpers.EnumUtils.StringValueOf(command))).Aggregate<byte, string>(null,
                                                                                                             (current, b)
                                                                                                             =>
                                                                                                             current +
                                                                                                             b.ToString(
                                                                                                                 "X2"));
                hash = Helpers.HexToAscii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.HexToAscii("FF01") + Encoding.Default.GetString(new byte[] {0}) + Helpers.EnumUtils.StringValueOf(command);
                _socket.Send(Encoding.Default.GetBytes(packet));

                commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }
            return EBattlEyeCommandResult.Succes;
        }


        public EBattlEyeCommandResult SendCommand(EBattlEyeCommand command, string parameters)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;
                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Encoding.Default.GetBytes(Helpers.HexToAscii("FF01") + Encoding.Default.GetString(new byte[] { 0 }) + Helpers.EnumUtils.StringValueOf(command) + parameters)).Aggregate<byte, string>(null,
                                                                                                             (current, b)
                                                                                                             =>
                                                                                                             current +
                                                                                                             b.ToString(
                                                                                                                 "X2"));
                hash = Helpers.HexToAscii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.HexToAscii("FF01") + Encoding.Default.GetString(new byte[] { 0 }) + Helpers.EnumUtils.StringValueOf(command) + parameters;
                _socket.Send(Encoding.Default.GetBytes(packet));

                commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }
            return EBattlEyeCommandResult.Succes;
        }

        public bool IsConnected()
        {
            return _socket.Connected;
        }

        public EBattlEyeConnectionResult Connect()
        {
            try
            {
                KeepRunning = true;
                IPAddress ipAddress = IPAddress.Parse(_loginCredentials.Host);
                EndPoint remoteEP = new IPEndPoint(ipAddress, _loginCredentials.Port);

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                OnMessageReceived(string.Format("Connecting to {0}:{1}... ", _loginCredentials.Host, _loginCredentials.Port));

                try
                {
                    _socket.Connect(remoteEP);

                    OnMessageReceived("Connected!");

                    OnMessageReceived("Logging in... ");

                    SendCommand(Helpers.HexToAscii("FF00") + _loginCredentials.Password);
                    new Thread(DoWork).Start();
                    new Thread(KeepAlive).Start();
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
            return EBattlEyeConnectionResult.Succes;
        }

        public void Disconnect()
        {
            OnMessageReceived("Disconnecting...");
            KeepRunning = false;
            if (_socket.Connected)
                _socket.DisconnectAsync(new SocketAsyncEventArgs());
        }

        private void DoWork()
        {
            var bytesReceived = new Byte[4096];
            int bytes = 0;
            bool saveString = false;

            string buffer = null;
            int bufferCount = 0;
            int packetCount = 0;

            while (_socket.Connected && KeepRunning)
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
                        OnMessageReceived("Login failed!");
                        Disconnect();
                    }
                }
                else if (bytesReceived[7] == 0x02)
                {
                    SendCommand(Helpers.HexToAscii("FF02") + Encoding.Default.GetString(new[] { bytesReceived[8] }));
                    OnMessageReceived(Encoding.Default.GetString(bytesReceived, 9, bytes - 9));
                }
                else if (bytesReceived[7] == 0x01)
                {
                    if (bytesReceived[7] == 0x01 && bytesReceived[9] == 0x00)
                    {
                        if (bytes > 9)
                        {
                            if (bytesReceived[11] == 0)
                            {
                                packetCount = bytesReceived[10];
                            }

                            if (bufferCount < packetCount)
                            {
                                buffer += Encoding.Default.GetString(bytesReceived, 12, bytes - 12);
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
                            // Response from server to Keep Alive packet which is of no use. :)
                        }
                    }
                    else
                    {
                        OnMessageReceived(Encoding.Default.GetString(bytesReceived, 9, bytes - 9));
                    }
                }

                bytesReceived = new Byte[4096];
            }
            if (!_socket.Connected)
                OnDisconnect(_loginCredentials);
        }

        private void KeepAlive()
        {
            while (_socket.Connected && KeepRunning)
            {
                TimeSpan timeout = DateTime.Now - commandSend;

                if (timeout.Seconds >= 30)
                {
                    SendCommand(Helpers.HexToAscii("FF01") + Encoding.Default.GetString(new byte[] { 0 }));
                }

                Thread.Sleep(1000);
            }
        }

        public event BattlEyeMessageEventHandler MessageReceivedEvent;
        public event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}


