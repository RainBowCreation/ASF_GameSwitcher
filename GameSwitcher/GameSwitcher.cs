using System;
using System.IO;
using System.Threading.Tasks;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Interaction;
using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using JetBrains.Annotations;
using System.Collections.Generic;
using GameSwitcher.Utils;

namespace GameSwitcher;

#pragma warning disable CA1812 // ASF uses this class during runtime
[UsedImplicitly]
internal sealed class GameSwitcher : IGitHubPluginUpdates
{
	public string Name => nameof(GameSwitcher);
	public string RepositoryName => "RainBowCreation/ASF_GameSwitcher";
	public Version Version => typeof(GameSwitcher).Assembly.GetName().Version ?? throw new InvalidOperationException(nameof(Version));

	// Configuration settings
	private static string BotName { get; set; } = string.Empty;
	private static string FilePath { get; set; } = string.Empty;
	private static int Minutes { get; set; }
	private static bool Increament { get; set; }

	public Task OnLoaded()
	{
		string[] header =  {"#####################################################################################",                            "#  _____       _       ____                 _____                _   _              #",                            "# |  __ \\     (_)     |  _ \\               / ____|              | | (_)             #",                          "# | |__) |__ _ _ _ __ | |_) | _____      _| |     _ __ ___  __ _| |_ _  ___  _ __   #",                            "# |  _  // _` | | '_ \\|  _ < / _ \\ \\ /\\ / / |    | '__/ _ \\/ _` | __| |/ _ \\| '_ \\  #",                         "# | | \\ \\ (_| | | | | | |_) | (_) \\ V  V /| |____| | |  __/ (_| | |_| | (_) | | | | #",                         "# |_|  \\_\\__,_|_|_| |_|____/ \\___/ \\_/\\_/  \\_____|_|  \\___|\\__,_|\\__|_|\\___/|_| |_| #",
							"#                                                                                   #",
							$"##############################################################${Name} {Version}#####"};

		foreach (string line in header)
		{
			ASF.ArchiLogger.LogGenericInfo($"{line}");
		}

		LoadConfiguration();

		if (string.IsNullOrEmpty(BotName))
		{
			ASF.ArchiLogger.LogGenericWarning("Bot name is missing from the configuration. Please specify it.");
			return Task.CompletedTask;
		}

		if (string.IsNullOrEmpty(FilePath))
		{
			FilePath = "app_ids.txt"; // Default to "app_ids.txt" if not specified
		}

		List<string>? appIds = ReadAppIds(FilePath);
		if (appIds == null || appIds.Count == 0)
		{
			ASF.ArchiLogger.LogGenericWarning("No AppIDs found. Exiting.");
			return Task.CompletedTask;
		}

		Task.Run(() => GameSwitcherTask());
		return Task.CompletedTask;
	}

	private static void LoadConfiguration()
	{
		// Load configuration values
		BotName = "YourBotName";
		FilePath = "app_ids.txt";
		Minutes = 1;
		Increament = false;
	}

	private static async Task GameSwitcherTask()
	{
		if (Minutes <= 0)
		{
			ASF.ArchiLogger.LogGenericWarning("Invalid Minutes config. Exiting.");
			return;
		}

		if (string.IsNullOrEmpty(BotName))
		{
			ASF.ArchiLogger.LogGenericWarning("Invalid Bot name. Exiting.");
			return;
		}

		Bot? bot = Bot.GetBot(BotName);
		if (bot == null)
		{
			ASF.ArchiLogger.LogGenericWarning("Invalid Bot name. Exiting.");
			return;
		}

		List<string>? appIds = ReadAppIds(FilePath);
		if (appIds == null || appIds.Count == 0)
		{
			ASF.ArchiLogger.LogGenericWarning("No AppIDs found. Exiting.");
			return;
		}

		foreach (string appId in appIds)
		{
			// Ensure you handle the possibility of a null return from the conversion method
			IReadOnlyCollection<uint>? uAppID = AppIDConverter.ConvertStringToAppIDs(appId);
			if (uAppID == null || uAppID.Count == 0) // Updated to check for null
			{
				ASF.ArchiLogger.LogGenericWarning("Error parsing appID. Skipping.");
				continue;
			}

			ASF.ArchiLogger.LogGenericInfo($"Playing game with AppID: {appId}");
			await bot.Actions.Play(uAppID).ConfigureAwait(false);
			await Task.Delay(TimeSpan.FromMinutes(Minutes)).ConfigureAwait(false);

			ASF.ArchiLogger.LogGenericInfo($"Stopping game with AppID: {appId}");
			await bot.Actions.Pause(true).ConfigureAwait(false);
			await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
		}
	}

	private static List<string>? ReadAppIds(string filePath)
	{
		try
		{
			// Assuming this part is unchanged, as it correctly handles reading the lines
			List<string> appIds = new List<string>(File.ReadAllLines(filePath));
			appIds.RemoveAll(string.IsNullOrWhiteSpace); // Remove empty lines
			return appIds.Count > 0 ? appIds : null; // Explicitly return null if empty
		}
		catch (Exception ex)
		{
			ASF.ArchiLogger.LogGenericWarning($"Error reading file: {ex.Message}");
			return null;
		}
	}
}
#pragma warning restore CA1812 // ASF uses this class during runtime
