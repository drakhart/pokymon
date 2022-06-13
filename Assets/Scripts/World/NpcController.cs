using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog _dialog;

    public void Interact()
    {
        DialogManager.SharedInstance.StartDialog(_dialog);
    }
}
