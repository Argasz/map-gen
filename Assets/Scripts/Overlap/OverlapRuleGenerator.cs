using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Overlap
{
    public static class OverlapRuleGenerator
    {
        public static Dictionary<int, SquareTileRule> Generate(List<List<SampleTile>> sampleMap, int dimx, int dimy, bool includeRotations = false, bool includeReflections = false)
        {
            var overlapTileMap = GenerateOverlapTileMap(sampleMap, dimx, dimy, includeRotations, includeReflections);
            return GenerateAdjacencyRules(overlapTileMap);
        }

        private static List<OverlapTile> GenerateOverlapTileMap(List<List<SampleTile>> sampleMap, int dimx, int dimy, bool includeRotations = false, bool includeReflections = false)
        {
            var overlapTileMap = new Dictionary<string, OverlapTile>();
            for (int x = 0; x < sampleMap.Count; x++)
            {
                for (int y = 0; y < sampleMap[0].Count; y++)
                {
                    var tile = new OverlapTile(dimx, dimy);
                    for (int i = 0; i < dimx; i++)
                    {
                        for (int k = 0; k < dimy; k++)
                        {
                            var xIdx = (x + i) % sampleMap.Count; // Wraparound
                            var yIdx = (y + k) % sampleMap[0].Count;
                            tile.TileGrid[i][k] = sampleMap[xIdx][yIdx].PrefabName;
                        }
                    }
                    UpdateTileMap(overlapTileMap, tile);
                    if (includeRotations)
                    {
                        AddRotations(tile, overlapTileMap, includeReflections);
                    }

                }
            }
            return overlapTileMap.Values.ToList();
        }

        private static void AddRotations(OverlapTile tile, Dictionary<string, OverlapTile> overlapTileMap, bool includeReflections)
        {
            var prevTile = new OverlapTile(tile);
            
            for(int i = 0; i < 3; i++)
            {
                OverlapTile rotated = RotateCCW(prevTile);
                UpdateTileMap(overlapTileMap, rotated);
                prevTile = rotated;
                if (includeReflections)
                {
                    var reflected = ReflectY(rotated);
                    UpdateTileMap(overlapTileMap, reflected);
                }
            }
        }

        private static OverlapTile RotateCCW(OverlapTile tile)
        {
            var rotated = new OverlapTile(tile.dimx, tile.dimy);
            for (int x = 0; x < tile.dimx; x++)
            {
                for (int y = 0; y < tile.dimy; y++)
                {
                    rotated.TileGrid[x][y] = tile.TileGrid[tile.dimy - y - 1][x];
                }
            }

            return rotated;
        }

        private static OverlapTile ReflectY(OverlapTile tile)
        {
            var reflected = new OverlapTile(tile.dimx, tile.dimy);
            for(int x = 0; x < tile.dimx; x++)
            {
                for(int y = 0; y < tile.dimy; y++)
                {
                    reflected.TileGrid[x][tile.dimy - y - 1] = tile.TileGrid[x][y];
                }
            }
            return reflected;
        }

        private static void UpdateTileMap(Dictionary<string, OverlapTile> overlapTileMap, OverlapTile tile)
        {
            tile.GenerateHashString();
            OverlapTile existingTile;
            if (!overlapTileMap.TryGetValue(tile.HashString, out existingTile))
            {
                tile.frequency = 1;
                overlapTileMap[tile.HashString] = tile;
            }
            else
            {
                existingTile.frequency++;
            }
        }

        private static Dictionary<int, SquareTileRule> GenerateAdjacencyRules(List<OverlapTile> overlapTileMap)
        {
            var rules = new Dictionary<int, SquareTileRule>();

            for (int i = 0; i < overlapTileMap.Count; i++)
            {
                var tile = overlapTileMap[i];
                for (int k = 0; k < overlapTileMap.Count; k++)
                {
                    var otherTile = overlapTileMap[k];
                    var tileName = tile.TileGrid[0][0];
                    SquareTileRule rule;
                    if (!rules.TryGetValue(i, out rule))
                    {
                        rule = new SquareTileRule();
                        rule.name = tileName;
                        rule.frequency = tile.frequency;
                        rule.tileIdx = i;
                        rules.Add(i, rule);
                    }

                    if (Compatible(tile, otherTile, CardinalDirection.WEST))
                    {
                        rule.WestPermitted.Add(k);
                    }
                    if (Compatible(tile, otherTile, CardinalDirection.NORTH))
                    {
                        rule.NorthPermitted.Add(k);
                    }
                    if (Compatible(tile, otherTile, CardinalDirection.EAST))
                    {
                        rule.EastPermitted.Add(k);
                    }
                    if (Compatible(tile, otherTile, CardinalDirection.SOUTH))
                    {
                        rule.SouthPermitted.Add(k);
                    }
                }
            }
            return rules;
        }

        private static bool Compatible(OverlapTile a, OverlapTile b, CardinalDirection direction)
        {
            return direction switch
            {
                CardinalDirection.WEST => WestOverlap(a, b),
                CardinalDirection.NORTH => NorthOverlap(a, b),
                CardinalDirection.EAST => EastOverlap(a, b),
                CardinalDirection.SOUTH => SouthOverlap(a, b),
                _ => false,
            };
        }

        private static bool SouthOverlap(OverlapTile a, OverlapTile b)
        {
            for (int x = 0; x < a.dimx; x++)
            {
                for (int y = 1; y < a.dimy; y++)
                {
                    if (a.TileGrid[x][y] != b.TileGrid[x][y - 1])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool EastOverlap(OverlapTile a, OverlapTile b)
        {
            for (int x = 1; x < a.dimx; x++)
            {
                for (int y = 0; y < a.dimy; y++)
                {
                    if (a.TileGrid[x][y] != b.TileGrid[x - 1][y])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool NorthOverlap(OverlapTile a, OverlapTile b)
        {
            return SouthOverlap(b, a);
        }

        private static bool WestOverlap(OverlapTile a, OverlapTile b)
        {

            return EastOverlap(b, a);
        }
    }
}
