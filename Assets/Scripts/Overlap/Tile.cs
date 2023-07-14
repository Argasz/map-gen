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
        public HashSet<int> LegalTiles;
        public List<Tile> AdjacentTiles;
        public EnablerCount[] enablerCounts;
        public string SelectedTile;

        public abstract void UpdateEntropy(Dictionary<int, int> frequencies);
        public abstract void Collapse(Dictionary<int, string> colorMap);

        public int CompareTo(Tile other)
        {
            return Entropy.CompareTo(other.Entropy);
        }
    }
}
