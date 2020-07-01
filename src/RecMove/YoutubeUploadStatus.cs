using Google.Apis.Upload;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecMove
{
    class YoutubeUploadStatus
    {
        /// <summary>
        /// 同期用オブジェクト
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// アップロードファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// アップロードファイルの現在のインデックス
        /// </summary>
        public int FileIndex { get; set; }
        
        /// <summary>
        /// アップロードのファイル数
        /// </summary>
        public int FileCount { get; set; }
        
        /// <summary>
        /// 現在ファイルのアップロード済みバイト数
        /// </summary>
        public long FileCurrentUploadedByte { get; set; }
        
        /// <summary>
        /// アップロード済みバイト数
        /// </summary>
        public long FileUploadedByte { get; set; }
        
        /// <summary>
        /// アップロードファイルすべてのバイト数
        /// </summary>
        public long FileAllByte { get; set; }

        /// <summary>
        /// アップロードステータス
        /// </summary>
        public UploadStatus Status { get; set; }

        /// <summary>
        /// 全体の完了フラグ
        /// </summary>
        public bool IsAllComplete { get; set; }

        /// <summary>
        /// オブジェクトクローン
        /// </summary>
        /// <returns></returns>
        public YoutubeUploadStatus Clone()
        {
            lock(lockObj)
            {
                var obj = new YoutubeUploadStatus();
                obj.FileName = this.FileName;
                obj.FileIndex = this.FileIndex;
                obj.FileCount = this.FileCount;
                obj.FileCurrentUploadedByte = this.FileCurrentUploadedByte;
                obj.FileUploadedByte = this.FileUploadedByte;
                obj.FileAllByte = this.FileAllByte;
                obj.Status = this.Status;
                return obj;
            }
        }
    }
}
