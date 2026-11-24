using UnityEngine;
using System.IO;

namespace SecondSpring.Save
{
    public sealed class UnitySaveSerializer : ISaveSerializer
    {
        public string Serialize(GameSaveData data) => JsonUtility.ToJson(data, true);
        public GameSaveData Deserialize(string json) => JsonUtility.FromJson<GameSaveData>(json);
    }

    public static class UnitySaveStorage
    {
        public static SaveManager CreateDefault() =>
            new SaveManager(Path.Combine(Application.persistentDataPath, "game-save.json"), new UnitySaveSerializer());
    }
}
