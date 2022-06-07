using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Camera _worldCamera;
    [SerializeField] private AudioClip _battleMusic;
    [SerializeField] private AudioClip _worldMusic;

    private GameState _gameState;

    private void Awake() {
        _gameState = GameState.World;
    }

    private void Start() {
        StatusConditionFactory.InitFactory();

        _playerController.OnEncounterPokymon += StartWildPokymonBattle;
        _battleManager.OnBattleFinish += FinishPokymonBattle;

        if (_gameState == GameState.World)
        {
            AudioManager.SharedInstance.PlayMusic(_worldMusic);
        }
    }

    private void Update() {
        if (_gameState == GameState.World)
        {
            _playerController.HandleUpdate();
        } else if (_gameState == GameState.Battle)
        {
            _battleManager.HandleUpdate();
        }
    }

    private void FinishPokymonBattle(bool hasPlayerWon)
    {
        _gameState = GameState.World;

        _battleManager.gameObject.SetActive(false);
        _worldCamera.gameObject.SetActive(true);

        if (hasPlayerWon)
        {
            // TODO: handle player victory
        }
        else
        {
            // TODO: handle player defeat
        }

        AudioManager.SharedInstance.PlayMusic(_worldMusic);
    }

    private void StartWildPokymonBattle()
    {
        var playerParty = _playerController.GetComponent<PokymonParty>();

        if (!playerParty.HasAnyPokymonAvailable)
        {
            // TODO: implement no available pokymon behavior
            print("There are no pokymon left to fight the enemy!");

            return;
        }

        var wildPokymon = FindObjectOfType<PokymonArea>().GetComponent<PokymonArea>().GetRandomWildPokymon();
        var wildPokymonCopy = new Pokymon(wildPokymon.Base, wildPokymon.Level, true);

        _gameState = GameState.Battle;

        _worldCamera.gameObject.SetActive(false);
        _battleManager.gameObject.SetActive(true);
        _battleManager.HandleStart(BattleType.WildPokymon, playerParty, wildPokymonCopy);

        AudioManager.SharedInstance.PlayMusic(_battleMusic);
    }
}

public enum GameState
{
    World,
    Battle,
}
