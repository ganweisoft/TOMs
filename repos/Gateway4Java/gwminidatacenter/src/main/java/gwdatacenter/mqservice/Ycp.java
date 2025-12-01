package gwdatacenter.mqservice;

import gwdatacenter.database.*;
import gwdatacenter.*;
import java.util.*;
import java.time.*;

	public class Ycp {
		public int YcNo;

		public final int getYcNo() {
			return YcNo;
		}

		public final void setYcNo(int value) {
			YcNo = value;
		}

		public int StaN;

		public final int getStaN() {
			return StaN;
		}

		public final void setStaN(int value) {
			StaN = value;
		}

		public int EquipNo;

		public final int getEquipNo() {
			return EquipNo;
		}

		public final void setEquipNo(int value) {
			EquipNo = value;
		}

		public String YcNm;

		public final String getYcNm() {
			return YcNm;
		}

		public final void setYcNm(String value) {
			YcNm = value;
		}

		public Boolean Mapping = null;

		public final Boolean getMapping() {
			return Mapping;
		}

		public final void setMapping(Boolean value) {
			Mapping = value;
		}

		public double YcMin;

		public final double getYcMin() {
			return YcMin;
		}

		public final void setYcMin(double value) {
			YcMin = value;
		}

		public double YcMax;

		public final double getYcMax() {
			return YcMax;
		}

		public final void setYcMax(double value) {
			YcMax = value;
		}

		public double PhysicMin;

		public final double getPhysicMin() {
			return PhysicMin;
		}

		public final void setPhysicMin(double value) {
			PhysicMin = value;
		}

		public double PhysicMax;

		public final double getPhysicMax() {
			return PhysicMax;
		}

		public final void setPhysicMax(double value) {
			PhysicMax = value;
		}

		public double ValMin;

		public final double getValMin() {
			return ValMin;
		}

		public final void setValMin(double value) {
			ValMin = value;
		}

		public double RestoreMin;

		public final double getRestoreMin() {
			return RestoreMin;
		}

		public final void setRestoreMin(double value) {
			RestoreMin = value;
		}

		public double RestoreMax;

		public final double getRestoreMax() {
			return RestoreMax;
		}

		public final void setRestoreMax(double value) {
			RestoreMax = value;
		}

		public double ValMax;

		public final double getValMax() {
			return ValMax;
		}

		public final void setValMax(double value) {
			ValMax = value;
		}

		public int ValTrait;

		public final int getValTrait() {
			return ValTrait;
		}

		public final void setValTrait(int value) {
			ValTrait = value;
		}

		public String MainInstruction;

		public final String getMainInstruction() {
			return MainInstruction;
		}

		public final void setMainInstruction(String value) {
			MainInstruction = value;
		}

		public String MinorInstruction;

		public final String getMinorInstruction() {
			return MinorInstruction;
		}

		public final void setMinorInstruction(String value) {
			MinorInstruction = value;
		}

		public int AlarmAcceptableTime;

		public final int getAlarmAcceptableTime() {
			return AlarmAcceptableTime;
		}

		public final void setAlarmAcceptableTime(int value) {
			AlarmAcceptableTime = value;
		}

		public int RestoreAcceptableTime;

		public final int getRestoreAcceptableTime() {
			return RestoreAcceptableTime;
		}

		public final void setRestoreAcceptableTime(int value) {
			RestoreAcceptableTime = value;
		}

		public int AlarmRepeatTime;

		public final int getAlarmRepeatTime() {
			return AlarmRepeatTime;
		}

		public final void setAlarmRepeatTime(int value) {
			AlarmRepeatTime = value;
		}

		public String ProcAdvice;

		public final String getProcAdvice() {
			return ProcAdvice;
		}

		public final void setProcAdvice(String value) {
			ProcAdvice = value;
		}

		public int LvlLevel;

		public final int getLvlLevel() {
			return LvlLevel;
		}

		public final void setLvlLevel(int value) {
			LvlLevel = value;
		}

		public String OutminEvt;

		public final String getOutminEvt() {
			return OutminEvt;
		}

		public final void setOutminEvt(String value) {
			OutminEvt = value;
		}

		public String OutmaxEvt;

		public final String getOutmaxEvt() {
			return OutmaxEvt;
		}

		public final void setOutmaxEvt(String value) {
			OutmaxEvt = value;
		}

		public String WaveFile;

		public final String getWaveFile() {
			return WaveFile;
		}

		public final void setWaveFile(String value) {
			WaveFile = value;
		}

		public String RelatedPic;

		public final String getRelatedPic() {
			return RelatedPic;
		}

		public final void setRelatedPic(String value) {
			RelatedPic = value;
		}

		public int AlarmScheme;

		public final int getAlarmScheme() {
			return AlarmScheme;
		}

		public final void setAlarmScheme(int value) {
			AlarmScheme = value;
		}

		public boolean CurveRcd;

		public final boolean getCurveRcd() {
			return CurveRcd;
		}

		public final void setCurveRcd(boolean value) {
			CurveRcd = value;
		}

		public Double CurveLimit = null;

		public final Double getCurveLimit() {
			return CurveLimit;
		}

		public final void setCurveLimit(Double value) {
			CurveLimit = value;
		}

		public String AlarmShield;

		public final String getAlarmShield() {
			return AlarmShield;
		}

		public final void setAlarmShield(String value) {
			AlarmShield = value;
		}

		public String Unit;

		public final String getUnit() {
			return Unit;
		}

		public final void setUnit(String value) {
			Unit = value;
		}

		public Integer AlarmRiseCycle = null;

		public final Integer getAlarmRiseCycle() {
			return AlarmRiseCycle;
		}

		public final void setAlarmRiseCycle(Integer value) {
			AlarmRiseCycle = value;
		}

		public String Reserve1;

		public final String getReserve1() {
			return Reserve1;
		}

		public final void setReserve1(String value) {
			Reserve1 = value;
		}

		public String Reserve2;

		public final String getReserve2() {
			return Reserve2;
		}

		public final void setReserve2(String value) {
			Reserve2 = value;
		}

		public String Reserve3;

		public final String getReserve3() {
			return Reserve3;
		}

		public final void setReserve3(String value) {
			Reserve3 = value;
		}

		public String RelatedVideo;

		public final String getRelatedVideo() {
			return RelatedVideo;
		}

		public final void setRelatedVideo(String value) {
			RelatedVideo = value;
		}

		public String ZiChanId;

		public final String getZiChanId() {
			return ZiChanId;
		}

		public final void setZiChanId(String value) {
			ZiChanId = value;
		}

		public String PlanNo;

		public final String getPlanNo() {
			return PlanNo;
		}

		public final void setPlanNo(String value) {
			PlanNo = value;
		}

		public String SafeTime;

		public final String getSafeTime() {
			return SafeTime;
		}

		public final void setSafeTime(String value) {
			SafeTime = value;
		}

		public LocalDateTime SafeBgn = null;

		public final LocalDateTime getSafeBgn() {
			return SafeBgn;
		}

		public final void setSafeBgn(LocalDateTime value) {
			SafeBgn = value;
		}

		public LocalDateTime SafeEnd = null;

		public final LocalDateTime getSafeEnd() {
			return SafeEnd;
		}

		public final void setSafeEnd(LocalDateTime value) {
			SafeEnd = value;
		}

		public String GWValue;

		public final String getGWValue() {
			return GWValue;
		}

		public final void setGWValue(String value) {
			GWValue = value;
		}

		public LocalDateTime GWTime = null;

		public final LocalDateTime getGWTime() {
			return GWTime;
		}

		public final void setGWTime(LocalDateTime value) {
			GWTime = value;
		}

		public String DataType;

		public final String getDataType() {
			return DataType;
		}

		public final void setDataType(String value) {
			DataType = value;
		}

		public String YcCode;

		public final String getYcCode() {
			return YcCode;
		}

		public final void setYcCode(String value) {
			YcCode = value;
		}
	}