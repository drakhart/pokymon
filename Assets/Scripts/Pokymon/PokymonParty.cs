using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokymonParty : MonoBehaviour
{
    [SerializeField] List<Pokymon> _pokymonList;
    public List<Pokymon> PokymonList => _pokymonList;

    public Pokymon FirstAvailablePokymon => _pokymonList.Where(p => !p.IsKnockedOut).FirstOrDefault();

    public int AvailablePokymonCount => _pokymonList.Count(p => !p.IsKnockedOut);

    public bool HasAnyPokymonAvailable => AvailablePokymonCount > 0;

    public int PokymonCount => _pokymonList.Count;

    private void Start() {
        foreach (var pokymon in _pokymonList)
        {
            pokymon.InitPokymon();
        }
    }

    public bool AddPokymon(Pokymon pokymon)
    {
        if (PokymonCount < Constants.MAX_PARTY_POKYMON_COUNT)
        {
            pokymon.IsWild = false;
            PokymonList.Add(pokymon);

            return true;
        }

        // TODO: implement sending pokymon to central storage (Bill's PC)
        return false;
    }
}
