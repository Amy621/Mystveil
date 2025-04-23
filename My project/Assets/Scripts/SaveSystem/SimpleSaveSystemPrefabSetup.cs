using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;

public class SimpleSaveSystemPrefabSetup : MonoBehaviour
{
    [MenuItem("Tools/Save System/Create Simple Save System")]
    public static void CreateSaveSystemPrefab()
    {
        // Create parent GameObject
        GameObject saveSystemRoot = new GameObject("SimpleSaveSystem");
        SimpleSaveSystem saveSystem = saveSystemRoot.AddComponent<SimpleSaveSystem>();
        
        // Create UI Canvas
        GameObject canvasObj = new GameObject("SaveMenuCanvas");
        canvasObj.transform.SetParent(saveSystemRoot.transform);
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create Save Menu Panel
        GameObject panelObj = new GameObject("SaveMenuPanel");
        panelObj.transform.SetParent(canvasObj.transform);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(300f, 400f);
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);
        
        // Add SaveMenu component
        SaveMenu saveMenu = panelObj.AddComponent<SaveMenu>();
        
        // Create Title
        GameObject titleObj = CreateTextObject("TitleText", panelObj.transform, "Save/Load Game", 20);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0f, 150f);
        
        // Create Save Info Text
        GameObject infoObj = CreateTextObject("SaveInfoText", panelObj.transform, "No save file found.", 14);
        RectTransform infoRect = infoObj.GetComponent<RectTransform>();
        infoRect.anchoredPosition = new Vector2(0f, 80f);
        infoRect.sizeDelta = new Vector2(280f, 100f);
        
        // Create Save Button
        GameObject saveButtonObj = CreateButtonObject("SaveButton", panelObj.transform, "Save Game");
        RectTransform saveButtonRect = saveButtonObj.GetComponent<RectTransform>();
        saveButtonRect.anchoredPosition = new Vector2(0f, 0f);
        
        // Create Load Button
        GameObject loadButtonObj = CreateButtonObject("LoadButton", panelObj.transform, "Load Game");
        RectTransform loadButtonRect = loadButtonObj.GetComponent<RectTransform>();
        loadButtonRect.anchoredPosition = new Vector2(0f, -60f);
        
        // Create Close Button
        GameObject closeButtonObj = CreateButtonObject("CloseButton", panelObj.transform, "Close");
        RectTransform closeButtonRect = closeButtonObj.GetComponent<RectTransform>();
        closeButtonRect.anchoredPosition = new Vector2(0f, -120f);
        
        // Assign references
        saveMenu.SetSaveInfoText(infoObj.GetComponent<TextMeshProUGUI>());
        saveMenu.SetSaveButton(saveButtonObj.GetComponent<Button>());
        saveMenu.SetLoadButton(loadButtonObj.GetComponent<Button>());
        saveMenu.SetCloseButton(closeButtonObj.GetComponent<Button>());
        
        // Assign UI to SaveSystem
        saveSystem.SaveMenuUI = panelObj;
        
        // Hide panel by default
        panelObj.SetActive(false);
        
        // Create prefab
        string prefabPath = "Assets/Prefabs/SimpleSaveSystem.prefab";
        
        // Ensure directory exists
        if (!System.IO.Directory.Exists("Assets/Prefabs"))
        {
            System.IO.Directory.CreateDirectory("Assets/Prefabs");
        }
        
        // Create prefab
#if UNITY_2018_3_OR_NEWER
        // For Unity 2018.3 and newer
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(saveSystemRoot, prefabPath);
#else
        // For older Unity versions
        GameObject prefab = PrefabUtility.CreatePrefab(prefabPath, saveSystemRoot);
#endif
        
        Debug.Log("Save System prefab created at: " + prefabPath);
        
        // Cleanup
        DestroyImmediate(saveSystemRoot);
        
        // Select the created prefab
        Selection.activeObject = prefab;
    }
    
    private static GameObject CreateTextObject(string name, Transform parent, string text, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(200f, 30f);
        rectTransform.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;
        
        return textObj;
    }
    
    private static GameObject CreateButtonObject(string name, Transform parent, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);
        
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(200f, 40f);
        rectTransform.anchoredPosition = Vector2.zero;
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        button.colors = colors;
        
        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        RectTransform textRectTransform = textObj.AddComponent<RectTransform>();
        textRectTransform.anchorMin = new Vector2(0f, 0f);
        textRectTransform.anchorMax = new Vector2(1f, 1f);
        textRectTransform.sizeDelta = Vector2.zero;
        textRectTransform.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 16;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;
        
        return buttonObj;
    }
}
#endif 