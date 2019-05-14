using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using System;

[DefaultExecutionOrder(-50)]
public class Character : MonoBehaviour
{
    public PhotonView networking;
    public CharacterStatistics stats;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CharacterMovement movement;
    [Header("Metarig")]
    public Transform leftUpperArm;
    public Transform leftLowerArm;
    public Transform rightUpperArm;
    public Transform rightLowerArm;
    public Transform chest;
    public SkinnedMeshRenderer bodyMeshRenderer;
    public GameProgress gameProgress;

    public event Action<int> PlacementChanged;
    public event Action<int> WaypointVisited;
        public CharacterPhoton characterPhoton;

    public IEquipment rightArmItem;
    public IEquipment leftArmItem;

    static Character localPlayer;
    public static Character GetLocalPlayer()
    {
        return localPlayer;
    }
    bool _isDead;
    public bool IsDead
    {
        get
        {
            return _isDead;
        }

        set
        {
            if(_isDead != value)
            {
                if(value)
                {
                    allCharacters.Remove(this);
                }
                else
                {
                    allCharacters.Add(this);
                }
            }
            _isDead = value;
            anim.SetBool("isDead", _isDead); 
            Dead?.Invoke(this);
        }
    }
    public int Health
    {
        get
        {
            return (stats != null ? stats.Health : 0);
        }

        set
        {
            stats.Health = value;
            if(stats.Health <=0)
            {
                movement.Die();
            }
        }
    }
    public bool IsLocalPlayer
    {
        get
        {
            return this == localPlayer;
        }
    }
    public bool IsBot
    {
        get
        {
            return movement.IsBot;
        }
    }
    public bool IsHost
    {
        get
        {
            return PhotonManager.IsMaster && IsLocalPlayer && !IsBot;
        }
    }
    public int ID
    {
        get; private set;
    }

    Identification identity;

    public static event Action<Character> CharacterCreated;
    public static event Action<Character> Dead;
    public static List<Character> allCharacters = new List<Character>();





    public void AddRightArmItem(IRightArmItem item)
    {
        if (rightArmItem != null)
        {
            rightArmItem.Remove();
        }
        rightArmItem = item;
    }

    public void AddItem(IEquipment equipment)
    {       
        if (equipment != null)
        {
            var types = equipment.GetType().GetInterfaces();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == typeof(IRightArmItem))
                {
                    if (rightArmItem != null)
                    {
                        rightArmItem.Remove();
                        //Debug.Log("Right Hand item removed");
                    }
                    //Debug.Log("Right Hand item equipped");
                    rightArmItem = equipment;
                    break;
                }
                else if (types[i] == typeof(ILefttArmItem))
                {
                    if (leftArmItem != null)
                    {
                        leftArmItem.Remove();
                        //Debug.Log("Left Hand item removed");
                    }
                    //Debug.Log("Left Hand item equipped");
                    leftArmItem = equipment;
                    break;
                }
            }
            equipment.Apply();
        }
    }

    public void CreateLocalPlayer()
    {
        localPlayer = this;
        Controller.Instance.character = this;
        Controller.Instance.gameCamera.SetTarget(transform);
        stats = CharacterSettingsModule.Statistics;
        ChangeArmor(stats.armorType);
    }


    private void Awake()
    {
        GameManager.LevelClear += KillMe;
        gameProgress = new GameProgress(this);
        movement = GetComponent<CharacterMovement>();
        identity = new Identification();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        allCharacters.Add(this);    //DON"T MOVE TO AWAKE, NEEDS TO BE A FRAME BREAK BETWEEN REMOVE AND ADD
        if (movement is ILocalPlayer)
        {
            if(PhotonManager.IsMultiplayer) //MULTIPLAYER
            {
                if (networking.isMine)
                {
                    CreateLocalPlayer();
                }
                else
                {
                    DisableOnNotMine();
                }
                ID = networking.viewID;
                PhotonManager.AddPlayer(this);
            }
            else //LOCAL PLAYER NO BOTS
            {
                CreateLocalPlayer();
                ID = identity.ID;
            }
        }
        else //BOTS
        {
            ID = identity.ID;
        }
        stats.Initialize(this);
        CharacterCreated?.Invoke(this);
    }

    void KillMe()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if(allCharacters.Contains(this))
            allCharacters.Remove(this);

        if(GetComponent<PhotonView>())
            PhotonManager.RemovePlayer(this);
        GameManager.LevelClear -= KillMe;
    }

    void DisableOnNotMine()
    {
        movement.isRemoteControl = true;
        rb.useGravity = false;
    }

    public void ChangeArmor(string armorID)
    {
        var config = ConfigsManager.GetConfig<CharacterConfig>();
        bodyMeshRenderer.sharedMesh = config.GetMesh(armorID);
    }

    public static Character GetCharacter(int id)
    {
        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (id == allCharacters[i].ID)
                return allCharacters[i];
        }
        Debug.LogError("Player with id: " + id + " not found");
        return null;
    }

    public void OnPlacementChanged(int placement)
    {
        PlacementChanged?.Invoke(placement);
    }

    public void OnWaypointVisited(int index)
    {
        WaypointVisited?.Invoke(index);
    }

}
public class Identification
{
    private static int counter = 0;

    private readonly int id = 1000;
    public int ID
    {
        get
        {
            return id;
        }
    }

    public Identification()
    {
        id = counter; 
        counter++;
    }
}

[Serializable]
public class GameProgress
{
    Character _character;
    public GameProgress(Character character)
    {
        _character = character;
    }
    public int Placement
    {
        get
        {
            return _placement;
        }
        set
        {
            _placement = value;
            if(_character && _placement != _prevPlacement)
            {
                Debug.Log("Changed to place: " + _placement + "; lap: " + lap);
                _character.OnPlacementChanged(value);
                _prevPlacement = _placement;
            }
        }
    }

    public int CurrentWaypoint
    {
        get
        {
            return _currentWaypoint;
        }

        set
        {
            if(_currentWaypoint != value)
            {
                _currentWaypoint = value;
                if(_character)
                {
                    _character.OnWaypointVisited(_currentWaypoint);
                }
            }
        }
    }

    public float raceProgress = 0;
    int _currentWaypoint = 0;
    public int lap = 1;
    int _placement;
    int _prevPlacement;
}

public interface IEquipment
{
    CollectionObject CollectionObject { get; set; }
    void Apply();
    void Remove();
    void BackToCollection();
}

public interface ILocalPlayer
{

}

public interface IRightArmItem : IEquipment{ }
public interface ILefttArmItem : IEquipment
{
    void Use();
    void StopUsing();
}

public interface IMovable
{
    Rigidbody Rigidbody
    {
        get;
    }

    bool ActiveAndEnabled
    {
        get;
    }

    RigidbodyConstraints PushConstraints
    {
        get;
    }

    RigidbodyConstraints NonPushConstraints
    {
        get;
    }
}