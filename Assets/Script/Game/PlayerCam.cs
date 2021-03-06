﻿using UnityEngine;
using System.Collections;

public class PlayerCam : MonoBehaviour
{
    private static PlayerCam mInstance;
    public static PlayerCam Instance
    {
        get
        {
            if (mInstance == null) mInstance = GameObject.FindGameObjectWithTag(TagManager.MAIN_CAMERA).AddComponent<PlayerCam>();
            return mInstance;
        }
    }

    [SerializeField]
    private Vector3 m_Offset = new Vector3(3f, 1.5f, -6f);

    [SerializeField]
    private float m_RaiseMax = 40f;

    [SerializeField]
    private float m_RaiseMin = -20f;

    private float mAngleX = 0f;
    public float AngleX { get { return mAngleX; } }

    private Transform mTs;
    private Transform mTargetTs;

    private float mX;

    private float mY;

    public float x;

    public float d = 8f;

    public float y;

    public float xSpeed = 50;

    public float ySpeed = 1;

    public Quaternion rotationEuler;

    private Vector3 cameraPosition;

    void Awake()
    {
        mInstance = this;
        mTs = transform;
    }

    void OnDestroy()
    {
        mInstance = null;
    }

    void LateUpdate()
    {
        UpdatePos();
    }

    public void SetTarget(Transform ts)
    {
        mTargetTs = ts;

        SetAngleX(0f);

        UpdatePos();
    }

    private void UpdatePos()
    {
        if (mTargetTs == null)
            return;

        x += mX * xSpeed * Time.deltaTime;
        //y -= mY * ySpeed * Time.deltaTime;

        if (x > 360)
        {
            x -= 360;
        }
        else if (x < 0)
        {
            x += 360;
        }

        rotationEuler = Quaternion.Euler(10f, x, 0);

        cameraPosition = rotationEuler * new Vector3(0, 0, -d) + mTargetTs.position;

        transform.LookAt(mTargetTs);

        transform.rotation = rotationEuler;

        transform.position = cameraPosition;

        //mTs.position = mTargetTs.position + (mTargetTs.rotation * m_Offset);
        //Debug.DrawLine(mTargetTs.position, mTs.position, Color.green);
    }

    public void MoveCamera(float x, float y)
    {
        if (mTargetTs == null)
            return;

        mX = x;
        mY = y;
    }

    public void SetAngleX(float angle)
    {
        if (mTargetTs == null)
            return;

        mAngleX = Mathf.Clamp(mAngleX - angle, m_RaiseMin, m_RaiseMax);

        var eulerAngles = mTargetTs.rotation.eulerAngles;

        eulerAngles.x = mAngleX;

        mTs.rotation = Quaternion.Euler(eulerAngles);

        /*
        if (mTs.transform.position.y >= 1f && mTs.transform.position.y <= 5f)
        {
            mTs.transform.position += new Vector3(0f, angle / 10f, 0f);
        }

        if (mTs.transform.position.y < 1f)
        {
            mTs.transform.position = new Vector3(mTs.transform.position.x, 1f, mTs.transform.position.z);
        }

        if (mTs.transform.position.y > 5f)
        {
            mTs.transform.position = new Vector3(mTs.transform.position.x, 5f, mTs.transform.position.z);
        }
        */
    }
}
