using JetBrains.Annotations;
using Running.Cards;
using Running.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Running.Cards 
{

    public class NumberCardPrefab : MonoBehaviour
    {
        [SerializeField] private Image _numberImage;
        [SerializeField] private Button _button;

        [Inject] private GameController _gameController;

        private int _value;
        private NumberCard _numberCard;

        public void Setup(NumberCard numberCard)
        {
            _numberImage.sprite = numberCard.NumberSprite;
            _value = numberCard.value;
            _numberCard = numberCard;
        }

        [UsedImplicitly]
        public void CardSelected()
        {
            _gameController.NumberCardSelected(_numberCard);
        }

        public Button GetButton()
        {
            return _button;
        }

    }
}
