using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hextant;
using YooAsset;
#if UNITY_EDITOR
    using UnityEditor;
#endif
[Settings(SettingsUsage.RuntimeProject)]
public class URSRuntimeSetting : Hextant.Settings<URSRuntimeSetting>
{
    public string FilesVersionIndexFileName = "files_version_index.txt";
    public string PatchDirectory = "patch";
    public string PatchTempDirectory = "patch_temp";
    /// <summary>
    /// AssetBundle�ļ��ĺ�׺��
    /// </summary>
    public string AssetBundleFileVariant = ".bundle";

    /// <summary>
    /// ԭ���ļ��ĺ�׺��
    /// </summary>
    public string RawFileVariant = "rawfile";

    /// <summary>
    /// ����������ļ��嵥�ļ�����
    /// </summary>
    public string FileManifestFileName = "file_manifest.txt";

    /// <summary>
    /// ����������ļ��嵥�ļ�����
    /// </summary>
    public string BundleManifestFileRelativePath = "bundles/bundle_manifest.txt";

    /// <summary>
    /// ����������ļ��嵥�ļ�����
    /// </summary>
    public string BundleManifestFileName = "bundle_manifest.txt";

    /// <summary>
    /// ��ʾapp��id�ļ����ƣ�����stream asset�� urs_buildin_resource ����
    /// </summary>
    public string ChannelFileName = "channel.txt";

    /// <summary>
    /// Ĭ�ϵ�ɳ��Ŀ¼�� persistent Ŀ¼�������ǿ��ָ�����Ŀ¼��������ĳ���汾��ʱ��
    /// </summary>
    public string ForceDownloadDirectory = string.Empty;

    public YooAssets.EPlayMode PlayMode = YooAssets.EPlayMode.EditorPlayMode;

    public string RemoteChannelRootUrl = "http://127.0.0.1:8000";

    public string RemoteAppVersionRouterFileName = "app_version_router.txt";
   //public string FallbackHostServer = "";

    public string DefaultTag = "default";

    public string BuildinTag = "buildin";
}
#if UNITY_EDITOR
[InitializeOnLoad]
 static class ProjectOpenURSRuntimeSetting
{
    static ProjectOpenURSRuntimeSetting()
    {
        var instance= URSRuntimeSetting.instance;
    }
}
#endif