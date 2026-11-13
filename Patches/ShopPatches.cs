using System;
using HarmonyLib;

namespace OneHP.Patches;

// Shop stock is chosen on the host only (GetAllItemsFromStatsManager returns early
// on other clients), so removing candidates here applies to every player.
[HarmonyPatch]
public static class ShopPatches
{
    private const string HealthUpgradeItemName = "Item Upgrade Player Health";

    [HarmonyPatch(typeof(ShopManager), "GetAllItemsFromStatsManager")]
    [HarmonyPostfix]
    private static void GetAllItemsFromStatsManager_Postfix(ShopManager __instance)
    {
        try
        {
            if (!Plugin.Enabled.Value) return;
            if (!SemiFunc.IsMasterClientOrSingleplayer()) return;

            int packs = 0;
            if (Plugin.RemoveHealthPacksFromShop.Value)
            {
                packs = __instance.potentialItemHealthPacks.Count;
                __instance.potentialItemHealthPacks.Clear();
            }

            int upgrades = 0;
            if (Plugin.RemoveHealthUpgradesFromShop.Value)
            {
                upgrades = __instance.potentialItemUpgrades.RemoveAll(
                    item => item != null && item.name == HealthUpgradeItemName);
            }

            Plugin.Logger.LogInfo($"Shop: removed {packs} health pack(s), {upgrades} health upgrade(s)");
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"OneHP shop error: {e}");
        }
    }
}
