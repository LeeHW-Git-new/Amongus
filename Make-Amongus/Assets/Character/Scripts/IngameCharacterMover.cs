using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//0x : 0 - 크루원      1 = 임포스터
//x0 : 0 - 살아있음    1 = 죽음
//00 = 살아있는 크루원 01 = 살아있는 임포트서
//10 = 죽은크루원      11 = 죽은 임포스터
public enum EPlayerType
{
    Crew = 0,
    Imposter = 1,
    Ghost = 2,
    Crew_Alive = 0,
    Imposter_Alive =1,
    Grew_Ghsot = 2,
    Imposter_Ghost = 3,
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

    [SyncVar]
    public bool isReporter = false;

    public EPlayerColor foundDeadbodyColor;
    
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
            RpcTeleport(target.transform.position);
            target.Dead(playerColor);
            killCooldown = GameSystem.Instance.killCooldown;
        }
    }

    public void Dead(EPlayerColor imposterColor)
    {
        playerType |= EPlayerType.Ghost;
        RpcDead(imposterColor, playerColor);
        var manager = NetworkRoomManager.singleton as AmongUsRoomManager;
        var deadbody = Instantiate(manager.spawnPrefabs[1], transform.position, transform.rotation).GetComponent<Deadbody>();
        NetworkServer.Spawn(deadbody.gameObject);
        deadbody.RpcSetColor(playerColor);
    }

    [ClientRpc]
    private void RpcDead(EPlayerColor imposterColor, EPlayerColor crewColor)
    {
        if(hasAuthority)
        {
            animator.SetBool("isGhost", true);
            IngameUIManager.Instance.KillUI.Open(imposterColor, crewColor);

            var players = GameSystem.Instance.GetPlayerList();
            foreach(var player in players)
            {
                if((player.playerType & EPlayerType.Ghost) == EPlayerType.Ghost)
                {
                    player.SetVisibility(true);
                }
            }

            GameSystem.Instance.ChangeLightMode(EPlayerType.Ghost);
        }
        else
        {
            var myPlayer = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as IngameCharacterMover;
            if(((int)myPlayer.playerType & 0x02)!=(int)EPlayerType.Ghost)
            {
                SetVisibility(false);
            }
        }

        var collider = GetComponent<BoxCollider2D>();
        if(collider)
        {
            collider.enabled = false;
        }
    }

    public void Report()
    {
        CmdReport(foundDeadbodyColor);
    }

    [Command]
    public void CmdReport(EPlayerColor deadbodyColor)
    {
        isReporter = true;
        GameSystem.Instance.StartReportMeeting(deadbodyColor);
    }
    public void SetVisibility(bool isVsible)
    {
        if(isVsible)
        {
            var color = PlayerColor.GetColor(playerColor);
            color.a = 1;
            spriteRenderer.material.SetColor("_PlayerColor", color);
            nicknameText.text = nickname;
        }
        else
        {
            var color = PlayerColor.GetColor(playerColor);
            color.a = 0f;
            spriteRenderer.material.SetColor("_PlayerColor", color);
            nicknameText.text = "";
        }
    }
}
