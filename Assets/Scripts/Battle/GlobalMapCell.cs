using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GlobalMapCell : MonoBehaviour
{
    [SerializeField] Image[] dangerImages;
    [SerializeField] GameObject currentPositionFade;
    [SerializeField] Text counterText;
    [SerializeField] GameObject isLocked;
    [SerializeField] GameObject isUnLock;

    [SerializeField] Color[] dangerColors;

    public void SetGlobalMapCellState(MapDangerStatus mapDangerState, bool isPlayerPosition, int counter, bool isLocked)
    {
        switch (mapDangerState)
        {
            case MapDangerStatus.Peace:
                foreach (var dangerImage in dangerImages)
                {
                    dangerImage.color = dangerColors[0];
                }
                break;

            case MapDangerStatus.Usually:
                foreach (var dangerImage in dangerImages)
                {
                    dangerImage.color = dangerColors[1];
                }
                break;

            case MapDangerStatus.Danger:
                foreach (var dangerImage in dangerImages)
                {
                    dangerImage.color = dangerColors[2];
                }
                break;
        }

        currentPositionFade.gameObject.SetActive(isPlayerPosition);
        counterText.text = counter != 0 ? $"{counter}" : "";
        this.isLocked.gameObject.SetActive(isLocked);
        this.isUnLock.gameObject.SetActive(!isLocked);
    }
}