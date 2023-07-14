using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquareTile : Tile
{
    public SquareTile(HashSet<TileFrequency> legalTiles, List<Tile> adjacentTiles)
    {
        Collapsed = false;
        Sides = 4;
        this.LegalTiles = legalTiles;
        this.AdjacentTiles = adjacentTiles;
        this.legalTileIdxs = (HashSet<int>)legalTiles.Select(x => x.name);
        this.entropyNoise = Random.value / 10f;
        entropy = Entropy();
    }
    public SquareTile()
    {
        Collapsed = false;
        Sides = 4;
        entropy = float.MaxValue;
        this.entropyNoise = Random.value / 10f;
        this.LegalTiles = new HashSet<TileFrequency>();
        this.AdjacentTiles = new List<Tile>(new Tile[Sides]);
        this.legalTileIdxs = new HashSet<int>();
    }

    public override void Propagate(List<SquareTileRule> rules, C5.IntervalHeap<SquareTile> tileQueue)
    {
        var westTile = AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.WEST);
        var northTile = AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.NORTH);
        var eastTile = AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.EAST);
        var southTile = AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.SOUTH);
        var removed = LegalTiles.RemoveWhere(x => {
            var thisRule = rules.Find(y => y.tileIdx == x.tileIdx);
            var westCompatible = westTile == null || thisRule.WestPermitted.Overlaps(westTile.legalTileIdxs);
            var northCompatible = northTile == null || thisRule.NorthPermitted.Overlaps(northTile.legalTileIdxs);
            var eastCompatible = eastTile == null || thisRule.EastPermitted.Overlaps(eastTile.legalTileIdxs);
            var southCompatible = southTile == null || thisRule.SouthPermitted.Overlaps(southTile.legalTileIdxs);

            var isCompatible = westCompatible && northCompatible && eastCompatible && southCompatible;
            if (!isCompatible)
            {
                legalTileIdxs.Remove(x.tileIdx);
            }
            return !isCompatible;
        });

        if(removed > 0)
        {
            if (LegalTiles.Count == 1)
            {
                Collapsed = true;
                SelectedTile = LegalTiles.First().name;
            }
            else if (LegalTiles.Count == 0)
            {
                throw new System.Exception("Impossible to solve based on sample."); // Maybe add backtracking
            }
            else
            {
                entropy = Entropy();
                tileQueue.Add(this);
            }

            PropagateNeighbours(rules, tileQueue);
        }
    }

    private void PropagateNeighbours(List<SquareTileRule> rules, C5.IntervalHeap<SquareTile> tileQueue)
    {
        var westTile = AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.WEST);
        var northTile = AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.NORTH);
        var eastTile = AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.EAST);
        var southTile = AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.SOUTH);

        if (westTile != null && !westTile.Collapsed)
        {
            westTile.Propagate(rules, tileQueue);
        }
        if (northTile != null && !northTile.Collapsed)
        {
            northTile.Propagate(rules, tileQueue);
        }
        if (eastTile != null && !eastTile.Collapsed)
        {
            eastTile.Propagate(rules, tileQueue);
        }
        if (southTile != null && !southTile.Collapsed)
        {
            southTile.Propagate(rules, tileQueue);
        }
    }

    public override float Entropy()
    {
        var totalWeight = LegalTiles.Aggregate(0, (acc, x) => acc += x.frequency);
        var sumOfWeightLogWeight = LegalTiles.Aggregate(0f, (acc, x) => x.frequency *  Mathf.Log(x.frequency, 2));
        return Mathf.Log(totalWeight, 2) - (sumOfWeightLogWeight / totalWeight) + entropyNoise;
    }

    public override void Collapse(List<SquareTileRule> rules, C5.IntervalHeap<SquareTile> tileQueue)
    {
        Collapsed = true;
        var random = Random.Range(0, LegalTiles.Count);
        var tile = LegalTiles.ElementAt(random);
        legalTileIdxs.RemoveWhere(x => x != tile.tileIdx);
        LegalTiles.RemoveWhere(x => x != tile);
        SelectedTile = tile.name;
        PropagateNeighbours(rules, tileQueue);
    }
}
