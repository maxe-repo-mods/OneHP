using System;
using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace OneHP.Patches;

// Health is owned by each player's own client. The host can still overwrite it:
// UpdateHealthRPC accepts calls from the master client, and on the owner it sets
// both maxHealth and health (and the saved per-player health). Once maxHealth is
// the target, the owner's own Heal() clamps to it, so healing cannot raise HP.
[HarmonyPatch]
public static class HealthPatches
{
    // Minimum seconds between corrections for the same player, so a correction
    // that is still in flight is not resent every frame.
    private const float ResendInterval = 0.5f;

    private static readonly Dictionary<int, float> _lastSent = new();

    [HarmonyPatch(typeof(PlayerHealth), "Update")]
    [HarmonyPostfix]
    private static void PlayerHealth_Update_Postfix(PlayerHealth __instance)
    {
        try
        {
            if (!Plugin.Enabled.Value) return;
            if (__instance.isMenuAvatar) return;
            if (!SemiFunc.IsMasterClientOrSingleplayer()) return;
            if (SemiFunc.IsMainMenu() || SemiFunc.RunIsLobbyMenu()) return;
            if (LevelGenerator.Instance == null || !LevelGenerator.Instance.Generated) return;

            var avatar = __instance.playerAvatar;
            if (avatar == null || avatar.deadSet || avatar.isDisabled) return;
            if (__instance.health <= 0) return;

            int target = Plugin.MaxHealth.Value;
            if (__instance.maxHealth == target && __instance.health <= target) return;

            int id = __instance.GetInstanceID();
            if (_lastSent.TryGetValue(id, out float last) && Time.time - last < ResendInterval) return;
            _lastSent[id] = Time.time;

            int newHealth = Mathf.Min(__instance.health, target);
            if (SemiFunc.IsMultiplayer())
                __instance.photonView.RPC("UpdateHealthRPC", RpcTarget.All, newHealth, target, false, false);
            else
                __instance.UpdateHealthRPC(newHealth, target, false, false);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"OneHP error: {e}");
        }
    }

    [HarmonyPatch(typeof(SemiFunc), nameof(SemiFunc.OnSceneSwitch))]
    [HarmonyPrefix]
    private static void OnSceneSwitch_Prefix()
    {
        _lastSent.Clear();
    }
}
