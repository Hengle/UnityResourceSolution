using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using YooAsset;

[Serializable]
public class URSRuntimeSetting
{
    public const string SAVE_RESOUCE_PATH = "urs_runtime_setting.txt";

    public const string SAVE_RESOUCE_PATH_NO_EXTENSION = "urs_runtime_setting";

    [SerializeField]
    public string FilesVersionIndexFileName = "files_version_index.txt";

    [SerializeField]
    public string PatchDirectory = "patch";

    [SerializeField]
    public string PatchTempDirectory = "patch_temp";
    /// <summary>
    /// AssetBundle�ļ��ĺ�׺��
    /// </summary>
    [SerializeField]
    public string AssetBundleFileVariant = ".bundle";

    /// <summary>
    /// ԭ���ļ��ĺ�׺��
    /// </summary>
    [SerializeField]
    public string RawFileVariant = "rawfile";

    /// <summary>
    /// ����������ļ��嵥�ļ�����
    /// </summary>
    [SerializeField]
    public string FileManifestFileName = "file_manifest.txt";

    /// <summary>
    /// ����������ļ��嵥�ļ�����
    /// </summary>
    [SerializeField]
    public string BundleManifestFileRelativePath = "bundles/bundle_manifest.txt";

    /// <summary>
    /// ����������ļ��嵥�ļ�����
    /// </summary>
    [SerializeField]
    public string BundleManifestFileName = "bundle_manifest.txt";

    /// <summary>
    /// ��ʾapp��id�ļ����ƣ�����stream asset�� urs_buildin_resource ����
    /// </summary>
    [SerializeField]
    public string ChannelFileName = "channel.txt";

    /// <summary>
    /// Ĭ�ϵ�ɳ��Ŀ¼�� persistent Ŀ¼�������ǿ��ָ�����Ŀ¼��������ĳ���汾��ʱ��
    /// </summary>
    [SerializeField]
    public string ForceDownloadDirectory = string.Empty;

    [SerializeField]
    public YooAssets.EPlayMode PlayMode = YooAssets.EPlayMode.EditorPlayMode;

    [SerializeField]
    public string RemoteChannelRootUrl = @"https://staticninja.happyelements.com";

    [SerializeField]
    public string RemoteAppVersionRouterFileName = "app_version_router.txt";
    //public string FallbackHostServer = "";

    [SerializeField]
    public string DefaultTag = "default";

    [SerializeField]
    public string BuildinTag = "buildin";

    public static URSRuntimeSetting instance
    {
        get
        {
            if (_instance == null)
            {
                var textAsset = Resources.Load<TextAsset>(SAVE_RESOUCE_PATH_NO_EXTENSION);
                if (textAsset != null)
                {
                    _instance = UnityEngine.JsonUtility.FromJson<URSRuntimeSetting>(textAsset.text);
                }
                else 
                {
                    _instance = new URSRuntimeSetting();
                }
            }
             return _instance;
        }
    }
    private static URSRuntimeSetting _instance = null;
}


