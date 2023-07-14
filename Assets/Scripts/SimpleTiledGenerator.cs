using C5;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleTiledGenerator
{
    private IntervalHeap<SquareTile> changedTiles;

    public SimpleTiledGenerator()
    {
    }

    public List<List<SquareTile>> Generate(List<SquareTileRule> rules, int dimx, int dimy)
    {
        var map = new List<List<SquareTile>>(new List<SquareTile>[dimx]);
        changedTiles = new IntervalHeap<SquareTile>();
        Initialize(rules, dimx, dimy, map);
        while(changedTiles.Count > 0)
        {
            var tile = changedTiles.DeleteMin();
            if (!tile.Collapsed)
            {
                tile.Collapse(rules, changedTiles);
            }
        }
        return map;
    }

    private void Initialize(List<SquareTileRule> rules, int dimx, int dimy, List<List<SquareTile>> map)
    {
        for (int x = 0; x < dimx; x++)
        {
            map[x] = new List<SquareTile>(new SquareTile[dimy]);
            for (int y = 0; y < dimy; y++)
            {
                var tile = new SquareTile();
                tile.LegalTiles = new System.Collections.Generic.HashSet<TileFrequency>(rules.Select(x => new TileFrequency(x.name, x.frequency, x.tileIdx)));
                tile.legalTileIdxs = new System.Collections.Generic.HashSet<int>(tile.LegalTiles.Select(x => x.tileIdx));
                tile.entropy = tile.Entropy();
                if (y > 0)
                {
                    var northTile = map[x][y - 1];
                    tile.AdjacentTiles[(int)CardinalDirection.NORTH] = northTile;
                    northTile.AdjacentTiles[(int)CardinalDirection.SOUTH] = tile;
                }
                if (x > 0)
                {
                    var westTile = map[x - 1][y];
                    tile.AdjacentTiles[(int)CardinalDirection.WEST] = westTile;
                    westTile.AdjacentTiles[(int)CardinalDirection.EAST] = tile;
                }
                map[x][y] = tile;
                changedTiles.Add(tile);
            }
        }
    }
}
