using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsMythicalWeapons.Locations
{
    public class SwordInTheStoneLocation : MonoBehaviour
    {
        public const string PrefabName = "ChebGonaz_SwordInTheStone.prefab";
        public const string NameLocalization = "$chebgonaz_swordinthestone";

        public static ConfigEntry<Heightmap.Biome> Biome;
        public static ConfigEntry<Minimap.PinType> MapMarker;
        public static ConfigEntry<int> Quantity;
        public static ConfigEntry<int> SwordSkillRequired;

        public static void CreateConfigs(BaseUnityPlugin plugin)
        {
            const string serverSynced = "SwordInTheStoneLocation (Server Synced)";
            const string client = "SwordInTheStoneLocation (Client)";

            MapMarker = plugin.Config.Bind(client, "MapMarker",
                Minimap.PinType.Boss, new ConfigDescription("The type of map marker shown (set to None to disable)."));

            Quantity = plugin.Config.Bind(serverSynced, "Quantity",
                30, new ConfigDescription(
                    "The amount of Sword in the Stone locations in the world.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            Biome = plugin.Config.Bind(serverSynced,
                "Biome",
                Heightmap.Biome.Meadows, new ConfigDescription(
                    "The biome in which a Sword in the Stone can appear.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SwordSkillRequired = plugin.Config.Bind(serverSynced, "SwordSkillRequired",
                100, new ConfigDescription(
                    "The sword skill required to take Excalibur out of the stone.", null,
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