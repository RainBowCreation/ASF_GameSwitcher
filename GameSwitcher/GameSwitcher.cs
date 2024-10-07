using System;
using System.IO;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace GameSwitcher;

#pragma warning disable CA1812 // ASF uses this class during runtime
[UsedImplicitly]
internal sealed class GameSwitcher : IGitHubPluginUpdates
{
	public string Name => nameof(GameSwitcher);
	public string RepositoryName => "RainBowCreation/ASF_GameSwitcher";
	public Version Version => typeof(GameSwitcher).Assembly.GetName().Version ?? throw new InvalidOperationException(nameof(Version));

	// Configuration settings
	private static string BotName { get; set; }
	private static string FilePath { get; set; }
	private static string Minutes { get; set; }
	private static string Increament { get; set; }

	public Task OnLoaded()
	{
		string[] header =  {"#####################################################################################",                            "#  _____       _       ____                 _____                _   _              #",                            "# |  __ \\     (_)     |  _ \\               / ____|              | | (_)             #",                          "# | |__) |__ _ _ _ __ | |_) | _____      _| |     _ __ ___  __ _| |_ _  ___  _ __   #",                            "# |  _  // _` | | '_ \\|  _ < / _ \\ \\ /\\ / / |    | '__/ _ \\/ _` | __| |/ _ \\| '_ \\  #",                         "# | | \\ \\ (_| | | | | | |_) | (_) \\ V  V /| |____| | |  __/ (_| | |_| | (_) | | | | #",                         "# |_|  \\_\\__,_|_|_| |_|____/ \\___/ \\_/\\_/  \\_____|_|  \\___|\\__,_|\\__|_|\\___/|_| |_| #",
							"#                                                                                   #",
							$"##############################################################${Name} {Version}#####"};
		foreach (string line in header)
		{
			ASF.ArchiLogger.LogGenericInfo($"{line}");
		}
		// Load configuration
		LoadConfiguration();
		// Check if BotName is specified in the configuration
		if (string.IsNullOrEmpty(BotName))
		{
			ASF.ArchiLogger.LogGenericWarning("Bot name is missing from the configuration. Please specify it.");
			return Task.CompletedTask;;
		}

		if (string.IsNullOrEmpty(FilePath))
		{
			FilePath = "app_ids.txt"; // Default to "app_ids.txt" if not specified
		}

		// Load AppIDs from the file
		List<string> appIds = ReadAppIds(FilePath);

		if (appIds == null || appIds.Count == 0)
		{
			ASF.ArchiLogger.LogGenericWarning("No AppIDs found. Exiting.");
			return Task.CompletedTask;;
		}

		Task.Run(() => GameSwitcherTask());
		return Task.CompletedTask;
	}

	private void LoadConfiguration()
	{
		// Example config values (hardcoded for demonstration purposes)
		BotName = "YourBotName";  // Replace with actual bot name from config
		FilePath = "app_ids.txt"; // Replace with actual path to the app IDs file from config
		Minutes = 1; // Replace with actual path to the app IDs file from config
		Increament = false; // Replace with actual path to the app IDs file from config
	}

	private async Task GameSwitcherTask()
	{
		if (string.IsNullOrEmpty(FilePath))
		{
			FilePath = "app_ids.txt"; // Default to "app_ids.txt" if not specified
		}

		// Load AppIDs from the file
		List<string> appIds = ReadAppIds(FilePath);

		if (appIds == null || appIds.Count == 0)
		{
			ASF.ArchiLogger.LogGenericWarning("No AppIDs found. Exiting.");
			return;
		}

		if (Minutes <= 0)
		{
			ASF.ArchiLogger.LogGenericWarning("Invalid Minutes config. Exiting.");
		}

		// Iterate over each AppID and send play/stop commands
		foreach (string appId in appIds)
		{
			ASF.ArchiLogger.LogGenericInfo($"Playing game with AppID: {appId}");
			await SendCommandToASF(BotName, $"!play {appId}");
			await Task.Delay(TimeSpan.FromMinutes(Minutes)); // Wait for 1 minute

			ASF.ArchiLogger.LogGenericInfo($"Stopping game with AppID: {appId}");
			await SendCommandToASF(BotName, "!stop");
			await Task.Delay(TimeSpan.FromSeconds(5)); // Short pause before starting next game
		}
	}

	private static List<string> ReadAppIds(string filePath)
	{
		try
		{
			List<string> appIds = new List<string>(File.ReadAllLines(filePath));
			appIds.RemoveAll(string.IsNullOrWhiteSpace); // Remove empty lines
			return appIds;
		}
		catch (Exception ex)
		{
			ASF.ArchiLogger.LogGenericWarning($"Error reading file: {ex.Message}");
			return null;
		}
	}

	private async Task SendCommandToASF(string botName, string command)
	{
		Bot bot = Bot.GetBot(botName);
		if (bot == null)
		{
			ASF.ArchiLogger.LogGenericWarning($"Bot '{botName}' not found.");
			return;
		}

		// Send the command to ASF and await the response
		string response = await bot.Commands.HandleMessage(command, bot.BotName);
		if (!string.IsNullOrEmpty(response))
		{
			ASF.ArchiLogger.LogGenericInfo($"Response from ASF: {response}");
		}
	}
}
#pragma warning restore CA1812 // ASF uses this class during runtime
