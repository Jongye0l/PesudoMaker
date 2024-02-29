using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PesudoMaker {
    public class Settings {
        private static readonly string SettingPath = Path.Combine(Main.ModEntry.Path, "Settings.json");
        public static Settings Instance;
        public List<KeySetting> KeySettings = new List<KeySetting>();
        
        public static Settings CreateInstance() {
            Instance = File.Exists(SettingPath) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingPath)) : new Settings();
            return Instance;
        }

        public void Save() {
            File.WriteAllText(SettingPath, JsonConvert.SerializeObject(Instance, Formatting.Indented));
        }
    }
}