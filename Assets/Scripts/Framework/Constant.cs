using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class Constant
    {
        public const string BundleExtension = ".bundle";

        public const string FileListName = "manifest.bin";

        // 热更资源地址
        public const string ResourceURL = "http://127.0.0.1/AssetBundles";

        public static AssetsLoadMode AssetsLoadMode = AssetsLoadMode.InEditor;
    }

    public enum AssetType
    {
        UI,
        Lua,
        Music,
        Sound,
        Effect,
        Sprite,
        Scene,
        Prefab,
        Model
    }

    public enum AssetsLoadMode
    {
        InEditor,
        PackageBundle,
        Hotfix
    }
}