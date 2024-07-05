using Colossal.Logging;
using Game;
using Game.Modding;
using Game.Tools;
using HarmonyLib;
using UnityEngine;

namespace LineColorRandomizer
{

    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger(nameof(LineColorRandomizer))
            .SetShowsErrorsInUI(true);
        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info("Plugin ColorRandomizer is loading!");

            var harmony = new Harmony("eu.sprtovo.cs2.color");
            harmony.PatchAll();

            foreach (var patchedMethod in harmony.GetPatchedMethods()) {
                log.Info($"Patched method: {patchedMethod.Module.Name}:{patchedMethod.Name}");
            }
            log.Info("Plugin ColorRandomizer is fully loaded!");
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
    
    [HarmonyPatch(typeof(RouteToolSystem), nameof(RouteToolSystem.TrySetPrefab))]
    class RandomColorPatch
    {
        static void Prefix(ref RouteToolSystem __instance)
        {
            Color32 color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            __instance.color = color;
            Mod.log.Info($"Set RoadTool color to {color}");
        }
    }
}