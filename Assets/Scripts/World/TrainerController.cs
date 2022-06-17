using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class TrainerController : MonoBehaviour
{
    [SerializeField] private GameObject _exclamationMark;
    [SerializeField] private Dialog _dialog;

    private Character _character;

    private void Awake() {
        _character = GetComponent<Character>();
    }

    private void Update() {
        _character.HandleUpdate();
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        ToggleExclamationMark(true);

        yield return new WaitForSecondsRealtime(0.6f);

        ToggleExclamationMark(false);

        var distanceToPlayer = player.transform.position - transform.position;
        var moveTarget = distanceToPlayer - distanceToPlayer.normalized;
        moveTarget = new Vector3(Mathf.RoundToInt(moveTarget.x), Mathf.RoundToInt(moveTarget.y));

        Debug.Log(moveTarget);

        yield return _character.MoveTowards(moveTarget);

        DialogManager.SharedInstance.StartDialog(_dialog, () =>
        {
            // TODO: Start trainer battle
        });
    }

    private void ToggleExclamationMark(bool active)
    {
        _exclamationMark.SetActive(active);
    }
}
