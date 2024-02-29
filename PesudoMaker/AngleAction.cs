using System;
using ADOFAI.Editor.Actions;
using HarmonyLib;

namespace PesudoMaker {
    public class AngleAction : EditorAction {

        private static bool useAbsoluteArbitraryAngle = (bool) typeof(scnEditor).GetField("useAbsoluteArbitraryAngle", AccessTools.all).GetValue(ADOBase.editor);
        private KeySetting _keySetting;

        public AngleAction(KeySetting keySetting) {
            _keySetting = keySetting;
        }

        public override EditorTabKey sectionKey => EditorTabKey.None;

        public override void Execute(scnEditor editor) {
            if(editor.levelData.isOldLevel) return;
            float num = _keySetting.Angle;
            num = 180f - num;
            if(editor.selectedFloors[0].isCCW)
                num *= -1f;
            char chara = '£';
            if (!useAbsoluteArbitraryAngle) {
                if(editor.selectedFloors[0].floatDirection == 999.0) num += editor.floors[Math.Max(0, editor.selectedFloors[0].seqID - 1)].floatDirection;
                else num += editor.selectedFloors[0].floatDirection;
            }
            editor.CreateFloorWithCharOrAngle(num, chara);
            editor.CreateFloorWithCharOrAngle(999f, '!', fullSpin: true);
        }
    }
}