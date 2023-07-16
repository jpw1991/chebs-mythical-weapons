using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsMythicalWeapons.Locations
{
    public class SwordInTheStoneLocation : MonoBehaviour
    {
        public const string PrefabName = "ChebGonaz_SwordInTheStone.prefab";
        public const string NameLocalization = "$chebgonaz_swordinthestone";

        private void Awake()
        {
            Logger.LogInfo($"Awakening at {transform.position}");

            if (!ChebsMythicalWeapons.ShowMapMarker.Value) return;

            Minimap.instance.AddPin(transform.position,
                ChebsMythicalWeapons.MapMarker.Value,
                ChebsMythicalWeapons.Localization.TryTranslate(NameLocalization),
                true, false,
                Game.instance.GetPlayerProfile().GetPlayerID());
        }
    }
}