using System;
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

    private int _currActionSelection;
    private int _currMoveSelection;
    private Tween _dialogTextTween;
    private List<Move> _moveList;

    private void Start() {
        _currActionSelection = 0;
    }

    public void HandlePlayerActionSelection(Action<int> OnSelected)
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                _currActionSelection = (_currActionSelection + 1) % 2 + (_currActionSelection >= 2 ? 2 : 0);
            }
            else
            {
                _currActionSelection = (_currActionSelection + 2) % 4;
            }

            SelectAction();
        }

        if (Input.GetButtonDown("Submit"))
        {
            OnSelected?.Invoke(_currActionSelection);
        }
    }

    public void HandlePlayerMoveSelection(Action<Move> OnSelected)
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                _currMoveSelection = (_currMoveSelection + 1) % 2 + (_currMoveSelection >= 2 ? 2 : 0);
            }
            else
            {
                _currMoveSelection = (_currMoveSelection + 2) % Constants.MAX_POKYMON_MOVE_COUNT;
            }

            _currMoveSelection = Mathf.Clamp(_currMoveSelection, 0, _moveList.Count - 1);

            SelectMove();
        }

        if (Input.GetButtonDown("Submit"))
        {
            var selectedMove = _moveList[_currMoveSelection];

            if (selectedMove.HasAvailablePP)
            {
                OnSelected?.Invoke(selectedMove);
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
            OnSelected?.Invoke(null);
        }
    }

    public void SelectAction()
    {
        for (int i = 0; i < _actionTexts.Count; i++)
        {
            _actionTexts[i].color = i == _currActionSelection
                ? ColorManager.SharedInstance.SelectedText
                : ColorManager.SharedInstance.DefaultText;
        }
    }

    public void SelectMove()
    {
        for (int i = 0; i < _moveTexts.Count; i++)
        {
            _moveTexts[i].color = i == _currMoveSelection
                ? ColorManager.SharedInstance.SelectedText
                : ColorManager.SharedInstance.DefaultText;
        }

        SetMoveDetails();
    }

    public Coroutine SetDialogText(string message)
    {
        _dialogTextTween.Kill();

        return StartCoroutine(AnimateDialogText(message));
    }

    public void SetMoveDetails()
    {
        var move = _moveList[_currMoveSelection];

        _ppText.text = $"PP {move.PP}/{move.MaxPP}";
        _ppText.color = PPColor(move.NormalizedPP);
        _typeText.text = move.Base.Type.ToString();
        _typeText.color = ColorManager.SharedInstance.ByPokymonType(move.Base.Type);
    }

    public void UpdateMoveData(List<Move> moveList)
    {
        _moveList = moveList;
        _currMoveSelection = 0;

        for (int i = 0; i < _moveTexts.Count; i++)
        {
            _moveTexts[i].text = i < moveList.Count ? moveList[i].Base.Name : "---";
        }

        SelectMove();
    }

    public void ToggleActionSelector(bool active)
    {
        SelectAction();

        _actionSelector.SetActive(active);
    }

    public void ToggleDialogText(bool active)
    {
        if (!active) {
            _dialogTextTween.Kill();
        }

        _dialogText.enabled = active;
    }

    public void ToggleMoveSelector(bool active)
    {
        if (active)
        {
            SetMoveDetails();
        }

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

        ToggleDialogText(true);

        _dialogText.text = "";
        _dialogTextTween = DOTween.To(() => _dialogText.text, x => _dialogText.text = x, message, message.Length / _dialogSpeed)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
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
