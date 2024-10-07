using System;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using JetBrains.Annotations;

namespace GameSwitcher;

#pragma warning disable CA1812 // ASF uses this class during runtime
[UsedImplicitly]
internal sealed class GameSwitcher : IGitHubPluginUpdates {
	public string Name => nameof(GameSwitcher);
	public string RepositoryName => "RainBowCreation/ASF_GameSwitcher";
	public Version Version => typeof(GameSwitcher).Assembly.GetName().Version ?? throw new InvalidOperationException(nameof(Version));

	public Task OnLoaded() {
		string[] header =  {"#####################################################################################",							"#  _____       _       ____                 _____                _   _              #",							"# |  __ \\     (_)     |  _ \\               / ____|              | | (_)             #",							"# | |__) |__ _ _ _ __ | |_) | _____      _| |     _ __ ___  __ _| |_ _  ___  _ __   #",							"# |  _  // _` | | '_ \\|  _ < / _ \\ \\ /\\ / / |    | '__/ _ \\/ _` | __| |/ _ \\| '_ \\  #",							"# | | \\ \\ (_| | | | | | |_) | (_) \\ V  V /| |____| | |  __/ (_| | |_| | (_) | | | | #",							"# |_|  \\_\\__,_|_|_| |_|____/ \\___/ \\_/\\_/  \\_____|_|  \\___|\\__,_|\\__|_|\\___/|_| |_| #",
							"#                                                                                   #",
							"##############################################################GameSwitcher 1.0.0#####"};
		foreach (string line in header) {
			ASF.ArchiLogger.LogGenericInfo($"{line}");
		}

		return Task.CompletedTask;
	}
}
#pragma warning restore CA1812 // ASF uses this class during runtime
