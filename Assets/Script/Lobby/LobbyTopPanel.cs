using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets.Network
{
    public class LobbyTopPanel : MonoBehaviour
    {
        [SerializeField]
        private Text m_StatusInfoTxt;

        [SerializeField]
        private Text m_HostInfoTxt;

        [SerializeField]
        private Button m_BackBtn;

        public bool isInGame = false;

        private bool isDisplayed = true;
        private Image panelImage;

        void Awake()
        {
            panelImage = GetComponent<Image>();

            m_BackBtn.onClick.AddListener(OnBack);
        }

        void OnDestroy()
        {
            m_BackBtn.onClick.RemoveListener(OnBack);
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

        private void OnBack()
        {

        }

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
            {
                t.gameObject.SetActive(isDisplayed);
            }

            if (panelImage != null)
            {
                panelImage.enabled = isDisplayed;
            }
        }
    }
}