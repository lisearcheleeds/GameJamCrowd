using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject titleUI;
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject resultUI;
    [SerializeField] Text resultText;

    int state;

    void Start()
    {
        state = 0;
        titleUI.SetActive(true);
        gameUI.SetActive(false);
        resultUI.SetActive(false);
    }

    void Update()
    {
        switch (state)
        {
            case 0:
                if (Input.anyKeyDown)
                {
                    state = 1;
                    gameManager.StartBattle();
                    titleUI.SetActive(false);
                    gameUI.SetActive(true);
                    resultUI.SetActive(false);
                }
                break;
            case 1:
                if (gameManager.State != 1)
                {
                    state = 2;

                    titleUI.SetActive(false);
                    gameUI.SetActive(false);
                    resultUI.SetActive(true);
                    resultText.text = $"Ranking {gameManager.AliveActorCount}!";
                }
                break;

        }
    }
}
