using UnityEngine;

public class SpringEnergy : MonoBehaviour
{
    public float baseLaunchForce = 8f;
    public float energyPerGhostHit = 6f;

    private float storedEnergy = 0f;

    void OnTriggerEnter(Collider other)
    {
        // Ghost adds energy
        if (other.CompareTag("Ghost"))
        {
            storedEnergy += energyPerGhostHit;
            Debug.Log("Energy Stored: " + storedEnergy);
        }

        // Player gets launched
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                float totalForce = baseLaunchForce + storedEnergy;

                rb.AddForce(Vector3.up * totalForce, ForceMode.Impulse);

                Debug.Log("Launched with: " + totalForce);

                storedEnergy = 0f; // reset
            }
        }
    }
}