using Running.BodyPart;
using Running.Operations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Running.Player 
{
    public class PlayerData
    {
        // general
        public bool IsAlive = true;

        // card values
        public int FirstCard = -2;
        public int SecondCard = -2;
        public OperationType OperationType { get; set; }

        // body part remaining
        public Dictionary<BodyPartType, int> BodyPartsRemaining = new Dictionary<BodyPartType, int>()
        {
            [BodyPartType.Brain] = 1,
            [BodyPartType.Heart] = 1,
            [BodyPartType.Eye] = 2,
            [BodyPartType.Lung] = 2,
            [BodyPartType.Kidney] = 2,
            [BodyPartType.FingerRight] = 4,
            [BodyPartType.FingerLeft] = 4
        };

        public List<BodyPartType> UsedOrgans = new List<BodyPartType>();

        public List<BodyPartType> RoundOrgans = new List<BodyPartType>();

        public int Life = 4;
    }
}

// wed: card abilities
// thu: card abilities, win lose system
// sat: ai
// sun: ai
// mon: sanity check of all features

// still to do, without replacement, card selection