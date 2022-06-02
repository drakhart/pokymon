using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private MoveBase _base;
    public MoveBase Base
    {
        get => _base;
        set => _base = value;
    }

    private int _pp;
    public int PP
    {
        get => _pp;
        set => _pp = Mathf.Clamp(value, 0, MaxPP);
    }

    public Move(MoveBase mBase)
    {
        _base = mBase;

        _pp = _base.PP;
    }

    public int MaxPP => _base.PP;

    public bool HasAvailablePP => _pp > 0;
}
