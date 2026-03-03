using UnityEngine;

public class Start : MonoBehaviour
{
    public MainController main;
    public int checkpointsCount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        main.RegisterStart(checkpointsCount);
    }
}