using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Character))]
public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog _dialog;
    [SerializeField] private float _patrolDelay;
    [SerializeField] private List<Vector2> _patrolMovements;

    private Character _character;
    private int _currentPatrolMovement;
    private float _idleTime;
    private NPCState _state;

    public bool CanPatrol => _state == NPCState.Idle && _patrolMovements.Count > 0;

    public bool HasFinishedMoving => _state == NPCState.Moving && !_character.IsMoving;

    private void Awake() {
        _character = GetComponent<Character>();
    }

    private void Update() {
        if (CanPatrol)
        {
            Patrol();
        }
        else if (HasFinishedMoving)
        {
            Idle();
        }

        _character.HandleUpdate();
    }

    public void Interact()
    {
        if (_state != NPCState.Idle)
        {
            return;
        }

        _state = NPCState.Talking;

        DialogManager.SharedInstance.StartDialog(_dialog, () => Idle());
    }

    private void Idle()
    {
        _state = NPCState.Idle;
        _idleTime = 0;
    }

    private void Patrol()
    {
        _idleTime += Time.deltaTime;

        if (_idleTime > _patrolDelay)
        {
            _state = NPCState.Moving;

            StartCoroutine(_character.MoveTowards(_patrolMovements[_currentPatrolMovement]));

            _currentPatrolMovement = (_currentPatrolMovement + 1) % _patrolMovements.Count;
            _idleTime = 0;
        }
    }
}

public enum NPCState
{
    Idle,
    Moving,
    Talking,
}
