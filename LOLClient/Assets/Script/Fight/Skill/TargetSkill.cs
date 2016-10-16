using UnityEngine;
using System.Collections;
using GameProtocol.dto.fight;
using GameProtocol;

public class TargetSkill : MonoBehaviour {
    Transform target;
    int skill;
    int userId;
    public void init(Transform target,int skill,int userId) {
        this.skill = skill;
        this.target = target;
        this.userId = userId;
    }

    void Update() {
        if (target) {
            transform.position = Vector3.Lerp(transform.position, target.position,0.5f);
            if(Vector3.Distance(transform.position,target.position)<0.1f){
                //向服务器发送 伤害目标                
                    DamageDTO dto = new DamageDTO();
                    dto.userId = userId;
                    dto.skill = skill;
                    dto.target = new int[][] {new int[]{ target.GetComponent<PlayerCon>().data.id} };
                    this.WriteMessage(Protocol.TYPE_FIGHT, 0, FightProtocol.DAMAGE_CREQ, dto);
                Destroy(gameObject);
            }
        }
    }
}
