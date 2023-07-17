namespace Assets.Scripts.Overlap
{
    public struct TileRemoval
    {
        public int tileIdx;
        public int coordX;
        public int coordY;

        public TileRemoval(int tileIdx, int coordX, int coordY)
        {
            this.tileIdx = tileIdx;
            this.coordX = coordX;
            this.coordY = coordY;
        }
    }
}