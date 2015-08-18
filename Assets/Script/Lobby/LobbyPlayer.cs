using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Networking.Network
{
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        #region COLRO

        private static Color NORMAL_COLOR = Color.white;
        private static Color JOIN_COLOR = new Color(255.0f / 255.0f, 0.0f, 101.0f / 255.0f, 1.0f);
        private static Color NOT_READY_COLOR = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        private static Color READY_COLOR = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        private static Color TRANSPARENT_COLOR = new Color(0f, 0f, 0f, 0f);

        private static Color OTHER_PLAYER_COLOR = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        private static Color LOCAL_PLAYER_COLOR = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        #endregion

        #region Field

        [SerializeField]
        private Button m_ColorButton;

        [SerializeField]
        private InputField m_NameInput;

        [SerializeField]
        private Button m_ReadyButton;

        [SerializeField]
        private Button m_WaitingPlayerButton;

        private LobbyPlayerParam mParam;

        private Text mColorButtonTxt;
        private Text mReadyButtonTxt;
        private Image mBackgroundImg;

        #endregion

        #region Unity Event

        void Awake()
        {
            mParam = GetComponent<LobbyPlayerParam>();

            mReadyButtonTxt = m_ReadyButton.GetComponentInChildren<Text>();
            mColorButtonTxt = m_ColorButton.GetComponentInChildren<Text>();
            mBackgroundImg = GetComponent<Image>();
        }

        void OnDestroy()
        {
            
        }

        #endregion

        #region Override 

        /// <summary>Called when the client connects to a server</summary>
        public override void OnStartClient()
        {
            Log("OnStartClient :" + readyToBegin);

            ChangeReadyButtonColor(readyToBegin ? JOIN_COLOR : NOT_READY_COLOR);
            ChangeReadyButtonText(readyToBegin ? "READY" : "...");

            //All networkbehaviour base function don't do anything
            //but NetworkLobbyPlayer redefine OnStartClient, so we need to call it here
            base.OnStartClient();
        }

        public override void OnClientEnterLobby()
        {
            name = string.Format("Player[{0}][{1}]", netId, slot);

            Log("OnClientEnterLobby => " + name + " : " + readyToBegin);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            ChangeColorButtonText(string.Format("[{0}][{1}]", slot, netId));

            ChangeNameInputInteractable(false);

            //ChangeReadyButtonColor(readyToBegin ? JOIN_COLOR : NOT_READY_COLOR);
            ChangeReadyButtonTextColor(NORMAL_COLOR);
            //ChangeReadyButtonText(readyToBegin ? "READY" : "...");
            ChangeReadyButtonInteractable(false);

            ChangeBackgroundColor(OTHER_PLAYER_COLOR);

            mParam.Refresh();

            /*
            //依照 slot 設定對應顏色(這裡不做的話，會變成大家一進來都是同一種顏色)
            if (playerColor == Color.white)
                CmdColorChange(slot);

            ChangeReadyButtonColor(NotReadyColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
            readyButton.interactable = false;

            OnClientReady(false);
            */
        }

        /// <summary>Called when the local player object has been set up</summary>
        public override void OnStartLocalPlayer()
        {
            Log("OnStartLocalPlayer:" + netId);

            //if( (isServer && !NetworkServer.active) || (isClient && !LobbyManager.s_Singleton.client.isConnected))
            //{//the server isn't started, rogue player, just delete it
            //    Destroy(gameObject);
            //    return;
            //}
            
            //have to use child count of player prefab already setup as "this.slot" is not set yet
            if (mParam.RoleName == string.Empty)
                mParam.CmdNameChanged("Player" + LobbyPlayerList._instance.playerListContentTransform.childCount);

            ChangeBackgroundColor(LOCAL_PLAYER_COLOR);
            ChangeNameInputInteractable(true);
            ChangeReadyButtonText("JOIN");
            ChangeReadyButtonInteractable(true);

            m_NameInput.onEndEdit.RemoveAllListeners();
            m_NameInput.onEndEdit.AddListener(OnNameChanged);

            m_ColorButton.onClick.RemoveAllListeners();
            m_ColorButton.onClick.AddListener(OnColorClicked);

            m_ReadyButton.onClick.RemoveAllListeners();
            m_ReadyButton.onClick.AddListener(OnReadyClicked);
        }

        public override void OnClientReady(bool readyState)
        {
            Log("OnClientReady:" + name + " : " + readyState);

            if (readyState)
            {
                ChangeReadyButtonColor(TRANSPARENT_COLOR);
                ChangeReadyButtonText("READY");
                ChangeReadyButtonTextColor(READY_COLOR);
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JOIN_COLOR : NOT_READY_COLOR);
                ChangeReadyButtonText(isLocalPlayer ? "JOIN" : "...");
                ChangeReadyButtonTextColor(Color.white);
            }

            ChangeReadyButtonInteractable(isLocalPlayer);
        }

        #endregion

        #region Other

        public void RpcToggleJoinButton(bool enable)
        {
            Log(name + " RpcToggleJoinButton:" + enable);

            m_ReadyButton.gameObject.SetActive(enable);

            m_WaitingPlayerButton.gameObject.SetActive(!enable);
        }

        private void ChangeNameInputInteractable(bool interactable)
        {
            m_NameInput.interactable = interactable;
        }
        
        private void ChangeReadyButtonColor(Color color)
        {
            ColorBlock cb = m_ReadyButton.colors;
            cb.normalColor = color;
            cb.pressedColor = color;
            cb.highlightedColor = color;
            cb.disabledColor = color;

            m_ReadyButton.colors = cb;
        }

        private void ChangeColorButtonText(string text)
        {
            mColorButtonTxt.text = text;
        }

        private void ChangeReadyButtonText(string text)
        {
            mReadyButtonTxt.text = text;
        }

        private void ChangeReadyButtonTextColor(Color color)
        {
            mReadyButtonTxt.color = color;
        }

        private void ChangeReadyButtonInteractable(bool interactable)
        {
            m_ReadyButton.interactable = interactable;
        }

        private void ChangeBackgroundColor(Color color)
        {
            
        }

        #endregion

        #region UI Handler

        private void OnNameChanged(string id)
        {
            mParam.ChangeName(id);
        }

        private void OnColorClicked()
        {
            mParam.ChangeColor();
        }

        private void OnReadyClicked()
        {
            m_ReadyButton.onClick.RemoveAllListeners();

            if (isLocalPlayer)
                SendReadyToBeginMessage();
        }

        #endregion

        #region Test

        private static void Log(string data)
        {
            Debug.Log("LP == " + data);
        }

        #endregion
    }
}
