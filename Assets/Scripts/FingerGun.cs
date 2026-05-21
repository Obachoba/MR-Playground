using UnityEngine;

public class FingerGun : MonoBehaviour
{
    public Transform indexTip;

    public GameObject projectilePrefab;

    public float projectileSpeed = 1f;

    public float fireRate = 0.2f;

    private float _nextFireTime = 0f;
    private bool _isShooting = false;

    void Update()
    {
        if (_isShooting && Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + fireRate;
        }
    }

    public void FingerGunActivated()
    {
        _isShooting = true;
        _nextFireTime = Time.time;
    }


    public void FingerGunDeactivated()
    {
        _isShooting = false;
    }

    private void Shoot()
    {
        if (indexTip == null) return;

        GameObject projectile = Instantiate(projectilePrefab, indexTip.position, indexTip.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = indexTip.forward * projectileSpeed;
        }

        Destroy(projectile, 3f);
    }
}
