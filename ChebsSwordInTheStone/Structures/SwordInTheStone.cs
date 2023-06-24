using UnityEngine;
using Logger = Jotunn.Logger;

namespace ChebsSwordInTheStone.Structures
{
    public class SwordInTheStone : MonoBehaviour
    {
        public const string PrefabName = "ChebGonaz_SwordInTheStone.prefab";
        
        public const string NameLocalization = "$chebgonaz_swordinthestone";

        public const string AlreadyLootedZdoKey = "ChebGonaz_ExcaliburLooted";

        private Container _container;
        private GameObject _visual;

        private void Awake()
        {
            _container = gameObject.AddComponent<Container>();
            _visual = transform.Find("New/Excalibur").gameObject;
        }

        public void Start()
        {
            _container.m_name = NameLocalization;
        }
        
        private void FixedUpdate()
        {
            UpdateSword();
        }
        
        public void UpdateSword()
        {
            _visual.SetActive(!AlreadyLooted);
        }

        public bool AlreadyLooted
        {
            // store in the ZDO whether the sword has been looted or not
            get => !TryGetComponent(out ZNetView zNetView) || zNetView.GetZDO().GetBool(AlreadyLootedZdoKey);
            set
            {
                if (TryGetComponent(out ZNetView zNetView))
                {
                    zNetView.GetZDO().Set(AlreadyLootedZdoKey, value);
                }
                else
                {
                    Logger.LogError($"Cannot AlreadyLootedZDOKey to {value} because it has no ZNetView component.");
                }
            }
        }
    }
}