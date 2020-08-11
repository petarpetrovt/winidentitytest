using System;
using System.Security.Principal;
using System.Threading;
using Server.Common;

namespace Server
{
	internal class RemotingService : MarshalByRefObject, IService
	{
		#region IService Members

		public void Ping() { }

		public string GetThreadIdentity()
			=> Thread.CurrentPrincipal?.Identity?.Name;

		public string GetWindowsIdentity(bool ifImpersonating)
			=> WindowsIdentity.GetCurrent(ifImpersonating)?.Name;

		#endregion

		#region MarshalByRefObject Members

		public override object InitializeLifetimeService()
			=> null;

		#endregion
	}
}
