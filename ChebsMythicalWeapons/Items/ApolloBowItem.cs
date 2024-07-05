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
    public class ApolloBowItem : Item
    {
        public override string ItemName => "ChebGonaz_BowOfApollo";
        public override string PrefabName => "ChebGonaz_BowOfApollo.prefab";
        public override string NameLocalization => "$chebgonaz_bowofapollo";
        public override string DescriptionLocalization => "$chebgonaz_bowofapollo_desc";

        public static ConfigEntry<float> ProjectileGravity,
            ProjectileVelocity, ProjectileVelocityMin,
            ProjectileAccuracy, ProjectileAccuracyMin,
            Knockback, BackstabBonus,
            PiercingDamage, FireDamage,
            BonusPiercingDamagePerLevel, BonusFireDamagePerLevel;

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
            BonusPiercingDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)",
                "BonusPiercingDamagePerLevel",
                10f, new ConfigDescription(
                    "Bow's piercing damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BonusFireDamagePerLevel = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "BonusFireDamagePerLevel",
                0f, new ConfigDescription(
                    "Bow's fire damage increase per level.", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));

            ProjectileGravity = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "ProjectileGravity",
                5f, new ConfigDescription(
                    "Replace projectile gravity for projectiles leaving the bow to manipulate the drop. For an arrow that flies straight and true, use 0; for default arrow behaviour, use 5. This, along with projectile velocity, influences the drop of the arrow as it flies.",
                    null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            ProjectileVelocity = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "ProjectileVelocity",
                60f, new ConfigDescription(
                    "Bow's projectile velocity value. This is the speed of the projectile when it leaves the bow. This, along with projectile gravity, influences the drop of the arrow as it flies.",
                    null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            ProjectileVelocityMin = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "ProjectileVelocityMin",
                2f, new ConfigDescription(
                    "Bow's projectile velocity min value (I don't know what this does).",
                    null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            ProjectileAccuracy = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "ProjectileAccuracy",
                0f, new ConfigDescription(
                    "Bow's projectile accuracy value (I don't know what this does).",
                    null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            ProjectileAccuracyMin = plugin.Config.Bind($"{GetType().Name} (Server Synced)", "ProjectileAccuracyMin",
                20f, new ConfigDescription(
                    "Bow's projectile accuracy min value (I don't know what this does).",
                    null,
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

            var itemDataShared = customItem.ItemDrop.m_itemData.m_shared;
            
            itemDataShared.m_maxQuality = 5;

            #region AttackSettings

            itemDataShared.m_attackForce = Knockback.Value;
            itemDataShared.m_backstabBonus = BackstabBonus.Value;
            itemDataShared.m_damages.m_pierce = PiercingDamage.Value;
            itemDataShared.m_damages.m_fire = FireDamage.Value;
            itemDataShared.m_damagesPerLevel.m_pierce = BonusPiercingDamagePerLevel.Value;
            itemDataShared.m_damagesPerLevel.m_fire = BonusFireDamagePerLevel.Value;

            itemDataShared.m_attack.m_projectileVel = ProjectileVelocity.Value;
            itemDataShared.m_attack.m_projectileVelMin = ProjectileVelocityMin.Value;
            
            itemDataShared.m_attack.m_projectileAccuracy = ProjectileAccuracy.Value;
            itemDataShared.m_attack.m_projectileAccuracyMin = ProjectileAccuracyMin.Value;

            #endregion

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
            var itemDataShared = item.m_itemData.m_shared;
            
            #region AttackSettings

            itemDataShared.m_attackForce = Knockback.Value;
            itemDataShared.m_backstabBonus = BackstabBonus.Value;
            itemDataShared.m_damages.m_pierce = PiercingDamage.Value;
            itemDataShared.m_damages.m_fire = FireDamage.Value;
            itemDataShared.m_damagesPerLevel.m_pierce = BonusPiercingDamagePerLevel.Value;
            itemDataShared.m_damagesPerLevel.m_fire = BonusFireDamagePerLevel.Value;

            itemDataShared.m_attack.m_projectileVel = ProjectileVelocity.Value;
            itemDataShared.m_attack.m_projectileVelMin = ProjectileVelocityMin.Value;
            
            itemDataShared.m_attack.m_projectileAccuracy = ProjectileAccuracy.Value;
            itemDataShared.m_attack.m_projectileAccuracyMin = ProjectileAccuracyMin.Value;

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
                            m_amountPerLevel = 40,
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
                            m_amountPerLevel = 20,
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
                            m_amountPerLevel = 13,
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
                            m_amountPerLevel = 9,
                        }
                    };
                    break;
                default: // initial state = 1, or unhandled case
                    Logger.LogWarning(
                        $"Unhandled case in recipes (quality = {itemQuality} for ApolloBowItem), please tell Cheb.");
                    break;
            }
        }
    }
}