using ChebsSwordInTheStone.Pickables;
using Jotunn.Managers;
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
            
            // // spawn pickable and add it
            // var pickablePrefab = PrefabManager.Instance.GetPrefab(
            //     SwordInTheStonePickable.PickablePrefabName.Substring(0, SwordInTheStonePickable.PickablePrefabName.Length - 7));
            // Instantiate(pickablePrefab, transform);
            
            if (!ChebsSwordInTheStone.ShowMapMarker.Value) return;
            
            Minimap.instance.AddPin(transform.position,
                ChebsSwordInTheStone.MapMarker.Value, 
                ChebsSwordInTheStone.Localization.TryTranslate(NameLocalization), 
                true, false,
                Game.instance.GetPlayerProfile().GetPlayerID());
        }

        // to do: make map marker work
        // private void Awake()
        // {
        //     MinimapManager.OnVanillaMapDataLoaded += CreateMapDrawing;
        // }
        //
        // private void CreateMapDrawing()
        // {
        //     if (!ChebsSwordInTheStone.ShowMapMarker.Value) return;
        //     
        //     var pinOverlay = MinimapManager.Instance.GetMapDrawing("PinOverlay");
        //     var pos = MinimapManager.Instance.WorldToOverlayCoords(transform.position, pinOverlay.TextureSize);
        //     Minimap.instance.AddPin(pos, Minimap.PinType.Boss,
        //         ChebsSwordInTheStone.Localization.TryTranslate(NameLocalization), true, true,
        //         Player.m_localPlayer.GetPlayerID());
        //     Minimap.instance.UpdatePins();
        //     
        //     Minimap.instance.CreateMapNamePin(Minimap.instance.AddPin(pos, Minimap.PinType.Boss,
        //         ChebsSwordInTheStone.Localization.TryTranslate(NameLocalization), true, true,
        //         Player.m_localPlayer.GetPlayerID()), new RectTransform());
        //     
        //     pinOverlay.MainTex.Apply();
        //     pinOverlay.FogFilter.Apply();
        //     pinOverlay.ForestFilter.Apply();
        //     pinOverlay.HeightFilter.Apply();
        // }
    }
}