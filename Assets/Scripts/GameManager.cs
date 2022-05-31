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
        playerController.OnPokymonEncountered += StartWildPokymonBattle;
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

        var playerParty = playerController.GetComponent<PokymonParty>();
        var wildPokymon = FindObjectOfType<PokymonArea>().GetComponent<PokymonArea>().GetRandomWildPokymon();
        var wildPokymonCopy = new Pokymon(wildPokymon.Base, wildPokymon.Level);

        worldCamera.gameObject.SetActive(false);
        battleManager.gameObject.SetActive(true);
        battleManager.HandleStart(BattleType.WildPokymon, playerParty, wildPokymonCopy);
    }
}

public enum GameState
{
    World,
    Battle,
}
