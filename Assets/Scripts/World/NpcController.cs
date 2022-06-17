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
    private NpcState _state;

    public bool CanPatrol => _state == NpcState.Idle && _patrolMovements.Count > 0;

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
        _state = NpcState.Idle;
        _idleTime = 0;
    }

    public void Interact(Vector3 source)
    {
        if (_state != NpcState.Idle)
        {
            return;
        }

        _state = NpcState.Interacting;

        _character.LookTowards(source);

        DialogManager.SharedInstance.StartDialog(_dialog, () => Idle());
    }

    private void Patrol()
    {
        _idleTime += Time.deltaTime;

        if (_idleTime > _patrolDelay)
        {
            StartCoroutine(_character.MoveTowards(_patrolMovements[_currentPatrolMovement],
                () => _state = NpcState.Moving,
                () =>
                {
                    _currentPatrolMovement = (_currentPatrolMovement + 1) % _patrolMovements.Count;

                    Idle();
                }
            ));
        }
    }
}

public enum NpcState
{
    Idle,
    Interacting,
    Moving,
}
