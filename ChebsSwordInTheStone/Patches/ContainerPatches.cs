using System.Collections.Generic;
using ChebsSwordInTheStone.Items;
using ChebsSwordInTheStone.Structures;
using HarmonyLib;
using Jotunn.Managers;

namespace ChebsSwordInTheStone.Patches
{
    [HarmonyPatch(typeof(Container))]
    public class ContainerPatches
    {
        [HarmonyPatch(nameof(Container.Interact))]
        [HarmonyPrefix]
        static bool Interact(Container __instance, Humanoid character, bool hold, bool alt)
        {
            if (!__instance.TryGetComponent(out SwordInTheStone swordInTheStone)) return true; // permit base method completion

            if (swordInTheStone.AlreadyLooted)
            {
                character.Message(MessageHud.MessageType.Center, "$chebgonaz_swordinthestone_empty");
                return false; // deny base method completion
            }

            var playerId = Game.instance.GetPlayerProfile().GetPlayerID();

            var player = Player.GetPlayer(playerId);
            if (player == null)
            {
                Jotunn.Logger.LogError("SwordInTheStone: Failed to get player");
                return false; // deny base method completion
            }

            if (player.GetSkillLevel(Skills.SkillType.Swords) < 100)
            {
                character.Message(MessageHud.MessageType.Center, "$chebgonaz_swordinthestone_unworthy");
                return false; // deny base method completion
            }
            
            // add the sword to the container
            var sword = ItemManager.Instance.GetItem(ChebsSwordInTheStone.Excalibur.ItemName);
            __instance.m_inventory.AddItem(sword.ItemDrop.m_itemData);

            return true; // permit base method completion
        }
        
        [HarmonyPatch(nameof(Container.Save))]
        [HarmonyPostfix]
        static void Save(Container __instance)
        {
            // If the sword has been taken, note that it has been looted
            if (!__instance.TryGetComponent(out SwordInTheStone swordInTheStone)) return;
            
            var sword = swordInTheStone.m_inventory.GetAllItems()
                .Find(item => item.m_shared.m_name.Equals(ChebsSwordInTheStone.Excalibur.NameLocalization));
            
            if (sword == null) swordInTheStone.AlreadyLooted = true;
        }
    }
}