using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace URS 
{
    public class BuildResourceAndMonoPlayer : ScriptableWizard
    {
        [SerializeField, Tooltip("��Ҫ��������Դ�汾")] public string BuildingResVersion;
        [SerializeField, Tooltip("Ҫ������Դ�汾")] public string BuildInResVersion;
        [SerializeField, Tooltip("��������")] public string Channel = "default_channel";

        [MenuItem("URS/Build(Resource And Player)-��Mono��",false,101)]
        private static void Open()
        {
           var sw=  DisplayWizard<BuildResourceAndMonoPlayer>(ObjectNames.NicifyVariableName(nameof(BuildResourceAndMonoPlayer)), "Build");
            sw.createButtonName = "Build";
        }

        private void OnWizardCreate()
        {
            Build.BuildResourceAndPlayer_Fast(BuildingResVersion, BuildInResVersion, Channel);
        }
    }
}

