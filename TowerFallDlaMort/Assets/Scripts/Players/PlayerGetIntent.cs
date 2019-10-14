using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Scripts.Players
{
    public class PlayerGetIntent : PlayerIntent
    {
        [SerializeField]
        private int id;
        [SerializeField] 
        private PlayerGetIntent otherPlayer;
        public void Update()
        {
            if (!otherPlayer || id == 1)
            {
                if (Input.GetKeyDown(KeyCode.Z) && !Input.GetKeyDown(KeyCode.S))
                {
                    wantToMoveForward = true;
                }
                if (Input.GetKeyUp(KeyCode.Z))
                {
                    wantToMoveForward = false;
                }
                if (Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.Z))
                {
                    wantToMoveBack = true;
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    wantToMoveBack = false;
                }
                if (Input.GetKeyDown(KeyCode.Q) && !Input.GetKeyDown(KeyCode.D))
                {
                    wantToMoveLeft = true;
                }
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    wantToMoveLeft = false;
                }
                if (Input.GetKeyDown(KeyCode.D) && !Input.GetKeyDown(KeyCode.Q))
                {
                    wantToMoveRight = true;
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    wantToMoveRight = false;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    wantToBlock = true;
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    wantToBlock = false;
                }
                if (!otherPlayer)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        wantToShootUp = true;
                    }
                    if (Input.GetKeyUp(KeyCode.UpArrow))
                    {
                        wantToShootUp = false;
                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow)  && !Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        wantToShootDown = true;
                    }
                    if (Input.GetKeyUp(KeyCode.DownArrow))
                    {
                        wantToShootDown = false;
                    }
                    if (Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        wantToShootLeft = true;
                    }
                    if (Input.GetKeyUp(KeyCode.LeftArrow))
                    {
                        wantToShootLeft = false;
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        wantToShootRight = true;
                    }
                    if (Input.GetKeyUp(KeyCode.RightArrow))
                    {
                        wantToShootRight = false;
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Y) && !Input.GetKeyDown(KeyCode.H))
                    {
                        wantToShootUp = true;
                    }
                    if (Input.GetKeyUp(KeyCode.Y))
                    {
                        wantToShootUp = false;
                    }
                    if (Input.GetKeyDown(KeyCode.H)  && !Input.GetKeyDown(KeyCode.Y))
                    {
                        wantToShootDown = true;
                    }
                    if (Input.GetKeyUp(KeyCode.H))
                    {
                        wantToShootDown = false;
                    }
                    if (Input.GetKeyDown(KeyCode.G) && !Input.GetKeyDown(KeyCode.J))
                    {
                        wantToShootLeft = true;
                    }
                    if (Input.GetKeyUp(KeyCode.G))
                    {
                        wantToShootLeft = false;
                    }
                    if (Input.GetKeyDown(KeyCode.J) && !Input.GetKeyDown(KeyCode.G))
                    {
                        wantToShootRight = true;
                    }
                    if (Input.GetKeyUp(KeyCode.J))
                    {
                        wantToShootRight = false;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
                {
                    wantToMoveForward = true;
                }
                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    wantToMoveForward = false;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.UpArrow))
                {
                    wantToMoveBack = true;
                }
                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    wantToMoveBack = false;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
                {
                    wantToMoveLeft = true;
                }
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    wantToMoveLeft = false;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    wantToMoveRight = true;
                }
                if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    wantToMoveRight = false;
                }

                if (Input.GetKeyDown(KeyCode.Keypad8) && !Input.GetKeyDown(KeyCode.Keypad5))
                {
                    wantToShootUp = true;
                }
                if (Input.GetKeyUp(KeyCode.Keypad8))
                {
                    wantToShootUp = false;
                }
                if (Input.GetKeyDown(KeyCode.Keypad5)  && !Input.GetKeyDown(KeyCode.Keypad8))
                {
                    wantToShootDown = true;
                }
                if (Input.GetKeyUp(KeyCode.Keypad5))
                {
                    wantToShootDown = false;
                }
                if (Input.GetKeyDown(KeyCode.Keypad4) && !Input.GetKeyDown(KeyCode.Keypad6))
                {
                    wantToShootLeft = true;
                }
                if (Input.GetKeyUp(KeyCode.Keypad4))
                {
                    wantToShootLeft = false;
                }
                if (Input.GetKeyDown(KeyCode.Keypad6) && !Input.GetKeyDown(KeyCode.Keypad4))
                {
                    wantToShootRight = true;
                }
                if (Input.GetKeyUp(KeyCode.Keypad6))
                {
                    wantToShootRight = false;
                }
                if (Input.GetKeyDown(KeyCode.Keypad0))
                {
                    wantToBlock = true;
                }
                if (Input.GetKeyUp(KeyCode.Keypad0))
                {
                    wantToBlock = false;
                }
            }
        }
    }
}