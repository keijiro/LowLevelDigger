using UnityEngine;
using UnityEngine.UIElements;

public sealed class Scoreboard : MonoBehaviour
{
    #region Editable Fields

    [Space]
    [SerializeField] ParticleSystem _coinEmitter = null;
    [SerializeField] ParticleSystem _heartEmitter = null;
    [SerializeField] int _scoreSpeed = 500;

    #endregion

    #region Private Members

    UIDocument _ui;
    Label _scoreText;

    (int current, int display) _score;

    #endregion

    #region Public Methods

    public void Award()
    {
        _coinEmitter.Emit(20);
        _heartEmitter.Play();
        _score.current += 20;
    }

    public void Tip()
    {
        _coinEmitter.Emit(1);
        _score.current++;
    }

    public void Penalize()
    {
        _score.current -= 10;
    }

    #endregion

    #region MonoBehaviour Methods

    void Start()
    {
        _ui = GetComponent<UIDocument>();
        _scoreText = _ui.rootVisualElement.Q<Label>("score-text");
        _score.current = 1000;
    }

    void Update()
    {
        if (_score.display < _score.current)
        {
            _score.display += (int)(_scoreSpeed * Time.deltaTime);
            _score.display = Mathf.Min(_score.display, _score.current);
        }
        else if (_score.display > _score.current)
        {
            _score.display -= (int)(_scoreSpeed * Time.deltaTime);
            _score.display = Mathf.Max(_score.display, _score.current);
        }
        else
        {
            return;
        }

        _scoreText.text = $"{_score.display:N0}";
    }

    #endregion
}
