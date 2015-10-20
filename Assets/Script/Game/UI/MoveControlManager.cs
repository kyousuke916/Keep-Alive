using UnityEngine;
using UnityEngine.UI;
using System;

using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MoveControlManager : MonoBehaviour 
{
    /// <summary> 拖曳區域</summary>
    [SerializeField]
    private Image m_TouchZoneUI = null;

    /// <summary> 搖桿</summary>
    [SerializeField]
    private JoyStick m_JoyStickUI = null;

    public event Action<Vector2> OnMove;

    public float X = 0f;

    public float Y = 0f;

    void Awake()
    {
        m_JoyStickUI.OnStrickMove += MoveAction;
    }

    void Destroy()
    {
        m_JoyStickUI.OnStrickMove -= MoveAction;
    }

    public void MoveAction(float moveX,float moveY)
    {
        OnMove(new Vector2(moveX, moveY));
    }
}
