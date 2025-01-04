using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    [SerializeField] Button _startButton;

    public EventHandler<EventArgs> GameStartedEvent;

    [UsedImplicitly]
    public void StartGame()
    {
        GameStartedEvent?.Invoke(this, null);
    }
}
