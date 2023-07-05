using System.Collections.Generic;
using HarmonyLib;
using Jotunn.Managers;

namespace ChebsMythicalWeapons.Patches
{
    [HarmonyPatch(typeof(InventoryGui))]
    public class InventoryGuiPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(InventoryGui.SetRecipe))]
        static void SetRecipe(InventoryGui __instance, int index, bool center)
        {
            if (!__instance.InUpradeTab()) return;
            // Mintymintos wants same upgrade costs for both bow and sword
            var keyName = __instance.m_selectedRecipe.Key.ToString();
            if (keyName.Contains(ChebsMythicalWeapons.Excalibur.ItemName)
                || keyName.Contains(ChebsMythicalWeapons.ApolloBow.ItemName))
            {
                var itemQuality = __instance.m_selectedRecipe.Value.m_quality;
                switch (itemQuality)
                        {
                            case 1:
                                __instance.m_selectedRecipe.Key.m_resources = new[]
                                {
                                    new Piece.Requirement()
                                    {
                                        m_resItem = PrefabManager.Instance.GetPrefab("Iron").GetComponent<ItemDrop>(),
                                        m_amount = 40,
                                        m_amountPerLevel = 40,
                                    }
                                };
                                break;
                            case 2:
                                __instance.m_selectedRecipe.Key.m_resources = new[]
                                {
                                    new Piece.Requirement()
                                    {
                                        m_resItem = PrefabManager.Instance.GetPrefab("Silver").GetComponent<ItemDrop>(),
                                        m_amount = 40,
                                        m_amountPerLevel = 20,
                                    }
                                };
                                break;
                            case 3:
                                __instance.m_selectedRecipe.Key.m_resources = new[]
                                {
                                    new Piece.Requirement()
                                    {
                                        m_resItem = PrefabManager.Instance.GetPrefab("BlackMetal").GetComponent<ItemDrop>(),
                                        m_amount = 40,
                                        m_amountPerLevel = 13,
                                    }
                                };
                                break;
                            default: // initial state = 1, or unhandled case
                                Jotunn.Logger.LogWarning($"Unhandled case in recipes (quality = {itemQuality} for Excalibur), please tell Cheb.");
                                break;
                        }
            }
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(InventoryGui.UpdateRecipeList))]
        static void UpdateRecipeList(InventoryGui __instance, ref List<Recipe> recipes)
        {
            // remove Excalibur from the crafting options if it is there
            if (__instance.InCraftTab())
            {
                recipes.RemoveAll(recipe =>
                {
                    var recipeAsStr = recipe.ToString();
                    return recipeAsStr.Contains(ChebsMythicalWeapons.Excalibur.ItemName)
                        || recipeAsStr.Contains(ChebsMythicalWeapons.ApolloBow.ItemName);
                });
            }
        }
    }
}