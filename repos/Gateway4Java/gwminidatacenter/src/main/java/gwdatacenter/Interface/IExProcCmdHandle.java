package gwdatacenter.Interface;

import gwdatacenter.database.*;

//外部设置模块接口
public interface IExProcCmdHandle
{
	boolean init(GWExProcTableRow Row);
	void SetParm(String main_instruction, String minor_instruction, String value);
}