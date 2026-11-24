using System;
using System.IO;
using System.Text.Json;
using SecondSpring.Save;

int checks = 0;
void Check(bool value, string label) { checks++; if (!value) throw new Exception(label); }
void Throws<T>(Action action) where T : Exception { checks++; try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
GameSaveData Data(string json) => new GameSaveData { version = 1, sections = new[] { new SaveSection { id = "clock", json = json } } };
string dir = Path.Combine(Path.GetTempPath(), "second-spring-test-" + Guid.NewGuid().ToString("N"));
try
{
    string path = Path.Combine(dir, "save.json");
    Directory.CreateDirectory(dir);
    var save = new SaveManager(path, new TestSerializer());
    Throws<FileNotFoundException>(() => save.Load());
    save.Save(Data("{\"day\":1}"));
    Check(save.Load().sections[0].json == "{\"day\":1}", "first save round trip");
    save.Save(Data("{\"day\":2}"));
    Check(save.Load().sections[0].json == "{\"day\":2}", "new save round trip");
    Check(save.LoadBackup().sections[0].json == "{\"day\":1}", "backup retains previous save");
    Throws<InvalidDataException>(() => save.Save(new GameSaveData { version = 999, sections = Data("{}").sections }));
    Check(save.Load().sections[0].json == "{\"day\":2}", "invalid data preserves current save");
    Throws<InvalidDataException>(() => save.Save(new GameSaveData { version = 1, sections = new[] { new SaveSection { id = "x", json = "{}" }, new SaveSection { id = "x", json = "{}" } } }));
    File.WriteAllText(path, "{invalid");
    Throws<InvalidDataException>(() => save.Load());
    Check(save.LoadBackup().sections[0].json == "{\"day\":1}", "backup remains available after corruption");
    File.WriteAllText(path, "{\"version\":2,\"sections\":[]}");
    Throws<InvalidDataException>(() => save.Load());
    Console.WriteLine($"Passed {checks} save checks.");
}
finally
{
    string resolved = Path.GetFullPath(dir);
    string allowed = Path.GetFullPath(Path.GetTempPath());
    if (!resolved.StartsWith(allowed, StringComparison.OrdinalIgnoreCase) || !Path.GetFileName(resolved).StartsWith("second-spring-test-", StringComparison.Ordinal))
        throw new InvalidOperationException("Unexpected test directory.");
    if (Directory.Exists(resolved)) Directory.Delete(resolved, true);
}

sealed class TestSerializer : ISaveSerializer
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions { IncludeFields = true };
    public string Serialize(GameSaveData data) => JsonSerializer.Serialize(data, _options);
    public GameSaveData Deserialize(string json) => JsonSerializer.Deserialize<GameSaveData>(json, _options);
}
