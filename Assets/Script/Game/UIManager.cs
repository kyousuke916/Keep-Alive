using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private Canvas m_UICanvas;
    public Canvas UICanvas { get { return m_UICanvas; } }

    [SerializeField]
    private RectTransform m_InfoRT;

    [SerializeField]
    private RectTransform m_JoyStickRT;

    private static PlayerController mLocalPlayerControler;

    private static PlayerCam mLocalPlayerCam;


    public static PlayerController LocalPlayer { get { return mLocalPlayerControler; } }
    public static PlayerCam LocalPlayerCam { get { return mLocalPlayerCam; } }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UIChat.Create(m_InfoRT);
        UIControl.Create(m_JoyStickRT);
    }

    public void SetLocalPlayer(PlayerController player, PlayerCam cam)
    {
        mLocalPlayerControler = player;
        mLocalPlayerCam = cam;
    }
}
