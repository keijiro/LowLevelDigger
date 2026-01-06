using UnityEngine;

public sealed class FramerateSetter : MonoBehaviour
{
    [SerializeField] RuntimePlatform _platform = RuntimePlatform.IPhonePlayer;
    [SerializeField] int _targetFramerate = 60;

    void Start()
    {
        if (Application.platform == _platform)
            Application.targetFrameRate = _targetFramerate;
    }
}
