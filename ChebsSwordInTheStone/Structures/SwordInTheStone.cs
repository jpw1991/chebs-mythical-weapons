using Jotunn.Configs;
using Jotunn.Entities;
using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsSwordInTheStone.Structures
{
    public class SwordInTheStone : MonoBehaviour
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
    }
}