using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace Framework.Utils
{
    public class FileUtil
    {
        /// <summary>
        /// 检测文件是否存在
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件是否存在</returns>
        public static bool IsExists(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Exists;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path">写入路径</param>
        /// <param name="data">文件数据</param>
        public static void WriteFile(string path, byte[] data)
        {
            path = PathUtil.FormatPathToStandard(path);
            string dir = path.Substring(0, path.LastIndexOf('/'));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
                fileInfo.Delete();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                }
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}