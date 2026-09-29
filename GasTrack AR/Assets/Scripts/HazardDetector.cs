using UnityEngine;

public class HazardDetector : MonoBehaviour
{
    // Drag a UI Text or Canvas into this slot in the Inspector
    public GameObject warningUI;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hazard"))
        {
            warningUI.SetActive(true);
            // Optional: Play an alarm sound here
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hazard"))
        {
            warningUI.SetActive(false);
        }
    }
}
