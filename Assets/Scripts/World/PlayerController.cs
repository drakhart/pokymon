using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private bool isMoving;

    public float speed;

    private Vector2 input;

    private Animator _animator;

    public LayerMask solidObjectsLayer, pokymonLayer;

    public event Action OnPokymonEncountered;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    public void HandleUpdate() {
        if (!isMoving)
        {
            input.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            input.y = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

            if (input.x != 0)
            {
                input.y = 0;
            }

            if (input != Vector2.zero)
            {
                _animator.SetFloat("Move X", input.x);
                _animator.SetFloat("Move Y", input.y);

                var target = transform.position;
                target.x += input.x;
                target.y += input.y;

                if (IsTargetWalkable(target))
                {
                    StartCoroutine(MoveTowards(target));
                }
            }
        }
    }

    private void LateUpdate() {
        _animator.SetBool("Is Moving", isMoving);
    }

    private bool IsPokymonEncountered()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.25f, pokymonLayer) != null)
        {
            if (Random.Range(0, 100) < 10)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTargetWalkable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.25f, solidObjectsLayer) != null)
        {
            return false;
        }

        return true;
    }

    IEnumerator MoveTowards(Vector3 destination)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            yield return null;
        }

        transform.position = destination;
        isMoving = false;

        if (IsPokymonEncountered())
        {
            OnPokymonEncountered();
        }
    }
}
