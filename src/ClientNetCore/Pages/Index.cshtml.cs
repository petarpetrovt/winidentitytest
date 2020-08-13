using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Server.Common;

namespace ClientNetCore.Pages
{
	public class IndexModel : PageModel
	{
		private string ServiceUrlTCP { get; }

		private string ServiceUrlIPC { get; }

		public string ServerThreadIdentityTCP { get; set; }

		public string ServerWindowsIdentityTCP { get; set; }

		public string ServerThreadIdentityIPC { get; set; }

		public string ServerWindowsIdentityIPC { get; set; }

		public string ServerThreadIdentityTCPIM { get; set; }

		public string ServerWindowsIdentityTCPIM { get; set; }

		public string ServerThreadIdentityIPCIM { get; set; }

		public string ServerWindowsIdentityIPCIM { get; set; }

		public string Errors { get; set; }

		public IndexModel(IOptions<ServerOptions> options)
		{
			if (options is null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			ServiceUrlTCP = options?.Value.TCP;
			ServiceUrlIPC = options?.Value.IPC;
		}

		public void OnGet()
		{
			var sbErrors = new StringBuilder();

			try
			{
				IService service = GetServiceTCP(ServiceUrlTCP);

				ServerThreadIdentityTCP = service.GetThreadIdentity();
				ServerWindowsIdentityTCP = service.GetWindowsIdentity(false);
			}
			catch (Exception ex)
			{
				sbErrors.AppendLine(ex.ToString());
				sbErrors.AppendLine();

				ServerThreadIdentityTCP = "Error";
				ServerWindowsIdentityTCP = "Error";
			}

			try
			{
				//IService service = GetServiceIPC(ServiceUrlIPC);

				//ServerThreadIdentityIPC = service.GetThreadIdentity();
				//ServerWindowsIdentityIPC = service.GetWindowsIdentity(false);
			}
			catch (Exception ex)
			{
				sbErrors.AppendLine(ex.ToString());
				sbErrors.AppendLine();

				ServerThreadIdentityIPC = "Error";
				ServerWindowsIdentityIPC = "Error";
			}

			if (User.Identity is WindowsIdentity identity)
			{
				using (identity.Impersonate())
				{
					try
					{
						IService service = GetServiceTCP(ServiceUrlTCP);

						ServerThreadIdentityTCPIM = service.GetThreadIdentity();
						ServerWindowsIdentityTCPIM = service.GetWindowsIdentity(false);
					}
					catch (Exception ex)
					{
						sbErrors.AppendLine(ex.ToString());
						sbErrors.AppendLine();

						ServerThreadIdentityTCPIM = "Error";
						ServerWindowsIdentityTCPIM = "Error";
					}

					try
					{
						//IService service = GetServiceIPC(ServiceUrlIPC);

						//ServerThreadIdentityIPCIM = service.GetThreadIdentity();
						//ServerWindowsIdentityIPCIM = service.GetWindowsIdentity(false);
					}
					catch (Exception ex)
					{
						sbErrors.AppendLine(ex.ToString());
						sbErrors.AppendLine();

						ServerThreadIdentityIPCIM = "Error";
						ServerWindowsIdentityIPCIM = "Error";
					}
				}
			}

			Errors = sbErrors.ToString();
		}

		private static IService GetServiceTCP(string url)
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

			var service = (IService)RemotingServices.Connect(typeof(IService), url);

			service.Ping();

			return service;
		}

		private static IService GetServiceIPC(string url)
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

			var service = (IService)RemotingServices.Connect(typeof(IService), url);

			service.Ping();

			return service;
		}
	}
}
