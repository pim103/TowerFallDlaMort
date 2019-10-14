﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Menus
{
    public class MenuChooseAgent : MonoBehaviour, MenuInterface
    {
        [SerializeField]
        private Button backButton;

        [SerializeField]
        private MenuController menuController;

        private void BackButton()
        {
            menuController.ActiveMenu(Menus.MAIN_MENU);
        }

        public void InitMenu()
        {
            backButton.onClick.AddListener(BackButton);
            Debug.Log("Choose Agent Menu");
        }
    }
}