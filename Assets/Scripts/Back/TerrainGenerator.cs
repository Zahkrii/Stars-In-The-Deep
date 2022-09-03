using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("地形 Tile Atlas")]
    public TileAtlas tileAtlas;

    [Header("Terrain Generation Settings")]
    public int worldSize = 100;// 地图大小

    public int chunkSize = 20;//区块大小

    private GameObject[] worldChunks;//所有区块

    public int dirtLayerHeight = 14;// 土壤层高度

    public float terrainFreq = 0.04f;//地形频率

    public float heightMultiplier = 25f;//地形乘值

    public int heightAddition = 25;//地形加值

    public float seed;//种子

    [Header("Cave Generation Settings")]
    public bool generateCave = true;//是否生成洞穴

    public float surfaceValue = 0.27f;//值越大洞窟越大

    public float caveFreq = 0.08f;//洞窟频率

    public Texture2D caveNoiseTexture;//噪声图

    [Header("Ore Generation Settings")]
    public float coalRarity;//煤矿稀有度

    public float ironRarity;//铁矿稀有度
    public float goldRarity;//金矿稀有度
    public float copperRarity;//铜矿稀有度

    private void Start()
    {
        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture(caveNoiseTexture, caveFreq);
        GenerateChunks();
        GenerateTerrain();
    }

    /// <summary>
    /// 生成噪声图(柏林)
    /// </summary>
    /// <param name="noiseTexture">噪声图</param>
    /// <param name="frequency">噪声频率</param>
    private void GenerateNoiseTexture(Texture2D noiseTexture, float frequency)
    {
        noiseTexture = new Texture2D(worldSize, worldSize);
        // 生成噪声图
        for (float x = .0f; x < noiseTexture.width; x++)
        {
            for (float y = .0f; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise(seed + x / noiseTexture.width * frequency, seed + y / noiseTexture.height * frequency);
                noiseTexture.SetPixel((int)x, (int)y, new Color(v, v, v));
            }
        }
        noiseTexture.Apply();
    }

    private void GenerateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++)
        {
            GameObject chunk = new GameObject();
            chunk.name = name = string.Format("chunk[{0}]", i.ToString());
            chunk.transform.parent = this.transform;
            worldChunks[i] = chunk;
        }
    }

    /// <summary>
    /// 生成地形
    /// </summary>
    private void GenerateTerrain()
    {
        Sprite tileSprite;
        for (int x = 0; x < worldSize; x++)
        {
            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;
            for (int y = 0; y < height; y++)
            {
                if (y < height - dirtLayerHeight)
                {
                    tileSprite = tileAtlas.stoneTile.tileSprite;
                }
                else if (y < height - 1)
                {
                    tileSprite = tileAtlas.dirtTile.tileSprite;
                }
                else
                {
                    tileSprite = tileAtlas.grassTile.tileSprite;
                }

                if (generateCave)
                {
                    if (caveNoiseTexture.GetPixel(x, y).r > surfaceValue)
                        PlaceTile(tileSprite, x, y);
                }
                else
                    PlaceTile(tileSprite, x, y);
            }
        }
    }

    /// <summary>
    /// 放置Tile
    /// </summary>
    /// <param name="tileSprite">Tile Sprite</param>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    private void PlaceTile(Sprite tileSprite, int x, int y)
    {
        GameObject newTile = new GameObject();
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
        newTile.name = name = string.Format(tileSprite.name + "[{0},{1}]", x, y);
        //分划区块
        float chunkCoord = Mathf.Round(x / chunkSize) * chunkSize;
        chunkCoord /= chunkSize;
        newTile.transform.parent = worldChunks[(int)chunkCoord].transform;
    }
}