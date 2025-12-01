using GWDataCenter.Database;

namespace GWDataCenter
{
    //外部设置模块接口
    public interface IExProcCmdHandle
    {
        bool init(GWExProcTableRow Row);
        void SetParm(string main_instruction, string minor_instruction, string value);
    }
}
