using System;
using System.Collections.Generic;


public class Constants
{
    public enum ShopType
    {
        ShopType_BuyPower,
        ShopType_MakeCoin
    }

    public const int cPowerAddInterval = 5; /*体力每5分钟加一次*/
    public const int cPowerAddValPerChange = 2; /*体力每次加2*/

    public const int cTask_WisemanTellme = 1; /*智者点拨任务*/
    public const int cTask_Copyer = 2;             /*副本任务*/
    public const int cTask_Stronger = 3;          /*强化任务*/
    public const int cTask_BuyPower = 4;       /*购买体力任务*/
    public const int cTask_MakeCoin = 5;       /*铸币任务*/
    public const int cTask_Chat = 6;              /*聊天任务*/

}

