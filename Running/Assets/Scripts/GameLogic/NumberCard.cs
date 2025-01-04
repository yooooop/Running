using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Running.Cards 
{ 
    [Serializable]
    [CreateAssetMenu(menuName = "Running/Cards/Number", fileName = "New Number Card")]
    public class NumberCard : ScriptableObject
    {
        [field: SerializeField] public Sprite NumberSprite { get; private set; }
        [field: SerializeField] public int value { get; private set; }
        [field: SerializeField] public NumberCardObjectPrefab NumberCardPrefab { get; private set; }
        
    }
}