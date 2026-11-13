using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace OneHP;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string PluginGuid = "maxenterme.OneHP";
    private const string PluginName = "OneHP";
    private const string PluginVersion = "1.1.0";

    internal static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;

    internal static ConfigEntry<bool> Enabled = null!;
    internal static ConfigEntry<int> MaxHealth = null!;
    internal static ConfigEntry<bool> RemoveHealthPacksFromShop = null!;
    internal static ConfigEntry<bool> RemoveHealthUpgradesFromShop = null!;

    private void Awake()
    {
        Instance = this;

        Enabled = Config.Bind(
            "General", "Enabled", true,
            "Force every player's max health (host only; applies to all players)."
        );
        MaxHealth = Config.Bind(
            "General", "MaxHealth", 1,
            new ConfigDescription("Max health forced on every player", new AcceptableValueRange<int>(1, 100))
        );

        RemoveHealthPacksFromShop = Config.Bind(
            "Shop", "RemoveHealthPacks", true,
            "Do not stock health packs in the shop (host only; applies to all players)."
        );
        RemoveHealthUpgradesFromShop = Config.Bind(
            "Shop", "RemoveHealthUpgrades", true,
            "Do not stock health upgrades in the shop (host only; applies to all players)."
        );

        new Harmony(PluginGuid).PatchAll(typeof(Plugin).Assembly);
        Logger.LogInfo($"{PluginName} v{PluginVersion} loaded!");
    }
}
