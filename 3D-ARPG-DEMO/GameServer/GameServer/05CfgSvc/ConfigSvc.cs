/*读取配置信息*/

using System;
using System.Collections.Generic;
using System.Xml;

public class ConfigSvc
{
    private static ConfigSvc s_instance = null;

    private Dictionary<int, GuideTaskCfg> _dictAutoGuideCfgData = new Dictionary<int, GuideTaskCfg>(); /*自动引导任务的字典*/
    private Dictionary<int, Dictionary<int, StrongerCfd>> _dictStrongerCfgData = new Dictionary<int, Dictionary<int, StrongerCfd>>(); /*强化数据字典*/
    private Dictionary<int, TaskRewardCfg> _dictTaskRewardCfgData = new Dictionary<int, TaskRewardCfg>(); /*任务奖励的字典*/
    private Dictionary<int, MapCfg> _dictMapCfgData = new Dictionary<int, MapCfg>(); /*地图的字典*/
    
    public static ConfigSvc Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new ConfigSvc();
            }

            return s_instance;
        }
    }

    public void Init()
    {
        CommonTools.Log("ConfigSvc Init  Done...");
        InitGuideCfgData();
        InitStrongerCfgData();
        InitTaskRewardCfgData();
        InitMapCfgData();
    }

    #region 任务引导
    private void InitGuideCfgData()
    {
        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.Load(@"..\..\..\Configs\guide.xml");

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            GuideTaskCfg guideData = new GuideTaskCfg
            {
                ID = id,
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                     case "coin":
                         guideData.gainCoin = int.Parse(elem.InnerText);
                         break;

                     case "exp":
                         guideData.gainExp = int.Parse(elem.InnerText);
                         break;
                }
            }
            #endregion

            _dictAutoGuideCfgData.Add(id, guideData);
        }
    }

    /// <summary>
    /// 获取任务
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public GuideTaskCfg GetGuideTaskData(int taskId)
    {
        GuideTaskCfg data = null;
        if (_dictAutoGuideCfgData.TryGetValue(taskId, out data))
        {
            return data;
        }

        return null;
    }
    #endregion

    #region 强化升级
    private void InitStrongerCfgData()
    {
        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.Load(@"..\..\..\Configs\strong.xml");

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            StrongerCfd strongerData = new StrongerCfd
            {
                ID = id,
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                int value = int.Parse(elem.InnerText);
                switch (elem.Name)
                {
                    case "pos":
                        strongerData.stongerType = value;
                        break;

                    case "starlv":
                        strongerData.starLv = value;
                        break;

                    case "addhp":
                        strongerData.addHp = value;
                        break;

                    case "addhurt":
                        strongerData.addHurt = value;
                        break;

                    case "adddef":
                        strongerData.addDef = value;
                        break;

                    case "minlv":
                        strongerData.minLv = value;
                        break;

                    case "coin":
                        strongerData.coin = value;
                        break;

                    case "crystal":
                        strongerData.crystal = value;
                        break;
                }
            }
            #endregion

            Dictionary<int, StrongerCfd> tmpDic = null;

            if (_dictStrongerCfgData.TryGetValue(strongerData.stongerType, out tmpDic))
            {
                tmpDic.Add(strongerData.starLv, strongerData);
            }
            else
            {
                tmpDic = new Dictionary<int, StrongerCfd>();
                tmpDic.Add(strongerData.starLv, strongerData);
                _dictStrongerCfgData.Add(strongerData.stongerType, tmpDic);
            }
        }
    }

    public StrongerCfd GetStrongerData(int stongerType, int starLv)
    {
        StrongerCfd data = null;
        Dictionary<int, StrongerCfd> tmpDic = null;
        if (_dictStrongerCfgData.TryGetValue(stongerType, out tmpDic))
        {
            if (tmpDic.ContainsKey(starLv))
            {
                data = tmpDic[starLv];
            }
        }

        return data;
    }
    #endregion

    #region 解析任务奖励配置文件

    private void InitTaskRewardCfgData()
    {
        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.Load(@"..\..\..\Configs\taskreward.xml");

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            TaskRewardCfg taskData = new TaskRewardCfg
            {
                ID = id,
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                    case "count":
                        taskData.count = int.Parse(elem.InnerText);
                        break;

                    case "coin":
                        taskData.coin = int.Parse(elem.InnerText);
                        break;

                    case "exp":
                        taskData.exp = int.Parse(elem.InnerText);
                        break;
                }
            }
            #endregion

            _dictTaskRewardCfgData.Add(id, taskData);
        }
    }

    /// <summary>
    /// 获取任务奖励
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TaskRewardCfg GetTaskRewardData(int id)
    {
        TaskRewardCfg data = null;
        if (_dictTaskRewardCfgData.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }

    #endregion


    #region 地图配置文件解析

    private void InitMapCfgData()
    {
        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.Load(@"..\..\..\Configs\map.xml");

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            MapCfg mapData = new MapCfg
            {
                ID = id,
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                    case "power":
                        mapData.costPower = int.Parse(elem.InnerText);
                        break;

                    case "coin":
                        mapData.rewardCoin = int.Parse(elem.InnerText);
                        break;

                    case "exp":
                        mapData.rewardExp = int.Parse(elem.InnerText);
                        break;

                    case "crystal":
                        mapData.rewardCrystal = int.Parse(elem.InnerText);
                        break;

                }
            }
            #endregion

            _dictMapCfgData.Add(id, mapData);
        }
    }

    public MapCfg GetMapCfgData(int id)
    {
        MapCfg data = null;
        if (_dictMapCfgData.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }

    #endregion
}

public class BaseData<T>
{
    public int ID;
}

/// <summary>
/// 任务引导
/// </summary>
public class GuideTaskCfg : BaseData<GuideTaskCfg>
{
    public int gainCoin; /*完成该任务可获得的金币*/
    public int gainExp; /*完成该任务可获得的经验*/
}

/// <summary>
/// 强化升级
/// </summary>
public class StrongerCfd : BaseData<StrongerCfd>
{
    public int stongerType;                   /*强化类型*/
    public int starLv;                           /*星级*/
    public int addHp;                         /*加血*/
    public int addHurt;                      /*加伤害*/
    public int addDef;                      /*加防御*/
    public int minLv;                       /*最小等级*/
    public int coin;                        /*需要的金币*/
    public int crystal;                   /*需要的水晶*/
}

/// <summary>
/// 任务奖励数据
/// </summary>
public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public int count;
    public int exp;
    public int coin;
}

public class TaskData : BaseData<TaskData>
{
    public int progress;
    public bool bTaked;
}

public class MapCfg : BaseData<MapCfg>
{
    public int costPower;                     /*体力消耗*/
    public int rewardCoin;                   /*获得的金币*/
    public int rewardExp;                    /*获得的经验*/
    public int rewardCrystal;               /*获得的水晶*/
}