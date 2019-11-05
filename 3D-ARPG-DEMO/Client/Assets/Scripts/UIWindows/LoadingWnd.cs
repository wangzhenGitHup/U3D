/****************************************************
    文件：LoadingWnd.cs
	作者：WangZhen
    日期：2019/6/9 22:36:29
	功能：加载进度界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;


public class LoadingWnd : WindowRoot
{
    public Text txtTips; /*进度条提示语句*/
    public Image imgFg; /*进度条前景图片*/
    public Image imgProgressPoint; /*进度光点图片*/
    public Text txtProgress; /*进度值*/

    private float fImgFgWidth; /*进度条前景图片的宽度*/
    private const float offsetX = 545.0f;


    protected override void InitWnd()
    {
        base.InitWnd();

        SetText(txtTips, "这是一条游戏tips");
        SetText(txtProgress, "0");

        imgFg.fillAmount = 0;
        fImgFgWidth = imgFg.GetComponent<RectTransform>().sizeDelta.x;
        imgProgressPoint.transform.localPosition = new Vector3(-offsetX, 0, 0);
    }

    public void SetProgress(float fProgress)
    {
        SetText(txtProgress, (int)(fProgress * 100) + "%");
        imgFg.fillAmount = fProgress;

        float curPosX = fProgress * fImgFgWidth - offsetX;
        imgProgressPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(curPosX, 0);
    }
}