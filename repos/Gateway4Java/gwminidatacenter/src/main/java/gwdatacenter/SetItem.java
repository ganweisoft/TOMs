package gwdatacenter;

import gwdatacenter.database.*;
import java.time.*;
import java.util.List;

public class SetItem
{
	private int equipno;
	private String type;
	private String strMainInstruct, strMinorInstruct, strValue;
	private String executor;
	private int waitingTime = 0; //等待时间,比如联动的滞后,单位：毫秒
	private int startTickCount;
	public boolean bRecord;

 //       public bool bVerifyLimits = true;//是否验证权限
	public boolean bCanRepeat = false; //是否允许设置项在执行队列中可以重复出现
	public boolean bShowDlg = true; //是否弹出设置是否成功的确认框

	public String Client_Instance_GUID = null; //发起设置的客户端的GUID值
	public String Description;
	public boolean IsCj = false; //该命令是否属于场景
	public boolean IsWaitSetParm = false; //该命令是否等待返回的类型
	private Boolean waitSetParmIsFinish = null; //等待返回的设置是否完成
	private Object oo = true;

	public boolean isQRTrigger = false; //设置是否由二维码扫描触发，默认为false

	public String RequestId = null; //请求命令的ID。第三方平台需要返回控制结果，往往会带入请求ID，比如华为OC平台
	private String csResponse;
	public final String getCsResponse()
	{
		return csResponse;
	}
	public final void setCsResponse(String value)
	{
		csResponse = value;
	}
	public boolean isSynchronization = false; //命令是同步执行还是异步执行，默认是异步
	public final Boolean getWaitSetParmIsFinish()
	{
		synchronized (oo)
		{
			return waitSetParmIsFinish;
		}
	}
	public final void setWaitSetParmIsFinish(Boolean value)
	{
		synchronized (oo)
		{
			waitSetParmIsFinish = value;
		}
	}
	private boolean bstopsetparm = false;
	public final boolean getBStopSetParm()
	{
		synchronized (oo)
		{
			return bstopsetparm;
		}
	}
	public final void setBStopSetParm(boolean value)
	{
		synchronized (oo)
		{
			bstopsetparm = value;
		}
	}
	public int m_SetNo = -1;
	public int CJ_EqpNo = -1, CJ_SetNo = -1;
	public final String getExecutor()
	{
		return executor;
	}
	public final void setExecutor(String value)
	{
		executor = value;
	}
	public String sysExecutor = ""; //系统内部执行者，比如"联动”、"定时任务"等
	public final int getEquipNo()
	{
		return equipno;
	}

	public final int getWaitingTime()
	{
		return waitingTime;
	}
	public final void setWaitingTime(int value)
	{
		waitingTime = value;
	}

	public final int getStartTickCount()
	{
		return startTickCount;
	}
	public final void setStartTickCount(int value)
	{
		startTickCount = value;
	}

	public final String getMainInstruct()
	{
		return strMainInstruct;
	}

	public final String getMinorInstruct()
	{
		return strMinorInstruct;
	}

	public final String getType()
	{
		return type;
	}
	public final void setType(String value)
	{
		type = value;
	}

	public final String getValue()
	{
		return strValue;
	}
	public final void setValue(String value)
	{
		strValue = value;
	}

	private Object _lock = new Object();
	private boolean enable = true;
	/** 
	 指示设置是否允许运行
	*/
	public final boolean getEnable()
	{
		synchronized (_lock)
		{
			return enable;
		}
	}
	public final void setEnable(boolean value)
	{
		synchronized (_lock)
		{
			enable = value;
		}
	}

	public String set_code;

	public final void GetEnableState()
	{
		GetSetNo();
		String strEnable = "True";
		sharpSystem.OutObject<String> tempOut_strEnable = new sharpSystem.OutObject<String>();
		if (m_SetNo != -1 && DataCenter.GetPropertyFromReserve(tempOut_strEnable, "setparm", "Reserve1", getEquipNo(), m_SetNo, "GWDataCenter.dll#EnableSetParm"))
		{
			strEnable = tempOut_strEnable.outArgValue;
		}

		if (strEnable.equalsIgnoreCase("true"))
		{
			setEnable(true);
		}
		else if (strEnable.equalsIgnoreCase("false"))
		{
			setEnable(false);
		}

		GetSetCode();
	}

	public boolean bOnlyDelayType = false;
	public int iDelayTime = 0;

	public final String getUserIPandPort()
	{
		return userIPandPort;
	}
	public final void setUserIPandPort(String value)
	{
		userIPandPort = value;
	}
	private String userIPandPort = "";

	public final String getCsReserve1()
	{
		return csreserve1;
	}
	public final void setCsReserve1(String value)
	{
		csreserve1 = value;
	}
	private String csreserve1 = "";
	public final String getCsReserve2()
	{
		return csreserve2;
	}
	public final void setCsReserve2(String value)
	{
		csreserve2 = value;
	}
	private String csreserve2 = "";
	public final String getCsReserve3()
	{
		return csreserve3;
	}
	public final void setCsReserve3(String value)
	{
		csreserve3 = value;
	}
	private String csreserve3 = "";
	public final void DoDelay()
	{
		if (bOnlyDelayType)
		{
			LocalDateTime StartDoTime = LocalDateTime.now(); //开始执行的时间
			while (((LocalDateTime.now().getSecond() - StartDoTime.getSecond()) * 1000 < iDelayTime))
			{
				if (getBStopSetParm())
				{
					return;
				}

                try {
                    Thread.sleep(1);
                } catch (InterruptedException e) {

                }
            }
		}
	}

	/** 
	 only 延时动作
	 
	 @param iTime
	*/
	public SetItem(int iTime)
	{
		bOnlyDelayType = true;
		iDelayTime = iTime;
		GetEnableState();
	}

	public SetItem(int Eqpno, String MainInstruct, String MinorInstruct, String Value)
	{
 //           bVerifyLimits = VerifyLimits;
		equipno = Eqpno;
		strMainInstruct = MainInstruct;
		strMinorInstruct = MinorInstruct;
		strValue = Value;
		type = GetSetType();
		GetRecord();
		GetEnableState();
	}

	public SetItem(int Eqpno, String MainInstruct, String MinorInstruct, String Value, String Type, String myexecutor)
	{
		//           bVerifyLimits = VerifyLimits;
		equipno = Eqpno;
		strMainInstruct = MainInstruct;
		strMinorInstruct = MinorInstruct;
		strValue = Value;
		type = Type;
		executor = myexecutor;
		GetRecord();
		GetEnableState();
	}

	/** 
	 
	 @param Eqpno
	 @param MainInstruct
	 @param MinorInstruct
	 @param Value
	 @param myexecutor 命令执行人，应该根据登录用户的权限来决定是否生成下发命令
	*/

	public SetItem(int Eqpno, String MainInstruct, String MinorInstruct, String Value, String myexecutor)
	{
		this(Eqpno, MainInstruct, MinorInstruct, Value, myexecutor, false);
	}

//C# TO JAVA CONVERTER NOTE: Java does not support optional parameters. Overloaded method(s) are created above:
//ORIGINAL LINE: public SetItem(int Eqpno, string MainInstruct, string MinorInstruct, string Value, string myexecutor, bool CanRepeat = false)
	public SetItem(int Eqpno, String MainInstruct, String MinorInstruct, String Value, String myexecutor, boolean CanRepeat)
	{
		bCanRepeat = CanRepeat;
   //         bVerifyLimits = VerifyLimits;
		equipno = Eqpno;
		strMainInstruct = MainInstruct;
		strMinorInstruct = MinorInstruct;
		strValue = Value;
		executor = myexecutor;
		type = GetSetType();
		GetRecord();
		GetEnableState();
	}


	public SetItem(int Eqpno, int Setno, String MainInstruct, String MinorInstruct, String Value, String myexecutor)
	{
		this(Eqpno, Setno, MainInstruct, MinorInstruct, Value, myexecutor, false);
	}

//C# TO JAVA CONVERTER NOTE: Java does not support optional parameters. Overloaded method(s) are created above:
//ORIGINAL LINE: public SetItem(int Eqpno, int Setno, string MainInstruct, string MinorInstruct, string Value, string myexecutor, bool CanRepeat = false)
	public SetItem(int Eqpno, int Setno, String MainInstruct, String MinorInstruct, String Value, String myexecutor, boolean CanRepeat)
	{
		bCanRepeat = CanRepeat;
		//         bVerifyLimits = VerifyLimits;
		equipno = Eqpno;
		m_SetNo = Setno;
		strMainInstruct = MainInstruct;
		strMinorInstruct = MinorInstruct;
		strValue = Value;
		executor = myexecutor;
		type = GetSetType();
		GetRecord();
		GetEnableState();
	}


	public SetItem(int Eqpno, int Setno, String Value, String myexecutor)
	{
		this(Eqpno, Setno, Value, myexecutor, false);
	}

//C# TO JAVA CONVERTER NOTE: Java does not support optional parameters. Overloaded method(s) are created above:
//ORIGINAL LINE: public SetItem(int Eqpno, int Setno, string Value, string myexecutor, bool CanRepeat = false)
	public SetItem(int Eqpno, int Setno, String Value, String myexecutor, boolean CanRepeat) {
		bCanRepeat = CanRepeat;
		equipno = Eqpno;
		m_SetNo = Setno;

		var r = StationItem.getDbSetparm().stream().filter(m -> (m.getEquipNo() == Eqpno && m.getSetNo() == Setno)).findFirst().orElse(null);
		if (r == null) {
			throw new RuntimeException("未找到设置点的缓存");
		}
		strMainInstruct = r.getMainInstruction();
		strMinorInstruct = r.getMinorInstruction();
		strValue = sharpSystem.StringHelper.isNullOrWhiteSpace(Value) ? r.getValue() : Value;

		executor = myexecutor;
		type = GetSetType();
		GetRecord();
		GetEnableState();
	}

	public final String GetSetItemDesc()
	{
		String strSetNm = "";
		String s_strMainInstruct, s_strMinorInstruct, s_strValue;
		if (strMainInstruct == null)
		{
			s_strMainInstruct = null;
		}
		else if (strMainInstruct.equals(""))
		{
			s_strMainInstruct = "";
		}
		else
		{
			s_strMainInstruct = strMainInstruct;
		}

		if (strMinorInstruct == null)
		{
			s_strMinorInstruct = null;
		}
		else if (strMinorInstruct.equals(""))
		{
			s_strMinorInstruct = "";
		}
		else
		{
			s_strMinorInstruct = strMinorInstruct;
		}

		if (strValue == null)
		{
			s_strValue = null;
		}
		else if (strValue.equals(""))
		{
			s_strValue = "";
		}
		else
		{
			s_strValue = strValue;
		}
		var query = StationItem.getDbSetparm().stream().filter(e -> e.getEquipNo() == equipno &&
				(s_strMainInstruct.equals(e.getMainInstruction())) &&
				(s_strMinorInstruct.equals(e.getMinorInstruction())) &&
				(s_strValue.equals(e.getValue())));

		if (query.count() > 0)
		{
			strSetNm = query.findFirst().get().getSetNm();
		}
		else
		{
			var query1 = StationItem.getDbSetparm().stream().filter(e -> (e.getEquipNo() == equipno && e.getSetNo() == m_SetNo));

			if (query1.count() > 0)
			{
				strSetNm = query.findFirst().get().getSetNm();
			}
		}
		if (strSetNm.equals(""))
		{
			return null;
		}

		String eqpnm = "";
		try
		{
			eqpnm = StationItem.GetEquipItemFromEquipNo(getEquipNo()).getEquipNm();
		}
		catch (RuntimeException e)
		{
		}
		return eqpnm + "-" + strSetNm;

	}


	/** 
	 获取该命令在setparm表中对应的set_no
	 
	 @return 
	*/
	public final int GetSetNo()
	{
		if (m_SetNo != -1)
		{
			return m_SetNo;
		}
		String s_strMainInstruct, s_strMinorInstruct, s_strValue;
		if (strMainInstruct == null)
		{
			s_strMainInstruct = null;
		}
		else if (strMainInstruct.equals(""))
		{
			s_strMainInstruct = "";
		}
		else
		{
			s_strMainInstruct = strMainInstruct;
		}

		if (strMinorInstruct == null)
		{
			s_strMinorInstruct = null;
		}
		else if (strMinorInstruct.equals(""))
		{
			s_strMinorInstruct = "";
		}
		else
		{
			s_strMinorInstruct = strMinorInstruct;
		}

		if (strValue == null)
		{
			s_strValue = null;
		}
		else if (strValue.equals(""))
		{
			s_strValue = "";
		}
		else
		{
			s_strValue = strValue;
		}

		var query = StationItem.getDbSetparm().stream().filter(e -> e.getEquipNo() == equipno &&
				(s_strMainInstruct.equals(e.getMainInstruction())) &&
				(s_strMinorInstruct.equals(e.getMinorInstruction())) &&
				(s_strValue.equals(e.getValue())));

		if (query.count() == 0)
		{
			return -1;
		}
		else
		{
			return query.findFirst().get().getSetNo();
		}
	}

	/** 
	 获取该命令在setparm表中对应的set_code
	 
	 @return 
	*/
	public final void GetSetCode()
	{
		var query = StationItem.getDbSetparm().stream().filter(e -> e.getEquipNo() == equipno && e.getSetNo() == m_SetNo);

		if (query.count() == 0)
		{
			set_code = "";
		}
		else
		{
			set_code = query.findFirst().get().getSetCode();
		}
	}



	/** 
	 获取该命令在setparm表中对应的record状态,以及备注字段信息
	 
	 @return 
	*/
	public final void GetRecord()
	{
		List result;
		if (m_SetNo != -1)
		{
			result = StationItem.getDbSetparm().stream().filter(e -> e.getEquipNo() == equipno && e.getSetNo() == m_SetNo).toList();
		}
		else
		{
			String s_strMainInstruct, s_strMinorInstruct, s_strValue;
			if (strMainInstruct == null)
			{
				s_strMainInstruct = null;
			}
			else if (strMainInstruct.equals(""))
			{
				s_strMainInstruct = "";
			}
			else
			{
				s_strMainInstruct = strMainInstruct;
			}

			if (strMinorInstruct == null)
			{
				s_strMinorInstruct = null;
			}
			else if (strMinorInstruct.equals(""))
			{
				s_strMinorInstruct = "";
			}
			else
			{
				s_strMinorInstruct = strMinorInstruct;
			}

			if (strValue == null)
			{
				s_strValue = null;
			}
			else if (strValue.equals(""))
			{
				s_strValue = "";
			}
			else
			{
				s_strValue = strValue;
			}

			result = StationItem.getDbSetparm().stream().filter(e -> e.getEquipNo() == equipno &&
					(s_strMainInstruct.equals(e.getMainInstruction())) &&
					(s_strMinorInstruct.equals(e.getMinorInstruction())) &&
					(s_strValue.equals(e.getValue()))).toList();
		}


		if (result.isEmpty())
		{
			bRecord = true;
		}
		else
		{
			bRecord = ((SetParmTableRow)result.getFirst()).getRecord();
		}

	}

	public final String GetSetType()
	{
		try
		{
			if (m_SetNo == -1)
			{
				String s_strMainInstruct, s_strMinorInstruct, s_strValue;
				if (strMainInstruct == null)
				{
					s_strMainInstruct = null;
				}
				else if (strMainInstruct.equals(""))
				{
					s_strMainInstruct = "";
				}
				else
				{
					s_strMainInstruct = strMainInstruct;
				}

				if (strMinorInstruct == null)
				{
					s_strMinorInstruct = null;
				}
				else if (strMinorInstruct.equals(""))
				{
					s_strMinorInstruct = "";
				}
				else
				{
					s_strMinorInstruct = strMinorInstruct;
				}

				if (strValue == null)
				{
					s_strValue = null;
				}
				else if (strValue.equals(""))
				{
					s_strValue = "";
				}
				else
				{
					s_strValue = strValue;
				}
				var query = StationItem.getDbSetparm().stream().filter(e -> e.getEquipNo() == equipno &&
						(s_strMainInstruct.equals(e.getMainInstruction())) &&
						(s_strMinorInstruct.equals(e.getMinorInstruction())) &&
						(s_strValue.equals(e.getValue())));

				if (query.count() == 0)
				{
					DataCenter.WriteLogFile("set_type of SetParm is null");
					return null;
				}
				else
				{
					return query.findFirst().get().getSetType();
				}
			}
			else
			{
				var query = StationItem.getDbSetparm().stream().filter(e -> (e.getSetNo() == equipno && e.getSetNo() == m_SetNo));
				if (query.count() == 0)
				{
					DataCenter.WriteLogFile(String.format("SetParm 不存在equip_no=%1$s set_no=%2$s的对应项", equipno, m_SetNo));
					return null;
				}
				else
				{
					//顺带获取备注字段的信息 2020/5/24
					var queryFirst = query.findFirst().get();

					csreserve1 = queryFirst.getReserve1();
					csreserve2 = queryFirst.getReserve2();
					csreserve3 = queryFirst.getReserve3();
					return queryFirst.getSetType();
				}
			}
		}
		catch (RuntimeException e)
		{
			return null;
		}
	}

	@Override
	public String toString()
	{
		return super.toString();
	}

}