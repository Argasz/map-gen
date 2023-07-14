using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class TiledMapLoader
{
    public static TiledMap LoadTiledMap(string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<TiledMap>(json);
    }
}
