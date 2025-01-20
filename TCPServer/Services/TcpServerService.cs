using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TCPServer.Services
{
	public class TcpServerService
	{
		private readonly IEncryptionService _encryptionService;
		private readonly Dictionary<string, List<Dictionary<string, int>>> _serverData;

		public TcpServerService(IEncryptionService encryptionService, Dictionary<string, List<Dictionary<string, int>>> serverData)
		{
			_encryptionService = encryptionService;
			_serverData = serverData;
		}

		public async Task StartServerAsync(string ipAddress, int port)
		{
			TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
			listener.Start();
			Console.WriteLine("Server is listening...");

			while (true)
			{
				TcpClient client = await listener.AcceptTcpClientAsync();
				Console.WriteLine("Client connected.");
				_ = HandleClientAsync(client);
			}
		}

		private async Task HandleClientAsync(TcpClient client)
		{
			try
			{
				NetworkStream stream = client.GetStream();
				byte[] buffer = new byte[client.ReceiveBufferSize];
				int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
				string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
				string message = _encryptionService.Decrypt(encryptedMessage);

				Console.WriteLine($"Received: {message}");

				var parts = message.Split('-');
				if (parts.Length == 2 && _serverData.ContainsKey(parts[0]))
				{
					var setData = _serverData[parts[0]];
					var subset = setData[0]; 
					if (subset.ContainsKey(parts[1]))
					{
						int timesToSend = subset[parts[1]];
						for (int i = 0; i < timesToSend; i++)
						{
							string currentTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
							string encryptedResponse = _encryptionService.Encrypt(currentTime);
							byte[] responseBytes = Encoding.UTF8.GetBytes(encryptedResponse);
							await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
							await Task.Delay(1000);
						}
					}
					else
					{
						await SendMessageAsync(stream, "EMPTY");
					}
				}
				else
				{
					await SendMessageAsync(stream, "EMPTY");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error handling client: {ex.Message}");
			}
			finally
			{
				client.Close();
			}
		}

		private async Task SendMessageAsync(NetworkStream stream, string message)
		{
			string encryptedMessage = _encryptionService.Encrypt(message);
			byte[] responseBytes = Encoding.UTF8.GetBytes(encryptedMessage);
			await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
		}
	}
}
