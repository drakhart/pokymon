using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgetMoveSelection : MonoBehaviour
{
    [SerializeField] private Text[] _moveTextList;

    private int _currSelection;

    public void SetMoveTexts(List<Move> moveList, LearnableMove learnableMove)
    {
        _currSelection = 0;

        for (var i = 0; i < moveList.Count; i++)
        {
            _moveTextList[i].text = moveList[i].Base.Name;
        }

        _moveTextList[_moveTextList.Length - 1].text = $"{learnableMove.Base.Name} (New)";

        UpdateSelectedMove();
    }

    public void HandlePlayerSelectForgetMove(Action<int> OnSelected)
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            _currSelection = (_currSelection + (Input.GetAxis("Vertical") < 0 ? 1 : Constants.MAX_POKYMON_MOVE_COUNT))
                % (Constants.MAX_POKYMON_MOVE_COUNT + 1);

            UpdateSelectedMove();
        }

        if (Input.GetButtonDown("Submit"))
        {
            OnSelected?.Invoke(_currSelection);
        }
    }

    private void UpdateSelectedMove()
    {
        for (var i = 0; i < _moveTextList.Length; i++)
        {
            _moveTextList[i].color = i == _currSelection
                ? ColorManager.SharedInstance.SelectedText
                : ColorManager.SharedInstance.DefaultText;
        }
    }
}
