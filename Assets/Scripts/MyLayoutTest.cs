using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

/*
 * 2023 5 10
 *  实现了一个简单的案例，在一个水平布局组件下包含有一张图片和一个文本
 *  目标是测试当文本内容发生修改时整个布局组的自适应布局情况
 *  在修改文本后的更新方式如下：
 *  1. 在至少一帧后将文本SetDirty（视觉上有明显延迟感）
 *  2. 以文本为根节点LayoutRebuilder.ForceRebuildLayoutImmediate
 *  3. 以布局组为根节点LayoutRebuilder.ForceRebuildLayoutImmediate
 *  其中对于23测试并不全面，都可以实现更新布局的效果，但当前结构比较简单，后续考虑测试嵌套布局组
 *
 *  更新嵌套布局组
 *  LayoutRebuilder.ForceRebuildLayoutImmediate 只更新到最近的布局组
 *
 *  考虑问题
 *  1. 是否是Content Size Filter使用不正确
 *     
 */
public class MyLayoutTest : MonoBehaviour
{
    public TextMeshProUGUI MyText;

    public string targetText = "";

    public Button ResetButton;
    public Button UpdateButton;
    public Button RectButton;
    public Button TextButton;
    public Button RenderButton;

    private void Start()
    {
        MyText.text = "abccba";
        
        ResetButton.SetTMPText("Reset");
        ResetButton.onClick.AddListener(Reset);
        
        UpdateButton.SetTMPText("Update");
        UpdateButton.onClick.AddListener(UpdateContent);
        
        RectButton.SetTMPText("Update-Rect");
        RectButton.onClick.AddListener(UpdateAndForceRebuildRect);
        
        TextButton.SetTMPText("Update-Text");
        TextButton.onClick.AddListener(UpdateAndForceRebuildText);
        
        RenderButton.SetTMPText("Render");
        RenderButton.onClick.AddListener(Rerender);
    }

    public void Reset()
    {
        MyText.text = "abccba";
        StartCoroutine(IERerender());
    }
    
    public void UpdateContent()
    {
        MyText.text = targetText;
        MyText.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        GetComponent<RectTransform>().parent.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
    }

    public void UpdateAndForceRebuildRect()
    {
        MyText.text = targetText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void UpdateAndForceRebuildText()
    {
        MyText.text = targetText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(MyText.GetComponent<RectTransform>());
    }

    public void Rerender()
    {
        MyText.SetLayoutDirty();
    }

    public IEnumerator IERerender()
    {
        yield return new WaitForEndOfFrame();
        MyText.SetLayoutDirty();
    }
}

public static class ButtonExt
{
    public static void SetTMPText(this Button btn, string text)
    {
        btn.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}