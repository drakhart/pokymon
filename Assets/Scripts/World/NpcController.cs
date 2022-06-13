using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog _dialog;
    [SerializeField] private List<Sprite> _frameList;

    private CustomAnimator _animator;

    private void Start() {
        var renderer = GetComponent<SpriteRenderer>();

        _animator = new CustomAnimator(renderer, _frameList);
        _animator.Start();
    }

    private void Update() {
        _animator.Update();
    }

    public void Interact()
    {
        DialogManager.SharedInstance.StartDialog(_dialog);
    }
}
