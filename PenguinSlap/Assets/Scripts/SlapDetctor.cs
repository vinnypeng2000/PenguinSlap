using System.Collections.Generic;
using UnityEngine;

public class SlapDetector : MonoBehaviour
{
    public PlayerController playerController; // Reference to PlayerController

    private void OnTriggerEnter(Collider other)
    {
        EnemyBehavior enemy = other.GetComponent<EnemyBehavior>();
        if (enemy != null)
        {
            Debug.Log("Enemy entered slap zone: " + enemy.name);
            playerController.AddEnemyToList(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyBehavior enemy = other.GetComponent<EnemyBehavior>();
        if (enemy != null)
        {
            Debug.Log("Enemy exited slap zone: " + enemy.name);
            playerController.RemoveEnemyFromList(enemy);
        }
    }
}
