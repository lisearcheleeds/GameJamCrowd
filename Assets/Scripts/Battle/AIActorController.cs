using System.Linq;
using UnityEditor;
using UnityEngine;

public class AIActorController : ActorController
{
    int tryCount = 3;
    int resetCount = 0;

    public AIActorController(ActorState actorState, ActorControllerCallback actorControllerCallback)
    {
        ActorState = actorState;
        this.actorControllerCallback = actorControllerCallback;
    }

    protected override void OnUpdateThink(Actor actor)
    {
        if (0 < resetCount)
        {
            resetCount--;
            return;
        }

        resetCount = 300;

        var isSelected = ActorState.Level == ActorState.Skills.Count(x => x);
        if (isSelected)
        {
            // そのうち習得出来る
            ActorState.LearnSkill(Random.Range(0, ActorState.Skills.Length));
        }

        // 移動
        for (var i = 0; i < tryCount; i++)
        {
            var randOffset = new Vector3(UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20));
            var raycastHit = actorControllerCallback.GetRay(new Ray(actor.transform.position + randOffset + Vector3.up * 100.0f, Vector3.down));

            // TODO: あとで当たり判定を避けたりさせる
            if (raycastHit.HasValue)
            {
                WayPoint = new Vector3(raycastHit.Value.point.x, 0, raycastHit.Value.point.z);
            }
        }
    }
}
