/****************************************************
    文件：PlayerController.cs
	作者：WangZhen
    日期：2019/6/15 22:19:41
	功能：角色控制器
*****************************************************/

using UnityEngine;

public class PlayerController : NPCController 
{
    //public CharacterController _playerCtrl; /*人物控制器*/
    public GameObject _daggerSkill1fx;
    public GameObject _daggerSkill2fx;
    public GameObject _daggerSkill3fx;

    public GameObject _daggerNormal1fx;
    public GameObject _daggerNormal2fx;
    public GameObject _daggerNormal3fx;
    public GameObject _daggerNormal4fx;
    public GameObject _daggerNormal5fx;

    
    private Vector3 cameraOffset; /*初始摄像机偏移量*/
    private float targetBlend; /*目标混合参数*/
    private float curBlend; /*当前混合参数*/

    public override void Init()
    {
        base.Init();

        cameraTrans = Camera.main.transform;
        cameraOffset = transform.position - cameraTrans.position;
        InitSkill();
    }

    private void InitSkill()
    {
        if (_daggerSkill1fx != null)
        {
            _dictFX.Add(_daggerSkill1fx.name, _daggerSkill1fx);
        }

        if (_daggerSkill2fx != null)
        {
            _dictFX.Add(_daggerSkill2fx.name, _daggerSkill2fx);
        }

        if (_daggerSkill3fx != null)
        {
            _dictFX.Add(_daggerSkill3fx.name, _daggerSkill3fx);
        }

        if (_daggerNormal1fx != null)
        {
            _dictFX.Add(_daggerNormal1fx.name, _daggerNormal1fx);
        }

        if (_daggerNormal2fx != null)
        {
            _dictFX.Add(_daggerNormal2fx.name, _daggerNormal2fx);
        }

        if (_daggerNormal3fx != null)
        {
            _dictFX.Add(_daggerNormal3fx.name, _daggerNormal3fx);
        }

        if (_daggerNormal4fx != null)
        {
            _dictFX.Add(_daggerNormal4fx.name, _daggerNormal4fx);
        }

        if (_daggerNormal5fx != null)
        {
            _dictFX.Add(_daggerNormal5fx.name, _daggerNormal5fx);
        }
    }

    private void Update()
    {
        if (curBlend != targetBlend)
        {
            UpdateMixBlend();
        }

        if (_bIsCurMove)
        {
            //控制方向
            SetCurDirection();
            //产生移动
            SetMove();
            //相机的跟随
            SetCameraFollow();
        }

        if (_bSkillMove)
        {
            SetSkillMove();
            //相机的跟随
            SetCameraFollow();
        }
    }

    /// <summary>
    /// 设置当前玩家方向
    /// </summary>
    private void SetCurDirection()
    {
        float angle = Vector2.SignedAngle(CurDirection, new Vector2(0, 1)) + cameraTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    /// <summary>
    /// 设置移动
    /// </summary>
    private void SetMove()
    {
        _playerCtrl.Move(transform.forward * Time.deltaTime * Constants.cPlayerMoveSpeed);
    }

    private void SetSkillMove()
    {
        _playerCtrl.Move(transform.forward * Time.deltaTime * _sillMoveSpd);
    }
    /// <summary>
    /// 相机跟随
    /// </summary>
    public void SetCameraFollow()
    {
        if (cameraTrans != null)
        {
            
            cameraTrans.position = transform.position - cameraOffset;
        }
    }

    /// <summary>
    /// 动画混合参数
    /// </summary>
    /// <param name="blend"></param>
    public override void SetBlend(float blend)
    {
        targetBlend = blend;
    }

    /// <summary>
    /// 更新动画混合
    /// </summary>
    private void UpdateMixBlend()
    {
        if (Mathf.Abs(curBlend - targetBlend) < Constants.cMoveLerpSpd * Time.deltaTime)
        {
            curBlend = targetBlend;
        }
        else if (curBlend > targetBlend)
        {
            curBlend -= Constants.cMoveLerpSpd * Time.deltaTime;
        }
        else
        {
            curBlend += Constants.cMoveLerpSpd * Time.deltaTime;
        }
        playerAni.SetFloat("Blend", curBlend);
    }

    public override void SetFx(string name, float expire)
    {
        GameObject objFx;
        if (_dictFX.TryGetValue(name, out objFx))
        {
            objFx.SetActive(true);
            _timeSvc.AddTimeTask((int tid) =>
            {
                objFx.SetActive(false);
            }, expire);
        }
    }
}