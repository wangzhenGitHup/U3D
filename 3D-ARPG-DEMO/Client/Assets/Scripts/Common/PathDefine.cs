/****************************************************
    文件：PathDefine.cs
	作者：WangZhen
    日期：2019/6/10 21:20:57
	功能：资源路径管理
*****************************************************/

using UnityEngine;

public class PathDefine 
{
    //随机名字文本路径
    public const string cPath_RandomNameCfg = "ResCfgs/rdname";

    //随机地图路径
    public const string cPath_MapCfg = "ResCfgs/map";

    //自动引导任务数据路径
    public const string cPath_AutoGuideCfg = "ResCfgs/guide";

    //强化数据路径
    public const string cPath_StrongerCfg = "ResCfgs/strong";

    //任务数据路径
    public const string cPath_TaskRewardCfg = "ResCfgs/taskreward";

    //技能数据路径
    public const string cPath_SkillCfg = "ResCfgs/skill";
    public const string cPath_SkillMoveCfg = "ResCfgs/skillmove";
    public const string cPath_SkillActionCfg = "ResCfgs/skillaction";

    //怪物数据路径
    public const string cPath_MonsterCfg = "ResCfgs/monster";

    //角色资源路径
    public const string cMainCityPlayerPrefab = "PrefabPlayer/AssassinCity";
    public const string cPlayerBattlePrefab = "PrefabPlayer/AssassinBattle";
    public const string cItemHpBarPrefab = "PrefabUI/ItemHpBar";

    #region 自动任务图标

    //任务小图标
    public const string cTaskDefaultIcon = "ResImage/task";            /*默认icon*/
    public const string cTaskWiseManIcon = "ResImage/wiseman"; /*智者icon*/
    public const string cTasGeneralIcon = "ResImage/general";      /*将军icon*/
    public const string cTaskArtisanIcon = "ResImage/artisan";     /*工匠icon*/
    public const string cTaskTraderIcon = "ResImage/trader";       /*商人icon*/

    //任务对话大图标
    public const string cTaskDialogIcon_Self = "ResImage/assassin";         /*玩家自己*/
    public const string cTaskDialogIcon_Wiseman = "ResImage/npc0";      /*智者icon*/
    public const string cTaskDialogIcon_General = "ResImage/npc1";      /*将军*/
    public const string cTaskDialogIcon_Artisan = "ResImage/npc2";      /*工匠*/
    public const string cTaskDialogIcon_Trader = "ResImage/npc3";      /*商人*/
    public const string cTaskDialogIcon_Default = "ResImage/npcguide"; 
    #endregion


    /************************强化相关****************************************************/
    public const string cItemIconArrorBg = "ResImage/btnstrong";
    public const string cItemIconPlaneBg = "ResImage/charbg3";

    public const string cItemIconHelmet = "ResImage/toukui";    /*头盔*/
    public const string cItemIconBody = "ResImage/body";      /*身体*/
    public const string cItemIconWaist = "ResImage/yaobu";    /*腰部*/
    public const string cItemIconArm = "ResImage/hand";     /*手臂*/
    public const string cItemIconLeg = "ResImage/leg";     /*腿部*/
    public const string cItemIconShoes= "ResImage/foot";  /*鞋子*/

    public const string cItemIconLightingStar = "ResImage/star2";  /*明亮的星星*/
    public const string cItemIconDarkStar = "ResImage/star1";     /*暗星星*/

    public const string cChatBtnNormal = "ResImage/btntype2";
    public const string cChatBtnPressed = "ResImage/btntype1"; 
    /************************************************************************************/

    public const string cTaskItem = "PrefabUI/ItemTask";
}