using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleDialogBox _dialogBox;
    [SerializeField] private PartySelection _partySelection;
    [SerializeField] private ForgetMoveSelection _forgetMoveSelection;

    [SerializeField] private BattleUnit _enemyUnit;
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private GameObject _pokyball;

    [SerializeField] private AudioClip _battleStartSFX;
    [SerializeField] private AudioClip _damageSFX;
    [SerializeField] private AudioClip _forgetMoveSFX;
    [SerializeField] private AudioClip _knockOutSFX;
    [SerializeField] private AudioClip _learnMoveSFX;
    [SerializeField] private AudioClip _levelUpSFX;
    [SerializeField] private AudioClip _physicalMoveSFX;
    [SerializeField] private AudioClip _pokyballCaptureSFX;
    [SerializeField] private AudioClip _pokyballEscapeSFX;
    [SerializeField] private AudioClip _pokyballShakeSFX;
    [SerializeField] private AudioClip _pokyballThrowSFX;
    [SerializeField] private AudioClip _decreaseStat;
    [SerializeField] private AudioClip _increaseStat;
    [SerializeField] private AudioClip _uiCancelSFX;
    [SerializeField] private AudioClip _uiMoveSFX;
    [SerializeField] private AudioClip _uiSubmitSFX;

    private BattleState _battleState;
    private BattleType _battleType;
    private PokymonParty _playerParty;
    private int _currSelectedAction;
    private int _currSelectedMove;
    private int _currSelectedPokymon;
    private float _lastInputTime;
    private LearnableMove _learnableMove;
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

        AudioManager.SharedInstance.PlaySFX(_battleStartSFX);

        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate() {
        if (Time.time < _lastInputTime + Constants.INPUT_DELAY_SECS)
        {
            return;
        }

        if (Input.anyKey || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            _lastInputTime = Time.time;

            if (_battleState == BattleState.PlayerSelectAction || _battleState == BattleState.PlayerSelectMove || _battleState == BattleState.PlayerSelectParty || _battleState == BattleState.PlayerSelectForgetMove)
            {
                if (Input.GetButtonDown("Submit"))
                {
                    AudioManager.SharedInstance.PlaySFX(_uiSubmitSFX);
                }
                else if (Input.GetButtonDown("Cancel"))
                {
                    AudioManager.SharedInstance.PlaySFX(_uiCancelSFX);
                }
                else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    AudioManager.SharedInstance.PlaySFX(_uiMoveSFX);
                }

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

                    case BattleState.PlayerSelectForgetMove:
                        _forgetMoveSelection.HandlePlayerSelectForgetMove((selectedMove) => {
                            _forgetMoveSelection.gameObject.SetActive(false);

                            StartCoroutine(PerformForgetMove(selectedMove));
                        });
                        break;

                    default:
                        break;
                }
            }
        }
    }

    private void HandlePlayerActionSelection()
    {
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

    private IEnumerator SetupBattle()
    {
        if (_battleType == BattleType.WildPokymon)
        {
            yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} appears!");
        }

        yield return ChooseNextTurn();
    }

    private void SetupPlayerPokymon(Pokymon playerPokymon)
    {
        _playerUnit.Pokymon = playerPokymon;
        _playerUnit.SetupPokymon();

        _dialogBox.SetMoveTexts(_playerUnit.Pokymon.MoveList);
    }

    private IEnumerator ChooseNextTurn()
    {
        if (_playerUnit.Pokymon.Speed < _enemyUnit.Pokymon.Speed)
        {
            yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} is faster!");

            EnemyMove();
        }
        else
        {
            PlayerSelectAction();
        }
    }

    private void CheckForBattleFinish(BattleUnit knockedOutUnit)
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

        _playerParty.PokymonList.ForEach(p => p.OnBattleFinish());

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

    private void PlayerSelectForgetMove()
    {
        _battleState = BattleState.PlayerSelectForgetMove;

        _dialogBox.SetDialogText($"Which move should {_playerUnit.Pokymon.Name} forget?");

        _forgetMoveSelection.gameObject.SetActive(true);
        _forgetMoveSelection.SetMoveTexts(_playerUnit.Pokymon.MoveList, _learnableMove);
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
            StartCoroutine(ShowLostTurnMessage("You can't run away from a duel!"));
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

    private IEnumerator PerformEnemyMove(Move move)
    {
        _battleState = BattleState.PerformEnemyMove;

        yield return PerformMove(move, _enemyUnit, _playerUnit);

        if (_battleState == BattleState.PerformEnemyMove)
        {
            PlayerSelectAction();
        }
    }

    private IEnumerator PerformPlayerMove(Move move)
    {
        _battleState = BattleState.PerformPlayerMove;

        yield return PerformMove(move, _playerUnit, _enemyUnit);

        if (_battleState == BattleState.PerformPlayerMove)
        {
            EnemyMove();
        }
    }

    private IEnumerator PerformMove(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        move.PP--;

        yield return _dialogBox.SetDialogText($"{sourceUnit.Pokymon.Name} used {move.Base.Name}.");

        if (move.IsPhysicalMove || move.IsSpecialMove)
        {
            yield return PerformPhysicalMove(move, sourceUnit, targetUnit);
        }

        if (targetUnit.Pokymon.IsKnockedOut)
        {
            yield return PerformKnockOut(targetUnit);
        } else {
            if (move.HasStatModifierEffects)
            {
                yield return ApplyStatMofidierEffects(move, sourceUnit, targetUnit);
            }

            if (move.HasStatusConditionEffects)
            {
                yield return ApplyStatusConditionEffects(move, sourceUnit, targetUnit);
            }
        }
    }

    private IEnumerator PerformPhysicalMove(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        AudioManager.SharedInstance.PlaySFX(_physicalMoveSFX);

        sourceUnit.PlayPhysicalMoveAnimation();
        yield return targetUnit.PlayReceivePhysicalMoveAnimation(move.Base.Type);

        AudioManager.SharedInstance.PlaySFX(_damageSFX);

        var damageDesc = targetUnit.Pokymon.CalculateDamage(sourceUnit.Pokymon, move);
        targetUnit.HUD.UpdateHPTextAnimated(targetUnit.Pokymon.HP + damageDesc.HP);
        targetUnit.HUD.UpdateHPBarAnimated();

        yield return _dialogBox.SetDialogText($"{targetUnit.Pokymon.Name} took {damageDesc.HP} HP damage.");
        yield return ShowDamageDescription(damageDesc);
    }

    private IEnumerator ApplyStatusConditionEffects(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        var addedStatusConditionQueue = new Queue<string>();

        foreach (var effect in move.Base.StatusConditionEffectList)
        {
            if (StatusConditionFactory.StatusConditionList.ContainsKey(effect.ConditionID))
            {
                if (Random.Range(0f, 100f) < effect.Probability)
                {
                    BattleUnit effectTarget = null;

                    switch (effect.Target)
                    {
                        case EffectTarget.Self:
                            effectTarget = sourceUnit;
                            break;

                        case EffectTarget.Foe:
                            effectTarget = targetUnit;
                            break;

                        case EffectTarget.Ally:
                            // TODO: implement allies
                            continue;
                    }

                    var statusCondition = StatusConditionFactory.StatusConditionList[effect.ConditionID];
                    var isConditionAdded = targetUnit.Pokymon.AddStatusCondition(statusCondition);

                    if (isConditionAdded)
                    {
                        addedStatusConditionQueue.Enqueue($"{effectTarget.Pokymon.Name} {statusCondition.StartMessage}");
                    }
                }
            }
            else
            {
                Debug.Log($"Status condition not implemented: {effect.ConditionID}");
            }
        }

        yield return ShowMessageQueue(addedStatusConditionQueue);
    }

    private IEnumerator ApplyStatMofidierEffects(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        var modifiedStatQueue = new Queue<string>();

        foreach (var effect in move.Base.StatModifierEffectList)
        {
            BattleUnit effectTarget = null;

            // TODO: check if move ID has a custom effect (i.e. 18, 46, 47, 48, 50, 54, 73, 77, 78, 79, 86, 92, 95, 100, 102, 105, 109, 113, 114, 115, 116, 118, 182, 186, 235, 240, 388...)
            sourceUnit.PlayStatusMoveAnimation();

            AudioManager.SharedInstance.PlaySFX(effect.Modifier > 0
                ? _increaseStat
                : _decreaseStat);

            switch (effect.Target)
            {
                case EffectTarget.Ally:
                    // TODO: implement allies
                    continue;

                case EffectTarget.Foe:
                    effectTarget = targetUnit;
                    break;

                case EffectTarget.Self:
                    effectTarget = sourceUnit;
                    break;
            }

            var incOrDec = effect.Modifier > 0 ? "increased" : "decreased";
            var isStatModified = effectTarget.Pokymon.ModifyStatStage(effect.Stat, effect.Modifier);

            if (!isStatModified)
            {
                incOrDec = effect.Modifier > 0 ? "already at its maximum" : "already at its minimum";
            }

            modifiedStatQueue.Enqueue($"{effectTarget.Pokymon.Name} {effect.Stat.ToString()} was {incOrDec}.");

            yield return effectTarget.PlayReceiveStatModifierEffectAnimation(move.Base.Type);
        }

        yield return ShowMessageQueue(modifiedStatQueue);
    }

    private IEnumerator ShowDamageDescription(DamageDescription desc)
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

    private IEnumerator ShowMessageQueue(Queue<string> messages)
    {
        while (messages.Count > 0)
        {
            var message = messages.Dequeue();

            yield return _dialogBox.SetDialogText(message);
        }
    }

    private IEnumerator PerformKnockOut(BattleUnit battleUnit)
    {
        AudioManager.SharedInstance.PlaySFX(_knockOutSFX);

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
                AudioManager.SharedInstance.PlaySFX(_levelUpSFX);

                _playerUnit.HUD.UpdateExpBarAnimated(true);
                _playerUnit.HUD.UpdateHPTextAnimated(prevHp);
                _playerUnit.HUD.UpdateHPBarAnimated();
                _playerUnit.HUD.UpdateLevelText();

                yield return _dialogBox.SetDialogText($"{_playerUnit.Pokymon.Name} leveled up!");

                _learnableMove = _playerUnit.Pokymon.LearnableMove;

                if (_learnableMove != null)
                {
                    if (_playerUnit.Pokymon.HasFreeMoveSlot)
                    {
                        _playerUnit.Pokymon.LearnMove(_learnableMove);
                        _dialogBox.SetMoveTexts(_playerUnit.Pokymon.MoveList);

                        AudioManager.SharedInstance.PlaySFX(_learnMoveSFX);

                        yield return _dialogBox.SetDialogText($"{_playerUnit.Pokymon.Name} learned {_learnableMove.Base.Name}!");
                    }
                    else
                    {
                        yield return _dialogBox.SetDialogText($"{_playerUnit.Pokymon.Name} tried to learn {_learnableMove.Base.Name}, but it can't learn more than {Constants.MAX_POKYMON_MOVE_COUNT} moves!");

                        PlayerSelectForgetMove();

                        yield return new WaitUntil(() => _battleState != BattleState.PlayerSelectForgetMove);
                    }
                }

                prevHp = _playerUnit.Pokymon.HP;
            }
        }

        CheckForBattleFinish(battleUnit);
    }

    private IEnumerator PerformPokymonSwitch(Pokymon nextPlayerPokymon)
    {
        _battleState = BattleState.Busy;

        _partySelection.gameObject.SetActive(false);
        _dialogBox.ToggleActionSelector(false);
        _dialogBox.ToggleDialogText(true);

        var isCurrentPokymonKnockedOut = true;

        if (!_playerUnit.Pokymon.IsKnockedOut)
        {
            isCurrentPokymonKnockedOut = false;

            _playerUnit.PlaySwitchAnimation();

            yield return _dialogBox.SetDialogText($"Come back, {_playerUnit.Pokymon.Name}!");
        }

        SetupPlayerPokymon(nextPlayerPokymon);

        yield return _dialogBox.SetDialogText(_playerParty.AvailablePokymonCount == 1
            ? $"You're my only hope, {_playerUnit.Pokymon.Name}!"
            : $"I choose you, {_playerUnit.Pokymon.Name}!"
        );

        if (isCurrentPokymonKnockedOut)
        {
            yield return ChooseNextTurn();
        }
        else
        {
            EnemyMove();
        }
    }

    private IEnumerator PerformPokyballThrow()
    {
        _battleState = BattleState.Busy;

        _dialogBox.SetDialogText($"You threw a {_pokyball.name}...");

        AudioManager.SharedInstance.PlaySFX(_pokyballThrowSFX);

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

            AudioManager.SharedInstance.PlaySFX(_pokyballShakeSFX);

            yield return pokyballSprite.transform.DOPunchRotation(new Vector3(0, 0, 15), 0.6f).WaitForCompletion();
        }

        yield return new WaitForSecondsRealtime(0.75f);

        if (captureDescription.IsCaptured)
        {
            AudioManager.SharedInstance.PlaySFX(_pokyballCaptureSFX);

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
            AudioManager.SharedInstance.PlaySFX(_pokyballEscapeSFX);

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

    private IEnumerator PerformPlayerRun()
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

    private IEnumerator PerformForgetMove(int selectedMove)
    {
        if (selectedMove == Constants.MAX_POKYMON_MOVE_COUNT)
        {
            yield return _dialogBox.SetDialogText($"{_playerUnit.Pokymon.Name} didn't learn {_learnableMove.Base.Name}.");
        }
        else
        {
            var oldMove = _playerUnit.Pokymon.MoveList[selectedMove];

            AudioManager.SharedInstance.PlaySFX(_forgetMoveSFX);

            yield return _dialogBox.SetDialogText($"1, 2, and... ... ... Poof! {_playerUnit.Pokymon.Name} forgot {oldMove.Base.Name}.");

            AudioManager.SharedInstance.PlaySFX(_learnMoveSFX);

            yield return _dialogBox.SetDialogText($"And... {_playerUnit.Pokymon.Name} learned {_learnableMove.Base.Name}.");

            _playerUnit.Pokymon.MoveList[selectedMove] = new Move(_learnableMove.Base);
        }

        _learnableMove = null;
        _battleState = BattleState.PerformPlayerMove;
    }

    private IEnumerator ShowLostTurnMessage(string message)
    {
        yield return _dialogBox.SetDialogText(message);

        EnemyMove();
    }

    private IEnumerator ShowPlayerRunSuccessMessage()
    {
        yield return _dialogBox.SetDialogText("You managed to run away...");

        FinishBattle(false);
    }
}

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
    PlayerSelectForgetMove,
    PerformEnemyMove,
    PerformPlayerMove,
    Busy,
}
