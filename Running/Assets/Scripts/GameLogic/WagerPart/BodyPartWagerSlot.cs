using JetBrains.Annotations;
using Running.BodyPart;
using Running.Game;
using Running.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class BodyPartWagerSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image _organImage;
    [SerializeField] TMP_Text _organText;
    [SerializeField] TMP_Text _tooltipText;
    [SerializeField] GameObject _tooltip;

    [Inject] private GameController _gameController;
    [Inject] private PlayerData _playerData;

    public event EventHandler<BodyPartType> BodyPartWageredEvent;

    private string _name;

    public void Setup(BodyPartScriptableObject bodyPart)
    {
        _organImage.sprite = bodyPart.BodyPartImage;
        _organText.text = bodyPart.BodyPartName;
        _name = bodyPart.BodyPartName;
        _tooltipText.text = bodyPart.BodyPartDescription;
    }

    [UsedImplicitly]
    public void BodyPartToWager()
    {

        switch (_name)
        {
            case "Heart":
                BodyPartWagered(BodyPartType.Heart);
                return;
            case "Brain":
                BodyPartWagered(BodyPartType.Brain);
                return;
            case "Eye":
                BodyPartWagered(BodyPartType.Eye);
                return;
            case "Lung":
                BodyPartWagered(BodyPartType.Lung);
                return;
            case "Kidney":
                BodyPartWagered(BodyPartType.Kidney);
                return;
            case "Right Rib":
                BodyPartWagered(BodyPartType.FingerRight);
                return;
            case "Left Rib":
                BodyPartWagered(BodyPartType.FingerLeft);
                return;
        }
    }

    private void BodyPartWagered(BodyPartType bodyPart)
    {
        if (_playerData.BodyPartsRemaining[bodyPart] != 0)
        {
            if (bodyPart != BodyPartType.FingerRight)
            {
                _playerData.UsedOrgans.Add(bodyPart);
            }
            _playerData.RoundOrgans.Add(bodyPart);
            _gameController.ApplyAbility(bodyPart);
            BodyPartWageredEvent?.Invoke(this, bodyPart);
            _playerData.BodyPartsRemaining[bodyPart]--;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltip != null)
        {
            _tooltip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltip != null)
        {
            _tooltip.SetActive(false);
        }
    }
}
