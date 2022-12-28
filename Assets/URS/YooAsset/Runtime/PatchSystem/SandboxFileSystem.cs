using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using YooAsset.Utility;
using URS;
using MHLab.Patch.Core.Utilities;

namespace YooAsset
{
    public static class SandboxFileSystem
    {
        // private static  Dictionary<string,FileMeta> _cachedFileMap = new Dictionary<string, FileMeta>(1000);

        private static FileManifest _sandboxFileManifest = null;
        private static BundleManifest _sandboxBundleManifest = null;

        public static FileManifest GetFileManifest()
        {
            return _sandboxFileManifest;
        }

        public static Dictionary<string, FileMeta> GetFileMap()
        {
            if (_sandboxFileManifest != null)
            {
                return _sandboxFileManifest.GetFileMetaMap();
            }
            return null;
        }
        public static BundleManifest GetBundleManifest()
        {
            return _sandboxBundleManifest;
        }
        public static bool TryGetHardiskFilePath(string relativePath, out string hardiskPath, out FileMeta fileMeta)
        {
            hardiskPath = null;
            fileMeta = null;
            var filemap = GetFileMap();
            if (filemap == null)
            {
                return false;
            }
            if (filemap.ContainsKey(relativePath))
            {
                var fm = filemap[relativePath];
                string filePath = MakeSandboxFilePath(relativePath);
                if (File.Exists(filePath))
                {
                    hardiskPath = filePath;
                    fileMeta = fm;
                    return true;
                }
                else
                {
                    _sandboxFileManifest.RemoveFile(relativePath);
                    Logger.Error($"Cache file is missing : {fm.RelativePath} Hash : {fm.Hash}");
                }
            }
            return false;
        }
        /// <summary>
        /// ��ѯ�Ƿ�Ϊ��֤�ļ�
        /// ע�⣺����¼���ļ��������Ǿ�����Ч��
        /// </summary>
        public static bool ContainsFile(string relativePath)
        {
            var filemap = GetFileMap();
            if (filemap == null)
            {
                return false;
            }
            if (filemap.ContainsKey(relativePath))
            {
                string filePath = MakeSandboxFilePath(relativePath);
                if (File.Exists(filePath))
                {
                    return true;
                }
                else
                {
                    var fileMeta = filemap[relativePath];
                    _sandboxFileManifest.RemoveFile(relativePath);
                    Logger.Error($"Cache file is missing : {fileMeta.RelativePath} Hash : {fileMeta.Hash}");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool ContainsFile(FileMeta fileMeta)
        {
            return ContainsFile(fileMeta.RelativePath);
        }
        /// <summary>
        /// ������֤�����ļ�
        /// </summary>
        public static void RegisterVerifyFile(FileMeta fileMeta)
        {
            if (_sandboxFileManifest == null)
            {
                _sandboxFileManifest = new FileManifest(null);
            }
            _sandboxFileManifest.ReplaceFile(fileMeta);

        }
        public static void GetFileMetaByTag(string[] tags, ref List<FileMeta> result)
        {
            _sandboxFileManifest.GetFileMetaByTag(tags, ref result);
        }
        public static void FlushSandboxFileManifestToHardisk()
        {
            if (_sandboxFileManifest != null)
            {
                var fileMap = _sandboxFileManifest.GetFileMetaMap();
                if (fileMap != null)
                {
                    _sandboxFileManifest.FileMetas = new List<FileMeta>(fileMap.Values).ToArray();
                    var savePath = MakeSandboxFilePath(URSRuntimeSetting.instance.FileManifestFileName);
                    FileManifest.Serialize(savePath, _sandboxFileManifest, true);
                }
            }
        }

        // ��֤�ļ�������
        public static bool CheckContentIntegrity(FileMeta fileMeta, string hardiskFilePath)
        {
            return CheckContentIntegrity(hardiskFilePath, fileMeta.SizeBytes, fileMeta.Hash);
        }
        public static bool CheckContentIntegrity(FileMeta fileMeta)
        {
            string filePath = MakeSandboxFilePath(fileMeta.RelativePath);
            return CheckContentIntegrity(filePath, fileMeta.SizeBytes, fileMeta.Hash);
        }
        public static bool CheckContentIntegrity(string filePath, long size, uint fileHash)
        {
            if (File.Exists(filePath) == false)
                return false;

            // ����֤�ļ���С
            long fileSize = FileUtility.GetFileSize(filePath);
            if (fileSize != size)
                return false;

            // ����֤�ļ�CRC
            var currentFileHash = Hashing.GetFileXXhash(filePath);
            return fileHash == currentFileHash;
        }
        private static string sSandboxDirectoryName = null;

        public static string GetSandboxDirectory()
        {
            if (sSandboxDirectoryName == null)
            {
                var forceSandboxDirectory = URSRuntimeSetting.instance.ForceSandboxDirectory;
                if (string.IsNullOrEmpty(forceSandboxDirectory))
                {
                    sSandboxDirectoryName = $"{AssetPathHelper.GetPersistentRootPath()}/sandbox";
                }
                else
                {
                    sSandboxDirectoryName = forceSandboxDirectory;
                }
            }
            return sSandboxDirectoryName;
        }

        private static string _sanboxPatchDirectory = null;
        public static string GetSanboxPatchDirectory() 
        {

            if (_sanboxPatchDirectory == null)
            {
                _sanboxPatchDirectory = $"{AssetPathHelper.GetPersistentRootPath()}/sandbox_patch";
            }
            return _sanboxPatchDirectory;
        }

        /// <summary>
        /// ��ȡ�����ļ��Ĵ洢·��
        /// </summary>
        public static string MakeSandboxPatchFilePath(string fileName)
        {
            return $"{GetSanboxPatchDirectory()}/{fileName}";
        }
        /// <summary>
        /// ɾ��ɳ���ڵĻ����ļ���
        /// </summary>
        public static void DeleteSandboxFolder()
        {
            string directoryPath = GetSandboxDirectory();
            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, true);
        }

        /// <summary>
        /// ɾ��ɳ���ڵĻ����ļ���
        /// </summary>
        public static void DeleteSandboxFile(string fileRelativePath)
        {
            string filePath = MakeSandboxFilePath(fileRelativePath);
            if (File.Exists(filePath))
                File.Delete(filePath);
            _sandboxFileManifest.RemoveFile(fileRelativePath);
        }
        /// <summary>
        /// ���ɳ���ڲ����嵥�ļ��Ƿ����
        /// </summary>
        public static bool CheckSandboxPatchManifestFileExist()
        {
            string filePath = MakeSandboxFilePath(URSRuntimeSetting.instance.FileManifestFileName);
            return File.Exists(filePath);
        }


        public static void InitSandboxFileAndBundle()
        {
            string filePath = MakeSandboxFilePath(URSRuntimeSetting.instance.FileManifestFileName);
            if (File.Exists(filePath))
            {
                Logger.Log($"Load sandbox FileManifest manifest." + filePath);
                string jsonData = File.ReadAllText(filePath);
                _sandboxFileManifest = FileManifest.Deserialize(jsonData);
                //  _cachedFileMap = sInitSandboxFileManifest.GetFileMetaMap();
            }
            filePath = MakeSandboxFilePath(URSRuntimeSetting.instance.BundleManifestFileName);
            if (File.Exists(filePath))
            {
                Logger.Log($"Load sandbox BundleManifest manifest." + filePath);
                string jsonData = File.ReadAllText(filePath);
                _sandboxBundleManifest = BundleManifest.Deserialize(jsonData);
            }
        }

        /// <summary>
        /// ���ɳ���ڲ����嵥�ļ��Ƿ����
        /// </summary>
        public static bool CheckSandboxBundleManifestFileExist()
        {
            string filePath = MakeSandboxFilePath(URSRuntimeSetting.instance.BundleManifestFileName);
            return File.Exists(filePath);
        }
        /// <summary>
        /// ��ȡɳ���ڲ����嵥�ļ��Ĺ�ϣֵ
        /// ע�⣺���ɳ���ڲ����嵥�ļ������ڣ����ؿ��ַ���
        /// </summary>
        /// <returns></returns>
        public static string GetSandboxPatchManifestFileHash()
        {
            string filePath = MakeSandboxFilePath(URSRuntimeSetting.instance.FileManifestFileName);
            if (File.Exists(filePath))
                return HashUtility.FileMD5(filePath);
            else
                return string.Empty;
        }

        /// <summary>
        /// ��ȡ�����ļ��Ĵ洢·��
        /// </summary>
        public static string MakeSandboxFilePath(string fileName)
        {
            return $"{GetSandboxDirectory()}/{fileName}";
        }
    }
}

