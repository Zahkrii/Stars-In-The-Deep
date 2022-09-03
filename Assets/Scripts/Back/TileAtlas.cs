using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 创建资源创建菜单，文件名为fileName，菜单项名为menuName
[CreateAssetMenu(fileName = "NewTileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    public TileClass stoneTile;

    public TileClass dirtTile;

    public TileClass grassTile;
}