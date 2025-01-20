using System.Net.Sockets;
using System.Text;

namespace TCPClient.Services
{
	public class TcpClientService
	{
		private readonly IEncryptionService _encryptionService;
		private readonly string _serverIp;
		private readonly int _serverPort;

		public TcpClientService(IEncryptionService encryptionService, string serverIp, int serverPort)
		{
			_encryptionService = encryptionService;
			_serverIp = serverIp;
			_serverPort = serverPort;
		}

		public async Task CommunicateAsync(string message)
		{
			string encryptedMessage = _encryptionService.Encrypt(message);

			try
			{
				using (TcpClient client = new TcpClient(_serverIp, _serverPort))
				{
					NetworkStream stream = client.GetStream();

					byte[] buffer = Encoding.UTF8.GetBytes(encryptedMessage);
					await stream.WriteAsync(buffer, 0, buffer.Length);
					while (true)
					{
						byte[] responseBuffer = new byte[client.ReceiveBufferSize];
						int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);

						if (bytesRead > 0)
						{
							string encryptedResponse = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
							string response = _encryptionService.Decrypt(encryptedResponse);
							Console.WriteLine($"{response}");
						}
						else
						{
							return;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Client error: {ex.Message}");
			}
		}
	}
}
