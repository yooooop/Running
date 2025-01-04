using Running.BodyPart;
using Running.Game;
using Running.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Scale : MonoBehaviour
{
    [SerializeField] private Transform _playerScale;
    [SerializeField] private Transform _opponentScale;
    [SerializeField] private List<BodyPartScriptableObject> _bodyPartList;

    private List<BodyPartPrefab> _instances = new List<BodyPartPrefab>();

    [Inject] private AnimatorController _animatorController;
    [Inject] private GameController _gameController;
    [Inject] private DiContainer _container;

    [Inject]
    private void OnInjected()
    {
        _animatorController.OrganPlacedEvent += OpponentOrganSpawn;
        _gameController.PlayerOrganWageredEvent += PlayerOrganSpawn;
        _gameController.ResetRoundEvent += DestroyOrgans;
    }

    private void OpponentOrganSpawn(object sender, BodyPartType type)
    {
        Debug.LogError("opponent organ spawn");
        foreach (BodyPartScriptableObject obj in _bodyPartList)
        {
            if (obj.BodyPartType == type)
            {
                BodyPartPrefab instance = _container.InstantiatePrefabForComponent<BodyPartPrefab>(obj.BodyPartPrefab, _opponentScale);
                instance.transform.localScale *= 3f;
                if (type == BodyPartType.Eye)
                {
                    instance.transform.localScale *= 10f;
                }
                instance.transform.rotation = Quaternion.Euler(-50, -45, -30);
                instance.ToggleBody();
                _instances.Add(instance);
                break;
            }
        }
    }

    private void PlayerOrganSpawn(object sender, BodyPartType type)
    {
        foreach (BodyPartScriptableObject obj in _bodyPartList)
        {
            if (obj.BodyPartType == type)
            {
                BodyPartPrefab instance = _container.InstantiatePrefabForComponent<BodyPartPrefab>(obj.BodyPartPrefab, _playerScale);
                instance.transform.localScale *= 3f;
                if (type == BodyPartType.Eye)
                {
                    instance.transform.localScale *= 10f;
                }
                instance.transform.rotation = Quaternion.Euler(-50, -45, -30);
                instance.ToggleBody();
                _instances.Add(instance);
                break;
            }
        }
    }

    private void DestroyOrgans(object sender, EventArgs e)
    {
        foreach (BodyPartPrefab prefab in _instances)
        {
            Destroy(prefab.gameObject);
        }
        _instances.Clear();
    }

}
