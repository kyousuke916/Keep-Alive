using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerManager
{
    //public string Key { get { return string.Format("{0}_{1}", Slot, ControllerId); } }
    public string Key { get { return RoleName; } }

    public bool IsLocalPlayer { get; private set; }
    public GameObject PlayerGo { get; private set; }
    public int Slot { get; private set; }
    public string RoleName { get; private set; }
    public Color RoleColor { get; private set; }

    public GamePlayer GamePlayer { get; private set; }
    public NetworkInstanceId NetID { get { return GamePlayer.netId; } }

    public PlayerManager(GameObject playerGo, int slot, string roleName, Color roleColor)
    {
        IsLocalPlayer = false;
        PlayerGo = playerGo;
        Slot = slot;
        RoleName = roleName;
        RoleColor = roleColor;

        GamePlayer = PlayerGo.GetComponent<GamePlayer>();
    }

    public void SetLocalPlayer()
    {
        IsLocalPlayer = true;
    }
}
