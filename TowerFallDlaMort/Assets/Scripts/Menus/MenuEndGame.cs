using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Games;
using Scripts.Menus;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuEndGame : MonoBehaviour, MenuInterface
{
    [SerializeField] 
    private Text endGameText;

    [SerializeField] 
    private Button backToMenu;

    [SerializeField] 
    private GameController gameController;

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void Start()
    {
        backToMenu.onClick.AddListener(BackToMenu);
    }

    public void InitMenu()
    {
        switch (gameController.winnerId)
        {
            case -1 :
                endGameText.text = "Match Nul";
                break;
            case 0:
                endGameText.text = "Victoire du joueur 1 !";
                break;
            case 1:
                endGameText.text = "Victoire du joueur 2 !";
                break;
        }
        
        Debug.Log("EndGame");
    }
}
