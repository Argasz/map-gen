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
        private int[] FrequencyMap;
        private string[] ColorMap;


        public OverlapMapGenerator()
        {
        }

        public List<List<SquareTile>> Generate(SquareTileRule[] rules, int dimx, int dimy)
        {
            FrequencyMap = new int[rules.Length];
            ColorMap = new string[rules.Length];
            var map = new List<List<SquareTile>>(new List<SquareTile>[dimx]);
            ChangedTiles = new IntervalHeap<SquareTile>();
            Initialize(rules, dimx, dimy, map);
            var removals = new Stack<TileRemoval>();
            while (ChangedTiles.Count > 0)
            {
                var tile = ChangedTiles.DeleteMin();
                if (!tile.Collapsed)
                {
                    tile.Collapse(ColorMap, FrequencyMap, removals);
                    PropagateEnablers(map, rules, removals);
                }
            }
            return map;
        }

        private void PropagateEnablers(List<List<SquareTile>> map, SquareTileRule[] rules, Stack<TileRemoval> removals)
        {
            while (removals.Count > 0)
            {
                var removal = removals.Pop();
                var cell = map[removal.coordX][removal.coordY];
                for(int i = 0; i < 4; i++)
                {
                    var neighbor = cell.AdjacentTiles[i];
                    if (neighbor != null && !neighbor.Collapsed)
                    {
                        var changed = false;
                        foreach (var tileIdx in rules[removal.tileIdx].allowedTilesByDirection[i])
                        {
                            var enablerCount = neighbor.enablerCounts[tileIdx];
                            var oppositeDirection = OppositeDirection(i);
                            if (enablerCount.ByDirection[oppositeDirection] == 1)
                            {
                                bool hasZeroInAnyDirection = CheckZeroEnablers(enablerCount);
                                if (!hasZeroInAnyDirection)
                                {
                                    neighbor.RemovePossibleTile(tileIdx, removals, FrequencyMap);
                                    if (neighbor.LegalTilesCount == 1 && !neighbor.Collapsed)
                                    {
                                        neighbor.Collapsed = true;
                                        var selectedTileIdx = neighbor.LegalTiles.Select((item, idx) => (item, idx)).First(x => x.item).idx;
                                        neighbor.SelectedTile = ColorMap[selectedTileIdx];
                                    }
                                    else if (neighbor.LegalTilesCount == 0)
                                    {
                                        throw new Exception("Contradiction");
                                    }
                                    else
                                    {
                                        changed = true;
                                    }

                                }
                            }
                            else if(enablerCount.ByDirection[oppositeDirection] == 0)
                            {
                                continue;
                            }
                            
                            enablerCount.ByDirection[oppositeDirection]--;
                        }
                        if (changed && !neighbor.Collapsed)
                        {
                            neighbor.UpdateEntropy();
                            ChangedTiles.Add((SquareTile)neighbor);
                        }
                    }
                }
            }
        }

        private static bool CheckZeroEnablers(EnablerCount enablerCount)
        {
            for (int d = 0; d < enablerCount.ByDirection.Length; d++)
            {
                if (enablerCount.ByDirection[d] == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private int OppositeDirection(int i)
        {
            return (i + 2) % 4;
        }


/*        private void Propagate(Dictionary<int, SquareTileRule> rules, SquareTile tile)
        {
            var toUpdate = new Stack<Tile>(tile.AdjacentTiles.Where(x => x != null && !x.Collapsed));
            while (toUpdate.Count > 0)
            {
                tile = (SquareTile)toUpdate.Pop();
                var westTile = tile.AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.WEST);
                var northTile = tile.AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.NORTH);
                var eastTile = tile.AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.EAST);
                var southTile = tile.AdjacentTiles.ElementAtOrDefault((int)CardinalDirection.SOUTH);
                var removed = tile.LegalTiles.RemoveAll(x =>
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
        }*/

        private void Initialize(SquareTileRule[] rules, int dimx, int dimy, List<List<SquareTile>> map)
        {
            var initialEnablerCounts = new EnablerCount[rules.Length];
            foreach (var rule in rules)
            {
                FrequencyMap[rule.tileIdx] = rule.frequency;
                ColorMap[rule.tileIdx] = rule.name;

                var enablerCount = new EnablerCount(4);

                for(int i = 0; i < 4; i++)
                {
                    var count = rule.allowedTilesByDirection[i].Count;
                    enablerCount.ByDirection[i] = count;
                }
                initialEnablerCounts[rule.tileIdx] = enablerCount;
            }

            for (int x = 0; x < dimx; x++)
            {
                map[x] = new List<SquareTile>(new SquareTile[dimy]);
                for (int y = 0; y < dimy; y++)
                {
                    var tile = new SquareTile(rules.Length);
                    tile.coordX = x;
                    tile.coordY = y;
                    tile.enablerCounts = initialEnablerCounts.Select(x => {
                        var theCount = new EnablerCount(4);
                        x.ByDirection.CopyTo(theCount.ByDirection, 0);
                        return theCount;
                        }).ToArray();
                    for(int i = 0; i < rules.Length; i++)
                    {
                        tile.LegalTiles[i] = true;
                    }
                    tile.InitializeEntropy(FrequencyMap);
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
