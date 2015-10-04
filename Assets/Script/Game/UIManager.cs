using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas m_UICanvas;
    public Canvas UICanvas { get { return m_UICanvas; } }

    [SerializeField]
    private RectTransform m_InfoRT;

    void Start()
    {
        UIChat.Create(m_InfoRT);
    }
}
