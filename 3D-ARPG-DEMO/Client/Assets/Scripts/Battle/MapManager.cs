/****************************************************
    文件：MapManager.cs
	作者：WangZhen
    日期：2019/6/22 23:35:2
	功能：地图管理
*****************************************************/

using UnityEngine;

public class MapManager : MonoBehaviour 
{
    public TriggerData[] _triggerArr;

    BattleManager battleMgr;
    int monsterBatchIdx = 1; /*怪物批次序号*/

    public void Init(BattleManager mgr)
    {
        CommonTools.Log("MapManager Init.....");
        battleMgr = mgr;

        //实例化第一批怪物
        battleMgr.LoadMonsterByBatchID(monsterBatchIdx);
    }

    public void TriggerMonsterBorn(TriggerData triggerData, int batchIdx)
    {
        if (battleMgr != null)
        {
            battleMgr.LoadMonsterByBatchID(batchIdx);
            battleMgr.ActiveCurrentBatchMonsters();
            battleMgr._bTriggerCheck = true;
            BoxCollider box = triggerData.GetComponent<BoxCollider>();
            if (box)
            {
                box.isTrigger = false;
            }
        }
    }

    public bool OpenNextTrigger()
    {
        monsterBatchIdx += 1;
        for (int i = 0; i < _triggerArr.Length; i++)
        {
            if (_triggerArr[i]._triggerBatch == monsterBatchIdx)
            {
                BoxCollider box = _triggerArr[i].GetComponent<BoxCollider>();
                if (box)
                {
                    box.isTrigger = true;
                    return true;
                }
            }
        }

        return false;
    }
}