/****************************************************
    文件：TriggerData.cs
	作者：WangZhen
    日期：2019/7/1 21:54:56
	功能：地图触发数据
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerData : MonoBehaviour {
    public MapManager _mapMgr;
    public int _triggerBatch; /*触发哪一批次怪物*/

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (_mapMgr != null)
            {
                _mapMgr.TriggerMonsterBorn(this, _triggerBatch);
            }
        }
    }
}
