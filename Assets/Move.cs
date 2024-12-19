using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    protected Piece piece;
    public bool Active = true;
    public bool IsBlocked = false;


    public virtual void Start()
    {
        piece = GetComponentInParent<Piece>();
    }

    public virtual void Execute()
    {
        piece.Sp.transform.position = piece.transform.position;
        piece.transform.position = transform.position;
    }

    public virtual void Update()
    {
        Collider2D[] touchingPieces = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.5f, 0.5f), 0);
        if (touchingPieces.Length > 0 )
        {
            IsBlocked = false;
        }
    }
}
