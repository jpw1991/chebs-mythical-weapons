using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsSwordInTheStone.Locations
{
    public class SwordInTheStoneLocation : MonoBehaviour
    {
        public const string PrefabName = "ChebGonaz_SwordInTheStone.prefab";
        public const string NameLocalization = "$chebgonaz_swordinthestone";

        private void Awake()
        {
            Logger.LogInfo($"Awakening at {transform.position}");

            if (!ChebsSwordInTheStone.ShowMapMarker.Value) return;
            
            Minimap.instance.AddPin(transform.position,
                ChebsSwordInTheStone.MapMarker.Value, 
                ChebsSwordInTheStone.Localization.TryTranslate(NameLocalization), 
                true, false,
                Game.instance.GetPlayerProfile().GetPlayerID());
        }
    }
}