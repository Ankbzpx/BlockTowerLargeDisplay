using UnityEngine;
using System.Collections;


public class PlayerControl : MonoBehaviour {

    public bool isTurn = true;

    public static int selectTimes = 0;

    Player1_GamepadControl _player1_GamepadControl;
    Player2_GamepadControl _player2_GamepadControl;

    // Use this for initialization
    void Start()
    {
        if (GetComponent<Player1_GamepadControl>() != null)
        {
            _player1_GamepadControl = GetComponent<Player1_GamepadControl>();
        }
        else if(GetComponent<Player2_GamepadControl>() != null)
        {
            _player2_GamepadControl = GetComponent<Player2_GamepadControl>();
        }

    }

    void FixedUpdate()
    {
        if (isTurn)
        {
            if (_player1_GamepadControl != null)
            {

                _player1_GamepadControl.isMoveCube = true;
                _player1_GamepadControl.isRotateCam = true;
            }
            else if (_player2_GamepadControl != null)
            {
                _player2_GamepadControl.isMoveCube = true;
                _player2_GamepadControl.isRotateCam = true;
            }
        }
        else
        {
            if (_player1_GamepadControl != null)
            {
                _player1_GamepadControl.isMoveCube = false;
                _player1_GamepadControl.isRotateCam = false;
            }
            else if (_player2_GamepadControl != null)
            {
                _player2_GamepadControl.isMoveCube = false;
                _player2_GamepadControl.isRotateCam = false;
            }
        }
    }

    private void Update()
    {
        if (isTurn)
        {
            if (_player1_GamepadControl != null)
            {
                _player1_GamepadControl.isSelectCube = true;

            }
            else if (_player2_GamepadControl != null)
            {
                _player2_GamepadControl.isSelectCube = true;

            }
        }
        else
        {
            if (_player1_GamepadControl != null)
            {
                _player1_GamepadControl.isSelectCube = false;

            }
            else if (_player2_GamepadControl != null)
            {
                _player2_GamepadControl.isSelectCube = false;

            }
        }
    }

}
