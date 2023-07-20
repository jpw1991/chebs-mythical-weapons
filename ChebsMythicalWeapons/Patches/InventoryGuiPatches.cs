using System.Collections.Generic;
using ChebsMythicalWeapons.Items;
using ChebsValheimLibrary.Common;
using HarmonyLib;
using Jotunn;
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
                        Logger.LogWarning(
                            $"Unhandled case in recipes (quality = {itemQuality} for Excalibur), please tell Cheb.");
                        break;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(InventoryGui.UpdateRecipeList))]
        static void UpdateRecipeList(InventoryGui __instance, ref List<Recipe> recipes)
        {
            // remove Excalibur from the crafting options if it is there
            // --------
            // Shawesome wants the blade only craftable during a Thunderstorm. There's a couple of potential ways to go
            // about doing this (as far as my own understanding of things goes):
            //
            // a) List it as a recipe normally, then make a boolean patch on InventoryGui.OnCraftPressed and block
            //    and show a message like "Can only craft during thunderstorm"
            // b) Only add it as a recipe if it is currently a thunderstorm
            //
            // I think A is the nicer option, but will also conflict more with other mods. So for that reason, I
            // decided to go with option B and remove it if there's no thunderstorm currently active.
            var currentEnvironment = EnvMan.instance.GetCurrentEnvironment();
            var thunderstormActive = BladeOfOlympusItem.CraftingWeatherCondition.Value == Weather.Env.None
                                     || currentEnvironment.m_name ==
                                     InternalName.GetName(BladeOfOlympusItem.CraftingWeatherCondition.Value);
            if (__instance.InCraftTab())
            {
                recipes.RemoveAll(recipe =>
                {
                    var recipeAsStr = recipe.ToString();
                    return recipeAsStr.Contains(ChebsMythicalWeapons.Excalibur.ItemName)
                           || recipeAsStr.Contains(ChebsMythicalWeapons.ApolloBow.ItemName)
                           || recipeAsStr.Contains(ChebsMythicalWeapons.Joyce.ItemName)
                           || (!thunderstormActive &&
                               (recipeAsStr.Contains(ChebsMythicalWeapons.BladeOfOlympus.ItemName)
                               || recipeAsStr.Contains(ChebsMythicalWeapons.GreatswordOfOlympus.ItemName)));
                });
            }
        }
    }
}