using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PartySelection : MonoBehaviour
{
    [SerializeField] private float _dialogSpeed = 45f;
    [SerializeField] private Text _dialogText;

    private Tween _dialogTextTween;
    private PartyMemberHUD[] _partyMemberHUDList;
    private List<Pokymon> _pokymonList;

    public void SetupPartySelection()
    {
        _partyMemberHUDList = GetComponentsInChildren<PartyMemberHUD>(true);
    }

    public void UpdatePartyData(List<Pokymon> pokymonList)
    {
        _pokymonList = pokymonList;

        SetDialogText("Choose a Pokymon.");

        for (var i = 0; i < _partyMemberHUDList.Length; i++)
        {
            if (i < pokymonList.Count)
            {
                _partyMemberHUDList[i].SetPokymonData(pokymonList[i]);
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
        for (var i = 0; i < _pokymonList.Count; i++)
        {
            _partyMemberHUDList[i].SetSelectedPokymon(i == selectedPokymon);
        }
    }

    public void SetDialogText(string message)
    {
        _dialogTextTween.Kill();

        _dialogText.text = "";

        _dialogTextTween = DOTween.To(() => _dialogText.text, x => _dialogText.text = x, message, message.Length / _dialogSpeed)
            .SetEase(Ease.Linear);
    }
}
