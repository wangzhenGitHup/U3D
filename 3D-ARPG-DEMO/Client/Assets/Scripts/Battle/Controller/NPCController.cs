/****************************************************
    文件：NPCController.cs
	作者：WangZhen
    日期：2019/3/23 22:56:42
	功能：角色实体控制基类
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public abstract class NPCController : MonoBehaviour 
{
    public Animator playerAni; /*角色动画*/
   // public GameObject _daggerSkill1fx;
    public CharacterController _playerCtrl; /*人物控制器*/
    public Transform hpRoot;

    protected bool _bSkillMove = false;
    protected float _sillMoveSpd = 0.0f;
    protected Dictionary<string, GameObject> _dictFX = new Dictionary<string, GameObject>();
    protected bool _bIsCurMove = false; /*当前角色是否在移动*/
    protected TimerSvc _timeSvc;
    protected Transform cameraTrans;

    private Vector2 curDirection = Vector2.zero; /*当前角色方向*/
    #region 属性

    public Vector2 CurDirection
    {
        get
        {
            return curDirection;
        }

        set
        {
            if (value == Vector2.zero)
            {
                _bIsCurMove = false;
            }
            else
            {
                curDirection = value;
                _bIsCurMove = true;
            }
        }
    }

    #endregion

    public virtual void Init()
    {
        _timeSvc = TimerSvc.Instance;
    }

    public virtual void SetBlend(float blend)
    {
        playerAni.SetFloat("Blend", blend);
    }

    public virtual void SetAction(int actVal)
    {
        playerAni.SetInteger("Action", actVal);
    }

    public virtual void SetFx(string name, float expire)
    {

    }

    public void SetSkillMoveState(bool bMove, float skillSpd = 0.0f)
    {
        _bSkillMove = bMove;
        _sillMoveSpd = skillSpd;
    }

    public virtual void SetAttackCameraDirection(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(dir, new Vector2(0, 1)) + cameraTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    public virtual void SetAttackLocalDirection(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(dir, new Vector2(0, 1));
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }
}