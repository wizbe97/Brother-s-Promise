using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject panelMainMenu;
    public GameObject panelSaveSelection;
    [SerializeField] private int onlineSceneIndex = 1;
    [SerializeField] private int offlineSceneIndex = 2;
    private int sceneIndexToLoad;


    private void Start()
    {
        panelMainMenu.SetActive(true);
        panelSaveSelection.SetActive(false);
    }

    public void PlayOnline()
    {
        ShowSaveSelection();
        sceneIndexToLoad = onlineSceneIndex;
    }

    public void PlayLocal()
    {
        ShowSaveSelection();
        sceneIndexToLoad = onlineSceneIndex;
    }

    public void PlayOffline()
    {
        ShowSaveSelection();
        sceneIndexToLoad = offlineSceneIndex;
    }

    private void ShowSaveSelection()
    {
        panelMainMenu.SetActive(false);
        panelSaveSelection.SetActive(true);
    }

    public void SelectSaveSlot(int slotId)
    {
        PlayerPrefs.SetInt("SaveSlot", slotId);

        SceneManager.LoadScene(sceneIndexToLoad);
        Debug.Log("Loading Online Lobby Scene with Save Slot: " + slotId);
    }

    public void BackToMainMenu()
    {
        panelSaveSelection.SetActive(false);
        panelMainMenu.SetActive(true);
        if (GameManager.Instance != null)
            GameManager.Instance.SetIsOnline(false);
    }

    public void OpenSettings()
    {
        Debug.Log("Open Settings Panel");
    }

    public void ExitGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetIsOnline(false);
        Application.Quit();
    }
}
