using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleDialogBox _dialogBox;
    [SerializeField] private PartySelection _partySelection;
    [SerializeField] private ForgetMoveSelection _forgetMoveSelection;

    [SerializeField] private Image _playerAvatar;
    [SerializeField] private Image _trainerAvatar;

    [SerializeField] private BattleUnit _enemyUnit;
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private GameObject _pokyball;

    [SerializeField] private AudioClip _battleMusic;
    [SerializeField] private AudioClip _battleStartSFX;
    [SerializeField] private AudioClip _evadeMoveSFX;
    [SerializeField] private AudioClip _forgetMoveSFX;
    [SerializeField] private AudioClip _knockOutSFX;
    [SerializeField] private AudioClip _learnMoveSFX;
    [SerializeField] private AudioClip _levelUpSFX;
    [SerializeField] private AudioClip _physicalMoveSFX;
    [SerializeField] private AudioClip _pokyballCaptureSFX;
    [SerializeField] private AudioClip _pokyballEscapeSFX;
    [SerializeField] private AudioClip _pokyballShakeSFX;
    [SerializeField] private AudioClip _pokyballThrowSFX;
    [SerializeField] private AudioClip _statusConditionSFX;
    [SerializeField] private AudioClip _decreaseStatSFX;
    [SerializeField] private AudioClip _increaseStatSFX;
    [SerializeField] private AudioClip _uiCancelSFX;
    [SerializeField] private AudioClip _uiMoveSFX;
    [SerializeField] private AudioClip _uiSubmitSFX;

    private BattleState _battleState;
    private BattleType _battleType;
    private int _escapeAttempts;
    private float _lastInputTime;
    private LearnableMove _learnableMove;
    private PlayerController _player;
    private PokymonParty _playerParty;
    private TrainerController _trainer;
    private PokymonParty _trainerParty;


    public event Action<bool> OnBattleFinish;

    public bool IsTrainerBattle => _battleType == BattleType.Trainer || _battleType == BattleType.GymLeader;
    public bool IsWildPokymonBattle => _battleType == BattleType.WildPokymon;

    public void HandleStart(BattleType type, PokymonParty playerParty, Pokymon enemyPokymon, PokymonParty trainerParty = null) {
        _battleType = type;
        _playerParty = playerParty;
        _enemyUnit.Pokymon = enemyPokymon;
        _player = playerParty.GetComponent<PlayerController>();

        _battleState = BattleState.StartBattle;
        _escapeAttempts = 0;

        _partySelection.SetupPartySelection();

        if (trainerParty != null)
        {
            _trainerParty = trainerParty;
            _trainer = trainerParty.GetComponent<TrainerController>();
        }

        AudioManager.SharedInstance.PlayMusic(_battleMusic);
        AudioManager.SharedInstance.PlaySFX(_battleStartSFX);

        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate() {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Time.time < _lastInputTime + Constants.INPUT_DELAY_SECS)
            {
                return;
            }
            else
            {
                _lastInputTime = Time.time;
            }
        }

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
                    _dialogBox.HandlePlayerActionSelection((selectedAction) =>
                    {
                        _dialogBox.StopDialogText();

                        switch (selectedAction)
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
                        }
                    });
                    break;

                case BattleState.PlayerSelectMove:
                    _dialogBox.HandlePlayerMoveSelection((selectedMove) =>
                    {
                        if (selectedMove == null)
                        {
                            PlayerSelectAction();
                        }
                        else
                        {
                            _playerUnit.NextMove = selectedMove;

                            StartCoroutine(TriggerMoves());
                        }
                    });
                    break;

                case BattleState.PlayerSelectParty:
                    _partySelection.HandlePartySelection(_playerUnit.Pokymon, (selectedPokymon) =>
                    {
                        _partySelection.gameObject.SetActive(false);
                        _partySelection.StopDialogText();

                        if (selectedPokymon == null)
                        {
                            PlayerSelectAction();
                        }
                        else
                        {
                            StartCoroutine(PerformPokymonSwitch(selectedPokymon));
                        }
                    });
                    break;

                case BattleState.PlayerSelectForgetMove:
                    _forgetMoveSelection.HandlePlayerSelectForgetMove((selectedMove) =>
                    {
                        _forgetMoveSelection.gameObject.SetActive(false);

                        StartCoroutine(PerformForgetMove(selectedMove));
                    });
                    break;

                default:
                    break;
            }
        }
    }

    private IEnumerator SetupBattle()
    {
        _playerUnit.Toggle(false);
        _enemyUnit.Toggle(false);

        if (IsWildPokymonBattle)
        {
            _enemyUnit.SetupPokymon();
            yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} appears!");
        }
        else
        {
            _playerAvatar.sprite = _player.Avatar;
            _trainerAvatar.sprite = _trainer.Avatar;

            var playerAvatarInitialPosition = _playerAvatar.transform.localPosition;
            _playerAvatar.transform.localPosition = playerAvatarInitialPosition - new Vector3(400, 0, 0);
            _playerAvatar.transform.DOLocalMoveX(playerAvatarInitialPosition.x, 1);
            _playerAvatar.gameObject.SetActive(true);

            var trainerAvatarInitialPosition = _trainerAvatar.transform.localPosition;
            _trainerAvatar.transform.localPosition = trainerAvatarInitialPosition + new Vector3(400, 0, 0);
            _trainerAvatar.transform.DOLocalMoveX(trainerAvatarInitialPosition.x, 1);
            _trainerAvatar.gameObject.SetActive(true);

            yield return _dialogBox.SetDialogText($"{_trainer.Name} wants to fight!");
            yield return _trainerAvatar.transform.DOLocalMoveX(trainerAvatarInitialPosition.x + 400, 1).WaitForCompletion();

            _trainerAvatar.gameObject.SetActive(false);
            _trainerAvatar.transform.localPosition = trainerAvatarInitialPosition;

            _enemyUnit.SetupPokymon();

            yield return _dialogBox.SetDialogText($"{_trainer.Name} sends {_enemyUnit.Pokymon.Name}!");
            yield return _playerAvatar.transform.DOLocalMoveX(playerAvatarInitialPosition.x - 400, 1).WaitForCompletion();

            _playerAvatar.gameObject.SetActive(false);
            _playerAvatar.transform.localPosition = playerAvatarInitialPosition;
        }

        SetupPlayerPokymon(_playerParty.FirstAvailablePokymon);

        yield return _dialogBox.SetDialogText($"Go {_playerUnit.Pokymon.Name}!");

        PlayerSelectAction();
    }

    private void SetupPlayerPokymon(Pokymon playerPokymon)
    {
        _playerUnit.Pokymon = playerPokymon;
        _playerUnit.SetupPokymon();

        _dialogBox.UpdateMoveData(_playerUnit.Pokymon.MoveList);
    }

    private IEnumerator SetupNextTrainerPokymon()
    {
        _battleState = BattleState.Busy;

        _enemyUnit.Pokymon = _trainerParty.FirstAvailablePokymon;
        _enemyUnit.SetupPokymon();

        yield return _dialogBox.SetDialogText($"{_trainer.Name} sends {_enemyUnit.Pokymon.Name}!");

        PlayerSelectAction();
    }

    private void CheckForBattleFinish(BattleUnit knockedOutUnit)
    {
        if (knockedOutUnit.IsPlayer)
        {
            if (_playerParty.HasAnyPokymonAvailable)
            {
                // TODO: fix this, since it prevents after turn effects from being applied
                StopAllCoroutines();

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
            if (IsWildPokymonBattle || !_trainerParty.HasAnyPokymonAvailable)
            {
                FinishBattle(true);
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(SetupNextTrainerPokymon());
            }
        }
    }

    private void FinishBattle(bool hasPlayerWon)
    {
        StopAllCoroutines();

        _battleState = BattleState.FinishBattle;

        _playerParty.PokymonList.ForEach(p => p.OnBattleFinish());

        OnBattleFinish?.Invoke(hasPlayerWon);
    }

    private void PlayerSelectAction()
    {
        _battleState = BattleState.PlayerSelectAction;

        _dialogBox.ToggleMoveSelector(false);
        _dialogBox.SetDialogText($"What will {_playerUnit.Pokymon.Name} do?");
        _dialogBox.ToggleActionSelector(true);
    }

    private void PlayerSelectMove()
    {
        _battleState = BattleState.PlayerSelectMove;

        _dialogBox.ToggleDialogText(false);
        _dialogBox.ToggleActionSelector(false);
        _dialogBox.ToggleMoveSelector(true);
    }

    private void PlayerSelectParty()
    {
        _battleState = BattleState.PlayerSelectParty;

        _partySelection.gameObject.SetActive(true);
        _partySelection.UpdatePartyData(_playerParty);
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
            StartCoroutine(SkipPlayerTurn("You can't catch a trained Pokymon!"));
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
            StartCoroutine(SkipPlayerTurn("You can't run away from a duel!"));
        }
    }

    private IEnumerator SkipPlayerTurn(string reason = null)
    {
        _playerUnit.CanMove = false;

        if (reason != null)
        {
            yield return _dialogBox.SetDialogText(reason);
        }

        yield return TriggerMoves();
    }

    private IEnumerator TriggerMoves()
    {
        _battleState = BattleState.PerformMove;

        _dialogBox.ToggleMoveSelector(false);

        _enemyUnit.NextMove = _enemyUnit.Pokymon.GetRandomAvailableMove();

        var playerMovesFirst = _playerUnit.CanMove &&
            ((_playerUnit.NextMove.Base.Priority == _enemyUnit.NextMove.Base.Priority && _playerUnit.Pokymon.Speed >= _enemyUnit.Pokymon.Speed)
                || _playerUnit.NextMove.Base.Priority > _enemyUnit.NextMove.Base.Priority);

        if (_playerUnit.CanMove && !playerMovesFirst)
        {
            yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} is faster!");
        }

        for (var i = 0; i < 2; i++)
        {
            var sourceUnit = (i == 0 && playerMovesFirst) || (i == 1 && !playerMovesFirst) ? _playerUnit : _enemyUnit;
            var targetUnit = sourceUnit == _playerUnit ? _enemyUnit : _playerUnit;

            yield return InvokeStartMoveEffects(sourceUnit);

            if (sourceUnit.CanMove)
            {
                yield return PerformMove(sourceUnit.NextMove, sourceUnit, targetUnit);
            }

            yield return InvokeFinishTurnEffects(sourceUnit);

            sourceUnit.CanMove = true;
        }

        PlayerSelectAction();
    }

    private IEnumerator InvokeStartMoveEffects(BattleUnit turnUnit)
    {
        if (turnUnit.Pokymon.HasStartMoveStatusConditions)
        {
            yield return InvokeStartMoveStatusConditionEffects(turnUnit);
        }
    }

    private IEnumerator InvokeStartMoveStatusConditionEffects(BattleUnit turnUnit)
    {
        foreach (var statusCondition in turnUnit.Pokymon.StartMoveStatusConditionList)
        {
            AudioManager.SharedInstance.PlaySFX(_statusConditionSFX);

            turnUnit.PlayReceiveStatusConditionEffectAnimation(statusCondition.ID);

            var (skipMove, message) = statusCondition.OnStartMove(turnUnit.Pokymon);

            if (message != null) {
                yield return _dialogBox.SetDialogText(message);
            }

            yield return CheckKnockOut(turnUnit);

            if (skipMove)
            {
                turnUnit.CanMove = false;
            }
        }
    }

    private IEnumerator PerformMove(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        _battleState = BattleState.PerformMove;

        move.PP--;

        yield return _dialogBox.SetDialogText($"{sourceUnit.Pokymon.Name} used {move.Base.Name}.");

        if (move.IsPhysicalMove || move.IsSpecialMove)
        {
            yield return PerformPhysicalMove(move, sourceUnit, targetUnit);
        }
        else if (move.IsStatusMove)
        {
            yield return PerformStatusMove(move, sourceUnit, targetUnit);
        }
    }

    private IEnumerator InvokeFinishTurnEffects(BattleUnit turnUnit)
    {
        if (turnUnit.Pokymon.HasFinishTurnStatusConditions)
        {
            yield return InvokeFinishTurnStatusConditionEffects(turnUnit);
        }
    }

    private IEnumerator InvokeFinishTurnStatusConditionEffects(BattleUnit turnUnit)
    {
        foreach (var statusCondition in turnUnit.Pokymon.FinishTurnStatusConditionList)
        {
            AudioManager.SharedInstance.PlaySFX(_statusConditionSFX);

            turnUnit.PlayReceiveStatusConditionEffectAnimation(statusCondition.ID);

            var message = statusCondition.OnFinishTurn(turnUnit.Pokymon);

            if (message != null)
            {
                yield return _dialogBox.SetDialogText(message);
            }

            yield return CheckKnockOut(turnUnit);
        }
    }

    private IEnumerator PerformPhysicalMove(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        var damageDesc = targetUnit.Pokymon.ReceivePhysicalMove(move, sourceUnit.Pokymon);

        sourceUnit.PlayPhysicalMoveAnimation();

        if (damageDesc.IsEvaded)
        {
            AudioManager.SharedInstance.PlaySFX(_evadeMoveSFX);

            targetUnit.PlayEvadeMoveAnimation();

            yield return _dialogBox.SetDialogText($"{targetUnit.Pokymon.Name} evaded the attack!");
        }
        else
        {
            AudioManager.SharedInstance.PlaySFX(_physicalMoveSFX);

            yield return targetUnit.PlayReceivePhysicalMoveAnimation(move.Base.Type);
            yield return _dialogBox.SetDialogText($"{targetUnit.Pokymon.Name} took {damageDesc.HP} HP damage.");

            yield return ShowDamageDescription(damageDesc);
            yield return CheckKnockOut(targetUnit);

            yield return ApplyStatMofidierEffects(move, sourceUnit, targetUnit);
            yield return ApplyStatusConditionEffects(move, sourceUnit, targetUnit);
        }
    }

    private IEnumerator PerformStatusMove(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        if (targetUnit.Pokymon.CanEvadeMove(move, sourceUnit.Pokymon))
        {
            AudioManager.SharedInstance.PlaySFX(_evadeMoveSFX);

            sourceUnit.PlayStatusMoveAnimation();
            targetUnit.PlayEvadeMoveAnimation();

            yield return _dialogBox.SetDialogText($"{targetUnit.Pokymon.Name} evaded the attack!");
        }
        else
        {
            yield return ApplyStatMofidierEffects(move, sourceUnit, targetUnit);
            yield return ApplyStatusConditionEffects(move, sourceUnit, targetUnit);
        }
    }

    private IEnumerator ApplyStatMofidierEffects(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        foreach (var effect in move.Base.StatModifierEffectList)
        {
            BattleUnit effectTarget = null;

            sourceUnit.PlayStatusMoveAnimation();

            AudioManager.SharedInstance.PlaySFX(effect.Modifier > 0
                ? _increaseStatSFX
                : _decreaseStatSFX);

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

            effectTarget.PlayReceiveStatModifierEffectAnimation(move.Base.Type);

            yield return _dialogBox.SetDialogText($"{effectTarget.Pokymon.Name} {effect.Stat.ToString()} was {incOrDec}.");
        }
    }

    private IEnumerator ApplyStatusConditionEffects(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        foreach (var effect in move.Base.StatusConditionEffectList)
        {
            if (StatusConditionFactory.StatusConditionList.ContainsKey(effect.ConditionID))
            {
                if (Random.Range(0, 100) < effect.Probability)
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

                    var statusCondition = targetUnit.Pokymon.ApplyStatusCondition(effect.ConditionID);

                    if (statusCondition != null)
                    {
                        AudioManager.SharedInstance.PlaySFX(_statusConditionSFX);

                        targetUnit.PlayReceiveStatusConditionEffectAnimation(statusCondition.ID);

                        yield return _dialogBox.SetDialogText(statusCondition.OnApplyMessage.Replace("%pokymon.name%", effectTarget.Pokymon.Name));
                    }
                }
            }
            else
            {
                Debug.Log($"Status condition not implemented: {effect.ConditionID}");
            }
        }
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

    private IEnumerator CheckKnockOut(BattleUnit targetUnit)
    {
        if (targetUnit.Pokymon.IsKnockedOut)
        {
            yield return PerformKnockOut(targetUnit);
        }
    }

    private IEnumerator PerformKnockOut(BattleUnit targetUnit)
    {
        AudioManager.SharedInstance.PlaySFX(_knockOutSFX);

        targetUnit.PlayKnockedOutAnimation();

        yield return _dialogBox.SetDialogText($"{targetUnit.Pokymon.Name} got knocked out!");

        if (targetUnit.IsPlayer)
        {
            yield return new WaitForSecondsRealtime(1f);
        }

        if (!targetUnit.IsPlayer)
        {
            yield return PerformEarnExp();
        }

        CheckForBattleFinish(targetUnit);
    }

    private IEnumerator PerformEarnExp()
    {
        var earnedExp = _enemyUnit.Pokymon.KnockOutExp;

        _playerUnit.Pokymon.EarnExp(earnedExp);

        yield return _dialogBox.SetDialogText($"{_playerUnit.Pokymon.Name} earned {earnedExp} EXP.");

        while (_playerUnit.Pokymon.LevelUp())
        {
            AudioManager.SharedInstance.PlaySFX(_levelUpSFX);

            yield return _dialogBox.SetDialogText($"{_playerUnit.Pokymon.Name} leveled up!");

            _learnableMove = _playerUnit.Pokymon.LearnableMove;

            if (_learnableMove != null)
            {
                if (_playerUnit.Pokymon.HasFreeMoveSlot)
                {
                    _playerUnit.Pokymon.LearnMove(_learnableMove);
                    _dialogBox.UpdateMoveData(_playerUnit.Pokymon.MoveList);

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

            _dialogBox.UpdateMoveData(_playerUnit.Pokymon.MoveList);
        }

        _learnableMove = null;
        _battleState = BattleState.Busy;
    }

    private IEnumerator PerformPokymonSwitch(Pokymon nextPlayerPokymon)
    {
        _battleState = BattleState.Busy;

        _dialogBox.ToggleActionSelector(false);

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
            PlayerSelectAction();
        }
        else
        {
            yield return SkipPlayerTurn();
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

            pokyballSprite.DOFade(0, 1f);

            yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} caught!");

            if (_playerParty.AddPokymon(_enemyUnit.Pokymon))
            {
                yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} added to party.");
            }
            else
            {
                yield return _dialogBox.SetDialogText($"{_enemyUnit.Pokymon.Name} sent to Bill's PC.");
            }

            Destroy(pokyballInstance);

            yield return PerformEarnExp();

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

            yield return InvokeFinishTurnEffects(_playerUnit);

            PlayerSelectAction();
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
                yield return SkipPlayerTurn("You couldn't run away!");

                yield return InvokeFinishTurnEffects(_playerUnit);
            }
        }
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
    Trainer,
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
    PerformMove,
    Busy,
}
