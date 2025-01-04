using Running.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Running.GamePhase
{

    public class GameScreenController : MonoBehaviour
    {
        [SerializeField] StartScreen _startScreen;
        [SerializeField] MainGameScreen _mainGameScreenPrefab;
        [SerializeField] Transform _gameContent;

        [Inject] private DiContainer _container;

        private void Start()
        {
            _startScreen.GameStartedEvent += InitializeGame;
        }

        private void InitializeGame(object sender, EventArgs e)
        {
            _startScreen.gameObject.SetActive(false);
            MainGameScreen currentScreen = _container.InstantiatePrefabForComponent<MainGameScreen>(_mainGameScreenPrefab, _gameContent);
        }
    }

}
