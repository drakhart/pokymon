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

    private Tween _dialogTextTween;

    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < _actionTexts.Count; i++)
        {
            _actionTexts[i].color = i == selectedAction
                ? ColorManager.SharedInstance.Selected
                : ColorManager.SharedInstance.Default;
        }
    }

    public void SelectMove(int selectedMove)
    {
        for (int i = 0; i < _moveTexts.Count; i++)
        {
            _moveTexts[i].color = i == selectedMove
                ? ColorManager.SharedInstance.Selected
                : ColorManager.SharedInstance.Default;
        }
    }

    public Coroutine SetDialogText(string message)
    {
        _dialogTextTween.Kill();
        StopAllCoroutines();

        return StartCoroutine(AnimateDialogText(message));
    }

    public void SetMoveDetails(Move move)
    {
        _ppText.color = move.HasAvailablePP
            ? ColorManager.SharedInstance.Default
            : ColorManager.SharedInstance.Warning;
        _ppText.text = $"PP {move.PP}/{move.MaxPP}";
        _typeText.text = move.Base.Type.ToString();
        _typeText.color = ColorManager.SharedInstance.PokymonType(move.Base.Type);
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

    private IEnumerator AnimateDialogText(string message)
    {
        _dialogText.text = "";

        _dialogTextTween = DOTween.To(() => _dialogText.text, x => _dialogText.text = x, message, message.Length / _dialogSpeed)
            .SetEase(Ease.Linear)
            .OnUpdate(() => {
                if ((int)(_dialogText.text.Length % (_dialogSpeed / 15)) == 0)
                {
                    AudioManager.SharedInstance.PlaySFX(_dialogSFX);
                }
            });

        yield return _dialogTextTween.WaitForCompletion();
        yield return new WaitForSecondsRealtime(_dialogPauseSecs);
    }
}
