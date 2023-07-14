using System;
using System.Collections.Generic;
using System.Linq;

public static class RuleGenerator
{
    public static List<SquareTileRule> GenerateRules(List<List<SampleTile>> sampleMap)
    {
        var rules = new Dictionary<string, SquareTileRule>();

        for(int x = 0; x < sampleMap.Count; x++)
        {
            for(int y = 0; y < sampleMap[0].Count; y++)
            {
                SquareTileRule rule;
                if(!rules.TryGetValue(sampleMap[x][y].PrefabName, out rule))
                {
                    rule = new SquareTileRule();
                    var name = sampleMap[x][y].PrefabName;
                    rule.name = name;
                    rules.Add(name, rule);
                }
                if (x > 0)
                {
                    var west = sampleMap[x - 1][y];
                    rule.WestPermitted.Add(0);
                }
                if (y > 0)
                {
                    var north = sampleMap[x][y - 1];
                    rule.NorthPermitted.Add(0);
                }
                if (x < sampleMap.Count - 1)
                {
                    var east = sampleMap[x + 1][y];
                    rule.EastPermitted.Add(0);
                }
                if (y < sampleMap[0].Count - 1)
                {
                    var south = sampleMap[x][y + 1];
                    rule.SouthPermitted.Add(0);
                }
                rule.frequency++;
            }
        }

        return rules.Values.ToList();
    }
}

