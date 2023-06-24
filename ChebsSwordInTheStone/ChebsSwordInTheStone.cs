using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using ChebsSwordInTheStone.Items;
using ChebsSwordInTheStone.Structures;
using ChebsValheimLibrary;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using Paths = BepInEx.Paths;

namespace ChebsSwordInTheStone
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class ChebsSwordInTheStone : BaseUnityPlugin
    {
        public const string PluginGuid = "com.chebgonaz.chebsswordinthestone";
        public const string PluginName = "ChebsSwordInTheStone";
        public const string PluginVersion = "1.0.0";
        
        private const string ConfigFileName = PluginGuid + ".cfg";
        private static readonly string ConfigFileFullPath = Path.Combine(Paths.ConfigPath, ConfigFileName);

        public readonly System.Version ChebsValheimLibraryVersion = new("2.0.0");

        private readonly Harmony harmony = new(PluginGuid);

        // if set to true, the particle effects that for some reason hurt radeon are dynamically disabled
        public static ConfigEntry<bool> RadeonFriendly;
        public static ConfigEntry<int> SwordInTheStoneQuantity;
        public static ConfigEntry<Heightmap.Biome> SwordInTheStoneLocationBiome;

        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        public static ExcaliburItem Excalibur = new();

        private void Awake()
        {
            if (!Base.VersionCheck(ChebsValheimLibraryVersion, out string message))
            {
                Jotunn.Logger.LogWarning(message);
            }

            CreateConfigValues();
            LoadAssetBundle();
            harmony.PatchAll();

            SetupWatcher();
        }
        
        private void CreateConfigValues()
        {
            Config.SaveOnConfigSet = true;

            RadeonFriendly = Config.Bind("General (Client)", "RadeonFriendly",
                false, new ConfigDescription("ONLY set this to true if you have graphical issues with " +
                                             "the mod. It will disable all particle effects for the mod's prefabs " +
                                             "which seem to give users with Radeon cards trouble for unknown " +
                                             "reasons. If you have problems with lag it might also help to switch" +
                                             "this setting on."));

            // todo: excalibur
            
            SwordInTheStoneQuantity = Config.Bind($"{GetType().Name} (Server Synced)", "SwordInTheStoneQuantity",
                3, new ConfigDescription(
                    "The amount of Sword in the Stone locations in the world.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SwordInTheStoneLocationBiome = Config.Bind($"{GetType().Name} (Server Synced)", "SwordInTheStoneLocationBiome",
                Heightmap.Biome.Meadows, new ConfigDescription(
                    "The biome in which a Sword in the Stone can appear.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.Error += (sender, e) => Jotunn.Logger.LogError($"Error watching for config changes: {e}");
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                Logger.LogInfo("Read updated config values");
                Config.Reload();

                // todo update excalibur recipe
            }
            catch (Exception exc)
            {
                Logger.LogError($"There was an issue loading your {ConfigFileName}: {exc}");
                Logger.LogError("Please check your config entries for spelling and format!");
            }
        }

        private void LoadAssetBundle()
        {
            // order is important (I think): items, creatures, structures
            var assetBundlePath = Path.Combine(Path.GetDirectoryName(Info.Location), "chebsswordinthestone");
            var chebgonazAssetBundle = AssetUtils.LoadAssetBundle(assetBundlePath);
            try
            {
                // sword
                var excaliburPrefab = Base.LoadPrefabFromBundle(Excalibur.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                ItemManager.Instance.AddItem(Excalibur.GetCustomItemFromPrefab(excaliburPrefab));

                // stone
                var swordInTheStonePrefab = chebgonazAssetBundle.LoadAsset<GameObject>(SwordInTheStone.PrefabName);
                swordInTheStonePrefab.AddComponent<SwordInTheStone>();

                var swordInTheStoneConfig = new LocationConfig()
                {
                    Biome = SwordInTheStoneLocationBiome.Value,
                    Quantity = SwordInTheStoneQuantity.Value
                };
                var customLocation = new CustomLocation(swordInTheStonePrefab, true, swordInTheStoneConfig);
                ZoneManager.Instance.AddCustomLocation(customLocation);

            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Exception caught while loading assets: {ex}");
            }
            finally
            {
                chebgonazAssetBundle.Unload(false);
            }
        }
    }
}