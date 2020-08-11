using System;
using System.Collections.Specialized;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Serialization.Formatters;
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
			ConfigureRemotingServices();

			if (args.Length < 2)
			{
				throw new Exception($"Missing TCP and IPC ports.\n\nUse: Server.exe 3331 MyIPCPORT\n\n");
			}

			if (!int.TryParse(args[0], out int port))
			{
				throw new Exception($"Invalid TCP port `{args[0]}`.");
			}

			RegisterTCPChannel(port, true);
			RegisterIPCChannel(args[1], true);

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

		private static void ConfigureRemotingServices()
		{
			RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;

			// The initial TTL after an object's creation.
			LifetimeServices.LeaseTime = TimeSpan.Parse("00:05:00");
			// The grace time for a method call that is placed on the object. These times are not additive. Calling a method 10 times will not increase the TTL to 20 minutes.
			LifetimeServices.LeaseManagerPollTime = TimeSpan.Parse("00:02:00");
			// If a sponsor is registered for a lease, they will be contacted when the TTL expires. They then contact the LeaseManager to request additional time for the
			// sponsored object. When the sponsor does not react during this time, the lease will expire and the object will be marked for garbage collection.
			LifetimeServices.RenewOnCallTime = TimeSpan.Parse("00:02:00");
			// The interval the TTL of remoted objects is being checked. The initial value for this interval is 10 seconds.
			LifetimeServices.SponsorshipTimeout = TimeSpan.Parse("00:00:10");
		}

		private static void RegisterTCPChannel(int port, bool isSecure)
		{
			var serverSinkProviderChain = new BinaryServerFormatterSinkProvider
			{
				TypeFilterLevel = TypeFilterLevel.Full,
			};

			var channelProperties = new ListDictionary
			{
				{ "name", $"TCP://:{port}" },
				{ "port", port },
				//{ "bindTo", null },
				//{ "priority", 1 },
				{ "secure", isSecure },
				//{ "impersonate", false },
				//{ "protectionLevel", ProtectionLevel.EncryptAndSign },
				//{ "machineName", null },
				//{ "rejectRemoteRequests", false },
				//{ "suppressChannelData", false },
				//{ "useIpAddress", true },
				{ "exclusiveAddressUse", true },
				//{ "authorizationModule", null },
			};

			// Impersonate or TokenImpersonationLevel settings are only valid when secure="true"
			if (isSecure)
			{
				// TODO:
				//channelProperties["impersonate"] = endpointOptions.Impersonate;
			}

			var channel = new TcpServerChannel(channelProperties, serverSinkProviderChain);

			ChannelServices.RegisterChannel(channel, false);

			Console.WriteLine($"Channel `{channelProperties["name"]}` listening on `{channelProperties["port"]}`.");
		}

		private static void RegisterIPCChannel(string portName, bool isSecure)
		{
			var serverSinkProviderChain = new BinaryServerFormatterSinkProvider
			{
				TypeFilterLevel = TypeFilterLevel.Full,
			};

			var channelProperties = new ListDictionary
			{
				{ "name", $"IPC://{portName}" },
				{ "secure", isSecure },
				{ "portName", portName },
				{ "authorizedGroup", "Everyone" },
			};

			var channel = new IpcServerChannel(channelProperties, serverSinkProviderChain);

			ChannelServices.RegisterChannel(channel, false);

			Console.WriteLine($"Channel `{channelProperties["name"]}` listening on `{channelProperties["portName"]}`.");
		}
	}
}
