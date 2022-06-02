using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public enum BattleType
{
    WildPokymon,
    PokymonTrainer,
    GymLeader,
}

public enum BattleState
{
    StartBattle,
    FinishBattle,
    PlayerSelectAction,
    PlayerSelectMove,
    PlayerSelectParty,
    PlayerSelectInventory,
    PerformEnemyMove,
    PerformPlayerMove,
    Busy,
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleDialogBox _dialogBox;
    [SerializeField] private PartySelection _partySelection;

    [SerializeField] private BattleUnit _enemyUnit;
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private GameObject _pokyball;

    [SerializeField] private float _inputDelaySecs = 0.25f;

    private BattleState _battleState;
    private BattleType _battleType;
    private PokymonParty _playerParty;
    private int _currSelectedAction;
    private int _currSelectedMove;
    private int _currSelectedPokymon;
    private float _timeSinceLastInput;
    private int _escapeAttempts;

    public event Action<bool> OnBattleFinish;

    public void HandleStart(BattleType type, PokymonParty playerParty, Pokymon enemyPokymon) {
        _battleType = type;
        _playerParty = playerParty;

        _battleState = BattleState.StartBattle;
        _escapeAttempts = 0;

        _partySelection.SetupPartySelection();
        SetupPlayerPokymon(_playerParty.FirstAvailablePokymon);

        _enemyUnit.Pokymon = enemyPokymon;
        _enemyUnit.SetupPokymon();

        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate() {
        switch (_battleState)
        {
            case BattleState.PlayerSelectAction:
                HandlePlayerActionSelection();
                break;

            case BattleState.PlayerSelectMove:
                HandlePlayerMoveSelection();
                break;

            case BattleState.PlayerSelectParty:
                HandlePartySelection();
                break;

            default:
                break;
        }
    }

    private void HandlePlayerActionSelection()
    {
        if (Time.time < _timeSinceLastInput + _inputDelaySecs)
        {
            return;
        }

        if (Input.anyKey || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            _timeSinceLastInput = Time.time;
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                _currSelectedAction = (_currSelectedAction + 1) % 2 + (_currSelectedAction >= 2 ? 2 : 0);
            }
            else
            {
                _currSelectedAction = (_currSelectedAction + 2) % 4;
            }

            _dialogBox.SelectAction(_currSelectedAction);
        }

        if (Input.GetButtonDown("Submit"))
        {
            switch (_currSelectedAction)
            {
                case 0: // Fight
                    PlayerSelectMove();
                    break;

                case 1: // Pokymon selection
                    PlayerSelectParty();
                    break;

                case 2: // Bag
                    PlayerSelectInventory();
                    break;

                case 3: // Run
                    PlayerRun();
                    break;

                default:
                    break;
            }
        }
    }

    private void HandlePlayerMoveSelection()
    {
        if (Time.time < _timeSinceLastInput + _inputDelaySecs)
        {
            return;
        }

        if (Input.anyKey || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            _timeSinceLastInput = Time.time;
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                _currSelectedMove = (_currSelectedMove + 1) % 2 + (_currSelectedMove >= 2 ? 2 : 0);
            }
            else
            {
                _currSelectedMove = (_currSelectedMove + 2) % Constants.MAX_POKYMON_MOVE_COUNT;
            }

            _currSelectedMove = Mathf.Clamp(_currSelectedMove, 0, _playerUnit.Pokymon.MoveCount - 1);

            _dialogBox.SelectMove(_currSelectedMove);
            _dialogBox.SetMoveDetails(_playerUnit.Pokymon.MoveList[_currSelectedMove]);
        }

        if (Input.GetButtonDown("Submit"))
        {
            PlayerMove();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            PlayerSelectAction();
        }
    }

    private void HandlePartySelection()
    {
        if (Time.time < _timeSinceLastInput + _inputDelaySecs)
        {
            return;
        }

        if (Input.anyKey || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            _timeSinceLastInput = Time.time;
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                _currSelectedPokymon = (_currSelectedPokymon + 1) % 2 + 2 * (int)(_currSelectedPokymon / 2);
            }
            else
            {
                _currSelectedPokymon = (_currSelectedPokymon + (Input.GetAxis("Vertical") < 0 ? 2 : _playerParty.PokymonCount - 2 + _playerParty.PokymonCount % 2))
                    % (_playerParty.PokymonCount % 2 == 0 ? _playerParty.PokymonCount : _playerParty.PokymonCount + 1);
            }

            _currSelectedPokymon = Mathf.Clamp(_currSelectedPokymon, 0, _playerParty.PokymonCount - 1);

            _partySelection.SelectPokymon(_currSelectedPokymon);
        }

        if (Input.GetButtonDown("Submit"))
        {
            var nextPlayerPokymon = _playerParty.PokymonList[_currSelectedPokymon];

            if (nextPlayerPokymon.IsKnockedOut)
            {
                _partySelection.SetDialogText("You can't choose a knocked out Pokymon.");

                return;
            }
            else if (nextPlayerPokymon == _playerUnit.Pokymon)
            {
                _partySelection.SetDialogText("You can't choose the Pokymon currently in battle.");

                return;
            }

            StartCoroutine(PerformPokymonSwitch(nextPlayerPokymon));
        }

        if (Input.GetButtonDown("Cancel"))
        {
            PlayerSelectAction();
        }
    }

    public IEnumerator SetupBattle()
    {
        if (_battleType == BattleType.WildPokymon)
        {
            yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} appears!");
        }

        if (_playerUnit.Pokymon.Speed < _enemyUnit.Pokymon.Speed)
        {
            yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} is fast!");

            EnemyMove();
        }
        else
        {
            PlayerSelectAction();
        }
    }

    private void SetupPlayerPokymon(Pokymon playerPokymon)
    {
        _playerUnit.Pokymon = playerPokymon;
        _playerUnit.SetupPokymon();

        _dialogBox.SetMoveTexts(_playerUnit.Pokymon.MoveList);
    }

    void CheckForBattleFinish(BattleUnit knockedOutUnit)
    {
        if (knockedOutUnit.IsPlayer)
        {
            if (_playerParty.HasAnyPokymonAvailable)
            {
                if (_playerParty.AvailablePokymonCount == 1)
                {
                    StartCoroutine(PerformPokymonSwitch(_playerParty.FirstAvailablePokymon));
                }
                else
                {
                    PlayerSelectParty();
                }
            }
            else
            {
                FinishBattle(false);
            }
        } else {
            FinishBattle(true);
        }
    }

    private void FinishBattle(bool hasPlayerWon)
    {
        _battleState = BattleState.FinishBattle;

        OnBattleFinish(hasPlayerWon);
    }

    private void PlayerSelectAction()
    {
        _battleState = BattleState.PlayerSelectAction;
        _currSelectedAction = 0;

        _partySelection.gameObject.SetActive(false);
        _dialogBox.ToggleMoveSelector(false);
        _dialogBox.SetDialogText($"What will {_playerUnit.Pokymon.Name} do?");
        _dialogBox.ToggleDialogText(true);
        _dialogBox.SelectAction(_currSelectedAction);
        _dialogBox.ToggleActionSelector(true);
    }

    private void PlayerSelectMove()
    {
        _battleState = BattleState.PlayerSelectMove;
        _currSelectedMove = 0;

        _dialogBox.ToggleDialogText(false);
        _dialogBox.ToggleActionSelector(false);
        _dialogBox.SelectMove(_currSelectedMove);
        _dialogBox.SetMoveDetails(_playerUnit.Pokymon.MoveList[_currSelectedMove]);
        _dialogBox.ToggleMoveSelector(true);
    }

    private void PlayerSelectParty()
    {
        _battleState = BattleState.PlayerSelectParty;
        _currSelectedPokymon = 0;

        _partySelection.gameObject.SetActive(true);
        _partySelection.UpdatePartyData(_playerParty.PokymonList);
        _partySelection.SelectPokymon(_currSelectedPokymon);
    }

    private void PlayerSelectInventory()
    {
        // TODO: implement actual inventory and items logic
        _battleState = BattleState.Busy;

        _dialogBox.ToggleActionSelector(false);

        if (_battleType == BattleType.WildPokymon)
        {
            StartCoroutine(PerformPokyballThrow());
        }
        else
        {
            StartCoroutine(ShowLostTurnMessage("You can't catch a trained Pokymon!"));
        }
    }

    private void PlayerRun()
    {
        _battleState = BattleState.Busy;

        _dialogBox.ToggleActionSelector(false);

        if (_battleType == BattleType.WildPokymon)
        {
            StartCoroutine(PerformPlayerRun());
        }
        else
        {
            StartCoroutine(ShowLostTurnMessage("You can't run from a duel!"));
        }
    }

    private void EnemyMove()
    {
        _dialogBox.ToggleDialogText(true);

        var move = _enemyUnit.Pokymon.GetRandomAvailableMove();

        StartCoroutine(PerformEnemyMove(move));
    }

    private void PlayerMove()
    {
        var move = _playerUnit.Pokymon.MoveList[_currSelectedMove];

        if (!move.HasAvailablePP)
        {
            return;
        }

        _dialogBox.ToggleMoveSelector(false);
        _dialogBox.ToggleDialogText(true);

        StartCoroutine(PerformPlayerMove(move));
    }

    IEnumerator PerformEnemyMove(Move move)
    {
        _battleState = BattleState.PerformEnemyMove;

        yield return PerformMove(_enemyUnit, _playerUnit, move);

        if (_battleState == BattleState.PerformEnemyMove)
        {
            PlayerSelectAction();
        }
    }

    IEnumerator PerformPlayerMove(Move move)
    {
        _battleState = BattleState.PerformPlayerMove;

        yield return PerformMove(_playerUnit, _enemyUnit, move);

        if (_battleState == BattleState.PerformPlayerMove)
        {
            EnemyMove();
        }
    }

    IEnumerator PerformMove(BattleUnit source, BattleUnit target, Move move)
    {
        move.PP--;

        yield return _dialogBox.SetDialogText($"{source.Pokymon.Name} used {move.Base.Name}.");

        source.PlayPhysicalMoveAnimation();
        target.PlayReceiveDamageAnimation();

        var damageDesc = target.Pokymon.CalculateDamage(source.Pokymon, move);

        target.HUD.UpdateHPTextAnimated(target.Pokymon.HP + damageDesc.HP);
        target.HUD.UpdateHPBarAnimated();

        yield return _dialogBox.SetDialogText($"{target.Pokymon.Name} took {damageDesc.HP} HP damage.");
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.IsKnockedOut)
        {
            yield return PerformKnockOut(target);
        }
    }

    IEnumerator PerformKnockOut(BattleUnit battleUnit)
    {
        battleUnit.PlayKnockedOutAnimation();

        yield return _dialogBox.SetDialogText($"{battleUnit.Pokymon.Name} got knocked out!");

        if (battleUnit.IsPlayer)
        {
            yield return new WaitForSecondsRealtime(1f);
        }

        if (!battleUnit.IsPlayer)
        {
            var prevHp = _playerUnit.Pokymon.HP;
            var earnedExp = battleUnit.Pokymon.KnockOutExp;

            _playerUnit.Pokymon.Exp += earnedExp;
            _playerUnit.HUD.UpdateExpBarAnimated();

            yield return _dialogBox.SetDialogText($"{_playerUnit.Pokymon.Name} earned {earnedExp} EXP.");

            while (_playerUnit.Pokymon.LevelUp())
            {
                _playerUnit.HUD.UpdateExpBarAnimated(true);
                _playerUnit.HUD.UpdateHPTextAnimated(prevHp);
                _playerUnit.HUD.UpdateHPBarAnimated();
                _playerUnit.HUD.UpdateLevelText();

                yield return _dialogBox.SetDialogText($"{_playerUnit.Pokymon.Name} leveled up!");

                // TODO: try to learn new move
                var learnableMove = _playerUnit.Pokymon.LearnableMove;

                if (learnableMove != null)
                {
                    if (_playerUnit.Pokymon.HasFreeMoveSlot)
                    {
                        _playerUnit.Pokymon.LearnMove(learnableMove);
                        _dialogBox.SetMoveTexts(_playerUnit.Pokymon.MoveList);

                        yield return _dialogBox.SetDialogText($"{_playerUnit.Pokymon.Name} learned {learnableMove.Move.Name}!");
                    }
                    else
                    {
                        // TODO: forget move
                    }
                }

                prevHp = _playerUnit.Pokymon.HP;
            }
        }

        CheckForBattleFinish(battleUnit);
    }

    IEnumerator PerformPokymonSwitch(Pokymon nextPlayerPokymon)
    {
        _battleState = BattleState.Busy;

        _partySelection.gameObject.SetActive(false);
        _dialogBox.ToggleActionSelector(false);
        _dialogBox.ToggleDialogText(true);

        if (!_playerUnit.Pokymon.IsKnockedOut)
        {
            _playerUnit.PlaySwitchAnimation();

            yield return _dialogBox.SetDialogText($"Come back, {_playerUnit.Pokymon.Name}!");
        }

        SetupPlayerPokymon(nextPlayerPokymon);

        yield return _dialogBox.SetDialogText(_playerParty.AvailablePokymonCount == 1
            ? $"You're my only hope, {_playerUnit.Pokymon.Name}!"
            : $"I choose you, {_playerUnit.Pokymon.Name}!"
        );

        EnemyMove();
    }

    IEnumerator PerformPokyballThrow()
    {
        _battleState = BattleState.Busy;

        yield return _dialogBox.SetDialogText($"You threw a {_pokyball.name}...");

        var pokyballInstance = Instantiate(_pokyball, _playerUnit.transform.position - new Vector3(3, 1), Quaternion.identity);
        var pokyballSprite = pokyballInstance.GetComponent<SpriteRenderer>();
        pokyballSprite.transform.localScale = new Vector3(2f, 2f, 2f);

        var throwSeq = DOTween.Sequence();
        throwSeq.Append(pokyballSprite.transform.DOLocalJump(_enemyUnit.transform.position + new Vector3(0, 1f), 2.25f, 1, 0.75f));
        throwSeq.Join(pokyballSprite.transform.DORotate(new Vector3(1, 1, -1 * 360), 0.75f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        throwSeq.Join(pokyballSprite.transform.DOScale(new Vector3(1f, 1f, 1f), 0.75f));
        yield return throwSeq.Play().WaitForCompletion();

        yield return _enemyUnit.PlayCaptureAnimation();

        yield return pokyballSprite.transform
            .DOLocalJump(_enemyUnit.transform.position - new Vector3(0, 2.15f), 0.5f, 2, 0.75f)
            .WaitForCompletion();

        var captureDescription = _enemyUnit.Pokymon.CalculateCapture();

        for (var i = 0; i < Mathf.Max(captureDescription.ShakeCount, 1); i++)
        {
            yield return new WaitForSecondsRealtime(0.75f);
            yield return pokyballSprite.transform.DOPunchRotation(new Vector3(0, 0, 15), 0.6f).WaitForCompletion();
        }

        yield return new WaitForSecondsRealtime(0.75f);

        if (captureDescription.IsCaptured)
        {
            yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} caught!");
            yield return pokyballSprite.DOFade(0, 1f).WaitForCompletion();

            if (_playerParty.AddPokymon(_enemyUnit.Pokymon))
            {
                yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} added to party.");
            }
            else
            {
                yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} sent to Bill's PC.");
            }

            Destroy(pokyballInstance);
            FinishBattle(true);
        }
        else
        {
            var destroySeq = DOTween.Sequence();
            destroySeq.Append(pokyballSprite.DOFade(0, 1.5f));
            destroySeq.Join(pokyballSprite.transform.DOLocalJump(_enemyUnit.transform.position + new Vector3(4f, -2.15f), 1.5f, 3, 1.5f));
            yield return destroySeq.Play();

            yield return _enemyUnit.PlayEscapeAnimation();
            yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} escaped!");

            Destroy(pokyballInstance);
            EnemyMove();
        }
    }

    IEnumerator PerformPlayerRun()
    {
        _escapeAttempts++;

        var playerSpeed = _playerUnit.Pokymon.Speed;
        var enemySpeed = _enemyUnit.Pokymon.Speed;

        if (playerSpeed >= enemySpeed)
        {
            yield return ShowPlayerRunSuccessMessage();
        } else {
            var escapeOdds = (playerSpeed * 128 / enemySpeed + 30 * _escapeAttempts) % 256;

            if (escapeOdds >= 255)
            {
                yield return ShowPlayerRunSuccessMessage();
            }
            else if (Random.Range(0, 255) < escapeOdds)
            {
                yield return ShowPlayerRunSuccessMessage();
            }
            else
            {
                yield return ShowLostTurnMessage("You couldn't run away!");
            }
        }
    }

    IEnumerator ShowDamageDescription(DamageDescription desc)
    {
        if (desc.Critical > 1f)
        {
            yield return _dialogBox.SetDialogText("Critical hit!");
        }

        if (desc.Type > 1f)
        {
            yield return _dialogBox.SetDialogText("It's super effective!");
        }
        else if (desc.Type < 1f)
        {
            yield return _dialogBox.SetDialogText("It's not very effective...");
        }
    }

    IEnumerator ShowLostTurnMessage(string message)
    {
        yield return _dialogBox.SetDialogText(message);

        EnemyMove();
    }

    IEnumerator ShowPlayerRunSuccessMessage()
    {
        yield return _dialogBox.SetDialogText("You managed to run away...");

        FinishBattle(false);
    }
}
