﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using BepInEx;
using BepInEx.Configuration;
using ChebsMythicalWeapons.Creatures;
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
        public const string PluginVersion = "4.5.0";

        private const string ConfigFileName = PluginGuid + ".cfg";
        private static readonly string ConfigFileFullPath = Path.Combine(Paths.ConfigPath, ConfigFileName);

        public readonly System.Version ChebsValheimLibraryVersion = new("2.6.1");

        private readonly Harmony _harmony = new(PluginGuid);

        // if set to true, the particle effects that for some reason hurt radeon are dynamically disabled
        public static ConfigEntry<bool> RadeonFriendly;

        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        public static ExcaliburItem Excalibur = new();
        public static ApolloBowItem ApolloBow = new();
        public static SunArrowItem SunArrow = new();
        public static BladeOfOlympusItem BladeOfOlympus = new();
        public static GreatswordOfOlympusItem GreatswordOfOlympus = new();
        public static JoyceItem Joyce = new();
        public static AegisItem Aegis = new();

        private void Awake()
        {
            if (!Base.VersionCheck(ChebsValheimLibraryVersion, out string message))
            {
                Jotunn.Logger.LogWarning(message);
            }

            CreateConfigValues();
            LoadAssetBundle();
            _harmony.PatchAll();

            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                Logger.LogInfo(!attr.InitialSynchronization
                    ? "Syncing configuration changes from server..."
                    : "Syncing initial configuration...");
            };

            StartCoroutine(WatchConfigFile());
        }
        
        #region ConfigUpdate
        private byte[] GetFileHash(string fileName)
        {
            var sha1 = HashAlgorithm.Create();
            using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            return sha1.ComputeHash(stream);
        }

        private IEnumerator WatchConfigFile()
        {
            var lastHash = GetFileHash(ConfigFileFullPath);
            while (true)
            {
                yield return new WaitForSeconds(5);
                var hash = GetFileHash(ConfigFileFullPath);
                if (!hash.SequenceEqual(lastHash))
                {
                    lastHash = hash;
                    ReadConfigValues();
                }
            }
        }
        
        private void ReadConfigValues()
        {
            try
            {
                var adminOrLocal = ZNet.instance.IsServerInstance() || ZNet.instance.IsLocalInstance();
                Logger.LogInfo($"Read updated config values (admin/local={adminOrLocal})");
                if (adminOrLocal) Config.Reload();
            }
            catch (Exception exc)
            {
                Logger.LogError($"There was an issue loading your {ConfigFileName}: {exc}");
            }
        }
        #endregion

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

            SwordInTheStoneLocation.CreateConfigs(this);
            Excalibur.CreateConfigs(this);

            #endregion

            #region ApolloBow

            ApolloStatueLocation.CreateConfigs(this);
            ApolloBow.CreateConfigs(this);
            SunArrow.CreateConfigs(this);

            #endregion

            #region BladeOfOlympus

            BladeOfOlympus.CreateConfigs(this);
            GreatswordOfOlympus.CreateConfigs(this);

            #endregion

            #region Joyce

            Joyce.CreateConfigs(this);
            MinotaurCreature.CreateConfigs(this);

            #endregion
            
            #region Aegis
            Aegis.CreateConfigs(this);
            #endregion
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
                    var excaliburPrefab = Base.LoadPrefabFromBundle(Excalibur.PrefabName, chebgonazAssetBundle,
                        RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(Excalibur.GetCustomItemFromPrefab(excaliburPrefab));

                    // stone pickable
                    var swordInTheStonePickablePrefab =
                        chebgonazAssetBundle.LoadAsset<GameObject>(SwordInTheStonePickable.PickablePrefabName);
                    swordInTheStonePickablePrefab.AddComponent<SwordInTheStonePickable>();
                    PrefabManager.Instance.AddPrefab(swordInTheStonePickablePrefab);

                    // stone location
                    var swordInTheStoneLocationPrefab =
                        chebgonazAssetBundle.LoadAsset<GameObject>(SwordInTheStoneLocation.PrefabName);
                    swordInTheStoneLocationPrefab.AddComponent<SwordInTheStoneLocation>();
                    var swordInTheStoneConfig = new LocationConfig()
                    {
                        Biome = SwordInTheStoneLocation.Biome.Value,
                        Quantity = SwordInTheStoneLocation.Quantity.Value,
                        Priotized = true,
                        ExteriorRadius = 2f,
                        ClearArea = true,
                    };
                    var customLocation =
                        new CustomLocation(swordInTheStoneLocationPrefab, false, swordInTheStoneConfig);
                    ZoneManager.Instance.AddCustomLocation(customLocation);
                }
                {
                    // bow
                    var apolloBowPrefab = Base.LoadPrefabFromBundle(ApolloBow.PrefabName, chebgonazAssetBundle,
                        RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(ApolloBow.GetCustomItemFromPrefab(apolloBowPrefab));

                    // bow pickable
                    var bowPickablePrefab =
                        chebgonazAssetBundle.LoadAsset<GameObject>(ApolloStatuePickable.PickablePrefabName);
                    bowPickablePrefab.AddComponent<ApolloStatuePickable>();
                    PrefabManager.Instance.AddPrefab(bowPickablePrefab);

                    // bow location
                    var bowLocationPrefab = chebgonazAssetBundle.LoadAsset<GameObject>(ApolloStatueLocation.PrefabName);
                    bowLocationPrefab.AddComponent<ApolloStatueLocation>();
                    var config = new LocationConfig()
                    {
                        Biome = ApolloStatueLocation.Biome.Value,
                        Quantity = ApolloStatueLocation.Quantity.Value,
                        Priotized = true,
                        ExteriorRadius = 2f,
                        ClearArea = true,
                    };
                    var customLocation = new CustomLocation(bowLocationPrefab, false, config);
                    ZoneManager.Instance.AddCustomLocation(customLocation);
                }
                {
                    // sun arrow
                    var prefab = Base.LoadPrefabFromBundle(SunArrow.PrefabName, chebgonazAssetBundle,
                        RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(SunArrow.GetCustomItemFromPrefab(prefab));
                }
                {
                    // blade of olympus
                    var prefab = Base.LoadPrefabFromBundle(BladeOfOlympus.PrefabName, chebgonazAssetBundle,
                        RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(BladeOfOlympus.GetCustomItemFromPrefab(prefab));
                }
                {
                    // greatsword of olympus
                    var prefab = Base.LoadPrefabFromBundle(GreatswordOfOlympus.PrefabName, chebgonazAssetBundle,
                        RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(GreatswordOfOlympus.GetCustomItemFromPrefab(prefab));
                }
                {
                    // axe
                    var prefab =
                        Base.LoadPrefabFromBundle(Joyce.PrefabName, chebgonazAssetBundle, RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(Joyce.GetCustomItemFromPrefab(prefab));

                    // minotaur
                    var minoPrefab = Base.LoadPrefabFromBundle(MinotaurCreature.PrefabName, chebgonazAssetBundle,
                        RadeonFriendly.Value);
                    minoPrefab.AddComponent<MinotaurCreature>();
                    var customCreatureConfig = new CreatureConfig
                    {
                        Name = MinotaurCreature.LocalizedName,
                        Faction = MinotaurCreature.Faction.Value,
                    };
                    customCreatureConfig.AddSpawnConfig(new SpawnConfig()
                    {
                        SpawnChance = MinotaurCreature.SpawnChance.Value,
                        SpawnInterval = MinotaurCreature.SpawnInterval.Value,
                        SpawnDistance = MinotaurCreature.SpawnDistance.Value,
                        MaxSpawned = MinotaurCreature.MaxSpawned.Value,
                        Biome = MinotaurCreature.Biome.Value
                    });
                    var customCreature = new CustomCreature(minoPrefab, true, customCreatureConfig);
                    if (minoPrefab.TryGetComponent(out Humanoid humanoid))
                    {
                        humanoid.m_health = MinotaurCreature.Health.Value;
                    }
                    else
                    {
                        Logger.LogError("Failed to set minotaur health (no humanoid component)");
                    }

                    CreatureManager.Instance.AddCreature(customCreature);
                }
                {
                    // Aegis
                    var prefab = Base.LoadPrefabFromBundle(Aegis.PrefabName, chebgonazAssetBundle,
                        RadeonFriendly.Value);
                    ItemManager.Instance.AddItem(Aegis.GetCustomItemFromPrefab(prefab));
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