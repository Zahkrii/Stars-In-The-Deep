using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    internal class EntityBehaviour : LuaBehaviour
    {
        private Action luaOnStart;
        private Action luaOnHide;
        //private Action luaOnDestroy;

        public override void Init(string luaScript)
        {
            base.Init(luaScript);
            scriptEnv.Get("OnStart", out luaOnStart);
            scriptEnv.Get("OnHide", out luaOnHide);
        }

        public void OnStart()
        {
            luaOnStart?.Invoke();
        }

        public void OnHide()
        {
            luaOnHide?.Invoke();
        }

        protected override void Clear()
        {
            base.Clear();
            luaOnStart = null;
            luaOnHide = null;
        }
    }
}