using using ArchiSteamFarm.Core;

namespace GameSwitcher.Utils;

public class AppIDConverter {
    public static IReadOnlyCollection<uint> ConvertStringToAppID(string appID) {
        uint appIDint = 0;

        foreach (string appID in appIDStrings) {
            if (uint.TryParse(appID, out uint parsedAppID)) {
                appIDint = parsedAppID;
            } else {
                ASF.LogGenericError($"Invalid AppID: {appID}");
                appIDint = 0;
            }
        }

        return appIDint.AsReadOnly();
    }
}
