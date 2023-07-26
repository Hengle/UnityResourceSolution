using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace URS 
{
    public class HybridBuild : ScriptableWizard
    {
        [SerializeField, Tooltip("��Ҫ��������Դ�汾")] public string BuildingResVersion;
        [SerializeField, Tooltip("Ҫ������Դ�汾")] public string BuildInResVersion;
        [SerializeField, Tooltip("��������")] public string Channel = "default_channel";
        [SerializeField, Tooltip("�ǲ���ҪbuildResource")] public bool BuildResource = true;
        [SerializeField, Tooltip("�ǲ���ҪbuildRaw")] public bool BuildRaw= true;
        [SerializeField, Tooltip("�ǲ���Ҫcopy BuildInRes")] public bool CopyBuildInRes = true;
        [SerializeField, Tooltip("�ǲ���Ҫbuild player")] public bool BuildPlayer = false;
        [SerializeField, Tooltip("�ǲ���Ҫbuild player")] public bool Debug = false;
        [MenuItem("URS/CustomBuild(�Զ��幹����",false,104)]
        private static void Open()
        {
           var sw=  DisplayWizard<HybridBuild>(ObjectNames.NicifyVariableName(nameof(HybridBuild)), "Build");
        }

        private void OnWizardCreate()
        {
            Build.HybridBuild(BuildingResVersion, BuildInResVersion, Channel, BuildResource, BuildRaw, CopyBuildInRes, BuildPlayer,Debug);
        }
    }
}

