using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsAndroid : MonoBehaviour
{
    public bool[] moveFoward = {false, false, false};
    public bool[] moveBack = {false, false, false};
    public bool[] moveLeft = {false, false, false};
    public bool[] moveRight = {false, false, false};
    public bool[] shootFoward = {false, false, false};
    public bool[] shootBack = {false, false, false};
    public bool[] shootLeft = {false, false, false};
    public bool[] shootRight = {false, false, false};
    
    public void WantToMoveFoward(int id)
    {
        moveFoward[id] = true;
    }
    public void WantToStopMoveFoward(int id)
    {
        moveFoward[id] = false;
    }
    public void WantToMoveBack(int id)
    {
        moveBack[id] = true;
    }
    public void WantToStopMoveBack(int id)
    {
        moveBack[id] = false;
    }
    public void WantToMoveLeft(int id)
    {
        moveLeft[id] = true;
    }
    public void WantToStopMoveLeft(int id)
    {
        moveLeft[id] = false;
    }
    public void WantToMoveRight(int id)
    {
        moveRight[id] = true;
    }
    public void WantToStopMoveRight(int id)
    {
        moveRight[id] = false;
    }
        
    public void WantToShootFoward(int id)
    {
        shootFoward[id] = true;
    }
    public void WantToStopShootFoward(int id)
    {
        shootFoward[id] = false;
    }
    public void WantToShootBack(int id)
    {
        shootBack[id] = true;
    }
    public void WantToStopShootBack(int id)
    {
        shootBack[id] = false;
    }
    public void WantToShootLeft(int id)
    {
        shootLeft[id] = true;
    }
    public void WantToStopShootLeft(int id)
    {
        shootLeft[id] = false;
    }
    public void WantToShootRight(int id)
    {
        shootRight[id] = true;
    }
    public void WantToStopShootRight(int id)
    {
        shootRight[id] = false;
    }
}
