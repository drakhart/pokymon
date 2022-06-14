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

    private void Awake() {
        _character = GetComponent<Character>();
    }

    private void Update() {
        if (CanPatrol)
        {
            Patrol();
        }

        _character.HandleUpdate();
    }

    private void Idle()
    {
        _state = NPCState.Idle;
        _idleTime = 0;
    }

    public void Interact()
    {
        if (_state != NPCState.Idle)
        {
            return;
        }

        _state = NPCState.Interacting;

        DialogManager.SharedInstance.StartDialog(_dialog, () => Idle());
    }

    private void Patrol()
    {
        _idleTime += Time.deltaTime;

        if (_idleTime > _patrolDelay)
        {
            StartCoroutine(_character.MoveTowards(_patrolMovements[_currentPatrolMovement],
                () => _state = NPCState.Moving,
                () =>
                {
                    _currentPatrolMovement = (_currentPatrolMovement + 1) % _patrolMovements.Count;

                    Idle();
                }
            ));
        }
    }
}

public enum NPCState
{
    Idle,
    Interacting,
    Moving,
}
