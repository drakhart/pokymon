using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> _idleUpFrameList, _idleDownFrameList, _idleRightFrameList, _idleLeftFrameList;
    [SerializeField] private List<Sprite> _moveUpFrameList, _moveDownFrameList, _moveRightFrameList, _moveLeftFrameList;

    private CustomAnimator _idleUpAnimator, _idleDownAnimator, _idleRightAnimator, _idleLeftAnimator;
    private CustomAnimator _moveUpAnimator, _moveDownAnimator, _moveRightAnimator, _moveLeftAnimator;

    private SpriteRenderer _renderer;
    private CustomAnimator _currentAnimator;

    public float MoveX, MoveY;
    public bool IsMoving;

    private void Start() {
        _renderer = GetComponent<SpriteRenderer>();
        _idleUpAnimator = new CustomAnimator(_renderer, _idleUpFrameList);
        _idleDownAnimator = new CustomAnimator(_renderer, _idleDownFrameList);
        _idleRightAnimator = new CustomAnimator(_renderer, _idleRightFrameList);
        _idleLeftAnimator = new CustomAnimator(_renderer, _idleLeftFrameList);
        _moveUpAnimator = new CustomAnimator(_renderer, _moveUpFrameList);
        _moveDownAnimator = new CustomAnimator(_renderer, _moveDownFrameList);
        _moveRightAnimator = new CustomAnimator(_renderer, _moveRightFrameList);
        _moveLeftAnimator = new CustomAnimator(_renderer, _moveLeftFrameList);

        _currentAnimator = _idleDownAnimator;
    }

    private void Update() {
        var previousAnimator = _currentAnimator;

        if (MoveX > 0)
        {
            _currentAnimator = IsMoving ? _moveRightAnimator : _idleRightAnimator;
        }
        else if (MoveX < 0)
        {
            _currentAnimator = IsMoving ? _moveLeftAnimator : _idleLeftAnimator;
        }
        else if (MoveY > 0)
        {
            _currentAnimator = IsMoving ? _moveUpAnimator : _idleUpAnimator;
        }
        else if (MoveY < 0)
        {
            _currentAnimator = IsMoving ? _moveDownAnimator : _idleDownAnimator;
        }

        if (_currentAnimator != previousAnimator)
        {
            _currentAnimator.Start();
        } else {
            _currentAnimator.Update();
        }
    }
}
