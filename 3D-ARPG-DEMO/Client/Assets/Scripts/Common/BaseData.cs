/****************************************************
    文件：BaseData.cs
	作者：WangZhen
    日期：2019/6/15 23:11:38
	功能：配置数据类
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 配置数据基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseData <T>
{
    public int ID;
}


/// <summary>
/// 地图数据
/// </summary>
public class MapCfg : BaseData<MapCfg>
{
    public string mapName;                               /*地图名称*/
    public string sceneName;                            /*场景名称*/
    public Vector3 mainCameraPos;                 /*摄像机位置*/
    public Vector3 mainCameraRote;               /*摄像机的旋转角度*/
    public Vector3 playerBornPos;                  /*角色出生位置*/
    public Vector3 playerBornRote;                /*角色出生时旋转角度*/
    public int costPower;                              /*体力消耗*/
    public List<MonsterData> monsterLst;   /*地图上的怪物*/
    public int rewardCoin;                           /*获得的金币*/
    public int rewardExp;                           /*获得的经验*/
    public int rewardCrystal;                     /*获得的水晶*/
}

/// <summary>
/// 自动引导数据
/// </summary>
public class AutoGuideCfg : BaseData<AutoGuideCfg>
{
    public int npcID; /*触发人物目标npc索引号*/
    public string dialogArr; /*对话*/
    public int targetTaskID; /*目标任务ID*/
    public int gainCoin; /*完成该任务可获得的金币*/
    public int gainExp; /*完成该任务可获得的经验*/
}

/// <summary>
/// 强化数据
/// </summary>
public class StrongerCfg : BaseData<StrongerCfg>
{
    public int stongerType;                   /*强化类型*/
    public int starLv;                           /*星级*/
    public int addHp;                         /*加血*/
    public int addHurt;                      /*加伤害*/
    public int addDef;                      /*加防御*/
    public int minLv;                       /*最小等级*/
    public int coin;                        /*需要的金币*/
    public int crystal;                    /*需要的水晶*/
}

/// <summary>
/// 任务奖励数据
/// </summary>
public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public string taskName;
    public int count;
    public int exp;
    public int coin;
}

public class TaskData : BaseData<TaskData>
{
    public int progress;
    public bool bTasked;
}
public class SkillActionCfg : BaseData<SkillActionCfg>
{
    public int delayTime;
    public float radius; /*伤害范围*/
    public float angle;  /*伤害角度*/
}

public class SkillDataCfg : BaseData<SkillDataCfg>
{
    public string skillName;
    public int skillTime;
    public int cdTime;
    public int aniAction; /*动画的动作*/
    public string fx;
    public bool bCombo;
    public bool bCollide;
    public bool bBreak;
    public Constants.DamageType damageType;
    public List<int> skillMoveList;
    public List<int> skillActionList;
    public List<int> skillDamageList;
}

public class SkillMoveCfg : BaseData<SkillMoveCfg>
{
    public int moveTime;
    public float moveDistance;
    public int delayTime;
}

public class MonsterCfd : BaseData<MonsterCfd>
{
    public string name;
    public string resPath;
    public BattleProps prop;
    public int skillID;
    public float atkDistance;
    public Constants.MonsterType monsterType;
    public bool bStop; /*怪物是否能被攻击中断当前的状态*/
}

public class MonsterData : BaseData<MonsterData>
{
    public int batchIndex;
    public int index;
    public MonsterCfd cfg;
    public Vector3 vecBornPos;
    public Vector3 vecRotate;
    public int lv;
}

public class BattleProps
{
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;
}