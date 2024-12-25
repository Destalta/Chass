using Mirror;
using System.Collections.Generic;
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

        AssignPieces();
    }

    public void AssignPieces()
    {
        foreach (Piece p in FindObjectsByType<Piece>(FindObjectsSortMode.None))
        {
            if (!NetworkServer.spawned.ContainsKey(p.GetComponent<NetworkIdentity>().netId))
            {
                NetworkServer.Spawn(p.gameObject);
            }

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

        // Send the message to the server
        CreatePlayerMessage characterMessage = new CreatePlayerMessage
        {
            name = "PlayerName",
        };

        NetworkClient.Send(characterMessage);
    }

    void OnCreateCharacter(NetworkConnectionToClient conn, CreatePlayerMessage message)
    {
        // Create the player GameObject
        GameObject playerObject = Instantiate(playerPrefab); // Ensure playerPrefab is assigned in the Inspector

        // Add the Player component to the GameObject
        Player player = playerObject.GetComponent<Player>();
        if (player == null)
        {
            player = playerObject.AddComponent<Player>();
        }

        // Set the player's name
        player.name = message.name;

        // Assign the player to a team
        if (WhitePlayer == null)
        {
            WhitePlayer = player;
            AssignPiecesToPlayer(conn, WhitePieces); // Assign white pieces to this player
        }
        else if (BlackPlayer == null)
        {
            BlackPlayer = player;
            AssignPiecesToPlayer(conn, BlackPieces); // Assign black pieces to this player
        }
        else
        {
            Debug.LogWarning("Both players are already assigned.");
            Destroy(playerObject); // Destroy the extra player object
            return;
        }

        // Assign the player GameObject to the connection
        NetworkServer.AddPlayerForConnection(conn, playerObject);
    }

    void AssignPiecesToPlayer(NetworkConnection conn, List<Piece> pieces)
    {
        foreach (Piece piece in pieces)
        {
            var identity = piece.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                if (!NetworkServer.spawned.ContainsKey(identity.netId))
                {
                    NetworkServer.Spawn(piece.gameObject);
                }

                Debug.Log($"Assigning ownership of {piece.name} to connection {conn.connectionId}");
                identity.AssignClientAuthority((NetworkConnectionToClient)conn);
            }
            else
            {
                Debug.LogError($"Piece {piece.name} does not have a NetworkIdentity component.");
            }
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // Safeguard against null identity
        if (conn.identity != null)
        {
            GameObject playerObject = conn.identity.gameObject;

            // Check if the disconnecting player is white or black
            if (WhitePlayer != null && WhitePlayer.gameObject == playerObject)
            {
                Debug.Log("Removing White Player");
                WhitePlayer = null;
            }
            else if (BlackPlayer != null && BlackPlayer.gameObject == playerObject)
            {
                Debug.Log("Removing Black Player");
                BlackPlayer = null;
            }

            // Destroy the player's GameObject
            NetworkServer.Destroy(playerObject);
        }
        else
        {
            Debug.LogWarning($"Connection {conn.connectionId} has no identity. Skipping cleanup.");
        }

        // Call the base class method to handle default disconnect logic
        base.OnServerDisconnect(conn);
    }
}
