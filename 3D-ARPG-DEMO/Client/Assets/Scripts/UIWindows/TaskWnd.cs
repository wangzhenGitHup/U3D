/****************************************************
    文件：TaskWnd.cs
	作者：WangZhen
    日期：2019/6/21 7:45:36
	功能：任务功能
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using Protocols;
using System.Collections.Generic;

public class TaskWnd : WindowRoot 
{
    public Transform scrollViewTrans;

    private PlayerData playerData = null;
    private List<TaskData> listTaskData = new List<TaskData>();

    protected override void InitWnd()
    {
        base.InitWnd();

        playerData = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (!GetWndState())
        {
            return;
        }

        listTaskData.Clear();

        List<TaskData> noRecieveList = new List<TaskData>();
        List<TaskData> finishList = new List<TaskData>();

        for (int i = 0; i < playerData.taskReward.Length; i++)
        {
            string[] taskInfo = playerData.taskReward[i].Split('|');
            TaskData tmpData = new TaskData
            {
                ID = int.Parse(taskInfo[0]),
                progress = int.Parse(taskInfo[1]),
                bTasked = taskInfo[2].Equals("1")
            };

            if (tmpData.bTasked)
            {
                finishList.Add(tmpData);
            }
            else
            {
                noRecieveList.Add(tmpData);
            }
        }

        listTaskData.AddRange(noRecieveList);
        listTaskData.AddRange(finishList);
        ClearChildItem();

        for (int i = 0; i < listTaskData.Count; i++)
        {
            GameObject objItem = _resSvc.LoadPrefab(PathDefine.cTaskItem);
            objItem.transform.SetParent(scrollViewTrans);
            objItem.transform.localPosition = Vector3.zero;
            objItem.transform.localScale = Vector3.one;
            objItem.name = "taskItem_" + i;

            TaskData taskData = listTaskData[i];
            TaskRewardCfg taskCfdData = _resSvc.GetTaskRewardCfgData(taskData.ID);

            SetText(GetTransformItem(objItem.transform, "txtName"), taskCfdData.taskName);
            SetText(GetTransformItem(objItem.transform, "txtPrg"), taskData.progress  + "/" + taskCfdData.count);
            SetText(GetTransformItem(objItem.transform, "txtExp"), "奖励：    经验" + taskCfdData.exp);
            SetText(GetTransformItem(objItem.transform, "txtCoin"), "金币" +taskCfdData.coin);

            Image imgProgress = GetTransformItem(objItem.transform, "prgBar/prgVal").GetComponent<Image>();
            float progressVal = taskData.progress * 1.0f / taskCfdData.count;
            imgProgress.fillAmount = progressVal;


            Button btnTake = GetTransformItem(objItem.transform, "btnTake").GetComponent<Button>();
            btnTake.onClick.AddListener(() =>
            {
                OnClickTake(objItem.name);
            });

            Transform compTrans = GetTransformItem(objItem.transform, "imgComp");
            if (taskData.bTasked)
            {
                btnTake.interactable = false;
                SetActive(compTrans);
            }
            else
            {
                SetActive(compTrans, false);
                if (taskData.progress == taskCfdData.count)
                {
                    btnTake.interactable = true;
                }
                else
                {
                    btnTake.interactable = false;
                }
            }
        }
    }

    private void ClearChildItem()
    {
        for (int i = 0; i < scrollViewTrans.childCount; i++)
        {
            Destroy(scrollViewTrans.GetChild(i).gameObject);
        }
    }
    private void OnClickTake(string name)
    {
        string[] nameArr = name.Split('_');
        int taskIdx = int.Parse(nameArr[1]);
        NetMsg netMsg = new NetMsg
        {
            cmd = (int)MsgCommand.Cmd_ReqTaskReward,
            reqTaskReward = new RequestTaskReward
            {
                taskId = listTaskData[taskIdx].ID,
            }
        };
        _netSvc.SendMsg(netMsg);

        TaskRewardCfg taskDataCfd = _resSvc.GetTaskRewardCfgData(listTaskData[taskIdx].ID);
        int coin = taskDataCfd.coin;
        int exp = taskDataCfd.exp;

        GameRoot.AddTips(Constants.Color("获得奖励：", Constants.TextColor.Blue) + Constants.Color("金币 + " + coin + "经验 +" + exp, Constants.TextColor.Green));
    }

    public void OnClickClose()
    {
        _audioSvc.PlayUIAudio(Constants.cUIClosedBtnSound);
        SetWndState(false);
    }
}