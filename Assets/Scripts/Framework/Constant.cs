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
        Scene
    }

    public enum AssetsLoadMode
    {
        InEditor,
        PackageBundle,
        Update
    }
}