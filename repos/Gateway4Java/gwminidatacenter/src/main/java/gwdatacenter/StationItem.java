package gwdatacenter;

import gwdatacenter.args.ChangedEquip;
import gwdatacenter.args.ChangedEquipState;
import gwdatacenter.args.SafetyLevel;
import gwdatacenter.database.*;
import sharpSystem.EventArgs;
import sharpSystem.EventHandler;

import java.util.*;

/** 
 对设备进行分类，每一个类型对应一个通讯线程
*/
public final class StationItem
{
	public static SafetyLevel m_SafetyLevel = SafetyLevel.forValue(1);
	public static HashMap<String, Object> EquipCategoryDict = new HashMap<String, Object>();
	public static List<SetItem> SetItemQueue = Collections.synchronizedList(new ArrayList<SetItem>() {
	});
	public static HashMap<String, ArrayList<EquipTableRow>> SubEquipListDict;

	public static sharpSystem.Event<EventHandler<EventArgs>> SetParmResultEvent = new sharpSystem.Event<EventHandler<EventArgs>>(); //通知执行设置后的结果 add by sd in 20190316
	public static sharpSystem.Event<EventHandler<EventArgs>> SetParmResponseEvent = new sharpSystem.Event<EventHandler<EventArgs>>(); //通知执行设置后的响应值（不是返回值），这个事件通常在dll中去执行

	public static sharpSystem.Event<EventHandler<EventArgs>> AppClose = new sharpSystem.Event<EventHandler<EventArgs>>(); //程序关闭事件
	public static sharpSystem.Event<EventHandler<EventArgs>> RightHandWave = new sharpSystem.Event<EventHandler<EventArgs>>();
	public static sharpSystem.Event<EventHandler<EventArgs>> LeftHandWave = new sharpSystem.Event<EventHandler<EventArgs>>();

	public static sharpSystem.Event<EventHandler<EventArgs>> StationCommError = new sharpSystem.Event<EventHandler<EventArgs>>();
	public static sharpSystem.Event<EventHandler<EventArgs>> StationCommOk = new sharpSystem.Event<EventHandler<EventArgs>>();
	public static sharpSystem.Event<EventHandler<EventArgs>> StationHaveAlarm = new sharpSystem.Event<EventHandler<EventArgs>>();
	public static sharpSystem.Event<EventHandler<EventArgs>> StationNoAlarm = new sharpSystem.Event<EventHandler<EventArgs>>();

	public static sharpSystem.Event<EventHandler<EventArgs>> EquipBackUpChanged = new sharpSystem.Event<EventHandler<EventArgs>>(); //备份状态发生改变

	public static sharpSystem.Event<EventHandler<EventArgs>> HaveEquipChanged = new sharpSystem.Event<EventHandler<EventArgs>>(); //发生设备动态添加、删除、修改的事件
	public static sharpSystem.Event<EventHandler<EventArgs>> HaveEquipReset = new sharpSystem.Event<EventHandler<EventArgs>>(); //发生设备配置重置事件


	private static ArrayList<EquipTableRow> db_eqp; //= new DataTable();
	private static ArrayList<YcpTableRow> db_ycp; //= new DataTable();
	private static ArrayList<YxpTableRow> db_yxp; // = new DataTable();
	private static ArrayList<SetParmTableRow> db_setparm; //= new DataTable();

	private static final Object db_eqp_lock = true;
	private static final Object db_ycp_lock = true;
	private static final Object db_yxp_lock = true;
	private static final Object db_setparm_lock = true;

	public static ArrayList<EquipTableRow> getDbEqp()
	{
		synchronized (db_eqp_lock)
		{
			return db_eqp;
		}
	}
	public static void setDbEqp(ArrayList<EquipTableRow> value)
	{
		synchronized (db_eqp_lock)
		{
			db_eqp = value;
		}
	}
	public static ArrayList<YcpTableRow> getDbYcp()
	{
		synchronized (db_ycp_lock)
		{
			return db_ycp;
		}
	}
	public static void setDbYcp(ArrayList<YcpTableRow> value)
	{
		synchronized (db_ycp_lock)
		{
			db_ycp = value;
		}
	}
	public static ArrayList<YxpTableRow> getDbYxp()
	{
		synchronized (db_yxp_lock)
		{
			return db_yxp;
		}
	}
	public static void setDbYxp(ArrayList<YxpTableRow> value)
	{
		synchronized (db_yxp_lock)
		{
			db_yxp = value;
		}
	}
	public static ArrayList<SetParmTableRow> getDbSetparm()
	{
		synchronized (db_setparm_lock)
		{
			return db_setparm;
		}
	}
	public static void setDbSetparm(ArrayList<SetParmTableRow> value)
	{
		synchronized (db_setparm_lock)
		{
			db_setparm = value;
		}
	}

	public static void FireSetParmResultEvent(SetItem item)
	{
		if (StationItem.SetParmResultEvent != null)
		{
			for (EventHandler<EventArgs> listener : SetParmResultEvent.listeners())
			{
				listener.invoke(item, new EventArgs());
			}
		}
	}
	public static void DoHaveEquipChanged(SubEquipList SE)
	{
		if (HaveEquipChanged != null)
		{
			for (EventHandler<EventArgs> listener : HaveEquipChanged.listeners())
			{
				listener.invoke(SE, new EventArgs());
			}
		}
	}
	public static void DoHaveEquipReset(ArrayList<Integer> EqpNoList)
	{
		if (HaveEquipReset != null)
		{
			for (EventHandler<EventArgs> listener : HaveEquipReset.listeners())
			{
				listener.invoke(EqpNoList, new EventArgs());
			}
		}
	}
	public static HashMap<String, ArrayList<EquipTableRow>> GetSubEquipListDataRow(ArrayList<EquipTableRow> Rows)
	{
		HashMap<String, ArrayList<EquipTableRow>> DS = new HashMap<String, ArrayList<EquipTableRow>>();
		try
		{
			for (EquipTableRow s : Rows)
			{
				String key = s.getLocalAddr().toUpperCase().strip();
				if (DS.containsKey(key))
				{
					DS.get(key).add(s);
				}
				else
				{
					ArrayList<EquipTableRow> list = new ArrayList<EquipTableRow>();
					list.add(s);
					DS.put(key, list);
				}
			}
		}
		catch (RuntimeException e)
		{
			DataCenter.WriteLogFile(e.toString() + "检查local_addr字段不能为空");
		}
		return DS;
	}

	public static boolean init()
	{
		DataCenter.brunning = true;
        try {
            GWDbProvider.Instance.Init();
        } catch (InterruptedException e) {
            throw new RuntimeException(e);
        }
        System.out.println("初始化数据库");

		UpdateMainDataTable();

		SubEquipListDict = GetSubEquipListDataRow(getDbEqp());

		SubEquipListDict.keySet().parallelStream().forEach((la) -> {
			SubEquipList sl = new SubEquipList(la, SubEquipListDict.get(la));
			if (sl.bCanExcute)
			{
				synchronized (EquipCategoryDict)
				{
					EquipCategoryDict.put(la, sl);
				}
			}
		});

		System.out.println("初始化其它服务");
		return true;
	}

	public static void UpdateMainDataTable() //获取Equip\YCP\YXP三个表
	{
		try
		{
			db_eqp = GWDbProvider.Instance.GetEquipTableList();
			db_ycp = GWDbProvider.Instance.GetYcpTableList();
			db_yxp = GWDbProvider.Instance.GetYxpTableList();
			db_setparm = GWDbProvider.Instance.GetSetParmTableList();
		}
		catch (RuntimeException e)
		{
			DataCenter.WriteLogFile(e.toString());
		}
	}

	/** 
	 从设备号得到设备对象
	 
	 @param iEquipNo
	 @return 
	*/
	public static EquipItem GetEquipItemFromEquipNo(int iEquipNo)
	{
		synchronized (EquipCategoryDict)
		{
			for (Map.Entry<String, Object> pair : EquipCategoryDict.entrySet())
			{
				SubEquipList EquipList = (SubEquipList)pair.getValue();
				for (EquipItem i : EquipList.getEquipList())
				{
					if (i.getIEquipno() == iEquipNo)
					{
						return i;
					}
				}
			}
		}
		return null;
	}


	public static ArrayList<ChangedEquip> ChangedEquipList = new ArrayList<ChangedEquip>();
	public static sharpSystem.Event<EventHandler<EventArgs>> ChangedEquipListChanged = new sharpSystem.Event<EventHandler<EventArgs>>(); //动态添加、删除、修改设备状态发生改变
	public static DelayEventFire ChangedEquipListChangedDelayEvent = null;

	public static sharpSystem.Event<EventHandler<EventArgs>> AddNewSubEquipList = new sharpSystem.Event<EventHandler<EventArgs>>(); //动态添加了一个新的设备通讯线程
	private static int iCount = 0;
	/** 
	 用于动态添加、删除、修改设备  -----事件延迟响应，合并更新，大大提高了运行效率，比如一次性添加1000个设备，现在只需要一次操作。 by 2021-12-16
	 
	 @param Eqp
	*/
	public static void AddChangedEquip(ChangedEquip Eqp)
	{
		synchronized (ChangedEquipList)
		{
			ChangedEquip tempVar = new ChangedEquip();
			tempVar.State = Eqp.State;
			tempVar.iStaNo = Eqp.iStaNo;
			tempVar.iEqpNo = Eqp.iEqpNo;
			ChangedEquipList.add(tempVar);
		}
		if (ChangedEquipListChangedDelayEvent == null)
		{
			ChangedEquipListChanged.addListener("StationItem_ChangedEquipListChanged", (Object sender, EventArgs e) -> StationItem_ChangedEquipListChanged(sender, e));
			ChangedEquipListChangedDelayEvent = new DelayEventFire(ChangedEquipListChanged.listeners().getFirst(), 500, ChangedEquipList, null);
		}
		ChangedEquipListChangedDelayEvent.AddEvent();
		iCount += 1;
		DataCenter.WriteLogFile(String.format("AddChangedEquip>>总共%1$s个设备", iCount));
	}

	private static void StationItem_ChangedEquipListChanged(Object sender, EventArgs e)
	{
		synchronized (ChangedEquipList)
		{
			StationItem.UpdateMainDataTable();
			for (ChangedEquip Eqp : ChangedEquipList)
			{
				SubEquipList m_SubEquipList = GetSubEquipList(Eqp.iStaNo, Eqp.iEqpNo, Eqp.State);
				if (m_SubEquipList != null)
				{
					m_SubEquipList.StartRefreshThread();
					m_SubEquipList.StartSetParmThread();
					m_SubEquipList.AddChangedEquipList1(Eqp);
				}
			}
			DataCenter.WriteLogFile(String.format("StationItem_ChangedEquipListChanged>>%1$s个设备", ChangedEquipList.size()));
			ChangedEquipList.clear();
		}
	}

	public static SubEquipList GetSubEquipList(int iStaNo, int iEqpNo, ChangedEquipState State)
	{
		SubEquipList rtnSubEquipList = null;
		SubEquipList thisSubEquipList = null;

		synchronized (StationItem.EquipCategoryDict)
		{
			//删除设备时不要查数据库，因为延迟响应的时候数据库已经被调用方删掉了 2021-12-22
			if (State == ChangedEquipState.Delete)
			{
				for (Map.Entry<String, Object> pair : StationItem.EquipCategoryDict.entrySet())
				{
					SubEquipList subEquipList = (SubEquipList)pair.getValue();
					for (EquipItem e : subEquipList.getEquipList())
					{
						if (e.getIEquipno() == iEqpNo)
						{
							return subEquipList;
						}
					}
				}
			}

			String strlocal_addr = null;
			EquipTableRow r = StationItem.getDbEqp().stream().filter(m -> m.getEquipNo() == iEqpNo).findFirst().orElse(null);
			if (r != null)
			{
				strlocal_addr = r.getLocalAddr().toUpperCase().strip();
			}
			else
			{
				EquipItem EI = StationItem.GetEquipItemFromEquipNo(iEqpNo);
				if (EI == null)
				{
					return null;
				}
				strlocal_addr = EI.getLocalAddr4Thread().toUpperCase().strip();
			}
			if (strlocal_addr == null)
			{
				return null;
			}
			for (Map.Entry<String, Object> pair : StationItem.EquipCategoryDict.entrySet())
			{
				SubEquipList EquipList = (SubEquipList)pair.getValue();
				if (EquipList.local_addr.equals(strlocal_addr))
				{
					rtnSubEquipList = EquipList;
					thisSubEquipList = EquipList;
					break;
				}
			}

			if (rtnSubEquipList == null)
			{
				StationItem.SubEquipListDict = StationItem.GetSubEquipListDataRow(StationItem.getDbEqp());
				SubEquipList NewEquipList = new SubEquipList(strlocal_addr, StationItem.SubEquipListDict.get(strlocal_addr));

				thisSubEquipList = NewEquipList;
				if (NewEquipList.bCanExcute)
				{
					StationItem.EquipCategoryDict.put(strlocal_addr, NewEquipList);
				}

				if (AddNewSubEquipList != null)
				{
					for (EventHandler<EventArgs> listener : AddNewSubEquipList.listeners())
					{
						listener.invoke(NewEquipList, null);
					}
				}
			}
			/*/////////////////////////////////////////
			在设备动态修改的时候，可能会修改local_addr的参数，这个时候，需要删除掉原来的线程
			*/
			/////////////////////////////////////////
			if (State == ChangedEquipState.Edit)
			{
				for (Map.Entry<String, Object> pair : StationItem.EquipCategoryDict.entrySet())
				{
					SubEquipList subEquipList = (SubEquipList)pair.getValue();
					if (!subEquipList.local_addr.equals(thisSubEquipList.local_addr))
					{
						for (EquipItem e : subEquipList.getEquipList())
						{
							if (e.getIEquipno() == iEqpNo)
							{
								//其它线程还存在相同的设备，则要删除这个设备。   
								ChangedEquip Eqp = new ChangedEquip();
								Eqp.iStaNo = iStaNo;
								Eqp.iEqpNo = iEqpNo;
								Eqp.State = ChangedEquipState.Delete;
								subEquipList.bEditLocalAddr = true;
								thisSubEquipList.bEditLocalAddr = true; //add by sd in 2018-10-12

								/**
								*/
								synchronized (DataCenter.getEquipItemDict())
								{
									if (DataCenter.getEquipItemDict().containsKey(iEqpNo))
									{
										DataCenter.getEquipItemDict().remove(iEqpNo);
									}
								}
								/**
								*/
								subEquipList.AddChangedEquipList1(Eqp);
								break;
							}
						}
					}
				}
				//删除这个设备的数据
			}

			return thisSubEquipList;
		}
	}
}