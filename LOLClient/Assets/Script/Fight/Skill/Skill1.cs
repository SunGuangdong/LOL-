using UnityEngine;
using System.Collections;
using GameProtocol;
using GameProtocol.dto.fight;

public class Skill1 :TryAgainSkill {

    void OnCollisionEnter(Collision c)
    {
        int target;
        if (c.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            target = c.gameObject.GetComponent<PlayerCon>().data.id;
        }
        else {
            return;
        }
        DamageDTO dto=new DamageDTO();
        dto.userId=GameData.user.id;
        dto.skill = 1;
        dto.target =new int[][]{new int[]{ target}};
        this.WriteMessage(Protocol.TYPE_FIGHT, 0, FightProtocol.DAMAGE_CREQ, dto);
    }
}
