/****************************************************
    文件：AudioSvc.cs
	作者：WangZhen
    日期：2019/6/9 23:56:25
	功能：声音服务模块
*****************************************************/

using UnityEngine;

public class AudioSvc : MonoBehaviour 
{
    public AudioSource bgAudio;
    public AudioSource uiAudio;

    public static AudioSvc Instance = null;

    public void InitAudioSvc()
    {
        CommonTools.Log("Init AudioSvc...");
        Instance = this;
    }

    public void PlayBGM(string musicName, bool isLoop = true)
    {
        AudioClip audioClip = ResSvc.Instance.LoadAudio("ResAudio/" + musicName, true);
        if (bgAudio.clip == null || bgAudio.clip.name != audioClip.name)
        {
            bgAudio.clip = audioClip;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    //ui音效
    public void PlayUIAudio(string musicName)
    {
        AudioClip audioClip = ResSvc.Instance.LoadAudio("ResAudio/" + musicName, true);
        uiAudio.clip = audioClip;
        uiAudio.Play();
    }

    public void PlayPlayerAudio(string musicNmae, AudioSource audioSrc)
    {
        AudioClip audioClip = ResSvc.Instance.LoadAudio("ResAudio/" + musicNmae, true);
        audioSrc.clip = audioClip;
        audioSrc.Play();
    }

    public void StopBGM()
    {
        if (bgAudio != null)
        {
            bgAudio.Stop();
        }
    }
}