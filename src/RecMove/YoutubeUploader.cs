using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecMove
{
    class YoutubeUploader
    {
        /// <summary>
        /// アップロードステータスの変更イベント
        /// </summary>
        public event Action<YoutubeUploadStatus> YoutubeUploadStatusChanged;
    
        /// <summary>
        /// アップロードアイテムリスト
        /// </summary>
        private readonly List<YoutubeUploadItem> uploadItems;
        
        /// <summary>
        /// アップロードタイトル（定型フォーマットあり）
        /// </summary>
        private readonly string uploadTitleFormat;

        /// <summary>
        /// 現在のアップロードステータス
        /// </summary>
        private readonly YoutubeUploadStatus status = new YoutubeUploadStatus();

        /// <summary>
        /// APIキー用のストリーム
        /// </summary>
        private readonly Stream apiStream;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="uploadItems"></param>
        /// <param name="uploadTitleFormat"></param>
        public YoutubeUploader(List<YoutubeUploadItem> uploadItems,string uploadTitleFormat,Stream apiStream)
        {
            this.uploadItems = uploadItems;
            this.uploadTitleFormat = uploadTitleFormat;
            this.apiStream = apiStream;
        }

        /// <summary>
        /// 非同期アップロードの開始
        /// </summary>
        /// <returns></returns>
        public async Task Run()
        {
            // Oauthする
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(apiStream).Secrets,
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access.
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );

            // YoutubeAPIのサービス作成
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            //ステータス更新用に情報を走査する
            foreach(var item in this.uploadItems)
            {
                if (!item.IsUpload) continue;

                this.status.FileCount++;
                this.status.FileAllByte += item.FileSize;
            }

            //実アップロードを実行
            var index = 0;
            foreach (var item in this.uploadItems)
            {
                if (!item.IsUpload) continue;
                index++;

                status.FileName = Path.GetFileName(item.FilePath);
                status.FileIndex = index;

                // 非同期アップロード’（１ファイル）
                await UploadFile(youtubeService, item, index);

                status.FileCurrentUploadedByte = 0;
                status.FileUploadedByte += item.FileSize;

                // ステータス変更イベントを発行
                YoutubeUploadStatusChanged?.Invoke(status.Clone());
            }
            youtubeService.Dispose();
        }

        /// <summary>
        /// アップロードを行う
        /// </summary>
        /// <param name="youtubeService"></param>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private async Task UploadFile(YouTubeService youtubeService, YoutubeUploadItem item, int index)
        {
            var video = new Video
            {
                Snippet = new VideoSnippet
                {
                    Title = item.GetFormatedTitle(this.uploadTitleFormat,index),
                    Description = item.GetFormatedTitle(this.uploadTitleFormat, index),
                    CategoryId = "20"   //ゲームカテゴリ
                },
                Status = new VideoStatus
                {
                    PrivacyStatus = "unlisted"
                }
            };
            using (var fileStream = new FileStream(item.FilePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += VideosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += VideosInsertRequest_ResponseReceived;
                // 実際のアップロードを実行する
                await videosInsertRequest.UploadAsync();
            }
        }

        /// <summary>
        /// 状態変更イベント
        /// </summary>
        /// <param name="progress"></param>
        void VideosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            status.Status = progress.Status;
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Debug.WriteLine("{0} bytes sent.", progress.BytesSent);
                    status.FileCurrentUploadedByte = progress.BytesSent;
                    break;

                case UploadStatus.Failed:
                    Debug.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
            YoutubeUploadStatusChanged?.Invoke(status.Clone());
        }

        /// <summary>
        /// レスポンス受信イベント
        /// </summary>
        /// <param name="video"></param>
        void VideosInsertRequest_ResponseReceived(Video video)
        {
            Debug.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }
    }
}
