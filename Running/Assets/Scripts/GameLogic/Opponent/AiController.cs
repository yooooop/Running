using Running.BodyPart;
using Running.Operations;
using Running.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Running.Ai
{

    public class AiController
    {
        public event EventHandler<BodyPartType> OpponentUseOrganEvent;
        public event EventHandler<BodyPartType> OpponentRegenerateOrganEvent;
        public event EventHandler<int> OpponentNumberRefreshEvent;
        public event EventHandler<OperationType> OpponentOperationRefreshEvent;

        [Inject] private PlayerData _playerData;
        [Inject] private OpponentData _opponentData;


        private Dictionary<BodyPartType, int> MaxBodyParts = new Dictionary<BodyPartType, int>()
        {
            [BodyPartType.Brain] = 1,
            [BodyPartType.Heart] = 1,
            [BodyPartType.Eye] = 2,
            [BodyPartType.Lung] = 2,
            [BodyPartType.Kidney] = 2,
            [BodyPartType.FingerRight] = 4,
            [BodyPartType.FingerLeft] = 4
        };

        public bool OpponentPlay()
        { 
            if (_opponentData.FirstCard <= 3)
            {
                Debug.LogError("Opponent wagers nothing (low card).");
                return false;
            }   
            
            if (_opponentData.FirstCard >= 4)
            {
                HighCardBehavior(); 
            }
            return true;
        }

        private void HighCardBehavior()
        {
            int randomNumber = Random.Range(1, 3);

            if (randomNumber == 1)
            {
                if (OpponentHasOrgan(BodyPartType.Kidney) || OpponentHasOrgan(BodyPartType.Lung) || OpponentHasOrgan(BodyPartType.Eye))
                {
                    UseOpponentOrgan();
                }
                else
                {
                    HighCardBehavior();
                }
            }
            else if (randomNumber == 2)
            {
                if (_opponentData.BodyPartsRemaining[BodyPartType.FingerLeft] <= 2 && OpponentHasOrgan(BodyPartType.FingerRight))
                {
                    UseOpponentRightFinger();
                }
                else
                {
                    HighCardBehavior();
                }
            }
            else
            {
                if (_opponentData.BodyPartsRemaining[BodyPartType.FingerLeft] == 1 &&
                (OpponentHasOrgan(BodyPartType.Brain) || OpponentHasOrgan(BodyPartType.Heart)))
                {
                    UseOpponentBrainOrHeart();
                }
                else
                {
                    HighCardBehavior();
                }
            }
        }

        private void UseOpponentOrgan(bool useKidney = true, bool useLung = true, bool useEye = true)
        {
            if (OpponentHasOrgan(BodyPartType.Kidney) && useKidney)
            {
                Debug.LogError("Opponent uses Kidney.");
                //?.Invoke(this, BodyPartType.Kidney);
                ApplyOpponentAbility(BodyPartType.Kidney);
                AfterOpponentUsesKidney();
            }
            else if (OpponentHasOrgan(BodyPartType.Lung) && useLung)
            {
                Debug.LogError("Opponent uses Lung.");
                //?.Invoke(this, BodyPartType.Lung);
                ApplyOpponentAbility(BodyPartType.Lung);
                AfterOpponentUsesLung();
            }
            else if (OpponentHasOrgan(BodyPartType.Eye) && useEye)
            {
                Debug.LogError("Opponent uses Eye.");
                //?.Invoke(this, BodyPartType.Eye);
                ApplyOpponentAbility(BodyPartType.Eye);
                AfterOpponentUsesEye();
            }
        }

        private void UseOpponentRightFinger()
        {
            Debug.LogError("Opponent uses Right Finger to regenerate.");
            //?.Invoke(this, BodyPartType.FingerRight);
            bool checkMax = true;
            foreach(BodyPartType part in MaxBodyParts.Keys)
            {
                if (_opponentData.BodyPartsRemaining[part] != MaxBodyParts[part])
                {
                    checkMax = false;
                }
            }

            if (checkMax)
            {
                return;
            }
            ApplyOpponentAbility(BodyPartType.FingerRight);
        }

        private void UseOpponentBrainOrHeart()
        {
            if (OpponentHasOrgan(BodyPartType.Brain))
            {
                Debug.LogError("Opponent uses Brain.");
                //?.Invoke(this, BodyPartType.Brain);
                ApplyOpponentAbility(BodyPartType.Brain);
            }
            else if (OpponentHasOrgan(BodyPartType.Heart))
            {
                Debug.LogError("Opponent uses Heart.");
                //?.Invoke(this, BodyPartType.Heart);
                ApplyOpponentAbility(BodyPartType.Heart);
            }
        }

        private bool OpponentHasOrgan(BodyPartType organType)
        {
            bool result = _opponentData.BodyPartsRemaining[organType] > 0;
            if (result)
            {
                int counter = 0;
                foreach (BodyPartType type in _opponentData.RoundOrgans)
                {
                    if (type == organType)
                    {
                        counter++;
                    }
                }
                if (counter >= _opponentData.BodyPartsRemaining[organType])
                {
                    result = false;
                }
            }

            return result;
        }

        private void AfterOpponentUsesKidney()
        {
            OperationType[] operations = (OperationType[])System.Enum.GetValues(typeof(OperationType));

            List<OperationType> filteredOperations = operations.Where(op => op != _opponentData.OperationType).ToList();
            System.Random rng = new System.Random();
            int n = filteredOperations.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                OperationType temp = filteredOperations[k];
                filteredOperations[k] = filteredOperations[n];
                filteredOperations[n] = temp;
            }

            filteredOperations.RemoveAt(0);
            filteredOperations.RemoveAt(0);
            filteredOperations.Add(_opponentData.OperationType);

            if (filteredOperations.Contains(OperationType.Multiplication) && filteredOperations.Contains(OperationType.Magnet))
            {
                UseOpponentOrgan(useKidney: false);
            }


        }

        private void AfterOpponentUsesLung()
        {
            if (_opponentData.FirstCard > 5 &&
                OpponentHasOrgan(BodyPartType.Kidney) ||
                OpponentHasOrgan(BodyPartType.Lung) ||
                OpponentHasOrgan(BodyPartType.Eye))
            {
                Debug.LogError("Opponent plays another organ.");
                UseOpponentOrgan(useLung: false);
            }
        }

        private void AfterOpponentUsesEye()
        {
            if (_playerData.FirstCard > _opponentData.FirstCard &&
                (_playerData.OperationType == OperationType.Multiplication ||
                 _playerData.OperationType == OperationType.Magnet))
            {
                Debug.LogError("Opponent does nothing (Player advantage).");
                return;
            }
            else if (_playerData.FirstCard > _opponentData.FirstCard)
            {
                Debug.LogError("Opponent refreshes number.");
                if (OpponentHasOrgan(BodyPartType.Lung))
                {
                    ApplyOpponentAbility(BodyPartType.Lung);
                }
            }
            else
            {
                Debug.LogError("Opponent wagers one more.");
                UseOpponentOrgan(useEye: false);
            }
        }

        private void ApplyOpponentAbility(BodyPartType bodyPartType)
        {
            if (bodyPartType == BodyPartType.FingerRight)
            {
                RegenerateOpponentOrgan();
            }
            else if (bodyPartType == BodyPartType.Kidney)
            {
                Debug.LogError("Opponent applies Kidney ability.");
                
            }
            else if (bodyPartType == BodyPartType.Lung)
            {
                Debug.LogError("Opponent applies Lung ability.");
                if (_opponentData.FirstCard > 5)
                {
                    Debug.LogError("Opponent refreshes operation.");
                    OperationType[] operations = (OperationType[])System.Enum.GetValues(typeof(OperationType));

                    OperationType[] filteredOperations = operations.Where(op => op != _opponentData.OperationType && op != OperationType.Default).ToArray();

                    int firstIndex = Random.Range(0, filteredOperations.Length);
                    Debug.LogError("the operation you had before was: " + _opponentData.OperationType);
                    _opponentData.OperationType = filteredOperations[firstIndex];
                    OpponentOperationRefreshEvent?.Invoke(this, filteredOperations[firstIndex]);
                }
                else
                {
                    Debug.LogError("Opponent refreshes number.");
                    int randomNumber = Random.Range(1, 10);
                    if (randomNumber == 10) randomNumber = -1;

                    while (_opponentData.FirstCard == randomNumber)
                    {
                        randomNumber = Random.Range(1, 10);
                        if (randomNumber == 10) randomNumber = -1;
                    }
                    Debug.LogError("the number you had before was: " + _opponentData.FirstCard);
                    _opponentData.FirstCard = randomNumber;
                    OpponentNumberRefreshEvent?.Invoke(this, randomNumber);

                }
            }
            else if (bodyPartType == BodyPartType.Eye)
            {
                Debug.LogError("Opponent applies Eye ability.");
                
            }
            else if (bodyPartType == BodyPartType.Brain)
            {
                Debug.LogError("Opponent applies Brain ability.");
                
            }
            else if (bodyPartType == BodyPartType.Heart)
            {
                Debug.LogError("Opponent applies Heart ability.");
                
            }
            if (bodyPartType != BodyPartType.FingerRight)
            {
                _opponentData.UsedOrgans.Add(bodyPartType);
            }
            _opponentData.RoundOrgans.Add(bodyPartType);
        }

        private void RegenerateOpponentOrgan()
        {
            List<BodyPartType> missingOrgans = _opponentData.UsedOrgans;
            int randomIndex = UnityEngine.Random.Range(0, missingOrgans.Count - 1);
            //Debug.LogError("random index" + randomIndex);
            BodyPartType missing = missingOrgans[randomIndex];
            while (_opponentData.BodyPartsRemaining[missing] == MaxBodyParts[missing])
            {
                randomIndex = UnityEngine.Random.Range(0, missingOrgans.Count - 1);
                missing = missingOrgans[randomIndex];
            }
            _opponentData.BodyPartsRemaining[missing]++;

            OpponentRegenerateOrganEvent?.Invoke(this, missing);
        }

        public void UseOrgan(BodyPartType type)
        {
            OpponentUseOrganEvent?.Invoke(this, type);
        }
    }

}


