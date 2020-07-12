using Google.Apis.Upload;
using Google.Apis.YouTube.v3.Data;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RecMove
{
    /// <summary>
    /// YoutubeUploadWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class YoutubeUploadWindow : Window
    {
        /// <summary>
        /// アップローダークラス
        /// </summary>
        private YoutubeUploader uploader = null;
        
        /// <summary>
        /// アップロードアイテムリスト
        /// </summary>
        private List<YoutubeUploadItem> uploadItemList = new List<YoutubeUploadItem>();

        /// <summary>
        /// API用のストリーム
        /// </summary>
        private Stream apiKeyStream = new MemoryStream();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcFiles"></param>
        public YoutubeUploadWindow(IEnumerable<string> srcFiles)
        {
            InitializeComponent();

            var data = new ObservableCollection<YoutubeUploadItem>();
            foreach (var file in srcFiles)
            {
                var fileInfo = new System.IO.FileInfo(file);
                var item = new YoutubeUploadItem()
                {
                    IsUpload = true,
                    FilePath = file,
                    FileUpdateTime = fileInfo.LastWriteTime,
                    FileSize = fileInfo.Length,
                    FileSizeMByte = Convert.ToInt32(fileInfo.Length / 1024 / 1024)
                };
                data.Add(item) ;
                uploadItemList.Add(item);
            }

            // グリッドにバインド
            MovieList.ItemsSource = data;
        }

        /// <summary>
        /// アップロードボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button_Upload.IsEnabled = false;
            MovieList.IsReadOnly = true;

            Label_Status.Content = "アップロード開始しました。";

            LoadApiKey(apiKeyStream);
            uploader = new YoutubeUploader(uploadItemList,TextBox_Title.Text, apiKeyStream);
            uploader.YoutubeUploadStatusChanged += YoutubeUploadStatusChanged;

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);

            // アップロード実行
            await uploader.Run();

            uploader.YoutubeUploadStatusChanged -= YoutubeUploadStatusChanged;
            uploader = null;

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);

            Label_Status.Content = "アップロード完了しました。";

            Button_Upload.IsEnabled = true;
            MovieList.IsReadOnly = false;
        }

        /// <summary>
        /// アップロードステータス変更イベント
        /// </summary>
        /// <param name="status"></param>
        private void YoutubeUploadStatusChanged(YoutubeUploadStatus status)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                // すべてのファイルアップロードを終えたらサウンドを流して完了状態にする
                if (status.IsAllComplete)
                {
                    UploadProgress.Maximum = 100;
                    UploadProgress.Value = 100;
                    Utility.PlayCompleteSoundAsync();
                    return;
                }

                var percent = Convert.ToInt32((status.FileCurrentUploadedByte + status.FileUploadedByte) / status.FileAllByte);
                UploadProgress.Maximum = 100;
                UploadProgress.Value = percent;
                TaskbarManager.Instance.SetProgressValue(percent, 100);

                switch (status.Status)
                {
                    case UploadStatus.Uploading:
                        Label_Status.Content = $"[{status.FileName}]をアップロード中です。({status.FileIndex}/{status.FileCount})";
                        break;

                    case UploadStatus.Failed:
                        Label_Status.Content = $"[{status.FileName}]のアップロードに失敗しました。({status.FileIndex}/{status.FileCount})";
                        break;

                    case UploadStatus.Completed:
                        Label_Status.Content = $"[{status.FileName}]をアップロード完了しました。({status.FileIndex}/{status.FileCount})";
                        break;
                }
            }));
        }

        /// <summary>
        /// APIキーのロード
        /// </summary>
        /// <returns></returns>
        private void LoadApiKey(Stream outStream)
        {
            using var apiKeySt = new MemoryStream(Properties.Resources.api);
            FileEncryptor.Decrypt(apiKeySt, outStream, "n1xDVuFqSN");
            outStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// 動画のプレビュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = MovieList.SelectedItem as YoutubeUploadItem;
            if (selectItem != null)
            {
                Utility.ShellExecute(selectItem.FilePath);
            }
        }
    }
}
