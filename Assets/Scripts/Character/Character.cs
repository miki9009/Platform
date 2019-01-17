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
    public SkinnedMeshRenderer bodyMeshRenderer;
    
    public IEquipment rightArmItem;
    public IEquipment leftArmItem;

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
        }
    }
    public CharacterPhoton characterPhoton;

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
            return movement.GetType() == typeof(CharacterMovementAI);
        }
    }


    Identification identity;

    public static event Action<Character> CharacterCreated;

    public static List<Character> allCharacters = new List<Character>();

    public int ID
    {
        get; private set;
    }

    static Character localPlayer;
    public static Character GetLocalPlayer()
    {
        return localPlayer;
    }

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
        CharacterCreated?.Invoke(this);
    }


    private void Awake()
    {
        allCharacters.Add(this);
        movement = GetComponent<CharacterMovement>();
        identity = new Identification();

        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

    }

    void Start()
    {
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

    }

    private void OnDestroy()
    {
        PhotonManager.RemovePlayer(this);
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
public interface ILefttArmItem : IEquipment { }