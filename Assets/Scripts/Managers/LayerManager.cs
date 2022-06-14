using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayers;
    public LayerMask InteractableLayers => _interactableLayers;

    [SerializeField] private LayerMask _playerLayers;
    public LayerMask PlayerLayers => _playerLayers;

    [SerializeField] private LayerMask _pokymonAreaLayers;
    public LayerMask PokymonAreaLayers => _pokymonAreaLayers;

    [SerializeField] private LayerMask _solidObjectsLayers;
    public LayerMask SolidObjectsLayers => _solidObjectsLayers;

    public LayerMask CollisionLayers => _interactableLayers | _playerLayers | _solidObjectsLayers;

    public static LayerManager SharedInstance;

    private void Awake() {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            print("There's more than one LayerManager instance!");

            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
