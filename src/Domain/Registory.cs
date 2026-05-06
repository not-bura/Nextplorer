using System.IO;

namespace Nextplorer.Domain
{
    public sealed class Registory
    {
        public static Config Config { get; }
        private static Temporary s_temporary;

        static Registory()
        {
            var _path = CheckPath();

            Config = Config.Load(_path);
            s_temporary = Temporary.Load(_path);
        }

        public static void Save()
        {

        }

        private static string CheckPath()
        {
            var _sourcePath = Environment.ProcessPath;
            var _name = Path.GetFileNameWithoutExtension(_sourcePath);

            var _directory = Path.GetDirectoryName(_sourcePath);

            var _destinationPath = Path.Combine(_directory!, _name!);

            if (Directory.Exists(_destinationPath))
            {
                return _destinationPath;
            }

            Directory.CreateDirectory(_destinationPath);
            return _destinationPath;
        }
    }

    public sealed class Temporary
    {
        public static Temporary Default()
        {
            return new()
            {

            };
        }

        public static Temporary Load(string directoryPath)
        {
            return Default();
        }
    }
}
