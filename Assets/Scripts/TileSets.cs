using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 创建资源创建菜单，文件名为fileName，菜单项名为menuName
[CreateAssetMenu(fileName = "NewTileSets", menuName = "Tile Sets")]
public class TileSets : ScriptableObject
{
    [Header("Regular")]
    public TileBase dirt;

    public TileBase stone;

    [Header("Background")]
    public TileBase dirtWall;
}