using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace URS 
{
    public class BuildResourceAndIL2CPPPlayer : ScriptableWizard
    {
        [SerializeField, Tooltip("��Ҫ��������Դ�汾")] public string BuildingResVersion;
        [SerializeField, Tooltip("Ҫ������Դ�汾")] public string BuildInResVersion;
        [SerializeField, Tooltip("��������")] public string Channel = "default_channel";

        [MenuItem("URS/Build(Resource And Player)-��IL2CPP��",false,100)]
        private static void Open()
        {
            DisplayWizard<BuildResourceAndIL2CPPPlayer>(ObjectNames.NicifyVariableName(nameof(BuildResourceAndIL2CPPPlayer)), "Build");
        }

        private void OnWizardCreate()
        {
            Build.BuildResourceAndPlayer_Standard(BuildingResVersion, BuildInResVersion, Channel);
        }
    }
}

