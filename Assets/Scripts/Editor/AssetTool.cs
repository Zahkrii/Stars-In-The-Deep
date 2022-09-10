using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Framework.Utils;

public class AssetTool
{
    [MenuItem("Assets/Create/New Lua Script", false, 1)]
    public static void CreateLuaScript()
    {
        string text = "function func()\n    print(\"lua\")\nend";
        var tmp = Selection.GetFiltered<Object>(SelectionMode.Assets);
        foreach (var obj in tmp)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
                continue;
            if (System.IO.Directory.Exists(path))
            {
                if (File.Exists(path + "/NewLuaScript.bytes"))
                    return;
                File.WriteAllText(path + "/NewLuaScript.bytes", text);
                AssetDatabase.Refresh();
                return;
            }
            else if (System.IO.File.Exists(path))
            {
                if (File.Exists(Path.GetDirectoryName(path) + "/NewLuaScript.bytes"))
                    return;
                File.WriteAllText(System.IO.Path.GetDirectoryName(path) + "/NewLuaScript.bytes", text);
                AssetDatabase.Refresh();
                return;
            }
        }
        if (File.Exists("Assets/NewLuaScript.bytes"))
            return;
        File.WriteAllText("Assets/NewLuaScript.bytes", text);
        AssetDatabase.Refresh();
        return;
    }
}