using ArchiSteamFarm.Core;
namespace GameSwitcher.Utils;

public static class Log
{
	public static void Info(string message)
	{
		ASF.ArchiLogger.LogGenericInfo(message);
	}
	public static void Warn(string message)
	{
		ASF.ArchiLogger.LogGenericWarning(message);
	}
	public static void Error(string message)
	{
		ASF.ArchiLogger.LogGenericError(message);
	}
}
