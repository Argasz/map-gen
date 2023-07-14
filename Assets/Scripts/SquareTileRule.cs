using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareTileRule : TileRule
{
    public SquareTileRule() {
        geometry = MapGeometry.SQUARE;
        frequency = 0;
        WestPermitted = new HashSet<int>();
        NorthPermitted = new HashSet<int>();
        EastPermitted = new HashSet<int>();
        SouthPermitted = new HashSet<int>();
    }
    public HashSet<int> WestPermitted;
    public HashSet<int> NorthPermitted;
    public HashSet<int> EastPermitted;
    public HashSet<int> SouthPermitted;
}
