/****************************************************
    文件：EntityBase.cs
	作者：WangZhen
    日期：2019/6/23 22:37:48
	功能：逻辑实体基类
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public abstract class EntityBase 
{
    public PlayerAniState _currentPlayerAniState = PlayerAniState.None;
    public BattleStateManager _stateMgr = null;
    public SkillManager _skillMgr = null;
    public bool _bCanControl = true;
    public BattleManager _battleMgr = null;
    public BattleProps BattleProps
    {
        get
        {
            return battleProps;
        }

        protected set
        {
            battleProps = value;
        }
    }
    public Queue<int> _comboQue = new Queue<int>();
    public int _nextSkillID = 0;
    public SkillDataCfg _skillCfg;
    public Constants.EntityType _entityType = Constants.EntityType.None;
    public bool _bCanReleaseSkill = true;
    public Constants.EntitiyState _entityState = Constants.EntitiyState.None;
    //技能位移回调ID
    public List<int> _listSkillMoveCallback = new List<int>();
    //技能的伤害计算回调ID
    public List<int> _listSkillDamageCallback = new List<int>();
    //技能结束的回调id
    public int _skillEndCallbackID = -1;

    protected NPCController _controller = null;

    private int hp;
    public int HP
    {
        get
        {
            return hp;
        }

        set
        {
            SetHpVal(hp, value);
            hp = value;
        }
    }

    private string entityName;

    public string EntityName
    {
        get
        {
            return entityName;
        }

        set
        {
            entityName = value;
        }
    }

    BattleProps battleProps;

    public virtual void SetBattleProps(BattleProps val)
    {
        battleProps = val;
        HP = val.hp;
    }

    public virtual void SetBlend(float blend)
    {
        if (_controller != null)
        {
            _controller.SetBlend(blend);
        }
    }

    public virtual void SetDirection(Vector2 dirVec)
    {
        if (_controller != null)
        {
            _controller.CurDirection = dirVec;
        }
    }

    public virtual void SetAction(int actVal)
    {
        if (_controller != null)
        {
            _controller.SetAction(actVal);
        }
    }

    /// <summary>
    /// 技能效果
    /// </summary>
    /// <param name="skillId"></param>
    public virtual void AttackEffect(int skillId)
    {
        _skillMgr.AttackEffect(this, skillId);
    }

    public virtual void AttackDamage(int skillId)
    {
        _skillMgr.AttackDamage(this, skillId);
    }

    public virtual void SetFx(string name, float expire)
    {
        if (_controller != null)
        {
            _controller.SetFx(name, expire);
        }
    }

    public virtual void SetSkillMoveState(bool bMove, float spd = 0.0f)
    {
        if (_controller != null)
        {
            _controller.SetSkillMoveState(bMove, spd);
        }
    }

    public virtual Vector2 GetCurrentDirection()
    {
        return Vector2.zero;
    }

    public virtual Vector3 GetEntityPosition()
    {
        return _controller.transform.position;
    }

    public virtual Transform GetEntityTransform()
    {
        return _controller.transform;
    }

    public CharacterController GetCharacterControl()
    {
        return _controller.GetComponent<CharacterController>();
    }

    public virtual void SetDodge()
    {
        if (_controller != null)
        {
            GameRoot.Instance._dynamicWnd.SetDodge(EntityName);
        }
    }

    public virtual void SetCritical(int val)
    {
        if (_controller != null)
        {
            GameRoot.Instance._dynamicWnd.SetCritical(EntityName, val);
        }
    }

    public virtual void SetHurt(int val)
    {
        if (_controller != null)
        {
            GameRoot.Instance._dynamicWnd.SetHurt(EntityName, val);
        }
    }

    public virtual void SetHpVal( int oldVal, int newVal)
    {
        if (_controller != null)
        {
            GameRoot.Instance._dynamicWnd.SetHpVal(EntityName, oldVal, newVal);
        }
    }


    public AnimationClip[] GetAnimationClips()
    {
        if (_controller != null)
        {
            return _controller.playerAni.runtimeAnimatorController.animationClips;
        }

        return null;
    }

    public void SetController(NPCController control)
    {
        _controller = control;
    }

    public void SetActive(bool bActive = true)
    {
        if (_controller != null)
        {
            _controller.gameObject.SetActive(bActive);
        }
    }

    public void CheckCombo()
    {
        _bCanControl = true;

        if (_skillCfg != null)
        {
            if (!_skillCfg.bBreak)
            {
                _entityState = Constants.EntitiyState.None;
            }

            if (_skillCfg.bCombo)
            {
                if (_comboQue.Count > 0)
                {
                    _nextSkillID = _comboQue.Dequeue();
                }
                else
                {
                    _nextSkillID = 0;
                }
            }

            //攻击完了要置空
            _skillCfg = null;
        }

        SetAction(Constants.cActionDefault);
    }

    public virtual Vector2 CalcTargetDirection()
    {
        return Vector2.zero;
    }

    public virtual void SetAttackDirection(Vector2 dir, bool bOffset = false)
    {
        if (_controller != null)
        {
            if (bOffset)
            {
                _controller.SetAttackCameraDirection(dir);
            }
            else
            {
                _controller.SetAttackLocalDirection(dir);
            }
        }
    }

    public void RemoveMoveCallbackID(int id)
    {
        int idx = -1;
        for (int i = 0; i < _listSkillMoveCallback.Count; i++)
        {
            if (_listSkillMoveCallback[i] == id)
            {
                idx = i;
                break;
            }
        }

        if (idx != -1)
        {
            _listSkillMoveCallback.RemoveAt(idx);
        }
    }

    public void RemoveDamageCallbackID(int id)
    {
        int idx = -1;
        for (int i = 0; i < _listSkillDamageCallback.Count; i++)
        {
            if (_listSkillDamageCallback[i] == id)
            {
                idx = i;
                break;
            }
        }

        if (idx != -1)
        {
            _listSkillDamageCallback.RemoveAt(idx);
        }
    }

    /// <summary>
    /// 能否被中断
    /// </summary>
    public virtual bool CanBeBreak()
    {
        return true;
    }

    public void RemoveSkillCallback()
    {
        //技能移动取消
        SetSkillMoveState(false);
        //方向归0
        SetDirection(Vector2.zero);

        for (int i = 0; i < _listSkillMoveCallback.Count; i++)
        {
            int id = _listSkillMoveCallback[i];
            TimerSvc.Instance.RemoveTask(id);
        }
        _listSkillMoveCallback.Clear();

        for (int i = 0; i < _listSkillDamageCallback.Count; i++)
        {
            int id = _listSkillDamageCallback[i];
            TimerSvc.Instance.RemoveTask(id);
        }
        _listSkillDamageCallback.Clear();

        //攻击被中断要删除技能的定时任务
        if (_skillEndCallbackID != -1)
        {
            TimerSvc.Instance.RemoveTask(_skillEndCallbackID);
            _skillEndCallbackID = -1;
        }

        //连招数据清空
        if (_nextSkillID != 0 || _comboQue.Count > 0)
        {
            _nextSkillID = 0;
            _comboQue.Clear();
            _battleMgr.LastAtkTime = 0;
            _battleMgr.ComboIdx = 0;
        }
    }

    #region AI

    public virtual void UpdateAILogic()
    {

    }

    public AudioSource GetAudioSource()
    {
        return _controller.GetComponent<AudioSource>();
    }
    #endregion


    #region 动作
    public void Idle()
    {
        _stateMgr.SwitchState(this, PlayerAniState.Idle, null);
    }

    public void Move()
    {
        _stateMgr.SwitchState(this, PlayerAniState.Move, null);
    }

    public void Attack(int skillId)
    {
        _stateMgr.SwitchState(this, PlayerAniState.Attack, skillId);
    }

    public void Born()
    {
        _stateMgr.SwitchState(this, PlayerAniState.Born, null);
    }

    public void Die()
    {
        _stateMgr.SwitchState(this, PlayerAniState.Die, null);
    }

    public void Hit()
    {
        _stateMgr.SwitchState(this, PlayerAniState.Hit, null);
    }
    #endregion
}