﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GamePlayer : NetworkBehaviour
{
    public RoleFixedPoint RoleFixedPoint { get; private set; }

    private GamePlayerSkill mPlayerSkill;

    private PlayerCam mPlayerCam;

    private PlayerController mPlayerController;

    void Awake()
    {
        enabled = false;

        InitComponent();
    }

    private void InitComponent()
    {
        RoleFixedPoint = GetComponentInChildren<RoleFixedPoint>();

        gameObject.AddComponent<Player_SyncPosition>();
        gameObject.AddComponent<Player_SyncRotation>();
        gameObject.AddComponent<GamePlayerSkill>();

        mPlayerSkill = GetComponent<GamePlayerSkill>();
        mPlayerSkill.Init(this);

        mPlayerController = GetComponent<PlayerController>();
    }

    public override void OnStartLocalPlayer()
    {
        enabled = isLocalPlayer;
        
        Debug.LogFormat(name + " : " + isLocalPlayer);

        if (isLocalPlayer)
        {
			mPlayerCam = PlayerCam.Instance;
			mPlayerCam.SetTarget(RoleFixedPoint.BodyTs);

			UIManager.Instance.SetLocalPlayer(mPlayerController, mPlayerCam);
			/*
            mPlayerController.uiSet();
            mPlayerController.Listencontroll = true;
            */

            //GameManager.SetLocalPlayer(gameObject, netId);

            //GameManager.SetLocalParam(netId);
        }
    }

    public void SetCameraAngleX(float angle)
    {
        mPlayerCam.SetAngleX(angle);
    }

    /// <summary>使用指定技能索引</summary>
    public void UseSkill(int index)
    {
        mPlayerSkill.UseSkill(index);
    }
}
