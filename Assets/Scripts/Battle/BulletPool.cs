using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform poolParent;

    List<Bullet> pool = new List<Bullet>();

    public void SpawnBullet(Actor owner, Vector3 position, Vector3 direction, float bulletSpeed, int attackDamage)
    {
        var bullet = pool.FirstOrDefault(b => !b.IsActive);

        if (bullet == null)
        {
            bullet = Instantiate(bulletPrefab, poolParent);
            pool.Add(bullet);
        }

        bullet.SpawnBullet(owner, position, direction, bulletSpeed, attackDamage);
    }

    public void Reset()
    {
        foreach (var bullet in pool)
        {
            bullet.Reset();
        }
    }
}