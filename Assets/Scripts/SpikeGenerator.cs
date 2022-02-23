using UnityEngine;

public class SpikeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject spike;
    [SerializeField] private float startOffset = 0f;
    [SerializeField] private float spawnEvery = 3f;

    private bool offsetReached;
    private float currentOffsetTimer;
    private float currentSpawnTimer;

    void Update()
    {
        if (!offsetReached)
        {
            this.currentOffsetTimer += Time.deltaTime;
            if (this.currentOffsetTimer >= this.startOffset)
                this.offsetReached = true;

            return;
        }

        this.currentSpawnTimer += Time.deltaTime;
        if (this.currentSpawnTimer >= this.spawnEvery)
        {
            Instantiate(spike, this.transform);
            this.currentSpawnTimer = 0f;
        }
    }
}
