using UnityEngine;
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

        mTs.position = mTargetTs.position + (mTargetTs.rotation * m_Offset);
        //Debug.DrawLine(mTargetTs.position, mTs.position, Color.green);
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
