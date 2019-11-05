/****************************************************
    文件：BattleManager.cs
	作者：WangZhen
    日期：2019/6/22 23:29:45
	功能：战斗管理
*****************************************************/

using UnityEngine;
using Protocols;
using System.Collections.Generic;
using System;

public class BattleManager : MonoBehaviour 
{
    public bool _bTriggerCheck = true;
    public bool _bIsPauseGame = false;

    ResSvc resSvc;
    AudioSvc audioSvc;

    BattleStateManager battleStateMgr;
    MapManager mapMgr;
    SkillManager skillMgr;
    EntityPlayer entityPlayer;
    MapCfg mapCfg;
    Dictionary<string, EntityMonster> dictMonster = new Dictionary<string, EntityMonster>();
    double lastAtkTime = 0;

    //普攻连招技能id号
    int[] comboDataArr = new int[] { 
        (int)Constants.SkillID.Normal_Attack1, 
        (int)Constants.SkillID.Normal_Attack2, 
        (int)Constants.SkillID.Normal_Attack3,
        (int)Constants.SkillID.Normal_Attack4,
        (int)Constants.SkillID.Normal_Attack5
    };
    int comboIdx = 0;

    #region 属性
    public double LastAtkTime
    {
        set
        {
            lastAtkTime = value;
        }
    }

    public int ComboIdx
    {
        set
        {
            comboIdx = value;
        }
    }

    public EntityPlayer GetEntityPlayer
    {
        get
        {
            return entityPlayer;
        }

        set
        {
            entityPlayer = value;
        }
    }

    #endregion

    public void Init(int chapterId, Action callback = null)
    {
        CommonTools.Log("BattleManager Init.....");

        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;

        battleStateMgr = gameObject.AddComponent<BattleStateManager>();
        battleStateMgr.Init();

        skillMgr = gameObject.AddComponent<SkillManager>();
        skillMgr.Init();

        //加载战场地图
        mapCfg = resSvc.GetMapCfgData(chapterId);
        resSvc.AsyncLoadScene(mapCfg.sceneName, () =>
        {
            //初始化地图数据
            GameObject objMap = GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr = objMap.GetComponent<MapManager>();
            mapMgr.Init(this);
            objMap.transform.localPosition = Vector3.zero;
            objMap.transform.localScale = Vector3.one;

            Camera.main.transform.position = mapCfg.mainCameraPos;
            Camera.main.transform.localEulerAngles = mapCfg.mainCameraRote;

            LoadPlayer(mapCfg);
            entityPlayer.Idle();

            audioSvc.PlayBGM(Constants.cFightBGM);
            BattleSys.Instance.SetPlayerControlWndState();

            ActiveCurrentBatchMonsters();

            if (callback != null)
            {
                callback();
            }
        });
    }

    private void LoadPlayer(MapCfg mapCfg)
    {
        GameObject objPlayer = resSvc.LoadPrefab(PathDefine.cPlayerBattlePrefab);
        objPlayer.transform.localPosition = mapCfg.playerBornPos;
        objPlayer.transform.localEulerAngles = mapCfg.playerBornRote;
        objPlayer.transform.localScale = Vector3.one;

        PlayerData playerData = GameRoot.Instance.PlayerData;
        BattleProps battleProps = new BattleProps
        {
            hp = playerData.hp,
            ad = playerData.ad,
            ap = playerData.ap,
            addef = playerData.addef,
            apdef = playerData.apdef,
            dodge = playerData.dodge,
            pierce = playerData.pierce,
            critical = playerData.critical
        };

        entityPlayer = new EntityPlayer
        { 
            _stateMgr = battleStateMgr,
            _skillMgr = skillMgr,
            _battleMgr = this
        };
        entityPlayer.SetBattleProps(battleProps);
        entityPlayer.EntityName = "Player"; 

        PlayerController playerContrl = objPlayer.GetComponent<PlayerController>();
        playerContrl.Init();
        entityPlayer.SetController(playerContrl);
    }

    public void SetPlayerDirection(Vector2 dirVec)
    {
        if (entityPlayer._bCanControl == false)
        {
            return;
        }
        if (entityPlayer._currentPlayerAniState == PlayerAniState.Idle || entityPlayer._currentPlayerAniState == PlayerAniState.Move)
        {
            if (dirVec == Vector2.zero)
            {
                entityPlayer.Idle();
            }
            else
            {
                entityPlayer.Move();
                entityPlayer.SetDirection(dirVec);
            }
        }
    }

    public Vector2 GetCurrentDirection()
    {
        return BattleSys.Instance.GetCurrentDirection();
    }


    #region 怪物相关

    public void LoadMonsterByBatchID(int batchIdx)
    {
        for (int i = 0; i < mapCfg.monsterLst.Count; i++)
        {
            MonsterData monsterData = mapCfg.monsterLst[i];
            if (monsterData.batchIndex == batchIdx)
            {
                GameObject objMonster = resSvc.LoadPrefab(monsterData.cfg.resPath, true);
                objMonster.transform.localPosition = monsterData.vecBornPos;
                objMonster.transform.localEulerAngles = monsterData.vecRotate;
                objMonster.transform.localScale = Vector3.one;
                objMonster.name = "monster" + batchIdx + "_" + i;

                EntityMonster entityMonster = new EntityMonster
                {
                    _battleMgr = this,
                    _stateMgr = battleStateMgr,
                    _skillMgr = skillMgr
                };
                entityMonster.monsterData = monsterData;
                entityMonster.SetBattleProps(monsterData.cfg.prop);
                entityMonster.EntityName = objMonster.name; 

                dictMonster.Add(objMonster.name, entityMonster);

                MonsterController monsterControl = objMonster.GetComponent<MonsterController>();
                monsterControl.Init();
                entityMonster.SetController(monsterControl);
                objMonster.SetActive(false);

                if (monsterData.cfg.monsterType == Constants.MonsterType.Normal)
                {
                    //绑定血条
                    GameRoot.Instance._dynamicWnd.AddHpBarItem(objMonster.name, entityMonster.HP, monsterControl.hpRoot.transform);
                }
                else if (monsterData.cfg.monsterType == Constants.MonsterType.Boss)
                {
                    BattleSys.Instance._playerControlWnd.InitBossInfo(true);
                }
            }
        }
    }

    public void ActiveCurrentBatchMonsters()
    {
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            foreach(var item in dictMonster)
            {
                item.Value.SetActive();
                item.Value.Born();
                TimerSvc.Instance.AddTimeTask((int ttid) =>
                {
                    item.Value.Idle();
                }, 1000);
            }
        }, 500);
    }

    public List<EntityMonster> GetAllEntityMonsters()
    {
        List<EntityMonster> tmpList = new List<EntityMonster>();
        foreach (var item in dictMonster)
        {
            tmpList.Add(item.Value);
        }

        return tmpList;
    }

    public void RemoveMonsterData(string key)
    {
        EntityMonster monster = null;
        if (dictMonster.TryGetValue(key, out monster))
        {
            dictMonster.Remove(key);
            GameRoot.Instance._dynamicWnd.RemoveHpBarItem(key);
        }
    }

    private void Update()
    {
        foreach (var item in dictMonster)
        {
            EntityMonster entityMonster = item.Value;
            entityMonster.UpdateAILogic();
        }

        //检测当前批次怪物是否全部死亡
        if (mapMgr != null)
        {
            if (_bTriggerCheck && dictMonster.Count == 0)
            {
                bool isExist = mapMgr.OpenNextTrigger();
                _bTriggerCheck = false;
                
                if (!isExist)
                {
                    //关卡结束，战斗胜利
                    FinishBattle(true,  entityPlayer.HP);
                }
            }
        }
    }

    public void FinishBattle(bool bWin,  int leftHp)
    {
        _bIsPauseGame = true;
        audioSvc.StopBGM();
        BattleSys.Instance.FinishBattle(bWin, leftHp);
    }

    public bool CanReleaseSkill()
    {
        return entityPlayer._bCanReleaseSkill;
    }

    
    #endregion

    #region 技能相关
    public void ReleaseSkill(Constants.SkillType skillType)
    {
        switch (skillType)
        {
            case Constants.SkillType.SkillType_NormalAtk:
                ReleaseNormalAtk();
                break;

            case Constants.SkillType.SkillType_Skill_1:
                ReleaseSkillOne();
                break;

            case Constants.SkillType.SkillType_Skill_2:
                ReleaseSkillTwo();
                break;

            case Constants.SkillType.SkillType_Skill_3:
                ReleaseSkillThree();
                break;
        }
    }
    /// <summary>
    /// 普通攻击
    /// </summary>
    private void ReleaseNormalAtk()
    {
        if (entityPlayer._currentPlayerAniState == PlayerAniState.Attack)
        {
            double nowTime = TimerSvc.Instance.GetNowTime();
            if (nowTime - lastAtkTime < Constants.cComboInterval && lastAtkTime != 0)
            {
                if (comboIdx < comboDataArr.Length - 1)
                {
                    comboIdx++;
                    //comboIdx = comboIdx >= (comboDataArr.Length) ? 0 : comboIdx;
                    entityPlayer._comboQue.Enqueue(comboDataArr[comboIdx]);
                    lastAtkTime = nowTime;
                }
                else
                {
                    comboIdx = 0;
                    lastAtkTime = 0;
                }
            }
        }
        else if(entityPlayer._currentPlayerAniState == PlayerAniState.Idle || 
            entityPlayer._currentPlayerAniState == PlayerAniState.Move)
        {
            comboIdx = 0;
            entityPlayer.Attack(comboDataArr[comboIdx]);
            lastAtkTime = TimerSvc.Instance.GetNowTime();
        }
    }

    private void ReleaseSkillOne()
    {
        entityPlayer.Attack((int)Constants.SkillID.SkillID_one);
    }

    private void ReleaseSkillTwo()
    {
        entityPlayer.Attack((int)Constants.SkillID.SkillID_two);
    }

    private void ReleaseSkillThree()
    {
        entityPlayer.Attack((int)Constants.SkillID.SkillID_three);
    }

    #endregion


}