using Running.Ai;
using Running.BodyPart;
using Running.Game;
using Running.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Running.Player 
{
    public class AnimatorController
    {
        public event EventHandler<int> AnimationPickedUpNumberEvent;
        public event EventHandler<OperationType> AnimationPickedUpOperationEvent;
        public event EventHandler<EventArgs> AnimationFinishedPickedUpNumberEvent;
        public event EventHandler<BodyPartType> OrganPlacedEvent;

        [Inject] private GameController _gameController;
        [Inject] private AiController _aiController;


        public void PickedUpCard(int num)
        {
            AnimationPickedUpNumberEvent?.Invoke(this, num);
        }

        public void PickedUpOperation(OperationType type)
        {
            AnimationPickedUpOperationEvent?.Invoke(this, type);
        }

        public void FinishedPickUpCard()
        {
            _gameController.StartNextPhase();
        }

        public void OrganUsed(BodyPartType type)
        {
            _aiController.UseOrgan(type);
        }

        public void OrganPlaced(BodyPartType type)
        {
            OrganPlacedEvent?.Invoke(this, type);
        }
    }
}

