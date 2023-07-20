using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets.Scripts.Overlap;
using UnityEngine.Profiling;
using System.Diagnostics;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 30;
    public int rows = 30;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] prefabsList;
    public Dictionary<string, GameObject> prefabs;
    public string SampleName;
    public string TilesetName;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList()
    {
        prefabs = new Dictionary<string, GameObject>();
        foreach(var obj in prefabsList)
        {
            if (obj.name != null)
            {
                prefabs.Add(obj.name, obj);
            }
        }
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for(int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0));
            }
        }
    }

    void BoardSetup ()
    {
        boardHolder = new GameObject("Board").transform;

        var retries = 10;
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var tiledMap = TiledMapParser.LoadMapSample(SampleName, TilesetName);
        UnityEngine.Debug.Log("Load sample: " + stopwatch.ElapsedMilliseconds);
        var rules = OverlapRuleGenerator.Generate(tiledMap, 3, 3, true, true);
        UnityEngine.Debug.Log("Generate rules: " + stopwatch.ElapsedMilliseconds);
        while (retries > 0)
        {
            try
            {
                var generator = new OverlapMapGenerator();
                var map = generator.Generate(rules, columns, rows);
                UnityEngine.Debug.Log("Generate map: " + stopwatch.ElapsedMilliseconds);
                for (int x = 0; x < columns; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        var tile = map[x][y];
                        if (tile.SelectedTile != null)
                        {
                            var obj = prefabs[tile.SelectedTile];
                            var inverseY = rows - y;
                            GameObject instance = Instantiate(obj, new Vector3(x, inverseY, 0), Quaternion.identity);

                            instance.transform.SetParent(boardHolder);
                        }


                    }
                }
                UnityEngine.Debug.Log("Tries:" + (10 - retries));
                break;
            }
            catch (Exception e)
            {
                retries--;
                if (retries == 0)
                {
                    throw e;
                }
            }

        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        InitializeList();
        BoardSetup();
/*        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);*/
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0), Quaternion.identity);
    }
}
