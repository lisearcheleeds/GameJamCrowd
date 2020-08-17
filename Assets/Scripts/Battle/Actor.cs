using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Actor : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform marker;
    [SerializeField] MeshRenderer markerRenderer;
    [SerializeField] LayerMask layer;
    [SerializeField] ParticleSystem deathParticle;

    public ActorState ActorState => actorController.ActorState;
    public bool IsPlayer => actorController is PlayerActorController;

    ActorController actorController;

    Action<Actor, Vector3, Vector3, float, int> spawnBullet;
    Action<Actor> onDeath;

    float gravity;
    bool[] prevIsGrounded = new bool[10];
    float jumpPower;

    int attackIntervalCount;
    bool isLineOfSight;

    public void SetActorController(ActorController actorController)
    {
        this.actorController = actorController;
    }

    public void SetBulletSpawner(Action<Actor, Vector3, Vector3, float, int> spawnBullet)
    {
        this.spawnBullet = spawnBullet;
    }

    public void SetOnDeath(Action<Actor> onDeath)
    {
        this.onDeath = onDeath;
    }

    public void OnKill(ActorState killedActor)
    {
        ActorState.OnKill(killedActor);
    }

    void Update()
    {
        if (ActorState.IsDead)
        {
            if (deathParticle.isStopped)
            {
                onDeath(this);
            }

            return;
        }

        actorController?.UpdateThink(this);

        UpdateMarker();
        UpdateMove();
        UpdateAttack();
    }

    void UpdateMarker()
    {
        isLineOfSight = false;

        if (actorController.NearestActor == null)
        {
            marker.gameObject.SetActive(false);
            return;
        }

        marker.gameObject.SetActive(true);
        marker.LookAt(actorController.NearestActor.transform);

        var direction = (actorController.NearestActor.transform.position - transform.position).normalized;
        var ray = new Ray(transform.position, direction);
        var raycastHit = new RaycastHit();

        if (Physics.Raycast(ray, out raycastHit, 1000.0f, LayerMask.GetMask(new[] { "Actor", "Map" })) && ((1 << raycastHit.transform.gameObject.layer) & layer.value) != 0)
        {
            markerRenderer.material.color = new Color(156.0f / 255.0f, 255.0f / 255.0f, 153.0f / 255.0f);
            isLineOfSight = true;
        }
        else
        {
            markerRenderer.material.color = Color.gray;
        }
    }

    void UpdateMove()
    {
        var move = Vector3.zero;

        // 移動
        if (actorController.WayPoint.HasValue)
        {
            move = (actorController.WayPoint.Value - transform.position).normalized * ActorState.MoveSpeed;
        }

        // 適当にジャンプっぽいものをする
        if (characterController.isGrounded)
        {
            move.y = 0.0f;
        }
        else
        {
            if (prevIsGrounded.Count(x => x) > prevIsGrounded.Length - 1)
            {
                jumpPower = 0.3f;
            }

            move.y = Mathf.Clamp(move.y - 0.1f + jumpPower, -0.6f, 3.0f);
            jumpPower = Mathf.Clamp(jumpPower - 0.005f, 0, 1.0f);
        }

        // 着地判定の更新
        var isGrounded = characterController.isGrounded;
        for (var i = 0; i < prevIsGrounded.Length; i++)
        {
            var tmp = prevIsGrounded[i];
            prevIsGrounded[i] = isGrounded;
            isGrounded = tmp;
        }

        characterController.Move(move);
    }

    void UpdateAttack()
    {
        if (attackIntervalCount <= ActorState.AttackInterval)
        {
            attackIntervalCount++;
        }

        if (actorController.NearestActor == null || !isLineOfSight)
        {
            return;
        }

        if (attackIntervalCount > ActorState.AttackInterval)
        {
            var direction = (actorController.NearestActor.transform.position - transform.position).normalized;
            spawnBullet(this, transform.position, direction, ActorState.BulletSpeed, ActorState.AttackDamage);

            attackIntervalCount = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ActorState.IsDead)
        {
            return;
        }

        actorController.OnCollisionEnter(this, collision);

        if (ActorState.IsDead)
        {
            deathParticle.Play();
        }
    }
}