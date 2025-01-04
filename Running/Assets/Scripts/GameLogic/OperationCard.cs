using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Running.Operations;


namespace Running.Cards
{
    [Serializable]
    [CreateAssetMenu(menuName = "Running/Cards/Operation", fileName = "New Operation Card")]
    public class OperationCard : ScriptableObject
    {
        [field: SerializeField] public Sprite OperationSprite { get; private set; }
        [field: SerializeField] public OperationType sign { get; private set; }
        [field: SerializeField] public OperationCardObjectPrefab OperationCardPrefab { get; private set; }
    }
}