using System;
using System.Runtime.Remoting;
using System.Threading;

namespace Server
{
	public class Program
	{
		private const string ServicePath = "Service";

		private static RemotingService _service;
		private static ObjRef _serviceRef;

		public static void Main(string[] args)
		{
			RemotingConfiguration.Configure("Server.exe.config", false);

			_service = new RemotingService();
			_serviceRef = RemotingServices.Marshal(_service, ServicePath);

			Console.WriteLine($"Marshalling service path `/{ServicePath}`.");
			Console.WriteLine();
			Console.WriteLine($"Press any key to exit...");

			while (!Console.KeyAvailable)
			{
				Thread.Sleep(10);
			}

			if (_serviceRef != null)
			{
				RemotingServices.Unmarshal(_serviceRef);
			}
		}
	}
}
