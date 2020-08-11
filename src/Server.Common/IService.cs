namespace Server.Common
{
	public interface IService
	{
		void Ping();

		string GetThreadIdentity();

		string GetWindowsIdentity(bool ifImpersonating);
	}
}
