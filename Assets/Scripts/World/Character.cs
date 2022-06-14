using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterAnimator))]
public class Character : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private AudioClip _stepsSFX;

    private CharacterAnimator _animator;
    public CharacterAnimator Animator => _animator;

    public bool IsMoving { get; private set; }

    private void Awake() {
        _animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator MoveTowards(Vector2 moveVector, Action onMoveFinish = null)
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

        IsMoving = true;

        AudioManager.SharedInstance.PlaySFX(_stepsSFX);

        while (Vector3.Distance(transform.position, targetPosition) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);

            yield return null;
        }

        transform.position = targetPosition;

        IsMoving = false;

        onMoveFinish?.Invoke();
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
            LayerManager.SharedInstance.SolidObjectsLayers | LayerManager.SharedInstance.InteractableLayers);
    }
}
