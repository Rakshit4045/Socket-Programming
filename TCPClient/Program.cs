using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TCPClient.Services;

namespace TCPClient
{
	public class Program
	{
		static async Task Main(string[] args)
		{
			// Build the configuration from appsettings.json
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			// Retrieve values from the configuration file
			var encryptionKey = configuration["AppSettings:EncryptionKey"];
			var encryptionIV = configuration["AppSettings:EncryptionIV"];
			var ipAddress = configuration["AppSettings:ServerIPAddress"];
			var port = int.Parse(configuration["AppSettings:ServerPort"]);

			var serviceProvider = new ServiceCollection()
				.AddSingleton<IEncryptionService>(new AESHelper(encryptionKey, encryptionIV))
				.BuildServiceProvider();

			var encryptionService = serviceProvider.GetService<IEncryptionService>();
			var clientService = new TcpClientService(encryptionService, ipAddress, port);

			while (true)
			{
				Console.Write("Enter a message (e.g., SetA-Two): ");
				string message = Console.ReadLine();

				if (message?.ToLower() == "exit") break;

				await clientService.CommunicateAsync(message);
			}
		}
	}
}
