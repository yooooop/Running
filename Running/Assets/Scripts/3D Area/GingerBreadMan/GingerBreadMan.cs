using Running.Ai;
using Running.BodyPart;
using Running.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Running.Player
{
    public class GingerBreadMan : MonoBehaviour
    {
        [SerializeField] GameObject _brain;
        [SerializeField] GameObject _heart;
        [SerializeField] List<GameObject> _lungs = new List<GameObject>();
        [SerializeField] List<GameObject> _kidneys = new List<GameObject>();
        [SerializeField] List<GameObject> _leftRibs = new List<GameObject>();
        [SerializeField] List<GameObject> _rightRibs = new List<GameObject>();
        [SerializeField] List<GameObject> _eyes = new List<GameObject>();

        [Inject] private GameController _gameController;
        [Inject] private AiController _aiController;
        [Inject] private OpponentData _opponentData;

        [Inject]
        private void OnInjected()
        {
            _aiController.OpponentUseOrganEvent += OnAiOrganDisableEvent;
            _aiController.OpponentRegenerateOrganEvent += OnAiOrganEnableEvent;
            _gameController.OpponentRegenerateOrganEvent += OnAiOrganEnableEvent;


        } 

        private void OnAiOrganDisableEvent(object sender, BodyPartType bodyPartType)
        {
            _opponentData.BodyPartsRemaining[bodyPartType]--;
            switch (bodyPartType.ToString())
            {
                case "Heart":
                    _heart.SetActive(false);
                    return;
                case "Brain":
                    _brain.SetActive(false);
                    return;
                case "Eye":
                    _eyes[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 2)].SetActive(false);
                    return;
                case "Lung":
                    _lungs[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 2)].SetActive(false);
                    return;
                case "Kidney":
                    _kidneys[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 2)].SetActive(false);
                    return;
                case "FingerRight":
                    _rightRibs[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 4)].SetActive(false);
                    return;
                case "FingerLeft":
                    _leftRibs[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 4)].SetActive(false);
                    return;
            }
        }

        private void OnAiOrganEnableEvent(object sender, BodyPartType bodyPartType)
        {
            if (bodyPartType == BodyPartType.FingerRight)
            {
                Debug.LogError(bodyPartType + " test: " + Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 4));
            }
            else
            {
                Debug.LogError(bodyPartType + " test: " + Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 2));
            }
            switch (bodyPartType.ToString())
            {
                case "Heart":
                    _heart.SetActive(false);
                    return;
                case "Brain":
                    _brain.SetActive(false);
                    return;
                case "Eye":
                    _eyes[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 2)].SetActive(true);
                    return;
                case "Lung":
                    _lungs[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 2)].SetActive(true);
                    return;
                case "Kidney":
                    _kidneys[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 2)].SetActive(true);
                    return;
                case "FingerRight":
                    _rightRibs[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 4)].SetActive(true);
                    return;
                case "FingerLeft":
                    _leftRibs[Mathf.Abs(_opponentData.BodyPartsRemaining[bodyPartType] % 4)].SetActive(true);
                    return;
            }
            _opponentData.BodyPartsRemaining[bodyPartType]++;
        }

    }
}
