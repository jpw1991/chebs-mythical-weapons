using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Items;
using Jotunn.Configs;
using Jotunn.Entities;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsMythicalWeapons.Items
{
    public class SunArrowItem : Item
    {
        public override string ItemName => "ChebGonaz_SunArrow";
        public override string PrefabName => "ChebGonaz_SunArrow.prefab";
        public override string NameLocalization => "$chebgonaz_sunarrow";
        public override string DescriptionLocalization => "$chebgonaz_sunarrow_desc";
        protected override string DefaultRecipe => "Flametal:2,FineWood:20";

        public static ConfigEntry<CraftingTable> CraftingStationRequired;
        public static ConfigEntry<int> CraftingStationLevel;
        public static ConfigEntry<string> CraftingCost;

        public static ConfigEntry<float> Knockback, PiercingDamage, FireDamage;

        public override void CreateConfigs(BaseUnityPlugin plugin)
        {
            CraftingStationRequired = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "CraftingStation",
                CraftingTable.Forge, new ConfigDescription("Crafting station where it's available",
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

            Knockback = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "Knockback",
                35f, new ConfigDescription(
                    "Bow's base knockback value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            PiercingDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "PiercingDamage",
                95f, new ConfigDescription(
                    "Base piercing damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            FireDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "FireDamage",
                5f, new ConfigDescription(
                    "Base fire damage value.", null,
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
                Amount = 20,
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
            itemDataShared.m_damages.m_pierce = PiercingDamage.Value;
            itemDataShared.m_damages.m_fire = FireDamage.Value;

            #endregion

            return customItem;
        }
    }
}