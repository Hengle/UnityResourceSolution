using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using YooAsset.Utility;
using System.IO;

namespace URS
{
    [Serializable]
    public class AssetMeta
    {
        /// <summary>
        /// ��Դ·��
        /// </summary>
        public string AssetPath;

        /// <summary>
        /// ������Դ��ID
        /// </summary>
        public int BundleID;

        /// <summary>
        /// ��������Դ��ID�б�
        /// </summary>
        public int[] DependIDs;
    }

    /// <summary>
    /// Bundle����
    /// </summary>
    [Serializable]
    public class BundleManifest
    {
        /// <summary>
        /// ��Դ�б������ռ�����Դ�б�
        /// </summary>
        public AssetMeta[] AssetList;

        /// <summary>
        /// ��Դ���б�
        /// </summary>
        public FileMeta[] BundleList;

        /// <summary>
        /// ��Դ�����ϣ��ṩBundleName��ȡPatchBundle��
        /// </summary>
        [NonSerialized]
        private  Dictionary<string, FileMeta> BundleMap = new Dictionary<string, FileMeta>();

        /// <summary>
        /// ��Դӳ�伯�ϣ��ṩAssetPath��ȡPatchAsset��
        /// </summary>
        [NonSerialized]
        private  Dictionary<string, AssetMeta> AssetMap = new Dictionary<string, AssetMeta>();


        /// <summary>
        /// ��ȡ��Դ�����б�
        /// </summary>
        public List<FileMeta> GetAllDependenciesRelativePath(string assetPath)
        {
            if (AssetMap.TryGetValue(assetPath, out AssetMeta patchAsset))
            {
                List<FileMeta> result = new List<FileMeta>(patchAsset.DependIDs.Length); // TODO:�Ż�gc
                foreach (var dependID in patchAsset.DependIDs)
                {
                    if (dependID >= 0 && dependID < BundleList.Length)
                    {
                        var dependPatchBundle = BundleList[dependID];
                        result.Add(dependPatchBundle);
                    }
                    else
                    {
                        throw new Exception($"Invalid depend id : {dependID} Asset path : {assetPath}");
                    }
                }
                return result;
            }
            else
            {
                Debug.LogWarning($"Not found asset path in patch manifest : {assetPath}");
                return null;
            }
        }

        /// <summary>
        /// ��ȡ��Դ������
        /// </summary>
        public FileMeta GetBundleFileMeta(string assetPath)
        {
            if (AssetMap.TryGetValue(assetPath, out AssetMeta patchAsset))
            {
                int bundleID = patchAsset.BundleID;
                if (bundleID >= 0 && bundleID < BundleList.Length)
                {
                    var patchBundle = BundleList[bundleID];
                    return patchBundle;
                }
                else
                {
                    throw new Exception($"Invalid depend id : {bundleID} Asset path : {assetPath}");
                }
            }
            else
            {
                Debug.LogWarning($"Not found asset path in patch manifest : {assetPath}");
                return FileMeta.ERROR_FILE_META;
            }
        }

        public void AfterDeserialize()
        {
            if (BundleList != null && BundleList.Length > 0)
            {
                foreach (var bundle in BundleList)
                {
                    if (!BundleMap.ContainsKey(bundle.RelativePath))
                    {
                        BundleMap.Add(bundle.RelativePath, bundle);
                    }
                }
            }

            if (AssetList != null && AssetList.Length > 0)
            {
                foreach (var asset in AssetList)
                {
                    if (!AssetMap.ContainsKey(asset.AssetPath))
                    {
                        AssetMap.Add(asset.AssetPath, asset);
                    }
                }
            }
        }
        /// <summary>
        /// ���л�
        /// </summary>
        public static void Serialize(string savePath, BundleManifest patchManifest,bool pretty=false)
        {
            string json = JsonUtility.ToJson(patchManifest, pretty);
            FileUtility.CreateFile(savePath, json);
        }

        /// <summary>
        /// �����л�
        /// </summary>
        public static BundleManifest Deserialize(string jsonData)
        {
            BundleManifest bundleManifest = JsonUtility.FromJson<BundleManifest>(jsonData);
            bundleManifest.AfterDeserialize();
            return bundleManifest;
        }
    }
}


