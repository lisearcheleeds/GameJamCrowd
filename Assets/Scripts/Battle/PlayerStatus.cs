using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] Slider hpSlider;

    ActorState playerState;

    public void StartBattle(ActorState playerState)
    {
        this.playerState = playerState;
    }

    void Update()
    {
        if (playerState == null)
        {
            return;
        }

        hpSlider.value = playerState.HP / playerState.HPMax;
    }
}