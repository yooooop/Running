using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Running.Cards
{
    public class NumberCardObjectPrefab : MonoBehaviour
    {
        [SerializeField] private GameObject _card;
        [SerializeField] private CardClickHandler _clickHandler;
        [SerializeField] private CardHighlight _highlight;
        [SerializeField] private GameObject _multiplier;

        public GameObject Card => _card;
        public CardClickHandler CardClickHandler => _clickHandler;
        public CardHighlight CardHighlight => _highlight;
        public GameObject Multiplier => _multiplier;
    }
}
