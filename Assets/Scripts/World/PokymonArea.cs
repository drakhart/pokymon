using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PokymonArea : MonoBehaviour
{
    [SerializeField] List<Pokymon> wildPokymonList;

    public List<Pokymon> WildPokymonList
    {
        get => wildPokymonList;
        set => wildPokymonList = value;
    }

    public Pokymon GetRandomWildPokymon()
    {
        var pokymon = wildPokymonList[Random.Range(0, wildPokymonList.Count - 1)];
        pokymon.InitPokymon();

        return pokymon;
    }
}
