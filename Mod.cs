using Colossal.Logging;
using Game;
using Game.Modding;
using Game.Tools;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace LineColorRandomizer
{

    public class Mod : IMod
    {
        public static Color32 generateRandomColor() {
            Color32 color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            log.Info($"Set RoadTool color to {color}");
            return color;
        }

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
    class ChangeColorOnInitialize
    {
        static void Prefix(ref RouteToolSystem __instance)
        {
            __instance.color = Mod.generateRandomColor();
        }
    }

    [HarmonyPatch(typeof(RouteToolSystem), "Apply")]
    class ChangeColorOnNewLine
    {

        static void Prefix(ref RouteToolSystem __instance, out bool __state)
        {
            __state = (RouteToolSystem.State) Traverse.Create(__instance).Field("m_State").GetValue() == RouteToolSystem.State.Create;
        }

        static void Postfix(ref RouteToolSystem __instance, bool __state)
        {
            if (__state) {
                if (__state = (RouteToolSystem.State) Traverse.Create(__instance).Field("m_State").GetValue() == RouteToolSystem.State.Default) {
                    __instance.color = Mod.generateRandomColor();
                }
            }
        }
    }
}