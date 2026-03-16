using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int checkpointsCount;
    [SerializeField] private int checkpointIndex;
    [SerializeField] private bool isStart = false;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(isStart)
                gameManager.RegisterStart(checkpointsCount);
            else
                gameManager.RegisterCheckpoint(checkpointIndex);
        }
    }
}
