using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    [Serializable]
    public class BundleInfo
    {
        //资源名
        public string AssetName;

        //包名
        public string BundleName;

        //依赖文件列表
        public List<string> Dependency;

        public BundleInfo(string assetName, string bundleName, List<string> dependency)
        {
            AssetName = assetName;
            BundleName = bundleName;
            Dependency = dependency;
        }
    }
}