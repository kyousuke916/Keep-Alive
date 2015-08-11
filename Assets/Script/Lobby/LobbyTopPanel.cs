using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Networking.Network
{
    public class LobbyTopPanel : MonoBehaviour
    {
        [SerializeField]
        private LobbyManager m_LobbyManager;

        [SerializeField]
        private LobbyManagerUI m_LobbyManagerUI;

        [SerializeField]
        private Text m_StatusInfoTxt;

        [SerializeField]
        private Text m_HostInfoTxt;

        [SerializeField]
        private Button m_BackBtn;

        [SerializeField]
        private Button m_TestBtn;

        public bool isInGame = false;

        private bool isDisplayed = true;
        private Image panelImage;

        void Awake()
        {
            panelImage = GetComponent<Image>();

            m_BackBtn.gameObject.SetActive(false);

            m_BackBtn.onClick.AddListener(m_LobbyManagerUI.GoBackButton);
            m_TestBtn.onClick.AddListener(OnTest);
        }

        void OnDestroy()
        {
            m_BackBtn.onClick.RemoveListener(m_LobbyManagerUI.GoBackButton);
            m_TestBtn.onClick.RemoveListener(OnTest);
        }

        private void OnTest()
        {
            Debug.Log("numPlayers:" + m_LobbyManager.numPlayers);
        }

        public void ShowBackBtn(bool show)
        {
            m_BackBtn.gameObject.SetActive(show);
        }

        /*void Update()
        {
            if (!isInGame)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleVisibility(!isDisplayed);
            }
        }*/

        public void StatusInfo(string status)
        {
            m_StatusInfoTxt.text = status;
        }

        public void HostInfo(string host)
        {
            m_HostInfoTxt.text = host;
        }

        public void ToggleVisibility(bool visible)
        {
            isDisplayed = visible;

            foreach (Transform t in transform)
                t.gameObject.SetActive(isDisplayed);

            if (panelImage != null)
                panelImage.enabled = isDisplayed;
        }
    }
}