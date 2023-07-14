
using System.Text;

public class OverlapTile
{
    public int dimx;
    public int dimy;
    public string[][] TileGrid;
    public string HashString;
    public int frequency;

    public OverlapTile(OverlapTile other)
    {
        this.dimx = other.dimx;
        this.dimy = other.dimy;
        TileGrid = new string[dimx][];
        for (int i = 0; i < TileGrid.Length; i++)
        {
            TileGrid[i] = new string[dimy];
            other.TileGrid[i].CopyTo(TileGrid[i], 0);
        }
    }

    public OverlapTile(int dimx, int dimy)
    {
        this.dimx = dimx;
        this.dimy = dimy;
        TileGrid = new string[dimx][];
        for(int i = 0; i < TileGrid.Length; i++)
        {
            TileGrid[i] = new string[dimy];
        }
    }

    public void GenerateHashString()
    {
        var stringBuilder = new StringBuilder();
        for (int x = 0; x < TileGrid.Length; x++)
        {
            for (int y = 0; y < TileGrid[0].Length; y++)
            {
                stringBuilder.Append(TileGrid[x][y]);
            }

        }
        HashString = stringBuilder.ToString();
    }

    public override bool Equals(object obj)
    {
        if(obj == null)
        {
            return false;
        }

        if(!(obj is OverlapTile))
        {
            return false;
        }

        return ((OverlapTile)obj).HashString == this.HashString;
    }

    public override int GetHashCode()
    {
        return HashString.GetHashCode();
    }
}

