using System;
using System.Collections;
using System.Configuration;
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
		private static string ServiceUrlTCP => ConfigurationManager.AppSettings["TCP"];
		private static string ServiceUrlIPC => ConfigurationManager.AppSettings["IPC"];

		public string ServerThreadIdentityTCP { get; set; }

		public string ServerWindowsIdentityTCP { get; set; }

		public string ServerThreadIdentityIPC { get; set; }

		public string ServerWindowsIdentityIPC { get; set; }

		public string ServerThreadIdentityTCPIM { get; set; }

		public string ServerWindowsIdentityTCPIM { get; set; }

		public string ServerThreadIdentityIPCIM { get; set; }

		public string ServerWindowsIdentityIPCIM { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				IService service = GetServiceTCP();

				ServerThreadIdentityTCP = service.GetThreadIdentity();
				ServerWindowsIdentityTCP = service.GetWindowsIdentity(false);
			}
			catch (Exception ex)
			{
				ServerThreadIdentityTCP = $"Error";
				ServerWindowsIdentityTCP = "Error";
			}

			try
			{
				IService service = GetServiceIPC();

				ServerThreadIdentityIPC = service.GetThreadIdentity();
				ServerWindowsIdentityIPC = service.GetWindowsIdentity(false);
			}
			catch
			{
				ServerThreadIdentityIPC = "Error";
				ServerWindowsIdentityIPC = "Error";
			}

			if (Thread.CurrentPrincipal.Identity is WindowsIdentity identity)
			{
				using (identity.Impersonate())
				{
					try
					{
						IService service = GetServiceTCP();

						ServerThreadIdentityTCPIM = service.GetThreadIdentity();
						ServerWindowsIdentityTCPIM = service.GetWindowsIdentity(false);
					}
					catch
					{
						ServerThreadIdentityTCPIM = "Error";
						ServerWindowsIdentityTCPIM = "Error";
					}

					try
					{
						IService service = GetServiceIPC();

						ServerThreadIdentityIPCIM = service.GetThreadIdentity();
						ServerWindowsIdentityIPCIM = service.GetWindowsIdentity(false);
					}
					catch
					{
						ServerThreadIdentityIPCIM = "Error";
						ServerWindowsIdentityIPCIM = "Error";
					}
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