using System.Collections;
using UnityEngine;

// 创建资源创建菜单，文件名为fileName，菜单项名为menuName
[CreateAssetMenu(fileName = "NewTileClass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    //定义每个Tile

    // Tile名称
    public string tileName;

    // Tile Sprite
    public Sprite tileSprite;
}