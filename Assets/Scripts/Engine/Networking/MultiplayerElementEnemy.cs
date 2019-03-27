using UnityEngine;

public class MultiplayerElementEnemy : MultiplayerEventElement
{
    public Animator anim;
    AnimatorClipInfo[] currentClipInfo;
    string currentClipName;
    Enemy enemy;

    protected override void OnMultiplayerAwake()
    {
        base.OnMultiplayerAwake();
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        enemy = GetComponent<Enemy>();
    }

    protected override void PhotonManager_MessageReceived(byte code, int id, object content)
    {
        if (code == PhotonEventCode.MULTIPLAYERELEMENT && id == ID)
        {
            var objects = (object[])content;
            lastRecievedPos = (Vector3)objects[0];
            lastRecievedRot = (Quaternion)objects[1];
            var recievedClip = (string)objects[2];
            if (currentClipName !=  recievedClip)
            {
                anim.Play(recievedClip);
                currentClipName = recievedClip;
            }
        }
    }

    public override void SendContent()
    {
        currentClipName = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        SendMultiplayerMessage(PhotonEventCode.MULTIPLAYERELEMENT, new object[] { transform.position, transform.rotation,  currentClipName});
    }

    protected override void OnMultiplayerInitialize()
    {
        base.OnMultiplayerInitialize();
        PhotonManager.MessageReceived += OnDie;
        if(enemy!=null)
        {
            enemy.EnemyHit += TriggerDieEvent;
        }
    }

    protected override void OnWillDestroy()
    {
        base.OnWillDestroy();
        PhotonManager.MessageReceived -= OnDie;
    }

    private void OnDie(byte code, int networkingID, object content)
    {
        if(code == PhotonEventCode.DIE && networkingID == ID)
        {
            int characterID = (int)content;
            var character = Character.GetCharacter(characterID);
            if (character != null)
            {
                enemy.Hit(character.movement);
            }
            else
            {
                Debug.LogError("Character with id: " + characterID + " not found.");
            }
        }
    }

    void TriggerDieEvent(Character character)
    {
        Debug.Log("Send Die Event");
        PhotonManager.SendMessage(PhotonEventCode.DIE, character.ID, true);
    }

    protected void Update()
    {
        if (IsRemote)
        {
            transform.position = Vector3.Lerp(transform.position, lastRecievedPos, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, lastRecievedRot, Time.deltaTime * lerpSpeed);
        }
    }

}