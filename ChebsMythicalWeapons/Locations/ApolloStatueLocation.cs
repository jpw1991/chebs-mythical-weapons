using System;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsMythicalWeapons.Locations
{
    public class ApolloStatueLocation : MonoBehaviour
    {
        public const string PrefabName = "ChebGonaz_ApolloStatueLocation.prefab";
        public const string NameLocalization = "$chebgonaz_apollostatue";

        public static ConfigEntry<Heightmap.Biome> Biome;
        public static ConfigEntry<Minimap.PinType> MapMarker;
        public static ConfigEntry<int> Quantity;
        public static ConfigEntry<int> BowSkillRequired;
        
        public static void CreateConfigs(BaseUnityPlugin plugin)
        {
            const string serverSynced = "ApolloStatueLocation (Server Synced)";
            const string client = "ApolloStatueLocation (Client)";

            MapMarker = plugin.Config.Bind(client, "MapMarker",
                Minimap.PinType.Boss, new ConfigDescription("The type of map marker shown (set to None to disable)."));
            
            Quantity = plugin.Config.Bind(serverSynced, "ApolloBowQuantity",
                30, new ConfigDescription(
                    "The amount of Statue of Apollo locations in the world.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            Biome = plugin.Config.Bind(serverSynced, "ApolloBowLocationBiome",
                Heightmap.Biome.Meadows, new ConfigDescription(
                    "The biome in which a Statue of Apollo can appear.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BowSkillRequired = plugin.Config.Bind(serverSynced, "BowSkillRequired",
                100, new ConfigDescription(
                    "The bow skill required to take Apollo's Bow from the statue.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }

        private void Awake()
        {
            Logger.LogInfo($"Awakening at {transform.position}");

            if (MapMarker.Value == Minimap.PinType.None) return;

            Minimap.instance.AddPin(transform.position,
                MapMarker.Value,
                ChebsMythicalWeapons.Localization.TryTranslate(NameLocalization),
                true, false,
                Game.instance.GetPlayerProfile().GetPlayerID());
        }
    }
}