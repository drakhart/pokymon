using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask _pokymonAreaLayers;
    [SerializeField] private LayerMask _solidObjectsLayers;

    [SerializeField] private float _speed;

    [SerializeField] private AudioClip _stepsSFX;

    private Animator _animator;
    private Vector2 _input;
    private bool _isMoving;

    public event Action OnPokymonEncountered;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    public void HandleUpdate() {
        if (!_isMoving)
        {
            _input.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            _input.y = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

            if (_input.x != 0)
            {
                _input.y = 0;
            }

            if (_input != Vector2.zero)
            {
                _animator.SetFloat("Move X", _input.x);
                _animator.SetFloat("Move Y", _input.y);

                var target = transform.position;
                target.x += _input.x;
                target.y += _input.y;

                if (IsTargetWalkable(target))
                {
                    StartCoroutine(MoveTowards(target));
                }
            }
        }
    }

    private void LateUpdate() {
        _animator.SetBool("Is Moving", _isMoving);
    }

    private bool IsPokymonEncountered()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.25f, _pokymonAreaLayers) != null)
        {
            if (Random.Range(0, 100) < Constants.POKYMON_ENCOUNTER_ODDS)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTargetWalkable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.25f, _solidObjectsLayers) != null)
        {
            return false;
        }

        return true;
    }

    private IEnumerator MoveTowards(Vector3 destination)
    {
        _isMoving = true;

        AudioManager.SharedInstance.PlaySFX(_stepsSFX);

        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, _speed * Time.deltaTime);

            yield return null;
        }

        transform.position = destination;
        _isMoving = false;

        if (IsPokymonEncountered())
        {
            OnPokymonEncountered();
        }
    }
}
