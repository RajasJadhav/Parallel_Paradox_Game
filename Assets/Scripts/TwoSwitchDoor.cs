using UnityEngine;

// Coordinates two pressure plates to control one door
// Both plates must be active simultaneously

public class TwoSwitchDoor : MonoBehaviour, IResettable
{
    [Header("References")]
    public PressurePlate switchA;     // Drag SwitchA here
    public PressurePlate switchB;     // Drag SwitchB here
    public Door linkedDoor;  // Drag the Door here

    private bool wasOpen = false;

    void Update()
    {
        // Check if both switches are currently active
        // We read the isActive state from each plate
        bool bothActive = switchA.IsActive() && switchB.IsActive();

        if (bothActive && !wasOpen)
        {
            linkedDoor.Open();
            wasOpen = true;
            Debug.Log("TwoSwitchDoor: Both switches held — door opened.");
        }
        else if (!bothActive && wasOpen)
        {
            linkedDoor.Close();
            wasOpen = false;
            Debug.Log("TwoSwitchDoor: A switch released — door closed.");
        }
    }

    public void ResetObject()
    {
        wasOpen = false;
        Debug.Log("TwoSwitchDoor: Reset.");
    }
}