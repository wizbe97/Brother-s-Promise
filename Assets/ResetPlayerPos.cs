using UnityEngine;
using UnityEngine.InputSystem;

public class ResetPlayerPos : MonoBehaviour
{
    [SerializeField] private Transform player1Target;
    [SerializeField] private Transform player2Target;

    private Rigidbody2D rb1;
    private Rigidbody2D rb2;

    void Start()
    {
        // Find the first two GameplayController scripts in the scene (or tag them instead)
        GameplayController[] players = FindObjectsOfType<GameplayController>();

        if (players.Length >= 2)
        {
            rb1 = players[0].GetComponent<Rigidbody2D>();
            rb2 = players[1].GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.LogError("Could not find both player instances in the scene.");
        }
    }

    void Update()
    {
        bool resetKeyPressed = Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
        bool gamepadStartPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

        if (resetKeyPressed || gamepadStartPressed)
        {
            Debug.Log("ResetPlayerPos Update called");

            if (rb1 != null && player1Target != null)
            {
                rb1.velocity = Vector2.zero;
                rb1.position = player1Target.position;
            }

            if (rb2 != null && player2Target != null)
            {
                rb2.velocity = Vector2.zero;
                rb2.position = player2Target.position;
            }
        }
    }
}
 