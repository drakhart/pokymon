using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> _idleUpFrameList, _idleDownFrameList, _idleRightFrameList, _idleLeftFrameList;
    [SerializeField] private List<Sprite> _moveUpFrameList, _moveDownFrameList, _moveRightFrameList, _moveLeftFrameList;
    [SerializeField] private FacingDirection _defaultFacingDirection = FacingDirection.Down;

    private CustomAnimator _idleUpAnimator, _idleDownAnimator, _idleRightAnimator, _idleLeftAnimator;
    private CustomAnimator _moveUpAnimator, _moveDownAnimator, _moveRightAnimator, _moveLeftAnimator;

    private SpriteRenderer _renderer;
    private CustomAnimator _currentAnimator;

    private FacingDirection _currentFacingDirection;
    public FacingDirection CurrentFacingDirection => _currentFacingDirection;

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

        SetCurrentFacingDirection(_defaultFacingDirection);
    }

    private void Update() {
        var previousAnimator = _currentAnimator;

        if (MoveY > 0)
        {
            SetCurrentFacingDirection(FacingDirection.Up);
        }
        else if (MoveY < 0)
        {
            SetCurrentFacingDirection(FacingDirection.Down);
        }
        else if (MoveX > 0)
        {
            SetCurrentFacingDirection(FacingDirection.Right);
        }
        else if (MoveX < 0)
        {
            SetCurrentFacingDirection(FacingDirection.Left);
        }

        if (_currentAnimator != previousAnimator)
        {
            _currentAnimator.Start();
        } else {
            _currentAnimator.Update();
        }
    }

    private void SetCurrentFacingDirection(FacingDirection facingDirection)
    {
        _currentFacingDirection = facingDirection;

        switch (_currentFacingDirection)
        {
            case FacingDirection.Up:
                _currentAnimator = IsMoving ? _moveUpAnimator : _idleUpAnimator;
                break;

            case FacingDirection.Down:
                _currentAnimator = IsMoving ? _moveDownAnimator : _idleDownAnimator;
                break;

            case FacingDirection.Right:
                _currentAnimator = IsMoving ? _moveRightAnimator : _idleRightAnimator;
                break;

            case FacingDirection.Left:
                _currentAnimator = IsMoving ? _moveLeftAnimator : _idleLeftAnimator;
                break;
        }
    }
}

public enum FacingDirection
{
    Down,
    Left,
    Right,
    Up,
}
