package gwdatacenter.database;

import gwdatacenter.*;
import java.time.*;

public class YxpTableRow
{
	private int sta_n;
	public final int getStaN()
	{
		return sta_n;
	}
	public final void setStaN(int value)
	{
		sta_n = value;
	}
	private int equip_no;
	public final int getEquipNo()
	{
		return equip_no;
	}
	public final void setEquipNo(int value)
	{
		equip_no = value;
	}
	private int yx_no;
	public final int getYxNo()
	{
		return yx_no;
	}
	public final void setYxNo(int value)
	{
		yx_no = value;
	}
	private String yx_nm;
	public final String getYxNm()
	{
		return yx_nm;
	}
	public final void setYxNm(String value)
	{
		yx_nm = value;
	}
	private String proc_advice_r;
	public final String getProcAdviceR()
	{
		return proc_advice_r;
	}
	public final void setProcAdviceR(String value)
	{
		proc_advice_r = value;
	}
	private String proc_advice_d;
	public final String getProcAdviceD()
	{
		return proc_advice_d;
	}
	public final void setProcAdviceD(String value)
	{
		proc_advice_d = value;
	}
	private int level_r;
	public final int getLevelR()
	{
		return level_r;
	}
	public final void setLevelR(int value)
	{
		level_r = value;
	}
	private int level_d;
	public final int getLevelD()
	{
		return level_d;
	}
	public final void setLevelD(int value)
	{
		level_d = value;
	}
	private String evt_01;
	public final String getEvt_01()
	{
		return evt_01;
	}
	public final void setEvt_01(String value)
	{
		evt_01 = value;
	}
	private String evt_10;
	public final String getEvt_10()
	{
		return evt_10;
	}
	public final void setEvt_10(String value)
	{
		evt_10 = value;
	}
	private String main_instruction;
	public final String getMainInstruction()
	{
		return main_instruction;
	}
	public final void setMainInstruction(String value)
	{
		main_instruction = value;
	}
	private String minor_instruction;
	public final String getMinorInstruction()
	{
		return minor_instruction;
	}
	public final void setMinorInstruction(String value)
	{
		minor_instruction = value;
	}
	private Integer alarm_acceptable_time = null;
	public final Integer getAlarmAcceptableTime()
	{
		return alarm_acceptable_time;
	}
	public final void setAlarmAcceptableTime(Integer value)
	{
		alarm_acceptable_time = value;
	}
	private Integer restore_acceptable_time = null;
	public final Integer getRestoreAcceptableTime()
	{
		return restore_acceptable_time;
	}
	public final void setRestoreAcceptableTime(Integer value)
	{
		restore_acceptable_time = value;
	}
	private int alarm_repeat_time;
	public final int getAlarmRepeatTime()
	{
		return alarm_repeat_time;
	}
	public final void setAlarmRepeatTime(int value)
	{
		alarm_repeat_time = value;
	}
	private String wave_file;
	public final String getWaveFile()
	{
		return wave_file;
	}
	public final void setWaveFile(String value)
	{
		wave_file = value;
	}
	private String related_pic;
	public final String getRelatedPic()
	{
		return related_pic;
	}
	public final void setRelatedPic(String value)
	{
		related_pic = value;
	}
	private int alarm_scheme;
	public final int getAlarmScheme()
	{
		return alarm_scheme;
	}
	public final void setAlarmScheme(int value)
	{
		alarm_scheme = value;
	}
	private boolean inversion;
	public final boolean getInversion()
	{
		return inversion;
	}
	public final void setInversion(boolean value)
	{
		inversion = value;
	}
	private boolean curve_rcd;
	public final boolean getCurveRcd()
	{
		return curve_rcd;
	}
	public final void setCurveRcd(boolean value)
	{
		curve_rcd = value;
	}
	private int initval;
	public final int getInitval()
	{
		return initval;
	}
	public final void setInitval(int value)
	{
		initval = value;
	}
	private int val_trait;
	public final int getValTrait()
	{
		return val_trait;
	}
	public final void setValTrait(int value)
	{
		val_trait = value;
	}
	private String alarm_shield;
	public final String getAlarmShield()
	{
		return alarm_shield;
	}
	public final void setAlarmShield(String value)
	{
		alarm_shield = value;
	}
	private Integer AlarmRiseCycle = null;
	public final Integer getAlarmRiseCycle()
	{
		return AlarmRiseCycle;
	}
	public final void setAlarmRiseCycle(Integer value)
	{
		AlarmRiseCycle = value;
	}
	private String Reserve1;
	public final String getReserve1()
	{
		return Reserve1;
	}
	public final void setReserve1(String value)
	{
		Reserve1 = value;
	}
	private String Reserve2;
	public final String getReserve2()
	{
		return Reserve2;
	}
	public final void setReserve2(String value)
	{
		Reserve2 = value;
	}
	private String Reserve3;
	public final String getReserve3()
	{
		return Reserve3;
	}
	public final void setReserve3(String value)
	{
		Reserve3 = value;
	}
	private String related_video;
	public final String getRelatedVideo()
	{
		return related_video;
	}
	public final void setRelatedVideo(String value)
	{
		related_video = value;
	}
	private String ZiChanID;
	public final String getZiChanID()
	{
		return ZiChanID;
	}
	public final void setZiChanID(String value)
	{
		ZiChanID = value;
	}
	private String PlanNo;
	public final String getPlanNo()
	{
		return PlanNo;
	}
	public final void setPlanNo(String value)
	{
		PlanNo = value;
	}
	private String SafeTime;
	public final String getSafeTime()
	{
		return SafeTime;
	}
	public final void setSafeTime(String value)
	{
		SafeTime = value;
	}
	private String GWValue;
	public final String getGWValue()
	{
		return GWValue;
	}
	public final void setGWValue(String value)
	{
		GWValue = value;
	}
	private LocalDateTime GWTime = null;
	public final LocalDateTime getGWTime()
	{
		return GWTime;
	}
	public final void setGWTime(LocalDateTime value)
	{
		GWTime = value;
	}
	private String datatype;
	public final String getDatatype()
	{
		return datatype;
	}
	public final void setDatatype(String value)
	{
		datatype = value;
	}
	private String yx_code;
	public final String getYxCode()
	{
		return yx_code;
	}
	public final void setYxCode(String value)
	{
		yx_code = value;
	}

}