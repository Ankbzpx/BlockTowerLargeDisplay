using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

    private void Start()
    {
        Cursor.visible = false;
    }

    #region PLAYER_DICTIONARY
    public static Dictionary<string, Player> PlayerList = new Dictionary<string, Player>();

    public static int playerRegistered = 0;

	//register the player in the dictionary
	public static void RegisterPlayer(string _localID, Player _player)
	{
		PlayerList.Add(_localID, _player);

		playerRegistered++;
	}

    //get the player by the player id
    public static Player GetPlayer(string _netID)
    {
        return PlayerList[_netID];
    }

    //unregister the player with the player id
    public static void UnregisterPlayer(string _netID)
    {
        PlayerList.Remove(_netID);
        playerRegistered -= 1;
	}

	#endregion

	#region CUBE_DICTIONARY
	public static Dictionary<int, Cube> Cubes = new Dictionary<int, Cube>();
	public static int cubeNumber = 54;

    //register the cubes
    public static void RegisterCube(int _cubeID, Cube _cube)
    {
        Cubes.Add(_cubeID, _cube);
    }

    //get a cube by its ID
    public static Cube GetCube(int _localID)
    {
        return Cubes[_localID];
    }

    //unregister all the cubes
    public static void UnregisterCube(int _cubeID)
    {
        Cubes.Remove(_cubeID);
    }

    //check whether cube is moving
    public static bool IsCubeMoving()
    {
        bool isCubeMoving = false;

        foreach (Cube _c in Cubes.Values)
        {
            if (_c.GetComponent<Rigidbody>().velocity.magnitude > 0.1f || _c.GetComponent<Rigidbody>().angularVelocity.magnitude > 0.1f)
            {
                isCubeMoving = true;
                //Debug.Log(_c + " is moving");
                //Debug.Log(_c.GetComponent<Rigidbody>().velocity.magnitude);
            }
        }

        return isCubeMoving;
    }

    //update the cube that is being selected
    //may not needed
    public static void UpdateCube()
    {
        int selectedCubeNum = 0;
        foreach(Cube c in Cubes.Values)
        {
            if (c.GetCubeState())
            {
                selectedCubeNum += 1;

                Cube.currentCubeID = c.GetCubeLocalID();
            }
        }

        if (selectedCubeNum == 0)
        {
            Cube.currentCubeID = 0;
        }
    }

    //get the maximun height of a cube, used for lifting the camera
    public static float MaximunHeight(int excludeID = 0)
    {
        float _maxH = 0f;
        foreach (Cube _c in Cubes.Values)
        {
            if (_c.transform.position.y >= _maxH && _c.localID != excludeID)
            {
                _maxH = _c.transform.position.y;
            }
        }
        return _maxH;
    }

    public static bool IsAtTop(int _cubeId, int excludeId = 0, float threshold = 0.15f)
    {
        bool atTop = false;

        if (Mathf.Abs(GetCube(_cubeId).transform.position.y - MaximunHeight(excludeId)) <= threshold)
            atTop = true;

        return atTop;
    }

    public static void UpdateMovingOutOfTower()
    {

		if (Cube.currentCubeID != 0 && !Cube.isMovingOutOfTower && !CheckContactSpecial(Cube.currentCubeID))
        {
            Cube.isMovingOutOfTower = true;
        }

    }

	//need simplification!
	public static bool CheckContact(int _localID, float threshold = 0.15f)
    {
        for (int i = 0; i < cubeNumber; i++)
        {
            if (_localID != i + 1)
            {
                if (GetCube(_localID).GetComponent<BoxCollider>().bounds.Intersects(GetCube(i + 1).GetComponent<BoxCollider>().bounds))
                {
                    if (!IsAtTop(i + 1, _localID, threshold))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

	//need simplification!
	public static bool CheckContactSpecial(int _localID, float threshold = 0.15f)
	{
		for (int i = 0; i < cubeNumber; i++)
		{
			if (_localID != i + 1)
			{
				if (GetCube(_localID).GetComponent<BoxCollider>().bounds.Intersects(GetCube(i + 1).GetComponent<BoxCollider>().bounds))
				{
						return true;
				}
			}
		}

		return false;
	}

	public static float ObtainStability()
    {
        float _v = 0f;

        Vector3 centerOfGravity = Vector3.zero;

        foreach (Cube _c in Cubes.Values)
        {
            Rigidbody _rb = _c.GetComponent<Rigidbody>();

            _v += _rb.velocity.magnitude;
            //centerOfGravity += _rb.centerOfMass;
        }
        //_v = new Vector3((centerOfGravity/24 - initialCenterOfGravity).x, 0, (centerOfGravity / 24 - initialCenterOfGravity).z).magnitude;

        return _v;
    }

    public static int ObtainTopComplexity()
    {
        int complexity = 0;

        for (int i = 0; i < cubeNumber; i++)
        {
            //if (IsAtTop(i + 1))
            //{

                if (ClampAngle(GetCube(i + 1).transform.rotation.eulerAngles.x,  180) > 10f || ClampAngle(GetCube(i + 1).transform.rotation.eulerAngles.z, 90) > 10f)
                {
                    complexity++;
                }
            //}
        }

        return complexity;
    }

    static float ClampAngle(float _angle, float _positiveRange)
    {
        float ResultAngle = Mathf.Abs(_angle) % _positiveRange;

        if (ResultAngle < _positiveRange/2)
        {
            return ResultAngle;
        }
        else
        {
            return _positiveRange - ResultAngle;
        }
    }

    #endregion

    private void Update()
    {
        if (isPlayer1Restart && isPlayer2Restart && !called)
            ProgramRestart();

        if (isPlayer1Quit && isPlayer2Quit && !called)
            ProgramQuit();

        UpdateMovingOutOfTower();
    }

    #region BUTTON
    public static bool isPlayer1Restart = false;
    public static bool isPlayer2Restart = false;
    public static bool isPlayer1Quit = false;
    public static bool isPlayer2Quit = false;

    bool called = false;


    //restart the program
    public void ProgramRestart()
    {
        called = true;
        isPlayer1Restart = false;
        isPlayer2Restart = false;
        isPlayer1Quit = false;
        isPlayer2Quit = false;

        Cube.highlightedCubeID = 0;
        Cube.globalID = 0;
        Cube.selectNumEachTurn = 0;
        Cube.cubeFallGround = 0;
        Cube.currentCubeID = 0;


        TurnGameManager.turnNum = 1;
        TurnGameManager.isParamatersSetup = false;
        TurnGameManager.maxTurnNum = 16;
        TurnGameManager.timeForEachTurn = 60;
        TurnGameManager.UpdateTime = 0;
        TurnGameManager.checkTime = 3;
        Cubes.Clear();

        FileWriter.WriteData();

        Player.playerGlobalID = 0;
        PlayerList.Clear();
        SceneManager.LoadScene(0);
    }


    //exit the program
    public void ProgramQuit()
    {
        called = true;
        isPlayer1Restart = false;
        isPlayer2Restart = false;
        isPlayer1Quit = false;
        isPlayer2Quit = false;

        FileWriter.WriteData();

        Application.Quit();
    }
    #endregion
}
