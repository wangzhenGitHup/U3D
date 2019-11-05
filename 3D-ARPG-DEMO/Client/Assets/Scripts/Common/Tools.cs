/****************************************************
    文件：Tools.cs
	作者：WangZhen
    日期：2019/6/10 21:32:6
	功能：工具类
*****************************************************/

using UnityEngine;
using System.Collections;

public class Tools 
{
    public static int GetRandomInt(int min, int max, System.Random randomFunc = null)
    {
        if (randomFunc == null)
        {
            randomFunc = new System.Random();
        }

        int val = randomFunc.Next(min, max + 1);
        return val;
    }
}