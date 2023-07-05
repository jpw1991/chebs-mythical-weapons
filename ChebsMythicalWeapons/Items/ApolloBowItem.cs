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
    public class ApolloBowItem : Item
    {
        public override string ItemName => "ChebGonaz_BowOfApollo";
        public override string PrefabName => "ChebGonaz_BowOfApollo.prefab";
        public override string NameLocalization => "$chebgonaz_bowofapollo";
        public override string DescriptionLocalization => "$chebgonaz_bowofapollo_desc";
        
        public static ConfigEntry<float> Knockback, BackstabBonus,
            PiercingDamage, FireDamage, BonusPiercingDamagePerLevel, BonusFireDamagePerLevel;

        public override void CreateConfigs(BaseUnityPlugin plugin)
        {
            Knockback = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "Knockback",
                35f, new ConfigDescription(
                    "Bow's base knockback value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BackstabBonus = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BackstabBonus",
                3f, new ConfigDescription(
                    "Bow's base backstab value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            PiercingDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "PiercingDamage",
                80f, new ConfigDescription(
                    "Bow's base piercing damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            FireDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "FireDamage",
                40f, new ConfigDescription(
                    "Bow's base fire damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusPiercingDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BonusPiercingDamagePerLevel",
                10f, new ConfigDescription(
                    "Bow's piercing damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusFireDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BonusFireDamagePerLevel",
                0f, new ConfigDescription(
                    "Bow's fire damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }

        public override CustomItem GetCustomItemFromPrefab(GameObject prefab)
        {
            var config = new ItemConfig
            {
                Name = NameLocalization,
                Description = DescriptionLocalization,
                CraftingStation = InternalName.GetName(CraftingTable.Forge),
                Requirements = new []
                {
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
            itemDataShared.m_damages.m_pierce = PiercingDamage.Value;
            itemDataShared.m_damages.m_fire = FireDamage.Value;
            itemDataShared.m_damagesPerLevel.m_pierce = BonusPiercingDamagePerLevel.Value;
            itemDataShared.m_damagesPerLevel.m_fire = BonusFireDamagePerLevel.Value;
            #endregion

            return customItem;
        }
    }
}