using System.Net.Sockets;
using System.Text;

namespace Unlocker
{
    public class TorControl
    {
        private readonly string _address;
        private readonly int _port;
        private readonly string _password;

        public TorControl(string address, int port, string password)
        {
            _address = address;
            _port = port;
            _password = password;
        }

        public async Task SendCommandAsync(string command)
        {
            using var client = new TcpClient(_address, _port);
            using var stream = client.GetStream();
            string authCommand = $"AUTHENTICATE \"{_password}\"\r\n";
            byte[] authBuffer = Encoding.ASCII.GetBytes(authCommand);
            await stream.WriteAsync(authBuffer);

            byte[] commandBuffer = Encoding.ASCII.GetBytes($"{command}\r\n");
            await stream.WriteAsync(commandBuffer);
        }

        public async Task SignalNewNymAsync()
        {
            await SendCommandAsync("SIGNAL NEWNYM");
        }
    }
}