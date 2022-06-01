using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private float _dialogCharsPerSec = 30f;
    [SerializeField] private float _dialogPauseSecs = 1f;

    [SerializeField] private Color _selectedColor = Color.blue;
    [SerializeField] private Color _defaultColor = Color.black;
    [SerializeField] private Color _warningColor = Color.red;

    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < _actionTexts.Count; i++)
        {
            _actionTexts[i].color = i == selectedAction ? _selectedColor : _defaultColor;
        }
    }

    public void SelectMove(int selectedMove)
    {
        for (int i = 0; i < _moveTexts.Count; i++)
        {
            _moveTexts[i].color = i == selectedMove ? _selectedColor : _defaultColor;
        }
    }

    public Coroutine SetDialogText(string message)
    {
        StopAllCoroutines();
        return StartCoroutine(WriteText(message));
    }

    public void SetMoveDetails(Move move)
    {
        _ppText.color = move.HasAvailablePP ? _defaultColor : _warningColor;
        _ppText.text = $"PP {move.PP}/{move.Base.PP}";
        _typeText.text = move.Base.Type.ToString();
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

    private IEnumerator WriteText(string message)
    {
        _dialogText.text = "";

        foreach (var character in message)
        {
            _dialogText.text += character;

            yield return new WaitForSecondsRealtime(1 / _dialogCharsPerSec);
        }

        yield return new WaitForSecondsRealtime(_dialogPauseSecs);
    }
}
