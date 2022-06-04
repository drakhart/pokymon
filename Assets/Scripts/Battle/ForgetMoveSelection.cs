using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ForgetMoveSelection : MonoBehaviour
{
    [SerializeField] private Text[] _moveTextList;

    private int _currSelectedMove;

    public void SetMoveTexts(List<Move> moveList, LearnableMove learnableMove)
    {
        _currSelectedMove = 0;

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
            _currSelectedMove = (_currSelectedMove + (Input.GetAxis("Vertical") < 0 ? 1 : Constants.MAX_POKYMON_MOVE_COUNT))
                % (Constants.MAX_POKYMON_MOVE_COUNT + 1);

            UpdateSelectedMove();
        }

        if (Input.GetButtonDown("Submit"))
        {
            OnSelected?.Invoke(_currSelectedMove);
        }
    }

    private void UpdateSelectedMove()
    {
        for (var i = 0; i < _moveTextList.Length; i++)
        {
            _moveTextList[i].color = i == _currSelectedMove
                ? ColorManager.SharedInstance.SelectedText
                : ColorManager.SharedInstance.DefaultText;
        }
    }
}
