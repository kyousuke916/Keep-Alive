using UnityEngine;
using System.Collections;

public class ButtonClick : MonoBehaviour {
	
	public void LoginLobby()
	{
		Application.LoadLevel ("Lobby");

	}

	public void LogOut()
	{
		GameObject gameObj = GameObject.Find ("LobbyManager");
		if (gameObj) 
		{
			gameObj.SetActive(false);
		}

		Application.LoadLevel ("Menu");
	}
}
