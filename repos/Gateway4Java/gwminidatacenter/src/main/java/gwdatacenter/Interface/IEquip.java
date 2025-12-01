package gwdatacenter.Interface;

import gwdatacenter.args.EquipEvent;
import gwdatacenter.EquipItem;

import java.util.*;
import java.util.concurrent.ConcurrentHashMap;

public interface IEquip
{
	int getMStaNo();
	void setMStaNo(int value);
	int getMEquipNo();
	void setMEquipNo(int value);
	int getMRetrytime();
	void setMRetrytime(int value);
	//YC点
	ConcurrentHashMap<Integer, Object> getYCResults();
	//YX点
	ConcurrentHashMap<Integer, Object> getYXResults();
	//事件列表
	ArrayList<EquipEvent> getEquipEventList();

	boolean getRunSetParmFlag();
	void setRunSetParmFlag(boolean value);

	boolean getResetFlag();
	void setResetFlag(boolean value);

	EquipItem getEquipitem();
	void setEquipitem(EquipItem value);

	boolean getBCanConfirm2NormalState();
	void setBCanConfirm2NormalState(boolean value);

	String getSetParmExecutor();
	void setParmExecutor(String value);
	boolean init(EquipItem eqpitm);
	CommunicationState GetData(CEquipBase p);
	boolean SetParm(String cmd1, String cmd2, String value);

	boolean Confirm2NormalState(String sYcYxType, int iYcYxNo);
	boolean CloseCommunication(); //用于释放通讯资源
}