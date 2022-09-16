using Framework.Managers;
using System;

namespace Framework
{
    public class UIBehaviour : LuaBehaviour
    {
        public string AssetName;

        //UI´ò¿ª
        private Action luaOnOpen;

        //UI¹Ø±Õ
        private Action luaOnClose;

        public override void Init(string luaScript)
        {
            base.Init(luaScript);
            scriptEnv.Get("OnOpen", out luaOnOpen);
            scriptEnv.Get("OnClose", out luaOnClose);
        }

        public void OnOpen()
        {
            luaOnOpen?.Invoke();
        }

        public void OnClose()
        {
            luaOnClose?.Invoke();
            Manager.PoolManager.Recycle("UI", AssetName, this.gameObject);
        }

        protected override void Clear()
        {
            base.Clear();
            luaOnOpen = null;
            luaOnClose = null;
        }
    }
}