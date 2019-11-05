/****************************************************
    文件：MonsterController.cs
	作者：WangZhen
    日期：2019/6/25 22:48:5
	功能：怪物控制器
*****************************************************/

using UnityEngine;

public class MonsterController : NPCController 
{
    private void Update()
    {
        //AI逻辑表现
        if (_bIsCurMove)
        {
            SetCurDirection();
            SetMove();
        }
    }

    private void SetCurDirection()
    {
        float angle = Vector2.SignedAngle(CurDirection, new Vector2(0, 1));
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    private void SetMove()
    {
        _playerCtrl.Move(transform.forward * Time.deltaTime * Constants.cNPCMoveSpeed);
        //给定一个向下的速度，便于在没有apply root时 怪物可以落地【资源问题】
        _playerCtrl.Move(Vector3.down * Time.deltaTime * Constants.cNPCMoveSpeed);
    }
}