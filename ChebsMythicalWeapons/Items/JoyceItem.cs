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
            shared.m_damages.m_fire = FireDamage.Value;
            shared.m_damagesPerLevel.m_slash = BonusSlashDamagePerLevel.Value;
            shared.m_damagesPerLevel.m_fire = BonusFireDamagePerLevel.Value;
            shared.m_toolTier = ToolTier.Value;
            shared.m_damages.m_chop = ChopDamage.Value;
            shared.m_damagesPerLevel.m_chop = BonusChopDamagePerLevel.Value;

            #endregion

            #region ShieldSettings

            shared.m_blockPower = BlockPower.Value; // block force
            shared.m_blockPowerPerLevel = BlockPowerPerLevel.Value;
            shared.m_deflectionForce = DeflectionForce.Value;
            shared.m_deflectionForcePerLevel = DeflectionForcePerLevel.Value;

            #endregion
        }
        
        public static void HandleUpgradesForSelectedRecipe(KeyValuePair<Recipe,ItemDrop.ItemData> selectedRecipe)
        {
            var itemQuality = selectedRecipe.Value.m_quality;
            switch (itemQuality)
            {
                case 1:
                    selectedRecipe.Key.m_resources = new[]
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
                    selectedRecipe.Key.m_resources = new[]
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
                    selectedRecipe.Key.m_resources = new[]
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
                    selectedRecipe.Key.m_resources = new[]
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