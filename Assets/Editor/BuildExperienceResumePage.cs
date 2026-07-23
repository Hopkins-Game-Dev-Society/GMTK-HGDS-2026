using BirthdayJobJam.Application;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

internal static class BuildExperienceResumePage
{
    private const string ScenePath = "Assets/Scenes/gameplay.unity";
    private const string ExperienceContentPath = "Assets/Game/Data/Application/ExperiencePageContent.asset";
    private const string DocEmblemPath = "Assets/Art/Temp/doc-emblem-temp.png";
    private const string ArialFontPath = "Assets/Fonts/arial/Arial SDF.asset";

    [MenuItem("Birthday Job Jam/Build Experience Resume Page")]
    public static void Build()
    {
        EditorSceneManager.OpenScene(ScenePath);
        EnsureDocEmblemIsSprite();

        ApplicationSignInPageView view = Object.FindAnyObjectByType<ApplicationSignInPageView>();
        if (view == null)
            throw new System.InvalidOperationException("ApplicationSignInPageView not found in gameplay scene.");

        RectTransform shell = GameObject.Find("Application UI Shell")?.GetComponent<RectTransform>();
        if (shell == null)
            throw new System.InvalidOperationException("Application UI Shell not found in gameplay scene.");

        ApplicationExperiencePageContent content = EnsureExperienceContent();
        TMP_FontAsset arial = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ArialFontPath);
        Sprite docSprite = AssetDatabase.LoadAssetAtPath<Sprite>(DocEmblemPath);

        DestroyChild(shell, "My Experience Panel");
        DestroyChild(shell, "Resume Picker Panel");

        GameObject experiencePanel = BuildExperiencePanel(shell, arial);
        FinderRefs finder = BuildFinderPanel(shell, arial, docSprite, content);

        SerializedObject serializedView = new SerializedObject(view);
        SetObject(serializedView, "experienceContent", content);
        SetObject(serializedView, "myExperiencePanel", experiencePanel);
        SetObject(serializedView, "myExperienceIntroText", experiencePanel.transform.Find("Intro Text").GetComponent<TMP_Text>());
        SetObject(serializedView, "uploadResumeButton", experiencePanel.transform.Find("Upload Resume Button").GetComponent<Button>());
        SetObject(serializedView, "uploadResumeButtonText", experiencePanel.transform.Find("Upload Resume Button/Label").GetComponent<TMP_Text>());
        SetObject(serializedView, "resumePickerPanel", finder.Root);
        SetObject(serializedView, "resumePickerTitleText", finder.TitleText);
        SetObject(serializedView, "resumePickerPathText", finder.PathText);
        SetObject(serializedView, "resumePickerStatusText", finder.StatusText);
        SetObject(serializedView, "resumePickerOpenButton", finder.OpenButton);
        SetObject(serializedView, "resumePickerOpenButtonText", finder.OpenButtonText);
        SetObject(serializedView, "resumePickerSelectButton", finder.SelectButton);
        SetObject(serializedView, "resumePickerSelectButtonText", finder.SelectButtonText);
        SetObject(serializedView, "resumePickerCancelButton", finder.CancelButton);
        SetObject(serializedView, "resumePickerCancelButtonText", finder.CancelButtonText);
        SetArray(serializedView, "resumeFileButtons", finder.FileButtons);
        SetArray(serializedView, "resumeFileNameTexts", finder.FileNameTexts);
        SetArray(serializedView, "resumeFileIconImages", finder.FileIconImages);
        serializedView.ApplyModifiedProperties();

        experiencePanel.SetActive(false);
        finder.Root.SetActive(false);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        Debug.Log("Built Experience resume upload page.");
    }

    private static ApplicationExperiencePageContent EnsureExperienceContent()
    {
        ApplicationExperiencePageContent content = AssetDatabase.LoadAssetAtPath<ApplicationExperiencePageContent>(ExperienceContentPath);
        if (content != null)
            return content;

        content = ScriptableObject.CreateInstance<ApplicationExperiencePageContent>();
        AssetDatabase.CreateAsset(content, ExperienceContentPath);
        AssetDatabase.SaveAssets();
        return content;
    }

    private static GameObject BuildExperiencePanel(RectTransform parent, TMP_FontAsset font)
    {
        GameObject panel = CreateRect("My Experience Panel", parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(92f, -330f), new Vector2(560f, 180f));
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(1f, 0.985f, 0.92f, 0.92f);

        CreateText("Intro Text", panel.transform, font, "Please upload your resume. Any resume. Ideally the correct one.", 23, FontStyles.Normal, TextAlignmentOptions.TopLeft, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(32f, -28f), new Vector2(-64f, 70f), new Color(0.18f, 0.18f, 0.18f, 1f));
        CreateButton("Upload Resume Button", panel.transform, font, "Upload Resume", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-36f, 30f), new Vector2(210f, 58f), new Color(0.13f, 0.42f, 0.86f, 1f), Color.white, 23);

        return panel;
    }

    private static FinderRefs BuildFinderPanel(RectTransform parent, TMP_FontAsset font, Sprite docSprite, ApplicationExperiencePageContent content)
    {
        FinderRefs refs = new FinderRefs();
        refs.Root = CreateRect("Resume Picker Panel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -8f), new Vector2(800f, 470f));
        refs.Root.transform.SetAsLastSibling();

        Image rootImage = refs.Root.AddComponent<Image>();
        rootImage.color = new Color(0.91f, 0.91f, 0.89f, 1f);

        GameObject titleBar = CreateRect("Title Bar", refs.Root.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 44f));
        Image titleBarImage = titleBar.AddComponent<Image>();
        titleBarImage.color = new Color(0.78f, 0.78f, 0.76f, 1f);

        CreateDot("Close Dot", titleBar.transform, new Vector2(22f, -22f), new Color(1f, 0.36f, 0.29f, 1f));
        CreateDot("Minimize Dot", titleBar.transform, new Vector2(48f, -22f), new Color(1f, 0.77f, 0.26f, 1f));
        CreateDot("Zoom Dot", titleBar.transform, new Vector2(74f, -22f), new Color(0.32f, 0.78f, 0.34f, 1f));

        refs.TitleText = CreateText("Title Text", titleBar.transform, font, "Choose Resume", 20, FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.12f, 0.12f, 0.12f, 1f));
        refs.PathText = CreateText("Path Text", refs.Root.transform, font, "Macintosh HD > Users > Applicant > Desktop > resume graveyard", 17, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(24f, -62f), new Vector2(-48f, 28f), new Color(0.24f, 0.24f, 0.24f, 1f));

        GameObject fileArea = CreateRect("File Area", refs.Root.transform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0f, 8f), new Vector2(-40f, -142f));
        GridLayoutGroup grid = fileArea.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(136f, 120f);
        grid.spacing = new Vector2(10f, 10f);
        grid.padding = new RectOffset(20, 20, 18, 18);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;

        int fileCount = Mathf.Max(10, content != null ? content.ResumeFileNames.Count : 10);
        refs.FileButtons = new Button[fileCount];
        refs.FileNameTexts = new TMP_Text[fileCount];
        refs.FileIconImages = new Image[fileCount];

        for (int i = 0; i < fileCount; i++)
            BuildFileButton(fileArea.transform, font, docSprite, ResumeName(content, i), refs, i);

        refs.StatusText = CreateText("Status Text", refs.Root.transform, font, "", 17, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(24f, 58f), new Vector2(-420f, 28f), new Color(0.55f, 0.12f, 0.12f, 1f));
        refs.OpenButton = CreateButton("Open Button", refs.Root.transform, font, "Open", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-288f, 32f), new Vector2(118f, 42f), new Color(0.7f, 0.72f, 0.78f, 1f), Color.white, 19);
        refs.OpenButtonText = refs.OpenButton.transform.Find("Label").GetComponent<TMP_Text>();
        refs.SelectButton = CreateButton("Select Button", refs.Root.transform, font, "Select", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-154f, 32f), new Vector2(118f, 42f), new Color(0.13f, 0.42f, 0.86f, 1f), Color.white, 19);
        refs.SelectButtonText = refs.SelectButton.transform.Find("Label").GetComponent<TMP_Text>();
        refs.CancelButton = CreateButton("Cancel Button", refs.Root.transform, font, "Cancel", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-24f, 32f), new Vector2(106f, 42f), new Color(0.62f, 0.62f, 0.6f, 1f), Color.white, 19);
        refs.CancelButtonText = refs.CancelButton.transform.Find("Label").GetComponent<TMP_Text>();

        return refs;
    }

    private static void BuildFileButton(Transform parent, TMP_FontAsset font, Sprite docSprite, string fileName, FinderRefs refs, int index)
    {
        Button button = CreateButton($"Resume File {index + 1:00}", parent, font, "", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(136f, 120f), new Color(0.92f, 0.92f, 0.9f, 0f), Color.white, 20);
        refs.FileButtons[index] = button;

        RectTransform label = button.transform.Find("Label").GetComponent<RectTransform>();
        Object.DestroyImmediate(label.gameObject);

        GameObject icon = CreateRect("Icon", button.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -8f), new Vector2(54f, 64f));
        Image iconImage = icon.AddComponent<Image>();
        iconImage.sprite = docSprite;
        iconImage.preserveAspect = true;
        refs.FileIconImages[index] = iconImage;

        refs.FileNameTexts[index] = CreateText("File Name", button.transform, font, fileName, 14, FontStyles.Bold, TextAlignmentOptions.Top, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 5f), new Vector2(-8f, 44f), new Color(0.17f, 0.17f, 0.17f, 1f));
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

    private static void CreateDot(string name, Transform parent, Vector2 position, Color color)
    {
        GameObject dot = CreateRect(name, parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0.5f, 0.5f), position, new Vector2(18f, 18f));
        Image image = dot.AddComponent<Image>();
        image.color = color;
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

    private static string ResumeName(ApplicationExperiencePageContent content, int index)
    {
        if (content != null && content.ResumeFileNames != null && index < content.ResumeFileNames.Count)
            return content.ResumeFileNames[index];

        string[] defaults =
        {
            "resume-new.doc",
            "resume-final.doc",
            "resume-final-final.doc",
            "resume-true-final.doc",
            "resume-1.doc",
            "resume-true-final-FINAL.doc",
            "resume-2.doc",
            "resume-use-this-one-maybe.doc",
            "resume-DONT-use.doc",
            "resume-real-actual-last.doc"
        };
        return defaults[Mathf.Clamp(index, 0, defaults.Length - 1)];
    }

    private static void EnsureDocEmblemIsSprite()
    {
        TextureImporter importer = AssetImporter.GetAtPath(DocEmblemPath) as TextureImporter;
        if (importer == null || importer.textureType == TextureImporterType.Sprite)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.SaveAndReimport();
    }

    private static void SetObject(SerializedObject serializedObject, string propertyName, Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null)
            throw new System.InvalidOperationException($"Serialized property not found: {propertyName}");

        property.objectReferenceValue = value;
    }

    private static void SetArray<T>(SerializedObject serializedObject, string propertyName, T[] values) where T : Object
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null)
            throw new System.InvalidOperationException($"Serialized property not found: {propertyName}");

        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }

    private sealed class FinderRefs
    {
        public GameObject Root;
        public TMP_Text TitleText;
        public TMP_Text PathText;
        public TMP_Text StatusText;
        public Button[] FileButtons;
        public TMP_Text[] FileNameTexts;
        public Image[] FileIconImages;
        public Button OpenButton;
        public TMP_Text OpenButtonText;
        public Button SelectButton;
        public TMP_Text SelectButtonText;
        public Button CancelButton;
        public TMP_Text CancelButtonText;
    }
}
