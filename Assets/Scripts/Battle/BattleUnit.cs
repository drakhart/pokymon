using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    public bool IsPlayer => isPlayer;

    [SerializeField] private BattleUnitHUD hud;
    public BattleUnitHUD HUD => hud;

    public Pokymon Pokymon;

    private Image _image;

    private Color originalColor;

    private Vector3 originalPosition;

    private void Awake() {
        _image = GetComponent<Image>();
        originalColor = _image.color;
        originalPosition = _image.transform.localPosition;
    }

    public void SetupPokymon(Pokymon pokymon)
    {
        Pokymon = pokymon;

        _image.sprite = IsPlayer ? Pokymon.Base.BackSprite : Pokymon.Base.FrontSprite;
        _image.color = originalColor;
        _image.transform.localScale = new Vector3(1f, 1f, 1f);

        hud.SetPokymonData(Pokymon);

        PlaySetupAnimation();
    }

    public void PlayKnockedOutAnimation()
    {
        var delay = 1.5f;
        var seq = DOTween.Sequence();
        seq.Append(_image.transform.DOLocalMoveY(originalPosition.y - (IsPlayer ? 1 : 2) * 150, delay / (IsPlayer ? 2 : 1)));
        seq.Join(_image.DOFade(0, delay / (IsPlayer ? 2 : 1)));
    }

    public void PlayPhysicalMoveAnimation()
    {
        var delay = 0.3f;
        var seq = DOTween.Sequence();
        seq.Append(_image.transform.DOLocalMoveX(originalPosition.x + (IsPlayer ? 1 : -1) * 60, delay));
        seq.Append(_image.transform.DOLocalMoveX(originalPosition.x, delay));
        seq.Play();
    }

    public void PlayReceiveDamageAnimation()
    {
        var delay = 0.1f;
        var seq = DOTween.Sequence();

        seq.Append(_image.DOColor(Color.gray, delay));

        for (var i = 0; i < 2; i++)
        {
            seq.Append(_image.transform.DOLocalMoveX(originalPosition.x + (IsPlayer ? -1 : 1) * 10, delay));
            seq.Append(_image.transform.DOLocalMoveX(originalPosition.x, delay));
        }

        seq.Append(_image.DOColor(originalColor, delay));

        seq.Play();
    }

    public void PlaySetupAnimation()
    {
        _image.transform.localPosition = new Vector3(originalPosition.x + (IsPlayer ? -1 : 1) * 400, originalPosition.y);
        _image.transform.DOLocalMoveX(originalPosition.x, 1);
    }

    public void PlaySwitchAnimation()
    {
        _image.transform.DOLocalMoveX(originalPosition.x + (IsPlayer ? -1 : 1) * 400, 1);
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var delay = 0.75f;
        var seq = DOTween.Sequence();

        seq.Append(_image.DOFade(0, delay));
        seq.Join(_image.transform.DOScale(new Vector3(0.25f, 0.25f, 1f), delay));
        seq.Join(_image.transform.DOLocalMoveY(originalPosition.y + 50, delay));

        yield return seq.Play().WaitForCompletion();
    }

    public IEnumerator PlayEscapeAnimation()
    {
        var delay = 0.75f;
        var seq = DOTween.Sequence();

        _image.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y - 60);

        seq.Append(_image.DOFade(1, delay));
        seq.Join(_image.transform.DOScale(new Vector3(1f, 1f, 1f), delay));
        seq.Join(_image.transform.DOLocalJump(originalPosition, 50f, 1, delay));

        yield return seq.Play().WaitForCompletion();
    }
}
