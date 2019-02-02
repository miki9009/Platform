using UnityEngine;
using Engine;

[RequireComponent(typeof(LevelElement))]
public abstract class MultiplayerElement : MonoBehaviour
{
    bool isMultiplayer;
    protected int id;
    protected virtual void Awake()
    {
        if (!PhotonManager.IsMultiplayer)
            return;
        isMultiplayer = true;
        Level.LevelLoaded += Initialize;
    }

    private void OnDestroy()
    {
        Level.LevelLoaded -= Initialize;
        if(isMultiplayer)
        {
            PhotonManager.MessageReceived -= PhotonManager_MessageReceived;
        }
    }

    private void Initialize()
    {
        var multiplayerElement = GetComponent<IMultiplayerElement>();
        if (multiplayerElement == null)
        {
            Debug.LogError("Didn't find IMultiplayer on gameObject: " + name);
            return;
        }

        id = GetComponent<LevelElement>().elementID;
        PhotonManager.MessageReceived += PhotonManager_MessageReceived;

        multiplayerElement.IsMultiplayer = PhotonManager.IsMultiplayer;
        multiplayerElement.IsRemote = multiplayerElement.IsMultiplayer && !PhotonManager.IsMaster;
    }

    protected abstract void PhotonManager_MessageReceived(byte code, int id, object content);


    public void SendMultiplayerMessage(byte code, object content)
    {
        PhotonManager.SendMessage(code, id, content);
    }
}

public interface IMultiplayerElement
{
    bool IsRemote { get; set; }
    bool IsMultiplayer { get; set; }
}