using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialog
{
    [SerializeField] private List<string> _lines;
    public List<string> Lines => _lines;

}
