using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Deadbody : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    [ClientRpc]

    public void RpcSetColor(EPlayerColor color)
    {
        spriteRenderer.material.SetColor("_PlayerColor", PlayerColor.GetColor(color));
    }
}
