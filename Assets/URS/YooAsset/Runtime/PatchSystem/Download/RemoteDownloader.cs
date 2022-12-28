using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using URS;
using BestHTTP;
//using static YooAsset.DownloaderOperation;
using MHLab.Patch.Core.Utilities;
using System.Net;
using MHLab.Patch.Core.Octodiff;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;

namespace YooAsset
{
    internal class RemoteDownloader
    {
        private enum ESteps
        {
            None,
            CreateDownload,
            CheckDownload,
            // TryAgain,
            Succeed,
            Failed,
        }

        // private readonly HardiskFileSearchResult _hardiskFileSearchResult;
        private readonly UpdateEntry _updateEntry;
        //  private UnityWebRequest _webRequest;
        //  private UnityWebRequestAsyncOperation _operationHandle;
        private HTTPRequest _request;
        private HTTPResponse _response;
        private bool _useStream = true;
        private ESteps _steps = ESteps.None;
        private string _lastError = string.Empty;

        private int _timeout;
        private int _failedTryAgain;
        private int _requestCount;
        private string _requestURL;

        // ���ñ���
        // private bool _isAbort = false;
        private long _latestDownloadBytes;
        private float _latestDownloadRealtime;
        private float _tryAgainTimer;

        /// <summary>
        /// ���ؽ��ȣ�0-100f��
        /// </summary>
        public float DownloadProgress { private set; get; }

        /// <summary>
        /// �Ѿ����ص����ֽ���
        /// </summary>
        public long DownloadedBytes { private set; get; }


        internal RemoteDownloader(UpdateEntry info)
        {
            _updateEntry = info;

#if UNITY_WEBGL
            _useStream = false;
#else
            _useStream = true;
#endif
        }
        internal void SendRequest(int failedTryAgain, int timeout)
        {
          
            if (string.IsNullOrEmpty(_updateEntry.GetLocalSaveFilePath()))
                throw new ArgumentNullException();

            if (_steps == ESteps.None)
            {
                _failedTryAgain = failedTryAgain;
                _timeout = timeout;
                _steps = ESteps.CreateDownload;
            }
        }
        internal void Update()
        {
            if (_steps == ESteps.None)
                return;
            if (_steps == ESteps.Failed || _steps == ESteps.Succeed)
                return;

            // ����������
            if (_steps == ESteps.CreateDownload)
            {
                // ���ñ���
                DownloadProgress = 0f;
                DownloadedBytes = 0;
                //_isAbort = false;
                _latestDownloadBytes = 0;
                _latestDownloadRealtime = Time.realtimeSinceStartup;
                _tryAgainTimer = 0f;

                _requestCount++;
                _requestURL = GetRequestURL();
                //_webRequest = new UnityWebRequest(_requestURL, UnityWebRequest.kHttpVerbGET);
                //DownloadHandlerFile handler = new DownloadHandlerFile(_hardiskFileSearchResult.HardiskPath);
                //handler.removeFileOnAbort = true;
                //_webRequest.downloadHandler = handler;
                //_webRequest.disposeDownloadHandlerOnDispose = true;
                //_operationHandle = _webRequest.SendWebRequest();
                _request = new HTTPRequest(new Uri(_requestURL), this.OnRequestFinish);
                _request.OnDownloadProgress = OnDownloadProgress;
                _request.DisableCache = true;
                _request.MaxRetries = _failedTryAgain;
                if (_useStream)
                {
                    _request.OnStreamingData += OnData;
                }
                _response = null;
                _request.Send();
                //_request.OnRequestFinishedDelegate
                _steps = ESteps.CheckDownload;
            }

            // ������ؽ��
            if (_steps == ESteps.CheckDownload)
            {
                // DownloadProgress = _webRequest.downloadProgress * 100f;
                // DownloadedBytes = _webRequest.downloadedBytes;

                // DownloadProgress= _request.progr
                if (_response == null)
                {
                    CheckTimeout();
                    return;
                }
                // ����������
                bool isError = true;
                switch (_request.State)
                {
                    // The request finished without any problem.
                    case HTTPRequestStates.Finished:
                        if (_response.IsSuccess)
                        {
                            isError = false;
                        }
                        else
                        {
                            _lastError = (string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                            _response.StatusCode,
                                                            _response.Message,
                                                            _response.DataAsText));
                        }
                        break;

                    default:
                        // There were an error while downloading the content.
                        // The incomplete file should be deleted.
                        _lastError = string.Format("can not download url{0},status {1}", _requestURL, _request.State);
                        break;
                }



                // ����ļ�������
                if (!isError)
                {
                    if (!_useStream)
                    {
                        ClearLocalSaveFile();
                        EnsureDirectory();
                        try
                        {
                            File.WriteAllBytes(_updateEntry.GetLocalSaveFilePath(), _response.Data);
                        }
                        catch (Exception e) 
                        {
                            isError = true;
                        }
                    }
                    if (!isError)
                    {
                        if (_updateEntry.IsPatch())
                        {
                            var patchTargetTemp = _updateEntry.GetPatchTemp();
                            if (File.Exists(patchTargetTemp))
                            {
                                File.Delete(patchTargetTemp);
                            }
                            File.Move(_updateEntry.GetPatchTargetPath(), patchTargetTemp);
                            try
                            {
                                DeltaFileApplier.Apply(patchTargetTemp, _updateEntry.GetLocalSaveFilePath(), _updateEntry.GetPatchTargetPath());
                                if (SandboxFileSystem.CheckContentIntegrity(_updateEntry.RemoteFileMeta, _updateEntry.GetPatchTargetPath()))
                                {
                                    SandboxFileSystem.RegisterVerifyFile(_updateEntry.RemoteFileMeta);
                                }
                                else
                                {
                                    Debug.LogWarning("����ʧ��" + _updateEntry.RemoteFileMeta.RelativePath);
                                    isError = true;
                                    _lastError = $"Verification failed";
                                }

                            }
                            catch (Exception e)
                            {
                                if (File.Exists(patchTargetTemp))
                                {
                                    File.Delete(patchTargetTemp);
                                }
                                SandboxFileSystem.DeleteSandboxFile(_updateEntry.GetRelativePath());
                                _lastError = $"patch failed message : {e.Message}";
                                isError = true;
                            }
                        }
                        else
                        {
                            if (SandboxFileSystem.CheckContentIntegrity(_updateEntry.RemoteFileMeta, _updateEntry.GetLocalSaveFilePath()))
                            {
                                SandboxFileSystem.RegisterVerifyFile(_updateEntry.RemoteFileMeta);
                            }
                            else
                            {
                                isError = true;
                                _lastError = $"Verification failed";
                            }
                        }
                    }
                }

                if (isError)
                {
                    // ע�⣺����ļ���֤ʧ����Ҫɾ���ļ�
                    ClearLocalSaveFile();
                    _steps = ESteps.Failed;
                }
                else
                {
                    _steps = ESteps.Succeed;
                    Logger.Log($"�������{_requestURL} ");
                    // SandboxFileSystem.RegisterVerifyFile(_downloadInfo.FileMeta);
                }

                // �ͷ�������
                DisposeRequest();
            }

            // ���³�������
            //if (_steps == ESteps.TryAgain)
            //{
            //    _tryAgainTimer += Time.unscaledDeltaTime;
            //    if (_tryAgainTimer > 0.5f)
            //    {
            //        _failedTryAgain--;
            //        _steps = ESteps.CreateDownload;
            //        Logger.Warning($"Try again download : {_requestURL}");
            //    }
            //}
        }
        internal void SetDone()
        {
            _steps = ESteps.Succeed;
        }

        public void ClearLocalSaveFile() 
        {
            if (File.Exists(_updateEntry.GetLocalSaveFilePath()))
            {
                File.Delete(_updateEntry.GetLocalSaveFilePath());
            }
        
        }
        private void OnRequestFinish(HTTPRequest originalRequest, HTTPResponse response)
        {
           // Debug.LogError("on OnRequestFinish" + _requestURL + " " + response.IsSuccess);

            _response = response;
            var fs = _request.Tag as System.IO.FileStream;
            if (fs != null)
                fs.Dispose();
           

        }

        private string GetRequestURL()
        {
            return _updateEntry.GetRemoteDownloadURL() ;
           // // �������������ַ
           // if (_requestCount % 2 == 0)
           //     return _hardiskFileSearchResult.RemoteFallBackDownloadURL;
           // else
           //     return _hardiskFileSearchResult.RemoteDownloadURL;
        }
        private void CheckTimeout()
        {
            // ע�⣺������ʱ������������������ݼ��ж�Ϊ��ʱ
            if (_response == null)
            {
                if (_latestDownloadBytes != DownloadedBytes)
                {
                    _latestDownloadBytes = DownloadedBytes;
                    _latestDownloadRealtime = Time.realtimeSinceStartup;
                }

                float offset = Time.realtimeSinceStartup - _latestDownloadRealtime;
                if (offset > _timeout)
                {
                    Logger.Warning($"Web file request timeout : {_requestURL}");
                    _lastError = "timeout ";
                    _steps = ESteps.Failed;
                    DisposeRequest() ;
                   // _isAbort = true;
                }
            }
        }
        private void DisposeRequest()
        {
            if (_request != null)
            {
                _request.Abort();
                _request = null;
                _response = null;
            }
        }
       //private void DisposeWebRequest()
       //{
       //    if (_webRequest != null)
       //    {
       //        _webRequest.Dispose();
       //        _webRequest = null;
       //        _operationHandle = null;
       //    }
       //}

        /// <summary>
        /// ��ȡ��Դ����Ϣ
        /// </summary>
        public UpdateEntry GetUpdateEntry()
        {
            return _updateEntry;
        }

        /// <summary>
        /// ����������Ƿ��Ѿ���ɣ����۳ɹ���ʧ�ܣ�
        /// </summary>
        public bool IsDone()
        {
            return _steps == ESteps.Succeed || _steps == ESteps.Failed;
        }

        /// <summary>
        /// ���ع����Ƿ�������
        /// </summary>
        /// <returns></returns>
        public bool HasError()
        {
            return _steps == ESteps.Failed;
        }

        /// <summary>
        /// ���������Ϣ
        /// </summary>
        public void ReportError()
        {
            Logger.Error($"Failed to download : {_requestURL} Error : {_lastError}");
        }

        private bool OnData(HTTPRequest req, HTTPResponse resp, byte[] dataFragment, int dataFragmentLength)
        {
            if (resp.IsSuccess)
            {
                var fs = req.Tag as System.IO.FileStream;
                if (fs == null)
                {
                    EnsureDirectory();
                    req.Tag = fs = new System.IO.FileStream(_updateEntry.GetLocalSaveFilePath(), System.IO.FileMode.Create);
                }
                fs.Write(dataFragment, 0, dataFragmentLength);
            }

            // Return true if dataFragment is processed so the plugin can recycle it
            return true;
        }

        private void EnsureDirectory()
        {
            var directoryName = Path.GetDirectoryName(_updateEntry.GetLocalSaveFilePath());
            if (!Directory.Exists(directoryName)) 
            {
                Directory.CreateDirectory(directoryName);
            }
        }
        private void OnDownloadProgress(HTTPRequest request, long downloaded, long length)
        {
            DownloadedBytes = downloaded;
            DownloadProgress = (downloaded / (float)length) * 100.0f;
        }
      
    }
}