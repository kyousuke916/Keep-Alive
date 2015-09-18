using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GamePlayerSkill : NetworkBehaviour
{
    private GamePlayer mGamePlayer;

    public void Init(GamePlayer gamePlayer)
    {
        mGamePlayer = gamePlayer;
    }

    /// <summary>本機玩家角色呼叫使用技能</summary>
    [Client]
    public void UseSkill(int index)
    {
        CmdUseSkill(index);
    }

    /// <summary>Server 接收到該角色使用技能</summary>
    [Command][Server]
    private void CmdUseSkill(int index)
    {
        RpcUseSkill(index);
    }

    /// <summary>所有 Client 端該角色收到 Server 允許該角色使用技能</summary>
    [ClientRpc]
    private void RpcUseSkill(int index)
    {
        Debug.Log(string.Format("{0} 使用技能 {1}", name, index));

        //Test
        var mat = transform.FindChild("Role/RightHand/Sword").GetComponent<Renderer>().material;

        switch (index)
        {
            case 0:
                mat.color = Color.red;
                break;
            case 1:
                mat.color = Color.green;
                break;
            case 2:
                mat.color = Color.blue;
                break;
        }
    }
}
