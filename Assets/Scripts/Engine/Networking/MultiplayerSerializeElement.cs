
public abstract class MultiplayerSerializeElement : MultiplayerObject
{

    protected object[] Objects;
    protected abstract void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);

    protected override void OnMultiplayerAwake()
    {
  
    }

    protected override void OnMultiplayerInitialize()
    {

    }

    protected override void OnWillDestroy()
    {

    }
}