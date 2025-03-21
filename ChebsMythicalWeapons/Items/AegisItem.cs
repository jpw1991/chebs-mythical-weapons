using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Common;
using ChebsValheimLibrary.Items;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using SoftReferenceableAssets;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsMythicalWeapons.Items
{
    public class AegisItem : Item
    {
        public override string ItemName => "ChebGonaz_Aegis";
        public override string PrefabName => "ChebGonaz_Aegis.prefab";
        public override string NameLocalization => "$chebgonaz_aegis";
        public override string DescriptionLocalization => "$chebgonaz_aegis_desc";
        protected override string DefaultRecipe => "Bronze:60,FineWood:60";

        public static ConfigEntry<CraftingTable> CraftingStationRequired;
        public static ConfigEntry<int> CraftingStationLevel;
        public static ConfigEntry<string> CraftingCost;
        
        public static ConfigEntry<float> BlockPower,
            BlockPowerPerLevel,
            DeflectionForce,
            DeflectionForcePerLevel;

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
            
            BlockPower = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BlockPower",
                70f, new ConfigDescription(
                    "Aegis's base blocking power.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BlockPowerPerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BlockPowerPerLevel",
                0f, new ConfigDescription(
                    "Aegis's blocking power increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DeflectionForce = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "DeflectionForce",
                50f, new ConfigDescription(
                    "Aegis's base deflection force.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DeflectionForcePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "DeflectionForcePerLevel",
                0f, new ConfigDescription(
                    "Aegis's deflection force increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }
        
        public override void UpdateRecipe()
        {
            UpdateRecipe(CraftingStationRequired, CraftingCost, CraftingStationLevel);
        }

        public override CustomItem GetCustomItemFromPrefab(GameObject prefab, bool fixReferences = true)
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

            var customItem = new CustomItem(prefab, fixReferences, config);
            if (customItem.ItemPrefab == null)
            {
                Logger.LogError($"GetCustomItemFromPrefab: {PrefabName}'s ItemPrefab is null!");
                return null;
            }

            var shared = customItem.ItemDrop.m_itemData.m_shared;
            SetItemDataShared(ref shared);

            return customItem;
        }
        
        public void UpdateItemValues()
        {
            var prefab = ZNetScene.instance?.GetPrefab(ItemName) ?? PrefabManager.Instance.GetPrefab(ItemName);
            if (prefab == null)
            {
                Logger.LogError($"Failed to update item values: prefab with name {ItemName} is null");
                return;
            }

            var item = prefab.GetComponent<ItemDrop>();
            var shared = item.m_itemData.m_shared;
            SetItemDataShared(ref shared);
        }

        private void SetItemDataShared(ref ItemDrop.ItemData.SharedData shared)
        {
            shared.m_maxQuality = 5;

            #region ShieldSettings

            shared.m_blockPower = BlockPower.Value; // block force
            shared.m_blockPowerPerLevel = BlockPowerPerLevel.Value;
            shared.m_deflectionForce = DeflectionForce.Value;
            shared.m_deflectionForcePerLevel = DeflectionForcePerLevel.Value;

            #endregion
        }

        public static void HandleUpgradesForSelectedRecipe(InventoryGui.RecipeDataPair selectedRecipe)
        {
            var itemQuality = selectedRecipe.Recipe.m_qualityResultAmountMultiplier;
            switch (itemQuality)
            {
                case 1:
                    selectedRecipe.Recipe.m_resources = new[]
                    {
                        new Piece.Requirement()
                        {
                            m_resItem = PrefabManager.Instance.GetPrefab("Iron").GetComponent<ItemDrop>(),
                            m_amount = 1,
                            m_amountPerLevel = 60,
                        }
                    };
                    break;
                case 2:
                    selectedRecipe.Recipe.m_resources = new[]
                    {
                        new Piece.Requirement()
                        {
                            m_resItem = PrefabManager.Instance.GetPrefab("Silver").GetComponent<ItemDrop>(),
                            m_amount = 1,
                            m_amountPerLevel = 30,
                        }
                    };
                    break;
                case 3:
                    selectedRecipe.Recipe.m_resources = new[]
                    {
                        new Piece.Requirement()
                        {
                            m_resItem = PrefabManager.Instance.GetPrefab("BlackMetal").GetComponent<ItemDrop>(),
                            m_amount = 1,
                            m_amountPerLevel = 19,
                        }
                    };
                    break;
                case 4:
                    selectedRecipe.Recipe.m_resources = new[]
                    {
                        new Piece.Requirement()
                        {
                            m_resItem = PrefabManager.Instance.GetPrefab("FlametalNew").GetComponent<ItemDrop>(),
                            m_amount = 1,
                            m_amountPerLevel = 15,
                        }
                    };
                    break;
                default: // initial state = 1, or unhandled case
                    Logger.LogWarning(
                        $"Unhandled case in recipes (quality = {itemQuality} for AegisItem), please tell Cheb.");
                    break;
            }
        }
    }
}