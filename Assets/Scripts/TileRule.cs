using System.Collections.Generic;

public class TileRule
{
    public TileRule() {
        if(geometry == MapGeometry.SQUARE)
        {
            allowedTilesByDirection = new List<List<int>>(new List<int>[4]);
            for(int i = 0; i < 4; i++)
            {
                allowedTilesByDirection[i] = new List<int>();
            }
        }
        else
        {
            allowedTilesByDirection = new List<List<int>>();
        }
    }
    public string name;
    public int tileIdx;
    public int frequency;
    public List<List<int>> allowedTilesByDirection;
    public MapGeometry geometry;
}