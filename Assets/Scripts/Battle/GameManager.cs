using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GlobalMapUI globalMapUI;
    [SerializeField] MapManager mapManager;
    [SerializeField] LevelUpUI levelUpUI;
    [SerializeField] PlayerStatus playerStatus;

    [SerializeField] ActorList actorList;

    [SerializeField] int cellSideCount;

    GlobalActorManager globalActorManager = new GlobalActorManager();
    float startTime;

    public int State { get; private set; }
    public int AliveActorCount => globalActorManager.ActorStateList.Count(x => !x.IsDead);

    public void StartBattle()
    {
        Debug.Log("StartBattle");

        globalActorManager.StartBattle((cellSideCount * cellSideCount * cellSideCount) - ((cellSideCount - 1) * (cellSideCount - 1) * (cellSideCount - 1)));
        globalMapUI.StartBattle(cellSideCount);

        actorList.Initialize(GetTransitionMapId, LeaveActor, OnDeath);

        var playerState = globalActorManager.ActorStateList.First(x => x.IsPlayer);
        LoadMap(playerState.CurrentMapId);

        levelUpUI.StartBattle(playerState);
        playerStatus.StartBattle(playerState);

        startTime = Time.time;
        State = 1;
    }

    void Update()
    {
        if (State != 1)
        {
            return;
        }

        var gameTime = Time.time - startTime;

        globalMapUI.UpdateUI(globalActorManager.ActorStateList, gameTime);
        levelUpUI.UpdateUI();
        globalActorManager.Update(globalMapUI.NextLockMapId, GetTransitionAdjacentMapIds, (actor) => actorList.JoinActor(actor, mapManager.GetTransitionCollider), OnDeath);
    }

    void LoadMap(int mapId)
    {
        Debug.Log($"LoadMap {mapId}");

        mapManager.Load(MapMaster.GetMapVO(mapId, globalMapUI.Layouts, cellSideCount));
        globalMapUI.SetMapId(mapId);

        var currentMapActors = globalActorManager.ActorStateList.Where(x => x.CurrentMapId == mapId && !x.IsDead).ToArray();
        foreach (var currentMapActor in currentMapActors)
        {
            actorList.JoinActor(currentMapActor, mapManager.GetTransitionCollider);
        }
    }

    int? GetTransitionMapId(Collider collider)
    {
        var mapId = mapManager.GetTransitionMapId(collider);
        return (mapId.HasValue && !globalMapUI.LockedMapIds.Contains(mapId.Value)) ? mapId : null;
    }

    int[] GetTransitionAdjacentMapIds(int mapId)
    {
        var mapIds = MapMaster.GetTransitionMapIds(mapId, globalMapUI.Layouts, cellSideCount);
        return mapIds.Where(x => x.HasValue && !globalMapUI.LockedMapIds.Contains(x.Value)).Select(x => x.Value).ToArray();
    }

    void LeaveActor(Actor actor, int transitionMapId)
    {
        globalActorManager.TransitionActor(actor, transitionMapId);

        if (actor.IsPlayer)
        {
            actorList.ClearActor();
            LoadMap(transitionMapId);
        }
        else
        {
            actorList.LeaveActor(actor);
        }
    }

    void OnDeath()
    {
        Debug.Log("OnDeath");
        var aliveList = globalActorManager.ActorStateList.Where(x => !x.IsDead).ToArray();

        if (!aliveList.Any(x => x.IsPlayer))
        {
            State = 3;
            return;
        }

        Debug.Log("aliveList : " + aliveList.Length);
        if (aliveList.Length <= 1)
        {
            State = 2;
            return;
        }
    }
}