package gwdatacenter.database;

import gwdatacenter.*;
import java.time.*;

public class YcpTableRow
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
	private int yc_no;
	public final int getYcNo()
	{
		return yc_no;
	}
	public final void setYcNo(int value)
	{
		yc_no = value;
	}
	private String yc_nm;
	public final String getYcNm()
	{
		return yc_nm;
	}
	public final void setYcNm(String value)
	{
		yc_nm = value;
	}
	private boolean mapping;
	public final boolean getMapping()
	{
		return mapping;
	}
	public final void setMapping(boolean value)
	{
		mapping = value;
	}
	private double yc_min;
	public final double getYcMin()
	{
		return yc_min;
	}
	public final void setYcMin(double value)
	{
		yc_min = value;
	}
	private double yc_max;
	public final double getYcMax()
	{
		return yc_max;
	}
	public final void setYcMax(double value)
	{
		yc_max = value;
	}
	private double physic_min;
	public final double getPhysicMin()
	{
		return physic_min;
	}
	public final void setPhysicMin(double value)
	{
		physic_min = value;
	}
	private double physic_max;
	public final double getPhysicMax()
	{
		return physic_max;
	}
	public final void setPhysicMax(double value)
	{
		physic_max = value;
	}
	private double val_min;
	public final double getValMin()
	{
		return val_min;
	}
	public final void setValMin(double value)
	{
		val_min = value;
	}
	private double restore_min;
	public final double getRestoreMin()
	{
		return restore_min;
	}
	public final void setRestoreMin(double value)
	{
		restore_min = value;
	}
	private double restore_max;
	public final double getRestoreMax()
	{
		return restore_max;
	}
	public final void setRestoreMax(double value)
	{
		restore_max = value;
	}
	private double val_max;
	public final double getValMax()
	{
		return val_max;
	}
	public final void setValMax(double value)
	{
		val_max = value;
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
	private String proc_advice;
	public final String getProcAdvice()
	{
		return proc_advice;
	}
	public final void setProcAdvice(String value)
	{
		proc_advice = value;
	}
	private int lvl_level;
	public final int getLvlLevel()
	{
		return lvl_level;
	}
	public final void setLvlLevel(int value)
	{
		lvl_level = value;
	}
	private String outmin_evt;
	public final String getOutminEvt()
	{
		return outmin_evt;
	}
	public final void setOutminEvt(String value)
	{
		outmin_evt = value;
	}
	private String outmax_evt;
	public final String getOutmaxEvt()
	{
		return outmax_evt;
	}
	public final void setOutmaxEvt(String value)
	{
		outmax_evt = value;
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
	private boolean curve_rcd;
	public final boolean getCurveRcd()
	{
		return curve_rcd;
	}
	public final void setCurveRcd(boolean value)
	{
		curve_rcd = value;
	}
	private Double curve_limit = null;
	public final Double getCurveLimit()
	{
		return curve_limit;
	}
	public final void setCurveLimit(Double value)
	{
		curve_limit = value;
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
	private String unit;
	public final String getUnit()
	{
		return unit;
	}
	public final void setUnit(String value)
	{
		unit = value;
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
	private String yc_code;
	public final String getYcCode()
	{
		return yc_code;
	}
	public final void setYcCode(String value)
	{
		yc_code = value;
	}


}