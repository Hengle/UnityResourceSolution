using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace URS
{
    public enum EnumHardiskDirectoryType
    {
        Invalid = 0,
        Persistent = 1,
        StreamAsset = 2
    }
    public class UpdateEntry
    {
        public FileMeta RemoteFileMeta { get; private set; }

        public string _remoteDownloadURL;

        public PatchItemVersion PatchItemVersion { get; private set; }

        private string _targetSaveFilePath;
        private string _patchSavePath;

        public bool IsPatch()
        {
            return PatchItemVersion != null;
        }

        public string GetRemoteDownloadURL()
        {
            return _remoteDownloadURL;
        }

        public string GetPatchTargetPath() 
        {
            return _targetSaveFilePath;
        }

        public string GetPatchTemp()
        {
            var pathRelativePath = $"{GetRelativePath()}---{PatchItemVersion.FromHashCode}---{PatchItemVersion.ToHashCode}.patch.temp";
            return SandboxFileSystem.MakeSandboxFilePath(pathRelativePath);
        }

        public uint GetRemoteDownloadFileHash() 
        {
            if (PatchItemVersion != null)
            {
                return PatchItemVersion.Hash;
            }
            else
            {
                return RemoteFileMeta.Hash;
            }
        }

        public long GetRemoteDownloadFileSize()
        {
            if (PatchItemVersion != null) 
            { 
                return PatchItemVersion.SizeBytes;
            }
            else
            {
                return RemoteFileMeta.SizeBytes;
            }
        }
        public string GetRelativePath() 
        {
            return RemoteFileMeta.RelativePath;
        }
        public string GetLocalSaveFilePath() 
        {
            if (PatchItemVersion != null)
            {
                return _patchSavePath;
            }
            else
            {
                return _targetSaveFilePath;
            }
        }

        public UpdateEntry(FileMeta remoteFileMeta, string remoteVersionRoot, PatchItemVersion patchItem =null,string remotePatchRoot=null) 
        {
            RemoteFileMeta = remoteFileMeta;
            PatchItemVersion = patchItem;
            _targetSaveFilePath = SandboxFileSystem.MakeSandboxFilePath(GetRelativePath());
            if (PatchItemVersion != null)
            {
                var pathRelativePath = $"{GetRelativePath()}---{PatchItemVersion.FromHashCode}---{PatchItemVersion.ToHashCode}.patch";
                _patchSavePath = SandboxFileSystem.MakeSandboxPatchFilePath(pathRelativePath);
                _remoteDownloadURL = $"{remotePatchRoot}/{pathRelativePath}";
            }
            else 
            {
                _remoteDownloadURL = $"{remoteVersionRoot}/{GetRelativePath()}"; 
            }
        }
    }

    public class UnzipEntry
    {
        public string HardiskSavePath;

        public string HardiskSourcePath;

        public FileMeta StreamFileMeta;

        public uint Hash 
        {
            get
            {
                return StreamFileMeta.Hash;
            }
        }
        /// <summary>
        /// �ļ���С
        /// </summary>
        public long SizeBytes
        {
            get
            {
                return StreamFileMeta.SizeBytes;

            }
        }

        public string GetRelativePath()
        {
            return StreamFileMeta.RelativePath;
        }

        public UnzipEntry(FileMeta streamFileMeta) 
        {
            StreamFileMeta = streamFileMeta;
            HardiskSavePath = SandboxFileSystem.MakeSandboxFilePath(StreamFileMeta.RelativePath);
            HardiskSourcePath = AssetPathHelper.MakeStreamingSandboxLoadPath(StreamFileMeta.RelativePath);
            HardiskSourcePath = AssetPathHelper.ConvertToWWWPath(HardiskSourcePath);
        }
    
    }

    /// <summary>
    ///  �Ż����ṹ��
    /// </summary>
    public class HardiskFileSearchResult
    {
        // <summary>
        /// ���ش洢��·������Զ��Ϊ�գ���RemoteDownloadURL Ϊnullʱ��HardiskPath Ϊ��Դ�����ʵ��·��������HardiskPath Ϊ����֮��ı���·��
        /// </summary>
        public string HardiskPath;

        public string OrignRelativePath;

        //  public string RemoteDownloadURL;

        //  public string RemoteFallBackDownloadURL;

        public UpdateEntry UpdateEntry; // Զ��������Ϣ
        /// <summary>
        /// ���ڲ�ѯ��fileMeta
        /// </summary>
        public FileMeta FileMeta;

        /// <summary>
        /// ��ݿ�ݣ���¼HardiskPath�ֶ��������ļ�������
        /// </summary>
        public EnumHardiskDirectoryType HardiskDirectoryType = EnumHardiskDirectoryType.Invalid;


        public static HardiskFileSearchResult EMPTY;

        static HardiskFileSearchResult()
        {
            EMPTY = new HardiskFileSearchResult(string.Empty);
        }
        /// <summary>
        /// �ļ���ϣֵ
        /// </summary>
        public uint Hash
        {
            get
            {
                if (FileMeta == null)
                    return 0;
                else
                    return FileMeta.Hash;
            }
        }

       ///// <summary>
       ///// У���CRC
       ///// </summary>
       //public string CRC
       //{
       //    get
       //    {
       //        if (FileMeta == null)
       //            return string.Empty;
       //        else
       //            return FileMeta.CRC;
       //    }
       //}


        /// <summary>
        /// �ļ���С
        /// </summary>
        public long SizeBytes
        {
            get
            {
                if (FileMeta == null)
                    return 0;
                else
                    return FileMeta.SizeBytes;
            }
        }
        /// <summary>
		/// �Ƿ�Ϊ�����ļ�
		/// </summary>
		public bool IsEncrypted
        {
            get
            {
                if (FileMeta == null)
                    return false;
                else
                    return FileMeta.IsEncrypted;
            }
        }

        /// <summary>
        /// �Ƿ�Ϊԭ���ļ�
        /// </summary>
        public bool IsUnityBundle
        {
            get
            {
                if (FileMeta == null)
                    return false;
                else
                    return FileMeta.IsUnityBundle;
            }
        }
        public bool IsLocalFileExist()
        {
            return UpdateEntry==null; 
        }

        public bool IsValidFile()
        {
            return this.FileMeta != null;
        }

        public string GetHardDiskSavePath()
        {
            return HardiskPath;
        }
        /// <summary>
        /// ��ҪԶ������
        /// </summary>
        /// <param name="fileMeta"></param>
        /// <param name="hardiskPath"></param>
        /// <param name="updateEntry"></param>
        /// <param name="hardiskDirectoryType"></param>
        public HardiskFileSearchResult(
            FileMeta fileMeta,
            UpdateEntry updateEntry
            )
        {
            this.FileMeta = fileMeta;
            OrignRelativePath = fileMeta.RelativePath;
            HardiskPath = SandboxFileSystem.MakeSandboxFilePath(OrignRelativePath);
            UpdateEntry=  updateEntry;
            HardiskDirectoryType = EnumHardiskDirectoryType.Invalid;
        }
        /// <summary>
        /// ����ҪԶ�����صı��غϷ��ļ�
        /// </summary>
        /// <param name="fileMeta"></param>
        /// <param name="hardiskPath"></param>
        /// <param name="hardiskDirectoryType"></param>
        public HardiskFileSearchResult(
           FileMeta fileMeta,
           string hardiskPath,
           EnumHardiskDirectoryType hardiskDirectoryType
            )
        {
            this.FileMeta = fileMeta;
            OrignRelativePath = fileMeta.RelativePath;
            HardiskPath = hardiskPath;
            UpdateEntry = null;
            HardiskDirectoryType = hardiskDirectoryType;
        }
        /// <summary>
        /// ���Ϸ����ļ�
        /// </summary>
        /// <param name="orignRelativePath"></param>
        /// <param name="hardiskPath"></param>
        /// <param name="hardiskDirectoryType"></param>
        public HardiskFileSearchResult(
          string orignRelativePath
           )
        {
            OrignRelativePath = orignRelativePath;
            HardiskPath = null;
            this.FileMeta = null;
            UpdateEntry = null;
            HardiskDirectoryType = EnumHardiskDirectoryType.Invalid;
        }
    }
}

