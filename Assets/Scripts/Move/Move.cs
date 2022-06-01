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
        set => _pp = value;
    }

    public bool HasAvailablePP => _pp > 0;

    public Move(MoveBase mBase)
    {
        this._base = mBase;
        this._pp = mBase.PP;
    }
}
