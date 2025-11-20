using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Editor.UGUI
{
    public class CreatLanguage
    {
        /// <summary>
        /// 修改创建UIText时的一些默认值
        /// </summary>
        [MenuItem("GameObject/UI/LanguageText")]
        public static void CreateText()
        {
            if (Selection.activeObject && Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("LanguageText", typeof(LanguageText));
                LanguageText text = go.GetComponent<LanguageText>();
                text.raycastTarget = false;
                text.fontSize = 70;
                text.alignment = TextAnchor.MiddleCenter;
                //text.horizontalOverflow = HorizontalWrapMode.Overflow;
                //text.verticalOverflow = VerticalWrapMode.Overflow;
                // 查找并设置默认字体
                string[] guids = AssetDatabase.FindAssets("HirukoPro-Black");
                if (guids.Length > 0)
                {
                    string fontPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    Font defaultFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
                    if (defaultFont != null)
                    {
                        text.font = defaultFont;
                    }
                }
                go.transform.SetParent(Selection.activeTransform);
                go.transform.localScale = Vector3.one;
                RectTransform rectTransform = go.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(300, 100);
                rectTransform.anchoredPosition = Vector2.zero;
                for (int i = 0; i < 2; i++)
                {
                    Outline outline = go.AddComponent<Outline>();
                    outline.effectColor = new Color(0, 0, 0, 1);
                    outline.effectDistance = new Vector2(2, -2);
                }
            }
        }
        [MenuItem("GameObject/UI/Scale Button")]
        public static void CreateButton()
        {
            if (Selection.activeObject && Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                // 创建按钮
                GameObject buttonGo = new GameObject("ScaleButton", typeof(Image), typeof(Button));

                // 设置按钮组件
                Button button = buttonGo.GetComponent<Button>();
                button.transition = Selectable.Transition.None;  // 设置transition为None

                // 设置图片组件
                Image image = buttonGo.GetComponent<Image>();
                image.raycastTarget = true;

                // 创建文本对象
                GameObject textGo = new GameObject("LanguageText", typeof(LanguageText));
                LanguageText text = textGo.GetComponent<LanguageText>();
                text.raycastTarget = false;
                text.fontSize = 70;
                text.alignment = TextAnchor.MiddleCenter;
                //text.horizontalOverflow = HorizontalWrapMode.Overflow;
                //text.verticalOverflow = VerticalWrapMode.Overflow;
                // 查找并设置默认字体
                string[] guids = AssetDatabase.FindAssets("HirukoPro-Black");
                if (guids.Length > 0)
                {
                    string fontPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    Font defaultFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
                    if (defaultFont != null)
                    {
                        text.font = defaultFont;
                    }
                }

                // 设置按钮的RectTransform
                buttonGo.transform.SetParent(Selection.activeTransform);
                buttonGo.transform.localScale = Vector3.one;
                RectTransform buttonRect = buttonGo.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(300, 100);
                buttonRect.anchoredPosition = Vector2.zero;

                // 设置文本的父对象和RectTransform
                textGo.transform.SetParent(buttonGo.transform);
                textGo.transform.localScale = Vector3.one;
                RectTransform textRect = textGo.GetComponent<RectTransform>();
                textRect.sizeDelta = new Vector2(300, 100);
                textRect.anchoredPosition = Vector2.zero;

                // 添加描边效果
                for (int i = 0; i < 2; i++)
                {
                    Outline outline = textGo.AddComponent<Outline>();
                    outline.effectColor = new Color(0, 0, 0, 1);
                    outline.effectDistance = new Vector2(2, -2);
                }

                // 选中新创建的按钮
                Selection.activeGameObject = buttonGo;
            }
        }
    }
}