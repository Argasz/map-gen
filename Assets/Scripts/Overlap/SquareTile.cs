
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Overlap
{
    public class SquareTile : Tile
    {
        public SquareTile(int numberOfTiles)
        {
            Directions = 4;
            AdjacentTiles = new List<Tile>(new Tile[Directions]);
            Collapsed = false;
            EntropyNoise = Random.value / 10000f;
            enablerCounts = new EnablerCount[numberOfTiles];
        }

        public override void Collapse(Dictionary<int, string> colorMap)
        {
            Collapsed = true;
            var random = Random.Range(0, LegalTiles.Count);
            var tile = LegalTiles.ElementAt(random);
            LegalTiles.RemoveWhere(x => x != tile);
            SelectedTile = colorMap[tile];
        }

        public override void UpdateEntropy(Dictionary<int, int> frequencies)
        {
            var totalWeight = LegalTiles.Aggregate(0, (acc, x) => acc += frequencies[x]);
            var sumOfWeightLogWeight = LegalTiles.Aggregate(0f, (acc, x) => frequencies[x] * Mathf.Log(frequencies[x], 2));
            Entropy = Mathf.Log(totalWeight, 2) - (sumOfWeightLogWeight / totalWeight) + EntropyNoise;
        }
    }
}
