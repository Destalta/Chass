using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;

public class Piece : NetworkBehaviour
{
    public bool IsBlack;
    public SpriteRenderer Sp;
    public Vector2 MousePos;
    private bool hovering;
    private bool clicked;
    private bool wasClicked;

    public int Priority;

    private Vector2 lastPos;
    private bool justClicked;
    private float justClickedTime = 0.07f;

    private Move[] moves;

    public int TurnCount = 0;

    public bool CanIMoveThis;


    void Start()
    {
        Sp = gameObject.FindObject("Sprite").GetComponent<SpriteRenderer>();
        moves = GetComponentsInChildren<Move>();
        if (isLocalPlayer)
        {
            CanIMoveThis = true;
        }
    }

    
    void Update()
    {
        Sp.transform.parent = null;

        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!CanIMoveThis)
        {
            return;
        }

        if (Vector2.Distance(MousePos, transform.position) <= 0.5f && !clicked)
        {
            Sp.DOColor(new Color(0.75f, 0.75f, 0.75f, 1), 0.1f);
            hovering = true;
        }
        else
        {
            Sp.DOColor(Color.white, 0.1f);
            hovering = false;
        }

        if (clicked)
        {
            Sp.transform.DOMove(MousePos, 0.1f);
        }
        else
        {
            Sp.transform.DOMove(transform.position, 0.1f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (hovering && !justClicked)
            {
                clicked = true;
                hovering = false;
                Sp.sortingOrder = 1;
                lastPos = transform.position;
                justClicked = true;
                StartCoroutine(JustClickedRoutine());
            }
        }

        if (clicked)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                Move move = moves[i];
                move.GetComponent<SpriteRenderer>().DOFade(1, 0.1f);
            }
        }
        else
        {
            for (int i = 0; i < moves.Length; i++)
            {
                Move move = moves[i];
                if (move.Active)
                {
                    move.GetComponent<SpriteRenderer>().DOFade(0, 0.1f);
                    if (Vector2.Distance(MousePos, move.transform.position) <= 0.25f && wasClicked)
                    {
                        move.Execute();
                        wasClicked = false;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (clicked)
            {
                if (!justClicked)
                {
                    Sp.transform.position = lastPos;
                    clicked = false;
                    Sp.sortingOrder = 0;
                    wasClicked = true;
                }
                else
                {
                    hovering = true;
                }
            }
        }
    }

    private IEnumerator JustClickedRoutine()
    {
        yield return new WaitForSeconds(justClickedTime);
        justClicked = false;
    }
}
