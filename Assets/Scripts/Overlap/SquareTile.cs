
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
            LegalTilesCount = numberOfTiles;
            LegalTiles = new bool[numberOfTiles];
        }

        public override void Collapse(string[] colorMap, int[] frequencyMap, Stack<TileRemoval> removals)
        {
            Collapsed = true;
            var remainingWeight = Random.Range(0, sumOfPossibleTileWeights);
            int selectedTile = -1;

            for(int i = 0; i < LegalTiles.Length; i++)
            {
                if(LegalTiles[i] == true)
                {
                    if (selectedTile == -1)
                    {
                        var weight = frequencyMap[i];
                        if (remainingWeight >= weight)
                        {
                            remainingWeight -= weight;
                        }
                        else
                        {
                            selectedTile = i;
                            continue; // Skip creating removal for selected tile
                        }
                    }
                    removals.Push(new TileRemoval(i, coordX, coordY));
                    LegalTiles[i] = false;
                }
            }
            LegalTilesCount = 1;
            SelectedTile = colorMap[selectedTile];
        }

        public override void InitializeEntropy(float initialEntropy)
        {
            Entropy = initialEntropy + EntropyNoise;
        }

        public override void RemovePossibleTile(int tileIdx, Stack<TileRemoval> removals, int[] frequencies)
        {
            LegalTiles[tileIdx] = false;
            removals.Push(new TileRemoval(tileIdx, coordX, coordY));
            LegalTilesCount--;

            var frequency = frequencies[tileIdx];
            sumOfPossibleTileWeights -= frequency;
            sumOfPossibleTileWeightsLog -= frequency * Mathf.Log(frequency, 2);
        }


        public override void UpdateEntropy()
        {
            Entropy = Mathf.Log(sumOfPossibleTileWeights, 2) - (sumOfPossibleTileWeightsLog / sumOfPossibleTileWeights);
        }
    }
}
