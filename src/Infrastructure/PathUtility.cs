using System.IO;

namespace Nextplorer.Infrastructure
{
    public static class PathUtility
    {
        public static string Home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static string Documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string Download = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        public static string Pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public static string Muisic = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        //public static string Trash = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        public static string Videos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        public static string[] GetDrives()
        {
            var _drives = DriveInfo.GetDrives();
            var _paths = new string[_drives.Length];
            for (int i = 0; i < _drives.Length; i++)
            {
                _paths[i] = _drives[i].Name;
            }
            return _paths;
        }
    }
}
