using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySelection : MonoBehaviour
{
    [SerializeField] private Text dialogText;

    private PartyMemberHUD[] _partyMemberHUDList;

    private List<Pokymon> _pokymonList;

    public void SetupPartySelection()
    {
        _partyMemberHUDList = GetComponentsInChildren<PartyMemberHUD>(true);
    }

    public void UpdatePartyData(List<Pokymon> pokymonList)
    {
        this._pokymonList = pokymonList;

        dialogText.text = "Choose a Pokymon.";

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
        dialogText.text = message;
    }
}
