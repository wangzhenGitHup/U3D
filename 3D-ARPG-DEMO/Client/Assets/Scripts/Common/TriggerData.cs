/****************************************************
    �ļ���TriggerData.cs
	���ߣ�WangZhen
    ���ڣ�2019/7/1 21:54:56
	���ܣ���ͼ��������
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerData : MonoBehaviour {
    public MapManager _mapMgr;
    public int _triggerBatch; /*������һ���ι���*/

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
