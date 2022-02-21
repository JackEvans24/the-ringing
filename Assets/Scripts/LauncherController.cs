using UnityEngine;

public class LauncherController : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float shootInterval;

    [SerializeField] private float projectileSpeed;
    [SerializeField] private Vector2 projectileDirection;

    private float currentInterval;

    // Update is called once per frame
    void Update()
    {
        this.currentInterval += Time.deltaTime;

        if (this.currentInterval > shootInterval)
        {
            // Instantiate projectile
            this.currentInterval = 0f;
            var obj = Instantiate(projectile, this.transform.position, this.transform.rotation);

            // Set projectile properties
            var projectileScript = obj.GetComponent<Projectile>();
            if (Mathf.Abs(this.projectileSpeed) > 0)
                projectileScript.speed = this.projectileSpeed;
            if (projectileDirection != Vector2.zero)
                projectileScript.direction = this.projectileDirection;
        }
    }
}
