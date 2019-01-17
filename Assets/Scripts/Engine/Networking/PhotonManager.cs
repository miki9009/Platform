using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.MonoBehaviour
{
    public static bool IsMultiplayer { get; private set; }
    public static bool IsMaster
    {
        get
        {
            return PhotonNetwork.isMasterClient;
        }
    }

    private static bool notStarted;

    static List<Character> _players;
    public static List<Character> Players
    {
        get
        {
            if (_players == null)
                return new List<Character>();
            else
                return _players;
        }
    }
    public int minPlayers = 2;
    public int spawnPointIndex;
    public string version = "1.0.0";
    public string roomName = "Room";
    public string playerPrefabName = "Cube";

    public static event System.Action<byte, int, object> MessageReceived;
    public static event System.Action<byte, object> GlobalMessageReceived;
    public static event System.Action<Character> PlayerJoined;
    public static event System.Action<int> PlayerLeft;

    private static PhotonManager instance;

    private void Awake()
    {
        instance = this;
        IsMultiplayer = true;
        notStarted = true;
    }

    private void OnDestroy()
    {
        IsMultiplayer = false;
        LevelManager.BeforeSceneLoading -= DisconnectClient;
    }

    private void Start()
    {
        if (PhotonNetwork.ConnectUsingSettings(version))
        {
            Debug.Log("Connected");
            PhotonNetwork.OnEventCall += PhotonEventListner;
        }
        LevelManager.BeforeSceneLoading += DisconnectClient;
        StartCoroutine(UpdateCoroutine());

    }

    void DisconnectClient()
    {
        Debug.Log("Disconnecting...");
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.DestroyAll();
        PhotonNetwork.OnEventCall -= PhotonEventListner;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
    }
    
    private static void OnMessageReceived(byte code, int networkingID, object content)
    {
        MessageReceived?.Invoke(code, networkingID, content);
    }

    private static void OnGlobalMessageReceived(byte code, object content)
    {
        GlobalMessageReceived?.Invoke(code, content);
    }



    private static void PhotonEventListner(byte code, object content, int senderID)
    {
        switch(code)
        {
            case PhotonEventCode.ATTACK:
                var arg0 = (object[])content;
                OnMessageReceived(code, (int)arg0[0], arg0[1]);
                break;
            case PhotonEventCode.TIME:
                OnGlobalMessageReceived(code, content);
                break;
            case PhotonEventCode.PLAYER_JOINED:
                OnPlayerJoined(content);
                break;
            case PhotonEventCode.PLAYER_LEFT:
                OnPlayerLeft(content);
                break;
            default:
                Debug.LogError("Unknown PhotonEventCode");
                break;
        }
    }

    void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        var player = PhotonNetwork.Instantiate(playerPrefabName, SpawnPoint.spawnPoints[_players.Count].transform.position, SpawnPoint.spawnPoints[_players.Count].transform.rotation,0);
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.isWriting)
    //    {

    //    }
    //    else
    //    {

    //    }
    //}

    public static void SendMessage(byte photonEventCode, int networkingID, object content, bool reliable = false, ReceiverGroup receivers = ReceiverGroup.Others)
    {
        var obj = new object[]{ networkingID, content };
        PhotonNetwork.RaiseEvent(photonEventCode, obj, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = receivers });
    }

    public static void SendGlobalMessage(byte photonEventCode, object content, bool reliable = true, ReceiverGroup receivers = ReceiverGroup.All)
    {
        PhotonNetwork.RaiseEvent(photonEventCode, content, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = receivers});
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnPhotonPlayerDisconnected");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 1);
    }

    void UpdateMasterMessages()
    {
        if (GameTime.Instance != null && GameTime.Instance.timeBased)
            SendGlobalMessage(PhotonEventCode.TIME, GameTime.Instance.TimeLeft);
    }

#if UNITY_EDITOR
    public float secondsToWaitInEditor = 10;
#endif

    IEnumerator UpdateCoroutine()
    {
        while(true)
        {
            if(IsMaster)
            {
                UpdateMasterMessages();
#if UNITY_EDITOR
                if (secondsToWaitInEditor > 0)
                    secondsToWaitInEditor--;
                else if (notStarted)
                {
                    GameManager.OnGameReady();
                    notStarted = false;
                }

#endif
            }
            yield return new WaitForSeconds(1);

        }

    }

    public static void AddPlayer(Character movement)
    {
        if (_players == null)
            _players = new List<Character>();
        _players.Add(movement);

        if (movement.networking.isMine)
            SendGlobalMessage(PhotonEventCode.PLAYER_JOINED, movement.ID, true, ReceiverGroup.Others);
        if(Players.Count == instance.minPlayers &&  notStarted)
        {
            GameManager.OnGameReady();
        }
    }

    public static void RemovePlayer(Character movement)
    {
        if (_players == null)
            _players = new List<Character>();
        if (_players.Contains(movement))
            _players.Remove(movement);
        if(IsMultiplayer && movement.networking.isMine)
            SendGlobalMessage(PhotonEventCode.PLAYER_LEFT, movement.ID, true, ReceiverGroup.Others);
    }

    static void OnPlayerJoined(object content)
    {
        int id = (int)content;
        var character = Character.GetCharacter(id);
        if(character!=null)
        {
            Debug.Log("Player Joined " + character.ID);
            PlayerJoined?.Invoke(character);
        }
        else
        {
            Debug.LogError("Player not found id: " + id);
        }
    }

    static void OnPlayerLeft(object content)
    {
        int id = (int)content;

        Debug.Log("Player Left: " + id);
        PlayerLeft?.Invoke(id);
    }


}
