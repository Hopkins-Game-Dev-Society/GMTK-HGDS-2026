using BirthdayJobJam.Application;
using BirthdayJobJam.Core;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

internal static class BuildSessionExpiryUI
{
    private const string ScenePath = "Assets/Scenes/gameplay.unity";
    private const string ArialFontPath = "Assets/Fonts/arial/Arial SDF.asset";

    [MenuItem("Birthday Job Jam/Build Session Expiry UI")]
    public static void Build()
    {
        EditorSceneManager.OpenScene(ScenePath);

        ApplicationSignInPageView view = Object.FindAnyObjectByType<ApplicationSignInPageView>(FindObjectsInactive.Include);
        if (view == null)
            throw new System.InvalidOperationException("ApplicationSignInPageView not found in gameplay scene.");

        RectTransform shell = GameObject.Find("Application UI Shell")?.GetComponent<RectTransform>();
        if (shell == null)
            throw new System.InvalidOperationException("Application UI Shell not found in gameplay scene.");

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ArialFontPath);

        DestroyChild(shell, "Session Timer Text");
        DestroyChild(shell, "Session Expired Reauth Panel");

        TMP_Text timerText = CreateText("Session Timer Text", shell, font, "Session expires in 02:00", 22, FontStyles.Bold, TextAlignmentOptions.Right, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-104f, -205f), new Vector2(360f, 36f), new Color(0.45f, 0.05f, 0.05f, 1f));
        GameObject panel = BuildReauthPanel(shell, font, out TMP_Text titleText, out TMP_Text bodyText, out TMP_InputField codeInput, out TMP_Text errorText, out Button submitButton, out TMP_Text submitButtonText);

        SerializedObject serializedView = new SerializedObject(view);
        SetObject(serializedView, "sessionTimerText", timerText);
        SetFloat(serializedView, "sessionDurationSeconds", 120f);
        SetFloat(serializedView, "sessionSecondsRemaining", 120f);
        SetBool(serializedView, "sessionTimerRunning", false);
        SetObject(serializedView, "sessionExpiredReauthPanel", panel);
        SetObject(serializedView, "sessionExpiredTitleText", titleText);
        SetObject(serializedView, "sessionExpiredBodyText", bodyText);
        SetObject(serializedView, "sessionReauthInput", codeInput);
        SetObject(serializedView, "sessionReauthErrorText", errorText);
        SetObject(serializedView, "sessionReauthSubmitButton", submitButton);
        SetObject(serializedView, "sessionReauthSubmitButtonText", submitButtonText);
        serializedView.ApplyModifiedProperties();

        GameplayTimer gameplayTimer = Object.FindAnyObjectByType<GameplayTimer>(FindObjectsInactive.Include);
        if (gameplayTimer != null)
        {
            SerializedObject serializedTimer = new SerializedObject(gameplayTimer);
            SetFloat(serializedTimer, "durationSeconds", 600f);
            SetFloat(serializedTimer, "secondsRemaining", 600f);
            serializedTimer.ApplyModifiedProperties();
        }

        timerText.gameObject.SetActive(false);
        panel.SetActive(false);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        Debug.Log("Built session expiry UI and set gameplay timer to 10 minutes.");
    }

    private static GameObject BuildReauthPanel(
        RectTransform parent,
        TMP_FontAsset font,
        out TMP_Text titleText,
        out TMP_Text bodyText,
        out TMP_InputField codeInput,
        out TMP_Text errorText,
        out Button submitButton,
        out TMP_Text submitButtonText)
    {
        GameObject panel = CreateRect("Session Expired Reauth Panel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(610f, 330f));
        panel.transform.SetAsLastSibling();
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.98f, 0.96f, 0.9f, 1f);

        titleText = CreateText("Title Text", panel.transform, font, "Session expired, 2FA Required", 30, FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -34f), new Vector2(-56f, 48f), new Color(0.08f, 0.08f, 0.08f, 1f));
        bodyText = CreateText("Body Text", panel.transform, font, "For your security, Workbay has forgotten who you are. Enter the authentication code to unlock refresh.", 21, FontStyles.Normal, TextAlignmentOptions.TopLeft, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -92f), new Vector2(-72f, 74f), new Color(0.2f, 0.2f, 0.2f, 1f));
        codeInput = CreateInputField("2FA Input", panel.transform, font, "2FA code", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-110f, -188f), new Vector2(240f, 56f));
        submitButton = CreateButton("Verify Button", panel.transform, font, "Verify", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(150f, -188f), new Vector2(150f, 56f), new Color(0.13f, 0.42f, 0.86f, 1f), Color.white, 22);
        submitButtonText = submitButton.transform.Find("Label").GetComponent<TMP_Text>();
        errorText = CreateText("Error Text", panel.transform, font, "", 20, FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 36f), new Vector2(-72f, 48f), new Color(0.72f, 0.12f, 0.12f, 1f));

        return panel;
    }

    private static TMP_InputField CreateInputField(string name, Transform parent, TMP_FontAsset font, string placeholder, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
    {
        GameObject root = CreateRect(name, parent, anchorMin, anchorMax, pivot, position, size);
        Image image = root.AddComponent<Image>();
        image.color = new Color(0.88f, 0.88f, 0.84f, 1f);

        TMP_InputField input = root.AddComponent<TMP_InputField>();
        input.targetGraphic = image;
        input.textViewport = root.GetComponent<RectTransform>();
        input.textComponent = CreateText("Text", root.transform, font, "", 24, FontStyles.Normal, TextAlignmentOptions.Left, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(0f, -1f), new Vector2(-28f, -10f), new Color(0.12f, 0.12f, 0.12f, 1f));
        input.placeholder = CreateText("Placeholder", root.transform, font, placeholder, 24, FontStyles.Normal, TextAlignmentOptions.Left, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(0f, -1f), new Vector2(-28f, -10f), new Color(0.42f, 0.42f, 0.42f, 0.82f));
        input.characterLimit = 12;
        return input;
    }

    private static Button CreateButton(string name, Transform parent, TMP_FontAsset font, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, Color buttonColor, Color textColor, float fontSize)
    {
        GameObject root = CreateRect(name, parent, anchorMin, anchorMax, pivot, position, size);
        Image image = root.AddComponent<Image>();
        image.color = buttonColor;
        Button button = root.AddComponent<Button>();
        button.targetGraphic = image;
        CreateText("Label", root.transform, font, label, fontSize, FontStyles.Bold, TextAlignmentOptions.Center, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, textColor);
        return button;
    }

    private static TMP_Text CreateText(string name, Transform parent, TMP_FontAsset font, string text, float fontSize, FontStyles style, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, Color color)
    {
        GameObject root = CreateRect(name, parent, anchorMin, anchorMax, pivot, position, size);
        TextMeshProUGUI tmp = root.AddComponent<TextMeshProUGUI>();
        tmp.font = font;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = alignment;
        tmp.color = color;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        tmp.raycastTarget = false;
        return tmp;
    }

    private static GameObject CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
    {
        GameObject root = new GameObject(name, typeof(RectTransform));
        root.transform.SetParent(parent, false);
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return root;
    }

    private static void DestroyChild(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);
        if (child != null)
            Object.DestroyImmediate(child.gameObject);
    }

    private static void SetObject(SerializedObject serializedObject, string propertyName, Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null)
            throw new System.InvalidOperationException($"Serialized property not found: {propertyName}");

        property.objectReferenceValue = value;
    }

    private static void SetFloat(SerializedObject serializedObject, string propertyName, float value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null)
            throw new System.InvalidOperationException($"Serialized property not found: {propertyName}");

        property.floatValue = value;
    }

    private static void SetBool(SerializedObject serializedObject, string propertyName, bool value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null)
            throw new System.InvalidOperationException($"Serialized property not found: {propertyName}");

        property.boolValue = value;
    }
}
