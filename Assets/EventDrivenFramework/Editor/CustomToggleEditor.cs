using EventDrivenFramework.Extends;
using UnityEditor;
using UnityEngine;

namespace EventDrivenFramework.Editor
{
    [CustomEditor(typeof(CustomToggle))]
    public class CustomToggleEditor : UnityEditor.UI.ToggleEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CustomToggle component = (CustomToggle) target;

            component.Color = EditorGUILayout.ColorField("Color", component.Color);
            component.OnSprite = (Sprite) EditorGUILayout.ObjectField("On Sprite"
                , component.OnSprite, typeof(Sprite), false);
            component.OffSprite = (Sprite) EditorGUILayout.ObjectField("Off Sprite"
                , component.OffSprite, typeof(Sprite), false);
        }
    }
}