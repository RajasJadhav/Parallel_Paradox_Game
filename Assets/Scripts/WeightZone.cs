using UnityEngine;

// Trigger zones on each side of the see-saw
// Tell the SeeSaw script when weight is added/removed

public class WeightZone : MonoBehaviour
{
    [Header("References")]
    public SeeSaw seeSaw;      // Drag the SeeSaw script here
    public string side = "left"; // "left" or "right"

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            seeSaw.AddWeight(side);
            Debug.Log($"WeightZone: {other.tag} added weight to {side} side.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            seeSaw.RemoveWeight(side);
        }
    }
}