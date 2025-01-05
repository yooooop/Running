using JetBrains.Annotations;
using Running.BodyPart;
using Running.Cards;
using Running.Operations;
using Running.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;


namespace Running.Game
{

    public class MainGameScreen : MonoBehaviour
    {
        [SerializeField] private Transform _numberContent;
        [SerializeField] private Transform _operationContent;
        [SerializeField] private Transform _handContent;
        [SerializeField] private Transform _headContent;
        [SerializeField] private NumberCardPrefab _numberCardPrefab;
        [SerializeField] private OperationCardPrefab _operationCardPrefab;
        [SerializeField] private GameObject _wagerUI;
        [SerializeField] private List<NumberCard> _numberCardList;
        [SerializeField] private List<OperationCard> _operationCardList;
        [SerializeField] private DialogBox _dialogBox;

        [Inject] private GameController _gameController;
        [Inject] private DiContainer _container;
        [Inject] private PlayerData _playerData;

        [Inject]
        private void OnInjected()
        {
            
            _gameController.WagerStartedEvent += WagerStarted;
            _gameController.BodyPartCardSelectedEvent += WagerWagered;
            _gameController.GameStartedEvent += ResetUI;
            _gameController.SetDialogEvent += OnMessageDisplay;
            
        }

        private void OnEnable()
        {

            _gameController.RealGameStart();
            
            _gameController.StartNextPhase();
        }

        private void DistributeOperationCards()
        {
            System.Random rng = new System.Random();
            int n = _operationCardList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                OperationCard temp = _operationCardList[k];
                _operationCardList[k] = _operationCardList[n];
                _operationCardList[n] = temp;
            }

            foreach (OperationCard card in _operationCardList)
            {
                _gameController.AddToOperationList(card);
            }
            _gameController.CardListCompletedEvent(false);
        }

        private void DistributeNumberCards()
        {
            System.Random rng = new System.Random();
            int n = _numberCardList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                NumberCard temp = _numberCardList[k];
                _numberCardList[k] = _numberCardList[n];
                _numberCardList[n] = temp;
            }

            foreach (NumberCard card in _numberCardList)
            {
                _gameController.AddToNumberList(card);
            }
            _gameController.CardListCompletedEvent(true);
        }
        

        private void WagerStarted(object sender, EventArgs e)
        {
            _wagerUI.SetActive(true);
        }

        private void WagerWagered(object sender, List<BodyPartType> bodyPartTypeList)
        {
            //Debug.LogError("body part type: " + bodyPartType);
            _wagerUI.SetActive(false);

        }

        private void ResetUI(object sender, bool test)
        {
            _gameController.ResetCards();
            DistributeOperationCards();
            DistributeNumberCards();
        }

        private void OnMessageDisplay(object sender, string text)
        {
            _dialogBox.gameObject.SetActive(true);
            _dialogBox.Setup(text);
        }
        
    }
}