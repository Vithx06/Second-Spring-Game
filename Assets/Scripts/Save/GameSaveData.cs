using System;

namespace SecondSpring.Save
{
    // Dữ liệu của từng phân hệ sẽ được bổ sung khi thống nhất cách xuất và khôi phục.
    [Serializable]
    public sealed class SaveSection
    {
        public string id;
        public string json;
    }

    [Serializable]
    public sealed class GameSaveData
    {
        public const int CurrentVersion = 1;
        public int version;
        public SaveSection[] sections;
    }

    public interface ISaveSerializer
    {
        string Serialize(GameSaveData data);
        GameSaveData Deserialize(string json);
    }
}
