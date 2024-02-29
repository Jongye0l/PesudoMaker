using Newtonsoft.Json;

namespace PesudoMaker {
    public class KeySetting {
        public int KeyCode = 0;
        public int Angle = 1;
        [JsonIgnore] public string AngleString;
    }
}