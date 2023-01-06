using BepInEx;
using MijuTools;
using SpaceCraft;
using HarmonyLib;
using BepInEx.Configuration;
using System.Linq;
using System.Collections.Generic;
using BepInEx.Logging;

namespace CustomProgressSpeed
{
    [BepInPlugin("mikeypdog.theplanetcraftermods.customprogressspeed", "Custom progress speed", "1.0.1.1")]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource logger;

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
            Logger.LogInfo($"Plugin loading..");
            logger = Logger;

            oxygenMultiplier = Config.Bind("GlobalMultipliers", "Oxygen", 0.3f, "The global multiplier of all oxygen generation.");
            energyMultiplier = Config.Bind("GlobalMultipliers", "Energy", 0.3f, "The global multiplier of all energy generation.");
            heatMultiplier = Config.Bind("GlobalMultipliers", "Heat", 0.3f, "The global multiplier of all heat generation.");
            pressureMultiplier = Config.Bind("GlobalMultipliers", "Pressure", 0.3f, "The global multiplier of all pressure generation.");
            biomassMultiplier = Config.Bind("GlobalMultipliers", "Biomass", 0.3f, "The global multiplier of all biomass generation.");
            plantsMultiplier = Config.Bind("GlobalMultipliers", "Plants", 0.3f, "The global multiplier of all plants generation.");
            insectsMultiplier = Config.Bind("GlobalMultipliers", "Insects", 0.3f, "The global multiplier of all insects generation.");

            Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Plugin is loaded!");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StaticDataHandler), "LoadStaticData")]
        static void StaticDataHandler_LoadStaticData()
        {
            var groups = GroupsHandler.GetGroupsConstructible();
            foreach (var g in groups)
            {
                // Use reflection to get the private field unitsGenerations
                var unitsGenerations = g
                        .GetType()
                        .GetField("unitsGenerations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(g) as Dictionary<DataConfig.WorldUnitType, float>;

                if (unitsGenerations == null)
                {
                    return;
                }

                // Multiply each value by the config value
                MultiplyGeneration(unitsGenerations, DataConfig.WorldUnitType.Oxygen, oxygenMultiplier.Value);
                MultiplyGeneration(unitsGenerations, DataConfig.WorldUnitType.Energy, energyMultiplier.Value);
                MultiplyGeneration(unitsGenerations, DataConfig.WorldUnitType.Heat, heatMultiplier.Value);
                MultiplyGeneration(unitsGenerations, DataConfig.WorldUnitType.Pressure, pressureMultiplier.Value);
                MultiplyGeneration(unitsGenerations, DataConfig.WorldUnitType.Biomass, biomassMultiplier.Value);
                MultiplyGeneration(unitsGenerations, DataConfig.WorldUnitType.Plants, plantsMultiplier.Value);
                MultiplyGeneration(unitsGenerations, DataConfig.WorldUnitType.Insects, insectsMultiplier.Value);
            }
        }

        private static void MultiplyGeneration(Dictionary<DataConfig.WorldUnitType, float> unitsGenerations, DataConfig.WorldUnitType unit, float mult)
        {
            if (!unitsGenerations.ContainsKey(unit))
            {
                return;
            }
            if (unitsGenerations[unit] < 0)
            {
                return;
            }
            unitsGenerations[unit] *= mult;
        }
    }
}