using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class BundleInfo : SerializedScriptableObject
    {
        public string AssetName;
        public string BundleName;
        public List<string> Dependency;
    }
}