using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class ContactManager : MonoBehaviour
{
    public bool IsGemInTray { get; set; }

    void OnEnable()
      => PhysicsEvents.PostSimulate += OnPostSimulate;

    void OnDisable()
      => PhysicsEvents.PostSimulate -= OnPostSimulate;

    void OnPostSimulate(PhysicsWorld world, float timeStep)
    {
        foreach (var e in world.triggerBeginEvents)
        {
            IsGemInTray = true;
            Debug.Log("Gem entered the tray.");
        }
    }
}
