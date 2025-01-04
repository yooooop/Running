using Running.BodyPart;
using Running.Game;
using Running.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BodyPartsRemainingViewUI : MonoBehaviour
{
    [SerializeField] GameObject _brain;
    [SerializeField] GameObject _heart;
    [SerializeField] List<GameObject> _lungs = new List<GameObject>();
    [SerializeField] List<GameObject> _kidneys = new List<GameObject>();
    [SerializeField] List<GameObject> _leftRibs = new List<GameObject>();
    [SerializeField] List<GameObject> _rightRibs = new List<GameObject>();
    [SerializeField] List<GameObject> _eyes = new List<GameObject>();

    [Inject] private GameController _gameController;
    [Inject] private PlayerData _playerData;

    [Inject]
    private void OnInjected()
    {
        _gameController.PlayerOrganWageredEvent += SetGameObjectOff;
        _gameController.PlayerOrganRegeneratedEvent += SetGameObjectOn;
    }

    public void SetGameObjectOff(object sender, BodyPartType part)
    {
        if (part == BodyPartType.FingerRight)
        {
            Debug.LogError(part + " test: " + Mathf.Abs(_playerData.BodyPartsRemaining[part] % 4));
        }
        else
        {
            Debug.LogError(part + " test: " + Mathf.Abs(_playerData.BodyPartsRemaining[part] % 2));
        }
        
        switch (part.ToString())
        {
            case "Heart":
                _heart.SetActive(false);
                break;
            case "Brain":
                _brain.SetActive(false);
                break;
            case "Eye":
                _eyes[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 2)].SetActive(false);
                break;
            case "Lung":
                _lungs[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 2)].SetActive(false);
                break;
            case "Kidney":
                _kidneys[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 2)].SetActive(false);
                break;
            case "FingerRight":
                _rightRibs[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 4)].SetActive(false);
                break;
            case "FingerLeft":
                _leftRibs[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 4)].SetActive(false);
                break;
        }
    }

    public void SetGameObjectOn(object sender, BodyPartType part)
    {
        if (part == BodyPartType.FingerRight)
        {
            Debug.LogError(part + " test: " + Mathf.Abs(_playerData.BodyPartsRemaining[part] % 4));
        }
        else
        {
            Debug.LogError(part + " test: " + Mathf.Abs(_playerData.BodyPartsRemaining[part] % 2));
        }
        switch (part.ToString())
        {
            case "Heart":
                _heart.SetActive(true);
                break;
            case "Brain":
                _brain.SetActive(true);
                break;
            case "Eye":
                _eyes[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 2)].SetActive(true);
                break;
            case "Lung":
                _lungs[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 2)].SetActive(true);
                break;
            case "Kidney":
                _kidneys[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 2)].SetActive(true);
                break;
            case "FingerRight":
                _rightRibs[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 4)].SetActive(true);
                break;
            case "FingerLeft":
                _leftRibs[Mathf.Abs(_playerData.BodyPartsRemaining[part] % 4)].SetActive(true);
                break;
        }
    }
}
