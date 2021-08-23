using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RobbyCharacterMover : CharacterMover
{
   [SyncVar(hook = nameof(SetOwnerNetId_Hook))]
    public uint ownerNetId;

    public void SetOwnerNetId_Hook(uint _,uint newOwnerId)
    {
        var players = FindObjectsOfType<AmongUsRoomPlayer>();
        foreach(var player in players)
        {
            player.lobbyPlayerCharacter = this;
            break;
        }
    }
    public void CompleteSpawn()
    {
        if(hasAuthority)
        {
            IsMoveable = true;
        }
    }
}
