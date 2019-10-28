using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace MiniFramework
{
    public class HttpDownload
    {
        public string FileName;
        public string SaveDir;
        public string FilePath;
        public float Progress;
        public long CurLength;
        public long TotalLength;
        private bool isReady;
        public HttpDownload(string saveDir)
        {
            SaveDir = saveDir;
            Directory.CreateDirectory(saveDir);
            isReady = true;
        }
        public IEnumerator Download(string url, Action<bool> callback = null)
        {
            if (!isReady)
            {
                Debug.LogError("下载失败");
            }
            isReady = false;
            FileName = url.Substring(url.LastIndexOf('/') + 1);
            FilePath = SaveDir + "/" + FileName;
            string tempFilePath = FilePath + ".tmp";
            CurLength = FileUtil.GetFileLength(tempFilePath);
            using (UnityWebRequest headRequest = UnityWebRequest.Head(url))
            {
                yield return headRequest.SendWebRequest();
                if (headRequest.isHttpError || headRequest.isNetworkError)
                {
                    Debug.LogError(headRequest.error);
                    if (callback != null)
                        callback(false);
                    isReady = true;
                    yield break;
                }
                TotalLength = long.Parse(headRequest.GetResponseHeader("Content-Length"));
            }
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Range", "bytes=" + CurLength + "-" + TotalLength);
                request.SendWebRequest();
                Debug.Log("开始下载:" + FileName + " 大小:" + UnitConvert.ByteAutoConvert(TotalLength) + " 已下载:" + UnitConvert.ByteAutoConvert(CurLength));
                using (FileStream fileStream = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fileStream.Seek(CurLength, SeekOrigin.Begin);
                    int index = 0;
                    while (!request.isDone)
                    {
                        yield return null;
                        byte[] data = request.downloadHandler.data;
                        int writeLength = data.Length - index;
                        fileStream.Write(data, index, writeLength);
                        index = data.Length;
                        CurLength += writeLength;
                        Progress = request.downloadProgress;
                    }
                }
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
                File.Move(tempFilePath, FilePath);
                Debug.Log("下载成功:" + FileName);
                if (callback != null)
                    callback(true);
                isReady = true;
            }
        }
    }
}