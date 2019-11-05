/****************************************************
    文件：Constants.cs
	作者：WangZhen
    日期：2019/6/9 22:32:1
	功能：通用常量
*****************************************************/

using UnityEngine;

public class Constants 
{
    /**************************************场景相关********************************************/
    //场景名称
    public const string cSceneLogin = "SceneLogin";
    public const string cSceneMainCity = "SceneMainCity";
    public const int cMainCityMapID = 10000;
    /********************************************************************************************/

    //账号key
    public const string cAccountKey = "AccountKey";
    //密码key
    public const string cPwdKey = "PwdKey";

    /**************************************音乐相关********************************************/
    //登陆背景音乐
    public const string cBGM = "bgLogin";
    //主城背景音乐
    public const string cBGMMainCity = "bgMainCity";
    //打开大窗口音效
    public const string cOpenBigWndSound = "uiOpenPage";
    //登录按钮
    public const string cLoginBtnSound = "uiLoginBtn";
    //常规UI点击音效
    public const string cNormalUIBtnSound = "uiClickBtn";
    public const string cUIExternBtnSound = "uiExtenBtn";
    public const string cUIClosedBtnSound = "uiCloseBtn";
    public const string cCopyerUpdateSound = "fbitem";

    //战斗场景BGM
    public const string cFightBGM = "bgHuangYe";
    //受击音效
    public const string cHitSound = "assassin_Hit";

    //战斗结算音效
    public const string cBattleWin = "fbwin";
    public const string cBattleLose = "fblose";
  
    /********************************************************************************************/

    /*************************************自动引导任务npc ID相关********************************************/
    public const int cNPC_Wiseman = 0;      /*智者i*/
    public const int cNPC_General = 1;      /*绅士*/
    public const int cNPC_Artisan = 2;      /*工匠*/
    public const int cNPC_Trader = 3;       /*商人*/
    /********************************************************************************************/

    //屏幕标准宽高
    public const int cScreenStandardWidth = 1334;
    public const int cScreenStandardHeight = 750;

    //摇杆偏移
    public const float cOperatorOffset = 90.0f;
    //角色移动速度
    public const float cPlayerMoveSpeed = 8.0f;
    //npc移动速度
    public const float cNPCMoveSpeed = 3.0f;
    //运动平滑
    public const float cMoveLerpSpd = 5.0f;
    //血条平滑速度
    public const float cHPSpd = 0.2f;
    //普攻连招有效间隔
    public const int cComboInterval = 500; //ms

    //混合参数: 待机和奔跑
    public const int cBlendIdle = 0;
    public const int cBlendRun = 1;
    public const int cActionDefault = -1;
    public const int cActionBorn = 0;
    public const int cActionDie = 100;
    public const int cActionHit = 101;

    //玩家层
    public const int cLayerPlayer = 9;
    //怪物层
    public const int cLayerMonster = 10;

    //死亡的动画长度
    public const int cDieAnimationLen = 5000;

    //玩家属性常量
    public enum PlayerPropType
    {
        PropType_Hp = 1,     /*血量属性*/
        PropType_Hurt = 2,  /*伤害属性*/
        PropType_Def = 3   /*防御属性*/
    }

    //聊天类型
    public enum ChatType
    {
        ChatType_World = 0,    /*世界*/
        ChatType_Guild = 1,    /*公会*/
        ChatType_Friend = 2   /*好友*/
    }

    public enum ShopType
    {
        ShopType_BuyPower,
        ShopType_MakeCoin
    }

    public const int cMaxChatInfo = 12;
    public const int cMaxStarLv = 10;


    public enum SkillType
    {
        SkillType_NormalAtk,
        SkillType_Skill_1,
        SkillType_Skill_2,
        SkillType_Skill_3
    }

    public enum SkillID
    {
        SkillID_one = 101,
        SkillID_two = 102,
        SkillID_three = 103,
        Normal_Attack1 = 111,
        Normal_Attack2 = 112,
        Normal_Attack3 = 113,
        Normal_Attack4 = 114,
        Normal_Attack5 = 115
    }

    public enum DamageType
    {
        None,
        DamageType_AD,
        DamageType_AP,
    }

    public enum EntityType
    {
        None,
        Player,
        Monster
    }

    public enum EntitiyState
    {
        None,
        NoEffectHit,
    }

    public enum MonsterType
    {
        None,
        Normal,
        Boss
    }
    /**************************文本颜色处理********************************/
    #region 颜色
    public enum TextColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }
    private const string ColorRed = "<color=#FF0000FF>";
    private const string ColorGreen = "<color=#00FF00FF>";
    private const string ColorBlue = "<color=#00B4FFFF>";
    private const string ColorYellow = "<color=#FFFF00FF>";
    private const string ColorEnd = "</color>";

    public static string Color(string str, TextColor c)
    {
        string result = "";
        switch (c)
        {
            case TextColor.Red:
                result = ColorRed + str + ColorEnd;
                break;
            case TextColor.Green:
                result = ColorGreen + str + ColorEnd;
                break;
            case TextColor.Blue:
                result = ColorBlue + str + ColorEnd;
                break;
            case TextColor.Yellow:
                result = ColorYellow + str + ColorEnd;
                break;
        }
        return result;
    }
    #endregion
    /*********************************************************************/
}