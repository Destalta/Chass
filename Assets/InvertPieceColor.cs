using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class InvertPieceColor : MonoBehaviour
{
    private Piece piece;
    private SpriteRenderer sp;

    private void Start()
    {
        
    }

    void Update()
    {
        piece = GetComponent<Piece>();
        if (piece.Sp)
        {
            sp = piece.Sp;
        }
        else
        {
            sp = piece.gameObject.FindObject("Sprite").GetComponent<SpriteRenderer>();
        }
        
        if (piece.IsBlack)
        {
            sp.material = (Material)Resources.Load("InvertColor");
        }
        else
        {
            sp.material = (Material)Resources.Load("PieceMaterial");
        }
    }
}
