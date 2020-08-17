using System;
using UnityEditor;
using UnityEngine;

public class PlayerActorController : ActorController
{
    public PlayerActorController(ActorState actorState, ActorControllerCallback actorControllerCallback)
    {
        ActorState = actorState;
        this.actorControllerCallback = actorControllerCallback;
    }

    protected override void OnUpdateThink(Actor actor)
    {
        if (Input.GetMouseButton(1))
        {
            var raycastHit = actorControllerCallback.GetRay(null);
            if (raycastHit.HasValue)
            {
                WayPoint = new Vector3(raycastHit.Value.point.x, 0, raycastHit.Value.point.z);
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            WayPoint = null;
        }
    }
}
