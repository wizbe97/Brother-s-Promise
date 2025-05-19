using UnityEngine;
using System.Collections;

public class DashJumpOffPlayer : MonoBehaviour
{
    [SerializeField] private float freezeDuration = 0.2f;
    [SerializeField] private float p1RecoilForce = 8f;
    [SerializeField] private float p1VerticalBoostForce = 3f; // NEW
    [SerializeField] private float wallJumpEnableDuration = 0.25f;

    private GameplayController _p1;

    private void Awake()
    {
        _p1 = GetComponentInParent<GameplayController>();
        if (_p1 == null)
            Debug.LogError("DashJumpOffPlayer must be on a child of a player with GameplayController");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out GameplayController p2)) return;
        if (!p2.Stats.AllowDash || !p2.IsDashing()) return;

        Vector2 toP1 = (Vector2)_p1.transform.position - (Vector2)p2.transform.position;
        Vector2 dashDir = p2.GetDashDirection();
        if (Vector2.Dot(dashDir.normalized, toP1.normalized) < 0.7f) return;

        StartCoroutine(HandleDashJump(p2, toP1.normalized));
    }

    private IEnumerator HandleDashJump(GameplayController p2, Vector2 dashDirection)
    {
        // Temporarily enable wall mechanics
        bool originalAllowWalls = p2.Stats.AllowWalls;
        p2.Stats.AllowWalls = true;

        // Zero velocity before freeze
        _p1.SetVelocity(Vector2.zero);
        p2.SetVelocity(Vector2.zero);

        // Freeze movement
        _p1.TogglePlayer(false);
        p2.TogglePlayer(false);

        yield return new WaitForSeconds(freezeDuration);

        // Apply recoil + upward force to P1
        Vector2 recoil = dashDirection * p1RecoilForce + Vector2.up * p1VerticalBoostForce;
        _p1.AddFrameForce(recoil);

        // Simulate wall stick on P2
        p2.WallDirection = (int)Mathf.Sign(dashDirection.x);
        p2.ToggleOnWall(true);

        // Unfreeze both players
        _p1.TogglePlayer(true);
        p2.TogglePlayer(true);

        // Allow short window for wall jump
        yield return new WaitForSeconds(wallJumpEnableDuration);

        // Reset wall state
        p2.ToggleOnWall(false);
        p2.Stats.AllowWalls = originalAllowWalls;
    }
}
