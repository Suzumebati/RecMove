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
            // ウインドウプロックの登録
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));

            // シェル通知の登録
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
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        /// <summary>
        /// タスクトレイの表示・非表示クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_VisibleClick(object sender, RoutedEventArgs e)
        {
            if (this.Visibility == Visibility.Hidden)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.Hide();
            }
        }

        /// <summary>
        /// タスクトレイの終了クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_CLoseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// ウインドウプロック
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
                            MoveFileAsync(TextBox_SrcDir.Text,
                                TextBox_DstDir.Text,
                                TextBox_FileExtention.Text,
                                Check_CreateYmdFolder.IsChecked,
                                Check_SaveSubFolder.IsChecked,
                                Check_CopyMode.IsChecked,
                                Check_Overwite.IsChecked,
                                Check_SeqNumberAdd.IsChecked);
                        }
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// ファイル移動
        /// </summary>
        /// <param name="srcDirPath"></param>
        /// <param name="dstDirPath"></param>
        /// <returns></returns>
        private Task MoveFileAsync(
            string srcDirPath,
            string dstDirPath,
            string fileExtention,
            bool? createYmdFolder,
            bool? saveSubFolder,
            bool? copyMode,
            bool? overwite,
            bool? seqNumberAdd)
        {
            return Task.Factory.StartNew(() =>
            {
                // 移動先フォルダの作成
                var dstDirAddPath = CreateDstFolder(dstDirPath, createYmdFolder);
                if (dstDirAddPath == null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Label_Status.Content = $"移動先フォルダが作成できませんでした。移動先->{dstDirAddPath}";
                    }));
                }

                // ファイルを移動する
                MoveFiles(srcDirPath, dstDirAddPath, fileExtention, saveSubFolder, copyMode, overwite, seqNumberAdd);

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Label_Status.Content = $"ファイルの移動完了 {srcDirPath} -> {dstDirAddPath}";
                }));

                // 移動完了時のサウンド再生
                PlayCompleteSoundAsync();
            });
        }

        /// <summary>
        /// 移動先のフォルダ作成
        /// </summary>
        /// <param name="dstDirPath"></param>
        /// <param name="createYmdFolder"></param>
        /// <returns></returns>
        private string CreateDstFolder(string dstDirPath, bool? createYmdFolder)
        {
            if (!(createYmdFolder ?? false))
            {
                if (!Directory.Exists(dstDirPath))
                {
                    try
                    {
                        Directory.CreateDirectory(dstDirPath);
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                }
                return dstDirPath;
            }

            try
            {
                var index = 0;
                var dstDirAddPath = Path.Combine(dstDirPath, DateTime.Now.ToString("yyyyMMdd"));
                // 既に現在日付でフォルダが存在する場合は連番で探す
                while (index < 100 && Directory.Exists(dstDirAddPath))
                {
                    index++;
                    dstDirAddPath = Path.Combine(dstDirPath, $"{DateTime.Now.ToString("yyyyMMdd")}_{index:00}");
                }
                Directory.CreateDirectory(dstDirAddPath);

                return dstDirAddPath;
            }
            catch (IOException)
            {
                return null;
            }
        }

        /// <summary>
        /// ファイルを移動する
        /// </summary>
        /// <param name="srcDirPath"></param>
        /// <param name="dstDirAddDate"></param>
        /// <param name="fileExtention"></param>
        /// <param name="saveSubFolder"></param>
        /// <param name="copyMode"></param>
        /// <param name="overwite"></param>
        /// <param name="seqNumberAdd"></param>
        private void MoveFiles(
            string srcDirPath,
            string dstDirAddDate,
            string fileExtention,
            bool? saveSubFolder,
            bool? copyMode,
            bool? overwite,
            bool? seqNumberAdd)
        {
            var isOverWrite = overwite ?? false;

            // ファイル列挙
            foreach (var srcFilePath in EnumerateTargetFiles(srcDirPath, fileExtention))
            {
                // 移動先ファイル名の生成
                string dstFilePath = null;
                if (saveSubFolder ?? false)
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
                if (!isOverWrite && (seqNumberAdd ?? false) && File.Exists(dstFilePath))
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
                if (!isOverWrite && File.Exists(dstFilePath))
                {
                    continue;
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Label_Status.Content = $"ファイルを{(copyMode ?? false ? "コピー中" : "移動中")}...{srcFilePath} -> {dstFilePath}";
                }));

                // ファイルの移動・コピー
                if (copyMode ?? false)
                {
                    // ファイルのコピー
                    File.Copy(srcFilePath, dstFilePath, isOverWrite);
                }
                else
                {
                    // ファイルの移動
                    File.Move(srcFilePath, dstFilePath, isOverWrite);
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
                using (var soundStream = Properties.Resources.nc122233)
                {
                    // 同期再生
                    using (var player = new SoundPlayer(soundStream))
                    {
                        player.PlaySync();
                    }
                }
            });
        }

    }
}
