using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySelection : MonoBehaviour
{
    [SerializeField] private Text dialogText;

    private PartyMemberHUD[] partyMemberHUDList;

    private List<Pokymon> pokymonList;

    public void SetupPartySelection()
    {
        partyMemberHUDList = GetComponentsInChildren<PartyMemberHUD>();
    }

    public void UpdatePartyData(List<Pokymon> pokymonList)
    {
        this.pokymonList = pokymonList;

        dialogText.text = "Choose a Pokymon.";

        for (var i = 0; i < partyMemberHUDList.Length; i++)
        {
            if (i < pokymonList.Count)
            {
                partyMemberHUDList[i].SetPokymonData(pokymonList[i]);
                partyMemberHUDList[i].gameObject.SetActive(true);
            }
            else
            {
                partyMemberHUDList[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectPokymon(int selectedPokymon)
    {
        for (var i = 0; i < pokymonList.Count; i++)
        {
            partyMemberHUDList[i].SetSelectedPokymon(i == selectedPokymon);
        }
    }

    public void SetDialogText(string message)
    {
        dialogText.text = message;
    }
}
