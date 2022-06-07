using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private GameObject _actionSelector;
    [SerializeField] private List<Text> _actionTexts;
    [SerializeField] private Text _dialogText;
    [SerializeField] private GameObject _moveDetails;
    [SerializeField] private GameObject _moveSelector;
    [SerializeField] private List<Text> _moveTexts;
    [SerializeField] private Text _ppText;
    [SerializeField] private Text _typeText;

    [SerializeField] private AudioClip _dialogSFX;
    [SerializeField] private float _dialogPauseSecs = 1f;
    [SerializeField] private float _dialogSpeed = 30f;

    [SerializeField] private float _highPPThreshold = 0.5f;
    [SerializeField] private float _lowPPThreshold = 0.20f;

    private Tween _dialogTextTween;

    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < _actionTexts.Count; i++)
        {
            _actionTexts[i].color = i == selectedAction
                ? ColorManager.SharedInstance.SelectedText
                : ColorManager.SharedInstance.DefaultText;
        }
    }

    public void SelectMove(int selectedMove)
    {
        for (int i = 0; i < _moveTexts.Count; i++)
        {
            _moveTexts[i].color = i == selectedMove
                ? ColorManager.SharedInstance.SelectedText
                : ColorManager.SharedInstance.DefaultText;
        }
    }

    public Coroutine SetDialogText(string message)
    {
        _dialogTextTween.Kill();

        return StartCoroutine(AnimateDialogText(message));
    }

    public void SetMoveDetails(Move move)
    {
        _ppText.color = PPColor(move.NormalizedPP);
        _ppText.text = $"PP {move.PP}/{move.MaxPP}";
        _typeText.text = move.Base.Type.ToString();
        _typeText.color = ColorManager.SharedInstance.ByPokymonType(move.Base.Type);
    }

    public void SetMoveTexts(List<Move> moves)
    {
        for (int i = 0; i < _moveTexts.Count; i++)
        {
            _moveTexts[i].text = i < moves.Count ? moves[i].Base.Name : "---";
        }
    }

    public void ToggleActionSelector(bool active)
    {
        _actionSelector.SetActive(active);
    }

    public void ToggleDialogText(bool active)
    {
        _dialogText.enabled = active;
    }

    public void ToggleMoveSelector(bool active)
    {
        _moveSelector.SetActive(active);
        _moveDetails.SetActive(active);
    }

    private Color PPColor(float scale)
    {
        if (scale <= _lowPPThreshold)
        {
            return ColorManager.SharedInstance.LowPP;
        }
        else if (scale > _highPPThreshold)
        {
            return ColorManager.SharedInstance.HighPP;
        }
        else
        {
            return ColorManager.SharedInstance.MediumPP;
        }
    }

    private IEnumerator AnimateDialogText(string message)
    {
        var lastSoundTime = Time.time;

        _dialogText.text = "";
        _dialogTextTween = DOTween.To(() => _dialogText.text, x => _dialogText.text = x, message, message.Length / _dialogSpeed)
            .SetEase(Ease.Linear)
            .OnUpdate(() => {
                if (Time.time > lastSoundTime + _dialogSFX.length)
                {
                    AudioManager.SharedInstance.PlaySFX(_dialogSFX);
                    lastSoundTime = Time.time;
                }
            });

        yield return _dialogTextTween.WaitForCompletion();
        yield return new WaitForSecondsRealtime(_dialogPauseSecs);
    }
}
