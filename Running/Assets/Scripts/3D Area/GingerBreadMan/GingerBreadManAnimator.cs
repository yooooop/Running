using Cysharp.Threading.Tasks;
using Running.BodyPart;
using Running.Cards;
using Running.Game;
using Running.Operations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Running.Player
{
    public class GingerBreadManAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _hand;
        [SerializeField] private List<BodyPartScriptableObject> _bodyPartList;
        [SerializeField] private List<NumberCard> _numberCardList;
        [SerializeField] private List<OperationCard> _operationCardList;

        private NumberCard _selectedNumber;
        private OperationCard _selectedOperation;
        private BodyPartScriptableObject _selectedBodyPart;

        private NumberCardObjectPrefab _savedNumberInstance;
        private OperationCardObjectPrefab _savedOperationInstance;
        private BodyPartPrefab _savedBodyPartInstance;



        [Inject] private GameController _gameController;
        [Inject] private DiContainer _container;
        [Inject] private AnimatorController _animatorController;


        [Inject]
        private void OnInjected()
        {
            _gameController.OpponentNumberCardSelectedEvent += OpponentNumberCadSelected;
            _gameController.OpponentOperationCardSelectedEvent += OpponentOperationCardSelected;
            _gameController.OpponentOrganWageredEvent += OrganWagered;
        }

        private void OpponentNumberCadSelected(object sender, int number)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            _animator.SetTrigger("PickUpCard");
            foreach (NumberCard card in _numberCardList)
            {
                if (card.value == number)
                {
                    _selectedNumber = card;
                    break;
                }
            }
        }

        private void OpponentOperationCardSelected(object sender, OperationType operationType)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            _animator.SetTrigger("PickUpOperation");
            foreach (OperationCard card in _operationCardList)
            {
                if (card.sign == operationType)
                {
                    _selectedOperation = card;
                    break;
                }
            }
        }

        private void OrganWagered(object sender, List<BodyPartType> list)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            float length = GetAnimationClipLength("WagerOrgan");
            length += GetAnimationClipLength("Idle");
            length *= 1000;

            /*List<BodyPartType> test = new List<BodyPartType> {
                BodyPartType.Brain,
                BodyPartType.FingerLeft,
                BodyPartType.FingerRight,
                BodyPartType.Heart,
                BodyPartType.Eye,
                BodyPartType.Lung,
                BodyPartType.Kidney,
            };*/

            PlayWagerOrganAnimation(list, (int)length).Forget();
        }

        private async UniTaskVoid PlayWagerOrganAnimation(List<BodyPartType> list, int length)
        {
            Debug.LogError("test start");
            foreach (BodyPartType type in list)
            {
                foreach (BodyPartScriptableObject obj in _bodyPartList)
                {
                    if (obj.BodyPartType == type)
                    {
                        _selectedBodyPart = obj;
                        _animator.SetTrigger("WagerOrgan");
                        
                        await UniTask.Delay(length);

                        _animator.ResetTrigger("WagerOrgan");
                        _animator.Play("Idle");
                        break;
                    }
                }
            }

            // finished organ wager event
            _animatorController.FinishedPickUpCard();
        }

        public void SpawnOrgan()
        {
            if (_selectedBodyPart != null)
            {
                BodyPartPrefab instance = _container.InstantiatePrefabForComponent<BodyPartPrefab>(_selectedBodyPart.BodyPartPrefab, _hand);

                instance.transform.position = instance.transform.position + new Vector3(3f, 0f, 0f);
                //instance.transform.rotation = Quaternion.Euler(0, 90, 90);
                instance.transform.localScale *= 0.2f;

                _savedBodyPartInstance = instance;
                _animatorController.OrganUsed(_selectedBodyPart.BodyPartType);
            }
        }

        public void DeSpawnOrgan()
        {
            if (_savedBodyPartInstance != null)
            {
                Destroy(_savedBodyPartInstance.gameObject);
            }

            _animatorController.OrganPlaced(_selectedBodyPart.BodyPartType);
            _selectedBodyPart = null;
        }

        public void SpawnNumberCard()
        {
            if (_selectedNumber != null)
            {
                NumberCardObjectPrefab instance = _container.InstantiatePrefabForComponent<NumberCardObjectPrefab>(_selectedNumber.NumberCardPrefab, _hand);

                instance.CardHighlight.ToggleOutline();
                instance.transform.position = instance.transform.position + new Vector3(2f, 0f, 0f);
                instance.transform.rotation = Quaternion.Euler(0, 90, 90);
                instance.transform.localScale *= 0.005f;

                instance.CardHighlight.IsAbleToSelect = false;
                instance.CardClickHandler.IsAbleToSelect = false;

                _savedNumberInstance = instance;

                _animatorController.PickedUpCard(_selectedNumber.value);
            }
        }

        public void DeSpawnNumberCard()
        {
            if (_savedNumberInstance != null)
            {
                Destroy(_savedNumberInstance.gameObject);
            }
            _selectedNumber = null;
            _animatorController.FinishedPickUpCard();
            _animator.ResetTrigger("PickUpCard");
            _animator.Play("Idle");
        }

        public void SpawnOperationCard()
        {
            if (_selectedOperation != null)
            {
                OperationCardObjectPrefab instance = _container.InstantiatePrefabForComponent<OperationCardObjectPrefab>(_selectedOperation.OperationCardPrefab, _hand);

                instance.CardHighlight.ToggleOutline();
                instance.transform.position = instance.transform.position + new Vector3(2f, 0f, 0f);
                instance.transform.rotation = Quaternion.Euler(0, -90, 90);
                instance.transform.localScale *= 0.005f;

                instance.CardHighlight.IsAbleToSelect = false;
                instance.CardClickHandler.IsAbleToSelect = false;

                _savedOperationInstance = instance;
            }
        }

        public void DeSpawnOperationCard()
        {
            if (_savedOperationInstance != null)
            {
                _animatorController.PickedUpOperation(_selectedOperation.sign);
                Destroy(_savedOperationInstance.gameObject);
            }
            _selectedOperation = null;
            _animatorController.FinishedPickUpCard();
            _animator.ResetTrigger("PickUpOperation");
        }

        private float GetAnimationClipLength(string animationName)
        {
            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == animationName)
                {
                    return clip.length;
                }
            }
            return 0f;
        }

        public void PlayWinningAnimation()
        {
            if (_animator != null)
            {
                //_animator.SetTrigger("WinAnimation");
            }
        }

    }
}
