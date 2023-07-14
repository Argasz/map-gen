using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TiledMap
{
    public List<Layer> layers;
    public List<TiledMapTileSet> tileSets;
}
