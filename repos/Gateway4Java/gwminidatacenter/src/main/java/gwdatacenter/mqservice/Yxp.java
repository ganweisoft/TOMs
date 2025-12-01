package gwdatacenter.mqservice;

import gwdatacenter.database.*;
import gwdatacenter.*;
import java.util.*;
import java.time.*;

	public class Yxp {
		private int YxNo;

		public final int getYxNo() {
			return YxNo;
		}

		public final void setYxNo(int value) {
			YxNo = value;
		}

		private int StaN;

		public final int getStaN() {
			return StaN;
		}

		public final void setStaN(int value) {
			StaN = value;
		}

		private int EquipNo;

		public final int getEquipNo() {
			return EquipNo;
		}

		public final void setEquipNo(int value) {
			EquipNo = value;
		}

		private String YxNm;

		public final String getYxNm() {
			return YxNm;
		}

		public final void setYxNm(String value) {
			YxNm = value;
		}

		private String ProcAdviceR;

		public final String getProcAdviceR() {
			return ProcAdviceR;
		}

		public final void setProcAdviceR(String value) {
			ProcAdviceR = value;
		}

		private String ProcAdviceD;

		public final String getProcAdviceD() {
			return ProcAdviceD;
		}

		public final void setProcAdviceD(String value) {
			ProcAdviceD = value;
		}

		private int LevelR;

		public final int getLevelR() {
			return LevelR;
		}

		public final void setLevelR(int value) {
			LevelR = value;
		}

		private int LevelD;

		public final int getLevelD() {
			return LevelD;
		}

		public final void setLevelD(int value) {
			LevelD = value;
		}

		private String Evt01;

		public final String getEvt01() {
			return Evt01;
		}

		public final void setEvt01(String value) {
			Evt01 = value;
		}

		private String Evt10;

		public final String getEvt10() {
			return Evt10;
		}

		public final void setEvt10(String value) {
			Evt10 = value;
		}

		private String MainInstruction;

		public final String getMainInstruction() {
			return MainInstruction;
		}

		public final void setMainInstruction(String value) {
			MainInstruction = value;
		}

		private String MinorInstruction;

		public final String getMinorInstruction() {
			return MinorInstruction;
		}

		public final void setMinorInstruction(String value) {
			MinorInstruction = value;
		}

		private int AlarmAcceptableTime;

		public final int getAlarmAcceptableTime() {
			return AlarmAcceptableTime;
		}

		public final void setAlarmAcceptableTime(int value) {
			AlarmAcceptableTime = value;
		}

		private int RestoreAcceptableTime;

		public final int getRestoreAcceptableTime() {
			return RestoreAcceptableTime;
		}

		public final void setRestoreAcceptableTime(int value) {
			RestoreAcceptableTime = value;
		}

		private int AlarmRepeatTime;

		public final int getAlarmRepeatTime() {
			return AlarmRepeatTime;
		}

		public final void setAlarmRepeatTime(int value) {
			AlarmRepeatTime = value;
		}

		private String WaveFile;

		public final String getWaveFile() {
			return WaveFile;
		}

		public final void setWaveFile(String value) {
			WaveFile = value;
		}

		private String RelatedPic;

		public final String getRelatedPic() {
			return RelatedPic;
		}

		public final void setRelatedPic(String value) {
			RelatedPic = value;
		}

		private int AlarmScheme;

		public final int getAlarmScheme() {
			return AlarmScheme;
		}

		public final void setAlarmScheme(int value) {
			AlarmScheme = value;
		}

		private boolean Inversion;

		public final boolean getInversion() {
			return Inversion;
		}

		public final void setInversion(boolean value) {
			Inversion = value;
		}

		private int Initval;

		public final int getInitval() {
			return Initval;
		}

		public final void setInitval(int value) {
			Initval = value;
		}

		private int ValTrait;

		public final int getValTrait() {
			return ValTrait;
		}

		public final void setValTrait(int value) {
			ValTrait = value;
		}

		private String AlarmShield;

		public final String getAlarmShield() {
			return AlarmShield;
		}

		public final void setAlarmShield(String value) {
			AlarmShield = value;
		}

		private Integer AlarmRiseCycle = null;

		public final Integer getAlarmRiseCycle() {
			return AlarmRiseCycle;
		}

		public final void setAlarmRiseCycle(Integer value) {
			AlarmRiseCycle = value;
		}

		private String RelatedVideo;

		public final String getRelatedVideo() {
			return RelatedVideo;
		}

		public final void setRelatedVideo(String value) {
			RelatedVideo = value;
		}

		private String ZiChanId;

		public final String getZiChanId() {
			return ZiChanId;
		}

		public final void setZiChanId(String value) {
			ZiChanId = value;
		}

		private String PlanNo;

		public final String getPlanNo() {
			return PlanNo;
		}

		public final void setPlanNo(String value) {
			PlanNo = value;
		}

		private String SafeTime;

		public final String getSafeTime() {
			return SafeTime;
		}

		public final void setSafeTime(String value) {
			SafeTime = value;
		}

		private boolean CurveRcd;

		public final boolean getCurveRcd() {
			return CurveRcd;
		}

		public final void setCurveRcd(boolean value) {
			CurveRcd = value;
		}

		private String Reserve1;

		public final String getReserve1() {
			return Reserve1;
		}

		public final void setReserve1(String value) {
			Reserve1 = value;
		}

		private String Reserve2;

		public final String getReserve2() {
			return Reserve2;
		}

		public final void setReserve2(String value) {
			Reserve2 = value;
		}

		private String Reserve3;

		public final String getReserve3() {
			return Reserve3;
		}

		public final void setReserve3(String value) {
			Reserve3 = value;
		}

		private LocalDateTime SafeBgn = null;

		public final LocalDateTime getSafeBgn() {
			return SafeBgn;
		}

		public final void setSafeBgn(LocalDateTime value) {
			SafeBgn = value;
		}

		private LocalDateTime SafeEnd = null;

		public final LocalDateTime getSafeEnd() {
			return SafeEnd;
		}

		public final void setSafeEnd(LocalDateTime value) {
			SafeEnd = value;
		}

		private String GWValue;

		public final String getGWValue() {
			return GWValue;
		}

		public final void setGWValue(String value) {
			GWValue = value;
		}

		private LocalDateTime GWTime = null;

		public final LocalDateTime getGWTime() {
			return GWTime;
		}

		public final void setGWTime(LocalDateTime value) {
			GWTime = value;
		}

		private String DataType;

		public final String getDataType() {
			return DataType;
		}

		public final void setDataType(String value) {
			DataType = value;
		}

		private Equip Equip;

		public final Equip getEquip() {
			return Equip;
		}

		public final void setEquip(Equip value) {
			Equip = value;
		}

		private String YxCode;

		public final String getYxCode() {
			return YxCode;
		}

		public final void setYxCode(String value) {
			YxCode = value;
		}
	}