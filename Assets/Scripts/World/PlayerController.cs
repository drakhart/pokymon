using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Character))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private AudioClip _stepsSFX;

    private Character _character;
    private Vector2 _input;

    public event Action OnPokymonEncounter;
    public event Action<TrainerController> OnTrainerEncounter;

    private void Awake() {
        _character = GetComponent<Character>();
    }

    public void HandleUpdate() {
        if (!_character.IsMoving)
        {
            _input.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            _input.y = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

            if (_input != Vector2.zero)
            {
                StartCoroutine(_character.MoveTowards(_input,
                    () => AudioManager.SharedInstance.PlaySFX(_stepsSFX),
                    () => OnMoveFinish()
                ));
            }

            if (Input.GetButtonDown("Submit"))
            {
                Interact();
            }
        }

        _character.HandleUpdate();
    }

    private void OnMoveFinish()
    {
        CheckForWildPokymon();
        CheckForTrainer();
    }

    private void CheckForWildPokymon()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.25f, LayerManager.SharedInstance.PokymonAreaLayers) != null)
        {
            if (Random.Range(0, 100) < Constants.POKYMON_ENCOUNTER_ODDS)
            {
                _character.Animator.IsMoving = false;

                OnPokymonEncounter?.Invoke();
            }
        }
    }

    private void CheckForTrainer()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.25f, LayerManager.SharedInstance.FoVLayers);

        if (collider != null)
        {
            var trainer = collider.GetComponentInParent<TrainerController>();

            if (trainer != null)
            {
                _character.Animator.IsMoving = false;

                OnTrainerEncounter?.Invoke(trainer);
            }
        }
    }

    private void Interact()
    {
        var facingDirection = new Vector3(_character.Animator.MoveX, _character.Animator.MoveY);
        var interactPosition = transform.position + facingDirection;

        Debug.DrawLine(transform.position, interactPosition, Color.magenta, 1.0f);

        var collider = Physics2D.OverlapCircle(interactPosition, 0.25f, LayerManager.SharedInstance.InteractableLayers);

        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform.position);
        }
    }
}
