
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

        public override void Collapse(string[] colorMap, Stack<TileRemoval> removals)
        {
            Collapsed = true;
            var remainingLegal = LegalTiles.Where(x => x).Select((item, index) => index).ToList();
            var random = Random.Range(0, remainingLegal.Count);
            var tile = remainingLegal.ElementAt(random);
            for(int i = remainingLegal.Count - 1; i >= 0; i--)
            {
                if(remainingLegal[i] != tile)
                {
                    removals.Push(new TileRemoval(i, coordX, coordY));
                    LegalTiles[i] = false;
                }
            }
            SelectedTile = colorMap[tile];
        }

        public override void UpdateEntropy(int[] frequencies)
        {
            var totalWeight = LegalTiles.Aggregate(0, (acc, x) => acc += frequencies[x]);
            var sumOfWeightLogWeight = LegalTiles.Aggregate(0f, (acc, x) => frequencies[x] * Mathf.Log(frequencies[x], 2));
            Entropy = Mathf.Log(totalWeight, 2) - (sumOfWeightLogWeight / totalWeight) + EntropyNoise;
        }
    }
}
