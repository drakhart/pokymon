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
        _animator.MoveX = moveVector.x;
        _animator.MoveY = moveVector.y;

        var targetPosition = transform.position;
        targetPosition.x += moveVector.x;
        targetPosition.y += moveVector.y;

        if (!IsTargetWalkable(targetPosition))
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

    private bool IsTargetWalkable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.25f, LayerManager.SharedInstance.SolidObjectsLayers | LayerManager.SharedInstance.InteractableLayers) != null)
        {
            return false;
        }

        return true;
    }
}
