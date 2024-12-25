using DG.Tweening;
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

    public virtual void OnExecute()
    {
        
    }

    public virtual void Update()
    {
        
    }
}
