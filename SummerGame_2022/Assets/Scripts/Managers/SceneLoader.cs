using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {
    public static SceneLoader Instance;

    [SerializeField] private LevelData levelData;
    [SerializeField] private bool fadeOnFirstScene;
    [SerializeField] private RawImage blackScreenPrefab;
    [SerializeField] private float fadeSeconds;

    private Transform canvasTransform;
    private RawImage blackScreen;

    private int buildIndex;
    private bool firstScene = true;

    private void Awake() {
        if (!Instance) {
            Instance = this;
            transform.parent = null;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (firstScene) {
            if (fadeOnFirstScene) {
                StartCoroutine(FadeTo(1, 0, fadeSeconds));
                firstScene = false;
            }
            return;
        }
        StartCoroutine(FadeTo(1, 0, fadeSeconds));
    }

    public void LoadScene(int buildIndex) {
        this.buildIndex = buildIndex;
        StartCoroutine(FadeTo(0, 1, fadeSeconds, true));
    }

    public void LoadScene(int buildIndex, float fadeSeconds) {
        this.buildIndex = buildIndex;
        StartCoroutine(FadeTo(0, 1, fadeSeconds, true));
    }

    public void LoadLevel(int level) {
        buildIndex = levelData.levelBuildIndex[level];
        StartCoroutine(FadeTo(0, 1, fadeSeconds, true));
    }

    public void Reload() {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator FadeTo(float from, float to, float time, bool changeScene = false) {
        if (!blackScreenPrefab) {
            yield break;
        }

        canvasTransform = FindObjectOfType<Canvas>().transform;
        blackScreen = Instantiate(blackScreenPrefab, canvasTransform);
        blackScreen.gameObject.SetActive(true);
        
        float count = 0;
        float t = 0;
        time = time == -1 ? fadeSeconds : time;

        Color color = blackScreen.color;
        color.a = from;
        blackScreen.color = color;

        while (count <= time) {
            count += Time.deltaTime;
            t += Time.deltaTime / time;
            color.a = Mathf.Lerp(from, to, t);
            blackScreen.color = color;
            yield return null;
        }

        color.a = to;
        blackScreen.color = color;

        if (changeScene) {
            SceneManager.LoadScene(buildIndex);
        }
    }
}
