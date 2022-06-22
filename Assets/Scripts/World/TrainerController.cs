using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(PokymonParty))]
public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog _dialog;
    [SerializeField] private GameObject _exclamationMark;
    [SerializeField] private GameObject _fov;

    [SerializeField] private Sprite _avatar;
    public Sprite Avatar => _avatar;

    [SerializeField] private string _name;
    public string Name => _name;

    private Character _character;

    private void Awake() {
        _character = GetComponent<Character>();
    }

    private void Update() {
        _character.HandleUpdate();

        SetFovDirection(_character.Animator.CurrentFacingDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player, Action startBattle)
    {
        ToggleExclamationMark(true);

        yield return new WaitForSecondsRealtime(0.6f);

        ToggleExclamationMark(false);

        var distanceToPlayer = player.transform.position - transform.position;
        var moveTarget = distanceToPlayer - distanceToPlayer.normalized;
        moveTarget = new Vector3(Mathf.RoundToInt(moveTarget.x), Mathf.RoundToInt(moveTarget.y));

        yield return _character.MoveTowards(moveTarget);

        DialogManager.SharedInstance.StartDialog(_dialog, () =>
        {
            startBattle();
        });
    }

    private void ToggleExclamationMark(bool active)
    {
        _exclamationMark.SetActive(active);
    }

    public void SetFovDirection(FacingDirection facingDirection)
    {
        float angle = 0f;

        if (facingDirection == FacingDirection.Right)
        {
            angle = 90f;
        }
        else if (facingDirection == FacingDirection.Up)
        {
            angle = 180f;
        }
        else if (facingDirection == FacingDirection.Left)
        {
            angle = 270f;
        }

        _fov.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void Interact(Vector3 source)
    {
        _character.LookTowards(source);

        GameManager.SharedInstance.StartTrainerBattle(this);
    }
}
