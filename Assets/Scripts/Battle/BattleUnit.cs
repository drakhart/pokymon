using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    [SerializeField] private BattleUnitHUD _hud;
    public BattleUnitHUD HUD => _hud;

    [SerializeField] private bool _isPlayer;
    public bool IsPlayer => _isPlayer;

    private Image _image;
    private Color _originalColor;
    private Vector3 _originalPosition;

    public Pokymon Pokymon;

    private void Awake() {
        _image = GetComponent<Image>();
        _originalColor = _image.color;
        _originalPosition = _image.transform.localPosition;
    }

    public void SetupPokymon()
    {
        _image.sprite = IsPlayer ? Pokymon.Base.BackSprite : Pokymon.Base.FrontSprite;
        _image.color = _originalColor;
        _image.transform.localScale = new Vector3(1f, 1f, 1f);

        _hud.SetPokymonData(Pokymon);

        PlaySetupAnimation();
    }

    public void PlayKnockedOutAnimation()
    {
        var delay = 1.5f;
        var seq = DOTween.Sequence();
        seq.Append(_image.transform.DOLocalMoveY(_originalPosition.y - (IsPlayer ? 1 : 2) * 150, delay / (IsPlayer ? 2 : 1)));
        seq.Join(_image.DOFade(0, delay / (IsPlayer ? 2 : 1)));
    }

    public void PlayPhysicalMoveAnimation()
    {
        var delay = 0.3f;
        var seq = DOTween.Sequence();
        seq.Append(_image.transform.DOLocalMoveX(_originalPosition.x + (IsPlayer ? 1 : -1) * 60, delay));
        seq.Append(_image.transform.DOLocalMoveX(_originalPosition.x, delay));
        seq.Play();
    }

    public YieldInstruction PlayReceivePhysicalMoveAnimation(PokymonType moveType)
    {
        var delay = 0.1f;
        var seq = DOTween.Sequence();

        seq.Append(_image.DOColor(ColorManager.SharedInstance.ByPokymonType(moveType), delay));

        for (var i = 0; i < 2; i++)
        {
            seq.Append(_image.transform.DOLocalMoveX(_originalPosition.x + (IsPlayer ? -1 : 1) * 10, delay));
            seq.Append(_image.transform.DOLocalMoveX(_originalPosition.x, delay));
        }

        seq.Append(_image.DOColor(_originalColor, delay));

        return seq.Play().WaitForCompletion();
    }

    public YieldInstruction PlayReceiveStatModifierEffectAnimation(PokymonType moveType)
    {
        var delay = 0.25f;
        var seq = DOTween.Sequence();

        for (var i = 0; i < 2; i++)
        {
            seq.Append(_image.DOColor(ColorManager.SharedInstance.ByPokymonType(moveType), delay));
            seq.Append(_image.DOColor(_originalColor, delay));
        }

        return seq.Play().WaitForCompletion();
    }

    public void PlayStatusMoveAnimation()
    {
        _image.transform.DOPunchScale(new Vector3(0, 0.1f), 1.2f);
    }

    public YieldInstruction PlayReceiveStatusConditionEffectAnimation(StatusConditionID statusConditionID)
    {
        var delay = 0.5f;
        var seq = DOTween.Sequence();

        seq.Append(_image.DOColor(ColorManager.SharedInstance.ByStatusCondition(statusConditionID), delay));
        seq.Append(_image.DOColor(_originalColor, delay));

        return seq.Play().WaitForCompletion();
    }

    public void PlaySetupAnimation()
    {
        _image.transform.localPosition = new Vector3(_originalPosition.x + (IsPlayer ? -1 : 1) * 400, _originalPosition.y);
        _image.transform.DOLocalMoveX(_originalPosition.x, 1);
    }

    public void PlaySwitchAnimation()
    {
        _image.transform.DOLocalMoveX(_originalPosition.x + (IsPlayer ? -1 : 1) * 400, 1);
    }

    public YieldInstruction PlayCaptureAnimation()
    {
        var delay = 0.75f;
        var seq = DOTween.Sequence();

        seq.Append(_image.DOFade(0, delay));
        seq.Join(_image.transform.DOScale(new Vector3(0.25f, 0.25f, 1f), delay));
        seq.Join(_image.transform.DOLocalMoveY(_originalPosition.y + 50, delay));

        return seq.Play().WaitForCompletion();
    }

    public YieldInstruction PlayEscapeAnimation()
    {
        var delay = 0.75f;
        var seq = DOTween.Sequence();

        _image.transform.localPosition = new Vector3(_originalPosition.x, _originalPosition.y - 60);

        seq.Append(_image.DOFade(1, delay));
        seq.Join(_image.transform.DOScale(new Vector3(1f, 1f, 1f), delay));
        seq.Join(_image.transform.DOLocalJump(_originalPosition, 50f, 1, delay));

        return seq.Play().WaitForCompletion();
    }
}
