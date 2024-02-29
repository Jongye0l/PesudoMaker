using System;
using System.Collections.Generic;
using System.Reflection;
using ADOFAI.Editor;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace PesudoMaker {
    public class Main {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static UnityModManager.ModEntry ModEntry;
        private static Harmony _harmony;
        public static bool Enabled;
        public static Settings Settings;
        private static Assembly _assembly;
        private static KeySetting _register;
        private static readonly KeyCode[] KeyCodes = (KeyCode[]) Enum.GetValues(typeof(KeyCode));
        public static EditorKeybindManager KeybindManager;

        private static readonly ISet<KeyCode> SkippedKeys = new HashSet<KeyCode> {
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.Mouse2,
            KeyCode.Mouse3,
            KeyCode.Mouse4,
            KeyCode.Mouse5,
            KeyCode.Mouse6,
            KeyCode.Escape
        };

        public static void Setup(UnityModManager.ModEntry modEntry) {
            Logger = modEntry.Logger;
            ModEntry = modEntry;
            Settings = Settings.CreateInstance();
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnUpdate = OnUpdate;
            _assembly = Assembly.GetExecutingAssembly();
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            Enabled = value;
            if(value) {
                _harmony = new Harmony(modEntry.Info.Id);
                _harmony.PatchAll(_assembly);
            } else _harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry) {
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("생성")) Settings.KeySettings.Add(new KeySetting());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for(int i = 0; i < Settings.KeySettings.Count; i++) {
                KeySetting keySetting = Settings.KeySettings[i];
                GUILayout.BeginHorizontal();
                AddSettingAngle(ref keySetting.Angle, 1, ref keySetting.AngleString, "각도");
                if(GUILayout.Button(_register == keySetting ? "키를 입력하세요" : ((KeyCode) keySetting.KeyCode).ToString()))
                    _register = _register == keySetting ? null : keySetting;
                if(GUILayout.Button("제거")) {
                    Settings.KeySettings.Remove(keySetting);
                    i--;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float deltaTime) {
            if(_register == null) return;
            foreach(KeyCode keyCode in KeyCodes) {
                if(!Input.GetKeyDown(keyCode) || SkippedKeys.Contains(keyCode)) continue;
                RemoveRegister(_register);
                _register.KeyCode = (int) keyCode;
                AddRegister(_register);
                _register = null;
                break;
            }
        }

        private static void AddSettingAngle(ref int value, int defaultValue, ref string valueString, string text) {
            GUILayout.Label(text);
            GUILayout.Space(4f);
            if(valueString == null) valueString = value.ToString();
            valueString = GUILayout.TextField(valueString, GUILayout.Width(50));
            int resultInt;
            try {
                resultInt = valueString.IsNullOrEmpty() ? defaultValue : int.Parse(valueString);
                if(resultInt < 0) {
                    resultInt = 0;
                    valueString = "0";
                }
            } catch (FormatException) {
                resultInt = defaultValue;
                valueString = defaultValue.ToString();
            }
            if(resultInt != value) {
                value = resultInt;
                Settings.Save();
            }
        }

        public static void AddRegister(KeySetting keySetting) {
            if(!ADOBase.editor) return;
            if(KeybindManager == null) KeybindManager = (EditorKeybindManager) typeof(scnEditor).GetField("keybindManager", AccessTools.all).GetValue(ADOBase.editor);
            KeybindManager.RegisterKeybind(new EditorKeybind((KeyCode) keySetting.KeyCode), new AngleAction(keySetting));
        }

        public static void RemoveRegister(KeySetting keySetting) {
            if(!ADOBase.editor || KeybindManager == null) return;
            KeybindManager.UnregisterKeybind(new EditorKeybind((KeyCode) keySetting.KeyCode));
        }
    }
}