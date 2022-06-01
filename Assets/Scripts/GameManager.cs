using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private Camera _worldCamera;
    [SerializeField] private PlayerController _playerController;

    private GameState _gameState;

    private void Awake() {
        _gameState = GameState.World;
    }

    private void Start() {
        _playerController.OnPokymonEncountered += StartWildPokymonBattle;
        _battleManager.OnBattleFinish += FinishPokymonBattle;
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
    }

    private void StartWildPokymonBattle()
    {
        _gameState = GameState.Battle;

        var playerParty = _playerController.GetComponent<PokymonParty>();
        var wildPokymon = FindObjectOfType<PokymonArea>().GetComponent<PokymonArea>().GetRandomWildPokymon();
        var wildPokymonCopy = new Pokymon(wildPokymon.Base, wildPokymon.Level, true);

        _worldCamera.gameObject.SetActive(false);
        _battleManager.gameObject.SetActive(true);
        _battleManager.HandleStart(BattleType.WildPokymon, playerParty, wildPokymonCopy);
    }
}

public enum GameState
{
    World,
    Battle,
}
