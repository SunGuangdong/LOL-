using UnityEngine;
using System.Collections;
using GameProtocol;
using GameProtocol.dto.fight;
using UnityEngine.UI;
using GameProtocol.constans;
using System.Collections.Generic;
public class FightScene : MonoBehaviour {
    public static FightScene instance;
    [SerializeField]
    /// <summary>
    /// 个人头像
    /// </summary>
    /// 
    private Image head;
    [SerializeField]
    /// <summary>
    /// 自身血条
    /// </summary>
    private Slider hpSlider;
    [SerializeField]
    /// <summary>
    /// 角色名
    /// </summary>
    private Text nameText;
    [SerializeField]
    /// <summary>
    /// 技能Icon
    /// </summary>
    private SkillGrid[] skills;
    [SerializeField]
    private Text levelText;

    public int skill = -1;//当前鼠标左键点击释放的技能ID

    public PlayerCon myHero;
    /// <summary>
    /// 当前自己的英雄是否死亡
    /// </summary>
    public bool dead = false;
    [SerializeField]
    private Transform NumParent;//掉血数字父容器

    private Camera mainCamera;
	void Start () {
        instance = this;
        mainCamera = Camera.main;
        this.WriteMessage(Protocol.TYPE_FIGHT, 0, FightProtocol.ENTER_CREQ, null);
	}


    public void NumUp(Transform p,string value) { 
            GameObject hp= (GameObject)Instantiate( Resources.Load("prefab/Hp"));
            hp.GetComponent<Text>().text = value;
            hp.transform.parent = NumParent;
            hp.transform.localScale = Vector3.one;
            hp.transform.localPosition = Camera.main.WorldToScreenPoint(p.position)+new Vector3(30,20);
    }

    public void initView(FightPlayerModel model,PlayerCon hero) {
        myHero = hero;
        head.sprite = Resources.Load<Sprite>("HeroIcon/"+model.code);
        hpSlider.value = model.hp / model.maxHp;
        nameText.text = HeroData.heroMap[model.code].name;
        levelText.text = model.level.ToString();
        int i = 0;
        foreach (FightSkill item in model.skills)
	    {
            skills[i].init(item);
            i++;
	    }             
    }

    public void refreshView(FightPlayerModel model)
    {
        hpSlider.value = model.hp / model.maxHp;
        levelText.text = model.level.ToString();
    }

    public void RefreshLevelUp() {
        int i = 0;
        foreach (FightSkill item in myHero.data.skills)
        {
            if (item.nextLevel <= myHero.data.level)
            {
                if (myHero.data.free > 0)
                {
                    skills[i].setBtnState(true);
                }
                else
                {
                    skills[i].setBtnState(false);
                }
            }
            else {
                skills[i].setBtnState(false);
            }
            skills[i].SkillChange(item);
            skills[i].setMask(0);
            i++;
        }
    }

    public void lookAt() {
        mainCamera.transform.position = myHero.transform.position + new Vector3(-6, 8, 0);
    }

    private int cameraH;
    private int cameraV;

    public float cameraSpeed = 1f;
    /// <summary>
    /// 相机横移 向右传-1 向左传1
    /// </summary>
    public void cameraHMove(int dir)
    {
        if(cameraH!=dir)
        cameraH = dir;
    }
    /// <summary>
    /// 相机纵移 向上传1 向下传-1
    /// </summary>
    public void cameraVMove(int dir)
    {
        if (cameraV != dir)
        cameraV = dir;
    }
    //x最大 150 最小0  z最小40 最大160
    void Update() {
        switch (cameraH) { 
            case 1:
                if (mainCamera.transform.position.z < 150) {
                   // mainCamera.transform.Translate(Vector3.forward*Time.deltaTime,Space.Self);
                    mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z + cameraH);
                }
                break;
            case -1:
                if (mainCamera.transform.position.z > 0)
                {
                 //   mainCamera.transform.Translate(Vector3.back * Time.deltaTime, Space.Self);
                    mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z + cameraH);
                }
                break;
        }
        switch (cameraV)
        {
            case 1:
                if (mainCamera.transform.position.x < 160)
                {
                 //   mainCamera.transform.Translate(Vector3.right * Time.deltaTime, Space.Self);
                    mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + cameraV, mainCamera.transform.position.y, mainCamera.transform.position.z);
                }
                break;
            case -1:
                if (mainCamera.transform.position.x >40)
                {
                 //   mainCamera.transform.Translate(Vector3.left * Time.deltaTime, Space.Self);
                    mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + cameraV, mainCamera.transform.position.y, mainCamera.transform.position.z);
                }
                break;
        }
    }

    public void leftClick(Vector2 position) {
        if (skill == -1) return;
         Ray ray = mainCamera.ScreenPointToRay(position);
        RaycastHit[] hits = Physics.RaycastAll(ray, 200);
        List<Transform> list = new List<Transform>();
        Vector3 tigger = Vector3.zero;
        for (int i = hits.Length - 1; i >= 0; i--)
        {
            RaycastHit item = hits[i];
            
           
            if (item.transform.gameObject.layer == LayerMask.NameToLayer("Water")) {
                tigger = item.point;
            }
            list.Add(item.transform);
        }
       
        myHero.baseSkill(skill, list.ToArray(), tigger);
        skill = -1;
    }

    public void rightClick(Vector2 position) {
        Ray ray = mainCamera.ScreenPointToRay(position);
        RaycastHit[] hits = Physics.RaycastAll(ray, 200);
        for (int i = hits.Length-1; i >= 0; i--)
        {
            RaycastHit item = hits[i];
        
            //如果是敌方单位 则进行普通攻击
            if (item.transform.gameObject.layer == LayerMask.NameToLayer("enemy")) {
               PlayerCon con= item.transform.GetComponent<PlayerCon>();
               if (Vector3.Distance(myHero.transform.position, item.transform.position) < con.data.aRange)
               {
                   //记住扩展 如果需要实现分裂攻击 消息体改为 范围内敌人ID数组
                   this.WriteMessage(Protocol.TYPE_FIGHT, 0, FightProtocol.ATTACK_CREQ, con.data.id);
                   return;
               }
               continue;
            }
            //己方单位无视
            //如果是地板层 则开始寻路
            if (item.transform.gameObject.layer == LayerMask.NameToLayer("Water")) {
                MoveDTO dto = new MoveDTO();
                dto.x = item.point.x;
                dto.y = item.point.y;
                dto.z = item.point.z;
                this.WriteMessage(Protocol.TYPE_FIGHT, 0, FightProtocol.MOVE_CREQ, dto);
                return;
                
            }
        }
    }

    public void SkillMask(int code) {
        foreach (SkillGrid item in skills)
        {
            if (item.skill.code == code) {
                item.setMask(item.skill.time);
            }
        }
    }
}
