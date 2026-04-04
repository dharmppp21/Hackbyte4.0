using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Runs when the Cutscene scene loads: optional Timeline (PlayableDirector), otherwise a short full-screen fade as a stand-in animation.
/// Hook a Timeline asset on <see cref="director"/> in the Inspector to replace the placeholder.
/// </summary>
[DisallowMultipleComponent]
public class CutscenePlayer : MonoBehaviour
{
    [SerializeField] PlayableDirector director;

    [Tooltip("Used only when no Timeline asset is assigned to the director.")]
    [SerializeField] float placeholderHoldSeconds = 2.5f;

    [Tooltip("If set, loads this scene after the cutscene finishes (e.g. main gameplay scene name).")]
    [SerializeField] string nextSceneAfterCutscene = "";

    IEnumerator Start()
    {
        if (director != null && director.playableAsset != null)
        {
            director.Play();
            yield return new WaitUntil(() => director.state != PlayState.Playing);
        }
        else
            yield return PlayPlaceholderCutscene();

        if (!string.IsNullOrWhiteSpace(nextSceneAfterCutscene))
            SceneManager.LoadScene(nextSceneAfterCutscene);
    }

    IEnumerator PlayPlaceholderCutscene()
    {
        var canvasGo = new GameObject("CutsceneOverlay");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("Fade");
        panel.transform.SetParent(canvasGo.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = panel.AddComponent<Image>();
        img.color = Color.black;

        // Fade from black → brief hold → fade to black (simple "opening" beat)
        float t = 0f;
        const float fadeInDur = 1.1f;
        while (t < fadeInDur)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeInDur);
            img.color = new Color(0f, 0f, 0f, 1f - EaseOutQuad(a) * 0.92f);
            yield return null;
        }

        img.color = new Color(0f, 0f, 0f, 0.08f);
        yield return new WaitForSeconds(placeholderHoldSeconds);

        t = 0f;
        const float fadeOutDur = 1f;
        while (t < fadeOutDur)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeOutDur);
            img.color = new Color(0f, 0f, 0f, Mathf.Lerp(0.08f, 1f, EaseInQuad(a)));
            yield return null;
        }

        Destroy(canvasGo);
    }

    static float EaseOutQuad(float x) => 1f - (1f - x) * (1f - x);
    static float EaseInQuad(float x) => x * x;
}
