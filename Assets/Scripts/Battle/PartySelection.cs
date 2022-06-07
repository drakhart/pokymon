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

    private Tween _dialogTextTween;
    private PartyMemberHUD[] _partyMemberHUDList;
    private PokymonParty _pokymonParty;

    public void SetupPartySelection()
    {
        _partyMemberHUDList = GetComponentsInChildren<PartyMemberHUD>(true);
    }

    public void UpdatePartyData(PokymonParty pokymonParty)
    {
        _pokymonParty = pokymonParty;

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
