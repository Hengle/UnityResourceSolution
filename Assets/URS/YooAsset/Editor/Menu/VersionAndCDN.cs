using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace URS 
{
    public class VersionAndCDN : ScriptableWizard
    {
      
        [SerializeField, Tooltip("��������")]       public string Channel = "default_channel";
        [SerializeField, Tooltip("������Ŀ��汾,���Ϊ��,�Զ�Ϊ���µİ汾")] public string ChannelTargetVersion="";
        [SerializeField, Tooltip("��Դ�汾����������")] public int VersionKeepCount = 4;
        [SerializeField, Tooltip("�Ƿ��ϴ�CDN")]    public bool UploadCDN = false;

        [MenuItem("URS/BuildAutoChannelVersionsAndUploadCDN",false,105)]
        private static void Open()
        {
           var sw=  DisplayWizard<VersionAndCDN>(ObjectNames.NicifyVariableName(nameof(VersionAndCDN)), "Build");
        }

        private void OnWizardCreate()
        {
            Build.BuildAutoChannelVersionsAndUploadCDN(Channel, ChannelTargetVersion, VersionKeepCount, UploadCDN);
        }
    }
}

