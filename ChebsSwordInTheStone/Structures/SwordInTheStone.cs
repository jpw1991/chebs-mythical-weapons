using Jotunn.Configs;
using Jotunn.Entities;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsSwordInTheStone.Structures
{
    public class SwordInTheStone : Container
    {
        public const string PrefabName = "ChebGonaz_SwordInTheStone.prefab";
        
        public const string NameLocalization = "$chebgonaz_swordinthestone";
        public const string DescriptionLocalization = "$chebgonaz_swordinthestone_desc";

        public const string IconName = "chebgonaz_swordinthestone_icon.png";
        
        public static CustomPiece GetCustomPieceFromPrefab(GameObject prefab, Sprite icon)
        {
            var config = new PieceConfig
            {
                Name = NameLocalization,
                Description = DescriptionLocalization,
                Icon = icon
            };

            var customPiece = new CustomPiece(prefab, false, config);
            if (customPiece.PiecePrefab == null)
            {
                Logger.LogError($"AddCustomPieces: {PrefabName}'s PiecePrefab is null!");
                return null;
            }

            // not player craftable
            config.Enabled = false;

            return customPiece;
        }
        
        public new bool Interact(Humanoid character, bool hold, bool alt)
        {
            if (!TryGetComponent(out ZNetView zNetView))
            {
                Logger.LogError("SwordInTheStone: Failed to get ZNetView");
                return true;
            }
            
            var playerId = Game.instance.GetPlayerProfile().GetPlayerID();

            var player = Player.GetPlayer(playerId);
            if (player == null)
            {
                Logger.LogError("SwordInTheStone: Failed to get player");
                return true;
            }

            if (player.GetSkillLevel(Skills.SkillType.Swords) < 100)
            {
                character.Message(MessageHud.MessageType.Center, "$chebgonaz_swordinthestone_unworthy");
                return true;
            }
            
            zNetView.InvokeRPC("RequestOpen", playerId);
            return true;
        }
    }
}