using UnityEngine;


// This script detects when the player overtakes an AI car and updates the game state accordingly.
public class OvertakeDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has the tag "CarAI"
        if (other.CompareTag("CarAI"))
        {
            // inform the GameManager that an overtake has occurred
            GameManager.instance.IncrementOvertakeCount();

            
        }
    }
}