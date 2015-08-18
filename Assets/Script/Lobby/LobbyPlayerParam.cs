using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LobbyPlayerParam : NetworkBehaviour
{
    private static Color[] Colors = new Color[] { Color.red, Color.magenta, Color.cyan, Color.blue, Color.green, Color.yellow };

    [SerializeField]
    private Image m_RoleImage;

    [SerializeField]
    private InputField m_NameInput;


    public void Refresh()
    {
        m_RoleImage.color = mRoleColor;
        m_NameInput.text = mRoleName;
    }

    #region Role Color

    [SyncVar(hook = "OnRoleColor")]
    private Color mRoleColor = Color.white;
    public Color RoleColor { get { return mRoleColor; } }

    public void ChangeColor()
    {
        int idx = System.Array.IndexOf(Colors, mRoleColor);
        if (idx < 0) idx = 0;
        idx = (idx + 1) % Colors.Length;

        CmdColorChange(idx);
    }

    [Command]
    public void CmdColorChange(int idx)
    {
        mRoleColor = Colors[idx];
    }

    private void OnRoleColor(Color color)
    {
        m_RoleImage.color = mRoleColor = color;
    }

    #endregion

    #region Role Name

    [SyncVar(hook = "OnRoleName")]
    private string mRoleName = "";
    public string RoleName { get { return mRoleName; } }

    public void ChangeName(string id)
    {
        CmdNameChanged(id);
    }

    [Command]
    public void CmdNameChanged(string id)
    {
        mRoleName = id;
    }

    private void OnRoleName(string id)
    {
        m_NameInput.text = mRoleName = id;
    }

    #endregion

}
