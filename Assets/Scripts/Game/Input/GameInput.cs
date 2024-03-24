using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable InconsistentNaming

[SuppressMessage("ReSharper", "CheckNamespace")]
public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public static GameInput Instance { get; private set; }

    public enum Binding
    {
        K_MoveRight,
        K_MoveLeft,
        K_MoveDown,
        K_LockFigure,
        K_RotateRight,
        K_RotateLeft,
        S_Swipe
    }

    public event EventHandler OnKeyboardMoveRight;
    public event EventHandler OnKeyboardMoveLeft;
    public event EventHandler OnKeyboardMoveDown;
    public event EventHandler OnKeyboardLockFigure;
    public event EventHandler OnKeyboardRotateRight;
    public event EventHandler OnKeyboardRotateLeft;

    public event EventHandler<OnScreenSwipeEventArgs> OnScreenSwipe;

    public class OnScreenSwipeEventArgs : EventArgs
    {
        public Vector2 swipeVector;
    }

    private GameInputActions gameInputActions;

    private Vector2 swipeStartingPosition;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;


        gameInputActions = new GameInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
            gameInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));

        gameInputActions.Game.Enable();

        gameInputActions.Game.K_MoveRight.performed += K_MoveRight_OnPerformed;
        gameInputActions.Game.K_MoveLeft.performed += K_MoveLeft_OnPerformed;
        gameInputActions.Game.K_MoveDown.performed += K_MoveDown_OnPerformed;
        gameInputActions.Game.K_LockFigure.performed += K_LockFigure_OnPerformed;
        gameInputActions.Game.K_RotateLeft.performed += K_RotateLeft_OnPerformed;
        gameInputActions.Game.K_RotateRight.performed += K_RotateRight_OnPerformed;

        gameInputActions.Game.S_Swipe.performed += S_Swipe_OnPerformed;
        gameInputActions.Game.S_Swipe.canceled += S_Swipe_OnCanceled;
    }

    private void K_MoveRight_OnPerformed(InputAction.CallbackContext obj)
    {
        ImitateClick(Binding.K_MoveRight);
    }

    private void K_MoveLeft_OnPerformed(InputAction.CallbackContext obj)
    {
        ImitateClick(Binding.K_MoveLeft);
    }

    private void K_MoveDown_OnPerformed(InputAction.CallbackContext obj)
    {
        ImitateClick(Binding.K_MoveDown);
    }

    private void K_LockFigure_OnPerformed(InputAction.CallbackContext obj)
    {
        ImitateClick(Binding.K_LockFigure);
    }

    private void K_RotateLeft_OnPerformed(InputAction.CallbackContext obj)
    {
        ImitateClick(Binding.K_RotateLeft);
    }

    private void K_RotateRight_OnPerformed(InputAction.CallbackContext obj)
    {
        ImitateClick(Binding.K_RotateRight);
    }

    private void S_Swipe_OnPerformed(InputAction.CallbackContext obj)
    {
        swipeStartingPosition = GetCursorPosition();
    }

    private void S_Swipe_OnCanceled(InputAction.CallbackContext obj)
    {
        ImitateClick(Binding.S_Swipe);
    }

    private void ImitateClick(Binding imitatingBinding)
    {
        switch (imitatingBinding)
        {
            default:
            case Binding.K_MoveRight:
                OnKeyboardMoveRight?.Invoke(this, EventArgs.Empty);
                break;
            case Binding.K_MoveLeft:
                OnKeyboardMoveLeft?.Invoke(this, EventArgs.Empty);
                break;
            case Binding.K_MoveDown:
                OnKeyboardMoveDown?.Invoke(this, EventArgs.Empty);
                break;
            case Binding.K_RotateRight:
                OnKeyboardRotateRight?.Invoke(this, EventArgs.Empty);
                break;
            case Binding.K_RotateLeft:
                OnKeyboardRotateLeft?.Invoke(this, EventArgs.Empty);
                break;
            case Binding.K_LockFigure:
                OnKeyboardLockFigure?.Invoke(this, EventArgs.Empty);
                break;
            case Binding.S_Swipe:
                OnScreenSwipe?.Invoke(this, new OnScreenSwipeEventArgs
                {
                    swipeVector = GetSwipeVector()
                });
                swipeStartingPosition = Vector2.zero;
                break;
        }
    }

    private Vector2 GetSwipeVector()
    {
        if (swipeStartingPosition == Vector2.zero) return Vector2.zero;

        return GetCursorPosition() - swipeStartingPosition;
    }

    private void OnDestroy()
    {
        gameInputActions.Game.K_MoveRight.performed -= K_MoveRight_OnPerformed;
        gameInputActions.Game.K_MoveLeft.performed -= K_MoveLeft_OnPerformed;
        gameInputActions.Game.K_MoveDown.performed -= K_MoveDown_OnPerformed;
        gameInputActions.Game.K_LockFigure.performed -= K_LockFigure_OnPerformed;
        gameInputActions.Game.K_RotateLeft.performed -= K_RotateLeft_OnPerformed;
        gameInputActions.Game.K_RotateRight.performed -= K_RotateRight_OnPerformed;

        gameInputActions.Game.S_Swipe.performed -= S_Swipe_OnPerformed;
        gameInputActions.Game.S_Swipe.canceled -= S_Swipe_OnCanceled;

        gameInputActions.Dispose();
    }

    public Vector2 GetCursorPosition()
    {
        var cursorPosition = gameInputActions.Game.S_Position.ReadValue<Vector2>();

        return cursorPosition;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.K_MoveRight:
                return gameInputActions.Game.K_MoveRight.bindings[0].ToDisplayString();
            case Binding.K_MoveLeft:
                return gameInputActions.Game.K_MoveLeft.bindings[0].ToDisplayString();
            case Binding.K_MoveDown:
                return gameInputActions.Game.K_MoveDown.bindings[0].ToDisplayString();
            case Binding.K_RotateRight:
                return gameInputActions.Game.K_RotateRight.bindings[0].ToDisplayString();
            case Binding.K_RotateLeft:
                return gameInputActions.Game.K_RotateLeft.bindings[0].ToDisplayString();
            case Binding.K_LockFigure:
                return gameInputActions.Game.K_LockFigure.bindings[0].ToDisplayString();
        }
    }

    public float GetBindingValue(Binding binding)
    {
        var inputValue = 0f;
        switch (binding)
        {
            default:
            case Binding.K_MoveRight:
                inputValue = gameInputActions.Game.K_MoveRight.IsPressed() ? 1f : 0f;
                break;
            case Binding.K_MoveLeft:
                inputValue = gameInputActions.Game.K_MoveLeft.IsPressed() ? 1f : 0f;
                break;
            case Binding.K_MoveDown:
                inputValue = gameInputActions.Game.K_MoveDown.IsPressed() ? 1f : 0f;
                break;
            case Binding.K_RotateRight:
                inputValue = gameInputActions.Game.K_RotateRight.IsPressed() ? 1f : 0f;
                break;
            case Binding.K_RotateLeft:
                inputValue = gameInputActions.Game.K_RotateLeft.IsPressed() ? 1f : 0f;
                break;
            case Binding.K_LockFigure:
                inputValue = gameInputActions.Game.K_LockFigure.IsPressed() ? 1f : 0f;
                break;
            case Binding.S_Swipe:
                inputValue = gameInputActions.Game.S_Swipe.IsPressed() ? 1f : 0f;
                break;
        }

        return inputValue;
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        gameInputActions.Game.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.K_MoveRight:
                inputAction = gameInputActions.Game.K_MoveRight;
                bindingIndex = 0;
                break;
            case Binding.K_MoveLeft:
                inputAction = gameInputActions.Game.K_MoveLeft;
                bindingIndex = 0;
                break;
            case Binding.K_MoveDown:
                inputAction = gameInputActions.Game.K_MoveDown;
                bindingIndex = 0;
                break;
            case Binding.K_RotateRight:
                inputAction = gameInputActions.Game.K_RotateRight;
                bindingIndex = 0;
                break;
            case Binding.K_RotateLeft:
                inputAction = gameInputActions.Game.K_RotateLeft;
                bindingIndex = 0;
                break;
            case Binding.K_LockFigure:
                inputAction = gameInputActions.Game.K_LockFigure;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                gameInputActions.Game.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, gameInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();
    }
}
