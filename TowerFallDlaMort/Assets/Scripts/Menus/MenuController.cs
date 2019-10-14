using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Menus
{
    public enum Menus
    {
        MAIN_MENU,
        CHOOSE_AGENT_MENU
    }

    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainMenu;

        [SerializeField]
        private GameObject chooseAgentMenu;

        public void DesactiveMenu()
        {
            mainMenu.SetActive(false);
            chooseAgentMenu.SetActive(false);
        }

        public void ActiveMenu(Menus typeMenu)
        {
            DesactiveMenu();

            MenuInterface menuInt;
            GameObject menuSelected;

            switch(typeMenu)
            {
                case Menus.MAIN_MENU:
                    menuSelected = mainMenu;
                    break;
                case Menus.CHOOSE_AGENT_MENU:
                    menuSelected = chooseAgentMenu;
                    break;
                default:
                    menuSelected = null;
                    break;
            }

            if(menuSelected != null)
            {
                menuSelected.SetActive(true);
                menuInt = menuSelected.GetComponent<MenuInterface>();

                menuInt.InitMenu();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            ActiveMenu(Menus.MAIN_MENU);
        }
    }
}