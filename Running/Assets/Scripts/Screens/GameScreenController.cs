using Cysharp.Threading.Tasks;
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
        [SerializeField] EndGameScreen _endGameScreen;
        [SerializeField] MainGameScreen _mainGameScreenPrefab;
        [SerializeField] Transform _gameContent;

        [Inject] private DiContainer _container;
        [Inject] private GameController _gameController;

        [Inject]
        private void OnInjected()
        {
            _gameController.GameFinishedEvent += EndGame;
        }

        private void Start()
        {
            _startScreen.GameStartedEvent += InitializeGame;
            _endGameScreen.GameStartedEvent += InitializeGame;
        }

        private void InitializeGame(object sender, EventArgs e)
        {
            _startScreen.gameObject.SetActive(false);
            _endGameScreen.gameObject.SetActive(false);
            //MainGameScreen currentScreen = _container.InstantiatePrefabForComponent<MainGameScreen>(_mainGameScreenPrefab, _gameContent);
            _mainGameScreenPrefab.gameObject.SetActive(true);
            //Debug.LogError("start game set");
        }

        private void EndGame(object sender, bool e)
        {
            DelayEndGameScreen().Forget();
        }

        private async UniTaskVoid DelayEndGameScreen()
        {
            await UniTask.Delay(5000);
            _startScreen.gameObject.SetActive(false);
            _mainGameScreenPrefab.gameObject.SetActive(false);
            //EndGameScreen currentScreen = _container.InstantiatePrefabForComponent<EndGameScreen>(_endGameScreen, _gameContent);
            //Debug.LogError("end game set");
            _endGameScreen.gameObject.SetActive(true);
        }
    }

}
