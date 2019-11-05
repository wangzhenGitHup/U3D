/****************************************************
    文件：LoopDragonAni.cs
	作者：WangZhen
    日期：2019/6/10 0:27:47
	功能：Nothing
*****************************************************/

using UnityEngine;

public class LoopDragonAni : MonoBehaviour 
{
    private Animation _animation;

    private void Awake()
    {
        _animation = transform.GetComponent<Animation>();
    }

    private void Start()
    {
        if (_animation != null)
        {
            InvokeRepeating("PlayDragonAni", 0, 20);
        }
    }

    private void PlayDragonAni()
    {
        if (_animation != null)
        {
            _animation.Play();
        }
    }
}