using ADOFAI.Editor;
using HarmonyLib;

namespace PesudoMaker {
    [HarmonyPatch(typeof(scnEditor), "RegisterKeybinds")]
    public class EditorPatch {
        public static void Postfix(EditorKeybindManager ___keybindManager) {
            Main.KeybindManager = ___keybindManager;
            foreach(KeySetting keySetting in Main.Settings.KeySettings) Main.AddRegister(keySetting);
        }
    }
}