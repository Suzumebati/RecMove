using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RecMove
{
    class ShNotifyManager
    {
        public const int MAX_PATH = 260;

        #region "Enum宣言"

        public enum WM : int
        {
            WM_DEVICECHANGE = 0x0219,
            WM_SHNOTIFY = 0x0401
        }

        public enum CSIDL
        {
            /// <summary>
            /// Desktop
            /// </summary>
            CSIDL_DESKTOP = 0x0000,
            /// <summary>
            /// Internet Explorer (icon on desktop)
            /// </summary>
            CSIDL_INTERNET = 0x0001,
            /// <summary>
            /// Start Menu\Programs
            /// </summary>
            CSIDL_PROGRAMS = 0x0002,
            /// <summary>
            /// My Computer\Control Panel
            /// </summary>
            CSIDL_CONTROLS = 0x0003,
            /// <summary>
            /// My Computer\Printers
            /// </summary>
            CSIDL_PRINTERS = 0x0004,
            /// <summary>
            /// My Documents
            /// </summary>
            CSIDL_PERSONAL = 0x0005,
            /// <summary>
            /// user name\Favorites
            /// </summary>
            CSIDL_FAVORITES = 0x0006,
            /// <summary>
            /// Start Menu\Programs\Startup
            /// </summary>
            CSIDL_STARTUP = 0x0007,
            /// <summary>
            /// user name\Recent
            /// </summary>
            CSIDL_RECENT = 0x0008,
            /// <summary>
            /// user name\SendTo
            /// </summary>
            CSIDL_SENDTO = 0x0009,
            /// <summary>
            /// desktop\Recycle Bin
            /// </summary>
            CSIDL_BITBUCKET = 0x000a,
            /// <summary>
            /// user name\Start Menu
            /// </summary>
            CSIDL_STARTMENU = 0x000b,
            /// <summary>
            /// logical "My Documents" desktop icon
            /// </summary>
            CSIDL_MYDOCUMENTS = 0x000c,
            /// <summary>
            /// "My Music" folder
            /// </summary>
            CSIDL_MYMUSIC = 0x000d,
            /// <summary>
            /// "My Videos" folder
            /// </summary>
            CSIDL_MYVIDEO = 0x000e,
            /// <summary>
            /// user name\Desktop
            /// </summary>
            CSIDL_DESKTOPDIRECTORY = 0x0010,
            /// <summary>
            /// My Computer
            /// </summary>
            CSIDL_DRIVES = 0x0011,
            /// <summary>
            /// Network Neighborhood (My Network Places)
            /// </summary>
            CSIDL_NETWORK = 0x0012,
            /// <summary>
            /// user name>nethood
            /// </summary>
            CSIDL_NETHOOD = 0x0013,
            /// <summary>
            /// windows\fonts
            /// </summary>
            CSIDL_FONTS = 0x0014,
            CSIDL_TEMPLATES = 0x0015,
            /// <summary>
            /// All Users\Start Menu
            /// </summary>
            CSIDL_COMMON_STARTMENU = 0x0016,
            /// <summary>
            /// All Users\Start Menu\Programs
            /// </summary>
            CSIDL_COMMON_PROGRAMS = 0X0017,
            /// <summary>
            /// All Users\Startup
            /// </summary>
            CSIDL_COMMON_STARTUP = 0x0018,
            /// <summary>
            /// All Users\Desktop
            /// </summary>
            CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,
            /// <summary>
            /// user name\Application Data
            /// </summary>
            CSIDL_APPDATA = 0x001a,
            /// <summary>
            /// user name\PrintHood
            /// </summary>
            CSIDL_PRINTHOOD = 0x001b,
            /// <summary>
            /// user name\Local Settings\Applicaiton Data (non roaming)
            /// </summary>
            CSIDL_LOCAL_APPDATA = 0x001c,
            /// <summary>
            /// non localized startup
            /// </summary>
            CSIDL_ALTSTARTUP = 0x001d,
            /// <summary>
            /// non localized common startup
            /// </summary>
            CSIDL_COMMON_ALTSTARTUP = 0x001e,
            CSIDL_COMMON_FAVORITES = 0x001f,
            CSIDL_INTERNET_CACHE = 0x0020,
            CSIDL_COOKIES = 0x0021,
            CSIDL_HISTORY = 0x0022,
            /// <summary>
            /// All Users\Application Data
            /// </summary>
            CSIDL_COMMON_APPDATA = 0x0023,
            /// <summary>
            /// GetWindowsDirectory()
            /// </summary>
            CSIDL_WINDOWS = 0x0024,
            /// <summary>
            /// GetSystemDirectory()
            /// </summary>
            CSIDL_SYSTEM = 0x0025,
            /// <summary>
            /// C:\Program Files
            /// </summary>
            CSIDL_PROGRAM_FILES = 0x0026,
            /// <summary>
            /// C:\Program Files\My Pictures
            /// </summary>
            CSIDL_MYPICTURES = 0x0027,
            /// <summary>
            /// USERPROFILE
            /// </summary>
            CSIDL_PROFILE = 0x0028,
            /// <summary>
            /// x86 system directory on RISC
            /// </summary>
            CSIDL_SYSTEMX86 = 0x0029,
            /// <summary>
            /// x86 C:\Program Files on RISC
            /// </summary>
            CSIDL_PROGRAM_FILESX86 = 0x002a,
            /// <summary>
            /// C:\Program Files\Common
            /// </summary>
            CSIDL_PROGRAM_FILES_COMMON = 0x002b,
            /// <summary>
            /// x86 Program Files\Common on RISC
            /// </summary>
            CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c,
            /// <summary>
            /// All Users\Templates
            /// </summary>
            CSIDL_COMMON_TEMPLATES = 0x002d,
            /// <summary>
            /// All Users\Documents
            /// </summary>
            CSIDL_COMMON_DOCUMENTS = 0x002e,
            /// <summary>
            /// All Users\Start Menu\Programs\Administrative Tools
            /// </summary>
            CSIDL_COMMON_ADMINTOOLS = 0x002f,
            /// <summary>
            /// user name\Start Menu\Programs\Administrative Tools
            /// </summary>
            CSIDL_ADMINTOOLS = 0x0030,
            /// <summary>
            /// Network and Dial-up Connections
            /// </summary>
            CSIDL_CONNECTIONS = 0x0031,
            /// <summary>
            /// All Users\My Music
            /// </summary>
            CSIDL_COMMON_MUSIC = 0x0035,
            /// <summary>
            /// All Users\My Pictures
            /// </summary>
            CSIDL_COMMON_PICTURES = 0x0036,
            /// <summary>
            /// All Users\My Video
            /// </summary>
            CSIDL_COMMON_VIDEO = 0x0037,
            /// <summary>
            /// Resource Direcotry
            /// </summary>
            CSIDL_RESOURCES = 0x0038,
            /// <summary>
            /// Localized Resource Direcotry
            /// </summary>
            CSIDL_RESOURCES_LOCALIZED = 0x0039,
            /// <summary>
            /// Links to All Users OEM specific apps
            /// </summary>
            CSIDL_COMMON_OEM_LINKS = 0x003a,
            /// <summary>
            /// USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
            /// </summary>
            CSIDL_CDBURN_AREA = 0x003b,
            /// <summary>
            /// Computers Near Me (computered from Workgroup membership)
            /// </summary>
            CSIDL_COMPUTERSNEARME = 0x003d,
            /// <summary>
            /// combine with CSIDL_ value to force folder creation in SHGetFolderPath()
            /// </summary>
            CSIDL_FLAG_CREATE = 0x8000,
            /// <summary>
            /// combine with CSIDL_ value to return an unverified folder path
            /// </summary>
            CSIDL_FLAG_DONT_VERIFY = 0x4000,
            /// <summary>
            /// combine with CSIDL_ value to insure non-alias versions of the pidl
            /// </summary>
            CSIDL_FLAG_NO_ALIAS = 0x1000,
            /// <summary>
            /// combine with CSIDL_ value to indicate per-user init (eg. upgrade)
            /// </summary>
            CSIDL_FLAG_PER_USER_INIT = 0x0800,
            /// <summary>
            /// mask for all possible 
            /// </summary>
            CSIDL_FLAG_MASK = 0xFF00,
        }

        public enum SHGetFolderLocationReturnValues : uint
        {
            /// <summary>
            /// Success
            /// </summary>
            S_OK = 0x00000000,
            /// <summary>
            /// The CSIDL in nFolder is valid but the folder does not exist
            /// </summary>
            S_FALSE = 0x00000001,
            /// <summary>
            /// The CSIDL in nFolder is not valid
            /// </summary>
            E_INVALIDARG = 0x80070057
        }

        public enum SHCNF
        {
            SHCNF_IDLIST = 0x0000,
            SHCNF_PATHA = 0x0001,
            SHCNF_PRINTERA = 0x0002,
            SHCNF_DWORD = 0x0003,
            SHCNF_PATHW = 0x0005,
            SHCNF_PRINTERW = 0x0006,
            SHCNF_TYPE = 0x00FF,
            SHCNF_FLUSH = 0x1000,
            SHCNF_FLUSHNOWAIT = 0x2000
        }

        public enum SHCNE : uint
        {
            SHCNE_RENAMEITEM = 0x00000001,
            SHCNE_CREATE = 0x00000002,
            SHCNE_DELETE = 0x00000004,
            SHCNE_MKDIR = 0x00000008,
            SHCNE_RMDIR = 0x00000010,
            SHCNE_MEDIAINSERTED = 0x00000020,
            SHCNE_MEDIAREMOVED = 0x00000040,
            SHCNE_DRIVEREMOVED = 0x00000080,
            SHCNE_DRIVEADD = 0x00000100,
            SHCNE_NETSHARE = 0x00000200,
            SHCNE_NETUNSHARE = 0x00000400,
            SHCNE_ATTRIBUTES = 0x00000800,
            SHCNE_UPDATEDIR = 0x00001000,
            SHCNE_UPDATEITEM = 0x00002000,
            SHCNE_SERVERDISCONNECT = 0x00004000,
            SHCNE_UPDATEIMAGE = 0x00008000,
            SHCNE_DRIVEADDGUI = 0x00010000,
            SHCNE_RENAMEFOLDER = 0x00020000,
            SHCNE_FREESPACE = 0x00040000,
            SHCNE_EXTENDED_EVENT = 0x04000000,
            SHCNE_ASSOCCHANGED = 0x08000000,
            SHCNE_DISKEVENTS = 0x0002381F,
            SHCNE_GLOBALEVENTS = 0x0C0581E0,
            SHCNE_ALLEVENTS = 0x7FFFFFFF,
            SHCNE_INTERRUPT = 0x80000000,
        }

        #endregion

        #region "構造体定義"

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHChangeNotifyEntry
        {
            public IntPtr pIdl;
            [MarshalAs(UnmanagedType.Bool)] public Boolean Recursively;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHNOTIFYSTRUCT
        {
            public IntPtr dwItem1;
            public IntPtr dwItem2;
        };

        #endregion

        #region "WindowsAPI"

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern uint SHGetSpecialFolderLocation(
                                                        IntPtr hWnd,
                                                        CSIDL nFolder,
                                                        out IntPtr Pidl);

        [DllImport("shell32.dll", EntryPoint = "#2", CharSet = CharSet.Auto)]
        private static extern uint SHChangeNotifyRegister(
                                                            IntPtr hWnd,
                                                            SHCNF fSources,
                                                            SHCNE fEvents,
                                                            int wMsg,
                                                            int cEntries,
                                                            ref SHChangeNotifyEntry pFsne);

        [DllImport("shell32.dll", EntryPoint = "#4", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean SHChangeNotifyUnregister(ulong hNotify);

        //[DllImport("Shell32.Dll", CharSet = CharSet.Auto)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern Boolean SHGetPathFromIDList(
        //                                                [In] IntPtr pidl,
        //                                                [In, Out, MarshalAs(UnmanagedType.LPTStr)] String pszPath);
        [DllImport("Shell32.Dll", CharSet = CharSet.Auto)]
        private static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

        #endregion

        /// <summary>
        /// 通知設定ID
        /// </summary>
        private uint notifyid;

        /// <summary>
        /// シェルのチェンジイベント受付を登録する
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="folderID"></param>
        /// <param name="recursively"></param>
        /// <returns></returns>
        public ulong RegisterChangeNotify(IntPtr hWnd, CSIDL folderID, bool recursively)
        {
            if (notifyid != 0) return (0);
            SHChangeNotifyEntry changeentry = new SHChangeNotifyEntry();
            changeentry.pIdl = GetPidlFromFolderID(hWnd, folderID);
            changeentry.Recursively = recursively;
            notifyid = SHChangeNotifyRegister(
                hWnd,
                SHCNF.SHCNF_TYPE | SHCNF.SHCNF_IDLIST,
                SHCNE.SHCNE_ALLEVENTS | SHCNE.SHCNE_INTERRUPT,
                (int)WM.WM_SHNOTIFY,
                1,
                ref changeentry);
            return (notifyid);
        }

        /// <summary>
        /// シェルのチェンジイベント受付を解除する
        /// </summary>
        /// <returns></returns>
        public Boolean UnregisterChangeNotify()
        {
            if (notifyid == 0) return (false);
            if (SHChangeNotifyUnregister(notifyid))
            {
                notifyid = 0;
                return (true);
            }
            return (false);
        }

        /// <summary>
        /// ドライブのマウント系のメッセージか判断する
        /// </summary>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public bool IsDriveMount(IntPtr lParam)
        {
            SHCNE notification = (SHCNE)(int)lParam;
            switch (notification)
            {
                case SHCNE.SHCNE_DRIVEADD:
                    return true;
                case SHCNE.SHCNE_MEDIAINSERTED:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// マウントされたドライブパスを返却する
        /// </summary>
        /// <param name="wParam"></param>
        /// <returns></returns>
        public string GetDrivePath(IntPtr wParam)
        {
            SHNOTIFYSTRUCT shNotify = (SHNOTIFYSTRUCT)Marshal.PtrToStructure(wParam, typeof(SHNOTIFYSTRUCT));
            return GetPathFromPidl(shNotify.dwItem1);
        }

        /// <summary>
        /// 該当フォルダのpIdlを取得する
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static IntPtr GetPidlFromFolderID(IntPtr hWnd, CSIDL id)
        {
            IntPtr pIdl = IntPtr.Zero;
            SHGetFolderLocationReturnValues res = (SHGetFolderLocationReturnValues)
                SHGetSpecialFolderLocation(
                hWnd,
                id,
                out pIdl);
            return (pIdl);
        }

        /// <summary>
        /// pIdlからパスを取得する
        /// </summary>
        /// <param name="pIdl"></param>
        /// <returns></returns>
        private static string GetPathFromPidl(IntPtr pIdl)
        {
            StringBuilder sb = new StringBuilder(MAX_PATH);
            IntPtr bufferAddress = Marshal.AllocCoTaskMem(MAX_PATH);
            try
            {
                if (!SHGetPathFromIDList(pIdl, bufferAddress))
                {
                    return "";
                }
                sb.Append(Marshal.PtrToStringAuto(bufferAddress));
            }
            finally
            {
                Marshal.FreeCoTaskMem(bufferAddress);
            }
            return sb.ToString();
        }
    }
}
