using Running.Game;
using Running.GamePhase;
using UnityEngine;
using Zenject;

namespace Running.Cards
{
    public class CardHighlight : MonoBehaviour
    {
        [SerializeField] private GameObject _outline;
        [SerializeField] private bool IsOperationCard;

        [Inject] private GameController _gameController;

        public bool IsAbleToSelect = true;

        void OnMouseEnter()
        {
            if (!IsAbleToSelect)
            {
                return;
            }

            _outline.SetActive(true);
        }

        void OnMouseExit()
        {
            _outline.SetActive(false);
        }

        public void ToggleOutline()
        {
            _outline.gameObject.SetActive(!_outline.gameObject.activeSelf);
        }
    }
}