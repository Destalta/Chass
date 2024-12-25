using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCountMove : Move
{

    public int MaxTurnCount = 1;

    public override void OnExecute()
    {
        base.OnExecute();
        if (piece.TurnCount > MaxTurnCount - 1)
        {
            Active = false;
        }
    }
}
