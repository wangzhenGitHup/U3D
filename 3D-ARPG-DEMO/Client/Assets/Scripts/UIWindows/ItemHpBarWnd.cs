/****************************************************
    文件：ItemHpBarWnd.cs
	作者：WangZhen
    日期：2019/6/27 23:34:30
	功能：血条
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class ItemHpBarWnd : MonoBehaviour 
{
    public Image imgHpGray;
    public Image imgHpRed;
    public Animation aniCritical;
    public Text txtCritical;
    public Animation aniDodge;
    public Text txtDodge;
    public Animation aniHp;
    public Text txtHp;

    private RectTransform rect;
    private Transform rootTrans;
    private int hpVal;
    private float scaleRate = 1.0f  * Constants.cScreenStandardHeight / Screen.height;
    private float currentProgress;
    private float targetProgress;

    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(rootTrans.transform.position);
        rect.anchoredPosition = screenPos * scaleRate;

        UpdateMixBlend();
        imgHpGray.fillAmount = currentProgress;
    }

    private void UpdateMixBlend()
    {
        if (Mathf.Abs(currentProgress - targetProgress) < Constants.cHPSpd * Time.deltaTime)
        {
            currentProgress = targetProgress;
        }
        else if (currentProgress > targetProgress)
        {
            currentProgress -= Constants.cHPSpd * Time.deltaTime;
        }
        else
        {
            currentProgress += Constants.cHPSpd * Time.deltaTime;
        }
    }

    public void SetInfo(int hp, Transform trans)
    {
        rect = transform.GetComponent<RectTransform>();
        rootTrans = trans;
        hpVal = hp;
        imgHpGray.fillAmount = 1;
        imgHpRed.fillAmount = 1;
    }

    public void SetCriticalVal(int val)
    {
        aniCritical.Stop();
        txtCritical.text = "暴击 +" + val;
        aniCritical.Play();
    }

    public void SetDodgeVal()
    {
        aniDodge.Stop();
        aniDodge.Play();
    }

    public void SetHurtVal(int val)
    {
        aniHp.Stop();
        txtHp.text = "-" + val;
        aniHp.Play();
    }

    public void SetHpVal(int oldVal, int newVal)
    {
        currentProgress = oldVal * 1.0f / hpVal;
        targetProgress = newVal * 1.0f / hpVal;
        imgHpRed.fillAmount = targetProgress;
    }
}