using ChebsValheimLibrary.Common;
using ChebsValheimLibrary.Items;
using Jotunn.Configs;
using Jotunn.Entities;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsSwordInTheStone.Items
{
    public class ExcaliburItem : Item
    {
        public override string ItemName => "ChebGonaz_Excalibur";
        public override string PrefabName => "ChebGonaz_Excalibur.prefab";

        public override string NameLocalization => "$chebgonaz_excalibur";
        public override string DescriptionLocalization => "$chebgonaz_excalibur_desc";
        
        public override CustomItem GetCustomItemFromPrefab(GameObject prefab)
        {
            var config = new ItemConfig
            {
                Name = NameLocalization,
                Description = DescriptionLocalization,
                CraftingStation = InternalName.GetName(CraftingTable.Forge),
                Requirements = new []
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

            return customItem;
        }
    }
}