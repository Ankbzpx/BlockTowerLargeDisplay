using UnityEngine;
using UnityEngine.UI;

//this script requires the PlayerControl script
[RequireComponent(typeof(PlayerControl))]
public class Player : MonoBehaviour {
    //used for player registeration
    public static int playerGlobalID = 0;
    int localID;

    PlayerControl playerControl;

    int _updateTime;

    //accessed by the TurnGameManager script
    public bool isPlayerReady = false;
    public bool isPlaying = false;
    public bool isWaiting = false;
    public bool isChecking = false;
    public bool isWin = false;
    public bool isLose = false;
    public bool isTie = false;

    //UI element reference
    [SerializeField]
    GameObject _playingTurnPanel;

    [SerializeField]
    GameObject _waitingTurnPanel;

    [SerializeField]
    GameObject _checkCubeFallPanel;

    [SerializeField]
    GameObject _winPanel;

    [SerializeField]
    GameObject _losePanel;

    [SerializeField]
    GameObject _tiePanel;

    [SerializeField]
    GameObject _playerReadyPanel;


    [SerializeField]
    Text _playerName;

    [SerializeField]
    Text _turnNum;

    [SerializeField]
    Text _playingTimeText;

    [SerializeField]
    Text _WaitingTimeText;

	// Use this for initialization
	void Start () {

        //give the player ID
        playerGlobalID++;
        localID = playerGlobalID;


        GameControl.RegisterPlayer(localID.ToString(), this);
        //the name may be editable by the player
        name = "Player " + localID;
        _playerName.text = name;

        playerControl = GetComponent<PlayerControl>();

        CloseAllPanel();

            _playerReadyPanel.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {

        if (Cube.cubeFallGround > 3)
        {
            isLose = true;
        }

        _turnNum.text = TurnGameManager.turnNum.ToString();
        _updateTime = (int)TurnGameManager.UpdateTime;

        UpdatePlayerState();
    }


    void UpdatePlayerState()
    {
        if (isPlayerReady)
        {
                if (_playerReadyPanel.activeSelf)
                _playerReadyPanel.SetActive(false);

            if (isWin)
            {
                CloseAllPanel();
                playerControl.isTurn = false;
                _winPanel.SetActive(true);
                //Debug.Log("isWin");
            }
            else if (isTie)
            {
                CloseAllPanel();
                playerControl.isTurn = false;
                _tiePanel.SetActive(true);
                //Debug.Log("isTie");
            }
            else if (isLose)
            {
                CloseAllPanel();
                playerControl.isTurn = false;
                _losePanel.SetActive(true);
                //Debug.Log("isLose");
            }
            else
            {
                if (isPlaying)
                {

                    _playingTimeText.text = _updateTime.ToString();
                    CloseAllPanel();

                    playerControl.isTurn = true;

                    _playingTurnPanel.SetActive(true);
                    //Debug.Log("isPlaying");
                }
                else if (isWaiting)
                {
					_WaitingTimeText.text = _updateTime.ToString();
                    CloseAllPanel();

                    playerControl.isTurn = false;

                    _waitingTurnPanel.SetActive(true);
                    //Debug.Log("isWaiting");
                }
                else if (isChecking)
                {
                    CloseAllPanel();

                    playerControl.isTurn = false;

                    _checkCubeFallPanel.SetActive(true);
                    //Debug.Log("isChecking");
                }
            }
        }
        else if (!isPlayerReady)
        {
            CloseAllPanel();
            playerControl.isTurn = false;

                _playerReadyPanel.SetActive(true);
        }
    }

    void CloseAllPanel()
    {
        _playingTurnPanel.SetActive(false);
        _waitingTurnPanel.SetActive(false);
        _checkCubeFallPanel.SetActive(false);
        _winPanel.SetActive(false);
        _losePanel.SetActive(false);
        _tiePanel.SetActive(false);

            _playerReadyPanel.SetActive(false);
    }

    //return the player local ID
    public int GetPlayerLocalID()
    {
        return localID;
    }


    void OnDestroy()
    {
        //unregister the player when the player is destroyed
        isPlaying = false;
        isWaiting = false;
        isChecking = false;
        isWin = false;
        isLose = false;
        isTie = false;
        isPlayerReady = false;


        GameControl.UnregisterPlayer(localID.ToString());
		Cube.cubeFallGround = 0;

    }
}
