using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace RecMove
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// シェル通知管理クラス
        /// </summary>
        private ShNotifyManager notifyer = new ShNotifyManager();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ウインドウロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ウインドウプロシージャの登録
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));

            // シェル通知の登録する
            var helper = new WindowInteropHelper(this);
            notifyer.RegisterChangeNotify(helper.Handle, ShNotifyManager.CSIDL.CSIDL_DESKTOP, true);
        }

        /// <summary>
        /// ウインドウクローズ中イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // シェル通知の解除
            notifyer.UnregisterChangeNotify();
            // 設定値の保存
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// ウインドウ状態変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        /// <summary>
        /// タスクトレイの表示・非表示クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_VisibleClick(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Hidden)
            {
                Show();
                WindowState = WindowState.Normal;
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// タスクトレイの終了クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// ウインドウプロシージャ実行（ウインドウメッセージの受信）
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((ShNotifyManager.WM)msg)
            {
                case ShNotifyManager.WM.WM_SHNOTIFY:
                    if (notifyer.IsDriveMount(lParam))
                    {
                        var mountedPath = notifyer.GetDrivePath(wParam);
                        if (TextBox_SrcDir.Text.StartsWith(mountedPath))
                        {
                            Label_Status.Content = $"指定ドライブのマウントを確認しました。Path={TextBox_SrcDir.Text}";
                            Button_Click(null,null);
                        }
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 今すぐ実行する押下時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(TextBox_SrcDir.Text))
            {
                Label_Status.Content = $"コピー元フォルダが存在しません。Path={TextBox_SrcDir.Text}";
                return;
            }

            if (Check_YoutubeUpload.IsChecked ?? false)
            {
                // Youtubeダイアログを開く
                ShowYoutubeDialog();
            }
            else
            {
                // ファイル移動する
                MoveFileAsync(TextBox_SrcDir.Text,
                    TextBox_DstDir.Text,
                    TextBox_FileExtention.Text,
                    Check_CreateYmdFolder.IsChecked ?? false,
                    Check_SaveSubFolder.IsChecked ?? false,
                    Check_CopyMode.IsChecked ?? false,
                    Check_Overwite.IsChecked ?? false,
                    Check_SeqNumberAdd.IsChecked ?? false);
            }
        }

        /// <summary>
        /// ファイルの移動
        /// </summary>
        /// <param name="srcDirPath"></param>
        /// <param name="dstDirPath"></param>
        /// <param name="fileExtention"></param>
        /// <param name="isAddingYmdDir"></param>
        /// <param name="isSavingSubDir"></param>
        /// <param name="isCopyMode"></param>
        /// <param name="isOverwite"></param>
        /// <param name="isAddingFileSeqNumber"></param>
        /// <returns></returns>
        private Task MoveFileAsync(
            string srcDirPath,
            string dstDirPath,
            string fileExtention,
            bool isAddingYmdDir,
            bool isSavingSubDir,
            bool isCopyMode,
            bool isOverwite,
            bool isAddingFileSeqNumber)
        {
            return Task.Factory.StartNew(() =>
            {
                // 移動先フォルダの作成
                var createDstDir = CreateDstFolder(dstDirPath, isAddingYmdDir);
                if (createDstDir == null)
                {
                    SetStatusLabel($"{(isCopyMode ? "コピー先" : "移動先")}フォルダが作成できませんでした。移動先->{dstDirPath}");
                    return;
                }

                // ファイルを移動する
                MoveFiles(srcDirPath, createDstDir, fileExtention, isSavingSubDir, isCopyMode, isOverwite, isAddingFileSeqNumber);

                SetStatusLabel($"ファイルの{(isCopyMode ? "コピー" : "移動")}完了 {srcDirPath} -> {createDstDir}");

                // 移動完了時のサウンド再生
                PlayCompleteSoundAsync();
            });
        }

        /// <summary>
        /// 移動先のフォルダ作成
        /// </summary>
        /// <param name="dstDirPath"></param>
        /// <param name="isAddingYmdDir"></param>
        /// <returns></returns>
        private string CreateDstFolder(string dstDirPath, bool isAddingYmdDir)
        {
            string createDstDir;
            if (isAddingYmdDir)
            {
                var index = 0;
                createDstDir = Path.Combine(dstDirPath, DateTime.Now.ToString("yyyyMMdd"));
                // 既に現在日付でフォルダが存在する場合は連番で探す
                while (index < 100 && Directory.Exists(createDstDir))
                {
                    index++;
                    createDstDir = Path.Combine(dstDirPath, $"{DateTime.Now.ToString("yyyyMMdd")}_{index:00}");
                }
            }
            else
            {
                createDstDir = dstDirPath;
            }

            if (!Directory.Exists(createDstDir))
            {
                try
                {
                    Directory.CreateDirectory(createDstDir);
                }
                catch (IOException)
                {
                    return null;
                }
            }
            return createDstDir;
        }

        /// <summary>
        /// ファイルを移動する
        /// </summary>
        /// <param name="srcDirPath"></param>
        /// <param name="dstDirAddDate"></param>
        /// <param name="fileExtention"></param>
        /// <param name="isSavingSubDir"></param>
        /// <param name="isCopyMode"></param>
        /// <param name="isOverwite"></param>
        /// <param name="isAddingFileSeqNumber"></param>
        private void MoveFiles(
            string srcDirPath,
            string dstDirAddDate,
            string fileExtention,
            bool isSavingSubDir,
            bool isCopyMode,
            bool isOverwite,
            bool isAddingFileSeqNumber)
        {
            // ファイル列挙
            foreach (var srcFilePath in EnumerateTargetFiles(srcDirPath, fileExtention))
            {
                // 移動先ファイル名の生成
                string dstFilePath;
                if (isSavingSubDir)
                {
                    // サブフォルダを維持する場合は構造を維持しつつ移動先ファイル名を生成する
                    dstFilePath = Path.Combine(dstDirAddDate, srcFilePath.Replace(srcDirPath, ""));
                    if (!Directory.Exists(Path.GetDirectoryName(dstFilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dstFilePath));
                    }
                }
                else
                {
                    dstFilePath = Path.Combine(dstDirAddDate, Path.GetFileName(srcFilePath));
                }

                // 既にファイルが有る場合は連番をつけて重複しないファイル名を作成する
                if (!isOverwite && isAddingFileSeqNumber && File.Exists(dstFilePath))
                {
                    var index = 1;
                    var seqDstFilePath = Path.Combine(Path.GetDirectoryName(dstFilePath), $"{Path.GetFileNameWithoutExtension(dstFilePath)}_{index:00}{Path.GetExtension(dstFilePath)}");
                    while (index < 100 && File.Exists(seqDstFilePath))
                    {
                        index++;
                        seqDstFilePath = Path.Combine(Path.GetDirectoryName(dstFilePath), $"{Path.GetFileNameWithoutExtension(dstFilePath)}_{index:00}{Path.GetExtension(dstFilePath)}");
                    }
                    dstFilePath = seqDstFilePath;
                }

                // 上書きしない設定なので既にファイルがある場合は次へ
                if (!isOverwite && File.Exists(dstFilePath))
                {
                    continue;
                }

                SetStatusLabel($"ファイルを{(isCopyMode ? "コピー中" : "移動中")}...{srcFilePath} -> {dstFilePath}");

                // ファイルの移動・コピー
                if (isCopyMode)
                {
                    // ファイルのコピー
                    File.Copy(srcFilePath, dstFilePath, isOverwite);
                }
                else
                {
                    // ファイルの移動
                    File.Move(srcFilePath, dstFilePath, isOverwite);
                }
            }
        }

        /// <summary>
        /// 移動対象の列挙
        /// </summary>
        /// <param name="srcDirPath"></param>
        /// <param name="fileExtention"></param>
        /// <returns></returns>
        private IEnumerable<string> EnumerateTargetFiles(string srcDirPath, string fileExtention)
        {
            IEnumerable<string> enumrateFiles = null;
            foreach (var extention in fileExtention.Split(";", StringSplitOptions.RemoveEmptyEntries))
            {
                if (enumrateFiles == null)
                {
                    enumrateFiles = Directory.EnumerateFiles(srcDirPath, extention, SearchOption.AllDirectories);
                }
                else
                {
                    enumrateFiles.Concat(Directory.EnumerateFiles(srcDirPath, extention, SearchOption.AllDirectories));
                }
            }
            return enumrateFiles;
        }

        /// <summary>
        /// 移動完了サウンドの再生
        /// </summary>
        /// <returns></returns>
        private Task PlayCompleteSoundAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                // リソースからサウンド読み出し
                using var soundStream = Properties.Resources.nc122233;
                // 同期的にサウンドを再生する
                using var player = new SoundPlayer(soundStream);
                player.PlaySync();
            });
        }

        /// <summary>
        /// ステータスラベルを設定する
        /// </summary>
        /// <param name="message"></param>
        private void SetStatusLabel(string message)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                Label_Status.Content = message;
            }));
        }

        /// <summary>
        /// Youtubeのアップロードダイアログを開く
        /// </summary>
        /// <returns></returns>
        private Task ShowYoutubeDialog()
        {
            return Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    var win = new YoutubeUploadWindow(EnumerateTargetFiles(TextBox_SrcDir.Text, TextBox_FileExtention.Text));
                    win.ShowDialog();
                }));
            });
        }
    }
}
