using UnityEditor;
using UnityEngine;

public abstract class ActorController
{
    protected ActorControllerCallback actorControllerCallback;

    public ActorState ActorState { get; protected set; }
    public Vector3? WayPoint { get; protected set; }
    public Actor NearestActor { get; protected set; }

    public void UpdateThink(Actor actor)
    {
        ActorState.Regeneration();
        NearestActor = actorControllerCallback.GetNearestActor(actor);

        OnUpdateThink(actor);
    }

    protected abstract void OnUpdateThink(Actor actor);

    public void OnCollisionEnter(Actor actor, Collision collision)
    {
        // map
        if (collision.gameObject.layer == 8)
        {
            var transitionMapId = actorControllerCallback.GetTransitionMapId(collision.collider);
            if (transitionMapId.HasValue && ActorState.PrevMapId != transitionMapId)
            {
                actorControllerCallback.LeaveActor(actor, transitionMapId.Value);
            }
        }

        // bullet
        if (collision.gameObject.layer == 10)
        {
            var bullet = collision.gameObject.GetComponent<BulletColliderConnector>()?.Bullet;
            if (bullet != null && bullet.Owner != actor)
            {
                ActorState.Damage(bullet.AttackDamage);
                if (ActorState.IsDead)
                {
                    bullet.Owner.OnKill(actor.ActorState);
                }
            }
        }
    }
}