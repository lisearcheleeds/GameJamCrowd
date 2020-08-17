using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GlobalActorManager
{
    public List<ActorState> ActorStateList { get; private set; } = new List<ActorState>();

    int? latestPlayerMapId;
    int stateChangeCount;

    public void StartBattle(int actorCount)
    {
        for (var i = 0; i < actorCount; i++)
        {
            ActorStateList.Add(new ActorState(i, i, i == 0));
        }
    }

    public void TransitionActor(Actor actor, int transitionMapId)
    {
        ActorStateList.First(x => x.Id == actor.ActorState.Id).TransitionMap(transitionMapId);
    }

    public void Update(int? nextLockMapId, Func<int, int[]> getTransitionAdjacentMapIds, Action<ActorState> joinActor, Action onDeath)
    {
        stateChangeCount++;
        stateChangeCount %= 30;

        if (stateChangeCount == 0)
        {
            latestPlayerMapId = ActorStateList.FirstOrDefault(x => x.IsPlayer)?.CurrentMapId;

            foreach (var actorState in ActorStateList)
            {
                if (actorState.IsDead)
                {
                    continue;
                }

                // プレイヤーが見てるので突然マップ移動とかさせられない
                if (actorState.CurrentMapId == latestPlayerMapId)
                {
                    continue;
                }

                actorState.Regeneration();

                var dice = UnityEngine.Random.Range(0, 100);

                if (actorState.CurrentMapId == nextLockMapId)
                {
                    dice = 0;
                }

                if (dice < 2)
                {
                    var transitonMapIds = getTransitionAdjacentMapIds(actorState.CurrentMapId);
                    if (transitonMapIds.Length > 0)
                    {
                        var randomIndex = UnityEngine.Random.Range(0, transitonMapIds.Length);
                        actorState.TransitionMap(transitonMapIds[randomIndex]);

                        if (transitonMapIds[randomIndex] == latestPlayerMapId)
                        {
                            joinActor(actorState);
                        }
                    }

                    continue;
                }

                if (dice < 4)
                {
                    actorState.GetExp(5);
                }

                if (dice < 5)
                {
                    var fightActors = ActorStateList.Where(x => x.CurrentMapId == actorState.CurrentMapId && x.Id != actorState.Id && !x.IsDead);
                    foreach (var fightActor in fightActors)
                    {
                        fightActor.Damage(actorState.AttackDamage);
                        if (fightActor.IsDead)
                        {
                            actorState.OnKill(fightActor);
                            onDeath();
                        }
                    }
                }
            }
        }
    }
}