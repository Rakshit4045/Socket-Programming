using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TCPServer.Services;

namespace TCPServer
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var serverData = new Dictionary<string, List<Dictionary<string, int>>>
				{
					{ "SetA", new List<Dictionary<string, int>> { new Dictionary<string, int> { { "One", 1 }, { "Two", 2 } } } },
					{ "SetB", new List<Dictionary<string, int>> { new Dictionary<string, int> { { "Three", 3 }, { "Four", 4 } } } },
					{ "SetC", new List<Dictionary<string, int>> { new Dictionary<string, int> { { "Five", 5 }, { "Six", 6 } } } },
					{ "SetD", new List<Dictionary<string, int>> { new Dictionary<string, int> { { "Seven", 7 }, { "Eight", 8 } } } },
					{ "SetE", new List<Dictionary<string, int>> { new Dictionary<string, int> { { "Nine", 9 }, { "Ten", 10 } } } }
				};

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

			// Create the service provider
			var serviceProvider = new ServiceCollection()
				.AddSingleton<IEncryptionService>(new AESHelper(encryptionKey, encryptionIV))
				.BuildServiceProvider();

			// Now create TcpServerService using the service provider
			var encryptionService = serviceProvider.GetService<IEncryptionService>();
			var serverService = new TcpServerService(encryptionService, serverData);

			// Start the server
			await serverService.StartServerAsync(ipAddress, port);
		}
	}
}
