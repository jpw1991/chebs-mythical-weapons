using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using ChebsMythicalWeapons.Items;
using ChebsMythicalWeapons.Locations;
using ChebsMythicalWeapons.Pickables;
using ChebsValheimLibrary;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using Paths = BepInEx.Paths;

namespace ChebsMythicalWeapons
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class ChebsMythicalWeapons : BaseUnityPlugin
    {
        public const string PluginGuid = "com.chebgonaz.chebsmythicalweapons";
        public const string PluginName = "ChebsMythicalWeapons";
        public const string PluginVersion = "2.0.0";
        
        private const string ConfigFileName = PluginGuid + ".cfg";
        private static readonly string ConfigFileFullPath = Path.Combine(Paths.ConfigPath, ConfigFileName);

        public readonly System.Version ChebsValheimLibraryVersion = new("2.0.0");

        private readonly Harmony harmony = new(PluginGuid);

        // if set to true, the particle effects that for some reason hurt radeon are dynamically disabled
        public static ConfigEntry<bool> RadeonFriendly;
        public static ConfigEntry<int> SwordInTheStoneQuantity, ApolloBowQuantity;
        public static ConfigEntry<Heightmap.Biome> SwordInTheStoneLocationBiome, ApolloBowLocationBiome;
        public static ConfigEntry<int> SwordSkillRequired, BowSkillRequired;
        public static ConfigEntry<bool> ShowMapMarker, ApolloBowShowMapMarker;
        public static ConfigEntry<Minimap.PinType> MapMarker, ApolloBowMapMarker;

        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        public static ExcaliburItem Excalibur = new();
        public static ApolloBowItem ApolloBow = new();
        public static SunArrowItem SunArrow = new();

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

            RadeonFriendly = Config.Bind($"{GetType().Name} (Client)", "RadeonFriendly",
                false, new ConfigDescription("ONLY set this to true if you have graphical issues with " +
                                             "the mod. It will disable all particle effects for the mod's prefabs " +
                                             "which seem to give users with Radeon cards trouble for unknown " +
                                             "reasons. If you have problems with lag it might also help to switch" +
                                             "this setting on."));

            #region SwordInTheStone
            SwordInTheStoneQuantity = Config.Bind($"{GetType().Name} (Server Synced)", "SwordInTheStoneQuantity",
                30, new ConfigDescription(
                    "The amount of Sword in the Stone locations in the world.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SwordInTheStoneLocationBiome = Config.Bind($"{GetType().Name} (Server Synced)", "SwordInTheStoneLocationBiome",
                Heightmap.Biome.Meadows, new ConfigDescription(
                    "The biome in which a Sword in the Stone can appear.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            SwordSkillRequired = Config.Bind($"{GetType().Name} (Server Synced)", "SwordSkillRequired",
                100, new ConfigDescription(
                    "The sword skill required to take Excalibur out of the stone.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            ShowMapMarker = Config.Bind($"{GetType().Name} (Client)", "ShowMapMarker",
                true, new ConfigDescription("Whether a Sword in the Stone will appear on the map or not."));
            
            MapMarker = Config.Bind($"{GetType().Name} (Client)", "MapMarker",
                Minimap.PinType.Boss, new ConfigDescription("The type of map marker shown for the Sword in the Stone."));
            
            Excalibur.CreateConfigs(this);
            #endregion
            #region ApolloBow
            ApolloBowQuantity = Config.Bind($"{GetType().Name} (Server Synced)", "ApolloBowQuantity",
                30, new ConfigDescription(
                    "The amount of Statue of Apollo locations in the world.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ApolloBowLocationBiome = Config.Bind($"{GetType().Name} (Server Synced)", "ApolloBowLocationBiome",
                Heightmap.Biome.Meadows, new ConfigDescription(
                    "The biome in which a Statue of Apollo can appear.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            BowSkillRequired = Config.Bind($"{GetType().Name} (Server Synced)", "BowSkillRequired",
                100, new ConfigDescription(
                    "The bow skill required to take Apollo's Bow from the statue.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            ApolloBowShowMapMarker = Config.Bind($"{GetType().Name} (Client)", "ApolloBowShowMapMarker",
                true, new ConfigDescription("Whether a Statue of Apollo will appear on the map or not."));
            
            ApolloBowMapMarker = Config.Bind($"{GetType().Name} (Client)", "ApolloBowMapMarker",
                Minimap.PinType.Boss, new ConfigDescription("The type of map marker shown for the Statue of Apollo."));
            
            ApolloBow.CreateConfigs(this);
            SunArrow.CreateConfigs(this);
            #endregion
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
            var assetBundlePath = Path.Combine(Path.GetDirectoryName(Info.Location), "chebsmythicalweapons");
            var chebgonazAssetBundle = AssetUtils.LoadAssetBundle(assetBundlePath);
            try
            {
                {
                    // sword
                    var excaliburPrefab = Base.LoadPrefabFromBundle(Excalibur.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(Excalibur.GetCustomItemFromPrefab(excaliburPrefab));

                    // stone pickable
                    var swordInTheStonePickablePrefab = chebgonazAssetBundle.LoadAsset<GameObject>(SwordInTheStonePickable.PickablePrefabName);
                    swordInTheStonePickablePrefab.AddComponent<SwordInTheStonePickable>();
                    PrefabManager.Instance.AddPrefab(swordInTheStonePickablePrefab);
                
                    // stone location
                    var swordInTheStoneLocationPrefab = chebgonazAssetBundle.LoadAsset<GameObject>(SwordInTheStoneLocation.PrefabName);
                    swordInTheStoneLocationPrefab.AddComponent<SwordInTheStoneLocation>();
                    var swordInTheStoneConfig = new LocationConfig()
                    {
                        Biome = SwordInTheStoneLocationBiome.Value,
                        Quantity = SwordInTheStoneQuantity.Value,
                        Priotized = true,
                        ExteriorRadius = 2f,
                        ClearArea = true,
                    };
                    var customLocation = new CustomLocation(swordInTheStoneLocationPrefab, false, swordInTheStoneConfig);
                    ZoneManager.Instance.AddCustomLocation(customLocation);
                }
                {
                    // bow
                    var apolloBowPrefab = Base.LoadPrefabFromBundle(ApolloBow.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(ApolloBow.GetCustomItemFromPrefab(apolloBowPrefab));
                
                    // bow pickable
                    var bowPickablePrefab = chebgonazAssetBundle.LoadAsset<GameObject>(ApolloStatuePickable.PickablePrefabName);
                    bowPickablePrefab.AddComponent<ApolloStatuePickable>();
                    PrefabManager.Instance.AddPrefab(bowPickablePrefab);   
                    
                    // bow location
                    var bowLocationPrefab = chebgonazAssetBundle.LoadAsset<GameObject>(ApolloStatueLocation.PrefabName);
                    bowLocationPrefab.AddComponent<ApolloStatueLocation>();
                    var config = new LocationConfig()
                    {
                        Biome = ApolloBowLocationBiome.Value,
                        Quantity = ApolloBowQuantity.Value,
                        Priotized = true,
                        ExteriorRadius = 2f,
                        ClearArea = true,
                    };
                    var customLocation = new CustomLocation(bowLocationPrefab, false, config);
                    ZoneManager.Instance.AddCustomLocation(customLocation);
                }
                {
                    // sun arrow
                    var prefab = Base.LoadPrefabFromBundle(SunArrow.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(SunArrow.GetCustomItemFromPrefab(prefab));
                }
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