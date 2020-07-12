using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace RecMove
{
    static class Utility
    {

        /// <summary>
        /// マンメンミ
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static public Process ShellExecute(string filePath)
        {
            // 関連付けで該当ファイルを実行する
            var process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.UseShellExecute = true;
            process.Start();

            return process;
        }

        /// <summary>
        /// 移動完了サウンドの再生
        /// </summary>
        /// <returns></returns>
        static public Task PlayCompleteSoundAsync()
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

    }
}
