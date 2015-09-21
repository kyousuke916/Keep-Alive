using UnityEngine;
using UnityEngine.UI;
using System;

using UnityEngine.Events;
using UnityEngine.EventSystems;


public class HitControlManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private Button m_HitBtnA = null;

    [SerializeField]
    private Button m_HitBtnB = null;

    [SerializeField]
    private Button m_HitBtnC = null;

    [SerializeField]
    private Button m_DragZone = null;

    public Action<int> OnHitAction;

    public Action<Vector2> OnDragAction;

	// Use this for initialization
	void Start () 
    {
        m_HitBtnA.onClick.AddListener(() => OnHitAction(0));
        m_HitBtnB.onClick.AddListener(() => OnHitAction(1));
        m_HitBtnC.onClick.AddListener(() => OnHitAction(2));
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragAction(eventData.delta.normalized);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragAction(Vector2.zero); 
    }
}
