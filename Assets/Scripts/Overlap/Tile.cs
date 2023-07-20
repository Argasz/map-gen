using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Overlap
{
    public abstract class Tile : IComparable<Tile>
    {
        public bool Collapsed;
        public int Directions;
        public float Entropy;
        public float EntropyNoise;
        public bool[] LegalTiles;
        public List<Tile> AdjacentTiles;
        public EnablerCount[] enablerCounts;
        public string SelectedTile;
        public int coordX;
        public int coordY;
        public int LegalTilesCount;
        public int sumOfPossibleTileWeights;
        public float sumOfPossibleTileWeightsLog;

        public abstract void UpdateEntropy();
        public abstract void Collapse(string[] colorMap, int[] frequencyMap, Stack<TileRemoval> removals);
        public abstract void RemovePossibleTile(int tileIdx, Stack<TileRemoval> removals, int[] frequencies);
        public abstract void InitializeEntropy(float initialEntropy);

        public int CompareTo(Tile other)
        {
            return Entropy.CompareTo(other.Entropy);
        }
    }
}
