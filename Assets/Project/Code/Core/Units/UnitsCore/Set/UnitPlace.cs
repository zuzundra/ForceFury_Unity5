using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct UnitPlace
{
    [SerializeField]
    EUnitRange _range;
    public EUnitRange Range
    {
        get
        {
            return _range;
        }
        set
        {
            _range = value;
        }
    }

    [SerializeField]
    EUnitPosition _position;
    public EUnitPosition Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
        }
    }
}
