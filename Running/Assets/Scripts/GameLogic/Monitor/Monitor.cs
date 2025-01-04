using Cysharp.Threading.Tasks;
using Running.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

public class Monitor : MonoBehaviour
{
    [SerializeField] TMP_Text _opponentResultText;
    [SerializeField] TMP_Text _opponentLifeText;
    [SerializeField] TMP_Text _playerResultText;
    [SerializeField] TMP_Text _playerLifeText;
    [SerializeField] TMP_Text _roundText;
    [SerializeField] private float _messageSmootheningEffectSpeed = 5;
    [SerializeField] private float _stopCharacterDelay = 0.3f;

    [Inject] GameController _gameController;

    [Inject]
    private void OnInjected()
    {
        _gameController.SetMonitorTextEvent += SetText;
    }
    
    private void SetText(object sender, MonitorTextArgs args)
    {
        string playerResult = args.PlayerFirstNum.ToString() + " " + args.PlayerOperation + " " + args.PlayerSecondNum.ToString() + " = " + args.PlayerResult.ToString();
        string opponentResult = args.OpponentFirstNum.ToString() + " " + args.OpponentOperation + " " + args.OpponentSecondNum.ToString() + " = " + args.OpponentResult.ToString();


        string roundText = "ROUND " + (args.Round + 1).ToString();
        SetResultSmoothAsync(playerResult, opponentResult, args.PlayerLife, args.OpponentLife, roundText).Forget();
    }

    private async UniTaskVoid SetResultSmoothAsync(string playerResult, string opponentResult, int playerLife, int opponentLife, string roundText)
    {
        StringBuilder playerMessage = new StringBuilder();
        StringBuilder opponentMessage = new StringBuilder();

        int playerIndex = 0;
        int opponentIndex = 0;

        float defaultDelay = 1 / _messageSmootheningEffectSpeed;

        while (playerIndex < playerResult.Length || opponentIndex < opponentResult.Length)
        {
            if (playerIndex < playerResult.Length)
            {
                char appendingChar = playerResult[playerIndex];
                playerMessage.Append(appendingChar);
                _playerResultText.text = playerMessage.ToString();
                playerIndex++;
            }

            if (opponentIndex < opponentResult.Length)
            {
                char appendingChar = opponentResult[opponentIndex];
                opponentMessage.Append(appendingChar);
                _opponentResultText.text = opponentMessage.ToString();
                opponentIndex++;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(defaultDelay));
        }

        await UniTask.Delay(400);

        _playerLifeText.text = "";
        _opponentLifeText.text = "";

        StringBuilder playerLives = new StringBuilder();
        StringBuilder opponentLives = new StringBuilder();

        for (int i = 0; i < Mathf.Max(playerLife, opponentLife); i++)
        {
            if (i < playerLife)
            {
                playerLives.Append('L');
                _playerLifeText.text = playerLives.ToString();
            }

            if (i < opponentLife)
            {
                opponentLives.Append('L');
                _opponentLifeText.text = opponentLives.ToString();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_stopCharacterDelay));
        }

            StringBuilder completeMessage = new StringBuilder();
            int index = 0;

            while (index < roundText.Length)
            {
                char appendingChar = roundText[index];
                completeMessage.Append(appendingChar);
                _roundText.text = completeMessage.ToString();

                await UniTask.Delay(TimeSpan.FromSeconds(defaultDelay));

                index++;
            }

    }

}
