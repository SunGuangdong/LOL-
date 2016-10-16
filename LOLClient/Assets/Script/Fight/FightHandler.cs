using UnityEngine;
using System.Collections;
using GameProtocol;
using GameProtocol.dto.fight;
using System.Collections.Generic;
using UnityEngine.UI;

public class FightHandler : MonoBehaviour,IHandler {
    FightRoomModel room;
    [SerializeField]
    private Transform[] positions;//队伍1的建筑初始位置表
    [SerializeField]
    private Transform startPosition;//队伍1的复活（出生）点
    [SerializeField]
    private Transform startPosition1;//队伍2的复活（出生）点

    [SerializeField]
    private Transform[] positions1;//队伍2的建筑初始位置表

    private Dictionary<int, PlayerCon> models = new Dictionary<int, PlayerCon>();
   // private Dictionary<int, GameObject> teamTwo = new Dictionary<int, GameObject>();

    public void MessageReceive(SocketModel model)
    {
        switch (model.command) { 
            case FightProtocol.START_BRO:
                start(model.GetMessage<FightRoomModel>());
                break;
            case FightProtocol.MOVE_BRO:
                move(model.GetMessage<MoveDTO>());
                break;
            case FightProtocol.ATTACK_BRO:
                atk(model.GetMessage<AttackDTO>());
                break;
            case FightProtocol.DAMAGE_BRO:
                damage(model.GetMessage<DamageDTO>());
                break;
            case FightProtocol.SKILL_UP_SRES:
                skillLevelUp(model.GetMessage<FightSkill>());
                break;
            case FightProtocol.SKILL_BRO:
                skill(model.GetMessage<SkillAtkModel>());
                break;

        }
    }

    private void skill(SkillAtkModel model) {
        List<Transform> list=new List<Transform>();
        if (model.type == 0)
        {
            list.Add(models[model.target].transform);
        }
        models[model.userId].skill(model.skill, list.ToArray(), new Vector3(model.position[0], model.position[1], model.position[2]));
        if (model.userId == GameData.user.id) {
            FightScene.instance.SkillMask(model.skill);
        }
    }

    private void skillLevelUp(FightSkill skill) {
        for (int i = 0; i < FightScene.instance.myHero.data.skills.Length; i++) {
            if (FightScene.instance.myHero.data.skills[i].code == skill.code) {
                FightScene.instance.myHero.data.free -= 1;
                FightScene.instance.myHero.data.skills[i] = skill;
                FightScene.instance.RefreshLevelUp();
                return;
            }
        }
    }

    private void damage(DamageDTO value) {
        foreach (int[] item in value.target)
	    {
            PlayerCon pc= models[item[0]];
            pc.data.hp -= item[1];
            //实例化掉血数字
            FightScene.instance.NumUp(pc.transform,item[1].ToString());
            pc.HpChange();
            if (pc.data.id == GameData.user.id) {
                FightScene.instance.refreshView(pc.data);
            }
            if (item[2] == 0) {
                if (item[0] >= 0)
                {
                    pc.gameObject.SetActive(false);
                    if (pc.data.id == GameData.user.id)
                    {
                        FightScene.instance.dead = true;
                    }
                }
                else {
                    Destroy(pc.gameObject);
                }
            }
	    } 
    }

    private void atk(AttackDTO dto) {
        PlayerCon obj = models[dto.userId];
        PlayerCon target = models[dto.targetId];
        obj.attack(new Transform[] { target.transform });
    }

    private void move(MoveDTO value) {
        Vector3 target = new Vector3(value.x, value.y, value.z);
        models[value.userId].SendMessage("move",target);
    }

    private void start(FightRoomModel value){
        room = value;

        int myTeam=-1;
        foreach (AbsFightModel item in value.teamOne)
        {
            if (item.id == GameData.user.id) {
                myTeam = item.team;
            }
        }
        if (myTeam == -1) {
            foreach (AbsFightModel item in value.teamTwo)
            {
                if (item.id == GameData.user.id)
                {
                    myTeam = item.team;
                }
            }
        }


        foreach (AbsFightModel item in value.teamOne)
        {
            GameObject go;
            PlayerCon pc;
            if (item.type == ModelType.HUMAN)
            {
                go = (GameObject)Instantiate(Resources.Load<GameObject>("prefab/Player/" + item.code), startPosition.position + new Vector3(Random.Range(0.5f, 1.5f), 0, Random.Range(0.5f, 1.5f)), startPosition.rotation);
                pc = go.GetComponent<PlayerCon>();
                pc.init((FightPlayerModel)item,myTeam);
            }
            else {
                go = (GameObject)Instantiate(Resources.Load<GameObject>("prefab/build/1_" + item.code), positions[item.code - 1].position, positions[item.code - 1].rotation);
                pc = go.GetComponent<PlayerCon>();
            }
            
            this.models.Add(item.id, pc);
            if (item.id == GameData.user.id) {
                FightScene.instance.initView((FightPlayerModel)item, pc);
                FightScene.instance.lookAt();
            }
        }

        foreach (AbsFightModel item in value.teamTwo)
        {
            GameObject go;
            PlayerCon pc;
            if (item.type == ModelType.HUMAN)
            {
                go = (GameObject)Instantiate(Resources.Load<GameObject>("prefab/Player/" + item.code), startPosition1.position + new Vector3(Random.Range(5, 15), 0, Random.Range(5, 15)), startPosition1.rotation);
                pc = go.GetComponent<PlayerCon>();
                pc.init((FightPlayerModel)item,myTeam);
            }
            else
            {
                go = (GameObject)Instantiate(Resources.Load<GameObject>("prefab/build/2_" + item.code), positions1[item.code - 1].position, positions1[item.code - 1].rotation);
                pc = go.GetComponent<PlayerCon>();
            }
            
            this.models.Add(item.id, pc);
            if (item.id == GameData.user.id)
            {
                FightScene.instance.initView((FightPlayerModel)item,pc);
                FightScene.instance.lookAt();
            }
        }
    }
}
