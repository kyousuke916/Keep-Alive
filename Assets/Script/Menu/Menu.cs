using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine.UI;

public class Menu : MonoBehaviour 
{
	public Image headPhoto;
	
	public string _path = "https://graph.facebook.com/";
	public string _picture = "/picture?type=large";

	public void FaceBookLogin()
	{
		var perms = new List<string>(){"public_profile", "email", "user_friends"};
		FB.LogInWithReadPermissions(perms, AuthCallback);

	}

	// Awake function from Unity's MonoBehavior
	void Awake ()
	{
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
	}
	
	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}
	
	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log(aToken.UserId);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				Debug.Log(perm);
			}

			StartCoroutine("LoadPicture", aToken.UserId);

			//Application.LoadLevel ("Lobby");
		} else {
			Debug.Log("User cancelled login");
		}
	}

	IEnumerator LoadPicture(string id)
	{
		WWW www = new WWW (_path + id + _picture);

		Debug.Log("head:"+www.url);

		yield return www;
		headPhoto.sprite = Sprite.Create(www.texture, new Rect(0,0, www.texture.width, www.texture.height), new Vector2());
		headPhoto.color = new Color (255, 255, 255, 255);
	}

}
