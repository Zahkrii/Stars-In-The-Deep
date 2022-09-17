using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[XLua.LuaCallCSharp]
public static class UnityExtension
{
    /// <summary>
    /// UnityEngine UI �� button ����¼���չ��ע�� this �ؼ��֣���չ�����ܹ����������͡����ӡ��������������� Button �������� OnClickEvent ����
    /// </summary>
    /// <param name="button"></param>
    /// <param name="callback"></param>
    public static void OnClickEvent(this Button button, object callback)
    {
        XLua.LuaFunction luaFunction = callback as XLua.LuaFunction;
        button.onClick.RemoveAllListeners();
        //�����������luaת�Ƶ�C#���������������luaEnv�ͷ�ʱ�ı���
        button.onClick.AddListener(() =>
        {
            luaFunction?.Call();
        });
    }

    /// <summary>
    /// slider OnValueChanged ��չ����
    /// </summary>
    /// <param name="slider"></param>
    /// <param name="callback"></param>
    public static void OnValueChangedEvent(this Slider slider, object callback)
    {
        XLua.LuaFunction luaFunction = callback as XLua.LuaFunction;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((float value) =>
        {
            luaFunction?.Call(value);
        });
    }
}