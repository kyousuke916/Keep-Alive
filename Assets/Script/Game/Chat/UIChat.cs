using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Networking.Network;
using System.Collections;

public class UIChat : MonoBehaviour
{
    #region Instance

    private const string UI_CHAT_PATH = "Game/Chat/ChatUI";

    private static MonoBehaviour Mono { get { return LobbyManager.s_Singleton; } }

    public static void Create(RectTransform rts)
    {
        Mono.StartCoroutine(InstantiateUI(rts));
    }

    private static IEnumerator InstantiateUI(RectTransform containertRT)
    {
        var request = Resources.LoadAsync<GameObject>(UI_CHAT_PATH);

        yield return request;

        var chatGo = Instantiate<GameObject>(request.asset as GameObject);
        var chatRT = chatGo.transform as RectTransform;

        chatRT.SetParent(containertRT, false);
        chatRT.anchoredPosition = Vector2.zero;
    }

    #endregion

    #region Field

    public static UIChat Instance { get; private set; }

    [SerializeField]
    private Text m_Chatline;

    [SerializeField]
    private InputField m_Input;

    #endregion

    #region Unity Event

    void Awake()
    {
        Instance = this;

        m_Chatline.text = "";

        m_Input.onEndEdit.AddListener(OnSubmit);
    }
    
    void OnDestroy()
    {
        Instance = null;

        m_Input.onEndEdit.RemoveListener(OnSubmit);
    }

    #endregion

    private void OnSubmit(string msg)
    {
        PostChatMessage(msg);

        m_Input.text = "";
        //m_Input.ActivateInputField();
        //m_Input.Select();
    }

    private void PostChatMessage(string msg)
    {
        if (Chat.Instance != null)
            Chat.Instance.PostChatMessage(msg);
    }

    public void InsertDialog(string msg)
    {
        //string dialog = ((string.IsNullOrEmpty(m_Chatline.text) ? "" : "\n") + msg);
        //((string.IsNullOrEmpty(m_Chatline.text) ? "" : "\n") + msg);

        string prefix = (string.IsNullOrEmpty(m_Chatline.text) ? "" : "\n");
        m_Chatline.text += string.Format("{0}{1}", prefix, msg);

        /*
        PlayerManager pm = GameManager.GetPlayerManager(GameManager.Instance.LocalPlayerKey);

        string prefix = (string.IsNullOrEmpty(m_Chatline.text) ? "" : "\n");
        string nickname = (pm != null) ? pm.RoleName : "unknow";
        m_Chatline.text += string.Format("{0}{1}:{2}", prefix, nickname, msg);
        */
    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InsertDialog(string.Format("Test:{0}", Time.time));
        }
    }*/
}
