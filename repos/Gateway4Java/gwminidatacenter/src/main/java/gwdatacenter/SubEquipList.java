package gwdatacenter;

import gwdatacenter.Interface.CEquipBase;
import gwdatacenter.Interface.CommunicationState;
import gwdatacenter.args.ChangedEquip;
import gwdatacenter.args.ChangedEquipState;
import gwdatacenter.args.EquipState;
import gwdatacenter.communication.SerialPort;
import gwdatacenter.database.*;
import gwdatacenter.mqservice.*;
import gwdatacenter.mqservice.EquipEvent;
import sharpSystem.EventArgs;

import java.time.format.DateTimeFormatter;
import java.util.*;
import java.time.*;

/** 
 对设备进行分类，每一个类型对应一个通讯线程
*/
	/**
	 从设备号得到设备对象
	 @param iEquipNo
	 @return 
	*/
	/** 
	 用于动态添加、删除、修改设备  -----事件延迟响应，合并更新，大大提高了运行效率，比如一次性添加1000个设备，现在只需要一次操作。 by 2021-12-16
	 @param Eqp
	*/
			//删除设备时不要查数据库，因为延迟响应的时候数据库已经被调用方删掉了 2021-12-22
			//DataRow r = StationItem.db.GetDataRowOfEquip(iStaNo, iEqpNo);
			/* /////////////////////////////////////////
			在设备动态修改的时候，可能会修改local_addr的参数，这个时候，需要删除掉原来的线程
			*/
			/////////////////////////////////////////
								//其它线程还存在相同的设备，则要删除这个设备。
								/**
								*/
								/**
								*/
				//删除这个设备的数据
/**
 如果设备的local_addr 相同,则这些设备将归为一个SubEquipList 类，将使用同一个通讯线程通讯
 比如 使用COM4的所有设备将运行在一个线程
*/
public class SubEquipList
{
	private boolean bStartThread = true;
	private boolean bStartRefreshThread = false;
	private boolean bStartSetParmThread = false;
	public static long ThreadInterval = 10; //100;//这个参数的调整，对CPU的影响很大
	private static final int DelayDelEqpSecond = 2; //延迟删除被删除设备的时间
	public String local_addr;

	private ArrayList<EquipItem> equiplist = new ArrayList<>();
	public final ArrayList<EquipItem> getEquipList()
	{
		return equiplist;
	}
	public final void setEquipList(ArrayList<EquipItem> value)
	{
		equiplist = value;
	}


	private Thread RefreshThread, SetParmThread;
	public EquipItem OldEquip;
	private Object existsetparm = (Object)false;
	private Object datarefreshbreak = (Object)false;
	private ArrayList<ChangedEquip> ChangedEquipList = new ArrayList<ChangedEquip>(); //动态添加删除的设备
	private ArrayList<ChangedEquip> ChangedEquipList1 = new ArrayList<ChangedEquip>(); //动态添加删除的设备
	private Object bChangingState = (Object)false; //用于动态添加删除设备

	public ArrayList<Integer> SetParmEquipList = new ArrayList<Integer>(); //正在进行设备设置的列表
	public boolean bCanExcute = true; //是否可以执行
	public boolean bEditLocalAddr = false; //是否处于动态修改设备的LocalAddr，并生成一个新SubEquipList---特殊情况

	public sharpSystem.Event<SubEquipListChangedEventHandler> SubEquipListChanged = new sharpSystem.Event<SubEquipListChangedEventHandler>();
	@FunctionalInterface
	public interface SubEquipListChangedEventHandler
	{
		void invoke(Object sender, EventArgs e);
	}

	public sharpSystem.Event<EquipAddEventHandler> EquipAdd = new sharpSystem.Event<EquipAddEventHandler>();
	@FunctionalInterface
	public interface EquipAddEventHandler
	{
		void invoke(Object sender, EventArgs e);
	}

	public sharpSystem.Event<EquipDelEventHandler> EquipDel = new sharpSystem.Event<EquipDelEventHandler>();
	@FunctionalInterface
	public interface EquipDelEventHandler
	{
		void invoke(Object sender, EventArgs e);
	}

	public sharpSystem.Event<EquipEditEventHandler> EquipEdit = new sharpSystem.Event<EquipEditEventHandler>();
	@FunctionalInterface
	public interface EquipEditEventHandler
	{
		void invoke(Object sender, EventArgs e);
	}


	public sharpSystem.Event<sharpSystem.EventHandler<EventArgs>> ChangedEquipListChanged = new sharpSystem.Event<sharpSystem.EventHandler<EventArgs>>();
	public DelayEventFire ChangedEquipListChangedDelayEvent = null;

	public final void AddChangedEquipList(ChangedEquip e)
	{
		synchronized (bChangingState)
		{
			//将对象添加到结尾处
			ChangedEquip tempVar = new ChangedEquip();
			tempVar.State = e.State;
			tempVar.iStaNo = e.iStaNo;
			tempVar.iEqpNo = e.iEqpNo;
			ChangedEquipList.add(tempVar);
			StationItem.DoHaveEquipChanged(this);
            try {
                Thread.sleep(1000);
            } catch (InterruptedException ex) {

            }
        }
	}

	public final void AddChangedEquipList1(ChangedEquip e)
	{
		synchronized (ChangedEquipList1)
		{
			ChangedEquip tempVar = new ChangedEquip();
			tempVar.State = e.State;
			tempVar.iStaNo = e.iStaNo;
			tempVar.iEqpNo = e.iEqpNo;
			ChangedEquipList1.add(tempVar);
		}
		if (ChangedEquipListChangedDelayEvent == null)
		{
			ChangedEquipListChangedDelayEvent = new DelayEventFire(ChangedEquipListChanged.listeners().getFirst(), 500, ChangedEquipList1, null);
		}
		ChangedEquipListChangedDelayEvent.AddEvent();
	}

	private void SubEquipList_ChangedEquipListChanged(Object sender, EventArgs e)
	{
		synchronized (ChangedEquipList1)
		{
			synchronized (bChangingState)
			{
				if (ChangedEquipList1.isEmpty())
				{
					return;
				}
				for (ChangedEquip item : ChangedEquipList1)
				{
					//将对象添加到结尾处
					ChangedEquip tempVar = new ChangedEquip();
					tempVar.State = item.State;
					tempVar.iStaNo = item.iStaNo;
					tempVar.iEqpNo = item.iEqpNo;
					ChangedEquipList.add(tempVar);
				}
				ChangedEquipList1.clear();
				StationItem.DoHaveEquipChanged(this);
                try {
                    Thread.sleep(1000);
                } catch (InterruptedException ex) {

                }
            }
		}
	}
	private int iCount = 0;

	private void EditEquipList()
	{
		synchronized (bChangingState)
		{
			if (ChangedEquipList.isEmpty())
			{
				return;
			}
			//StationItem.UpdateMainDataTable(); ---加入设备时已经更新了数据库，这里就不要重复操作了 2020/8/17
			EquipItem eqpitm = null;
			for (int k = 0; k < ChangedEquipList.size(); k++)
			{
				//StationItem.UpdateMainDataTable();
				if (ChangedEquipList.get(k).State == ChangedEquipState.Add)
				{
					SerialPort spt = new SerialPort();
					int stano = ChangedEquipList.get(k).iStaNo;
					int eqpno = ChangedEquipList.get(k).iEqpNo;

					synchronized (DataCenter.getEquipItemDict())
					{
						//eqpitm = new EquipItem(stano, eqpno, spt);
						//var v = from i in getEquipList().<EquipItem>OfType() where (i.iStano == stano && i.iEquipno == eqpno) select i;
						var v = getEquipList().stream().filter(i -> i.getIStano() == stano && i.getIEquipno() == eqpno);
						if (v.count() == 0) //队列不存在该项的时候才加入，否则新加一个 通讯线程的时候会重复
						{
							EquipTableRow dr = StationItem.getDbEqp().stream().filter(m -> m.getEquipNo() == eqpno).findFirst().orElse(null);
							eqpitm = new EquipItem(stano, eqpno, spt, dr);
							eqpitm.getICommunication().setResetFlag(true);
							synchronized (getEquipList())
							{
								getEquipList().add(eqpitm);
							}
						}
						else
						{
							eqpitm = v.findFirst().orElse(null);
							eqpitm.getICommunication().setResetFlag(true);
						}

						if (!DataCenter.getEquipItemDict().containsKey(eqpno))
						{
							DataCenter.getEquipItemDict().put(eqpno, eqpitm);
						}
						if (EquipAdd != null)
						{
							for (EquipAddEventHandler listener : EquipAdd.listeners())
							{
								listener.invoke(eqpno, new EventArgs());
							}
						}
						iCount += 1;
					}

				}
				if (ChangedEquipList.get(k).State == ChangedEquipState.Delete)
				{
					synchronized (getEquipList())
					{
						for (EquipItem e : getEquipList())
						{
							if (e.getIEquipno() == ChangedEquipList.get(k).iEqpNo)
							{
								if (!bEditLocalAddr)
								{
									e.setState(EquipState.NoCommunication); //删除前先设为不通讯，这样远端如果有站点级联的时候可以反映出状态 2022-03-10
									if (EquipDel != null)
									{
										for (EquipDelEventHandler listener : EquipDel.listeners())
										{
											listener.invoke(e.getIEquipno(), new EventArgs());
										}
									}
								}

								getEquipList().remove(e);

								if (!bEditLocalAddr)
								{
									synchronized (DataCenter.getEquipItemDict())
									{
										if (DataCenter.getEquipItemDict().containsKey(ChangedEquipList.get(k).iEqpNo))
										{
											DataCenter.getEquipItemDict().remove(ChangedEquipList.get(k).iEqpNo);
										}
									}
								}
								synchronized (StationItem.EquipCategoryDict)
								{
									if (getEquipList().isEmpty()) //没有设备的时候，清理线程
									{
										if (StationItem.EquipCategoryDict.values().contains(this)) //if (StationItem.EquipCategoryDict.ContainsValue(this))
										{
											StationItem.EquipCategoryDict.remove(local_addr); //Remove(local_addr);
										}
										bStartThread = false; //停止线程循环
										setDataRefreshBreak(true); //停止线程循环
									}
								}
								e.getICommunication().CloseCommunication(); //释放通讯资源，比如释放串口，否则资源占用。

								if (bEditLocalAddr)
								{
									bEditLocalAddr = false;
								}
								break;
							}
						}
					}
				}
				if (ChangedEquipList.get(k).State == ChangedEquipState.Edit)
				{
					synchronized (getEquipList())
					{
						for (EquipItem e : getEquipList())
						{
							if (e.getIEquipno() == ChangedEquipList.get(k).iEqpNo)
							{
								//修改时，直接对该设备进行复位操作即可
								e.ResetWhenDBChanged(e.getIStano(), e.getIEquipno());
								if (e.getICommunication() != null)
								{
									e.getICommunication().setResetFlag(true);
								}
								break;
							}
						}
					}
					////////////////////////////////////////////////////////////
					int eqpno = ChangedEquipList.get(k).iEqpNo;
					if (EquipEdit != null)
					{
						for (EquipEditEventHandler listener : EquipEdit.listeners())
						{
							listener.invoke(eqpno, new EventArgs());
						}
					}
				}
				//ChangedEquipList.Remove(ChangedEquipList[k]);//不能在循环过程中删除
			}
			ChangedEquipList.clear();
		}
	}

	public final boolean getExistSetParm()
	{
		synchronized (existsetparm)
		{
			return (Boolean)existsetparm;
		}
	}
	public final void setExistSetParm(boolean value)
	{
		synchronized (existsetparm)
		{
			existsetparm = (Object)(Boolean)value;
		}
	}
	public final boolean getDataRefreshBreak()
	{
		synchronized (datarefreshbreak)
		{
			return (Boolean)datarefreshbreak;
		}
	}
	public final void setDataRefreshBreak(boolean value)
	{
		synchronized (datarefreshbreak)
		{
			datarefreshbreak = (Object)(Boolean)value;
		}
	}
	private Object resetequip = false;
	public final boolean getResetEquips()
	{
		synchronized (resetequip)
		{
			return (Boolean)resetequip;
		}
	}
	public final void setResetEquips(boolean value)
	{
		synchronized (resetequip)
		{
			resetequip = value;
		}
	}

	public SubEquipList(String str_local_addr, ArrayList<EquipTableRow> DataRowList)
	{
		//对挂在同一串口下的设备应该共用一个串口实例,否则会有冲突
		SerialPort spt = new SerialPort();
		local_addr = str_local_addr;

		for (EquipTableRow r : DataRowList)
		{
			int stano = r.getStaN();
			int eqpno = r.getEquipNo();

			EquipItem e = new EquipItem(stano, eqpno, spt, r);
			synchronized (getEquipList())
			{
				getEquipList().add(e);
			}
			synchronized (DataCenter.getEquipItemDict())
			{
				if (!DataCenter.getEquipItemDict().containsKey(eqpno))
				{
					DataCenter.getEquipItemDict().put(eqpno, e);
				}
				else
				{
					DataCenter.getEquipItemDict().remove(eqpno);
					DataCenter.getEquipItemDict().put(eqpno, e);
				}
			}

		}

		ChangedEquipListChanged.removeListener("SubEquipList_ChangedEquipListChanged");
		ChangedEquipListChanged.addListener("SubEquipList_ChangedEquipListChanged", (Object sender, EventArgs e) -> SubEquipList_ChangedEquipListChanged(sender, e));
		if (IsRageMode())
		{
			ThreadInterval = 0;
			synchronized (getEquipList())
			{
				for (EquipItem item : getEquipList())
				{
					item.IsRageMode = true;
				}
			}
		}
	}

	private boolean IsRageMode()
	{
		String[] ss;
		String strResult = DataCenter.GetPropertyFromPropertyService("RunRageMode", "", "");
		if (strResult.strip().toUpperCase().equals("ALL"))
		{
			return true;
		}
		if (sharpSystem.StringHelper.isNullOrEmpty(strResult.strip()))
		{
			return false;
		}
		else
		{
			ss = strResult.split("[%]", -1);
		}
		for (String s : ss)
		{
			if (s.toUpperCase().strip().equals(local_addr.toUpperCase().strip()))
			{
				return true;
			}
		}
		return false;
	}

	public final void Refresh()
	{
		while (bStartThread)
		{
			while (getExistSetParm() && getDataRefreshBreak() && bStartThread) //设置状态
			{
                try {
                    Thread.sleep(ThreadInterval);
                } catch (InterruptedException e) {

                }
            }
			while (!getDataRefreshBreak() && bStartThread) //获取数据状态
			{
				//数据库更新后进行数据重置
				if (getResetEquips())
				{
					try
					{
						synchronized (getEquipList())
						{
							for (EquipItem e : getEquipList())
							{
								e.ResetWhenDBChanged(e.getIStano(), e.getIEquipno());
								if (e.getICommunication() != null)
								{
									e.getICommunication().setResetFlag(true);
								}
							}
						}
					}
					catch (RuntimeException e1)
					{
						DataCenter.WriteLogFile(e1.toString());
					}
					setResetEquips(false);
				}
				try
				{
					EditEquipList();
					EquipRefrash();
				}
				catch (RuntimeException e)
				{
					DataCenter.WriteLogFile(e.toString());
				}
                try {
                    Thread.sleep(ThreadInterval);
                } catch (InterruptedException e) {
                }
            }
		}
	}

	private void EquipRefrash()
	{
		var stateItems = new ArrayList<StateItem>();
		for (EquipItem e : getEquipList())
		{
			try
			{

				synchronized (bChangingState)
				{
					if (!ChangedEquipList.isEmpty()) //如果有设备添加变化，则立即返回
					{
						break;
					}
				}

				if (e.getICommunication() == null)
				{
					continue;
				}
				if (e.isBackupState() == true) //备机状态不实际采集通讯 add by sd in 2020/4/15
				{
					if (e.getState() != EquipState.BackUp)
					{
						e.setState(EquipState.BackUp);
					}

					continue;
				}

				/////////add by sd in 2014/7/30,把设置的设备号放在一个队列里面，以便设备设置后的状态可以尽快在界面上反映出来
				synchronized (SetParmEquipList)
				{
					if (!SetParmEquipList.isEmpty())
					{
						if (!SetParmEquipList.contains(e.getIEquipno()))
						{
							continue;
						}
						else
						{
							SetParmEquipList.remove((Integer)e.getIEquipno());
						}
					}
				}

				////////////////////
				if (!e.getEnable()) //设备不允许运行
				{
					e.setState(EquipState.NoCommunication); //设置成不通讯
					continue;
				}

				e.setIAccNum(e.getIAccNum() + 1);

				if ((e.getIAccNum() % e.getIAccCyc()) == 0)
				{
					e.setIAccNum(0);
					e.setReset(false);
					OldEquip = e;
					if (e.getICommunication() == null)
					{
						continue;
					}

					if (!e.bInitOk || e.getICommunication().getResetFlag()) //如果第一次运行或者有重置才会调用或者通讯恢复后第一次调用才会调用init
					{
						//e.State = EquipState.Initial;//2024-02-05 加入这行是因为级联动态添加设备的时候设备状态数据有时候不上传的原因
						if (!e.getICommunication().init(e))
						{
							e.iCommFaultRetryCount += 1;
							e.getICommunication().setResetFlag(true); //add in 2014-7-2   解决某些设备通讯中断后yxp值出现unknow 的现象
							//通讯重试超过预设次数，则通讯失败
							if (e.getICommunication().getMRetrytime() <= e.iCommFaultRetryCount)
							{
								synchronized (e.EquipRWstate)
								{
									e.bCommunicationOk = false;
									e.setState(EquipState.NoCommunication);
									e.iCommFaultRetryCount = 0;
									e.bInitOk = false;
								}
							}

							continue;
						}
						else
						{
							e.bInitOk = true;
							e.getICommunication().setResetFlag(false);
						}

						if (e.getState() != EquipState.NoCommunication) //2024-04-10 避免一直通讯失败的设备来回在通讯正常和失败之间切换
						{
							e.setState(EquipState.Initial); //2024-02-05 加入这行是因为级联动态添加设备的时候设备状态数据有时候不上传的原因
						}
					}

					CommunicationState ret = e.getICommunication().GetData((CEquipBase)e.getICommunication());
					if (ret == CommunicationState.ok) //数据成功获取
					{
						e.bCommunicationOk = true;
						e.iCommFaultRetryCount = 0;

						synchronized (e.getICommunication().getYCResults())
						{
							// 把pair.Value通过MQTT发出去
							if (!e.getICommunication().getYCResults().isEmpty())
							{
								MqRtValueMessage tempVar = new MqRtValueMessage();
								tempVar.setTime(LocalDateTime.now().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME));
								tempVar.setFlow(UUID.randomUUID().toString());
								DataItem tempVar2 = new DataItem();
								tempVar2.setDeviceId(e.getICommunication().getMEquipNo());
								tempVar2.setAttribute(new HashMap<>(e.getICommunication().getYCResults()));
								tempVar.setDataItems(new ArrayList<>(Collections.singletonList(tempVar2)));
								MqttProvider.Instance.PublishYcRtValueAsync(tempVar);
							}
						}

						synchronized (e.getICommunication().getYXResults())
						{
							// 把pair.Value通过MQTT发出去
							if (!e.getICommunication().getYXResults().isEmpty())
							{
								MqRtValueMessage tempVar3 = new MqRtValueMessage();
								tempVar3.setTime(LocalDateTime.now().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME));
								tempVar3.setFlow(UUID.randomUUID().toString());
								DataItem tempVar4 = new DataItem();
								tempVar4.setDeviceId(e.getICommunication().getMEquipNo());
								tempVar4.setAttribute(new HashMap<>(e.getICommunication().getYXResults()));
								tempVar3.setDataItems(new ArrayList<>(Collections.singletonList(tempVar4)));
								MqttProvider.Instance.PublishYxRtValueAsync(tempVar3);
							}
						}

						//设备返回的事件列表，比如：用于门禁设备的刷卡记录，不同于YC和YX
						synchronized (e.getICommunication().getEquipEventList())
						{
							var equipEventList = e.getICommunication().getEquipEventList();
							if (!equipEventList.isEmpty())
							{
								// 把evt通过MQTT发出去

								var evtMsg = new MqEvtMessage();
								evtMsg.setTime(LocalDateTime.now().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME));
								evtMsg.setFlow(UUID.randomUUID().toString());
								var tempArr = new ArrayList<EquipEventItem>();
								tempArr.addAll(equipEventList.stream().map(evt -> {
											EquipEventItem tempVar5 = new EquipEventItem();
											tempVar5.setMsg(evt.msg);
											tempVar5.setMsg4Linkage(evt.msg4Linkage);
											tempVar5.setLevel(evt.level);
											tempVar5.setOccurDateTime(evt.dt);
											tempVar5.setEquipNo(evt.iEquipNo);
											return tempVar5;
										}).toList());

								var tempEquipEvent = new EquipEvent();
								tempEquipEvent.setDeviceId(e.getICommunication().getMEquipNo());
								tempEquipEvent.setEquipEvents(tempArr);

								evtMsg.setEventItems(new ArrayList<>(Collections.singletonList(tempEquipEvent)));

								MqttProvider.Instance.PublishEvtValueAsync(evtMsg);

								e.getICommunication().getEquipEventList().clear();
							}
						}

						e.setDataFrash(true);
					}

					if (ret == CommunicationState.setreturn)
					{
						if (e.DoSetParm && e.getState() == EquipState.CommunicationOK)
						{
							synchronized (e.EquipRWstate)
							{
								e.setState(EquipState.HaveSetParm);
							}
						}

						setDataRefreshBreak(true);
						return;
					}

					if (ret == CommunicationState.fail)
					{
						e.iCommFaultRetryCount += 1;
						e.getICommunication().setResetFlag(true);
						//通讯重试超过预设次数，则通讯失败
						if (e.getICommunication().getMRetrytime() <= e.iCommFaultRetryCount)
						{
							synchronized (e.EquipRWstate)
							{
								e.bCommunicationOk = false;
								e.setState(EquipState.NoCommunication);
								e.iCommFaultRetryCount = 0;
							}
						}
					}
				}
			}
			catch (RuntimeException ex){
				ex.printStackTrace();
			}
			finally
			{
				if (e.getICommunication() != null)
				{
					StateItem tempVar6 = new StateItem();
					tempVar6.setDeviceId(e.getICommunication().getMEquipNo());
					tempVar6.setState(e.getState().toString());
					stateItems.add(tempVar6);
				}
			}
		}

		if (!stateItems.isEmpty())
		{
			MqRtStateMessage tempVar7 = new MqRtStateMessage();
			tempVar7.setTime(LocalDateTime.now().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME));
			tempVar7.setFlow(UUID.randomUUID().toString());
			tempVar7.setStateItems(stateItems);
			MqttProvider.Instance.PublishRtStateAsync(tempVar7);
		}
	}
	public LocalDateTime StartRefreshTime = LocalDateTime.MIN;
	public final void StartRefreshThread()
	{
		if (!bStartRefreshThread)
		{
			RefreshThread = new Thread(()->{
				Refresh();
			});
			RefreshThread.start();
			bStartRefreshThread = true;
			StartRefreshTime = LocalDateTime.now();
		}
	}

	///////////////////////////////////////////////////

	public final void SetParmScan()
	{
		while (bStartThread)
		{
			try
			{
				synchronized (getEquipList())
				{
					for (EquipItem e : getEquipList())
					{
						{
							if (e.SetItemQueue.size() > 0)
							{
								setExistSetParm(true);
								SendSetParmFlag(true);
								e.DoSetParm = true;
								SetItem setitem;
								sharpSystem.OutObject<SetItem> tempOut_setitem = new sharpSystem.OutObject<SetItem>();
								e.SetItemQueue.remove(tempOut_setitem);
								setitem = tempOut_setitem.outArgValue;
								if ((!e.Reserve3.toUpperCase().strip().equals("FASTSET")) && (!setitem.getType().toUpperCase().equals("S"))) //比如云台控制等对设置时效性要求很高的设备，或者系统命令，就不再等待数据刷新的终止
								{
									while (!getDataRefreshBreak()) //等待,确保数据刷新过程已经中止
									{
										Thread.sleep(ThreadInterval); //必须sleep,否则DataRefreshBreak在同步的时候会耗时较大
									}
								}
								try
								{
									SetParm(e, setitem);
									/////////add by sd in 2014/7/30,把设置的设备号放在一个队列里面，以便设备设置后的状态可以尽快在界面上反映出来
									synchronized (SetParmEquipList)
									{
										if (!SetParmEquipList.contains(e.getIEquipno()))
										{
											SetParmEquipList.add(e.getIEquipno());
										}
									}
								}
								catch (RuntimeException e1)
								{
									DataCenter.WriteLogFile(e1.toString());
								}

								e.DoSetParm = false;
							}
						}
					}
					setExistSetParm(false);
					setDataRefreshBreak(false);
					SendSetParmFlag(false);
				}
			}
			catch (RuntimeException e)
			{
				DataCenter.WriteLogFile(e.toString());
			} catch (InterruptedException e) {

            }

            try {
                Thread.sleep(ThreadInterval);
            } catch (InterruptedException e) {

            }
        }
	}
	private Object bLockSetParm = true;
	public final void SetParm(EquipItem e, SetItem setitem)
	{
		//SetParm函数采用线程池模式执行，避免某些设置函数由于耗时阻塞其它设备的设置。

		new Thread(new Runnable() {
			@Override
			public void run() {
				synchronized (bLockSetParm) //增加了DoEquipSetItem方式，要考虑线程安全
				{
					if (e.getICommunication() == null)
					{
						return;
					}
					if (e.getEquipBase() == null)
					{
						return;
					}

					e.setState(EquipState.HaveSetParm);
					//        e.DataFrash = true;
					String msg = "";
					e.getICommunication().init(e);

					if (setitem.getBStopSetParm()) //中断的设置不执行
					{
						return;
					}

					String s1, s2, s3, s4;
					s1 = ResourceService.GetString("AlarmCenter.DataCenter.Msg2");
					s2 = ResourceService.GetString("AlarmCenter.DataCenter.Msg3");
					s3 = ResourceService.GetString("AlarmCenter.DataCenter.Msg4");
					s4 = ResourceService.GetString("AlarmCenter.DataCenter.Msg5");
					String desc = setitem.GetSetItemDesc();

					if (setitem.getType() == null)
					{
						return;
					}

					e.getICommunication().setParmExecutor(setitem.getExecutor()); //add by sd in 20160801
					String csExecutor;
					csExecutor = setitem.getExecutor();
					if (sharpSystem.StringHelper.isNullOrEmpty(setitem.getExecutor()))
					{
						csExecutor = setitem.sysExecutor;
					}

					e.setCurSetItem(setitem); //传入当前设置项,设备dll在返回响应值的时候可能会用到

					if (e.getICommunication().SetParm(setitem.getMainInstruct(), setitem.getMinorInstruct(), setitem.getValue()))
					{
						setitem.setWaitSetParmIsFinish(true);
						if (desc == null)
						{
							msg += String.format(s1 + "{0}---" + s2 + s4 + "{1}-{2}-{3}", e.getEquipNm(), setitem.getMainInstruct(), setitem.getMinorInstruct(), setitem.getValue()) + "---by:" + csExecutor;
						}
						else
						{
							msg += desc + ">>" + s2 + "---by:" + csExecutor;
						}

					}
					else
					{
						setitem.setWaitSetParmIsFinish(false);
						if (desc == null)
						{
							msg += String.format(s1 + "{0}---" + s3 + s4 + "{1}-{2}-{3}", e.getEquipNm(), setitem.getMainInstruct(), setitem.getMinorInstruct(), setitem.getValue()) + "---by:" + csExecutor;
						}
						else
						{
							msg += desc + ">>" + s3 + "---by:" + csExecutor;
						}
						System.out.println(msg);
					}
					//StationItem.FireSetParmResponseEvent(setitem.EquipNo, setitem.m_SetNo, setitem.Value, setitem.csResponse, bSetFinish, setitem.RequestId);
					if (!setitem.isSynchronization) //只有异步命令才激发设置结果事件
					{
						StationItem.FireSetParmResultEvent(setitem);
					}
				}
			}
		}).run();
	}

	public final void SendSetParmFlag(boolean flag)
	{
		synchronized (getEquipList())
		{
			for (EquipItem e : getEquipList())
			{
				if (e.getICommunication() != null)
				{
					e.getICommunication().setRunSetParmFlag(flag);
				}
			}
		}
	}

	public final void StartSetParmThread()
	{
		if (!bStartSetParmThread)
		{
			SetParmThread = new Thread(() -> SetParmScan());
			SetParmThread.start();
			bStartSetParmThread = true;
		}
	}

}