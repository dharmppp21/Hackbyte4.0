using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Builds a screen-space title layout: full-screen background, centered title art, Start at bottom center.
/// Assign your imported PNGs as Sprites on the component (Texture Type: Sprite 2D and UI).
/// </summary>
[DisallowMultipleComponent]
public class TitleScreenController : MonoBehaviour
{
    [Header("Art")]
    [Tooltip("Wide white plate / backdrop sprite (optional — uses solid white if empty).")]
    [SerializeField] Sprite backgroundSprite;

    [Tooltip("Title artwork e.g. SPLIT WITHIN (optional).")]
    [SerializeField] Sprite titleSprite;

    [Header("Cutscene")]
    [SerializeField] string cutsceneSceneName = "Cutscene";
    [SerializeField] bool loadCutsceneScene = true;

    void Awake()
    {
        BuildUi();
    }

    void BuildUi()
    {
        var canvasGo = new GameObject("TitleCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        EnsureEventSystem();

        // Background — full screen
        var bgGo = CreateUiChild(canvasGo.transform, "Background");
        StretchFull(bgGo.GetComponent<RectTransform>());
        var bgImage = bgGo.AddComponent<Image>();
        if (backgroundSprite != null)
        {
            bgImage.sprite = backgroundSprite;
            bgImage.type = Image.Type.Simple;
            bgImage.preserveAspect = false;
            bgImage.color = Color.white;
        }
        else
            bgImage.color = Color.white;

        // Title — centered on canvas
        var titleGo = CreateUiChild(canvasGo.transform, "Title");
        var titleRt = titleGo.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.5f, 0.5f);
        titleRt.anchorMax = new Vector2(0.5f, 0.5f);
        titleRt.pivot = new Vector2(0.5f, 0.5f);
        titleRt.anchoredPosition = new Vector2(0f, 40f);
        titleRt.sizeDelta = new Vector2(1600f, 900f);
        var titleImg = titleGo.AddComponent<Image>();
        if (titleSprite != null)
        {
            titleImg.sprite = titleSprite;
            titleImg.preserveAspect = true;
            titleImg.color = Color.white;
        }
        else
        {
            titleImg.enabled = false;
            var fallback = titleGo.AddComponent<Text>();
            fallback.text = "SPLIT WITHIN";
            fallback.alignment = TextAnchor.MiddleCenter;
            fallback.color = Color.white;
            fallback.font = GetUiFont();
            fallback.fontSize = 64;
            fallback.fontStyle = FontStyle.Bold;
        }

        // Start — bottom center of the canvas (bottom of the white / screen area)
        var btnRoot = CreateUiChild(canvasGo.transform, "StartButton");
        var btnRt = btnRoot.GetComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0f);
        btnRt.anchorMax = new Vector2(0.5f, 0f);
        btnRt.pivot = new Vector2(0.5f, 0f);
        btnRt.anchoredPosition = new Vector2(0f, 96f);
        btnRt.sizeDelta = new Vector2(340f, 96f);

        var btnGraphic = btnRoot.AddComponent<Image>();
        btnGraphic.color = new Color(0f, 0f, 0f, 0.25f);
        var outline = btnRoot.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(3f, -3f);
        outline.useGraphicAlpha = true;

        var button = btnRoot.AddComponent<Button>();
        button.targetGraphic = btnGraphic;
        var colors = button.colors;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.35f);
        colors.pressedColor = new Color(1f, 1f, 1f, 0.5f);
        button.colors = colors;
        button.onClick.AddListener(OnStartClicked);

        var textGo = CreateUiChild(btnRoot.transform, "Label");
        StretchFull(textGo.GetComponent<RectTransform>());
        var label = textGo.AddComponent<Text>();
        label.text = "Start";
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.font = GetUiFont();
        label.fontSize = 44;
        label.fontStyle = FontStyle.Bold;

        btnRoot.transform.SetAsLastSibling();
    }

    static GameObject CreateUiChild(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static Font GetUiFont()
    {
        var f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f != null)
            return f;
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;
        var es = new GameObject("EventSystem");
#if ENABLE_INPUT_SYSTEM
        es.AddComponent<EventSystem>();
        es.AddComponent<InputSystemUIInputModule>();
#else
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
#endif
    }

    void OnStartClicked()
    {
        if (!loadCutsceneScene || string.IsNullOrWhiteSpace(cutsceneSceneName))
            return;
        SceneManager.LoadScene(cutsceneSceneName);
    }
}
