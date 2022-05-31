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
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogBox dialogBox;
    [SerializeField] private PartySelection partySelection;
    [SerializeField] private GameObject pokyball;
    [SerializeField] private float timeBetweenInputs = 0.25f;

    public BattleState State;

    private BattleType type;
    private PokymonParty playerParty;
    private Pokymon enemyPokymon;
    private int currSelectedAction;
    private int currSelectedMove;
    private int currSelectedPokymon;
    private float timeSinceLastInput;

    public event Action<bool> OnBattleFinish;

    public void HandleStart(BattleType type, PokymonParty playerParty, Pokymon enemyPokymon) {
        this.type = type;
        this.playerParty = playerParty;
        this.enemyPokymon = enemyPokymon;

        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate() {
        switch (State)
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
        if (Time.time < timeSinceLastInput + timeBetweenInputs)
        {
            return;
        }

        if (Input.anyKey || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            timeSinceLastInput = Time.time;
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                currSelectedAction = (currSelectedAction + 1) % 2 + (currSelectedAction >= 2 ? 2 : 0);
            }
            else
            {
                currSelectedAction = (currSelectedAction + 2) % 4;
            }

            dialogBox.SelectAction(currSelectedAction);
        }

        if (Input.GetButtonDown("Submit"))
        {
            switch (currSelectedAction)
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
                    OnBattleFinish(false);
                    break;

                default:
                    break;
            }
        }
    }

    private void HandlePlayerMoveSelection()
    {
        if (Time.time < timeSinceLastInput + timeBetweenInputs)
        {
            return;
        }

        if (Input.anyKey || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            timeSinceLastInput = Time.time;
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                currSelectedMove = (currSelectedMove + 1) % 2 + (currSelectedMove >= 2 ? 2 : 0);
            }
            else
            {
                currSelectedMove = (currSelectedMove + 2) % 4;
            }

            currSelectedMove = Mathf.Clamp(currSelectedMove, 0, playerUnit.Pokymon.Moves.Count - 1);

            dialogBox.SelectMove(currSelectedMove);
            dialogBox.SetMoveDetails(playerUnit.Pokymon.Moves[currSelectedMove]);
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
        if (Time.time < timeSinceLastInput + timeBetweenInputs)
        {
            return;
        }

        if (Input.anyKey || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            timeSinceLastInput = Time.time;
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                currSelectedPokymon = (currSelectedPokymon + 1) % 2 + 2 * (int)(currSelectedPokymon / 2);
            }
            else
            {
                currSelectedPokymon = (currSelectedPokymon + (Input.GetAxis("Vertical") < 0 ? 2 : playerParty.PokymonCount - 2 + playerParty.PokymonCount % 2))
                    % (playerParty.PokymonCount % 2 == 0 ? playerParty.PokymonCount : playerParty.PokymonCount + 1);
            }

            currSelectedPokymon = Mathf.Clamp(currSelectedPokymon, 0, playerParty.PokymonCount - 1);

            partySelection.SelectPokymon(currSelectedPokymon);
        }

        if (Input.GetButtonDown("Submit"))
        {
            var nextPlayerPokymon = playerParty.PokymonList[currSelectedPokymon];

            if (nextPlayerPokymon.IsKnockedOut)
            {
                partySelection.SetDialogText("You can't choose a knocked out Pokymon.");

                return;
            }
            else if (nextPlayerPokymon == playerUnit.Pokymon)
            {
                partySelection.SetDialogText("You can't choose the Pokymon currently in battle.");

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
        State = BattleState.StartBattle;

        partySelection.SetupPartySelection();

        SetupPlayerPokymon(playerParty.FirstAvailablePokymon);

        enemyUnit.Pokymon = enemyPokymon;
        enemyUnit.SetupPokymon(enemyUnit.Pokymon);

        if (type == BattleType.WildPokymon)
        {
            enemyUnit.IsWild = true;

            yield return dialogBox.SetDialogText($"{GetUnitName(enemyUnit)} appears!");
        }

        if (playerUnit.Pokymon.Speed < enemyUnit.Pokymon.Speed)
        {
            yield return dialogBox.SetDialogText($"{GetUnitName(enemyUnit)} is fast!");

            EnemyMove();
        }
        else
        {
            PlayerSelectAction();
        }
    }

    private void SetupPlayerPokymon(Pokymon playerPokymon)
    {
        playerUnit.Pokymon = playerPokymon;

        playerUnit.SetupPokymon(playerUnit.Pokymon);
        dialogBox.SetMoveTexts(playerUnit.Pokymon.Moves);
    }

    private void FinishBattle(bool hasPlayerWon)
    {
        State = BattleState.FinishBattle;

        OnBattleFinish(hasPlayerWon);
    }

    private string GetUnitName(BattleUnit unit)
    {
        var name = unit.Pokymon.Base.Name;

        if (unit.IsWild)
        {
            return $"Wild {name}";
        }

        return name;
    }

    private void PlayerSelectAction()
    {
        State = BattleState.PlayerSelectAction;
        currSelectedAction = 0;

        partySelection.gameObject.SetActive(false);
        dialogBox.ToggleMoveSelector(false);
        dialogBox.SetDialogText($"What will {GetUnitName(playerUnit)} do?");
        dialogBox.ToggleDialogText(true);
        dialogBox.SelectAction(currSelectedAction);
        dialogBox.ToggleActionSelector(true);
    }

    private void PlayerSelectMove()
    {
        State = BattleState.PlayerSelectMove;
        currSelectedMove = 0;

        dialogBox.ToggleDialogText(false);
        dialogBox.ToggleActionSelector(false);
        dialogBox.SelectMove(currSelectedMove);
        dialogBox.SetMoveDetails(playerUnit.Pokymon.Moves[currSelectedMove]);
        dialogBox.ToggleMoveSelector(true);
    }

    private void PlayerSelectParty()
    {
        State = BattleState.PlayerSelectParty;
        currSelectedPokymon = 0;

        partySelection.gameObject.SetActive(true);
        partySelection.UpdatePartyData(playerParty.PokymonList);
        partySelection.SelectPokymon(currSelectedPokymon);
    }

    private void PlayerSelectInventory()
    {
        // TODO: implement actual inventory and items logic
        dialogBox.ToggleActionSelector(false);

        if (enemyUnit.IsWild)
        {
            StartCoroutine(ThrowPokyball());
        }
    }

    private void EnemyMove()
    {
        dialogBox.ToggleDialogText(true);

        var move = enemyUnit.Pokymon.GetRandomAvailableMove();

        StartCoroutine(PerformEnemyMove(move));
    }

    private void PlayerMove()
    {
        var move = playerUnit.Pokymon.Moves[currSelectedMove];

        if (!move.HasAvailablePP)
        {
            return;
        }

        dialogBox.ToggleMoveSelector(false);
        dialogBox.ToggleDialogText(true);

        StartCoroutine(PerformPlayerMove(move));
    }

    IEnumerator PerformEnemyMove(Move move)
    {
        State = BattleState.PerformEnemyMove;

        yield return PerformMove(enemyUnit, playerUnit, move);

        if (State == BattleState.PerformEnemyMove)
        {
            PlayerSelectAction();
        }
    }

    IEnumerator PerformPlayerMove(Move move)
    {
        State = BattleState.PerformPlayerMove;

        yield return PerformMove(playerUnit, enemyUnit, move);

        if (State == BattleState.PerformPlayerMove)
        {
            EnemyMove();
        }
    }

    IEnumerator PerformMove(BattleUnit source, BattleUnit target, Move move)
    {
        move.PP--;

        yield return dialogBox.SetDialogText($"{GetUnitName(source)} used {move.Base.Name}.");

        source.PlayPhysicalMoveAnimation();
        target.PlayReceiveDamageAnimation();

        var damageDescription = target.Pokymon.ReceiveDamage(source.Pokymon, move);

        target.HUD.UpdatePokymonData(damageDescription.Damage);
        yield return ShowDamageDescription(damageDescription);

        if (damageDescription.IsKnockedOut)
        {
            yield return dialogBox.SetDialogText($"{GetUnitName(target)} got knocked out!");

            target.PlayKnockedOutAnimation();

            yield return new WaitForSecondsRealtime(2);

            CheckForBattleFinish(target);
        }
    }

    IEnumerator PerformPokymonSwitch(Pokymon nextPlayerPokymon)
    {
        State = BattleState.Busy;

        partySelection.gameObject.SetActive(false);
        dialogBox.ToggleActionSelector(false);
        dialogBox.ToggleDialogText(true);

        if (!playerUnit.Pokymon.IsKnockedOut)
        {
            playerUnit.PlaySwitchAnimation();

            yield return dialogBox.SetDialogText($"Come back, {GetUnitName(playerUnit)}!");
        }

        SetupPlayerPokymon(nextPlayerPokymon);

        yield return dialogBox.SetDialogText($"I choose you, {GetUnitName(playerUnit)}!");

        EnemyMove();
    }

    void CheckForBattleFinish(BattleUnit knockedOutUnit)
    {
        if (knockedOutUnit.IsPlayer)
        {
            if (playerParty.HasAnyPokymonAvailable)
            {
                PlayerSelectParty();
            }
            else
            {
                FinishBattle(false);
            }
        } else {
            FinishBattle(true);
        }
    }

    IEnumerator ShowDamageDescription(DamageDescription desc)
    {
        if (desc.Critical > 1f)
        {
            yield return dialogBox.SetDialogText("Critical hit!");
        }

        if (desc.Type > 1f)
        {
            yield return dialogBox.SetDialogText("It's super effective!");
        }
        else if (desc.Type < 1f)
        {
            yield return dialogBox.SetDialogText("It's not very effective...");
        }
    }

    IEnumerator ThrowPokyball()
    {
        State = BattleState.Busy;

        yield return dialogBox.SetDialogText($"You threw a {pokyball.name}...");

        var pokyballInstance = Instantiate(pokyball, playerUnit.transform.position - new Vector3(3, 0), Quaternion.identity);

        var pokyballSprite = pokyballInstance.GetComponent<SpriteRenderer>();
        yield return pokyballSprite.transform
            .DOLocalJump(enemyUnit.transform.position + new Vector3(0, 1f), 2.25f, 1, 0.75f)
            .WaitForCompletion();

        yield return enemyUnit.PlayCaptureAnimation();

        yield return pokyballSprite.transform
            .DOLocalJump(enemyUnit.transform.position - new Vector3(0, 2.15f), 0.5f, 2, 0.75f)
            .WaitForCompletion();

        var captureDescription = enemyUnit.Pokymon.ReceiveCaptureAttempt();

        for (var i = 0; i < Mathf.Max(captureDescription.ShakeCount, 1); i++)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            yield return pokyballSprite.transform.DOPunchRotation(new Vector3(0, 0, 15), 0.6f).WaitForCompletion();
        }

        yield return new WaitForSecondsRealtime(0.5f);

        if (captureDescription.IsCaptured)
        {
            yield return dialogBox.SetDialogText($"{GetUnitName(enemyUnit)} caught!");
            yield return pokyballSprite.DOFade(0, 1f).WaitForCompletion();

            if (playerParty.AddPokymon(enemyUnit.Pokymon))
            {
                yield return dialogBox.SetDialogText($"{GetUnitName(enemyUnit)} added to party.");
            }
            else
            {
                yield return dialogBox.SetDialogText($"{GetUnitName(enemyUnit)} sent to Bill's PC.");
            }

            Destroy(pokyballInstance);
            FinishBattle(true);
        }
        else
        {
            yield return pokyballSprite.DOFade(0, 1f);
            yield return enemyUnit.PlayEscapeAnimation();
            yield return dialogBox.SetDialogText($"{GetUnitName(enemyUnit)} escaped!");

            Destroy(pokyballInstance);
            EnemyMove();
        }
    }
}
