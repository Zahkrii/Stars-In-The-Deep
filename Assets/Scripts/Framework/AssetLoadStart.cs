using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoadStart : MonoBehaviour
{
    public AssetsLoadMode loadMode;

    private void Awake()
    {
        Constant.AssetsLoadMode = loadMode;
        DontDestroyOnLoad(this);
    }
}