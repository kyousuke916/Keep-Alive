using UnityEngine;
using System.Collections;

public class RoleFixedPoint : MonoBehaviour
{
    [SerializeField]
    private Transform m_EyeTs;
    public Transform EyeTs { get { return m_EyeTs; } }

    [SerializeField]
    private Transform m_BodyTs;
    public Transform BodyTs { get { return m_BodyTs; } }

    [SerializeField]
    private Transform m_LeftHandTs;
    public Transform LeftHandTs { get { return m_LeftHandTs; } }

    [SerializeField]
    private Transform m_RightHandTs;
    public Transform RightHandTs { get { return m_RightHandTs; } }
}
