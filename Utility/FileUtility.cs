using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ZHSM
{
    public class FileUtility
    {
        public static void CopyAllFiles(string sourceDirectory, string destinationDirectory, params string[] ignore)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    throw new Exception(sourceDirectory + "\n源文件夹不存在。");
                }

                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                var filesToCopy = Directory.GetFiles(sourceDirectory)
                    .Where((file) =>
                        {
                            var isOk = true;
                            var fileName = Path.GetFileName(file);
                            if (ignore != null && ignore.Length > 0)
                            {
                                for (var i = 0; i < ignore.Length; i++)
                                {
                                    var end = ignore[i];
                                    if (fileName.EndsWith(end, StringComparison.OrdinalIgnoreCase))
                                    {
                                        isOk = false;
                                        break;
                                    }
                                }
                            }

                            return isOk;
                        }


                    );


                foreach (string file in filesToCopy)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationPath = Path.Combine(destinationDirectory, fileName);
                    File.Copy(file, destinationPath, true); // 第三个参数表示如果文件已存在，是否覆盖
                }

                string[] subdirectories = Directory.GetDirectories(sourceDirectory);

                foreach (string subdirectory in subdirectories)
                {
                    string subdirectoryName = Path.GetFileName(subdirectory);
                    string newDestinationDirectory = Path.Combine(destinationDirectory, subdirectoryName);
                    CopyAllFiles(subdirectory, newDestinationDirectory);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public static void ClearFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            string[] files = Directory.GetFiles(folderPath);
            string[] subfolders = Directory.GetDirectories(folderPath);
            
            // 删除文件
            foreach (string file in files)
            {
                File.Delete(file);
            }

            // 递归删除子文件夹中的文件
            foreach (string subfolder in subfolders)
            {
                ClearFolder(subfolder);
            }
        }
    }
}