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
    public class JoyceItem : Item
    {
        public override string ItemName => "ChebGonaz_Joyce";
        public override string PrefabName => "ChebGonaz_Joyce.prefab";
        public override string NameLocalization => "$chebgonaz_joyce";
        public override string DescriptionLocalization => "$chebgonaz_joyce_desc";

        public static ConfigEntry<CraftingTable> CraftingStationRequired;

        public static ConfigEntry<float> Knockback,
            BackstabBonus,
            SlashDamage,
            FireDamage,
            ChopDamage,
            BonusSlashDamagePerLevel,
            BonusFireDamagePerLevel,
            BonusChopDamagePerLevel,
            BlockPower,
            BlockPowerPerLevel,
            DeflectionForce,
            DeflectionForcePerLevel;

        public static ConfigEntry<int> ToolTier;

        public override void CreateConfigs(BaseUnityPlugin plugin)
        {
            CraftingStationRequired = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "CraftingStation",
                CraftingTable.BlackForge, new ConfigDescription("Crafting station where it's available",
                    null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            Knockback = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "Knockback",
                50f, new ConfigDescription(
                    "Joyce's base knockback value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BackstabBonus = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BackstabBonus",
                3f, new ConfigDescription(
                    "Joyce's base backstab value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SlashDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "SlashDamage",
                100f, new ConfigDescription(
                    "Joyce's base slash damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            FireDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "FireDamage",
                50f, new ConfigDescription(
                    "Joyce's base spirit damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            ChopDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "ChopDamage",
                100f, new ConfigDescription(
                    "Joyce's base chop damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusSlashDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "BonusSlashDamagePerLevel",
                20f, new ConfigDescription(
                    "Joyce's slash damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusFireDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "BonusFireDamagePerLevel",
                0f, new ConfigDescription(
                    "Joyce's spirit damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusChopDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "BonusChopDamagePerLevel",
                20f, new ConfigDescription(
                    "Joyce's chop damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BlockPower = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BlockPower",
                43f, new ConfigDescription(
                    "Joyce's base blocking power.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BlockPowerPerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BlockPowerPerLevel",
                0f, new ConfigDescription(
                    "Joyce's blocking power increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DeflectionForce = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "DeflectionForce",
                43f, new ConfigDescription(
                    "Joyce's base deflection force.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DeflectionForcePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "DeflectionForcePerLevel",
                0f, new ConfigDescription(
                    "Joyce's deflection force increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            ToolTier = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "ToolTier",
                4, new ConfigDescription(
                    "Joyce's tool tier level (determines what can be cut eg. 4 = black metal axe).", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }

        public override CustomItem GetCustomItemFromPrefab(GameObject prefab, bool fixReferences = true)
        {
            var config = new ItemConfig
            {
                Name = NameLocalization,
                Description = DescriptionLocalization,
                CraftingStation = InternalName.GetName(CraftingTable.Forge),
                Requirements = new[]
                {
                    // add an upgrade amount of 20 silver per level of Excalibur
                    new RequirementConfig()
                    {
                        Amount = 20,
                        AmountPerLevel = 20,
                        Item = "Silver",
                    }
                },
            };

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
            itemDataShared.m_damages.m_fire = FireDamage.Value;
            itemDataShared.m_damagesPerLevel.m_slash = BonusSlashDamagePerLevel.Value;
            itemDataShared.m_damagesPerLevel.m_fire = BonusFireDamagePerLevel.Value;
            itemDataShared.m_toolTier = ToolTier.Value;
            itemDataShared.m_damages.m_chop = ChopDamage.Value;
            itemDataShared.m_damagesPerLevel.m_chop = BonusChopDamagePerLevel.Value;

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