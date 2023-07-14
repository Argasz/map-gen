using C5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Overlap
{
    public class OverlapMapGenerator
    {
        private IntervalHeap<SquareTile> ChangedTiles;
        private Dictionary<int, int> FrequencyMap;
        private Dictionary<int, string> ColorMap;


        public OverlapMapGenerator()
        {
            FrequencyMap = new Dictionary<int, int>();
            ColorMap = new Dictionary<int, string>();
        }

        public List<List<SquareTile>> Generate(Dictionary<int, SquareTileRule> rules, int dimx, int dimy)
        {
            var map = new List<List<SquareTile>>(new List<SquareTile>[dimx]);
            ChangedTiles = new IntervalHeap<SquareTile>();
            Initialize(rules, dimx, dimy, map);
            while (ChangedTiles.Count > 0)
            {
                var tile = ChangedTiles.DeleteMin();
                if (!tile.Collapsed)
                {
                    tile.Collapse(ColorMap);
                    Propagate(rules, tile);
                }
            }
            return map;
        }

        private void PropagateWithRemovals()
        {

        }

        private void Propagate(Dictionary<int, SquareTileRule> rules, SquareTile tile)
        {
            var toUpdate = new Stack<Tile>(tile.AdjacentTiles.Where(x => x != null && !x.Collapsed));
            while (toUpdate.Count > 0)
            {
                tile = (SquareTile)toUpdate.Pop();
                var westTile = tile.AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.WEST);
                var northTile = tile.AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.NORTH);
                var eastTile = tile.AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.EAST);
                var southTile = tile.AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.SOUTH);
                var removed = tile.LegalTiles.RemoveWhere(x =>
                {
                    var thisRule = rules[x];
                    var westCompatible = westTile == null || thisRule.WestPermitted.Overlaps(westTile.LegalTiles);
                    var northCompatible = northTile == null || thisRule.NorthPermitted.Overlaps(northTile.LegalTiles);
                    var eastCompatible = eastTile == null || thisRule.EastPermitted.Overlaps(eastTile.LegalTiles);
                    var southCompatible = southTile == null || thisRule.SouthPermitted.Overlaps(southTile.LegalTiles);

                    var isCompatible = westCompatible && northCompatible && eastCompatible && southCompatible;

                    return !isCompatible;
                });

                if (removed > 0)
                {
                    if (tile.LegalTiles.Count == 1)
                    {
                        tile.Collapsed = true;
                        tile.SelectedTile = ColorMap[tile.LegalTiles.First()];
                    }
                    else if (tile.LegalTiles.Count == 0)
                    {
                        throw new Exception("Impossible to solve based on sample."); // Maybe add backtracking
                    }
                    else
                    {
                        tile.UpdateEntropy(FrequencyMap);
                        ChangedTiles.Add(tile);
                    }
                    foreach (var adj in tile.AdjacentTiles)
                    {
                        if (adj != null && !adj.Collapsed)
                        {
                            toUpdate.Push(adj);
                        }
                    }
                }
            }
        }

        private void Initialize(Dictionary<int, SquareTileRule> rules, int dimx, int dimy, List<List<SquareTile>> map)
        {
            foreach (var rule in rules.Values)
            {
                FrequencyMap.Add(rule.tileIdx, rule.frequency);
                ColorMap.Add(rule.tileIdx, rule.name);
            }

            for (int x = 0; x < dimx; x++)
            {
                map[x] = new List<SquareTile>(new SquareTile[dimy]);
                for (int y = 0; y < dimy; y++)
                {
                    var tile = new SquareTile(rules.Count);
                    tile.LegalTiles = new System.Collections.Generic.HashSet<int>(rules.Values.Select(x => x.tileIdx));
                    tile.UpdateEntropy(FrequencyMap);
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
                    ChangedTiles.Add(tile);
                }
            }
        }
    }
}
