using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace URS 
{
    public class BuildResourceAndMonoAndroidProject : ScriptableWizard
    {
        [SerializeField,Tooltip("��Ҫ��������Դ�汾")] public string BuildingResVersion;
        [SerializeField,Tooltip("Ҫ������Դ�汾")] public string BuildInResVersion;
        [SerializeField,Tooltip("��������")] public string Channel="default_channel";

        [MenuItem("URS/Build(Resource And AndroidProject)-��Ԫ��SDK��Mono��",false,103)]
        private static void Open()
        {
            DisplayWizard<BuildResourceAndMonoAndroidProject>(ObjectNames.NicifyVariableName(nameof(BuildResourceAndMonoAndroidProject)),"Build");
        }

        private void OnWizardCreate()
        {
            Build.ExportAndroidProject_Mono(BuildingResVersion, BuildInResVersion, Channel);
        }
    }
}

