using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CharacterAnimator))]
public class Character : MonoBehaviour
{
    [SerializeField] private float _speed;

    private CharacterAnimator _animator;
    public CharacterAnimator Animator => _animator;

    public bool IsMoving { get; private set; }

    private void Awake() {
        _animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator MoveTowards(Vector2 moveVector, Action onMoveStart = null, Action onMoveFinish = null)
    {
        if (moveVector.x != 0)
        {
            moveVector.y = 0;
        }

        _animator.MoveX = Mathf.Clamp(moveVector.x, -1, 1);
        _animator.MoveY = Mathf.Clamp(moveVector.y, -1, 1);

        var targetPosition = transform.position;
        targetPosition.x += moveVector.x;
        targetPosition.y += moveVector.y;

        if (!IsPathWalkable(targetPosition))
        {
            yield break;
        }

        yield return transform.DOMove(targetPosition, Math.Max(Math.Abs(moveVector.x), Math.Abs(moveVector.y)) / _speed)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                IsMoving = true;
                onMoveStart?.Invoke();
            })
            .OnComplete(() =>
            {
                IsMoving = false;
                onMoveFinish?.Invoke();
            })
            .WaitForCompletion();
    }

    public void HandleUpdate()
    {
        _animator.IsMoving = IsMoving;
    }

    private bool IsPathWalkable(Vector3 target)
    {
        var path = target - transform.position;
        var direction = path.normalized;

        return !Physics2D.BoxCast(transform.position + direction, new Vector2(0.25f, 0.25f), 0, direction, path.magnitude - 1,
            LayerManager.SharedInstance.CollisionLayers);
    }
}
