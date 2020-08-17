using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActorList : MonoBehaviour
{
    [SerializeField] CameraManager cameraManager;
    [SerializeField] Actor actorPrefab;
    [SerializeField] Transform actorParent;
    [SerializeField] BulletPool bulletPool;

    List<Actor> list = new List<Actor>();

    ActorControllerCallback actorControllerCallback;
    Action onDeath;

    public void Initialize(Func<Collider, int?> getTransitionMapId, Action<Actor, int> leaveActor, Action onDeath)
    {
        actorControllerCallback = new ActorControllerCallback(
            cameraManager.GetRaycastHit,
            GetNearestActor,
            getTransitionMapId,
            leaveActor);

        this.onDeath = onDeath;
    }

    public void JoinActor(ActorState actorState, Func<int, Vector3?> getTransitionCollider)
    {
        var joinPosition = getTransitionCollider(actorState.PrevMapId);
        if (!joinPosition.HasValue)
        {
            joinPosition = new Vector3(Random.Range(-25, 25), 10, Random.Range(-25, 25));
        }

        var actor = Instantiate<Actor>(actorPrefab, joinPosition.Value, Quaternion.identity, actorParent);
        list.Add(actor);

        actor.SetBulletSpawner(bulletPool.SpawnBullet);
        actor.SetOnDeath(OnDeath);

        if (actorState.IsPlayer)
        {
            actor.SetActorController(new PlayerActorController(actorState, actorControllerCallback));
            cameraManager.SetTrackTarget(actor.transform);
        }
        else
        {
            actor.SetActorController(new AIActorController(actorState, actorControllerCallback));
        }

    }

    public void LeaveActor(Actor actor)
    {
        Destroy(actor.gameObject);
        list.Remove(actor);
    }

    public void ClearActor()
    {
        cameraManager.SetTrackTarget(null);

        foreach (var actor in list)
        {
            Destroy(actor.gameObject);
        }

        list.Clear();
        bulletPool.Reset();
    }

    public Actor GetNearestActor(Actor from)
    {
        var aliveList = list.Where(x => !x.ActorState.IsDead).ToArray();

        if (aliveList.Length <= 1)
        {
            return null;
        }

        return aliveList
            .Where(to => from != to)
            .OrderBy(to => (from.transform.position - to.transform.position).sqrMagnitude)
            .First();
    }

    void OnDeath(Actor actor)
    {
        LeaveActor(actor);
        onDeath();
    }
}