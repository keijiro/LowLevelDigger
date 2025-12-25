using UnityEngine;
using UnityEngine.UIElements;

public class BucketController : MonoBehaviour
{
    [field:SerializeField] Bucket _bucket = null;
    [field:SerializeField] UIDocument _ui = null;
    [field:SerializeField] float _openAngle = 60f;
    [field:SerializeField] float _moveSpeed = 180f;

    Button _openButton;
    Button _closeButton;
    float _targetAngle;
    float _currentAngle;

    void OnEnable()
    {
        if (_bucket == null || _ui == null)
            return;

        var root = _ui.rootVisualElement;

        _openButton = root.Q<Button>("open-button");
        _closeButton = root.Q<Button>("close-button");

        if (_openButton != null)
            _openButton.clicked += OnOpenClicked;
        if (_closeButton != null)
            _closeButton.clicked += OnCloseClicked;

        _currentAngle = _bucket.BottomAngle;
        _targetAngle = _currentAngle;
    }

    void OnDisable()
    {
        if (_openButton != null)
            _openButton.clicked -= OnOpenClicked;
        if (_closeButton != null)
            _closeButton.clicked -= OnCloseClicked;
    }

    void Update()
    {
        if (_bucket == null)
            return;

        var nextAngle = Mathf.MoveTowards(_currentAngle, _targetAngle, _moveSpeed * Time.deltaTime);
        if (Mathf.Approximately(nextAngle, _currentAngle))
            return;

        _currentAngle = nextAngle;
        _bucket.BottomAngle = _currentAngle;
    }

    void OnOpenClicked()
      => _targetAngle = _openAngle;

    void OnCloseClicked()
      => _targetAngle = 0f;
}
