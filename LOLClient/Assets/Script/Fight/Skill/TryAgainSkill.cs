using UnityEngine;
using System.Collections;
/// <summary>
/// 方向性的  过去了还要回来的技能
/// </summary>
public class TryAgainSkill : MonoBehaviour {
    public float maxDis;
    public float speed;
    private float dis=0;
    private int state = 0;//0 向前飞状态 1 往回飞状态
    protected PlayerCon actor;//技能释放者对象
    public void init(PlayerCon actor, float speed, float maxDis)
    {
        this.actor = actor;
        this.maxDis = maxDis;
        this.speed = speed;
    }

   void FixedUpdate()
   {
       if (state == 0)
       {
           Vector3 t = Vector3.forward * speed * Time.fixedDeltaTime;
           transform.Translate(t);
           dis += t.z;
           if (dis >= maxDis) state = 1;
       }
       else {
           transform.position = Vector3.Lerp(transform.position, actor.transform.position, 0.5f);
           if (Vector3.Distance(transform.position, actor.transform.position) < 0.1f) {
               Destroy(gameObject);
           }
       }
    }

}
