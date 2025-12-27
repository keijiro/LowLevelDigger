using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public sealed class InputHandler : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] UIDocument _ui = null;

    #endregion

    #region Public Properties

    public Vector2 Position => Pointer.current.position.value;
    public bool IsPressed { get; private set; }

    #endregion

    #region MonoBehaviour Implementation

    VisualElement _area;

    void OnEnable()
    {
        _area = _ui.rootVisualElement.Q<VisualElement>("game-area");
        _area.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    void OnDisable()
      => _area.UnregisterCallback<PointerDownEvent>(OnPointerDown);

    void LateUpdate()
      => IsPressed &= !Mouse.current.leftButton.wasReleasedThisFrame;

    #endregion

    #region UI Event Handlers

    void OnPointerDown(PointerDownEvent evt)
      => IsPressed = true;

    #endregion
}
