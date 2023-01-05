using BepInEx;
using MijuTools;
using SpaceCraft;
using HarmonyLib;
using BepInEx.Configuration;
using System.Linq;

namespace CustomProgressSpeed
{
    [BepInPlugin("mikeypdog.theplanetcraftermods.customprogressspeed", "Custom progress speed", "1.0.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private static ConfigEntry<float> oxygenMultiplier;
        private static ConfigEntry<float> energyMultiplier;
        private static ConfigEntry<float> heatMultiplier;
        private static ConfigEntry<float> pressureMultiplier;
        private static ConfigEntry<float> biomassMultiplier;
        private static ConfigEntry<float> plantsMultiplier;
        private static ConfigEntry<float> insectsMultiplier;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin is loaded!");

            oxygenMultiplier = Config.Bind("GlobalMultipliers", "Oxygen", 0.3f, "The global multiplier of all oxygen generation.");
            energyMultiplier = Config.Bind("GlobalMultipliers", "Energy", 0.3f, "The global multiplier of all energy generation.");
            heatMultiplier = Config.Bind("GlobalMultipliers", "Heat", 0.3f, "The global multiplier of all heat generation.");
            pressureMultiplier = Config.Bind("GlobalMultipliers", "Pressure", 0.3f, "The global multiplier of all pressure generation.");
            biomassMultiplier = Config.Bind("GlobalMultipliers", "Biomass", 0.3f, "The global multiplier of all biomass generation.");
            plantsMultiplier = Config.Bind("GlobalMultipliers", "Plants", 0.3f, "The global multiplier of all plants generation.");
            insectsMultiplier = Config.Bind("GlobalMultipliers", "Insects", 0.3f, "The global multiplier of all insects generation.");

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WorldUnit), "SetIncreaseAndDecreaseForWorldObjects")]
        static void WorldUnit_SetIncreaseAndDecreaseForWorldObjects(bool __result, ref float ___increaseValuePerSec, DataConfig.WorldUnitType ___unitType)
        {
            // __result is true only if the world unit generation just changed and has been recalculated
            if (!__result)
            {
                return;
            }

            switch (___unitType)
            {
                case DataConfig.WorldUnitType.Null:
                case DataConfig.WorldUnitType.Terraformation:
                case DataConfig.WorldUnitType.Animals:
                    break;
                case DataConfig.WorldUnitType.Oxygen:
                    ___increaseValuePerSec *= oxygenMultiplier.Value;
                    break;
                case DataConfig.WorldUnitType.Energy:
                    ___increaseValuePerSec *= energyMultiplier.Value;
                    break;
                case DataConfig.WorldUnitType.Heat:
                    ___increaseValuePerSec *= heatMultiplier.Value;
                    break;
                case DataConfig.WorldUnitType.Pressure:
                    ___increaseValuePerSec *= pressureMultiplier.Value;
                    break;
                case DataConfig.WorldUnitType.Biomass:
                    ___increaseValuePerSec *= biomassMultiplier.Value;
                    break;
                case DataConfig.WorldUnitType.Plants:
                    ___increaseValuePerSec *= plantsMultiplier.Value;
                    break;
                case DataConfig.WorldUnitType.Insects:
                    ___increaseValuePerSec *= insectsMultiplier.Value;
                    break;
                default:
                    break;
            }
        }
    }
}