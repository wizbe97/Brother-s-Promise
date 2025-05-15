using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject panelMainMenu;
    public GameObject panelSaveSelection;


    private void Start()
    {
        panelMainMenu.SetActive(true);
        panelSaveSelection.SetActive(false);
    }

    public void PlayOnline()
    {
        GameManager.Instance.SetIsOnline(true);
        ShowSaveSelection();
    }

    public void PlayLocal()
    {
        GameManager.Instance.SetIsOnline(true);
        ShowSaveSelection();
    }

    public void PlayOffline()
    {
        GameManager.Instance.SetIsOnline(false);
        ShowSaveSelection();
    }

    private void ShowSaveSelection()
    {
        panelMainMenu.SetActive(false);
        panelSaveSelection.SetActive(true);
    }

    public void SelectSaveSlot(int slotId)
    {
        PlayerPrefs.SetInt("SaveSlot", slotId);
        // NO need to save PlayOnline anymore! Just GameManager.Instance.IsOnline holds it.

        if (GameManager.Instance.IsOnline)
        {
            SceneManager.LoadScene("2_LobbyOnline");
            Debug.Log("Loading Online Lobby Scene with Save Slot: " + slotId);
        }
        else
        {
            SceneManager.LoadScene("Lobby_Offline");
            Debug.Log("Loading Offline Lobby Scene with Save Slot: " + slotId);
        }
    }

    public void BackToMainMenu()
    {
        panelSaveSelection.SetActive(false);
        panelMainMenu.SetActive(true);
        GameManager.Instance.SetIsOnline(false);
    }

    public void OpenSettings()
    {
        Debug.Log("Open Settings Panel");
    }

    public void ExitGame()
    {
        GameManager.Instance.SetIsOnline(false);
        Application.Quit();
    }
}
