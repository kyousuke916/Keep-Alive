using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System;
using UnityEngine.Events;

public class HitControlManager : MonoBehaviour 
{
    [SerializeField]
    private Button m_HitBtnA = null;

    [SerializeField]
    private Button m_HitBtnB = null;

    [SerializeField]
    private Button m_HitBtnC = null;

    public Action<int> OnHitAction;

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
}
