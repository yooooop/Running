using Running.Game;
using Running.GamePhase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Running.Cards
{

    public class CardClickHandler : MonoBehaviour
    {
        [SerializeField] private NumberCard _numberCard;
        [SerializeField] private OperationCard _operationCard;
        [SerializeField] private GameObject _card;

        [Inject] private GameController _gameController;

        public bool IsAbleToSelect = true;

        private void OnMouseDown()
        {
            if (!IsAbleToSelect)
            {
                return;
            }
            
            
            if (_numberCard != null)
            {
                Debug.Log($"{gameObject.name} clicked!");
                _card.SetActive(false);
                _gameController.NumberCardSelected(_numberCard);
            } 
            else if (_operationCard != null)
            {
                Debug.Log($"{gameObject.name} clicked!");
                _card.SetActive(false);
                _gameController.OperationCardSelected(_operationCard);
            }
        }

    }

}