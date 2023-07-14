using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Common;
using ChebsValheimLibrary.Items;
using Jotunn.Configs;
using Jotunn.Entities;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsMythicalWeapons.Items
{
    public class BladeOfOlympusItem : Item
    {
        public override string ItemName => "ChebGonaz_BladeOfOlympus";
        public override string PrefabName => "ChebGonaz_BladeOfOlympus.prefab";
        public override string NameLocalization => "$chebgonaz_bladeofolympus";
        public override string DescriptionLocalization => "$chebgonaz_bladeofolympus_desc";

        protected override string DefaultRecipe => "Flametal:20,Silver:20,BlackMetal:20,FineWood:20";

        public static ConfigEntry<CraftingTable> CraftingStationRequired;
        public static ConfigEntry<int> CraftingStationLevel;
        public static ConfigEntry<string> CraftingCost;
        public static ConfigEntry<Weather.Env> CraftingWeatherCondition;

        public static ConfigEntry<float> Knockback,
            BackstabBonus,
            SlashDamage,
            LightningDamage,
            BonusSlashDamagePerLevel,
            BonusLightningDamagePerLevel,
            BlockPower,
            BlockPowerPerLevel,
            DeflectionForce,
            DeflectionForcePerLevel;

        public override void CreateConfigs(BaseUnityPlugin plugin)
        {
            CraftingStationRequired = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "CraftingStation",
                CraftingTable.BlackForge, new ConfigDescription("Crafting station where it's available",
                    null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            CraftingStationLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "CraftingStationLevel",
                1,
                new ConfigDescription("Crafting station level required to craft",
                    new AcceptableValueRange<int>(1, 5),
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            CraftingCost = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "CraftingCosts",
                DefaultRecipe, new ConfigDescription(
                    "Materials needed to craft it. None or Blank will use Default settings.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            CraftingWeatherCondition = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "CraftingWeatherCondition",
                Weather.Env.Nofogts, new ConfigDescription(
                    "The weather event required to forge the blade. Set to None to craft under any weather conditions.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            Knockback = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "Knockback",
                50f, new ConfigDescription(
                    "BladeOfOlympus's base knockback value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BackstabBonus = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BackstabBonus",
                3f, new ConfigDescription(
                    "BladeOfOlympus's base backstab value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SlashDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "SlashDamage",
                100f, new ConfigDescription(
                    "BladeOfOlympus's base slash damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            LightningDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "LightningDamage",
                50f, new ConfigDescription(
                    "BladeOfOlympus's base spirit damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusSlashDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "BonusSlashDamagePerLevel",
                20f, new ConfigDescription(
                    "BladeOfOlympus's slash damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusLightningDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "BonusLightningDamagePerLevel",
                0f, new ConfigDescription(
                    "BladeOfOlympus's spirit damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BlockPower = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BlockPower",
                43f, new ConfigDescription(
                    "BladeOfOlympus's base blocking power.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BlockPowerPerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BlockPowerPerLevel",
                0f, new ConfigDescription(
                    "BladeOfOlympus's blocking power increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DeflectionForce = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "DeflectionForce",
                43f, new ConfigDescription(
                    "BladeOfOlympus's base deflection force.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DeflectionForcePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "DeflectionForcePerLevel",
                0f, new ConfigDescription(
                    "BladeOfOlympus's deflection force increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }

        public override void UpdateRecipe()
        {
            UpdateRecipe(CraftingStationRequired, CraftingCost, CraftingStationLevel);
        }

        public override CustomItem GetCustomItemFromPrefab(GameObject prefab)
        {
            var config = new ItemConfig
            {
                Name = NameLocalization,
                Description = DescriptionLocalization,
            };

            if (string.IsNullOrEmpty(CraftingCost.Value))
            {
                CraftingCost.Value = DefaultRecipe;
            }

            SetRecipeReqs(
                config,
                CraftingCost,
                CraftingStationRequired,
                CraftingStationLevel
            );

            var customItem = new CustomItem(prefab, false, config);
            if (customItem.ItemPrefab == null)
            {
                Logger.LogError($"GetCustomItemFromPrefab: {PrefabName}'s ItemPrefab is null!");
                return null;
            }

            var itemDataShared = customItem.ItemDrop.m_itemData.m_shared;

            #region AttackSettings

            itemDataShared.m_attackForce = Knockback.Value;
            itemDataShared.m_backstabBonus = BackstabBonus.Value;
            itemDataShared.m_damages.m_slash = SlashDamage.Value;
            itemDataShared.m_damages.m_lightning = LightningDamage.Value;
            itemDataShared.m_damagesPerLevel.m_slash = BonusSlashDamagePerLevel.Value;
            itemDataShared.m_damagesPerLevel.m_lightning = BonusLightningDamagePerLevel.Value;

            #endregion

            #region ShieldSettings

            itemDataShared.m_blockPower = BlockPower.Value; // block force
            itemDataShared.m_blockPowerPerLevel = BlockPowerPerLevel.Value;
            itemDataShared.m_deflectionForce = DeflectionForce.Value;
            itemDataShared.m_deflectionForcePerLevel = DeflectionForcePerLevel.Value;

            #endregion

            return customItem;
        }
    }
}