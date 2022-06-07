using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PartySelection : MonoBehaviour
{
    [SerializeField] private AudioClip _dialogSFX;
    [SerializeField] private float _dialogSpeed = 30f;
    [SerializeField] private Text _dialogText;

    private int _currSelection;
    private Tween _dialogTextTween;
    private PartyMemberHUD[] _partyMemberHUDList;
    private PokymonParty _pokymonParty;

    public void SetupPartySelection()
    {
        _partyMemberHUDList = GetComponentsInChildren<PartyMemberHUD>(true);
    }

    public void HandlePartySelection(Pokymon currentPokymon, Action<Pokymon> OnSelected)
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                _currSelection = (_currSelection + 1) % 2 + 2 * (int)(_currSelection / 2);
            }
            else
            {
                _currSelection = (_currSelection + (Input.GetAxis("Vertical") < 0 ? 2 : _pokymonParty.PokymonCount - 2 + _pokymonParty.PokymonCount % 2))
                    % (_pokymonParty.PokymonCount % 2 == 0 ? _pokymonParty.PokymonCount : _pokymonParty.PokymonCount + 1);
            }

            _currSelection = Mathf.Clamp(_currSelection, 0, _pokymonParty.PokymonCount - 1);

            SelectPokymon(_currSelection);
        }

        if (Input.GetButtonDown("Submit"))
        {
            var selectedPokymon = _pokymonParty.PokymonList[_currSelection];

            if (selectedPokymon.IsKnockedOut)
            {
                SetDialogText("You can't choose a knocked out Pokymon.");

                return;
            }
            else if (selectedPokymon == currentPokymon)
            {
                SetDialogText("You can't choose the Pokymon currently in battle.");

                return;
            }

            OnSelected?.Invoke(selectedPokymon);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            OnSelected?.Invoke(null);
        }
    }

    public void UpdatePartyData(PokymonParty pokymonParty)
    {
        _pokymonParty = pokymonParty;
        _currSelection = 0;

        SetDialogText("Choose a Pokymon.");

        for (var i = 0; i < _partyMemberHUDList.Length; i++)
        {
            if (i < _pokymonParty.PokymonCount)
            {
                _partyMemberHUDList[i].SetPokymonData(_pokymonParty.PokymonList[i]);
                _partyMemberHUDList[i].gameObject.SetActive(true);
            }
            else
            {
                _partyMemberHUDList[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectPokymon(int selectedPokymon)
    {
        for (var i = 0; i < _pokymonParty.PokymonCount; i++)
        {
            _partyMemberHUDList[i].SetSelectedPokymon(i == selectedPokymon);
        }
    }

    public void SetDialogText(string message)
    {
        var lastSoundTime = Time.time;

        _dialogTextTween.Kill();

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
    }
}
