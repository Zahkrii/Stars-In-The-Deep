using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    [Header("Terrain Gen")]
    [SerializeField] private int width;

    [SerializeField] private int height;//世界尺寸
    [SerializeField] private float smoothness;//地形平滑度
    [SerializeField] private float seed;//随机种子

    [Header("Cave Gen")]
    [Range(0f, 1f)]
    [SerializeField] private float modifier;//修饰

    [Header("Tile")]
    [SerializeField] private TileBase tileSets;//Tile集

    [SerializeField] private Tilemap worldMap;//世界地图
    [SerializeField] private Tilemap caveMap;//世界地图
    private int[,] zeroOneMap;

    // Start is called before the first frame update
    private void Start()
    {
        zeroOneMap = GenerateArray(width, height, true);
        zeroOneMap = TerrainGeneration(zeroOneMap);
        RenderMap(zeroOneMap, worldMap, tileSets);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private int[,] GenerateArray(int width, int height, bool isEmpty)
    {
        // 1代表生成方块，0代表不生成
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++) // this will go through the width of the map
            for (int y = 0; y < height; y++) // this will go through the height of the map
                map[x, y] = (isEmpty) ? 0 : 1; // isEmpty == true，则赋值0，否则赋值1
        return map;
    }

    public int[,] TerrainGeneration(int[,] map)
    {
        int perlinHeight;
        for (int x = 0; x < width; x++)//this wi1l go through the width of the map
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * height / 2);
            perlinHeight += height / 2;
            for (int y = 0; y < perlinHeight; y++)// this will go through the perlinheight of the map
            {
                // 2表示生成洞穴
                int caveValue = Mathf.RoundToInt(Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed));//得到0或1
                map[x, y] = (caveValue == 1) ? 2 : 1;
            }
        }
        return map;
    }

    private void RenderMap(int[,] map, Tilemap worldMap, TileBase tileSets)
    {
        for (int x = 0; x < width; x++) // this will go through the width of the map
            for (int y = 0; y < height; y++) // this will go through the height of the map
                if (map[x, y] == 1)
                    worldMap.SetTile(new Vector3Int(x, y, 0), tileSets);
    }
}