using Running.Ai;
using Running.CameraControl;
using Running.Game;
using Running.GamePhase;
using Running.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GlobalBindsInstaller : MonoInstaller
{
    [SerializeField] private CameraController _cameraController;

    public override void InstallBindings()
    {
        Container.Bind<CameraController>().FromInstance(_cameraController).AsSingle();
        Container.Bind<GameController>().AsSingle().NonLazy();
        Container.Bind<AnimatorController>().AsSingle().NonLazy();
        Container.Bind<PlayerData>().AsSingle().NonLazy();
        Container.Bind<OpponentData>().AsSingle().NonLazy();
        Container.Bind<AiController>().AsSingle().NonLazy();
        Container.Bind<MainGameScreen>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameScreenController>().FromComponentInHierarchy().AsSingle();
    }
}
