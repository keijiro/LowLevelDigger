using UnityEngine;
using UnityEngine.UIElements;

public interface IStageInitializable
{
    void InitializeStage(StageManager stage);
}

public class StageManager : MonoBehaviour
{
    [SerializeField] UIDocument _ui = null;
    [SerializeField] PaydirtManager _paydirtManager = null;
    [Space]
    [SerializeField] Animation _bucketAnimation = null;
    [SerializeField] float _bucketCloseWait = 2;
    [Space]
    [SerializeField] MonoBehaviour[] _initializers = null;

    Button _flushButton;

    void Start()
    {
        var root = _ui.rootVisualElement;
        _flushButton = root.Q<Button>("flush-button");
        _flushButton.clicked += OnFlushClicked;

        foreach (var initializer in _initializers)
            ((IStageInitializable)initializer).InitializeStage(this);
    }

    async void OnFlushClicked()
    {
        ConsoleManager.AddLine("Flush started.");

        _bucketAnimation.Play("HatchOpen");

        await Awaitable.WaitForSecondsAsync(_bucketCloseWait);

        _paydirtManager.RequestInjection();
        _bucketAnimation.Play("HatchClose");
    }
}
