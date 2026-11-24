using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SecondSpring.Save
{
    public sealed class SaveManager
    {
        private readonly string _savePath;
        private readonly ISaveSerializer _serializer;
        public string BackupPath => _savePath + ".bak";

        public SaveManager(string savePath, ISaveSerializer serializer)
        {
            if (string.IsNullOrWhiteSpace(savePath)) throw new ArgumentException("Save path is required.", nameof(savePath));
            _savePath = Path.GetFullPath(savePath);
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        // Bên điều phối phải ngừng cập nhật trong lúc thu thập dữ liệu và lưu.
        public void Save(GameSaveData data)
        {
            Validate(data);
            string json = _serializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(json)) throw new InvalidDataException("Empty serialized save.");
            string directory = Path.GetDirectoryName(_savePath);
            Directory.CreateDirectory(directory);
            // File tạm nằm cùng thư mục để thao tác thay thế không vượt ổ đĩa.
            string tempPath = Path.Combine(directory, "." + Path.GetFileName(_savePath) + "." + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                byte[] bytes = new UTF8Encoding(false).GetBytes(json);
                using (var stream = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }
                if (File.Exists(_savePath)) File.Replace(tempPath, _savePath, BackupPath);
                else File.Move(tempPath, _savePath);
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }

        public GameSaveData Load() => LoadFrom(_savePath);
        public GameSaveData LoadBackup() => LoadFrom(BackupPath);

        private GameSaveData LoadFrom(string path)
        {
            string json = File.ReadAllText(path, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(json)) throw new InvalidDataException("Empty save file.");
            GameSaveData data;
            try { data = _serializer.Deserialize(json); }
            catch (Exception error) { throw new InvalidDataException("Cannot read save file.", error); }
            Validate(data);
            return data;
        }

        private static void Validate(GameSaveData data)
        {
            if (data == null || data.version != GameSaveData.CurrentVersion || data.sections == null || data.sections.Length == 0)
                throw new InvalidDataException("Invalid or unsupported save data.");
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (SaveSection section in data.sections)
                if (section == null || string.IsNullOrWhiteSpace(section.id) || string.IsNullOrWhiteSpace(section.json) || !ids.Add(section.id))
                    throw new InvalidDataException("Invalid or duplicate save section.");
        }
    }
}
