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

        public const string AlreadyLootedZDOKey = "ChebGonaz_ExcaliburLooted";
        
        public static CustomPiece GetCustomPieceFromPrefab(GameObject prefab, Sprite icon)
        {
            var config = new PieceConfig
            {
                Name = NameLocalization,
                Description = DescriptionLocalization,
                Icon = icon,
                PieceTable = "_HammerPieceTable"
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
        
        public bool AlreadyLooted
        {
            // store in the ZDO whether the sword has been looted or not
            get => !TryGetComponent(out ZNetView zNetView) || zNetView.GetZDO().GetBool(AlreadyLootedZDOKey);
            set
            {
                if (TryGetComponent(out ZNetView zNetView))
                {
                    zNetView.GetZDO().Set(AlreadyLootedZDOKey, value);
                }
                else
                {
                    Logger.LogError($"Cannot AlreadyLootedZDOKey to {value} because it has no ZNetView component.");
                }
            }
        }
    }
}