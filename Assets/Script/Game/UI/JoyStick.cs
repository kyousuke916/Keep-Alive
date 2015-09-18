using UnityEngine;
using UnityEngine.UI;
using System;

using UnityEngine.Events;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour ,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [System.Serializable]
    public class VirtualJoystickEvent : UnityEvent<Vector3> { }

    /// <summary> 搖桿UI</summary>
    [SerializeField]
    private RectTransform m_JoyStickUI = null;

    [SerializeField]
    private ScrollRect m_MoveRect = null;

    /// <summary> 蘑菇頭</summary>
    [SerializeField]
    private RectTransform m_StickRect = null;

    [SerializeField]
    private Button m_StickBtn = null;

    /// <summary> 蘑菇頭位移限制</summary>
    private float mDragRange;

    public event Action<float, float> OnStrickMove;

    public VirtualJoystickEvent Controlling;

    void Awake()
    {
        mDragRange = m_JoyStickUI.sizeDelta.x / 2f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        m_StickRect.localPosition = Vector3.ClampMagnitude(m_StickRect.localPosition, mDragRange);

        Debug.Log(string.Format("{0}/{1}", m_MoveRect.velocity.x, m_MoveRect.velocity.y));

        this.Controlling.Invoke(m_StickRect.localPosition.normalized);

        OnStrickMove(m_StickRect.localPosition.normalized.x, m_StickRect.localPosition.normalized.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnStrickMove(0f, 0f);
    }
}
