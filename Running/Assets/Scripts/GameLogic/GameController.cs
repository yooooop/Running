using Running.Cards;
using Running.Operations;
using Running.GamePhase;
using Running.BodyPart;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Running.Player;
using Zenject;
using System.Linq;
using Running.Ai;
using Running.CameraControl;
using Cysharp.Threading.Tasks;

namespace Running.Game
{

    public class GameController
    {


        public event EventHandler<bool> GameStartedEvent;
        public event EventHandler<NumberCard> NumberCardSelectedEvent;
        public event EventHandler<OperationCard> OperationCardSelectedEvent;
        public event EventHandler<EventArgs> WagerStartedEvent;
        public event EventHandler<List<BodyPartType>> BodyPartCardSelectedEvent;
        public event EventHandler<EventArgs> LungAbilitySelectedEvent;
        public event EventHandler<List<OperationCard>> OperationCardListEvent;
        public event EventHandler<List<NumberCard>> NumberCardListEvent;
        public event EventHandler<int> OpponentNumberCardSelectedEvent;
        public event EventHandler<OperationType> OpponentOperationCardSelectedEvent;
        public event EventHandler<int> RefreshNumberCardEvent;
        public event EventHandler<OperationType> RefreshOperationCardEvent;
        public event EventHandler<EventArgs> ResetRoundEvent;
        public event EventHandler<BodyPartType> PlayerOrganWageredEvent;
        public event EventHandler<BodyPartType> PlayerOrganRegeneratedEvent;
        public event EventHandler<BodyPartType> OpponentRegenerateOrganEvent;
        public event EventHandler<string> SetDialogEvent;
        public event EventHandler<EventArgs> HeartUsedEvent;
        public event EventHandler<EventArgs> ResetAllCardsEvent;
        public event EventHandler<MonitorTextArgs> SetMonitorTextEvent;
        public event EventHandler<bool> FlashCardsEvent;
        public event EventHandler<bool> EnableCardsEvent;
        public event EventHandler<List<BodyPartType>> OpponentOrganWageredEvent;
        public event EventHandler<bool> GameFinishedEvent;
        public event EventHandler<bool> RealGameStartedEvent;

        private PhaseType _currentPhase = PhaseType.Default;
        private List<NumberCard> _numberCardList = new List<NumberCard>();
        private List<OperationCard> _operationCardList = new List<OperationCard>();
        private List<BodyPartType> _bodyPartWageredList;
        private int _roundCount = 0;

        public Dictionary<BodyPartType, int> MaxBodyParts = new Dictionary<BodyPartType, int>()
        {
            [BodyPartType.Brain] = 1,
            [BodyPartType.Heart] = 1,
            [BodyPartType.Eye] = 2,
            [BodyPartType.Lung] = 2,
            [BodyPartType.Kidney] = 2,
            [BodyPartType.FingerRight] = 4,
            [BodyPartType.FingerLeft] = 4
        };

        [Inject] private PlayerData _playerData;
        [Inject] private OpponentData _opponentData;
        [Inject] private AiController _aiController;
        [Inject] private CameraController _cameraController;

        [Inject]
        private void OnInjected()
        {
            _cameraController.OnCameraSwitchFinishedEvent += OnCameraMonitorSet;
        }

        public void RealGameStart()
        {
            RealGameStartedEvent?.Invoke(this, true);
            SetDialogEvent?.Invoke(this, "Let's start a game");
            ResetAllCardsEvent?.Invoke(this, EventArgs.Empty);
            GameStartedEvent?.Invoke(this, true);
            _playerData.BodyPartsRemaining = MaxBodyParts;
            _playerData.Life = 4;
            _playerData.UsedOrgans.Clear();
            _playerData.RoundOrgans.Clear();
            _opponentData.BodyPartsRemaining = MaxBodyParts;
            _opponentData.Life = 4;
            _opponentData.UsedOrgans.Clear();
            _opponentData.RoundOrgans.Clear();
            _cameraController.SwitchActiveCamera(CameraControl.CameraType.Main);

        }

        public void OrganWagered(BodyPartType bodyPartType)
        {
            PlayerOrganWageredEvent?.Invoke(this, bodyPartType);
        }

        public void SetBodyPart(List<BodyPartType> bodyPartTypeList)
        {
            _bodyPartWageredList = bodyPartTypeList;

        }

        public void NumberCardSelected(NumberCard num)
        {
            NumberCardSelectedEvent?.Invoke(this, num);
        }

        public void OperationCardSelected(OperationCard operationType)
        {
            OperationCardSelectedEvent?.Invoke(this, operationType);
        }

        public PhaseType GetPhaseType()
        {
            return _currentPhase;
        }

        public void GameFinished(bool didPlayerWin)
        {
            
            GameFinishedEvent?.Invoke(this, didPlayerWin);
            WaitForCameraSwitch(CameraControl.CameraType.EndGame).Forget();
        }

        public void StartNextPhase()
        {
            //Debug.LogError("start next phase");
            Debug.LogError("current phase: " + _currentPhase);
            switch (_currentPhase)
            {
                case PhaseType.Default:
                    StartRound();
                    StartDelay().Forget();
                    
                    return;
                case PhaseType.FirstNumberSelection:
                    DelayForFlash(false).Forget();
                    _currentPhase = PhaseType.OperationSelection;
                    return;
                case PhaseType.SecondNumberSelection:
                    DelayForFlash(true).Forget();
                    _currentPhase = PhaseType.Reveal;
                    return;
                case PhaseType.OperationSelection:
                    SelectedOperation();
                    return;
                case PhaseType.Wager:
                    WaitForCameraSwitch(CameraControl.CameraType.ScaleView).Forget();
                    return;
                case PhaseType.Reveal:
                    int player = RevealCards();
                    int opponent = RevealOpponentCards();
                    EndResult(player, opponent);
                    ResetRoundEvent?.Invoke(this, EventArgs.Empty);
                    return;
                case PhaseType.OpponentPlay:
                    bool wagered = _aiController.OpponentPlay();
                    _currentPhase = PhaseType.SecondNumberSelection;
                    if (!wagered)
                    {
                        StartNextPhase();
                    }
                    else
                    {
                        OpponentOrganWageredEvent?.Invoke(this, _opponentData.RoundOrgans);
                    }
                    return;
            }
        }

        private async UniTaskVoid StartDelay()
        {
            await UniTask.Delay(3000);

            DelayForFlash(true).Forget();
        }

        private async UniTaskVoid WaitForCameraSwitch(CameraControl.CameraType type)
        {
            await UniTask.Delay(1000);

            _cameraController.SwitchActiveCamera(type);

            await UniTask.Delay(3000);

            if (type == CameraControl.CameraType.ScaleView)
            {
                WagerStartedEvent?.Invoke(this, EventArgs.Empty);
            }
            else if (type == CameraControl.CameraType.Main)
            {
                await UniTask.Delay(2000);
                StartNextPhase();
            }

        }

        private async UniTaskVoid DelayForFlash(bool isNumber)
        {
            await UniTask.Delay(1000);
            FlashCardsEvent?.Invoke(this, isNumber);

        }

        public void NumberSelected(int num)
        {
            if (_playerData.FirstCard == -2)
            {
                _playerData.FirstCard = num;
            }
            else
            {
                _playerData.SecondCard = num;
            }
        }

        public void OperationSelected(OperationType operationType)
        {
            _playerData.OperationType = operationType;
        }

        public void AddToNumberList(NumberCard numberButton)
        {
            _numberCardList.Add(numberButton);
        }

        public void AddToOperationList(OperationCard operationButton)
        {
            _operationCardList.Add(operationButton);
        }

        public void CardListCompletedEvent(bool isNumber)
        {
            if (isNumber)
            {
                NumberCardListEvent?.Invoke(this, _numberCardList);
            }
            else
            {
                OperationCardListEvent?.Invoke(this, _operationCardList);
            }
        }

        public void ResetCards()
        {
            ResetAllCardsEvent?.Invoke(this, EventArgs.Empty);
        }

        private void StartRound()
        {
            _playerData.FirstCard = -2;
            _playerData.SecondCard = -2;
            _playerData.OperationType = OperationType.Default;
            _opponentData.FirstCard = -2;
            _opponentData.SecondCard = -2;
            _opponentData.OperationType = OperationType.Default;
            if (_roundCount % 3 == 0) 
            {
                GameStartedEvent?.Invoke(this, true);
            }
            /*foreach (Button card in _operationCardList)
            {
                card.interactable = false;
            }
            foreach (Button card in _numberCardList)
            {
                card.interactable = true;
            }*/
            _currentPhase = PhaseType.FirstNumberSelection;
            _roundCount++;
        }

        public void SelectedNumber()
        {
            /*foreach (Button card in _operationCardList)
            {
                card.interactable = true;
            }
            foreach (Button card in _numberCardList)
            {
                card.interactable = false;
            }*/
            
            int randomNumber = UnityEngine.Random.Range(1, 10);
            if (_opponentData.FirstCard == -2)
            {
                //_currentPhase = PhaseType.FirstNumberSelection;
                if (randomNumber == 10) randomNumber = -1;
                _opponentData.FirstCard = randomNumber;
            }
            else
            {
                //_currentPhase = PhaseType.Reveal;
                while (_opponentData.FirstCard == randomNumber)
                {
                    randomNumber = UnityEngine.Random.Range(1, 10);
                }
                if (randomNumber == 10) randomNumber = -1;
                _opponentData.SecondCard = randomNumber;
            }
            OpponentNumberCardSelectedEvent?.Invoke(this, randomNumber);
        }
        // opponent can't select 0 TODO

        private void SelectedOperation()
        {
            /*foreach (Button card in _operationCardList)
            {
                card.interactable = false;
            }
            foreach (Button card in _numberCardList)
            {
                card.interactable = true;
            }*/
            _currentPhase = PhaseType.Wager;
            _opponentData.OperationType = GetRandomOperationType();
            OpponentOperationCardSelectedEvent?.Invoke(this, _opponentData.OperationType);
            //WagerStartedEvent?.Invoke(this, null);

        }

        public OperationType GetRandomOperationType()
        {
            OperationType[] values = (OperationType[])System.Enum.GetValues(typeof(OperationType));
            int randomIndex = UnityEngine.Random.Range(1, values.Length - 1);
            return values[randomIndex];
        }

        private int RevealCards()
        {
            int playerResult = 0;
            Debug.Log("player first card: " + _playerData.FirstCard);
            Debug.Log("player second card: " + _playerData.SecondCard);
            Debug.Log("player operation: " + _playerData.OperationType);
            switch (_playerData.OperationType)
            {
                case OperationType.Addition:
                    playerResult = _playerData.FirstCard + _playerData.SecondCard;
                    //Debug.LogError("player result: " + playerResult);
                    //EndResult();
                    return playerResult;
                case OperationType.Division:
                    playerResult = _playerData.FirstCard / _playerData.SecondCard;
                    //Debug.LogError("player result: " + playerResult);
                    //EndResult();
                    return playerResult;
                case OperationType.Multiplication:
                    playerResult = _playerData.FirstCard * _playerData.SecondCard;
                    //Debug.LogError("player result: " + playerResult);
                    // EndResult();
                    return playerResult;
                case OperationType.Subtraction:
                    playerResult = _playerData.FirstCard - _playerData.SecondCard;
                    //Debug.LogError("player result: " + playerResult);
                    //EndResult();
                    return playerResult;
                case OperationType.Magnet:
                    playerResult = CombineNumbers(_playerData.FirstCard, _playerData.SecondCard);
                    //Debug.LogError("player result: " + playerResult);
                    //EndResult();
                    return playerResult;
            }
            return 0;


        }

        private int RevealOpponentCards()
        {
            int playerResult = 0;
            Debug.Log("opponent first card: " + _opponentData.FirstCard);
            Debug.Log("opponent second card: " + _opponentData.SecondCard);
            Debug.Log("opponent operation: " + _opponentData.OperationType);
            switch (_opponentData.OperationType)
            {
                case OperationType.Addition:
                    playerResult = _opponentData.FirstCard + _opponentData.SecondCard;
                    //Debug.LogError("opponent result: " + playerResult);
                    //EndResult();
                    return playerResult;
                case OperationType.Division:
                    playerResult = _opponentData.FirstCard / _opponentData.SecondCard;
                    //Debug.LogError("opponent result: " + playerResult);
                    //EndResult();
                    return playerResult;
                case OperationType.Multiplication:
                    playerResult = _opponentData.FirstCard * _opponentData.SecondCard;
                    //Debug.LogError("opponent result: " + playerResult);
                    //EndResult();
                    return playerResult;
                case OperationType.Subtraction:
                    playerResult = _opponentData.FirstCard - _opponentData.SecondCard;
                    //Debug.LogError("opponent result: " + playerResult);
                    //EndResult();
                    return playerResult;
                case OperationType.Magnet:
                    playerResult = CombineNumbers(_opponentData.FirstCard, _opponentData.SecondCard);
                    //Debug.LogError("opponent result: " + playerResult);
                    //EndResult();
                    return playerResult;
            }

            return 0;
        }

        public int CombineNumbers(int num1, int num2)
        {
            Debug.LogError("num: " + num1 + " num2: " + num2);
            if (num2 == -1)
            {
                return -int.Parse($"{num1}1");
            }

            return int.Parse($"{num1}{num2}");
        }

        private string TypeToString(OperationType type)
        {
            switch(type)
            {
                case OperationType.Addition:
                    return "+";
                case OperationType.Division:
                    return "/";
                case OperationType.Subtraction:
                    return "-";
                case OperationType.Magnet:
                    return "U";
                case OperationType.Multiplication:
                    return "x";
            }
            return "";
        }

        public void Wagered()
        {
            _currentPhase = PhaseType.OpponentPlay; 

            BodyPartCardSelectedEvent?.Invoke(this, _bodyPartWageredList);

            WaitForCameraSwitch(CameraControl.CameraType.Main).Forget();
        }


        private void EndResult(int player, int opponent)
        {
            Debug.Log("player result: " + player);
            Debug.Log("opponent result: " + opponent);

            _currentPhase = PhaseType.Result;
            _currentPhase = PhaseType.Default;
            if (player > opponent)
            {
                if (_opponentData.RoundOrgans.Contains(BodyPartType.Brain) || _opponentData.RoundOrgans.Contains(BodyPartType.Brain))
                {
                    if (_opponentData.BodyPartsRemaining[BodyPartType.Brain] == 0 || _opponentData.BodyPartsRemaining[BodyPartType.Heart] == 0)
                    {
                        _opponentData.Life = 1;
                    }
                }

                if (_opponentData.RoundOrgans.Contains(BodyPartType.Kidney) || _opponentData.RoundOrgans.Contains(BodyPartType.Lung))
                {
                    if (_opponentData.BodyPartsRemaining[BodyPartType.Kidney] == 0 || _opponentData.BodyPartsRemaining[BodyPartType.Lung] == 0)
                    {
                        _opponentData.Life = 1;
                    }
                }

                foreach (BodyPartType type in _playerData.RoundOrgans)
                {
                    if (_playerData.BodyPartsRemaining[type] < MaxBodyParts[type])
                    {
                        _playerData.BodyPartsRemaining[type]++;
                    }
                    PlayerOrganRegeneratedEvent?.Invoke(this, type);
                }
                _opponentData.Life--;
            }
            else
            {
                if (_playerData.RoundOrgans.Contains(BodyPartType.Brain) || _playerData.RoundOrgans.Contains(BodyPartType.Brain))
                {
                    if (_playerData.BodyPartsRemaining[BodyPartType.Brain] == 0 || _playerData.BodyPartsRemaining[BodyPartType.Heart] == 0)
                    {
                        _playerData.Life = 1;
                    }
                }

                if (_playerData.RoundOrgans.Contains(BodyPartType.Kidney) || _playerData.RoundOrgans.Contains(BodyPartType.Lung))
                {
                    if (_playerData.BodyPartsRemaining[BodyPartType.Kidney] == 0 || _playerData.BodyPartsRemaining[BodyPartType.Lung] == 0)
                    {
                        _playerData.Life = 1;
                    }
                }

                foreach (BodyPartType type in _opponentData.RoundOrgans)
                {
                    if (_opponentData.BodyPartsRemaining[type] < MaxBodyParts[type])
                    {
                        OpponentRegenerateOrganEvent?.Invoke(this, type);
                    }
                }
                _playerData.Life--;
            }
            _cameraController.SwitchActiveCamera(CameraControl.CameraType.ScreenMonitor);

            DelayInSwitchingCamera().Forget();

            Debug.LogError("test2");
            _playerData.RoundOrgans.Clear();
            _opponentData.RoundOrgans.Clear();
        }

        private void OnCameraMonitorSet(object sender, CameraControl.CameraType type)
        {
            if (type == CameraControl.CameraType.ScreenMonitor)
            {
                MonitorTextArgs monitorTextArgs = new MonitorTextArgs(ofn: _opponentData.FirstCard, osn: _opponentData.SecondCard, 
                                                                      pfn: _playerData.FirstCard, psn: _playerData.SecondCard, 
                                                                      po: TypeToString(_playerData.OperationType), oo: TypeToString(_opponentData.OperationType),
                                                                      pr: RevealCards(), or: RevealOpponentCards(), pl: _playerData.Life, ol: _opponentData.Life,
                                                                      round: _roundCount);
                SetMonitorTextEvent?.Invoke(this, monitorTextArgs);

            }
        }

        private async UniTaskVoid DelayInSwitchingCamera()
        {
            await UniTask.Delay(9000);

            if (_opponentData.Life == 0 || _playerData.Life == 0)
            {
                return;
            }

            _cameraController.SwitchActiveCamera(CameraControl.CameraType.Main);
            // win lose logic goes here
            StartNextPhase();
        }

        public void ApplyAbility(BodyPartType bodyPartType)
        {

            if (bodyPartType == BodyPartType.FingerRight)
            {
                List<BodyPartType> missingOrgans = _playerData.UsedOrgans;
                int randomIndex = UnityEngine.Random.Range(0, missingOrgans.Count - 1);
                //Debug.LogError("random index" + randomIndex);
                BodyPartType missing = missingOrgans[randomIndex];
                while (_playerData.BodyPartsRemaining[missing] == MaxBodyParts[missing])
                {
                    randomIndex = UnityEngine.Random.Range(0, missingOrgans.Count - 1);
                    missing = missingOrgans[randomIndex];
                }
                _playerData.BodyPartsRemaining[missing]++;
                PlayerOrganRegeneratedEvent?.Invoke(this, missing);

            }
            else if (bodyPartType == BodyPartType.Eye)
            {
                if (_opponentData.FirstCard > _playerData.FirstCard)
                {
                    SetDialogEvent?.Invoke(this, "opponent, they're higher");
                }
                else
                {
                    SetDialogEvent?.Invoke(this, "player, you're higher");
                }
            }
            else if (bodyPartType == BodyPartType.Kidney)
            {
                OperationType[] operations = (OperationType[])System.Enum.GetValues(typeof(OperationType));

                OperationType[] filteredOperations = operations.Where(op => op != _playerData.OperationType && op != OperationType.Default).ToArray();

                int firstIndex = UnityEngine.Random.Range(0, filteredOperations.Length);
                int secondIndex;

                do
                {
                    secondIndex = UnityEngine.Random.Range(0, filteredOperations.Length);
                } while (secondIndex == firstIndex);

                OperationType firstOperation = filteredOperations[firstIndex];
                OperationType secondOperation = filteredOperations[secondIndex];

                SetDialogEvent?.Invoke(this, "your operation is not: " + firstOperation + " or " + secondOperation);
            }
            else if (bodyPartType == BodyPartType.Brain)
            {
                SetDialogEvent?.Invoke(this, "opponent's card is: " + _opponentData.FirstCard);
            }
            else if (bodyPartType == BodyPartType.Heart)
            {
                _playerData.FirstCard *= 2;
                HeartUsedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public void RefreshCard(string type)
        {
            if (type == "number")
            {
                int randomNumber = UnityEngine.Random.Range(1, 10);
                if (randomNumber == 10) randomNumber = -1;

                while (_playerData.FirstCard == randomNumber)
                {
                    randomNumber = UnityEngine.Random.Range(1, 10);
                    if (randomNumber == 10) randomNumber = -1;
                }
                //Debug.LogError("the number you had before was: " + _playerData.FirstCard);
                _playerData.FirstCard = randomNumber;

                // in the future, probably just use the player data first card value to refresh and it should work
                RefreshNumberCardEvent?.Invoke(this, randomNumber);
            }
            else if (type == "operation")
            {
                OperationType[] operations = (OperationType[])System.Enum.GetValues(typeof(OperationType));

                OperationType[] filteredOperations = operations.Where(op => op != _playerData.OperationType).ToArray();

                int firstIndex = UnityEngine.Random.Range(0, filteredOperations.Length);
                //Debug.LogError("the operation you had before was: " + _playerData.OperationType);
                _playerData.OperationType = filteredOperations[firstIndex];
                RefreshOperationCardEvent?.Invoke(this, filteredOperations[firstIndex]);
            }
        }

    }

}

