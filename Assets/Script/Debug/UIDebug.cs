using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System;

public class UIDebug : MonoBehaviour
{
    public static UIDebug Instance { get; private set; }

    [SerializeField]
    private Canvas m_UIRootCanvas;
    private RectTransform UICanvasRT { get { return m_UIRootCanvas.transform as RectTransform; } }

    [SerializeField]
    private Canvas m_UIScrollViewCanvas;

    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private Button m_SwitchBtn;

    [SerializeField]
    private Text m_SwitchBtnText;

    private List<Item> mItemList = new List<Item>();
    private StringBuilder mSB = new StringBuilder();

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        CheckWithCreateEventSystem();

        UpdateBtnText();

        m_SwitchBtn.onClick.AddListener(OnSwitch);
    }

    void OnDestroy()
    {
        Instance = null;
    }

    private void OnSwitch()
    {
        if (m_UIScrollViewCanvas.enabled)
            Hide();
        else
            Show();
    }

    private void Show()
    {
        m_UIScrollViewCanvas.enabled = true;
    }

    private void Hide()
    {
        m_UIScrollViewCanvas.enabled = false;
    }

    private void CheckWithCreateEventSystem()
    {
        var eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
            go.AddComponent<TouchInputModule>();

            go.transform.SetParent(UICanvasRT);
        }
    }

    public static void Log(string format, params object[] args)
    {
        if (Instance == null)
            return;

        Instance.LogFormat(format, args);
    }

    private void LogFormat(string format, params object[] args)
    {
        var item = new Item
        {
            Msg = string.Format(format, args)
        };

        mItemList.Insert(0, item);

        UpdateLog();
    }

    private void UpdateLog()
    {
        mSB.Length = 0;

        for (int i = 0; i < mItemList.Count; i++)
        {
            var item = mItemList[i];

            mSB.Append(string.Format("[{0}] {1}", item.DateTime.ToString("HH:mm:ss.ffff"), item.Msg));
            mSB.AppendLine();
        }

        m_Text.text = mSB.ToString();

        UpdateBtnText();
    }

    private void UpdateBtnText()
    {
        m_SwitchBtnText.text = string.Format("除錯[{0}]", mItemList.Count);
    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIDebug.Log("Test:{0}", Time.time);
        }
    }*/

    public class Item
    {
        public DateTime DateTime = DateTime.Now;
        public string Msg = string.Empty;
    }
}
