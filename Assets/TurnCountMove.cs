using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCountMove : Move
{

    public int MaxTurnCount = 1;

    public override void Execute()
    {
        base.Execute();
        if (piece.TurnCount > MaxTurnCount - 1)
        {
            Active = false;
        }
    }
}
