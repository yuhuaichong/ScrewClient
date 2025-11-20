using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(LanguageText), true)]
    [CanEditMultipleObjects]
    public class LanguageTextEditor : GraphicEditor
    {
        SerializedProperty m_Text;
        SerializedProperty m_FontData;
        SerializedProperty m_languageId;
        SerializedProperty m_EditorText;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
            m_languageId = serializedObject.FindProperty("m_languageId");
            m_EditorText = serializedObject.FindProperty("m_editorText");
        }

        public override void OnInspectorGUI()
        {
            var lan = target as LanguageText;
            serializedObject.Update();
            if (GUILayout.Button("语言包刷新"))
            {
                LanguageUtils.ReloadConfig();
            }
            GUILayout.Label("全局的语言设置");
            var lang = EditorGUILayout.EnumPopup(LanguageUtils.GetLanguage());
            if ((SystemLanguage_My)lang != LanguageUtils.GetLanguage())
            {
                LanguageUtils.SetLanguage((SystemLanguage_My)lang);
                LanguageUtils.SaveCache();
                var canvasScaler = lan.GetComponentInParent<CanvasScaler>();
                if (canvasScaler != null)
                {
                    var canvas = lan.GetComponentInParent<Canvas>();
                    if (canvas)
                    {
                        var lanTexts = canvas.GetComponentsInChildren<LanguageText>();
                        foreach (var text in lanTexts)
                        {
                            text.UpdateLanguage();
                            text.SetAllDirty();
                        }
                    }
                }
            }
            EditorGUILayout.PropertyField(m_languageId, new GUIContent("语言包ID"));
            EditorGUILayout.PropertyField(m_EditorText, new GUIContent("编辑器可见文本"));

            if (EditorGUILayout.Toggle("根据文本自动阿语排版", lan.AutoArabicByText) != lan.AutoArabicByText)
            {
                lan.AutoArabicByText = !lan.AutoArabicByText;
                lan.text = lan.BaseText;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);

            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();

            lan.UpdateLanguage();
        }
    }
}