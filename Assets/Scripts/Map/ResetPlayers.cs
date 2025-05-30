using UnityEngine;

public class ResetPlayers : MonoBehaviour
{
    [Header("Area Start Point")]
    [SerializeField] private Transform areaStartPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // When *any* player hits the spikes, reset *all* players
        var allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in allPlayers)
        {
            var controller = player.GetComponent<GameplayController>();
            if (controller != null)
            {
                controller.RepositionImmediately(areaStartPoint.position, resetVelocity: true);
            }
            else
            {
                player.transform.position = areaStartPoint.position;
                Debug.LogWarning($"[ResetPlayers] Player {player.name} missing GameplayController. Repositioned manually.");
            }
        }
    }
}
