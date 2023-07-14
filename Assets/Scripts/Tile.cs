using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : IComparable<Tile>
{
    public bool Collapsed;
    public int Sides;
    public float entropy;
    public float entropyNoise;
    public System.Collections.Generic.HashSet<TileFrequency> LegalTiles;
    public List<Tile> AdjacentTiles;
    public System.Collections.Generic.HashSet<int> legalTileIdxs;
    public string SelectedTile;

    public abstract void Propagate(List<SquareTileRule> rules, IntervalHeap<SquareTile> tileQueue);
    public abstract float Entropy();
    public abstract void Collapse(List<SquareTileRule> rules, IntervalHeap<SquareTile> tileQueue);

    public int CompareTo(Tile other)
    {
        return entropy.CompareTo(other.entropy);
    }
}
