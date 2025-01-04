using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Running.Cards
{
    public class OperationCardObjectPrefab : MonoBehaviour
    {
        [SerializeField] private GameObject _card;
        [SerializeField] private CardClickHandler _clickHandler;
        [SerializeField] private CardHighlight _highlight;

        public GameObject Card => _card;
        public CardClickHandler CardClickHandler => _clickHandler;
        public CardHighlight CardHighlight => _highlight;
    }
}