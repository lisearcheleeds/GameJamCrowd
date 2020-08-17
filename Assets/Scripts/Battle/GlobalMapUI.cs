using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GlobalMapUI : MonoBehaviour
{
    [SerializeField] Text mapText;
    [SerializeField] Text timeText;
    [SerializeField] Image timeImage;
    [SerializeField] Vector2 cellMagine;
    [SerializeField] GlobalMapCell cellTemplate;
    [SerializeField] RectTransform cellParent;

    public (int row, int column)[] Layouts { get; private set; }

    List<GlobalMapCell> globalMapCells = new List<GlobalMapCell>();
    public List<int> LockedMapIds { get; } = new List<int>();
    public int? NextLockMapId { get; private set; }

    int updateInterval;
    int cellMaxCount;

    public void StartBattle(int cellSideCount)
    {
        UpdateGlobalMapLayout(cellSideCount);

        foreach (var layout in Layouts)
        {
            var cell = Instantiate(cellTemplate, cellParent);
            var rowMax = cellSideCount * 2 - 1;
            var rowEvenNumberOffset = (layout.row % 2) * cellMagine.x * 1.5f;
            var rowOffset = ((rowMax / 2 - Mathf.Abs(layout.row - rowMax / 2)) + (layout.row % 2)) * cellMagine.x * 1.5f;
            var areaOffset = new Vector3(cellMagine.x * (cellSideCount + 0.5f), -cellMagine.y * (cellSideCount - 1.0f));

            cell.transform.localPosition = new Vector3(layout.column * cellMagine.x * 3.0f + rowEvenNumberOffset - rowOffset, -layout.row * cellMagine.y, 0) - areaOffset;
            globalMapCells.Add(cell);
        }
    }

    public void SetMapId(int mapId)
    {
        mapText.text = $"Map: {mapId}";
    }

    public void UpdateUI(List<ActorState> actorState, float gameTime)
    {
        gameObject.SetActive(Input.GetKey(KeyCode.Tab));

        var playerMapId = actorState.FirstOrDefault(x => x.IsPlayer)?.CurrentMapId ?? -1;

        updateInterval++;
        updateInterval %= 60;

        if (Input.GetKeyDown(KeyCode.Tab) || (Input.GetKey(KeyCode.Tab) && 0 == updateInterval))
        {
            for (var i = 0; i < globalMapCells.Count; i++)
            {
                var mapDangerStatus = (NextLockMapId.HasValue && NextLockMapId.Value == i) ? MapDangerStatus.Danger : MapDangerStatus.Usually;

                if (i == cellMaxCount / 2)
                {
                    mapDangerStatus = MapDangerStatus.Peace;
                }

                globalMapCells[i].SetGlobalMapCellState(
                    mapDangerStatus,
                    playerMapId == i,
                    actorState.Count(x => x.CurrentMapId == i && !x.IsDead),
                    LockedMapIds.Contains(i));
            }
        }

        if (updateInterval == 0)
        {
            var lockTime = 15;
            var countUp = gameTime % lockTime;
            var countDown = lockTime - countUp;
            timeText.text = $"{(int)countDown}";
            timeImage.fillAmount = countUp / lockTime;

            if (LockedMapIds.Count < cellMaxCount && LockedMapIds.Count < Mathf.FloorToInt(gameTime / lockTime))
            {
                if (NextLockMapId.HasValue)
                {
                    LockedMapIds.Add(NextLockMapId.Value);
                }

                var unlockMapIds = Enumerable.Range(0, cellMaxCount).Where(x => !LockedMapIds.Contains(x) && x != cellMaxCount / 2).ToArray();
                NextLockMapId = unlockMapIds[Random.Range(0, unlockMapIds.Length)];
            }
        }
    }

    void UpdateGlobalMapLayout(int cellSideCount)
    {
        var rowMax = cellSideCount * 2 - 1;
        cellMaxCount = (cellSideCount * cellSideCount * cellSideCount) - ((cellSideCount - 1) * (cellSideCount - 1) * (cellSideCount - 1));

        var mapLayout = new List<(int, int)>();
        var columnCount = new List<int>();

        var row = 0;
        var rowFirstIndex = 0;

        for (var i = 0; i < cellMaxCount; i++)
        {
            var offset = rowMax / 2 - Mathf.Abs(row - rowMax / 2);

            if (cellSideCount + offset <= i - rowFirstIndex)
            {
                rowFirstIndex = i;
                row++;
            }

            var column = i - rowFirstIndex;
            mapLayout.Add((row, column));
        }

        Layouts = mapLayout.ToArray();
    }
}