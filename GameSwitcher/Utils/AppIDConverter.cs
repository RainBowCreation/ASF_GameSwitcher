
using System;
using System.Collections.Generic;
using ArchiSteamFarm.Core;

namespace GameSwitcher.Utils;

public static class AppIDConverter
{
    // Define static readonly fields for the delimiters
    private static readonly char[] Delimiters = new[] { ',', ' ' };

    public static IReadOnlyCollection<uint> ConvertStringToAppIDs(string appIDs)
    {
        if (appIDs == null)
        {
            throw new ArgumentNullException(nameof(appIDs), "AppIDs cannot be null.");
        }

        // Use the static readonly delimiters instead of creating a new array
        string[] appIDStrings = appIDs.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

        List<uint> appIDList = new List<uint>();

        foreach (string appID in appIDStrings)
        {
            if (uint.TryParse(appID, out uint parsedAppID))
            {
                appIDList.Add(parsedAppID);
            }
            else
            {
                ASF.ArchiLogger.LogGenericWarning($"Invalid AppID: {appID}");
            }
        }

        return appIDList.AsReadOnly();
    }
}
