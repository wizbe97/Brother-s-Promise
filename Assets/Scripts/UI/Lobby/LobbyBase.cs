using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class LobbyBase : MonoBehaviour
{
    [Header("Scene Setup")]
    public int sceneIndexToLoad;

    [Header("UI References")]
    [SerializeField] protected GameObject panelLobby;
    [SerializeField] protected TextMeshProUGUI roomNameText;
    [SerializeField] protected Button startButton;
    public Transform imageBrother1;
    public Transform imageBrother2;

    protected int saveSlot;
    protected bool isOnline;

    protected virtual void Start()
    {
        saveSlot = PlayerPrefs.GetInt("SaveSlot", 0);
        isOnline = GameManager.Instance.IsOnline;

        Debug.Log($"[LobbyBase] SaveSlot={saveSlot}, PlayOnline={isOnline}");

        if (startButton != null)
            startButton.gameObject.SetActive(false);

        if (panelLobby != null)
            panelLobby.SetActive(false);
    }

    protected void ShowLobbyPanel(string roomName = "Lobby")
    {
        if (panelLobby != null)
            panelLobby.SetActive(true);

        if (roomNameText != null)
            roomNameText.text = $"Room: {roomName}";
    }

    protected void CheckIfAllReady()
    {
        bool canStart = CanStartGameInternal();
        UpdateStartButton(canStart);
    }

    protected void UpdateStartButton(bool canStart)
    {
        if (startButton != null)
            startButton.gameObject.SetActive(canStart);
    }

    public virtual void StartGameButtonPressed()
    {
        if (sceneIndexToLoad <= 0)
            return;
    }

    protected abstract bool CanStartGameInternal();
}
