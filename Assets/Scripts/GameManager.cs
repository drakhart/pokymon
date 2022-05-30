using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Camera worldCamera;

    private GameState _gameState;

    private void Awake() {
        _gameState = GameState.World;
    }

    private void Start() {
        playerController.OnPokymonEncountered += StartPokymonBattle;
        battleManager.OnBattleFinish += FinishPokymonBattle;
    }

    private void Update() {
        if (_gameState == GameState.World)
        {
            playerController.HandleUpdate();
        } else if (_gameState == GameState.Battle)
        {
            battleManager.HandleUpdate();
        }
    }

    private void FinishPokymonBattle(bool hasPlayerWon)
    {
        _gameState = GameState.World;

        battleManager.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        if (!hasPlayerWon)
        {
            // TODO: handle player defeat
        }
    }

    private void StartPokymonBattle()
    {
        _gameState = GameState.Battle;

        var playerParty = playerController.GetComponent<PokymonParty>();
        var enemyPokymon = FindObjectOfType<PokymonArea>().GetComponent<PokymonArea>().GetRandomWildPokymon();

        worldCamera.gameObject.SetActive(false);
        battleManager.gameObject.SetActive(true);
        battleManager.HandleStart(playerParty, enemyPokymon);
    }
}

public enum GameState
{
    World,
    Battle,
}
