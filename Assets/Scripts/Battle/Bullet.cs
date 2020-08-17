using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool IsActive => gameObject.activeSelf;
    public Actor Owner { get; private set; }
    public int AttackDamage { get; private set; }

    Vector3 direction;
    float bulletSpeed;

    int lifeTime;

    public void SpawnBullet(Actor owner, Vector3 position, Vector3 direction, float bulletSpeed, int attackDamage)
    {
        gameObject.SetActive(true);

        transform.position = position + direction * bulletSpeed;
        this.direction = direction;
        this.bulletSpeed = bulletSpeed;

        Owner = owner;
        AttackDamage = attackDamage;

        transform.LookAt(position + direction);

        lifeTime = 300;
    }

    void Update()
    {
        transform.position += direction * bulletSpeed;

        lifeTime--;
        if (lifeTime <= 0)
        {
            Reset();
        }
    }

    public void Reset()
    {
        gameObject.SetActive(false);
    }
}
