using System.Collections.Generic;
using HarmonyLib;

namespace ChebsSwordInTheStone.Patches
{
    [HarmonyPatch(typeof(InventoryGui))]
    public class InventoryGuiPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(InventoryGui.UpdateRecipeList))]
        static void UpdateRecipeList(InventoryGui __instance, ref List<Recipe> recipes)
        {
            // remove Excalibur from the crafting options if it is there
            if (__instance.InCraftTab())
            {
                recipes.RemoveAll(recipe =>
                    recipe.ToString().Contains(ChebsSwordInTheStone.Excalibur.ItemName));    
            }
        }
    }
}