using System.Threading.Tasks;
using Luban;
using SimpleJSON;

namespace ZHSM
{
    public interface ILubanTables
    {
        LubanTableType GetTableType { get; }
        Task LoadAsync(System.Func<string, Task<ByteBuf>> loader);
        Task LoadAsync(System.Func<string, Task<JSONNode>> loader);
        void TranslateText(System.Func<string, string, string> translator);
    }
}


