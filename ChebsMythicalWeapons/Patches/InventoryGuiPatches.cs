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
            if (__instance == null)
            {
                Logger.LogError("instance is null");
                return;
            }
            if (!__instance.InUpradeTab()) return;
            // Mintymintos wants same upgrade costs for both bow and sword
            var selectedRecipeName = __instance.m_selectedRecipe.Recipe?.m_item?.m_itemData?.m_shared?.m_name;
            if (selectedRecipeName == null) return;
            if (selectedRecipeName.Contains(ChebsMythicalWeapons.Excalibur.ItemName))
            {
                var test = __instance.m_selectedRecipe;
                ExcaliburItem.HandleUpgradesForSelectedRecipe(__instance.m_selectedRecipe);
            }
            else if (selectedRecipeName.Contains(ChebsMythicalWeapons.ApolloBow.ItemName))
            {
                ApolloBowItem.HandleUpgradesForSelectedRecipe(__instance.m_selectedRecipe);
            }
            else if (selectedRecipeName.Contains(ChebsMythicalWeapons.Aegis.ItemName))
            {
                AegisItem.HandleUpgradesForSelectedRecipe(__instance.m_selectedRecipe);
            }
            else if (selectedRecipeName.Contains(ChebsMythicalWeapons.Joyce.ItemName))
            {
                JoyceItem.HandleUpgradesForSelectedRecipe(__instance.m_selectedRecipe);
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
            if (__instance == null)
            {
                Logger.LogError("instance is null");
                return;
            }
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