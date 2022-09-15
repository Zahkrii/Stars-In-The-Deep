using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[XLua.LuaCallCSharp]
public static class UnityExtension
{
    /// <summary>
    /// UnityEngine UI 的 button 点击事件扩展。注意 this 关键字，扩展方法能够向现有类型“添加”方法，这里是向 Button 类型添加 OnClickEvent 方法
    /// </summary>
    /// <param name="button"></param>
    /// <param name="callback"></param>
    public static void OnClickEvent(this Button button, object callback)
    {
        XLua.LuaFunction luaFunction = callback as XLua.LuaFunction;
        button.onClick.RemoveAllListeners();
        //将监听对象从lua转移到C#的匿名方法，解决luaEnv释放时的报错
        button.onClick.AddListener(() =>
        {
            luaFunction?.Call();
        });
    }

    /// <summary>
    /// slider OnValueChanged 扩展方法
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