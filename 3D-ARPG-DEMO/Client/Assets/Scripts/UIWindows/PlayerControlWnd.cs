/****************************************************
    文件：PlayerControlWnd.cs
	作者：WangZhen
    日期：2019/6/23 22:6:57
	功能：玩家控制界面
*****************************************************/

using UnityEngine;
using Protocols;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerControlWnd : WindowRoot
{
    #region UI变量

    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpProgress;
    public Transform expProgressTrans;
    public Image imgSkill1CD;
    public Text txtSkill1CD;
    public Image imgSkill2CD;
    public Text txtSkill2CD;
    public Image imgSkill3CD;
    public Text txtSkill3CD;
    public Text txtHpBar;
    public Image imgHpBar;

    public Transform transBossHpBar;
    public Image imgBossRed;
    public Image imgBossYellow;
    public Text txtBossLvName;

    #endregion

    private Vector2 vecStartPos = Vector2.zero;
    private Vector2 vecDefaultPos = Vector2.zero;
    private float fOperatorDistance;
    private GridLayoutGroup gridExpBg;
    private Vector2 currentDirection;
    private bool bSkill1InCD = false;
    private float fSkill1CDTime;
    private int skill1Num;
    private float skill1FillCount = 0;
    private float skill1AccumulateTime = 0;

    private bool bSkill2InCD = false;
    private float fSkill2CDTime;
    private int skill2Num;
    private float skill2FillCount = 0;
    private float skill2AccumulateTime = 0;

    private bool bSkill3InCD = false;
    private float fSkill3CDTime;
    private int skill3Num;
    private float skill3FillCount = 0;
    private float skill3AccumulateTime = 0;
    private int curHp;
    float currentBossHpVal;
    float targetBossHpVal;

    public Vector2 CurrentDirection
    {
        get
        {
            return currentDirection;
        }

        set
        {
            currentDirection = value;
        }
    }

    protected override void InitWnd()
    {
        base.InitWnd();

        vecDefaultPos = imgDirBg.transform.position;
        //主要为了根据UI适配算出偏移量
        fOperatorDistance = Screen.height * 1.0f / Constants.cScreenStandardHeight * Constants.cOperatorOffset;
        fSkill1CDTime = _resSvc.GetSkillCfg((int)Constants.SkillID.SkillID_one).cdTime / 1000.0f;
        fSkill2CDTime = _resSvc.GetSkillCfg((int)Constants.SkillID.SkillID_two).cdTime / 1000.0f;
        fSkill3CDTime = _resSvc.GetSkillCfg((int)Constants.SkillID.SkillID_three).cdTime / 1000.0f;
        
        curHp = GameRoot.Instance.PlayerData.hp;
        SetText(txtHpBar, curHp + "/" + curHp);
        imgHpBar.fillAmount = 1;

        //隐藏操作杆
        SetActive(imgDirPoint, false);
        InitBossInfo(false);
        RefreshUI();
        RegisterTouchEvents();
    }

    public void InitBossInfo(bool bState, float hpVal = 1.0f)
    {
        SetActive(transBossHpBar, bState);
        imgBossRed.fillAmount = hpVal;
        imgBossYellow.fillAmount = hpVal;
    }

    public void SetBossHpVal(int oldVal, int newVal, int totalVal)
    {
        currentBossHpVal = oldVal * 1.0f / totalVal;
        targetBossHpVal = newVal * 1.0f / totalVal;
        imgBossRed.fillAmount = targetBossHpVal;
    }

    private void BossHpBarAction()
    {
        if (Mathf.Abs(currentBossHpVal - targetBossHpVal) < Constants.cHPSpd * Time.deltaTime)
        {
            currentBossHpVal = targetBossHpVal;
        }
        else if (currentBossHpVal > targetBossHpVal)
        {
            currentBossHpVal -= Constants.cHPSpd * Time.deltaTime;
        }
        else
        {
            currentBossHpVal += Constants.cHPSpd * Time.deltaTime;
        }
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        UpdateSkill1(delta);
        UpdateSkill2(delta);
        UpdateSkill3(delta);

        if (transBossHpBar.gameObject.activeSelf)
        {
            BossHpBarAction();
            imgBossYellow.fillAmount = currentBossHpVal;
        }
    }

    private void UpdateSkill1(float delta)
    {
        if (bSkill1InCD)
        {
            skill1FillCount += delta;
            if (skill1FillCount >= fSkill1CDTime)
            {
                skill1FillCount = 0;
                bSkill1InCD = false;
                SetActive(imgSkill1CD, false);
            }
            else
            {
                imgSkill1CD.fillAmount = 1 - skill1FillCount / fSkill1CDTime;
            }

            skill1AccumulateTime += delta;
            if (skill1AccumulateTime >= 1)
            {
                skill1AccumulateTime -= 1;
                skill1Num -= 1;
                SetText(txtSkill1CD, skill1Num);
            }
        }
    }

    private void UpdateSkill2(float delta)
    {
        if (bSkill2InCD)
        {
            skill2FillCount += delta;
            if (skill2FillCount >= fSkill2CDTime)
            {
                skill2FillCount = 0;
                bSkill2InCD = false;
                SetActive(imgSkill2CD, false);
            }
            else
            {
                imgSkill2CD.fillAmount = 1 - skill2FillCount / fSkill2CDTime;
            }

            skill2AccumulateTime += delta;
            if (skill2AccumulateTime >= 1)
            {
                skill2AccumulateTime -= 1;
                skill2Num -= 1;
                SetText(txtSkill2CD, skill2Num);
            }
        }
    }

    private void UpdateSkill3(float delta)
    {
        if (bSkill3InCD)
        {
            skill3FillCount += delta;
            if (skill3FillCount >= fSkill3CDTime)
            {
                skill3FillCount = 0;
                bSkill3InCD = false;
                SetActive(imgSkill3CD, false);
            }
            else
            {
                imgSkill3CD.fillAmount = 1 - skill3FillCount / fSkill3CDTime;
            }

            skill3AccumulateTime += delta;
            if (skill3AccumulateTime >= 1)
            {
                skill3AccumulateTime -= 1;
                skill3Num -= 1;
                SetText(txtSkill3CD, skill3Num);
            }
        }
    }

    public void RefreshUI()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;

        SetText(txtLevel, playerData.lv);
        SetText(txtName, playerData.name);

        //exp
        int expVal = (int)(playerData.exp / CommonTools.CalcExpValueByLv(playerData.lv) * 100);
        SetText(txtExpProgress, expVal + "%");

        //exp自适应处理
        gridExpBg = expProgressTrans.GetComponent<GridLayoutGroup>();

        //缩放比例
        float globalScaleRate = 1.0f * Constants.cScreenStandardHeight / Screen.height;
        float realWidth = Screen.width * globalScaleRate;
        float cellWidth = (realWidth - 180) / 10;
        gridExpBg.cellSize = new Vector2(cellWidth, 7);

        //进度条
        int expIdx = expVal / 10;
        for (int i = 0; i < expProgressTrans.childCount; i++)
        {
            Image img = expProgressTrans.GetChild(i).GetComponent<Image>();
            if (i < expIdx)
            {
                img.fillAmount = 1;
            }
            else if (i == expIdx)
            {
                img.fillAmount = expVal % 10 * 1.0f / 10;
            }
            else
            {
                img.fillAmount = 0;
            }
        }
    }

    public void RegisterTouchEvents()
    {
        //按下
        OnClickDown(imgTouch.gameObject, (PointerEventData evt) =>
        {
            imgDirBg.transform.position = evt.position;
            vecStartPos = evt.position;
            SetActive(imgDirPoint);
        });

        //抬起
        OnClickUp(imgTouch.gameObject, (PointerEventData evt) =>
        {
            imgDirBg.transform.position = vecDefaultPos;
            imgDirPoint.transform.localPosition = Vector2.zero;
            SetActive(imgDirPoint, false);
            currentDirection = Vector2.zero;
            //方向信息传递出去
            BattleSys.Instance.SetPlayerDirection(currentDirection);
        });

        //拖动
        OnDragEvt(imgTouch.gameObject, (PointerEventData evt) =>
        {
            Vector2 curDir = evt.position - vecStartPos;
            float len = curDir.magnitude;

            //有没有超出
            if (len > fOperatorDistance)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(curDir, fOperatorDistance);
                imgDirPoint.transform.position = vecStartPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = evt.position;
            }

            currentDirection = curDir.normalized;
            //方向信息传递
            BattleSys.Instance.SetPlayerDirection(currentDirection);
        });
    }

    public void SetHpBarVal(int val)
    {
        SetText(txtHpBar, val + "/" + curHp);
        imgHpBar.fillAmount = val * 1.0f / curHp;
    }


    public void OnClickHead()
    {
        BattleSys.Instance.BattleMgr._bIsPauseGame = true;
        BattleSys.Instance.SetFinishBattleWnd(FinishBattleType.Pause);
    }

    public bool GetCanReleaseSkill()
    {
        return BattleSys.Instance.BattleMgr.CanReleaseSkill();
    }

    public void OnClickNormalAtk()
    {
        BattleSys.Instance.ReleaseSkill(Constants.SkillType.SkillType_NormalAtk);
    }

    public void OnClickSkillOne()
    {
        if (bSkill1InCD == false && GetCanReleaseSkill())
        {
            BattleSys.Instance.ReleaseSkill(Constants.SkillType.SkillType_Skill_1);
            bSkill1InCD = true;
            SetActive(imgSkill1CD);
            imgSkill1CD.fillAmount = 1;
            skill1Num = (int)fSkill1CDTime;
            SetText(txtSkill1CD, skill1Num);
        }
    }

    public void OnClickSkillTwo()
    {
        if (bSkill2InCD == false && GetCanReleaseSkill())
        {
            BattleSys.Instance.ReleaseSkill(Constants.SkillType.SkillType_Skill_2);
            bSkill2InCD = true;
            SetActive(imgSkill2CD);
            imgSkill2CD.fillAmount = 1;
            skill2Num = (int)fSkill2CDTime;
            SetText(txtSkill2CD, skill2Num);
        }
    }

    public void OnClickSkillThree()
    {
        if (bSkill3InCD == false && GetCanReleaseSkill())
        {
            BattleSys.Instance.ReleaseSkill(Constants.SkillType.SkillType_Skill_3);
            bSkill3InCD = true;
            SetActive(imgSkill3CD);
            imgSkill3CD.fillAmount = 1;
            skill3Num = (int)fSkill3CDTime;
            SetText(txtSkill3CD, skill3Num);
        }
    }

}