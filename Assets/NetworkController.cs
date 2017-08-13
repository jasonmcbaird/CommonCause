using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkController: NetworkManager
{

	public Transform spawnPosition;   
	public int currentPlayer;
	public GameObject playerOnePrefab;
	public GameObject playerTwoPrefab;

	//Called on client when connect
	public override void OnClientConnect(NetworkConnection conn)
	{
		ClientScene.AddPlayer(conn, 0);
		currentPlayer += 1;
	}

	// Server
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) { 
		var playerPrefab = currentPlayer % 2 == 0 ? playerOnePrefab : playerTwoPrefab;
		print("Spawning player " + currentPlayer);

		var player = GameObject.Instantiate(playerPrefab, new Vector3(spawnPosition.position.x, spawnPosition.position.y, 1), Quaternion.identity) as GameObject;        

		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		NetworkServer.Spawn(player);
	}
}