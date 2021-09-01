using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum EPlayerType
{
    Crew,
    Imposter
}

public class IngameCharacterMover : CharacterMover
{
    [SyncVar(hook = nameof(SetPlayerType_Hook))]
    public EPlayerType playerType;

    private void SetPlayerType_Hook(EPlayerType _, EPlayerType type)
    {
        if(hasAuthority && type == EPlayerType.Imposter)
        {
            IngameUIManager.Instance.KillButtonUI.Show(this);
            playerFinder.SetKillRange(GameSystem.Instance.killRange + 1f);
        }
    }

    [SerializeField]
    private PlayerFinder playerFinder;

    [SyncVar]
    private float killCooldown;

    public float KillCooldown { get { return killCooldown; } }


    public bool isKillable { get { return killCooldown < 0f && playerFinder.targets.Count != 0f; } }
    
    [ClientRpc]
    public void RpcTeleport(Vector3 position)
    {
        transform.position = position;
    }

    public void SetNicknameColor(EPlayerType type)
    {
        if(playerType == EPlayerType.Imposter && type == EPlayerType.Imposter)
        {
            nicknameText.color = Color.red;
        }
    }

    public void SetKillCooldown()
    {
        if(isServer)
        {
            killCooldown = GameSystem.Instance.killCooldown;
        }
    }

    public override void Start()
    {
        base.Start();

        if(hasAuthority)
        {
            IsMoveable = true;

            var myRoomPlayer = AmongUsRoomPlayer.MyRoomPlayer;
            myRoomPlayer.myCharacter = this;
            CmdSetPlayerCharacter(myRoomPlayer.nickname, myRoomPlayer.playerColor);
        }

        GameSystem.Instance.AddPlayer(this);
    }

    private void Update()
    {
        if(isServer && playerType == EPlayerType.Imposter)
        {
            killCooldown -= Time.deltaTime;
        }
            
    }

    [Command]
    private void CmdSetPlayerCharacter(string nickname, EPlayerColor color)
    {
        this.nickname = nickname;
        playerColor = color;
    }

    public void Kill()
    {
        CmdKill(playerFinder.GetFirstTarget().netId);
    }

    [Command]
    private void CmdKill(uint targetNetId)
    {
        IngameCharacterMover target = null;
        foreach(var player in GameSystem.Instance.GetPlayerList())
        {
            if(player.netId == targetNetId)
            {
                target = player;
            }
        }

        if(target != null)
        {
            var manager = NetworkRoomManager.singleton as AmongUsRoomManager;
            var deadbody = Instantiate(manager.spawnPrefabs[1], target.transform.position, target.transform.rotation).GetComponent<Deadbody>();
            NetworkServer.Spawn(deadbody.gameObject);
            deadbody.RpcSetColor(target.playerColor);
            killCooldown = GameSystem.Instance.killCooldown;
        }
    }
}
