/****************************************************
    文件：ResSvc.cs
	作者：WangZhen
    日期：2019/6/9 22:20:57
	功能：资源加载服务
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml;

public class ResSvc : MonoBehaviour
{
    public static ResSvc Instance = null;

    private Action asyncProgress = null; /*进度委托*/
    private Dictionary<string, AudioClip> dictAudioClip = new Dictionary<string, AudioClip>(); /*缓存音乐*/
    private Dictionary<string, Sprite> dictSpriteCache = new Dictionary<string, Sprite>(); /*图片缓存*/

    private List<string> listRandomSurname = new List<string>(); /*姓*/
    private List<string> listRandomMan = new List<string>(); /*男人的名字*/
    private List<string> listRandomWoman = new List<string>(); /*女人的名字*/
    private Dictionary<int, MapCfg> dictMapCfgData = new Dictionary<int, MapCfg>(); /*地图配置字典*/
    private Dictionary<string, GameObject> dictObjPrefab = new Dictionary<string, GameObject>(); /*预制体字典*/
    private Dictionary<int, AutoGuideCfg> dictAutoGuideCfgData = new Dictionary<int, AutoGuideCfg>(); /*自动引导任务的字典*/
    private Dictionary<int, Dictionary<int, StrongerCfg>> dictStrongerCfgData = new Dictionary<int, Dictionary<int, StrongerCfg>>(); /*强化数据字典: first key: pos, second key: starLv*/
    private Dictionary<int, TaskRewardCfg> dictTaskRewardCfgData = new Dictionary<int, TaskRewardCfg>(); /*任务奖励的字典*/
    private Dictionary<int, SkillDataCfg> dictSkillCfgData = new Dictionary<int, SkillDataCfg>(); /*技能数据字典*/
    private Dictionary<int, SkillMoveCfg> dictSkillMoveCfgData = new Dictionary<int, SkillMoveCfg>(); /*技能数据字典*/
    private Dictionary<int, MonsterCfd> dictMonsterCfgData = new Dictionary<int, MonsterCfd>(); /*怪物数据字典*/
    private Dictionary<int, SkillActionCfg> dictSkillActionCfgData = new Dictionary<int, SkillActionCfg>(); /*技能伤害数据字典*/


    private void Update()
    {
        if (asyncProgress != null)
        {
            asyncProgress();
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void InitResSvc()
    {
        CommonTools.Log("Init ResSvc.....");
        Instance = this;
        InitRandomNameCfg(PathDefine.cPath_RandomNameCfg);

        InitMonsterCfgData(PathDefine.cPath_MonsterCfg);
        InitMapCfg(PathDefine.cPath_MapCfg);
        
        InitAutoGuideCfgData(PathDefine.cPath_AutoGuideCfg);
        InitStrongerCfgData(PathDefine.cPath_StrongerCfg);
        InitTaskRewardCfgData(PathDefine.cPath_TaskRewardCfg);

        InitSkillActionCfgData(PathDefine.cPath_SkillActionCfg);
        InitSkillCfgData(PathDefine.cPath_SkillCfg);
        InitSkillMoveCfgData(PathDefine.cPath_SkillMoveCfg);
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="finishCallback">回调函数</param>
    public void AsyncLoadScene(string sceneName, Action finishCallback)
    {
        //打开加载界面
        GameRoot.Instance.GetLoadingWnd.SetWndState(true);

        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
        asyncProgress = () =>
        {
            //设置进度值
            float curProgress = sceneAsync.progress;
            GameRoot.Instance.GetLoadingWnd.SetProgress(curProgress);

            if (curProgress >= 1.0f)
            {
                asyncProgress = null;
                sceneAsync = null;
                //关闭加载界面
                GameRoot.Instance.GetLoadingWnd.SetWndState(false);
                if(finishCallback != null)
                {
                    finishCallback();
                }
            }
        };
    }

    /// <summary>
    /// 加载音效
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="isCache">是否缓存起来</param>
    /// <returns></returns>
    public AudioClip LoadAudio(string path, bool isCache = false)
    {
        AudioClip audioClip = null;
        if (!dictAudioClip.TryGetValue(path, out audioClip))
        {
            audioClip = Resources.Load<AudioClip>(path);
            if (isCache)
            {
                dictAudioClip.Add(path, audioClip);
            }
        }
        return audioClip;
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    /// <param name="path"></param>
    /// <param name="bCache"></param>
    /// <returns></returns>
    public Sprite LoadSprite(string path, bool bCache = false)
    {
        Sprite sp = null;
        if (!dictSpriteCache.TryGetValue(path, out sp))
        {
            sp = Resources.Load<Sprite>(path);
            if (bCache)
            {
                dictSpriteCache.Add(path, sp);
            }
        }

        return sp;
    }

    /// <summary>
    /// 获取随机角色名称
    /// </summary>
    /// <param name="isMan"></param>
    /// <returns></returns>
    public string GetRandomRoleName(bool isMan = true)
    {
        //System.Random random = new System.Random();
        string roleName = listRandomSurname[Tools.GetRandomInt(0, listRandomSurname.Count - 1)];
        if (isMan)
        {
            roleName += listRandomMan[Tools.GetRandomInt(0, listRandomMan.Count - 1)];
        }
        else 
        {
            roleName += listRandomWoman[Tools.GetRandomInt(0, listRandomWoman.Count - 1)];
        }

        return roleName;
    }

    /// <summary>
    /// 获取预制体对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="bCache"></param>
    /// <returns></returns>
    public GameObject LoadPrefab(string path, bool bCache = false)
    {
        GameObject objPrefab = null;
        if (!dictObjPrefab.TryGetValue(path, out objPrefab))
        {
            objPrefab = Resources.Load<GameObject>(path);
            if (bCache)
            {
                dictObjPrefab.Add(path, objPrefab);
            }
        }

        GameObject objItem = null;
        if (objPrefab != null)
        {
            //实例化对象
            objItem = Instantiate(objPrefab);
        }

        return objItem;
    }

    #region 解析随机角色名配置文件
    private void InitRandomNameCfg(string xmlPath)
    {
        TextAsset xmlText = Resources.Load<TextAsset>(xmlPath);
        if (!xmlText)
        {
            CommonTools.Log("xml file: " + PathDefine.cPath_RandomNameCfg + "not exits!", 
                LogType.LogType_Error);
            return;
        }

        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText.text);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            //int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                    case "surname":
                        listRandomSurname.Add(elem.InnerText);
                        break;

                    case "man":
                        listRandomMan.Add(elem.InnerText);
                        break;

                    case "woman":
                        listRandomWoman.Add(elem.InnerText);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 根据id获取地图数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MapCfg GetMapCfgData(int id)
    {
        MapCfg data;
        if (dictMapCfgData.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }

    #endregion

    #region 解析地图配置文件
    private void InitMapCfg(string xmlPath)
    {
        Debug.Log(xmlPath);
        TextAsset xmlText = Resources.Load<TextAsset>(xmlPath);
        if (!xmlText)
        {
            CommonTools.Log("xml file: " + PathDefine.cPath_RandomNameCfg + "not exits!",
                LogType.LogType_Error);
            return;
        }

        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText.text);

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
                monsterLst = new List<MonsterData>()
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                    case "power":
                        mapData.costPower = int.Parse(elem.InnerText);
                        break;

                    case "mapName":
                        mapData.mapName = elem.InnerText;
                        break;

                    case "sceneName":
                        mapData.sceneName = elem.InnerText;
                        break;

                    case "mainCamPos":
                        {
                            string[] valArr = elem.InnerText.Split(',');
                            mapData.mainCameraPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                        }
                        break;

                    case "mainCamRote":
                        {
                            string[] valArr = elem.InnerText.Split(',');
                            mapData.mainCameraRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                        }
                        break;

                    case "playerBornPos":
                        {
                            string[] valArr = elem.InnerText.Split(',');
                            mapData.playerBornPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                        }
                        break;

                    case "playerBornRote":
                        {
                            string[] valArr = elem.InnerText.Split(',');
                            mapData.playerBornRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                        }
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
           #region 地图中怪物数据处理
                    case "monsterLst":
                        {
                            string[] dataArr = elem.InnerText.Split('#');
                            for (int batchIdx = 0; batchIdx < dataArr.Length; batchIdx++)
                            {
                                if (batchIdx == 0)
                                {
                                    continue;
                                }

                                string[] subDataArr = dataArr[batchIdx].Split('|');
                                for (int idx = 0; idx < subDataArr.Length; idx++)
                                {
                                    if (idx == 0)
                                    {
                                        continue;
                                    }

                                    string[] itemArr = subDataArr[idx].Split(',');
                                    MonsterData monsterData = new MonsterData
                                    {
                                        ID = int.Parse(itemArr[0]),
                                        batchIndex = batchIdx,
                                        cfg = GetMonsterCfg(int.Parse(itemArr[0])),
                                        vecBornPos = new Vector3(float.Parse(itemArr[1]), float.Parse(itemArr[2]), float.Parse(itemArr[3])),
                                        vecRotate = new Vector3(0, float.Parse(itemArr[4]), 0),
                                        lv = int.Parse(itemArr[5])
                                    };
                                    mapData.monsterLst.Add(monsterData);
                                }
                            }
                        }
                        break;
                 #endregion

                }
            }
            #endregion
            //保存地图数据
            dictMapCfgData.Add(id, mapData);
        }
    }
    #endregion

    #region 解析自动引导任务配置文件
    private void InitAutoGuideCfgData(string xmlPath)
    {
        TextAsset xmlText = Resources.Load<TextAsset>(xmlPath);
        if (!xmlText)
        {
            CommonTools.Log("xml file: " + PathDefine.cPath_RandomNameCfg + "not exits!",
                LogType.LogType_Error);
            return;
        }

        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText.text);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            AutoGuideCfg guideData = new AutoGuideCfg
            {
                ID = id,
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                    case "npcID":
                        guideData.npcID = int.Parse(elem.InnerText);
                        break;

                    case "dilogArr":
                        guideData.dialogArr = elem.InnerText;
                        break;

                    case "actID":
                        guideData.targetTaskID = int.Parse(elem.InnerText);
                        break;

                    case "coin":
                        guideData.gainCoin = int.Parse(elem.InnerText);
                        break;

                    case "exp":
                        guideData.gainExp = int.Parse(elem.InnerText);
                        break;
                }
            }
            #endregion
            //保存地图数据
            dictAutoGuideCfgData.Add(id, guideData);
        }
    }

    /// <summary>
    /// 根据id获取自动任务引导数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public AutoGuideCfg GetAutoGuideCfgData(int id)
    {
        AutoGuideCfg data;
        if (dictAutoGuideCfgData.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }
    #endregion

    #region 解析强化配置文件
    private void InitStrongerCfgData(string xmlPath)
    {
        TextAsset xmlText = Resources.Load<TextAsset>(xmlPath);
        if (!xmlText)
        {
            CommonTools.Log("xml file: " + PathDefine.cPath_RandomNameCfg + "not exits!",
                LogType.LogType_Error);
            return;
        }

        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText.text);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            StrongerCfg strongerData = new StrongerCfg
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

            Dictionary<int, StrongerCfg> tmpDic = null;

            if (dictStrongerCfgData.TryGetValue(strongerData.stongerType, out tmpDic))
            {
                tmpDic.Add(strongerData.starLv, strongerData);
            }
            else
            {
                tmpDic = new Dictionary<int, StrongerCfg>();
                tmpDic.Add(strongerData.starLv, strongerData);
                dictStrongerCfgData.Add(strongerData.stongerType, tmpDic);
            }
        }
    }

    /// <summary>
    /// 获取强化数据
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="starLv">星级</param>
    /// <returns></returns>
    public StrongerCfg GetStrongerCfgData(int stongerType, int starLv)
    {
        StrongerCfg data = null;
        Dictionary<int, StrongerCfg> tmpDic = null;
        if (dictStrongerCfgData.TryGetValue(stongerType, out tmpDic))
        {
            if (tmpDic.ContainsKey(starLv))
            {
                data = tmpDic[starLv];
            }
        }

        return data;
    }

    /// <summary>
    /// 根据位置 星级 属性类型 获取 要增加的值
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="starLv"></param>
    /// <param name="propTye"></param>
    /// <returns></returns>
    public int GetPropsIncreaseValue(int stongerType, int starLv, Constants.PlayerPropType propType)
    {
        int resultVal = 0;
        Dictionary<int, StrongerCfg> dictStrongData = null;
        if (dictStrongerCfgData.TryGetValue(stongerType, out dictStrongData))
        {
            for (int i = 0; i < starLv; i++)
            {
                StrongerCfg tmpData;
                if (dictStrongData.TryGetValue(i, out tmpData))
                {
                    switch (propType)
                    {
                        case Constants.PlayerPropType.PropType_Hp: /*血量*/
                            resultVal += tmpData.addHp;
                            break;

                        case Constants.PlayerPropType.PropType_Hurt: /*伤害*/
                            resultVal += tmpData.addHurt;
                            break;

                        case Constants.PlayerPropType.PropType_Def: /*防御*/
                            resultVal += tmpData.addDef;
                            break;
                        
                    }
                }
            }
        }

        return resultVal;
    }
    #endregion

    #region 解析任务奖励配置文件

    private void InitTaskRewardCfgData(string xmlPath)
    {
        TextAsset xmlText = Resources.Load<TextAsset>(xmlPath);
        if (!xmlText)
        {
            CommonTools.Log("xml file: " + PathDefine.cPath_RandomNameCfg + "not exits!",
                LogType.LogType_Error);
            return;
        }

        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText.text);

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
                    case "taskName":
                        taskData.taskName = elem.InnerText;
                        break;

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

            dictTaskRewardCfgData.Add(id, taskData);
        }
    }

    public TaskRewardCfg GetTaskRewardCfgData(int id)
    {
        TaskRewardCfg data;
        if (dictTaskRewardCfgData.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }
    #endregion

    #region 技能配置文件

    private void InitSkillCfgData(string xmlPath)
    {
        TextAsset xmlText = Resources.Load<TextAsset>(xmlPath);
        if (!xmlText)
        {
            CommonTools.Log("xml file: " + PathDefine.cPath_RandomNameCfg + "not exits!",
                LogType.LogType_Error);
            return;
        }

        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText.text);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            SkillDataCfg skillData = new SkillDataCfg
            {
                ID = id,
                skillMoveList = new List<int>(),
                skillActionList = new List<int>(),
                skillDamageList = new List<int>()
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                    case "skillName":
                        skillData.skillName = elem.InnerText;
                        break;

                    case "cdTime":
                        skillData.cdTime = int.Parse(elem.InnerText);
                        break;

                    case "isCombo":
                        skillData.bCombo = elem.InnerText.Equals("1");
                        break;

                    case "isCollide":
                        skillData.bCollide = elem.InnerText.Equals("1");
                        break;

                    case "isBreak":
                        skillData.bBreak = elem.InnerText.Equals("1");
                        break;

                    case "skillTime":
                        skillData.skillTime = int.Parse(elem.InnerText);
                        break;

                    case "aniAction":
                        skillData.aniAction = int.Parse(elem.InnerText);
                        break;

                    case "fx":
                        skillData.fx = elem.InnerText;
                        break;

                    case "dmgType":
                        {
                            if(elem.InnerText.Equals("1"))
                            {
                                skillData.damageType = Constants.DamageType.DamageType_AD;
                            }
                            else if (elem.InnerText.Equals("2"))
                            {
                                skillData.damageType = Constants.DamageType.DamageType_AP;
                            }
                        }
                        
                        break;

                    case "skillMoveLst":
                        {
                            string[] skillList = elem.InnerText.Split('|');
                            for (int skillIdx = 0; skillIdx < skillList.Length; skillIdx++)
                            {
                                if (skillList[skillIdx] != "")
                                {
                                    skillData.skillMoveList.Add(int.Parse(skillList[skillIdx]));
                                }
                            }
                        }
                        break;

                    case "skillActionLst":
                        {
                            string[] skillActionList = elem.InnerText.Split('|');
                            for (int skillIdx = 0; skillIdx < skillActionList.Length; skillIdx++)
                            {
                                if (skillActionList[skillIdx] != "")
                                {
                                    skillData.skillActionList.Add(int.Parse(skillActionList[skillIdx]));
                                }
                            }
                        }
                        break;

                    case "skillDamageLst":
                        {
                            string[] damageList = elem.InnerText.Split('|');
                            for (int damagedx = 0; damagedx < damageList.Length; damagedx++)
                            {
                                if (damageList[damagedx] != "")
                                {
                                    skillData.skillDamageList.Add(int.Parse(damageList[damagedx]));
                                }
                            }
                        }
                        break;
                }
            }
            #endregion

            dictSkillCfgData.Add(id, skillData);
        }
    }

    public SkillDataCfg GetSkillCfg(int id)
    {
        SkillDataCfg skillData = null;
        if (dictSkillCfgData.TryGetValue(id, out skillData))
        {
            return skillData;
        }

        return null;
    }
    #endregion

    #region 技能移动配置文件
    private void InitSkillMoveCfgData(string xmlPath)
    {
        TextAsset xmlText = Resources.Load<TextAsset>(xmlPath);
        if (!xmlText)
        {
            CommonTools.Log("xml file: " + PathDefine.cPath_RandomNameCfg + "not exits!",
                LogType.LogType_Error);
            return;
        }

        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText.text);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            SkillMoveCfg skillData = new SkillMoveCfg
            {
                ID = id,
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                    case "moveTime":
                        skillData.moveTime = int.Parse(elem.InnerText);
                        break;

                    case "moveDis":
                        skillData.moveDistance = float.Parse(elem.InnerText);
                        break;

                    case "delayTime":
                        skillData.delayTime = int.Parse(elem.InnerText);
                        break;
                }
            }
            #endregion

            dictSkillMoveCfgData.Add(id, skillData);
        }
    }

    public SkillMoveCfg GetSkillMoveCfg(int id)
    {
        SkillMoveCfg skillData = null;
        if (dictSkillMoveCfgData.TryGetValue(id, out skillData))
        {
            return skillData;
        }

        return null;
    }

    #endregion

    #region 技能伤害数据配置文件
    private void InitSkillActionCfgData(string xmlPath)
    {
        TextAsset xmlText = Resources.Load<TextAsset>(xmlPath);
        if (!xmlText)
        {
            CommonTools.Log("xml file: " + PathDefine.cPath_RandomNameCfg + "not exits!",
                LogType.LogType_Error);
            return;
        }

        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText.text);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            SkillActionCfg skillData = new SkillActionCfg
            {
                ID = id,
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                    case "delayTime":
                        skillData.delayTime = int.Parse(elem.InnerText);
                        break;

                    case "radius":
                        skillData.radius = float.Parse(elem.InnerText);
                        break;

                    case "angle":
                        skillData.angle = float.Parse(elem.InnerText);
                        break;
                }
            }
            #endregion

            dictSkillActionCfgData.Add(id, skillData);
        }
    }

    public SkillActionCfg GetSkillActionCfg(int id)
    {
        SkillActionCfg skillData = null;
        if (dictSkillActionCfgData.TryGetValue(id, out skillData))
        {
            return skillData;
        }

        return null;
    }
    #endregion

    #region 怪物配置文件

    private void InitMonsterCfgData(string xmlPath)
    {
        TextAsset xmlText = Resources.Load<TextAsset>(xmlPath);
        if (!xmlText)
        {
            CommonTools.Log("xml file: " + PathDefine.cPath_RandomNameCfg + "not exits!",
                LogType.LogType_Error);
            return;
        }

        //开始解析xml文件
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText.text);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement element = nodeList[i] as XmlElement;

            if (element.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int id = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

            MonsterCfd monsterData = new MonsterCfd
            {
                ID = id,
                prop = new BattleProps()
            };

            #region 遍历解析
            foreach (XmlElement elem in nodeList[i].ChildNodes)
            {
                switch (elem.Name)
                {
                    case "mName":
                        monsterData.name = elem.InnerText;
                        break;

                    case "resPath":
                        monsterData.resPath = elem.InnerText;
                        break;

                    case "skillID":
                        monsterData.skillID = int.Parse(elem.InnerText);
                        break;

                    case "atkDis":
                        monsterData.atkDistance = float.Parse(elem.InnerText);
                        break;

                    case "isStop":
                        monsterData.bStop =  elem.InnerText.Equals("1");
                        break;

                    case "mType":
                        if (elem.InnerText.Equals("1"))
                        {
                            monsterData.monsterType = Constants.MonsterType.Normal;
                        }
                        else if (elem.InnerText.Equals("2"))
                        {
                            monsterData.monsterType = Constants.MonsterType.Boss;
                        }
                        break;

                    case "hp":
                        monsterData.prop.hp = int.Parse(elem.InnerText);
                        break;

                    case "ad":
                        monsterData.prop.ad = int.Parse(elem.InnerText);
                        break;

                    case "ap":
                        monsterData.prop.ap = int.Parse(elem.InnerText);
                        break;

                    case "addef":
                        monsterData.prop.addef = int.Parse(elem.InnerText);
                        break;

                    case "apdef":
                        monsterData.prop.apdef = int.Parse(elem.InnerText);
                        break;

                    case "dodge":
                        monsterData.prop.dodge = int.Parse(elem.InnerText);
                        break;

                    case "pierce":
                        monsterData.prop.pierce = int.Parse(elem.InnerText);
                        break;

                    case "critical":
                        monsterData.prop.critical = int.Parse(elem.InnerText);
                        break;
                }
            }
            #endregion

            dictMonsterCfgData.Add(id, monsterData);
        }
    }

    public MonsterCfd GetMonsterCfg(int id)
    {
        MonsterCfd monsterData = null;
        if (dictMonsterCfgData.TryGetValue(id, out monsterData))
        {
            return monsterData;
        }

        return null;
    }

    #endregion
}