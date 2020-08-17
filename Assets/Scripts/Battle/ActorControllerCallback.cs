using System;
using UnityEditor;
using UnityEngine;

public class ActorControllerCallback
{
    public Func<Ray?, RaycastHit?> GetRay { get; }
    public Func<Actor, Actor> GetNearestActor { get; }
    public Func<Collider, int?> GetTransitionMapId { get; }
    public Action<Actor, int> LeaveActor { get; }

    public ActorControllerCallback(
        Func<Ray?, RaycastHit?> getRay,
        Func<Actor, Actor> getNearestActor,
        Func<Collider, int?> getTransitionMapId,
        Action<Actor, int> leaveActor)
    {
        this.GetRay = getRay;
        this.GetNearestActor = getNearestActor;
        this.GetTransitionMapId = getTransitionMapId;
        this.LeaveActor = leaveActor;
    }
}
