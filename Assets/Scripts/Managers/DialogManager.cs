using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private float _dialogSpeed;
    [SerializeField] private AudioClip _dialogSFX;

    private Dialog _currentDialog;
    private int _currentDialogLine;
    private Text _dialogText;
    private Tween _dialogTextTween;
    private Action _onCurrentDialogFinish;

    public event Action OnDialogStart;
    public event Action OnDialogFinish;

    public static DialogManager SharedInstance;

    private bool IsDialogLineBeingWritten => _dialogTextTween != null && _dialogTextTween.IsPlaying();

    private void Awake() {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            print("There's more than one DialogManager instance!");

            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        _dialogText = _dialogBox.GetComponentInChildren<Text>();
    }

    public void HandleUpdate()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (IsDialogLineBeingWritten)
            {
                _dialogTextTween.Complete();

                return;
            }

            if (++_currentDialogLine < _currentDialog.Lines.Count)
            {
                ShowCurrentDialogLine();
            }
            else
            {
                ToggleDialogBox(false);

                OnDialogFinish?.Invoke();
                _onCurrentDialogFinish?.Invoke();
            }
        }
    }

    public void StartDialog(Dialog dialog, Action onNpcDialogFinish)
    {
        _currentDialog = dialog;
        _onCurrentDialogFinish = onNpcDialogFinish;
        _currentDialogLine = 0;

        ShowCurrentDialogLine();

        OnDialogStart?.Invoke();
    }

    private void ShowCurrentDialogLine()
    {
        StartCoroutine(AnimateDialogLine(_currentDialog.Lines[_currentDialogLine]));
    }

    private void ToggleDialogBox(bool active)
    {
        _dialogBox.SetActive(active);
    }

    private IEnumerator AnimateDialogLine(string line)
    {
        var lastSoundTime = Time.time;

        ToggleDialogBox(true);

        _dialogText.text = "";
        _dialogTextTween = DOTween.To(() => _dialogText.text, x => _dialogText.text = x, line, line.Length / _dialogSpeed)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                if (Time.time > lastSoundTime + _dialogSFX.length && _dialogText.text[Math.Max(_dialogText.text.Length - 1, 0)] != ' ')
                {
                    AudioManager.SharedInstance.PlaySFX(_dialogSFX);
                    lastSoundTime = Time.time;
                }
            })
            .OnComplete(() =>
            {
                _dialogTextTween = null;
                _dialogText.text = $"{_dialogText.text} ⇩";
            });

        yield return _dialogTextTween.WaitForCompletion();
    }
}
