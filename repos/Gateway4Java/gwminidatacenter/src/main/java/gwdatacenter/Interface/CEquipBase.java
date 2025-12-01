package gwdatacenter.Interface;

import gwdatacenter.*;
import gwdatacenter.args.EquipEvent;
import gwdatacenter.communication.SerialPort;
import gwdatacenter.database.*;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;

/** 
 设备动态库的基类，派生于IEquip
*/
public class CEquipBase implements IEquip
{
	private int iSta_no, iEquip_no, iRetrytime;
	private boolean b = true;
	private EquipItem myequipitem;
	public ArrayList<YcpTableRow> ycprows;
	public ArrayList<YxpTableRow> yxprows;
	public EquipTableRow equiprow;
	public String EquipNm;

	public SerialPort serialport;

	private ConcurrentHashMap<Integer, Object> ycresults;
	private ConcurrentHashMap<Integer, Object> yxresults;
	private ArrayList<EquipEvent> equipEventlist;

	private Object runSetParmFlag = (Object)false;
	private Object resetFlag = (Object)false;

	public HashMap<Integer, Boolean> ycpdataflag = new HashMap<Integer, Boolean>();
	public HashMap<Integer, Boolean> yxpdataflag = new HashMap<Integer, Boolean>();

	public String setparmexecutor = null;

	private boolean bSetCacheData = false; //设备是否缓存标记，缓存意味着实时数据会记录到数据库，作为重启系统的初始数据
	private boolean bFirstGetData = true; //第一次调用GetData
	public final boolean getRunSetParmFlag()
	{
		synchronized (runSetParmFlag)
		{
			return (Boolean)runSetParmFlag;
		}
	}
	public final void setRunSetParmFlag(boolean value)
	{
		synchronized (runSetParmFlag)
		{
			runSetParmFlag = (Object)(Boolean)value;
		}
	}

	public final boolean getResetFlag()
	{
		synchronized (resetFlag)
		{
			return (Boolean)resetFlag;
		}
	}
	public final void setResetFlag(boolean value)
	{
		synchronized (resetFlag)
		{
			resetFlag = (Object)(Boolean)value;
		}
	}

	public final EquipItem getEquipitem()
	{
		return myequipitem;
	}
	public final void setEquipitem(EquipItem value)
	{
		myequipitem = value;
		serialport = myequipitem.serialport;
	}

//	public final EquipItem getEquipitem()
//	{
//		return DataCenter.GetEquipItem(iEquip_no);
//	}

	public final ConcurrentHashMap<Integer, Object> getYCResults()
	{
		return ycresults;
	}
	public final ConcurrentHashMap<Integer, Object> getYXResults()
	{
		return yxresults;
	}
	public final ArrayList<EquipEvent> getEquipEventList()
	{
		return equipEventlist;
	}

	public final int getMStaNo()
	{
		return iSta_no;
	}
	public final void setMStaNo(int value)
	{
		iSta_no = value;
	}

	public final int getMEquipNo()
	{
		return iEquip_no;
	}
	public final void setMEquipNo(int value)
	{
		iEquip_no = value;
	}

	public final int getMRetrytime()
	{
		return iRetrytime;
	}
	public final void setMRetrytime(int value)
	{
		iRetrytime = value;
	}

	public final boolean getBCanConfirm2NormalState()
	{
		return getEquipitem().getBCanConfirm2NormalState();
	}
	public final void setBCanConfirm2NormalState(boolean value)
	{
		getEquipitem().setBCanConfirm2NormalState(value);
	}

	public final String getSetParmExecutor()
	{
		return setparmexecutor;
	}
	public final void setParmExecutor(String value)
	{
		setparmexecutor = value;
	}

	public CEquipBase()
	{
		ycresults = new ConcurrentHashMap<>();
		yxresults = new ConcurrentHashMap<>();
		equipEventlist = new ArrayList<EquipEvent>();

		iRetrytime = 3; //默认通讯失败重试次数为3
	}

	/** 
	 
	 在调用GetData 之前调用
	 
	 @param item
	 @return 
	*/
	public boolean init(EquipItem item)
	{
		if (item == null)
		{
			DataCenter.WriteLogFile("CEquipBase调用init(EquipItem item)时，item==null)");
			return false;
		}
		iSta_no = item.getIStano();
		iEquip_no = item.getIEquipno();
		setMRetrytime(serialport.getCommFaultReTryTime());
		bSetCacheData = item.getBSetCacheData();
		// 设备第一次运行时调用;
		//或者数据库配置进行了更改后调用
		if (b || getResetFlag())
		{
			myequipitem = item;
			equiprow = StationItem.getDbEqp().stream().filter(m -> m.getEquipNo() == iEquip_no).findFirst().orElse(null);
			if (equiprow == null)
			{
				return false;
			}

			var rs = new ArrayList<>(StationItem.getDbYcp().stream().filter(m -> m.getEquipNo() == iEquip_no).toList());
			if (!rs.isEmpty())
			{
				ycprows = rs;
			}
			else
			{
				ycprows = null;
			}

			ArrayList<YxpTableRow> rs1 = new ArrayList<>(StationItem.getDbYxp().stream().filter(m -> m.getEquipNo() == iEquip_no).toList());
			if (!rs1.isEmpty())
			{
				yxprows = rs1;
			}
			else
			{
				yxprows = null;
			}

			EquipNm = equiprow.getEquipNm();
			b = false;
			OnLoaded();
		}
		return true;
	}

	/** 
	 休眠
	 
	 @param t 休眠时间，单位：毫秒
	*/

	public final void Sleep(int t)
	{
		Sleep(t, true);
	}

	/**
	 休眠

	 @param t 休眠时间，单位：毫秒
	 @param bBreak 当设置事件发生时，是否中断。在设置函数中休眠，不要中断。
	 */
	public final void Sleep(int t, boolean bBreak) {
		try {
			if (!getEquipitem().IsRageMode) //非急速模式，正常休眠，避免海量线程场景下急剧的资源消耗（10000线程可能会卡死）
			{
				Thread.sleep(t);
				return;
			} else //急速模式慎用，
			{
				if (!bBreak) {
					Thread.sleep(t);
					return;
				}
				int count = t / 10;
				for (int k = 0; k < count; k++) {
					if (getRunSetParmFlag()) {
						break;
					}
					Thread.sleep(10);
				}
			}
		} catch (InterruptedException e) {

		}
	}
	/** 
	 获取设备数据
	 
	 @return 
	*/

	public CommunicationState GetData(CEquipBase pEquip)
	{
		try
		{
			if (getRunSetParmFlag())
			{
				return CommunicationState.setreturn;
			}
			if (ycprows != null)
			{
				for (YcpTableRow r : ycprows)
				{
					/**if have setparm, immediately return              
					*/
					if (getRunSetParmFlag())
					{
						return CommunicationState.setreturn;
					}
					else
					{
						if (pEquip.GetYC(r))
						{
						}
						else
						{
							return CommunicationState.fail;
						}
					}
				}
			}
			if (yxprows != null)
			{
				for (YxpTableRow r : yxprows)
				{
					if (getRunSetParmFlag())
					{
						return CommunicationState.setreturn;
					}
					else
					{
						if (pEquip.GetYX(r))
						{
						}
						else
						{
							return CommunicationState.fail;
						}
					}
				}
			}
			if (!pEquip.GetEvent())
			{
				return CommunicationState.fail;
			}
			if (bFirstGetData)
			{
				bFirstGetData = false;
			}
		}
		catch (RuntimeException e)
		{
			DataCenter.WriteLogFile(General.GetExceptionInfo(e));
			return CommunicationState.fail;
		}
		return CommunicationState.ok;
	}

	public boolean OnLoaded()
	{
		return true;
	}

	public boolean GetYC(YcpTableRow r)
	{
		return false;
	}

	public boolean GetYX(YxpTableRow r)
	{
		return false;
	}

	public boolean GetEvent()
	{
		return true;
	}

	/** 
	 
	 
	 @param MainInstruct
	 @param MinorInstruct
	 @param Value
	 @return 
	*/
	public boolean SetParm(String MainInstruct, String MinorInstruct, String Value)
	{
		return false;
	}

	public boolean Confirm2NormalState(String sYcYxType, int iYcYxNo)
	{
		return false;
	}

	public boolean CloseCommunication()
	{
		return true;
	}

	public final void YCToPhysic(YcpTableRow r)
	{
		return;
	}
	public final void YXToPhysic(YxpTableRow r)
	{
		return;
	}
	/** 
	 设置YC点为未取值状态
	 
	 @param Rows
	*/
	public final void SetYCDataNoRead(ArrayList<YcpTableRow> Rows)
	{
		if (Rows == null || Rows.isEmpty())
		{
			return;
		}
		for (YcpTableRow r : Rows)
		{
			int iycno = r.getYcNo();
			if (!ycpdataflag.containsKey(iycno))
			{
				ycpdataflag.put(iycno, false);
			}
			else
			{
				ycpdataflag.put(iycno, false);
			}
		}
	}
	/** 
	 设置YX点为未取值状态
	 
	 @param Rows
	*/
	public final void SetYXDataNoRead(ArrayList<YxpTableRow> Rows)
	{
		if (Rows == null || Rows.isEmpty())
		{
			return;
		}

		for (YxpTableRow r : Rows)
		{
			int iyxno = r.getYxNo();
			if (!yxpdataflag.containsKey(iyxno))
			{
				yxpdataflag.put(iyxno, false);
			}
			else
			{
				yxpdataflag.put(iyxno, false);
			}
		}
	}
	public final void SetYcpTableRowData(YcpTableRow r, String o)
	{
		SetYCData(r, o);
	}
	public final void SetYcpTableRowData(YcpTableRow r, int o)
	{
		SetYCData(r, o);
	}
	public final void SetYcpTableRowData(YcpTableRow r, float o)
	{
		SetYCData(r, o);
	}

	public final void SetYcpTableRowData(YcpTableRow r, double o)
	{
		SetYCData(r, o);
	}

	// TODO：Java自带SDK中没有Tuple的同义类，需引用其他包，暂搁置
//	public final void SetYcpTableRowData(YcpTableRow r, ValueTuple<LocalDateTime, Double> o)
//	{
//		SetYCData(r, o);
//	}
//	public final void SetYcpTableRowData(YcpTableRow r, ValueTuple<Double, Double> o)
//	{
//		SetYCData(r, o);
//	}
//	public final void SetYcpTableRowData(YcpTableRow r, ValueTuple<Double, Double, Double> o)
//	{
//		SetYCData(r, o);
//	}
//	public final void SetYcpTableRowData(YcpTableRow r, ValueTuple<Double, Double, Double, Double> o)
//	{
//		SetYCData(r, o);
//	}
//	public final void SetYcpTableRowData(YcpTableRow r, ValueTuple<Double, Double, Double, Double, Double> o)
//	{
//		SetYCData(r, o);
//	}
//	public final void SetYcpTableRowData(YcpTableRow r, ValueTuple<Double, Double, Double, Double, Double, Double> o)
//	{
//		SetYCData(r, o);
//	}
//	public final void SetYcpTableRowData(YcpTableRow r, ValueTuple<Double, Double, Double, Double, Double, Double, Double> o)
//	{
//		SetYCData(r, o);
//	}

	public final void SetYCData(YcpTableRow r, Object o)
	{
		int iycno = r.getYcNo();
		synchronized (getYCResults())
		{
			if (!getYCResults().containsKey(iycno))
			{
				getYCResults().put(iycno, o);
			}
			else
			{
				getYCResults().replace(iycno, o);
			}
		}
	}

	public final Object GetYCData(YcpTableRow r)
	{
		synchronized (getYCResults())
		{
			int iycno = r.getYcNo();
			if (getYCResults().containsKey(iycno))
			{
				return getYCResults().get(iycno);
			}
			return null;
		}
	}

	public final void SetYxpTableRowData(YxpTableRow r, boolean o)
	{
		SetYXData(r, o);
	}
	public final void SetYxpTableRowData(YxpTableRow r, String o)
	{
		SetYXData(r, o);
	}

	public final void SetYXData(YxpTableRow r, Object o)
	{
		int iyxno = r.getYxNo();
		synchronized (getYXResults())
		{
			if (!getYXResults().containsKey(iyxno))
			{
				getYXResults().put(iyxno, o);
			}
			else
			{
				getYXResults().replace(iyxno, o);
			}
		}
	}

	public final Object GetYXData(YxpTableRow r)
	{
		synchronized (getYXResults())
		{
			int iyxno = r.getYxNo();
			if (getYXResults().containsKey(iyxno))
			{
				return getYXResults().get(iyxno);
			}
			return null;
		}
	}

}