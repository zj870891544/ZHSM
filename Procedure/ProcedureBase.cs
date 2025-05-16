//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ZHSM
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        // 获取流程是否使用原生对话框
        // 在一些特殊的流程（如游戏逻辑对话框资源更新完成前的流程）中，可以考虑调用原生对话框进行消息提示行为
        public abstract bool UseNativeDialog
        {
            get;
        }

        private IFsm<IProcedureManager> m_ProcedureOwner;

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);

            m_ProcedureOwner = procedureOwner;
        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            GameEntry.Event.Subscribe(NetworkChangeSceneEventArgs.EventId, OnNetworkChangeScene);
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            
            GameEntry.Event.Unsubscribe(NetworkChangeSceneEventArgs.EventId, OnNetworkChangeScene);
        }
        
        private void OnNetworkChangeScene(object sender, GameEventArgs e)
        {
            NetworkChangeSceneEventArgs ne = e as NetworkChangeSceneEventArgs;
            if (ne == null) return;
            
            ChangeScene(ne.SceneId);
        }
        
        protected void ChangeScene(int nextSceneId)
        {
            m_ProcedureOwner.SetData<VarInt32>("NextSceneId", nextSceneId);
            ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
        }
    }
}
