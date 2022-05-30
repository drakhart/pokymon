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

    private void Start() {
        foreach (var pokymon in pokymonList)
        {
            pokymon.InitPokymon();
        }
    }
}
