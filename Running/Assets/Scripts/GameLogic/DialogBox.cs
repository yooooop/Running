using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Text;
using System;

public class DialogBox : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogBoxText;
    [SerializeField] private float _messageSmootheningEffectSpeed = 20;
    [SerializeField] private float _stopCharacterDelay = 0.3f;

    public event EventHandler<EventArgs> MessageFinishedWritingEvent;

    public void Setup(string text)
    {
        SetMessageSmoothAsync(text).Forget();
    }

    private async UniTaskVoid SetMessageSmoothAsync(string message)
    {
        StringBuilder completeMessage = new StringBuilder();
        int index = 0;
        float defaultDelay = 1 / _messageSmootheningEffectSpeed;

        while (index < message.Length)
        {
            char appendingChar = message[index];
            completeMessage.Append(appendingChar);
            _dialogBoxText.text = completeMessage.ToString();

            float delay = appendingChar is '.' or '!' or '?' ? _stopCharacterDelay : defaultDelay;
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            index++;
        }

        await UniTask.Delay(2000);

        gameObject.SetActive(false);

        MessageFinishedWritingEvent?.Invoke(this, EventArgs.Empty);
    }
}
