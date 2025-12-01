using System.Data;
using GWDataCenter;
using GWDataCenter.Database;

namespace BCDataSimu.STD;

/// <summary>
/// 设备通讯类
/// 类的名称一定要是“CEquip",而且必须从CEquipBase 派生
/// </summary>
class CEquip : CEquipBase
{
    public CEquip():base()
    {
    }

    /// <summary>
    /// 获取设备数据
    /// </summary>
    /// <returns></returns>
    int iCounttemp = 0;
    public override CommunicationState GetData(CEquipBase pEquip)
    {
        Sleep(1000);
        if (RunSetParmFlag)
        {
            return CommunicationState.setreturn;
        }

        var commState = base.GetData(pEquip);
        if (commState != CommunicationState.ok)
        {
            return commState;
        }
        
        if (!pEquip.GetEvent())
            return CommunicationState.fail;
        return CommunicationState.ok;
    }

    public override bool GetYC(YcpTableRow r)
    {
        base.SetYCData(r, Random.Shared.Next((int)r.val_min, (int)r.val_max));
        return true;
    }

    public override bool GetYX(YxpTableRow r)
    {
        base.SetYXData(r, Random.Shared.Next(0, 1));
        return true;
    }

    //模拟一些数据
    public override bool SetParm(string MainInstruct, string MinorInstruct, string Value)
    {
        try
        {
            if (MainInstruct.Equals("SetYCYXValue", StringComparison.OrdinalIgnoreCase))//可以强制设置YCYX的值
            {
                if (MinorInstruct.Length > 2)//e.g. MinorInstruct=C_2 0r X_15 
                {
                    if (string.IsNullOrEmpty(Value))
                        return false;
                    int ycyxno = Convert.ToInt32(MinorInstruct.Substring(2));
                    if (ycyxno > 0)
                    {
                        if (MinorInstruct[0] == 'C' || MinorInstruct[0] == 'c')//表示设置YC值
                        {
                            lock (YCResults)
                            {
                                YCResults[ycyxno] = Convert.ToDouble(Value);
                            }
                            return true;
                        }
                        if (MinorInstruct[0] == 'X' || MinorInstruct[0] == 'x')//表示设置YX值
                        {
                            lock (YXResults)
                            {
                                bool v = Convert.ToInt32(Value) > 0;
                                YXResults[ycyxno] = v;
                            }
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        catch (Exception e)
        {
            DataCenter.WriteLogFile(e.ToString());
            return false;
        }
    }

    public override bool Confirm2NormalState(string sYcYxType, int iYcYxNo)
    {
        return true;
    }
}