using BepInEx;
using BepInEx.Configuration;
using ChebsValheimLibrary.Common;
using ChebsValheimLibrary.Items;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsMythicalWeapons.Items
{
    public class ExcaliburItem : Item
    {
        public override string ItemName => "ChebGonaz_Excalibur";
        public override string PrefabName => "ChebGonaz_Excalibur.prefab";
        public override string NameLocalization => "$chebgonaz_excalibur";
        public override string DescriptionLocalization => "$chebgonaz_excalibur_desc";

        public static ConfigEntry<float> Knockback,
            BackstabBonus,
            SlashDamage,
            SpiritDamage,
            BonusSlashDamagePerLevel,
            BonusSpiritDamagePerLevel,
            BlockPower,
            BlockPowerPerLevel,
            DeflectionForce,
            DeflectionForcePerLevel;

        public override void CreateConfigs(BaseUnityPlugin plugin)
        {
            Knockback = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "Knockback",
                50f, new ConfigDescription(
                    "Excalibur's base knockback value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BackstabBonus = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BackstabBonus",
                3f, new ConfigDescription(
                    "Excalibur's base backstab value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            SlashDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "SlashDamage",
                100f, new ConfigDescription(
                    "Excalibur's base slash damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            SpiritDamage = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "SpiritDamage",
                50f, new ConfigDescription(
                    "Excalibur's base spirit damage value.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusSlashDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "BonusSlashDamagePerLevel",
                20f, new ConfigDescription(
                    "Excalibur's slash damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusSpiritDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "BonusSpiritDamagePerLevel",
                0f, new ConfigDescription(
                    "Excalibur's spirit damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            BlockPower = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BlockPower",
                43f, new ConfigDescription(
                    "Excalibur's base blocking power.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BlockPowerPerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BlockPowerPerLevel",
                0f, new ConfigDescription(
                    "Excalibur's blocking power increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DeflectionForce = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "DeflectionForce",
                43f, new ConfigDescription(
                    "Excalibur's base deflection force.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DeflectionForcePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "DeflectionForcePerLevel",
                0f, new ConfigDescription(
                    "Excalibur's deflection force increase per level.", null,
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

            #region AttackSettings

            shared.m_attackForce = Knockback.Value;
            shared.m_backstabBonus = BackstabBonus.Value;
            shared.m_damages.m_slash = SlashDamage.Value;
            shared.m_damages.m_spirit = SpiritDamage.Value;
            shared.m_damagesPerLevel.m_slash = BonusSlashDamagePerLevel.Value;
            shared.m_damagesPerLevel.m_spirit = BonusSpiritDamagePerLevel.Value;

            #endregion

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
                            m_amountPerLevel = 40,
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
                            m_amountPerLevel = 20,
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
                            m_amountPerLevel = 13,
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
                            m_amountPerLevel = 9,
                        }
                    };
                    break;
                default: // initial state = 1, or unhandled case
                    Logger.LogWarning(
                        $"Unhandled case in recipes (quality = {itemQuality} for ExcaliburItem), please tell Cheb.");
                    break;
            }
        }
    }
}