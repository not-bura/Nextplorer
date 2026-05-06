using MessagePack;
using System.IO;

namespace Nextplorer.Domain
{
    public sealed class Config
    {
        private const string PATH = "config";

        private static string s_path;

        public required Vector2 Position { get; set; }
        public required Vector2 Size { get; set; }
        public required double Scale { get; set; }

        [MessagePackObject(true)]
        public struct SerializeObject
        {
            public required Vector2 Position { get; set; }
            public required Vector2 Size { get; set; }
            public required double Scale { get; set; }
        }

        private Config()
        {
        }

        private static Config Default()
        {
            return new Config
            {
                Position = new(0, 0),
                Size = new(800.0, 450.0),
                Scale = 1.0,
            };
        }

        private SerializeObject ToMessagePackObject()
        {
            return new SerializeObject()
            {
                Position = Position,
                Size = Size,
                Scale = Scale,
            };
        }

        public static Config Load(string directoryPath)
        {
            var _path = Path.Combine(directoryPath, PATH);
            if (File.Exists(_path))
            {
                SerializeObject _deserialize;
                using (var _fs = File.OpenRead(_path))
                {
                    _deserialize = MessagePackSerializer.Deserialize<SerializeObject>(_fs);
                }

                var _result = new Config
                {
                    Position = _deserialize.Position,
                    Size = _deserialize.Size,
                    Scale = _deserialize.Scale,
                };

                s_path = _path;
                return _result;
            }

            var _data = Default();
            var _serialize = _data.ToMessagePackObject();

            using (var _fs = File.Create(_path))
            {
                MessagePackSerializer.Serialize(_fs, _serialize);
            }

            s_path = _path;
            return _data;
        }

        public void Save()
        {
            var _serialize = ToMessagePackObject();

            var _path = s_path;
            if (File.Exists(_path))
            {
                using (var _fs = File.OpenWrite(_path))
                {
                    MessagePackSerializer.Serialize(_fs, _serialize);
                }

                return;
            }

            using (var _fs = File.Create(_path))
            {
                MessagePackSerializer.Serialize(_fs, _serialize);
            }
        }
    }
}
