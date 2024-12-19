using Mirror;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{

    public Player WhitePlayer;
    public Player BlackPlayer;

    public List<Piece> WhitePieces = new List<Piece>();
    public List<Piece> BlackPieces = new List<Piece>();


    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreateCharacter);

        foreach (Piece p in FindObjectsByType<Piece>(FindObjectsSortMode.None))
        {
            if (!p.IsBlack)
            {
                WhitePieces.Add(p);
            }
            else
            {
                BlackPieces.Add(p);
            }
        }

        WhitePieces.Sort((p1, p2) => p2.Priority.CompareTo(p1.Priority));
        BlackPieces.Sort((p1, p2) => p2.Priority.CompareTo(p1.Priority));

    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        // you can send the message here, or wherever else you want
        CreatePlayerMessage characterMessage = new CreatePlayerMessage
        {
            name = "PlayerName",
        };

        NetworkClient.Send(characterMessage);
    }

    void OnCreateCharacter(NetworkConnectionToClient conn, CreatePlayerMessage message)
    {
        // Create an empty GameObject
        GameObject playerObject = new GameObject("PlayerObject");

        // Add the Player component to the GameObject
        Player player = playerObject.AddComponent<Player>();

        // Set the player's name
        player.name = message.name;

        // Assign the player to a team
        if (WhitePlayer == null)
        {
            WhitePlayer = player;
        }
        else if (BlackPlayer == null)
        {
            BlackPlayer = player;
        }
        else
        {
            Debug.LogWarning("Both players are already assigned.");
            return;
        }

        // Assign the player's main piece (ensure the lists are not empty)
        if (WhitePieces.Count > 0 && BlackPieces.Count > 0)
        {
            Piece mainPiece = (player == WhitePlayer) ? WhitePieces[0] : BlackPieces[0];

            // Set the position of the new player to the main piece's position
            playerObject.transform.position = mainPiece.transform.position;

            // Add the player GameObject to the connection
            NetworkServer.AddPlayerForConnection(conn, playerObject);
        }
        else
        {
            Debug.LogError("Piece lists are empty. Ensure pieces are added in OnStartServer.");
        }
    }

}