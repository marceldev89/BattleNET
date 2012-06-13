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
                hash += 0;
                hash = Helpers.HexString2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + command;
                _socket.Send(Encoding.Default.GetBytes(packet));
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

                    SendCommand(Helpers.HexString2Ascii("FF") + Helpers.HexString2Ascii("00") + _loginCredentials.Password);
                    new Thread(DoWork).Start();
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
            var bytesReceived = new Byte[256];
            int bytes = 0;

            while (_socket.Connected && KeepRunning)
            {
                bytes = _socket.Receive(bytesReceived, bytesReceived.Length, 0);

                if (bytesReceived[7] == 0x00 && bytesReceived[8] == 0x01)
                {
                    OnMessageReceived("Logged in!");
                }
                else if (bytesReceived[7] == 0x02)
                {
                    SendCommand(Helpers.HexString2Ascii("FF") + Helpers.HexString2Ascii("02") +
                         Encoding.Default.GetString(new[] { bytesReceived[8] }));
                    OnMessageReceived(Encoding.Default.GetString(bytesReceived, 9, bytes));
                    bytesReceived = new Byte[256];
                }
            }
            if (!_socket.Connected)
                OnDisconnect(_loginCredentials);
        }

        public event BattlEyeMessageEventHandler MessageReceivedEvent;
        public event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}


