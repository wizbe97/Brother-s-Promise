using System;
using UnityEngine;
using Fusion;

public class PlayerAnimator : NetworkBehaviour
{
    public enum PlayerStates
    {
        IDLE,
        RUN,
        JUMP,
        FALL
    }

    [Header("References")]
    public Animator animator;
    public Transform visualTransform;

    private GameplayController _controller;
    private PlayerStates _currentState;

    [Networked] private PlayerStates NetworkState { get; set; }
    [Networked] private bool FacingRight { get; set; }

    private bool IsOnline => GameManager.Instance != null && GameManager.Instance.IsOnline;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (visualTransform == null) visualTransform = transform;

        _controller = GetComponentInParent<GameplayController>();
    }

    private void Update()
    {
        if (_controller == null || animator == null) return;

        // Always run offline logic
        if (!IsOnline)
        {
            UpdateAnimationState();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsOnline || _controller == null || !Object.HasStateAuthority) return;

        var velocity = _controller.Velocity;
        var grounded = _controller.IsGrounded();

        PlayerStates newState;

        if (!grounded)
        {
            newState = velocity.y > 0.01f ? PlayerStates.JUMP : PlayerStates.FALL;
        }
        else if (Mathf.Abs(velocity.x) > 0.01f)
        {
            newState = PlayerStates.RUN;
        }
        else
        {
            newState = PlayerStates.IDLE;
        }

        NetworkState = newState;
        FacingRight = _controller.Input.x > 0.01f || (_controller.Input.x == 0 && FacingRight);
    }


    public override void Render()
    {
        if (!IsOnline || _controller == null || animator == null) return;

        // Use same animation names as offline
        if (_currentState != NetworkState)
        {
            _currentState = NetworkState;
            PlayAnimation(_currentState);
        }

        FlipSprite();
    }

    // === YOUR ORIGINAL OFFLINE METHOD ===
    private void UpdateAnimationState()
    {
        FlipSprite();

        var velocity = _controller.Velocity;
        var grounded = _controller.IsGrounded();

        PlayerStates newState;

        if (!grounded)
        {
            newState = velocity.y > 0.01f ? PlayerStates.JUMP : PlayerStates.FALL;
        }
        else if (Mathf.Abs(velocity.x) > 0.01f)
        {
            newState = PlayerStates.RUN;
        }
        else
        {
            newState = PlayerStates.IDLE;
        }

        if (_currentState != newState)
        {
            _currentState = newState;

            PlayAnimation(_currentState);
        }
    }

    private void PlayAnimation(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.IDLE:
                animator.Play("Idle");
                break;
            case PlayerStates.RUN:
                animator.Play("Run");
                break;
            case PlayerStates.JUMP:
                animator.Play("Jump");
                break;
            case PlayerStates.FALL:
                animator.Play("Fall");
                break;
        }
    }

    private void FlipSprite()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsOnline)
        {
            // Use the networked FacingRight value
            visualTransform.rotation = Quaternion.Euler(0f, FacingRight ? 0f : 180f, 0f);
        }
        else
        {
            // Use local input for offline or no GameManager
            float x = _controller.Input.x;

            if (x > 0.01f)
                visualTransform.rotation = Quaternion.Euler(0f, 0f, 0f); // face right
            else if (x < -0.01f)
                visualTransform.rotation = Quaternion.Euler(0f, 180f, 0f); // face left
        }
    }
}


