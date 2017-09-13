using UnityEngine;
//using UnityEngine.UI;
using System.Collections.Generic;

public class Player2_GamepadControl : MonoBehaviour
{

    enum PlayMode { Mode_1, Mode_2 };
    enum Direction { Left, Right, Up, Down };

    Player player2;

    //the target field of view
    float targetFoV = 10f;
    //How quickly the zoom reaches the target value
    float Dampening = 10.0f;
    // The minimum field of view value we want to zoom to
    public float MinFov = 15.0f;
    // The maximum field of view value we want to zoom to
    public float MaxFov = 80.0f;

    public float wheelSpeed = 50f;
    public float rotateSpeed = 50f;
    public float controllerFactor = 3f;
	public float cubeThreshold = 0.15f;
	public float heightThreshold = 0.4f;
	public float moveFactor = 25f;
    public float rotationFactor = 10f;

    Vector3 tempPosi = Vector3.zero;
    bool isUsing = false;

    AudioManager audioManager;

    //[SerializeField]
    //Text controlModeText;

    //[SerializeField]
    //Text playModeText;

    PlayMode playMode;

    float move2_6;
    float move2_7;

    float H2_input;
    float V2_input;
    float move2_3;
    float move2_4;
    float move2_5;


    //Vector3 cameraPosition;
    //float maxHeightDiff;
    //public float minHeightDiff = 3.5f;

    bool allowSwitch = false;

    Cube hightedCube;

    List<Cube> potentialCubes;


    public bool isSelectCube = false;
    public bool isMoveCube = false;
    public bool isRotateCam = false;
	
		float longPressTime;
	float switchCubeTime;

    // Use this for initialization
    void Start()
    {

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        player2 = GetComponent<Player>();

        targetFoV = Camera.main.fieldOfView;

        //avoid selecting the top blocks
        if (Cube.highlightedCubeID == 0)
            Cube.highlightedCubeID = 4;

		if (!Cube.showHighlight)
			Cube.showHighlight = true;

		//set default playmode
		playMode = PlayMode.Mode_1;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
 
        if (isMoveCube)
            MoveCube();

        if (isRotateCam)
            RotateCamera();



    }

    private void Update()
    {
        if (isSelectCube)
            SelectCube();

        if (!player2.isPlayerReady && TurnGameManager.isParamatersSetup)
            GetReady();

        RestartOrQuit();
    }

    //the function for the player to select the cube
    void SelectCube()
    {


        if (Input.GetButtonDown("A2") && Cube.currentCubeID == 0)
        {

            if (Cube.highlightedCubeID != 0)
            {
                if (GameControl.IsAtTop(Cube.highlightedCubeID))
                {
                    if (audioManager != null)
                        audioManager.Play("Step");

                    Debug.Log("Cube at top");
                }
                else
                {

                    if (audioManager != null)
                        audioManager.Play("Click");

                    GameControl.GetCube(Cube.highlightedCubeID).Select();
                }


            }
        }
        else if (Input.GetButtonDown("A2") && Cube.currentCubeID != 0)
        {
            if (GameControl.CheckContact(Cube.currentCubeID, 0.3f))
            {
                if (audioManager != null)
                    audioManager.Play("Step");

                //Debug.Log("Cube at top");
            }
            else
            {
                if (audioManager != null)
                    audioManager.Play("Click");

                GameControl.GetCube(Cube.currentCubeID).Select();
            }
        }
        else if (Cube.currentCubeID == 0)
        {
            H2_input = Input.GetAxisRaw("H2");
            V2_input = Input.GetAxisRaw("V2");

            int tempHighlightID = Cube.highlightedCubeID;

				if (H2_input != 0f || V2_input != 0f)
				{
					longPressTime += Time.deltaTime;

					if (longPressTime > 0.3f)
						allowSwitch = true;
				}
				else
				{
					longPressTime = 0f;
				}

				if (allowSwitch)
                {
					switchCubeTime += Time.deltaTime;

					if (switchCubeTime < 0.1f)
						return;

					if (H2_input != 0f || V2_input != 0f)
                    {
						if (Mathf.Abs(H2_input) >= Mathf.Abs(V2_input))
                        {
                            //left
                            if (H2_input <= 0)
                            {

                                //function to switch highlighted cube
                                SwitchCubes(Direction.Left);
                            }
                            //right
                            else if (H2_input > 0)
                            {

                                //function to switch highlighted cube
                                SwitchCubes(Direction.Right);
                            }
                        }
                        else if (Mathf.Abs(H2_input) < Mathf.Abs(V2_input))
                        {
                            //up
                            if (V2_input >= 0)
                            {

                                //function to switch highlighted cube
                                SwitchCubes(Direction.Up);
                            }
                            //down
                            else if (V2_input < 0)
                            {

                                //function to switch highlighted cube
                                SwitchCubes(Direction.Down);
                            }
                        }

                    }

                    if (Cube.highlightedCubeID != tempHighlightID)
                    {
						allowSwitch = false;
						switchCubeTime = 0;
					}
            }
            else
            {
                if (H2_input == 0f && V2_input == 0f)
                {
                    allowSwitch = true;
                }
            }
        }
    }

    //the function to let the player to move the selected cube
    void MoveCube()
    {

        if (Cube.currentCubeID == 0)
        {
            return;
        }
        else if (Cube.currentCubeID != 0)
        {
            H2_input = Input.GetAxisRaw("H2");
            V2_input = Input.GetAxisRaw("V2");


            Rigidbody _rb = GameControl.GetCube(Cube.currentCubeID).GetComponent<Rigidbody>();

            if (Input.GetButton("Y2"))
            {
                playMode = PlayMode.Mode_2;
            }
            else
            {
                playMode = PlayMode.Mode_1;
            }

            //calculate the local direction
            Vector3 localForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
            Vector3 localRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);

            if (Cube.currentCubeID != 0)
            {
                if (playMode == PlayMode.Mode_1)
                {
                    //playModeText.text = "Play Mode 1";

                    Vector3 movement = localForward * V2_input / moveFactor + localRight * H2_input / moveFactor;
                    if (movement != Vector3.zero)
                    {
                        _rb.MovePosition(_rb.position + movement);
                    }


                    move2_6 = Input.GetAxis("6th_2");
                    move2_7 = Input.GetAxis("7th_2");

                    Vector3 movement_2 = Vector3.zero;

                    if (Mathf.Abs(move2_7) >= Mathf.Abs(move2_6))
                    {
                        movement_2 = move2_7 / moveFactor * Vector3.up;
                    }
                    else
                    {
                        movement_2 = move2_6 / moveFactor * new Vector3(rotateSpeed, 0, 0);
                    }


                    if (movement_2 != Vector3.zero)
                    {

                        Vector3 newPosition = _rb.position + new Vector3(0, movement_2.y, 0);
                        float rotateRadius = movement_2.x;

                        _rb.MovePosition(newPosition);
                        GameControl.GetCube(Cube.currentCubeID).transform.Rotate(Vector3.up, rotateRadius, Space.World);
                    }

                }
                else if (playMode == PlayMode.Mode_2)
                {
                    //playModeText.text = "Play Mode 2";

                    Vector3 movement = Vector3.zero;


                    if (Mathf.Abs(V2_input) >= Mathf.Abs(H2_input))
                    {
                        movement = V2_input / moveFactor * Vector3.up;
                    }
                    else
                    {
                        movement = H2_input / moveFactor * new Vector3(rotateSpeed, 0, 0);
                    }


                    if (movement != Vector3.zero)
                    {

                        Vector3 newPosition = GameControl.GetCube(Cube.currentCubeID).transform.position + new Vector3(0, movement.y, 0);
                        float rotateRadius = movement.x;

                        _rb.MovePosition(newPosition);
                        GameControl.GetCube(Cube.currentCubeID).transform.Rotate(Vector3.up, rotateRadius, Space.World);

                    }
                }
            }
        }

    }

    void RotateCamera()
    {
        move2_3 = Input.GetAxis("3rd_2");
        move2_4 = Input.GetAxis("4th_2");
        move2_5 = Input.GetAxis("5th_2");
        //cameraPosition = Camera.main.transform.root.position;

        //rotate camera by controller
        if (move2_4 != 0f || move2_5 != 0f && isUsing)
        {
            Vector2 rotateVector = new Vector2(move2_4, move2_5);

            Vector3 newDir = Input.mousePosition;

            Vector3 deltaRotation = rotateSpeed * rotateVector;

            if ((-deltaRotation.y < 0 && Camera.main.transform.localRotation.x >= 0f) || (-deltaRotation.y > 0 && Camera.main.transform.localRotation.x <= 0.45f))
            {
                Camera.main.transform.Rotate(Vector3.right, -deltaRotation.y / rotationFactor /2f, Space.Self);

            }

            Camera.main.transform.parent.Rotate(Vector3.up, deltaRotation.x / rotationFactor);

            tempPosi = newDir;

            isUsing = false;
        }

        if (move2_4 == 0f || move2_5 == 0f)
        {
            tempPosi = new Vector2(move2_4, move2_5);
            isUsing = true;
        }

        //zoom by scoller
        if (move2_3 != 0f)
        {
            targetFoV = Mathf.Clamp(Camera.main.fieldOfView - wheelSpeed * move2_3, MinFov, MaxFov);

            // The framerate independent damping factor
            var factor = 1.0f - Mathf.Exp(-Dampening * Time.fixedDeltaTime);

            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFoV, factor);
        }

        //if (Input.GetButton("L2"))
        //{
        //    maxHeightDiff = GameControl.MaximunHeight() + minHeightDiff;

        //    Vector3 targetPosition = new Vector3(cameraPosition.x, Mathf.Clamp(cameraPosition.y - wheelSpeed*Time.fixedDeltaTime ,minHeightDiff,maxHeightDiff)  ,cameraPosition.z);


        //    Camera.main.transform.root.position = Vector3.Lerp(cameraPosition, targetPosition, 1.0f - Mathf.Exp(-Dampening * Time.fixedDeltaTime));
        //}

        //if (Input.GetButton("R2"))
        //{
        //    maxHeightDiff = GameControl.MaximunHeight() + minHeightDiff;

        //    Vector3 targetPosition = new Vector3(cameraPosition.x, Mathf.Clamp(cameraPosition.y + wheelSpeed * Time.fixedDeltaTime, minHeightDiff, maxHeightDiff), cameraPosition.z);

        //    Camera.main.transform.root.position = Vector3.Lerp(cameraPosition, targetPosition, 1.0f - Mathf.Exp(-Dampening * Time.fixedDeltaTime));

        //}


    }

	void SwitchCubes(Direction _dir)
	{
		if (Cube.highlightedCubeID == 0)
			return;

		hightedCube = GameControl.GetCube(Cube.highlightedCubeID);

		potentialCubes = new List<Cube>();

		if (GameControl.Cubes.Values.Count != 0)
		{
			switch (_dir)
			{
				case Direction.Left:

					foreach (Cube _cube in GameControl.Cubes.Values)
					{
						Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

						if (Vector3.Dot(_cDir, new Vector3(-Camera.main.transform.right.x, 0, -Camera.main.transform.right.z).normalized) > 0f)
						{
							if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) <= cubeThreshold)
							{
								potentialCubes.Add(_cube);
							}
						}
					}

					Cube.highlightedCubeID = GetClosestCubeID(potentialCubes);

					if (Cube.highlightedCubeID != hightedCube.GetCubeLocalID())
						if (audioManager != null)
							audioManager.Play("Ding");

					break;
				case Direction.Right:

					foreach (Cube _cube in GameControl.Cubes.Values)
					{
						Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

						if (Vector3.Dot(_cDir, new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized) > 0f)
						{
							if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) <= cubeThreshold)
							{
								potentialCubes.Add(_cube);
							}
						}
					}

					Cube.highlightedCubeID = GetClosestCubeID(potentialCubes);

					if (Cube.highlightedCubeID != hightedCube.GetCubeLocalID())
						if (audioManager != null)
							audioManager.Play("Ding");

					break;
				case Direction.Up:

					foreach (Cube _cube in GameControl.Cubes.Values)
					{
						Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

						if (Vector3.Dot(_cDir, Vector3.up) > 0f)
						{
							if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) > cubeThreshold && Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) < heightThreshold)
							{
								potentialCubes.Add(_cube);
							}
						}
					}

					if (potentialCubes.Count == 0)
					{
						foreach (Cube _cube in GameControl.Cubes.Values)
						{
							Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

							if (Vector3.Dot(_cDir, Vector3.up) > 0f)
							{
								if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) > cubeThreshold)
								{
									potentialCubes.Add(_cube);
								}
							}
						}
					}

					Cube.highlightedCubeID = GetClosestCubeID(potentialCubes, true);

					if (Cube.highlightedCubeID != hightedCube.GetCubeLocalID())
						if (audioManager != null)
							audioManager.Play("Ding");

					break;
				case Direction.Down:

					foreach (Cube _cube in GameControl.Cubes.Values)
					{
						Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

						if (Vector3.Dot(_cDir, Vector3.down) > 0f)
						{
							if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) > cubeThreshold && Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) < heightThreshold)
							{
								potentialCubes.Add(_cube);
							}
						}
					}

					if (potentialCubes.Count == 0)
					{
						foreach (Cube _cube in GameControl.Cubes.Values)
						{
							Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

							if (Vector3.Dot(_cDir, Vector3.down) > 0f)
							{
								if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) > cubeThreshold)
								{
									potentialCubes.Add(_cube);
								}
							}
						}
					}

					Cube.highlightedCubeID = GetClosestCubeID(potentialCubes, true);

					if (Cube.highlightedCubeID != hightedCube.GetCubeLocalID())
						if (audioManager != null)
							audioManager.Play("Ding");

					break;
				default:
					break;
			}
		}

		potentialCubes.Clear();
	}

	int GetClosestCubeID(List<Cube> _potentialCubes, bool isNearCamera = false)
	{
		if (_potentialCubes.Count == 0)
			return Cube.highlightedCubeID;

		int id = Cube.highlightedCubeID;

		float minDistance = 50f;

		if (isNearCamera)
		{
			for (int i = 0; i < _potentialCubes.Count; i++)
			{
				Vector3 refPoint = new Vector3(Camera.main.transform.position.x, _potentialCubes[i].transform.position.y, Camera.main.transform.position.z);

				float _tempDis = (_potentialCubes[i].transform.position - hightedCube.transform.position).magnitude + 2 * (refPoint - _potentialCubes[i].GetComponent<Collider>().ClosestPoint(refPoint)).magnitude;
				if (_tempDis < minDistance && Cube.highlightedCubeID != _potentialCubes[i].transform.GetComponent<Cube>().localID)
				{
					id = _potentialCubes[i].transform.GetComponent<Cube>().localID;
					minDistance = _tempDis;
				}
			}
		}
		else
		{
			for (int i = 0; i < _potentialCubes.Count; i++)
			{
				float _tempDis = (_potentialCubes[i].transform.position - hightedCube.transform.position).magnitude;
				if (_tempDis < minDistance && Cube.highlightedCubeID != _potentialCubes[i].transform.GetComponent<Cube>().localID)
				{
					id = _potentialCubes[i].transform.GetComponent<Cube>().localID;
					minDistance = _tempDis;
				}
			}
		}


		return id;
	}


	public void ShowClickSound()
    {
        if (audioManager != null)
            audioManager.Play("Click");
    }

    void GetReady()
    {
        if (Input.GetButtonDown("A2"))
        {
            player2.isPlayerReady = true;
        }
    }


    void RestartOrQuit()
    {
        if (Input.GetButton("Back2"))
            GameControl.isPlayer2Restart = true;
        else
            GameControl.isPlayer2Restart = false;

        if (Input.GetButton("Home2"))
            GameControl.isPlayer2Quit = true;
        else
            GameControl.isPlayer2Quit = false;
    }
}

