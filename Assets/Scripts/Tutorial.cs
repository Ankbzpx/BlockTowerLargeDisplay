using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour
{
    //definition of enums
    enum TutorialStage { Beginning, Select, Deselect, Move1, Move2, RotateCamera, SwitchCube, End, None };
    enum PlayMode { Mode_1, Mode_2 };
    enum Direction { Left, Right, Up, Down };
    enum TutorialPlayer {Player1, Player2};

    PlayMode playMode;
    TutorialStage tutorialStage;
    TutorialPlayer tutorialPlayer;

    //the reference of audiomanager
    AudioManager audioManager;


    bool player1Finished = false;
    bool player2Finished = false;


    //the target field of view
    float targetFoV = 10f;
    //How quickly the zoom reaches the target value
    float Dampening = 10.0f;
    // The minimum field of view value we want to zoom to
    public float MinFov = 15.0f;
    // The maximum field of view value we want to zoom to
    public float MaxFov = 80.0f;

    //the factors that can be changed through inspector
    public float wheelSpeed = 50f;
    public float rotateSpeed = 50f;
    public float controllerFactor = 3f;
	public float cubeThreshold = 0.15f;
	public float heightThreshold = 0.4f;
	public float moveFactor = 25f;
    public float rotationFactor = 10f;

    //temporary variale used for rotation (convert axis to button)
    Vector3 tempPosi = Vector3.zero;
    bool isUsing = false;

    float H1_input;
    float V1_input;
    float move1_3;
    float move1_4;
    float move1_5;
    float move1_6;
    float move1_7;

    float H2_input;
    float V2_input;
    float move2_3;
    float move2_4;
    float move2_5;
    float move2_6;
    float move2_7;


    //whether is is allow for switch the cubes to realize discrete cube switch
    bool allowSwitch = false;

    //temp cube reference for the hightlighted cube
    Cube hightedCube;

    //the list of potential cubes in certain direction
    List<Cube> potentialCubes;


    ////the UI reference
    //[SerializeField]
    //Canvas _playerCanvas;
    [SerializeField]
    Canvas tutorialCanvas_1;

    [SerializeField]
    GameObject BeginningPanel_1;
    [SerializeField]
    GameObject SelectPanel_1;
    [SerializeField]
    GameObject DeselectPanel_1;
    [SerializeField]
    GameObject Move1Panel_1;
    [SerializeField]
    GameObject Move2Panel_1;
    [SerializeField]
    GameObject RotateCameraPanel_1;
    [SerializeField]
    GameObject SwitchCubePanel_1;
    [SerializeField]
    GameObject EndPanel_1;
    [SerializeField]
    GameObject WaitPanel_1;

    //the UI reference
    [SerializeField]
    Canvas tutorialCanvas_2;

    [SerializeField]
    GameObject BeginningPanel_2;
    [SerializeField]
    GameObject SelectPanel_2;
    [SerializeField]
    GameObject DeselectPanel_2;
    [SerializeField]
    GameObject Move1Panel_2;
    [SerializeField]
    GameObject Move2Panel_2;
    [SerializeField]
    GameObject RotateCameraPanel_2;
    [SerializeField]
    GameObject SwitchCubePanel_2;
    [SerializeField]
    GameObject EndPanel_2;
    [SerializeField]
    GameObject WaitPanel_2;

	float longPressTime;
	float switchCubeTime;

	void Start()
    {
        //_playerControl = GetComponent<PlayerControl>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        //_player = GetComponent<Player>();


            tutorialStage = TutorialStage.Beginning;
            tutorialPlayer = TutorialPlayer.Player1;


        //_playerControl.enabled = false;
        //_playerCanvas.gameObject.SetActive(false);
        tutorialCanvas_1.gameObject.SetActive(true);

        CloseAllPanel();

        Debug.Log(tutorialStage);

        Cube.showHighlight = true;

        Cube.highlightedCubeID = 4;
    }

    void Update()
    {
        if (tutorialStage == TutorialStage.Beginning)
        {
            if (tutorialPlayer == TutorialPlayer.Player1)
            {
                if (!BeginningPanel_1.activeSelf)
                    BeginningPanel_1.SetActive(true);

                if (WaitPanel_1.activeSelf)
                    WaitPanel_1.SetActive(false);
                if (!WaitPanel_2.activeSelf)
                    WaitPanel_2.SetActive(true);

                if (Input.GetButtonDown("A1"))
                    tutorialStage = TutorialStage.SwitchCube;
                else if (Input.GetButtonDown("B1"))
                    tutorialStage = TutorialStage.None;
            }
            else if (tutorialPlayer == TutorialPlayer.Player2)
            {
                if (!BeginningPanel_2.activeSelf)
                    BeginningPanel_2.SetActive(true);

                if (!WaitPanel_1.activeSelf)
                    WaitPanel_1.SetActive(true);
                if (WaitPanel_2.activeSelf)
                    WaitPanel_2.SetActive(false);

                if (Input.GetButtonDown("A2"))
                    tutorialStage = TutorialStage.SwitchCube;
                else if (Input.GetButtonDown("B2"))
                    tutorialStage = TutorialStage.None;
            }


        }
        else if (tutorialStage == TutorialStage.SwitchCube)
        {
            if (tutorialPlayer == TutorialPlayer.Player1)
            {
                if (BeginningPanel_1.activeSelf)
                    BeginningPanel_1.SetActive(false);
                if (!SwitchCubePanel_1.activeSelf)
                    SwitchCubePanel_1.SetActive(true);

				if (Cube.currentCubeID == 0)
				{
					H1_input = Input.GetAxisRaw("H1");
					V1_input = Input.GetAxisRaw("V1");

					int tempHighlightID = Cube.highlightedCubeID;

					if (H1_input != 0f || V1_input != 0f)
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

						if (H1_input != 0f || V1_input != 0f)
						{
							if (Mathf.Abs(H1_input) >= Mathf.Abs(V1_input))
							{
								//left
								if (H1_input <= 0)
								{

									//function to switch highlighted cube
									SwitchCubes(Direction.Left);
								}
								//right
								else if (H1_input > 0)
								{

									//function to switch highlighted cube
									SwitchCubes(Direction.Right);
								}
							}
							else if (Mathf.Abs(H1_input) < Mathf.Abs(V1_input))
							{
								//up
								if (V1_input >= 0)
								{

									//function to switch highlighted cube
									SwitchCubes(Direction.Up);
								}
								//down
								else if (V1_input < 0)
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
						if (H1_input == 0f && V1_input == 0f)
						{
							allowSwitch = true;
						}
					}
				}
                if (Input.GetButtonDown("A1"))
                    tutorialStage = TutorialStage.Select;
            }
            else if (tutorialPlayer == TutorialPlayer.Player2)
            {
                if (BeginningPanel_2.activeSelf)
                    BeginningPanel_2.SetActive(false);
                if (!SwitchCubePanel_2.activeSelf)
                    SwitchCubePanel_2.SetActive(true);

				if (Cube.currentCubeID == 0)
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
                if (Input.GetButtonDown("A2"))
                    tutorialStage = TutorialStage.Select;
            }
        }
        else if (tutorialStage == TutorialStage.Select)
        {
            if (tutorialPlayer == TutorialPlayer.Player1)
            {
                if (SwitchCubePanel_1.activeSelf)
                    SwitchCubePanel_1.SetActive(false);
                if (!SelectPanel_1.activeSelf)
                    SelectPanel_1.SetActive(true);

                if (Input.GetButtonDown("A1") && Cube.currentCubeID == 0)
                {

                    if (Cube.highlightedCubeID != 0)
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

                                tutorialStage = TutorialStage.Move1;
                            }

                        }
                    }
                }
                else if (Cube.currentCubeID == 0)
                {
                    H1_input = Input.GetAxisRaw("H1");
                    V1_input = Input.GetAxisRaw("V1");

                    int tempHighlightID = Cube.highlightedCubeID;

                    if (H1_input != 0f || V1_input != 0f)
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

					if (H1_input != 0f || V1_input != 0f)
                    {
						if (Mathf.Abs(H1_input) >= Mathf.Abs(V1_input))
                        {
                            //left
                            if (H1_input <= 0)
                            {

                                //function to switch highlighted cube
                                SwitchCubes(Direction.Left);
                            }
                            //right
                            else if (H1_input > 0)
                            {

                                //function to switch highlighted cube
                                SwitchCubes(Direction.Right);
                            }
                        }
                        else if (Mathf.Abs(H1_input) < Mathf.Abs(V1_input))
                        {
                            //up
                            if (V1_input >= 0)
                            {

                                //function to switch highlighted cube
                                SwitchCubes(Direction.Up);
                            }
                            //down
                            else if (V1_input < 0)
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
                        if (H1_input == 0f && V1_input == 0f)
                        {
                            allowSwitch = true;
                        }
                    }
                }
            }
            else if (tutorialPlayer == TutorialPlayer.Player2)
            {
                if (SwitchCubePanel_2.activeSelf)
                    SwitchCubePanel_2.SetActive(false);
                if (!SelectPanel_2.activeSelf)
                    SelectPanel_2.SetActive(true);

                if (Input.GetButtonDown("A2") && Cube.currentCubeID == 0)
                {

                    if (Cube.highlightedCubeID != 0)
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

                                tutorialStage = TutorialStage.Move1;
                            }

                        }
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



        }

        else if (tutorialStage == TutorialStage.Move1)
        {
            if (tutorialPlayer == TutorialPlayer.Player1)
            {
                if (SelectPanel_1.activeSelf)
                    SelectPanel_1.SetActive(false);
                if (!Move1Panel_1.activeSelf)
                    Move1Panel_1.SetActive(true);

                if (Cube.currentCubeID != 0)
                {
                    H1_input = Input.GetAxisRaw("H1");
                    V1_input = Input.GetAxisRaw("V1");

                    //calculate the local direction
                    Vector3 localForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                    Vector3 localRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);


                    Vector3 movement = localForward * V1_input / moveFactor + localRight * H1_input / moveFactor;
                    if (movement != Vector3.zero)
                    {

                        Vector3 newPosition = GameControl.GetCube(Cube.currentCubeID).transform.position + movement;
                        GameControl.GetCube(Cube.currentCubeID).GetComponent<Rigidbody>().MovePosition(newPosition);


                    }

                }

                if (Input.GetButtonDown("A1"))
                    tutorialStage = TutorialStage.Move2;
            }
            else if (tutorialPlayer == TutorialPlayer.Player2)
            {
                if (SelectPanel_2.activeSelf)
                    SelectPanel_2.SetActive(false);
                if (!Move1Panel_2.activeSelf)
                    Move1Panel_2.SetActive(true);

                if (Cube.currentCubeID != 0)
                {
                    H2_input = Input.GetAxisRaw("H2");
                    V2_input = Input.GetAxisRaw("V2");

                    //calculate the local direction
                    Vector3 localForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                    Vector3 localRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);


                    Vector3 movement = localForward * V2_input / moveFactor + localRight * H2_input / moveFactor;
                    if (movement != Vector3.zero)
                    {

                        Vector3 newPosition = GameControl.GetCube(Cube.currentCubeID).transform.position + movement;
                        GameControl.GetCube(Cube.currentCubeID).GetComponent<Rigidbody>().MovePosition(newPosition);


                    }

                }

                if (Input.GetButtonDown("A2"))
                    tutorialStage = TutorialStage.Move2;
            }


        }
        else if (tutorialStage == TutorialStage.Move2)
        {
            if (tutorialPlayer == TutorialPlayer.Player1)
            {
                if (Move1Panel_1.activeSelf)
                    Move1Panel_1.SetActive(false);
                if (!Move2Panel_1.activeSelf)
                    Move2Panel_1.SetActive(true);

                Vector3 movement = Vector3.zero;


                if (Input.GetButton("Y1"))
                {
                    playMode = PlayMode.Mode_2;
                }
                else
                {
                    playMode = PlayMode.Mode_1;
                }

                H1_input = Input.GetAxisRaw("H1");
                V1_input = Input.GetAxisRaw("V1");

                if (Cube.currentCubeID != 0)
                {

                    Rigidbody _rb = GameControl.GetCube(Cube.currentCubeID).GetComponent<Rigidbody>();

                    //calculate the local direction
                    Vector3 localForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                    Vector3 localRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);

                    if (playMode == PlayMode.Mode_1)
                    {
                        //move XZ
                        movement = localForward * V1_input / moveFactor + localRight * H1_input / moveFactor;
                        if (movement != Vector3.zero)
                        {
                            _rb.MovePosition(_rb.position + movement);
                        }

                        //move Z
                        move1_6 = Input.GetAxis("6th_1");
                        move1_7 = Input.GetAxis("7th_1");

                        Vector3 movement_2 = Vector3.zero;

                        if (Mathf.Abs(move1_7) >= Mathf.Abs(move1_6))
                        {
                            movement_2 = move1_7 / moveFactor * Vector3.up;
                        }
                        else
                        {
                            movement_2 = move1_6 / moveFactor * new Vector3(rotateSpeed, 0, 0);
                        }


                        if (movement_2 != Vector3.zero)
                        {

                            Vector3 newPosition = _rb.position + new Vector3(0, movement_2.y, 0);
                            float rotateRadius = movement_2.x;

                            _rb.MovePosition(newPosition);
                            GameControl.GetCube(Cube.currentCubeID).transform.Rotate(Vector3.up, rotateRadius, Space.World);
                        }

                    }
                    //move Y
                    else if (playMode == PlayMode.Mode_2)
                    {
                        movement = Vector3.zero;

                        if (Mathf.Abs(V1_input) >= Mathf.Abs(H1_input))
                        {
                            movement = V1_input / moveFactor * Vector3.up;
                        }
                        else
                        {
                            movement = H1_input / moveFactor * new Vector3(rotateSpeed, 0, 0);
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

                if (Input.GetButtonDown("A1"))
                    tutorialStage = TutorialStage.RotateCamera;
            }
            else if (tutorialPlayer == TutorialPlayer.Player2)
            {
                if (Move1Panel_2.activeSelf)
                    Move1Panel_2.SetActive(false);
                if (!Move2Panel_2.activeSelf)
                    Move2Panel_2.SetActive(true);

                Vector3 movement = Vector3.zero;


                if (Input.GetButton("Y2"))
                {
                    playMode = PlayMode.Mode_2;
                }
                else
                {
                    playMode = PlayMode.Mode_1;
                }

                H2_input = Input.GetAxisRaw("H2");
                V2_input = Input.GetAxisRaw("V2");

                if (Cube.currentCubeID != 0)
                {

                    Rigidbody _rb = GameControl.GetCube(Cube.currentCubeID).GetComponent<Rigidbody>();

                    //calculate the local direction
                    Vector3 localForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                    Vector3 localRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);

                    if (playMode == PlayMode.Mode_1)
                    {

                        movement = localForward * V2_input / moveFactor + localRight * H2_input / moveFactor;
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

                        movement = Vector3.zero;


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

                if (Input.GetButtonDown("A2"))
                    tutorialStage = TutorialStage.RotateCamera;
            }

        }
        else if (tutorialStage == TutorialStage.RotateCamera)
        {
            if (tutorialPlayer == TutorialPlayer.Player1)
            {
                if (Move2Panel_1.activeSelf)
                    Move2Panel_1.SetActive(false);
                if (!RotateCameraPanel_1.activeSelf)
                    RotateCameraPanel_1.SetActive(true);

                move1_3 = Input.GetAxis("3rd_1");
                move1_4 = Input.GetAxis("4th_1");
                move1_5 = Input.GetAxis("5th_1");
                //cameraPosition = Camera.main.transform.root.position;

                //rotate camera by controller
                if (move1_4 != 0f || move1_5 != 0f && isUsing)
                {
                    Vector2 rotateVector = new Vector2(move1_4, move1_5);

                    Vector3 newDir = Input.mousePosition;

                    Vector3 deltaRotation = rotateSpeed * rotateVector;

                    if ((-deltaRotation.y < 0 && Camera.main.transform.localRotation.x >= 0f) || (-deltaRotation.y > 0 && Camera.main.transform.localRotation.x <= 0.45f))
                    {
                        Camera.main.transform.Rotate(Vector3.right, -deltaRotation.y / rotationFactor / 2f, Space.Self);

                    }

                    Camera.main.transform.parent.Rotate(Vector3.up, deltaRotation.x / rotationFactor);

                    tempPosi = newDir;

                    isUsing = false;
                }

                if (move1_4 == 0f || move1_5 == 0f)
                {
                    tempPosi = new Vector2(move1_4, move1_5);
                    isUsing = true;
                }

                //zoom by scoller
                if (move1_3 != 0f)
                {
                    targetFoV = Mathf.Clamp(Camera.main.fieldOfView - wheelSpeed * move1_3, MinFov, MaxFov);

                    // The framerate independent damping factor
                    var factor = 1.0f - Mathf.Exp(-Dampening * Time.fixedDeltaTime);

                    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFoV, factor);
                }

                if (Input.GetButtonDown("A1"))
                    tutorialStage = TutorialStage.Deselect;
            }
            else if (tutorialPlayer == TutorialPlayer.Player2)
            {
                if (Move2Panel_2.activeSelf)
                    Move2Panel_2.SetActive(false);
                if (!RotateCameraPanel_2.activeSelf)
                    RotateCameraPanel_2.SetActive(true);

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
                        Camera.main.transform.Rotate(Vector3.right, -deltaRotation.y / rotationFactor / 2f, Space.Self);

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

                if (Input.GetButtonDown("A2"))
                    tutorialStage = TutorialStage.Deselect;
            }
        }

        else if (tutorialStage == TutorialStage.Deselect)
        {
            if (tutorialPlayer == TutorialPlayer.Player1)
            {
                if (RotateCameraPanel_1.activeSelf)
                    RotateCameraPanel_1.SetActive(false);
                if (!DeselectPanel_1.activeSelf)
                    DeselectPanel_1.SetActive(true);

                if (Input.GetButtonDown("A1") && Cube.currentCubeID != 0)
                {
                    if (audioManager != null)
                        audioManager.Play("Click");


                    GameControl.GetCube(Cube.currentCubeID).Select();

                    tutorialStage = TutorialStage.End;
                }
            }
            else if (tutorialPlayer == TutorialPlayer.Player2)
            {
                if (RotateCameraPanel_2.activeSelf)
                    RotateCameraPanel_2.SetActive(false);
                if (!DeselectPanel_2.activeSelf)
                    DeselectPanel_2.SetActive(true);

                if (Input.GetButtonDown("A2") && Cube.currentCubeID != 0)
                {
                    if (audioManager != null)
                        audioManager.Play("Click");


                    GameControl.GetCube(Cube.currentCubeID).Select();

                    tutorialStage = TutorialStage.End;
                }
            }
        }
        else if (tutorialStage == TutorialStage.End)
        {
            if (tutorialPlayer == TutorialPlayer.Player1)
            {
                if (DeselectPanel_1.activeSelf)
                    DeselectPanel_1.SetActive(false);
                if (BeginningPanel_1.activeSelf)
                    BeginningPanel_1.SetActive(false);
                if (!EndPanel_1.activeSelf)
                    EndPanel_1.SetActive(true);

                if (Input.GetButtonDown("B1") || Input.GetButtonDown("A1"))
                {
                    //_playerControl.enabled = true;
                    //_playerCanvas.gameObject.SetActive(true);
                    //tutorialCanvas_1.gameObject.SetActive(false);
                    //hasShownTutorial = true;
                    //CloseAllPanel();
                    tutorialStage = TutorialStage.None;
                }
            }
            else if (tutorialPlayer == TutorialPlayer.Player2)
            {

                if (DeselectPanel_2.activeSelf)
                    DeselectPanel_2.SetActive(false);
                if (BeginningPanel_2.activeSelf)
                    BeginningPanel_2.SetActive(false);
                if (!EndPanel_2.activeSelf)
                    EndPanel_2.SetActive(true);

                if (Input.GetButtonDown("B2") || Input.GetButtonDown("A2"))
                {
                    //_playerControl.enabled = true;
                    //_playerCanvas.gameObject.SetActive(true);
                    //tutorialCanvas_1.gameObject.SetActive(false);
                    //hasShownTutorial = true;
                    //CloseAllPanel();
                    tutorialStage = TutorialStage.None;
                }
            }
        }
        else if (tutorialStage == TutorialStage.None)
        {

            if (tutorialPlayer == TutorialPlayer.Player1)
            {
                CloseAllPanel();
                player1Finished = true;
                tutorialPlayer = TutorialPlayer.Player2;
            }
            else if (tutorialPlayer == TutorialPlayer.Player2)
            {
                CloseAllPanel();
                player2Finished = true;
            }

            if (player1Finished && !player2Finished)
            {
                tutorialStage = TutorialStage.Beginning;
            }
            else if (player1Finished && player2Finished)
            {
                CloseAllPanel();
                //_playerControl.enabled = true;
                //_playerCanvas.gameObject.SetActive(true);
                //_player.isPlayerReady = true;

                tutorialStage = TutorialStage.None;

                Cube.highlightedCubeID = 4;
                Cube.globalID = 0;
                Cube.selectNumEachTurn = 0;
                Cube.cubeFallGround = 0;
                Cube.currentCubeID = 0;
                Cube.selectDelaying = false;


                TurnGameManager.turnNum = 1;
                TurnGameManager.isParamatersSetup = false;
                TurnGameManager.maxTurnNum = 16;
                TurnGameManager.timeForEachTurn = 60;
                TurnGameManager.UpdateTime = 0;
                TurnGameManager.checkTime = 3;
                GameControl.Cubes.Clear();

                FileWriter.WriteData();

                Player.playerGlobalID = 0;
                GameControl.PlayerList.Clear();

                SceneManager.LoadScene(0);
            }
        }
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

	void CloseAllPanel()
    {

        BeginningPanel_1.SetActive(false);
        SelectPanel_1.SetActive(false);
        DeselectPanel_1.SetActive(false);
        Move1Panel_1.SetActive(false);
        Move2Panel_1.SetActive(false);
        RotateCameraPanel_1.SetActive(false);
        SwitchCubePanel_1.SetActive(false);
        EndPanel_1.SetActive(false);
        WaitPanel_1.SetActive(false);

        BeginningPanel_2.SetActive(false);
        SelectPanel_2.SetActive(false);
        DeselectPanel_2.SetActive(false);
        Move1Panel_2.SetActive(false);
        Move2Panel_2.SetActive(false);
        RotateCameraPanel_2.SetActive(false);
        SwitchCubePanel_2.SetActive(false);
        EndPanel_2.SetActive(false);
        WaitPanel_2.SetActive(false);
    }
}

