using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Menus
{
    public class MainMenu : MonoBehaviour, MenuInterface
    {
        [SerializeField]
        private MenuController menuController;

        [SerializeField]
        private Button playButton;

        [SerializeField]
        private Button quitButton;

        private void Start()
        {
            playButton.onClick.AddListener(WantToPlayButton);
            quitButton.onClick.AddListener(WantToQuit);
        }

        private void WantToPlayButton()
        {
            menuController.ActiveMenu(Menus.CHOOSE_AGENT_MENU);
        }

        private void WantToQuit()
        {
            Application.Quit();
        }

        public void InitMenu()
        {
            Debug.Log("Main Menu");
        }
    }
}