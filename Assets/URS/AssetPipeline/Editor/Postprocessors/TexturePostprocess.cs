using System;
using System.Collections.Generic;
using System.Reflection;
using Daihenka.AssetPipeline.Import;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

class CustomAssetImportPostprocessor  
{
    public const string VFXDirectoryPath = "Assets/GameResources/VFX/Texture";

    private static string[] Platforms = new string[] {"Android","iOS" };
   public static void OnPostprocessTexture(string assetPath,AssetImporter assetImporter)
   {
        if (assetPath == null) return;
        if (!assetPath.Contains(VFXDirectoryPath)) return;
   
        var ti = assetImporter as TextureImporter;
        if (ti == null) return;

        bool showWarning = false;
        string message = "";
        foreach (var platform in Platforms)
        {
            
            var currentSetting = ti.GetPlatformTextureSettings(platform);
            // Debug.LogError("platform.name " + platform + " tPath ? " + (tPath));
            if (currentSetting == null)
            {
                message += $"  û�а�װ{platform}����չ  ";
                showWarning = true;
            }
            if (!currentSetting.overridden)
            {
                message += $"   {platform}û�е��overrider  ";
                showWarning = true;
            }
            if (currentSetting.overridden&&currentSetting.maxTextureSize > 1024)
            {

                message += $"  {platform}maxTextureSize ���ܸ���1024  ";
                showWarning = true;
            }
            if (currentSetting.overridden&&currentSetting.format != TextureImporterFormat.ASTC_4x4
                && currentSetting.format != TextureImporterFormat.ASTC_5x5
                && currentSetting.format != TextureImporterFormat.ASTC_6x6
                && currentSetting.format != TextureImporterFormat.ASTC_8x8
                && currentSetting.format != TextureImporterFormat.ASTC_10x10
                && currentSetting.format != TextureImporterFormat.ASTC_12x12
                     )
            {

                message += $"{platform}format ����astc,��ǰ�ĸ�ʽ�� {currentSetting.format}";
                showWarning = true;
            }
           
        }
        if (showWarning)
        {
            if (EditorApplication.isUpdating && EditorUtility.DisplayDialog("����", $"��Ч��ͼû������,·��{assetPath}��ԭ�� {message}", "ȷ��"))
            {

            }
        }
        else
        {
            Debug.Log($"��Ч��ͼ����OK,·��{assetPath}");
        }
    }

    [MenuItem("Tools/�����Ч��ͼ����")]
    public static void CheckVFXTexture()
    {
        var textures= AssetDatabase.FindAssets("t:Texture", new string[] { VFXDirectoryPath });
        if (textures == null) return;
        //Debug.LogError("platform.name " + textures.Length);
        foreach (var guid in textures)
        {
            var tPath = AssetDatabase.GUIDToAssetPath(guid);
            var assetImporter= AssetImporter.GetAtPath(tPath);
         
            var ti = assetImporter as TextureImporter;

            //Debug.LogError("platform.name " + tPath + " is null? " + (ti==null));
            if (ti == null) continue;

            bool showWarning = false;
            string message = "";
            foreach (var platform in Platforms)
            {
               
                var currentSetting = ti.GetPlatformTextureSettings(platform);
               // Debug.LogError("platform.name " + platform + " tPath ? " + (tPath));
                if (currentSetting == null)
                {
                    message += $"  û�а�װ{platform}����չ  ";
                    showWarning = true;
                }
                if (!currentSetting.overridden)
                {
                    message += $"   {platform}û�е��overrider  ";
                    showWarning = true;
                }
                if (currentSetting.overridden&&currentSetting.maxTextureSize > 1024)
                {

                    message += $"  {platform}maxTextureSize ���ܸ���1024 ";
                    showWarning = true;
                }
                if (currentSetting.overridden&&currentSetting.format != TextureImporterFormat.ASTC_4x4
                    && currentSetting.format != TextureImporterFormat.ASTC_5x5
                    && currentSetting.format != TextureImporterFormat.ASTC_6x6
                    && currentSetting.format != TextureImporterFormat.ASTC_8x8
                    && currentSetting.format != TextureImporterFormat.ASTC_10x10
                    && currentSetting.format != TextureImporterFormat.ASTC_12x12
                         )
                {

                    message += $"{platform}format ����astc ,��ǰ�ĸ�ʽ�� {currentSetting.format}";
                    showWarning = true;
                }
                
            }
            if (showWarning)
            {
                Debug.LogError($"��Ч��ͼû������,·��{tPath},ԭ��:{message}");
            }
            else
            {
                Debug.Log($"��Ч��ͼ����OK,·��{tPath}");
            }

        }
    }
}