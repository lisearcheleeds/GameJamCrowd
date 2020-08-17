using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] Button[] skillButtons;
    [SerializeField] GameObject levelupGuide;

    ActorState playerActorState;

    public void StartBattle(ActorState playerActorState)
    {
        this.playerActorState = playerActorState;

        for (var i = 0; i < skillButtons.Length; i++)
        {
            var index = i;
            skillButtons[i].onClick.AddListener(() => OnClickSkillButton(index));
        }
    }

    public void UpdateUI()
    {
        var isSelected = playerActorState.Level == playerActorState.Skills.Count(x => x);

        levelupGuide.SetActive(!isSelected);

        if (!isSelected || Input.GetKey(KeyCode.LeftShift))
        {
            gameObject.SetActive(true);

            for (var i = 0; i < playerActorState.Skills.Length; i++)
            {
                skillButtons[i].targetGraphic.raycastTarget = !isSelected;
                skillButtons[i].interactable = !playerActorState.Skills[i];
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnClickSkillButton(int index)
    {
        playerActorState.LearnSkill(index);
    }
}