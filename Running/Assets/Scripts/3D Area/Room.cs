using Cysharp.Threading.Tasks;
using Running.Ai;
using Running.Cards;
using Running.Game;
using Running.Operations;
using Running.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Running.Room
{

    public class Room : MonoBehaviour
    {
        [SerializeField] Transform _table;
        [SerializeField] Transform _playerHand;
        [SerializeField] Transform _opponentHand;
        [SerializeField] Transform _playerOperationHead;
        [SerializeField] Transform _opponentOperationHead;
        [SerializeField] DeadManPrefab _playerDeadMan;
        [SerializeField] DeadManPrefab _opponentDeadMan;
        [SerializeField] GameObject _playerWinningMan;
        [SerializeField] GameObject _opponentWinningMan;
        [SerializeField] GameObject _playerEndChair;
        [SerializeField] GameObject _opponentEndChair;
        [SerializeField] GameObject _playerGameChair;
        [SerializeField] GameObject _opponentGameChair;
        

        private Vector3 _playerNumberCardPosition = new Vector3(-3.75f, 0.024f, -3.53f);
        private Vector3 _playerOperationCardPosition = new Vector3(-1.99f, 0.024f, -2.11f);
        private Vector3 _opponentNumberCardPosition = new Vector3(-3.75f, 0.024f, 3.44f);
        private Vector3 _opponentOperationCardPosition = new Vector3(-1.99f, 0.024f, 1.92f);
        private float _numberCardSpacing = 0.9f;
        private List<NumberCard> _numberCardList = new List<NumberCard>();
        private List<OperationCard> _operationCardList = new List<OperationCard>();
        private List<NumberCardObjectPrefab> _playerNumberCardObjectList = new List<NumberCardObjectPrefab>();
        private List<NumberCardObjectPrefab> _opponentNumberCardObjectList = new List<NumberCardObjectPrefab>();
        private List<OperationCardObjectPrefab> _playerOperationCardObjectList = new List<OperationCardObjectPrefab>();
        private List<OperationCardObjectPrefab> _opponentOperationCardObjectList = new List<OperationCardObjectPrefab>();
        private List<NumberCardObjectPrefab> _playerHands = new List<NumberCardObjectPrefab>();
        private List<OperationCardObjectPrefab> _playerHeads = new List<OperationCardObjectPrefab>();
        private List<NumberCardObjectPrefab> _opponentHands = new List<NumberCardObjectPrefab>();
        private List<OperationCardObjectPrefab> _opponentHeads = new List<OperationCardObjectPrefab>();

        [Inject] GameController _gameController;
        [Inject] AiController _aiController;
        [Inject] AnimatorController _animatorController;
        [Inject] private DiContainer _container;
        [Inject] private PlayerData _playerData;
        [Inject] private OpponentData _opponentData;

        [Inject]
        private void OnInjected()
        {
            _gameController.NumberCardListEvent += InstantiateNumberCards;
            _gameController.OperationCardListEvent += InstantiateOperationCards;
            _gameController.NumberCardSelectedEvent += NumberCardSelected;
            _gameController.OperationCardSelectedEvent += OperationCardSelected;
            _animatorController.AnimationPickedUpNumberEvent += OpponentNumberCadSelected;
            _animatorController.AnimationPickedUpOperationEvent += OpponentOperationCardSelected;
            _gameController.ResetRoundEvent += ResetHands;
            _gameController.RefreshNumberCardEvent += RefreshNumber;
            _gameController.RefreshOperationCardEvent += RefreshOperation;
            _aiController.OpponentNumberRefreshEvent += RefreshOpponentNumber;
            _aiController.OpponentOperationRefreshEvent += RefreshOpponentOperation;
            _gameController.HeartUsedEvent += MultiplierSet;
            _gameController.ResetAllCardsEvent += ResetTableCards;
            _gameController.FlashCardsEvent += FlashCards;
            _gameController.GameFinishedEvent += GameEnded;
            _gameController.RealGameStartedEvent += GameStarted;
        }
        private void InstantiateNumberCards(object sender, List<NumberCard> list)
        {
            
            // instantiate player cards
            foreach (NumberCard card in list)
            {
                NumberCardObjectPrefab numberCardInstance = _container.InstantiatePrefabForComponent<NumberCardObjectPrefab>(card.NumberCardPrefab, _table);

                Vector3 cardPosition = _table.TransformPoint(_playerNumberCardPosition);

                numberCardInstance.transform.position = cardPosition;
                numberCardInstance.transform.rotation = Quaternion.Euler(180, 0, 0);

                _playerNumberCardObjectList.Add(numberCardInstance);

                _playerNumberCardPosition.x += 0.75f;

                numberCardInstance.CardHighlight.IsAbleToSelect = false;
            }

            // instantiate opponent cards
            foreach (NumberCard card in list)
            {
                NumberCardObjectPrefab numberCardInstance = _container.InstantiatePrefabForComponent<NumberCardObjectPrefab>(card.NumberCardPrefab, _table);

                Vector3 cardPosition = _table.TransformPoint(_opponentNumberCardPosition);

                numberCardInstance.transform.position = cardPosition;
                numberCardInstance.transform.rotation = Quaternion.Euler(180, 0, 0);

                numberCardInstance.CardHighlight.IsAbleToSelect = false;
                numberCardInstance.CardClickHandler.IsAbleToSelect = false;

                _opponentNumberCardObjectList.Add(numberCardInstance);

                _opponentNumberCardPosition.x += 0.75f;

                numberCardInstance.CardHighlight.IsAbleToSelect = false;
            }

            _numberCardList = list;
        }


        private void InstantiateOperationCards(object sender, List<OperationCard> list)
        {

            // instantiate player cards
            foreach (OperationCard card in list)
            {
                OperationCardObjectPrefab operationCardInstance = _container.InstantiatePrefabForComponent<OperationCardObjectPrefab>(card.OperationCardPrefab, _table);

                Vector3 cardPosition = _table.TransformPoint(_playerOperationCardPosition);

                operationCardInstance.transform.position = cardPosition;
                operationCardInstance.transform.rotation = Quaternion.Euler(180, 0, 0);

                _playerOperationCardObjectList.Add(operationCardInstance);

                _playerOperationCardPosition.x += 1f;

                operationCardInstance.CardHighlight.IsAbleToSelect = false;
            }

            // instantiate opponent cards
            foreach (OperationCard card in list)
            {
                OperationCardObjectPrefab operationCardInstance = _container.InstantiatePrefabForComponent<OperationCardObjectPrefab>(card.OperationCardPrefab, _table);

                Vector3 cardPosition = _table.TransformPoint(_opponentOperationCardPosition);

                operationCardInstance.transform.position = cardPosition;
                operationCardInstance.transform.rotation = Quaternion.Euler(180, 0, 0);

                operationCardInstance.CardHighlight.IsAbleToSelect = false;
                operationCardInstance.CardClickHandler.IsAbleToSelect = false;

                _opponentOperationCardObjectList.Add(operationCardInstance);

                _opponentOperationCardPosition.x += 1f;

                operationCardInstance.CardHighlight.IsAbleToSelect = false;
            }

            _operationCardList = list;
        }


        private void NumberCardSelected(object sender, NumberCard numberCard)
        {
            NumberCardObjectPrefab instance = _container.InstantiatePrefabForComponent<NumberCardObjectPrefab>(numberCard.NumberCardPrefab, _playerHand);


            instance.CardHighlight.IsAbleToSelect = false;
            instance.CardClickHandler.IsAbleToSelect = false;

            if (_playerData.FirstCard == -2)
            {
                _playerData.FirstCard = numberCard.value;
                instance.transform.localPosition = new Vector3(-0.34f, -2.06f, 0f);
                instance.transform.localRotation = Quaternion.Euler(64.1f, 163.3f, 343.3f);
            }
            else
            {
                _playerData.SecondCard = numberCard.value;
                instance.transform.localPosition = new Vector3(0.27f, -2.06f, 0f);
                instance.transform.localRotation = Quaternion.Euler(64.1f, 205.39f, 16.7f);
            }

            _playerHands.Add(instance);

            _gameController.SelectedNumber();
        }

        private void OperationCardSelected(object sender, OperationCard operationCard)
        {
            _playerData.OperationType = operationCard.sign;
            OperationCardObjectPrefab instance = _container.InstantiatePrefabForComponent<OperationCardObjectPrefab>(operationCard.OperationCardPrefab, _playerOperationHead);

            instance.CardHighlight.IsAbleToSelect = false;
            instance.CardClickHandler.IsAbleToSelect = false;

            instance.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            _playerHeads.Add(instance);
            _gameController.StartNextPhase();
        }

        private void OpponentNumberCadSelected(object sender, int number)
        {
            foreach (NumberCard card in _numberCardList)
            {
                if (card.value == number)
                {
                    int index = _numberCardList.IndexOf(card);
                    _opponentNumberCardObjectList[index].gameObject.SetActive(false);

                    NumberCardObjectPrefab instance = _container.InstantiatePrefabForComponent<NumberCardObjectPrefab>(card.NumberCardPrefab, _opponentHand);

                    instance.CardHighlight.IsAbleToSelect = false;
                    instance.CardClickHandler.IsAbleToSelect = false;
                    if (_opponentData.SecondCard == -2)
                    {
                        instance.transform.localPosition = new Vector3(0.15f, 0.14f, 0);
                        instance.transform.localRotation = Quaternion.Euler(116.4f, 126f, -218.3f);
                    }
                    else
                    {
                        instance.transform.localPosition = new Vector3(-0.37f, 0.14f, 0);
                        instance.transform.localRotation = Quaternion.Euler(114.9f, 178.2f, -195f);
                    }
                    _opponentHands.Add(instance);

                    break;
                }
            }
        }

        private void OpponentOperationCardSelected(object sender, OperationType operationType)
        {
            foreach (OperationCard card in _operationCardList)
            {
                if (card.sign == operationType)
                {
                    int index = _operationCardList.IndexOf(card);
                    _opponentOperationCardObjectList[index].gameObject.SetActive(false);

                    OperationCardObjectPrefab instance = _container.InstantiatePrefabForComponent<OperationCardObjectPrefab>(card.OperationCardPrefab, _opponentOperationHead);

                    instance.CardHighlight.IsAbleToSelect = false;
                    instance.CardClickHandler.IsAbleToSelect = false;

                    instance.transform.localPosition = new Vector3(-0.3f, 0.4f, 0f);
                    instance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                    _opponentHeads.Add(instance);
                    break;
                }
            }
        }

        private void ResetTableCards(object sender, EventArgs e)
        {
            foreach (NumberCardObjectPrefab numbers in _playerNumberCardObjectList)
            {
                numbers.Multiplier.SetActive(false);
                Destroy(numbers.gameObject);
            }

            foreach (NumberCardObjectPrefab numbers in _opponentNumberCardObjectList)
            {
                numbers.Multiplier.SetActive(false);
                Destroy(numbers.gameObject);
            }

            foreach (OperationCardObjectPrefab numbers in _playerOperationCardObjectList)
            {
                Destroy(numbers.gameObject);
            }

            foreach (OperationCardObjectPrefab numbers in _opponentOperationCardObjectList)
            {
                Destroy(numbers.gameObject);
            }

            _numberCardList.Clear();
            _operationCardList.Clear();

            _playerNumberCardObjectList.Clear();
            _opponentNumberCardObjectList.Clear();

            _playerOperationCardObjectList.Clear();
            _opponentOperationCardObjectList.Clear();

            _playerNumberCardPosition = new Vector3(-3.75f, 0.024f, -3.53f);
            _playerOperationCardPosition = new Vector3(-1.99f, 0.024f, -2.11f);
            _opponentNumberCardPosition = new Vector3(-3.75f, 0.024f, 3.44f);
            _opponentOperationCardPosition = new Vector3(-1.99f, 0.024f, 1.92f);
        }

        private void ResetHands(object sender, EventArgs e)
        {
            foreach (NumberCardObjectPrefab numbers in _playerHands)
            {
                numbers.Multiplier.SetActive(false);
                Destroy(numbers.gameObject);
            }

            foreach (OperationCardObjectPrefab operations in _playerHeads)
            {
                Destroy(operations.gameObject);
            }
            _playerHands.Clear();
            _playerHeads.Clear();

            foreach (NumberCardObjectPrefab numbers in _opponentHands)
            {
                numbers.Multiplier.SetActive(false);
                Destroy(numbers.gameObject);
            }

            foreach (OperationCardObjectPrefab operations in _opponentHeads)
            {
                Destroy(operations.gameObject);
            }
            _opponentHands.Clear();
            _opponentHeads.Clear();
        }

        private void RefreshNumber(object sender, int num)
        {
            bool multiplierSet = false;
            foreach (NumberCardObjectPrefab numbers in _playerHands)
            {
                if (numbers.Multiplier.activeSelf)
                {
                    multiplierSet = true;
                }
                Destroy(numbers.gameObject);
            }
            _playerHands.Clear();

            foreach (NumberCard card in _numberCardList)
            {
                if (card.value == num)
                {
                    int index = _numberCardList.IndexOf(card);
                    _playerNumberCardObjectList[index].gameObject.SetActive(false);

                    NumberCardObjectPrefab instance = _container.InstantiatePrefabForComponent<NumberCardObjectPrefab>(card.NumberCardPrefab, _playerHand);

                    instance.Multiplier.SetActive(multiplierSet);
                    _playerData.FirstCard *= multiplierSet ? 2 : 1;

                    instance.CardHighlight.IsAbleToSelect = false;
                    instance.CardClickHandler.IsAbleToSelect = false;

                    instance.transform.localPosition = new Vector3(-0.34f, -2.06f, 0f);
                    instance.transform.localRotation = Quaternion.Euler(64.1f, 163.3f, 343.3f);

                    _playerHands.Add(instance);
                    break;
                }
            }

            
        }

        private void RefreshOperation(object sendder, OperationType operation)
        {
            foreach (OperationCardObjectPrefab operations in _playerHeads)
            {
                Destroy(operations.gameObject);
            }
            _playerHeads.Clear();

            foreach (OperationCard card in _operationCardList)
            {
                if (card.sign == operation)
                {
                    int index = _operationCardList.IndexOf(card);
                    _playerOperationCardObjectList[index].gameObject.SetActive(false);

                    OperationCardObjectPrefab instance = _container.InstantiatePrefabForComponent<OperationCardObjectPrefab>(card.OperationCardPrefab, _playerOperationHead);

                    instance.CardHighlight.IsAbleToSelect = false;
                    instance.CardClickHandler.IsAbleToSelect = false;

                    instance.transform.localPosition = new Vector3(0f, 0.4f, 0f);
                    instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                    _playerHeads.Add(instance);

                    break;
                }
            }

        }

        private void RefreshOpponentNumber(object sender, int num)
        {
            bool multiplierSet = false;
            foreach (NumberCardObjectPrefab numbers in _opponentHands)
            {
                if (numbers.Multiplier.activeSelf)
                {
                    multiplierSet = true;
                }
                Destroy(numbers.gameObject);
            }
            _opponentHands.Clear();

            foreach (NumberCard card in _numberCardList)
            {
                if (card.value == num)
                {
                    int index = _numberCardList.IndexOf(card);
                    _opponentNumberCardObjectList[index].gameObject.SetActive(false);


                    NumberCardObjectPrefab instance = _container.InstantiatePrefabForComponent<NumberCardObjectPrefab>(card.NumberCardPrefab, _opponentHand);

                    instance.Multiplier.SetActive(multiplierSet);
                    _opponentData.FirstCard *= multiplierSet ? 2 : 1;

                    instance.CardHighlight.IsAbleToSelect = false;
                    instance.CardClickHandler.IsAbleToSelect = false;

                    instance.transform.localPosition = new Vector3(0.15f, 0.14f, 0);
                    instance.transform.localRotation = Quaternion.Euler(116.4f, 126f, -218.3f);

                    _opponentHands.Add(instance);

                    break;
                }
            }

        }

        private void RefreshOpponentOperation(object sender, OperationType operation)
        {
            foreach (OperationCardObjectPrefab operations in _opponentHeads)
            {
                Destroy(operations.gameObject);
            }
            _opponentHeads.Clear();

            foreach (OperationCard card in _operationCardList)
            {
                if (card.sign == operation)
                {
                    int index = _operationCardList.IndexOf(card);
                    _opponentOperationCardObjectList[index].gameObject.SetActive(false);

                    OperationCardObjectPrefab instance = _container.InstantiatePrefabForComponent<OperationCardObjectPrefab>(card.OperationCardPrefab, _opponentOperationHead);

                    instance.CardHighlight.IsAbleToSelect = false;
                    instance.CardClickHandler.IsAbleToSelect = false;

                    instance.transform.localPosition = new Vector3(-0.3f, 0.4f, 0f);
                    instance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

                    _opponentHeads.Add(instance);
                }
            }

        }

        private void MultiplierSet(object sender, EventArgs e)
        {
            foreach (NumberCardObjectPrefab card in _playerHands)
            {
                card.Multiplier.SetActive(true);
            }
        }

        private void FlashCards(object sender, bool isNumber)
        {
            if (isNumber)
            {
                foreach(NumberCardObjectPrefab card in _playerNumberCardObjectList)
                {
                    FlashNumberCardAsync(card).Forget();
                }
            }
            else
            {
                foreach (OperationCardObjectPrefab card in _playerOperationCardObjectList)
                {
                    FlashOperationCardAsync(card).Forget();
                }
            }
            EnableCards(isNumber);
        }

        private async UniTaskVoid FlashNumberCardAsync(NumberCardObjectPrefab numberCard)
        {
            for (int i = 0; i < 6; i++)
            {
                numberCard.CardHighlight.ToggleOutline();
                await UniTask.Delay(200);
            }
        }

        private async UniTaskVoid FlashOperationCardAsync(OperationCardObjectPrefab operationCard)
        {
            for (int i = 0; i < 6; i++)
            {
                operationCard.CardHighlight.ToggleOutline();
                await UniTask.Delay(200);
            }
        }

        public void EnableCards(bool isNumber)
        {
            if (isNumber)
            {
                foreach(NumberCardObjectPrefab card in _playerNumberCardObjectList)
                {
                    card.CardHighlight.IsAbleToSelect = true;
                    card.CardClickHandler.IsAbleToSelect = true;
                }
                foreach (OperationCardObjectPrefab card in _playerOperationCardObjectList)
                {
                    card.CardHighlight.IsAbleToSelect = false;
                    card.CardClickHandler.IsAbleToSelect = false;
                }
            }
            else
            {
                foreach (NumberCardObjectPrefab card in _playerNumberCardObjectList)
                {
                    card.CardHighlight.IsAbleToSelect = false;
                    card.CardClickHandler.IsAbleToSelect = false;
                }
                foreach (OperationCardObjectPrefab card in _playerOperationCardObjectList)
                {
                    card.CardHighlight.IsAbleToSelect = true;
                    card.CardClickHandler.IsAbleToSelect = true;
                }
            }
        }

        private void GameEnded(object sender, bool didPlayerWin)
        {

            _playerEndChair.SetActive(true);
            _opponentEndChair.SetActive(true);
            _playerGameChair.SetActive(false);
            _opponentGameChair.SetActive(false);
            if (didPlayerWin)
            {
                _playerWinningMan.gameObject.SetActive(true);
                _opponentWinningMan.gameObject.SetActive(false);
                //PlayWinningAnimation(_playerWinningMan).Forget();
                _playerDeadMan.gameObject.SetActive(false);
                _opponentDeadMan.gameObject.SetActive(true);
                
            }
            else
            {
                _playerWinningMan.gameObject.SetActive(false);
                _opponentWinningMan.gameObject.SetActive(true);
                _playerDeadMan.gameObject.SetActive(true);
                _opponentDeadMan.gameObject.SetActive(false);
                //PlayWinningAnimation(_opponentWinningMan).Forget();
                
            }
        }

        private void GameStarted(object sender, bool test)
        {
            Debug.LogError("game started run");
            _playerEndChair.SetActive(false);
            _opponentEndChair.SetActive(false);
            _playerGameChair.SetActive(true);
            _opponentGameChair.SetActive(true);
            _playerWinningMan.gameObject.SetActive(false);
            _opponentWinningMan.gameObject.SetActive(false);
            _playerDeadMan.gameObject.SetActive(false);
            _opponentDeadMan.gameObject.SetActive(false);
        }


        private async UniTaskVoid PlayWinningAnimation(GingerBreadMan winningMan)
        {
            await UniTask.Delay(1000);
            winningMan.PlayWinningAnimation();
        }
    }

}


// operation refresh doesn't show up properly
// operation refresh card disappears immediately after TODO