using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using System;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    // [SerializeField] private Animator _loadingScreenAnimator; 

    private int _lastLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(transform.parent ? transform.parent.gameObject : gameObject);
    }

    // public void StartLoadingScreen() {
    //     if (_loadingScreenAnimator != null)
    //         _loadingScreenAnimator.Play("In");
    // }

    // public void FinishLoadingScreen() {
    //     if (_loadingScreenAnimator != null)
    //         _loadingScreenAnimator.Play("Out");
    // }

    public void ResetLastLevelsIndex()
    {
        _lastLevelIndex = 1; // Set it to 1 if you don't want the first scene to load again
    }

    /// <summary>
    /// Loads the next scene in build index (skips index 0).
    /// </summary>
	public void LoadNextLevel(NetworkRunner runner)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        // Ensure _lastLevelIndex stays within bounds and doesn't load the first scene
        _lastLevelIndex = (_lastLevelIndex + 1 >= sceneCount) ? 1 : _lastLevelIndex + 1;

        string sceneName = SceneUtility.GetScenePathByBuildIndex(_lastLevelIndex);
        string sceneShortName = System.IO.Path.GetFileNameWithoutExtension(sceneName);

        runner.LoadScene(sceneShortName);
    }
    /// <summary>
    /// Loads a specific scene index (you provide).
    /// </summary>
	public void LoadSpecificLevel(NetworkRunner runner, int sceneIndex)
    {
        _lastLevelIndex = sceneIndex;

        // Ensure we don't try to load the first scene (index 0)
        if (_lastLevelIndex == 0)
        {
            _lastLevelIndex = 1;
        }

        string sceneName = SceneUtility.GetScenePathByBuildIndex(_lastLevelIndex);
        string sceneShortName = System.IO.Path.GetFileNameWithoutExtension(sceneName);

        runner.LoadScene(sceneShortName);
    }
}
