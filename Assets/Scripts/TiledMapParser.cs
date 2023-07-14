using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TiledMapParser
{

    public static List<List<SampleTile>> LoadMapSample(string sampleResourceName, string tilesetResourceName)
    {
        return parseSquareMap(sampleResourceName, tilesetResourceName);
    }

    private static List<List<SampleTile>> parseSquareMap(string sampleResourceName, string tilesetResourceName)
    {
        var prefabNameMap = PopulatePrefabNameMap(tilesetResourceName);

        var mapData = Resources.Load<TextAsset>("Maps\\" + sampleResourceName);
        var map = JsonUtility.FromJson<TiledMap>(mapData.text);
        var layer = map.layers[0];
        var tiledMap = new List<List<SampleTile>>(layer.width);

        for (int i = 0; i < layer.width; i++)
        {
            tiledMap.Add(new List<SampleTile>(new SampleTile[layer.height]));
        }

        for (int i = 0; i < layer.data.Count; i++)
        {
            var x = i % layer.width;
            var y = i / layer.width;
            /*
             * The constant subtracted here will need to be dynamically read 
             * from the firstGid property in the tilesets in the map json if we use multiple tilesets for one map.
             */
            tiledMap[x][y] = new SampleTile(prefabNameMap[layer.data[i] - 1]); 

        }
        return tiledMap;
    }

    private static Dictionary<int, string> PopulatePrefabNameMap(string tilesetResourceName)
    {
        var tileSetData = Resources.Load<TextAsset>("Maps\\" + tilesetResourceName);
        var tileSet = JsonUtility.FromJson<TileSet>(tileSetData.text);
        var prefabNameMap = new Dictionary<int, string>();

        foreach (var tile in tileSet.tiles)
        {
            var nameProp = tile.properties.Find(x => x.name == "prefabName");
            prefabNameMap[tile.id] = nameProp.value;
        }

        return prefabNameMap;
    }
}
