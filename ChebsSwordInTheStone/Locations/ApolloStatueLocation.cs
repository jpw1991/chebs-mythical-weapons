using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsSwordInTheStone.Locations
{
    public class ApolloStatueLocation : MonoBehaviour
    {
        public const string PrefabName = "ChebGonaz_ApolloStatueLocation.prefab";
        public const string NameLocalization = "$chebgonaz_apollostatue";

        private void Awake()
        {
            Logger.LogInfo($"Awakening at {transform.position}");

            if (!ChebsSwordInTheStone.ApolloBowShowMapMarker.Value) return;
            
            Minimap.instance.AddPin(transform.position,
                ChebsSwordInTheStone.ApolloBowMapMarker.Value, 
                ChebsSwordInTheStone.Localization.TryTranslate(NameLocalization), 
                true, false,
                Game.instance.GetPlayerProfile().GetPlayerID());
        }
    }
}