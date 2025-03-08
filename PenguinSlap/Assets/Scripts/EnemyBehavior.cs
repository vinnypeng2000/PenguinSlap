using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private GameObject player; // Reference to the player
    private Vector3 flingDirection; // Direction to fling the enemy
    private Rigidbody rb; // Reference to the Rigidbody component

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger zone and has the tag "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.gameObject; // Store the player reference
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player leaves the trigger zone, reset the flag
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null; // Reset player reference
        }
    }

    public void OnSlap()
    {
        if (isPlayerInRange && player != null)
        {
            // Calculate the direction from the enemy to the player
            Vector3 directionToPlayer = (transform.position - player.transform.position).normalized;

            // Calculate the fling direction (45 degrees upwards away from the player)
            flingDirection = directionToPlayer + Vector3.up; // Add upward component for fling
            flingDirection.Normalize(); // Ensure the direction vector is normalized

            // Apply force to the enemy's Rigidbody to fling it away from the player
            rb.AddForce(flingDirection * 10f, ForceMode.Impulse); // Adjust the multiplier as needed for force
        }
    }
}
