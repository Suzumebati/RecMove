using System;
using System.Collections.Generic;
using System.Text;

namespace RecMove
{
    class YoutubeUploadItem
    {
        /// <summary>
        /// 録画日付置き換え文字
        /// </summary>
        public const string datePlaceHolder = "{REC_DATE}";
        
        /// <summary>
        /// アップロードファイルのインデックス
        /// </summary>
        public const string indexPlaceHolder = "{REC_INDEX}";

        /// <summary>
        /// アップロード対象か否か
        /// </summary>
        public bool IsUpload { get; set; }
        
        /// <summary>
        /// アップロードファイルのパス
        /// </summary>
        public string FilePath { get; set; }
        
        /// <summary>
        /// アップロードファイルのサイズ
        /// </summary>
        public long FileSize { get; set; }
        
        /// <summary>
        /// アップロードファイルのMbyteサイズ
        /// </summary>
        public long FileSizeMByte { get; set; }
        
        /// <summary>
        /// アップロードファイルのファイル更新時間
        /// </summary>
        public DateTime FileUpdateTime { get; set; }

        /// <summary>
        /// タイトル名取得
        /// </summary>
        /// <param name="titleFormat"></param>
        /// <param name="recIndex"></param>
        /// <returns></returns>
        public string GetFormatedTitle(string titleFormat,int recIndex)
        {
            titleFormat = titleFormat.Replace(datePlaceHolder,this.FileUpdateTime.ToString("yyyy/MM/dd"));
            titleFormat = titleFormat.Replace(indexPlaceHolder, recIndex.ToString());
            return titleFormat;
        }
    }
}
