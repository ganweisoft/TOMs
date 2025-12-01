package gwdatacenter;

import gwdatacenter.Interface.CEquipBase;
import gwdatacenter.Interface.ICanReset;
import gwdatacenter.Interface.IEquip;
import gwdatacenter.args.EquipEvent;
import gwdatacenter.args.EquipState;
import gwdatacenter.args.NoSetItemPermissionEventArgs;
import gwdatacenter.args.SafeTimeSpan;
import gwdatacenter.communication.SerialPort;
import gwdatacenter.database.*;
import sharpSystem.EventArgs;
import sharpSystem.StringHelper;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.net.URL;
import java.net.URLClassLoader;
import java.nio.file.Paths;
import java.time.OffsetTime;
import java.time.ZoneOffset;
import java.util.*;
import java.io.*;
import java.util.concurrent.ConcurrentLinkedDeque;
import java.util.regex.Pattern;
import java.util.jar.JarEntry;
import java.util.jar.JarFile;


public class EquipItem implements ICanReset, Comparable
{
	private int istano;
	private Object iequipno;
	private int iacc_cyc, iacc_num, alarm_scheme;
	private String equip_nm;
	private String local_addr, equip_addr;
	public String communication_drv, communication_param, communication_time_param;
	public String alarmMsg, RestorealarmMsg, advice_Msg, wave_file, restore_wave_file, related_pic, equip_detail;
	public int attrib;
	public Integer AlarmRiseCycle = null; //报警升级周期
	public String Reserve1; //保留字段1
	public String Reserve2; //保留字段2
	public String Reserve3; //保留字段3
	public String related_video, ZiChanID, PlanNo;
	public ArrayList<SafeTimeSpan> SafeTimeSpanList = new ArrayList<SafeTimeSpan>();

	private Package dll;
	private IEquip icommunication;
	private CEquipBase EquipBase;
	public final CEquipBase getEquipBase()
	{
		return EquipBase;
	}
	public final void setEquipBase(CEquipBase value)
	{
		EquipBase = value;
	}
	private SetItem curSetItem;
	public final SetItem getCurSetItem()
	{
		return curSetItem;
	}
	public final void setCurSetItem(SetItem value)
	{
		curSetItem = value;
	}
	private Object datafrash = (Object)false;

	private Object state;

	public ConcurrentLinkedDeque<SetItem> SetItemQueue = new ConcurrentLinkedDeque<SetItem>();
	public LinkedList OffLineSetItemQueue = new LinkedList();

    @Deprecated
	public sharpSystem.Event<EquipValueFrashEventHandler> ValueFrashed = new sharpSystem.Event<EquipValueFrashEventHandler>();
	@FunctionalInterface
	public interface EquipValueFrashEventHandler
	{
		void invoke(Object sender, EventArgs e);
	}

	public sharpSystem.Event<EqpStateChangedEventHandler> EqpStateChanged = new sharpSystem.Event<EqpStateChangedEventHandler>();
	@FunctionalInterface
	public interface EqpStateChangedEventHandler
	{
		void invoke(Object sender, EventArgs e);
	}

	public sharpSystem.Event<SetItemNoPermissionEventHandler> NoSetItemPermissionEvent = new sharpSystem.Event<SetItemNoPermissionEventHandler>();
	@FunctionalInterface
	public interface SetItemNoPermissionEventHandler
	{
		void invoke(Object sender, NoSetItemPermissionEventArgs e);
	}

	public sharpSystem.Event<sharpSystem.EventHandler<EventArgs>> EquipCommError = new sharpSystem.Event<sharpSystem.EventHandler<EventArgs>>();
	public sharpSystem.Event<sharpSystem.EventHandler<EventArgs>> EquipCommOk = new sharpSystem.Event<sharpSystem.EventHandler<EventArgs>>();
	public sharpSystem.Event<sharpSystem.EventHandler<EventArgs>> EquipHaveAlarm = new sharpSystem.Event<sharpSystem.EventHandler<EventArgs>>();
	public sharpSystem.Event<sharpSystem.EventHandler<EventArgs>> EquipNoAlarm = new sharpSystem.Event<sharpSystem.EventHandler<EventArgs>>();

	public int iCommFaultRetryCount = 0;
	public boolean bInitOk = false;
	public boolean bCommunicationOk = false;
	private EquipTableRow dr;

	public SerialPort serialport; //适用于串口通讯

	public boolean bCanMonitor = false; //能否进行监控，与授权文件有关
	public boolean DoSetParm = false; //标记是否该设备处于设置动作
	public Object EquipRWstate = false; //设备读写状态锁定标记
	public Object EquipResetLock = false; //设备重置状态锁定标记

	private Object bsetcachedata = false; //设备是否缓存标记，缓存意味着实时数据会记录到数据库，作为重启系统的初始数据
	public final boolean getBSetCacheData()
	{
		synchronized (bsetcachedata)
		{
			return (Boolean)bsetcachedata;
		}
	}
	public final void setBSetCacheData(boolean value)
	{
		synchronized (bsetcachedata)
		{
			bsetcachedata = value;
		}
	}

	//////////////////////

	private Object reset = false;
	private Object debug = false;
	private Object canconfirm = false;
	private Object isbackup = false;

	public boolean IsRageMode = false; //是否处于急速运行模式
	/** 
	 指示设备是否需要重置
	*/
	public final Object getReset()
	{
		synchronized (reset)
		{
			return reset;
		}
	}
	public final void setReset(Object value)
	{
		synchronized (reset)
		{
			reset = value;
		}
	}
	private Object _lock = new Object();
	private boolean enable = true;
	/** 
	 指示设备是否需要运行
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

	/** 
	 指示设备是否需要进入调试状态
	*/
	public final boolean isDebug()
	{
		synchronized (debug)
		{
			return (Boolean)debug;
		}
	}
	public final void setDebug(boolean value)
	{
		synchronized (debug)
		{
			debug = value;
		}
	}

	public boolean IsBackupEquip = false; //是否在数据库中配置为热备设备，这个状态是配置到数据库中，不随着主备机之间的状态进行改变

	/** 
	 指示设备是否处于备份状态，这个状态会根据主备机之间的状态进行改变
	*/
	public final boolean isBackupState()
	{
		synchronized (isbackup)
		{
			return (Boolean)isbackup;
		}
	}
	public final void setBackupState(boolean value)
	{
		synchronized (isbackup)
		{
			isbackup = value;
		}
	}

	public sharpSystem.Event<sharpSystem.EventHandler<EventArgs>> RevEqpEvt = new sharpSystem.Event<sharpSystem.EventHandler<EventArgs>>(); //收到设备的通讯事件，比如：用于门禁设备的刷卡记录
	public final void DoRevEqpEvt(EquipEvent Evt)
	{
		if (RevEqpEvt != null)
		{
			for (sharpSystem.EventHandler<EventArgs> listener : RevEqpEvt.listeners())
			{
				listener.invoke(Evt, null);
			}
		}
	}
	public String strLinkageEvent;
	public final boolean getBCanConfirm2NormalState()
	{
		synchronized (canconfirm)
		{
			return (Boolean)canconfirm;
		}
	}
	public final void setBCanConfirm2NormalState(boolean value)
	{
		synchronized (canconfirm)
		{
			debug = value;
		}
	}

	private String evtGUID;
	public final String getEventGUID()
	{
		return evtGUID;
	}
	public final void setEventGUID(String value)
	{
		evtGUID = value;
	}
	private boolean bFirstEnter = true;

	private void GetSafeTimeSpanList(String s)
	{
		try
		{
			SafeTimeSpanList.clear();
			s = s.strip();
			if (sharpSystem.StringHelper.isNullOrEmpty(s))
			{
				return;
			}
			String[] ss = s.split("[+]", -1);
			for (String s1 : ss)
			{
				String[] tt = s1.split("[-]", -1);
				if (tt.length == 2)
				{
					var splitTt = tt[0].split("[:]", -1);
					var splitTt2 = tt[1].split("[:]", -1);

					OffsetTime t1 = OffsetTime.of(Integer.parseInt(splitTt[0]), Integer.parseInt(splitTt[1]), Integer.parseInt(splitTt[2]), 0, ZoneOffset.UTC);
					OffsetTime t2 = OffsetTime.of(Integer.parseInt(splitTt2[0]), Integer.parseInt(splitTt2[1]), Integer.parseInt(splitTt2[2]), 0, ZoneOffset.UTC);
					SafeTimeSpanList.add(new SafeTimeSpan(t1, t2));
				}
			}
		}
		catch (RuntimeException e)
		{
		}
	}

	public final EquipState getState()
	{
		synchronized (state)
		{
			return (EquipState)state;
		}
	}
	public final void setState(EquipState value)
	{
		state = value;
	}

	/** 
	 指示设备数据是否刷新
	*/
	public final boolean getDataFrash()
	{
		synchronized (datafrash)
		{
			return (Boolean)datafrash;
		}
	}
	public final void setDataFrash(boolean value)
	{
		synchronized (datafrash)
		{
			datafrash = (Object)(Boolean)value;
		}
	}

	public final Package getDll()
	{
		return dll;
	}

	public final IEquip getICommunication()
	{
		return icommunication;
	}

	public final int getIStano()
	{
		return istano;
	}

	public final int getIEquipno()
	{
		synchronized (iequipno)
		{
			return (Integer)iequipno;
		}
	}

	public final int getAlarmScheme()
	{
		return alarm_scheme;
	}

	public final int getIAccCyc()
	{
		return iacc_cyc;
	}

	public final int getIAccNum()
	{
		return iacc_num;
	}
	public final void setIAccNum(int value)
	{
		iacc_num = value;
	}

	public final String getLocalAddr()
	{
		if (local_addr.length() > 2)
		{
			return local_addr.split("[()]")[0].strip(); //末尾添加(k)样式，用于划分多线程，比如192.168.0.11(1)、192.168.0.11(2)
		}
		else
		{
			return local_addr;
		}
	}

	public final String getLocalAddr4Thread()
	{
		return local_addr;
	}

	public final String getEquipAddr()
	{
		return equip_addr;
	}

	public final String getEquipNm()
	{
		synchronized (equip_nm)
		{
			return equip_nm;
		}
	}
	public final void setEquipNm(String value)
	{
		synchronized (equip_nm)
		{
			equip_nm = value;
		}
	}

	public final int compareTo(Object obj)
	{
		EquipItem eqp = obj instanceof EquipItem ? (EquipItem)obj : null;
        assert eqp != null;
        return Integer.compare(getIEquipno(), eqp.getIEquipno());
	}

	public EquipItem(int sta, int eqp, SerialPort p, EquipTableRow r)
	{
		istano = sta;
		iequipno = eqp;
		setState(EquipState.Initial);
		serialport = p;
		ResetWhenDBChanged(sta, eqp, r);
		iacc_num = 0;
	}

	public final boolean ResetWhenDBChanged(Object... o)
	{
		synchronized (EquipResetLock)
		{
			int sta = (Integer)o[0];
			int eqp = (Integer)o[1];
			if (o.length > 2)
			{
				dr = (EquipTableRow)o[2]; //设备初始化的时候会传入这个参数，加快大设备数的启动时间
			}
			else //动态修改设备的时候不传入
			{
				dr = StationItem.getDbEqp().stream().filter(m -> m.getEquipNo() == eqp).findFirst().orElse(null);
			}

			if(dr == null){
				return false;
			}

			try
			{
                equip_nm = (dr.getEquipNm() != null) ? dr.getEquipNm() : "";
				equip_detail = dr.getEquipDetail() != null ? dr.getEquipDetail() : "";
				iacc_cyc = dr.getAccCyc();
				alarm_scheme = dr.getAlarmScheme();
				local_addr = dr.getLocalAddr() != null ? dr.getLocalAddr() : "";
				equip_addr = dr.getEquipAddr() != null ? dr.getEquipAddr() : "";
				related_pic = dr.getRelatedPic() != null ? dr.getRelatedPic() : "";
				communication_drv = dr.getCommunicationDrv().strip();
				alarmMsg = equip_nm + ":" + (dr.getOutOfContact() != null ? dr.getOutOfContact() : "");
				advice_Msg = dr.getProcAdvice() != null ? dr.getProcAdvice() : "";
				attrib = dr.getAttrib();
				setBSetCacheData(attrib == 1);
				RestorealarmMsg = equip_nm + ":" + (dr.getContacted() != null ? dr.getContacted() : "");

				AlarmRiseCycle = dr.getAlarmRiseCycle() == null ? 0 : dr.getAlarmRiseCycle();
				Reserve1 = dr.getReserve1() != null ? dr.getReserve1() : "";
				Reserve2 = dr.getReserve2() != null ? dr.getReserve2() : "";
				Reserve3 = dr.getReserve3() != null ? dr.getReserve3() : "";

				related_video = dr.getRelatedVideo() != null ? dr.getRelatedVideo() : "";
				ZiChanID = dr.getZiChanID() != null ? dr.getZiChanID() : "";
				PlanNo = dr.getPlanNo() != null ? dr.getPlanNo() : "";

				String strEnable = "True";
				sharpSystem.OutObject<String> tempOut_strEnable = new sharpSystem.OutObject<String>();
				if (DataCenter.GetPropertyFromReserveWithEquipTableRow(tempOut_strEnable, "Reserve1", "GWDataCenter.dll#EnableEquip", dr))
				{
					strEnable = tempOut_strEnable.outArgValue;
					if (strEnable.toLowerCase().equals("true"))
					{
						setEnable(true);
					}
					else if (strEnable.toLowerCase().equals("false"))
					{
						setEnable(false);
					}
				}
				else
				{
					strEnable = tempOut_strEnable.outArgValue;
				}

				if (!sharpSystem.StringHelper.isNullOrEmpty(dr.getBackup()))
				{
					IsBackupEquip = dr.getBackup().equalsIgnoreCase("TRUE");
				}
				else
				{
					IsBackupEquip = false;
				}
				GetSafeTimeSpanList(dr.getSafeTime() != null ? dr.getSafeTime() : "");

				String wf = dr.getEventWav() != null ? dr.getEventWav() : "";

				String[] fs = wf.split("[/]", -1);
				wave_file = fs[0];
				if (fs.length == 2)
				{
					restore_wave_file = fs[1];
				}

				communication_param = dr.getCommunicationParam() != null ? dr.getCommunicationParam() : "";
				communication_time_param = dr.getCommunicationTimeParam() != null ? dr.getCommunicationTimeParam() : "";
				GetInterfaceOfEquip(); //加入这一项，确保可以动态修改dll名称
				bFirstEnter = false;
			}
			catch (RuntimeException e)
			{
				DataCenter.WriteLogFile(e.toString());
			}
			return true;
		}
	}

	/** 
	 
	 @param item
	*/
	public final void AddSetItem(SetItem item)
	{
		if (item == null)
		{
			return;
		}

		if ((getState() != EquipState.NoCommunication && getState() != EquipState.Initial) || item.getType().equals("S"))
		{
			SetItemQueue.add(item);
		}
	}

	/** 
	 得到设备动态库中的 IEquip 接口
	*/
	private void GetInterfaceOfEquip()
	{
		icommunication = null;
		try
		{
			String FullPathName = GetFullPathName4CommDrv(communication_drv);
			if(StringHelper.isNullOrEmpty(FullPathName)){
				return;
			}

			List<String> classNames = new ArrayList<>();

            URL jarUrl;
            Enumeration<JarEntry> entries;
            try (var jarFile = new JarFile(FullPathName)) {
                jarUrl = new File(FullPathName).toURI().toURL();
                entries = jarFile.entries();

				while (entries.hasMoreElements()) {
					JarEntry jarEntry = entries.nextElement();
					String className = jarEntry.getName();
					if(className.endsWith(".class")) {
						className = className.replace(".class", "").replace("/", ".");
						classNames.add(className);
					}
				}
            }

			for (String t : classNames)
			{
				if (!StringHelper.isNullOrEmpty(t) && t.contains("CEquip"))
				{
					// 2. 创建URLClassLoader，指定父类加载器
                    Class<?> targetClass;
                    try (URLClassLoader classLoader = new URLClassLoader(
                            new URL[]{jarUrl},
                            EquipItem.class.getClassLoader() // 使用当前类加载器作为父类
                    )) {
                        targetClass = classLoader.loadClass(t);
                    }

                    // 4. 实例化类（假设有无参构造）
					Constructor<?> constructor = targetClass.getDeclaredConstructor();
					Object tempVar2 = constructor.newInstance();
					icommunication = tempVar2 instanceof IEquip ? (IEquip)tempVar2 : null;
					if(icommunication == null)
						continue;

					icommunication.setEquipitem(this);
					setEquipBase(tempVar2 instanceof CEquipBase ? (CEquipBase)tempVar2 : null);
					break;
				}
			}
			if (icommunication == null)
			{
				throw new IllegalArgumentException("icommunication as  IEquip is null");
			}

		}
		catch (RuntimeException | ClassNotFoundException | InstantiationException | IllegalAccessException | IOException | NoSuchMethodException | InvocationTargetException e) {
			e.printStackTrace();
			DataCenter.WriteLogFile(e.toString());
			bCommunicationOk = false;
			setState(EquipState.NoCommunication);
        }
    }

	private String GetFullPathName4CommDrv(String moduleName)
	{
		moduleName = moduleName.replace(".dll", "") + ".jar";

		String fullPathName1 = Paths.get(General.GetApplicationRootPath(), "dll", moduleName).toString();

		final Pattern ext = Pattern.compile("(?<=.)\\.[^.]+$");

		String fullPathName2 = Paths.get(General.GetApplicationRootPath(), "dll", ext.matcher(moduleName).replaceAll(""), moduleName).toString();

		if ((new File(fullPathName1)).isFile())
		{
			if ((new File(fullPathName2)).isFile())
			{
				DataCenter.WriteLogFile(String.format("Equip表中加载的文件%1$s同时存在于%2$s和%3$s两个目录,可能出错!", moduleName, fullPathName1, fullPathName2));
			}
			return fullPathName1;
		}
		else if ((new File(fullPathName2)).isFile())
		{
			return fullPathName2;
		}
		return null;
	}
}