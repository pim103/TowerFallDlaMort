using Scripts.Games;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Menus
{
    public enum Menus
    {
        MAIN_MENU,
        CHOOSE_AGENT_MENU,
        END_GAME_MENU
    }

    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainMenu;

        [SerializeField]
        private GameObject chooseAgentMenu;

        [SerializeField] 
        private GameObject endGameMenu;
        
        [SerializeField]
        private GameObject menuContainer;

        [SerializeField]
        private GameObject gameContainer;

        [SerializeField]
        public GameController gameController;

        public void DesactiveMenu()
        {
            mainMenu.SetActive(false);
            chooseAgentMenu.SetActive(false);
            endGameMenu.SetActive(false);
        }

        public void ActiveMenu(Menus typeMenu)
        {
            DesactiveMenu();

            MenuInterface menuInt = null;
            GameObject menuSelected = null;

            switch(typeMenu)
            {
                case Menus.MAIN_MENU:
                    menuSelected = mainMenu;
                    break;
                case Menus.CHOOSE_AGENT_MENU:
                    menuSelected = chooseAgentMenu;
                    break;
                case Menus.END_GAME_MENU:
                    menuSelected = endGameMenu;
                    break;
            }

            if(menuSelected != null)
            {
                menuSelected.SetActive(true);
                menuInt = menuSelected.GetComponent<MenuInterface>();

                menuInt.InitMenu();
            }
        }

        public void StartGame(bool enablePaintWeapon)
        {
            gameContainer.SetActive(true);
            menuContainer.SetActive(false);

            gameController.InitStartGame(enablePaintWeapon);
        }

        public void ReturnMenu()
        {
            // TODO : Create Restart button, add parameter to return

            gameContainer.SetActive(false);
            menuContainer.SetActive(true);

            ActiveMenu(Menus.MAIN_MENU);
        }

        // Start is called before the first frame update
        void Start()
        {
            ActiveMenu(Menus.MAIN_MENU);
        }
    }
}