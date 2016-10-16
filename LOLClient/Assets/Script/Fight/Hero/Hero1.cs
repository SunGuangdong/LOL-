using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameProtocol;
using GameProtocol.dto.fight;

public class Hero1:PlayerCon {
    private Transform[] list;

    [SerializeField]
    private GameObject effect;//攻击粒子

    public override void attack(Transform[] target)
    {
        this.list = target;
        if (state == AnimState.RUN) {
            agent.CompleteOffMeshLink();
        }
        transform.LookAt(target[0]);
        state = AnimState.ATTACK;
        anim.SetInteger("state", AnimState.ATTACK);
    }

    public override void attacked()
    {
        //如果是近战 直接向服务器发送伤害消息
       // 如果是远程则进行下面的循环发送粒子
        foreach (Transform item in list)
        {
           GameObject go=(GameObject)Instantiate(effect, transform.position + transform.up*2, transform.rotation);
            //让粒子向敌人位移
           go.GetComponent<TargetSkill>().init(list[0], -1, data.id);
           state = AnimState.IDLE;
           anim.SetInteger("state", AnimState.IDLE);
        }
    }

    public override void skilled()
    {
        state = AnimState.IDLE;
        anim.SetInteger("state", AnimState.IDLE);
    }

    public override void skill(int code, Transform[] target, Vector3 ps)
    {
        if (state == AnimState.RUN)
        {
            agent.CompleteOffMeshLink();
        }
        

        switch (code) { 
            case 1:
                transform.LookAt(ps);
                GameObject go = (GameObject)Instantiate(Resources.Load<GameObject>("prefab/Skill/skill1"), transform.position + transform.up * 2, transform.rotation);
                  go.GetComponent<Skill1>().init(this, 2, 20);
                break;
            case 2:
                //如果是范围攻击  则根据运算方法 获取指定范围内的敌方单位 并给粒子特效（这个范围是指 释放多个技能特效给不同的角色的情况）
                //只有单一的一个技能特效 而在这个特效范围内产生的伤害 则由技能特效自行处理
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                return;
        }

        state = AnimState.SKILL;
        anim.SetInteger("state", AnimState.SKILL);
    }


    public override void baseSkill(int code, Transform[] target,Vector3 ps)
    {
        SkillAtkModel atk=new SkillAtkModel();
        switch (code)
        {
            case 1:                
                atk.skill = code;
                atk.position = new float[] { ps.x, ps.y, ps.z };
                atk.type = 1;
                this.WriteMessage(Protocol.TYPE_FIGHT, 0, FightProtocol.SKILL_CREQ, atk);
                break;
            case 2:
               
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                return;
        }

    }
}
