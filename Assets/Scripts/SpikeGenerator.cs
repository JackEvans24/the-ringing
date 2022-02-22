using UnityEngine;

public class SpikeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject spike;
    [SerializeField] private float spawnEvery = 3f;

    private float currentSpawnTimer;

    void Update()
    {
        this.currentSpawnTimer += Time.deltaTime;
        if (this.currentSpawnTimer >= this.spawnEvery)
        {
            Instantiate(spike, this.transform);
            this.currentSpawnTimer = 0f;
        }
    }
}
