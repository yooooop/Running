using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Running.BodyPart
{
    [Serializable]
    [CreateAssetMenu(menuName = "Running/BodyPart", fileName = "New Body Part")]
    public class BodyPartScriptableObject : ScriptableObject
    {
        [field: SerializeField] public Sprite BodyPartImage { get; private set; }
        [field: SerializeField] public string BodyPartName { get; private set; }
        [field: SerializeField] public BodyPartType BodyPartType { get; private set; }
        [field: SerializeField] public BodyPartPrefab BodyPartPrefab { get; private set; }

    }
}