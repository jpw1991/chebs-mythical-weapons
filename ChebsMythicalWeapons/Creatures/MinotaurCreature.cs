using BepInEx;
using BepInEx.Configuration;
using Jotunn.Managers;

namespace ChebsMythicalWeapons.Creatures
{
    public class MinotaurCreature
    {
        public const string PrefabName = "ChebGonaz_Minotaur.prefab";
        public const string CreatureName = "ChebGonaz_Minotaur";
        public static string LocalizedName => LocalizationManager.Instance.TryTranslate("$chebgonaz_minotaur");

        public static ConfigEntry<Character.Faction> Faction;
        public static ConfigEntry<float> SpawnChance, SpawnInterval, SpawnDistance, Health;
        public static ConfigEntry<int> MaxSpawned;
        public static ConfigEntry<Heightmap.Biome> Biome;

        public static void CreateConfigs(BaseUnityPlugin plugin)
        {
            const string serverSynced = "MinotaurCreature (Server Synced)";

            Faction = plugin.Config.Bind<Character.Faction>(serverSynced, "Faction",
                Character.Faction.PlainsMonsters, new ConfigDescription(
                    "The faction of the character.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SpawnChance = plugin.Config.Bind<float>(serverSynced, "SpawnChance",
                1f, new ConfigDescription(
                    "The chance of spawning the creature.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SpawnInterval = plugin.Config.Bind<float>(serverSynced, "SpawnInterval",
                1f, new ConfigDescription(
                    "The interval between creature spawns.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SpawnDistance = plugin.Config.Bind<float>(serverSynced, "SpawnDistance",
                1f, new ConfigDescription(
                    "The distance at which the creature can spawn.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            MaxSpawned = plugin.Config.Bind<int>(serverSynced, "MaxSpawned",
                1, new ConfigDescription(
                    "The maximum number of creatures that can be spawned.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            Biome = plugin.Config.Bind(serverSynced, "Biome",
                Heightmap.Biome.Plains, new ConfigDescription(
                    "The biome where the creature spawns.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            Health = plugin.Config.Bind<float>(serverSynced, "Health",
                2500f, new ConfigDescription(
                    "The health of the creature.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }
    }
}