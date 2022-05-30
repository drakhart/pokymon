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

    [SerializeField] private bool isWild;
    public bool IsWild => isWild;

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

    public void SetupPokemon(Pokymon pokymon)
    {
        Pokymon = pokymon;

        _image.sprite = IsPlayer ? Pokymon.Base.BackSprite : Pokymon.Base.FrontSprite;
        _image.color = originalColor;

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
}
