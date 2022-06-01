using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PokymonArea : MonoBehaviour
{
    [SerializeField] private List<Pokymon> _wildPokymonList;
    public List<Pokymon> WildPokymonList => _wildPokymonList;

    public Pokymon GetRandomWildPokymon()
    {
        var pokymon = _wildPokymonList[Random.Range(0, _wildPokymonList.Count)];
        pokymon.InitPokymon();

        return pokymon;
    }
}
