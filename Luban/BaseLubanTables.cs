using System;
using System.Threading.Tasks;
using Luban;
using SimpleJSON;
using UnityEngine;

namespace ZHSM
{
    public class BaseLubanTables : ILubanTables
    {
        public virtual LubanTableType GetTableType { get; }
        
        public virtual Task LoadAsync(Func<string, Task<ByteBuf>> loader)
        {
            return null;
        }

        public virtual Task LoadAsync(Func<string, Task<JSONNode>> loader)
        {
            Debug.Log("load async >>>>>>>>>>>>>>>>>>> ");
            return null;
        }

        public virtual void TranslateText(Func<string, string, string> translator)
        {
            
        }
    }
}

