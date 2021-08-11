using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobbyCharacterMover : CharacterMover
{
    public void CompleteSpawn()
    {
        if(hasAuthority)
        {
            isMoveable = true;
        }
    }
}
