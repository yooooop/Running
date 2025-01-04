using JetBrains.Annotations;
using Running.Game;
using Running.Operations;
using Running.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Running.BodyPart
{
    public class BodyPartWagerUI : MonoBehaviour
    {
        [SerializeField] List<BodyPartScriptableObject> _bodyPartScriptableObjectList;
        [SerializeField] BodyPartWagerSlot _bodyPartWagerSlot;
        [SerializeField] Transform _spawnParent;
        [SerializeField] GameObject _lungUI;

        [Inject] private DiContainer _container;
        [Inject] private GameController _gameController;
        [Inject] private PlayerData _playerData;

        private List<BodyPartType> _bodyPartList = new List<BodyPartType>();
        private List<BodyPartWagerSlot> _bodyPartSlotList = new List<BodyPartWagerSlot>();

        private void OnEnable()
        {
            foreach (BodyPartWagerSlot slot in _bodyPartSlotList)
            {
                Destroy(slot.gameObject);
            }
            _bodyPartSlotList.Clear();

            foreach (BodyPartScriptableObject bodyPart in _bodyPartScriptableObjectList)
            {
                BodyPartWagerSlot slot = _container.InstantiatePrefabForComponent<BodyPartWagerSlot>(_bodyPartWagerSlot, _spawnParent);
                slot.Setup(bodyPart);
                slot.BodyPartWageredEvent += AddToList;
                _bodyPartSlotList.Add(slot);
            }
        }

        private void OnDisable()
        {
            foreach (BodyPartWagerSlot slot in _bodyPartSlotList)
            {
                slot.BodyPartWageredEvent -= AddToList;
            }
        }

        public void AddToList(object sender, BodyPartType bodyPartType)
        {
            _bodyPartList.Add(bodyPartType);
            _gameController.OrganWagered(bodyPartType);
            if (bodyPartType == BodyPartType.Lung)
            {
                _lungUI.SetActive(true);
            }
        }

        [UsedImplicitly]
        public void WagerParts()
        {
            _gameController.SetBodyPart(_bodyPartList);
            _gameController.Wagered();
            
        }

        [UsedImplicitly]
        public void SelectedRefresh(string refresh)
        {
            _lungUI.SetActive(false);
            _gameController.RefreshCard(refresh);
        }
    }
}