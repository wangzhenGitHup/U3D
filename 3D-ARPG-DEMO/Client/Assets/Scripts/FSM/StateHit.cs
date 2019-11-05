/****************************************************
    文件：StateHit.cs
	作者：WangZhen
    日期：2019/6/27 22:48:19
	功能：受击状态
*****************************************************/

using UnityEngine;

public class StateHit : IState 
{
    public void EnterState(EntityBase entity, params object[] args)
    {
        entity._currentPlayerAniState = PlayerAniState.Hit;
        entity.RemoveSkillCallback();
    }

    public void DoState(EntityBase entity, params object[] args)
    {
        if (entity._entityType == Constants.EntityType.Player)
        {
            entity._bCanReleaseSkill = false;
            AudioSource playerAudio = entity.GetAudioSource();
            AudioSvc.Instance.PlayPlayerAudio(Constants.cHitSound, playerAudio);
        }

        entity.SetDirection(Vector2.zero);
        entity.SetAction(Constants.cActionHit);

        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            entity.SetAction(Constants.cActionDefault);
            entity.Idle();
        }, (int)(GetHitActionTimes(entity) * 1000));
    }

    public void ExitState(EntityBase entity, params object[] args)
    {

    }

    private float GetHitActionTimes(EntityBase entity)
    {
        AnimationClip[] clips = entity.GetAnimationClips();
        for (int i = 0; i < clips.Length; i++)
        {
            string clipName = clips[i].name;
            if (clipName.Contains("hit") || 
                clipName.Contains("Hit") || 
                clipName.Contains("HIT"))
            {
                return clips[i].length;
            }
        }
        return 1.0f;
    }
}