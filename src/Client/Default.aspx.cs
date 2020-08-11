using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Principal;
using System.Threading;
using System.Web.UI;
using Server.Common;

namespace Client
{
	public partial class _Default : Page
	{
		private const string ServiceUrlTCP = "tcp://localhost:3331/Service";
		private const string ServiceUrlIPC = "ipc://TEST/Service";

		public string ServerThreadIdentityTCP { get; set; }

		public string ServerWindowsIdentityTCP { get; set; }

		public string ServerThreadIdentityIPC { get; set; }

		public string ServerWindowsIdentityIPC { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			IService service = GetServiceTCP();

			ServerThreadIdentityTCP = service.GetThreadIdentity();
			ServerWindowsIdentityTCP = service.GetWindowsIdentity(false);

			service = GetServiceIPC();

			ServerThreadIdentityIPC = service.GetThreadIdentity();
			ServerWindowsIdentityIPC = service.GetWindowsIdentity(false);

			if (Thread.CurrentPrincipal.Identity is WindowsIdentity identity)
			{
				using (identity.Impersonate())
				{
					service = GetServiceTCP();

					ServerThreadIdentityTCP = service.GetThreadIdentity();
					ServerWindowsIdentityTCP = service.GetWindowsIdentity(false);

					service = GetServiceIPC();

					ServerThreadIdentityIPC = service.GetThreadIdentity();
					ServerWindowsIdentityIPC = service.GetWindowsIdentity(false);
				}
			}
		}

		private static IService GetServiceTCP()
		{
			var properties = new Hashtable
			{
				// (Client and server) A string representing the name of the channel.
				{ "name", $"TCP_CLIENT_{Guid.NewGuid()}" },
				//{ "priority", 1 },
				{ "secure", true },
			};

			var clientSinkProvider = new BinaryClientFormatterSinkProvider
			{
			};

			var channel = new TcpClientChannel(properties, clientSinkProvider);

			ChannelServices.RegisterChannel(channel, ensureSecurity: false);

			var service = (IService)RemotingServices.Connect(typeof(IService), ServiceUrlTCP);

			service.Ping();

			return service;
		}

		private static IService GetServiceIPC()
		{
			var properties = new Hashtable
			{
				// (Client and server) A string representing the name of the channel.
				{ "name", $"IPC_CLIENT_{Guid.NewGuid()}" },
				//{ "priority", 1 },
				{ "secure", true },
			};

			var clientSinkProvider = new BinaryClientFormatterSinkProvider
			{
			};

			var channel = new IpcClientChannel(properties, clientSinkProvider);

			ChannelServices.RegisterChannel(channel, ensureSecurity: false);

			var service = (IService)RemotingServices.Connect(typeof(IService), ServiceUrlIPC);

			service.Ping();

			return service;
		}
	}
}