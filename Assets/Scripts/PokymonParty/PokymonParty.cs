using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokymonParty : MonoBehaviour
{
    [SerializeField] List<Pokymon> pokymonList;

    public List<Pokymon> PokymonList => pokymonList;

    public Pokymon FirstAvailablePokymon => pokymonList.Where(p => !p.IsKnockedOut).FirstOrDefault();

    public int AvailablePokymonCount => pokymonList.Count(p => !p.IsKnockedOut);

    public bool HasAnyPokymonAvailable => AvailablePokymonCount > 0;

    public int PokymonCount => pokymonList.Count;

    private void Start() {
        foreach (var pokymon in pokymonList)
        {
            pokymon.InitPokymon();
        }
    }

    public bool AddPokymon(Pokymon pokymon)
    {
        if (PokymonCount < Constants.MAX_PARTY_POKYMON_COUNT)
        {
            PokymonList.Add(pokymon);

            return true;
        }

        // TODO: implement sending pokymon to central storage (Bill's PC)
        return false;
    }
}
