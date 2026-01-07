using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public sealed class InputHandler : MonoBehaviour
{
    #region Public Properties

    public Vector2 Position => CalculateNormalizedPosition();
    public bool IsPressed { get; private set; }
    public int TouchCount { get; private set; }

    #endregion

    #region Private Members

    VisualElement _area;

    Vector2 CalculateNormalizedPosition()
    {
        var height = _area.resolvedStyle.height;
        if (height == 0 || float.IsNaN(height)) return Vector2.zero;
        var screenPos = Pointer.current.position.value;
        var panelPos = RuntimePanelUtils.ScreenToPanel(_area.panel, screenPos);
        var localPos = _area.WorldToLocal(panelPos);
        return localPos / height;
    }

    int CountActiveTouches()
    {
        var count = 0;
        if (Touchscreen.current != null)
            foreach (var touch in Touchscreen.current.touches)
                if (touch.press.isPressed) count++;
        return count;
    }

    #endregion

    #region MonoBehaviour Implementation

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _area = root.Q<VisualElement>("game-area");
        _area.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    void OnDisable()
      => _area.UnregisterCallback<PointerDownEvent>(OnPointerDown);

    void LateUpdate()
    {
        IsPressed &= !Pointer.current.press.wasReleasedThisFrame;
        TouchCount = IsPressed ? Mathf.Max(1, CountActiveTouches()) : 0;
    }

    #endregion

    #region UI Event Handlers

    void OnPointerDown(PointerDownEvent evt)
      => (IsPressed, TouchCount) = (true, 1);

    #endregion
}
