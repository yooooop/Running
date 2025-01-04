using JetBrains.Annotations;
using Running.Cards;
using Running.Game;
using Running.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Running.Cards
{

    public class OperationCardPrefab : MonoBehaviour
    {
        [SerializeField] private Image _operationImage;
        [SerializeField] private Button _button;


        [Inject] private GameController _gameController;

        private OperationType _operationType;
        private OperationCard _operationCard;

        public void Setup(OperationCard operationCard)
        {
            _operationImage.sprite = operationCard.OperationSprite;
            _operationType = operationCard.sign;
            _operationCard = operationCard;
        }

        [UsedImplicitly]
        public void CardSelected()
        {
            _gameController.OperationCardSelected(_operationCard);
        }

        public Button GetButton()
        {
            return _button;
        }

    }


}