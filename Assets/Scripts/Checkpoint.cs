using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;
    public MainController main;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        main.RegisterCheckpoint(checkpointIndex);
    }
}