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
    public class AegisItem : Item
    {
        public override string ItemName => "ChebGonaz_Aegis";
        public override string PrefabName => "ChebGonaz_Aegis.prefab";
        public override string NameLocalization => "$chebgonaz_aegis";
        public override string DescriptionLocalization => "$chebgonaz_aegis_desc";

        public static ConfigEntry<float> BlockPower,
            BlockPowerPerLevel,
            DeflectionForce,
            DeflectionForcePerLevel;

        public override void CreateConfigs(BaseUnityPlugin plugin)
        {
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

        public override CustomItem GetCustomItemFromPrefab(GameObject prefab)
        {
            var config = new ItemConfig
            {
                Name = NameLocalization,
                Description = DescriptionLocalization,
                CraftingStation = InternalName.GetName(CraftingTable.Forge),
                Requirements = new[]
                {
                    // add an upgrade amount of 20 silver per level of Aegis
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