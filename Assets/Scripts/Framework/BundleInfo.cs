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
        public string AssetName;
        public string BundleName;
        public List<string> Dependency;

        public BundleInfo(string assetName, string bundleName, List<string> dependency)
        {
            AssetName = assetName;
            BundleName = bundleName;
            Dependency = dependency;
        }
    }
}