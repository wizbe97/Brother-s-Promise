using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
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

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (visualTransform == null) visualTransform = transform;

        _controller = GetComponentInParent<GameplayController>();
    }

    private void Update()
    {
        if (_controller == null || animator == null) return;

        UpdateAnimationState();
    }

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

            switch (_currentState)
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
    }

    private void FlipSprite()
    {
        float x = _controller.Input.x;

        if (x > 0.01f)
            visualTransform.rotation = Quaternion.Euler(0f, 0f, 0f); // face right
        else if (x < -0.01f)
            visualTransform.rotation = Quaternion.Euler(0f, 180f, 0f); // face left
    }
}
