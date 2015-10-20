using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Networking.Network;
using System.Collections;


public class UIControl : MonoBehaviour {

    #region Instance

    private const string UI_PATH = "Game/UI/ControlUI";

    private static MonoBehaviour Mono { get { return LobbyManager.s_Singleton; } }

    private PlayerController mLocalPlayer;

    private PlayerCam mLocalPlayerCam;

    [SerializeField]
    private HitControlManager m_HitManger;

    [SerializeField]
    private MoveControlManager m_MoveManger;

    public static void Create(RectTransform rts)
    {
        Mono.StartCoroutine(InstantiateUI(rts));
    }

    private static IEnumerator InstantiateUI(RectTransform containertRT)
    {
        var request = Resources.LoadAsync<GameObject>(UI_PATH);

        yield return request;

        var uiObj = Instantiate<GameObject>(request.asset as GameObject);
        var objRT = uiObj.GetComponent<RectTransform>();

        objRT.SetParent(containertRT, false);
        objRT.anchoredPosition = new Vector2(0f, 0f);
    }

    #endregion

    // Use this for initialization
	void Start ()
    {        
        m_HitManger.OnHitAction += PlayerHitAction;
        m_HitManger.OnDragAction += FixPlayerCam;
        m_MoveManger.OnMove += PlayerMove;

        mLocalPlayer = UIManager.LocalPlayer;

        mLocalPlayerCam = UIManager.LocalPlayerCam;
	}
	
    private void PlayerHitAction(int action)
    {

    }

    private void FixPlayerCam(Vector2 value)
    {
        mLocalPlayerCam.MoveCamera(value.x, value.y);
    }

    private void PlayerMove(Vector2 value)
    {
        mLocalPlayer.Move(value.x, value.y);
    }
}
