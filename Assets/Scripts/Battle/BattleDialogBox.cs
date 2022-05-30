using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private Text dialogText;

    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject moveSelector;
    [SerializeField] private GameObject moveDetails;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> moveTexts;
    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;

    public float charactersPerSecond = 30f;

    public float timeToWaitAfterDialog = 1f;

    public Color selectedColor = Color.blue;
    public Color defaultColor = Color.black;
    public Color warningColor = Color.red;

    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].color = i == selectedAction ? selectedColor : defaultColor;
        }
    }

    public void SelectMove(int selectedMove)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            moveTexts[i].color = i == selectedMove ? selectedColor : defaultColor;
        }
    }

    public Coroutine SetDialogText(string message)
    {
        StopAllCoroutines();
        return StartCoroutine(WriteText(message));
    }

    public void SetMoveDetails(Move move)
    {
        ppText.color = move.HasAvailablePP ? defaultColor : warningColor;
        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();
    }

    public void SetMoveTexts(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            moveTexts[i].text = i < moves.Count ? moves[i].Base.Name : "---";
        }
    }

    public void ToggleDialogText(bool active)
    {
        dialogText.enabled = active;
    }

    public void ToggleActionSelector(bool active)
    {
        actionSelector.SetActive(active);
    }

    public void ToggleMoveSelector(bool active)
    {
        moveSelector.SetActive(active);
        moveDetails.SetActive(active);
    }

    private IEnumerator WriteText(string message)
    {
        dialogText.text = "";

        foreach (var character in message)
        {
            dialogText.text += character;

            yield return new WaitForSecondsRealtime(1 / charactersPerSecond);
        }

        yield return new WaitForSecondsRealtime(timeToWaitAfterDialog);
    }
}
