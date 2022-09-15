using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Managers
{
    public class EventManager : MonoBehaviour
    {
        public delegate void EventHandler(object args);

        private Dictionary<uint, EventHandler> events = new Dictionary<uint, EventHandler>();

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="e"></param>
        public void Subscribe(uint id, EventHandler e)
        {
            if (events.ContainsKey(id))
                events[id] += e;
            else
                events.Add(id, e);
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="e"></param>
        public void Unsubscribe(uint id, EventHandler e)
        {
            if (events.ContainsKey(id))
            {
                if (events[id] != null)
                    events[id] -= e;
                else//if (events[id] == null)
                    events.Remove(id);
            }
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        public void Execute(uint id, object args = null)
        {
            EventHandler eventHandler;
            if (events.TryGetValue(id, out eventHandler))
                eventHandler(args);
        }
    }
}