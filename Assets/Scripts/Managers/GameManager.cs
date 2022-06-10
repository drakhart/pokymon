using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Camera _worldCamera;
    [SerializeField] private AudioClip _worldMusic;
    [SerializeField] private Image _transitionPanel;

    private GameState _gameState;

    private void Awake() {
        Screen.SetResolution(1440, 1080, true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start() {
        StatusConditionFactory.InitFactory();

        _playerController.OnEncounterPokymon += StartWildPokymonBattle;
        _battleManager.OnBattleFinish += FinishPokymonBattle;

        _transitionPanel.color = new Color32(0x00, 0x00, 0x00, 0xff);

        StartCoroutine(FadeToWorld());
    }

    private void Update() {
        switch(_gameState)
        {
            case GameState.World:
                _playerController.HandleUpdate();
                break;

            case GameState.Battle:
                _battleManager.HandleUpdate();
                break;
        }
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

        StartCoroutine(FadeToBattle(BattleType.WildPokymon, playerParty, wildPokymonCopy));
    }

    private void FinishPokymonBattle(bool hasPlayerWon)
    {
        if (hasPlayerWon)
        {
            // TODO: handle player victory
        }
        else
        {
            // TODO: handle player defeat
        }

        StartCoroutine(FadeToWorld());
    }

    private IEnumerator FadeToBattle(BattleType battleType, PokymonParty playerParty, Pokymon enemyPokymon)
    {
        _gameState = GameState.Battle;

        yield return _transitionPanel.DOFade(1, 0.4f).WaitForCompletion();

        _worldCamera.gameObject.SetActive(false);
        _battleManager.gameObject.SetActive(true);
        _battleManager.HandleStart(battleType, playerParty, enemyPokymon);

        yield return _transitionPanel.DOFade(0, 0.4f).WaitForCompletion();
    }

    private IEnumerator FadeToWorld()
    {
        _gameState = GameState.World;

        yield return _transitionPanel.DOFade(1, 0.4f).WaitForCompletion();

        _battleManager.gameObject.SetActive(false);
        _worldCamera.gameObject.SetActive(true);

        AudioManager.SharedInstance.PlayMusic(_worldMusic);

        yield return _transitionPanel.DOFade(0, 0.4f).WaitForCompletion();
    }
}

public enum GameState
{
    World,
    Battle,
}
