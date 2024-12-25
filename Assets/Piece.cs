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
    public bool Hovering;
    public bool Clicked;
    public bool WasClicked;

    public int Priority;

    private Vector2 lastPos;
    private bool justClicked;
    private float justClickedTime = 0.07f;

    private Move[] moves;

    public int TurnCount = 0;

    public bool CanIMoveThis;


    private Vector3? lastMovePos;

    void Start()
    {
        // Initialize the sprite renderer and moves array
        Sp = gameObject.FindObject("Sprite").GetComponent<SpriteRenderer>();
        moves = GetComponentsInChildren<Move>();
    }

    void Update()
    {
        // Use isOwned to check if this client owns the piece
        if (isOwned)
        {
            CanIMoveThis = true;
        }
        else
        {
            CanIMoveThis = false;
        }

        foreach (Move move in GetComponentsInChildren<Move>())
        {
            Collider2D[] touchingPieces = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.45f, 0.45f), 0);
            if (touchingPieces.Length > 0)
            {
                move.IsBlocked = false;
            }
            else
            {
                move.IsBlocked = true;
            }
        }

        // Detach the sprite for visual movement
        Sp.transform.parent = null;

        // Get mouse position in world space
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //if (!CanIMoveThis)
        //{
        //    return;
        //}

        // Highlight the piece when hovering
        if (Vector2.Distance(MousePos, transform.position) <= 0.5f && !Clicked)
        {
            Sp.DOColor(new Color(0.75f, 0.75f, 0.75f, 1), 0.1f);
            Hovering = true;
        }
        else
        {
            Sp.DOColor(Color.white, 0.1f);
            Hovering = false;
        }

        // Handle dragging
        if (Clicked)
        {
            Sp.transform.DOMove(MousePos, 0.1f);
        }
        else
        {
            Sp.transform.DOMove(transform.position, 0.1f);
        }

        // Handle clicking
        if (Input.GetMouseButtonDown(0))
        {
            if (Hovering && !justClicked)
            {
                Clicked = true;
                Hovering = false;
                Sp.sortingOrder = 1;
                lastPos = transform.position;
                justClicked = true;
                StartCoroutine(JustClickedRoutine());
            }
        }

        // Handle releasing the mouse button
        if (Input.GetMouseButtonUp(0))
        {
            if (Clicked)
            {
                if (!justClicked)
                {
                    Sp.transform.position = lastPos;
                    Clicked = false;
                    Sp.sortingOrder = 0;
                    WasClicked = true;

                    // Execute valid moves
                    foreach (Move move in moves)
                    {
                        if (move.Active)
                        {
                            move.GetComponent<SpriteRenderer>().DOFade(0, 0.1f);
                            if (Vector2.Distance(MousePos, move.transform.position) <= 0.35f && WasClicked)
                            {
                                Sp.transform.position = lastPos;
                                if (!isServer)
                                {
                                    lastMovePos = move.transform.position;
                                    ExecuteMoveToServer(move.transform.position);
                                }
                                else
                                {
                                    //ExecuteMoveToClients(move.transform.position);
                                    lastMovePos = move.transform.position;
                                    ExecuteMove(move.transform.position);
                                }
                                move.GetComponent<SpriteRenderer>().DOFade(0, 0f);
                                WasClicked = false;
                            }
                        }
                    }
                }
                else
                {
                    Hovering = true;
                }
            }
        }




        // Show potential moves when clicked
        if (Clicked)
        {
            foreach (Move move in moves)
            {
                move.GetComponent<SpriteRenderer>().DOFade(1, 0.1f);
            }
        }
    }

    [Command]
    public void ExecuteMoveToServer(Vector3 pos)
    {
        transform.position = pos;
    }

    [ClientRpc]
    public void ExecuteMoveToClients(Vector3 pos)
    {
        transform.position = pos;
    }

    public void ExecuteMove(Vector3 pos)
    {
        transform.position = pos;
    }

    private IEnumerator JustClickedRoutine()
    {
        yield return new WaitForSeconds(justClickedTime);
        justClicked = false;
    }

    private void OnDrawGizmos()
    {
        if (lastMovePos != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere((Vector3)lastMovePos, 0.5f);
        }
    }
}