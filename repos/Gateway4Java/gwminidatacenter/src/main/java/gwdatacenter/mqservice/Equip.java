package gwdatacenter.mqservice;

import gwdatacenter.database.*;
import gwdatacenter.*;
import java.util.*;
import java.time.*;

public class Equip {
	public int EquipNo;

	public final int getEquipNo() {
		return EquipNo;
	}

	public final void setEquipNo(int value) {
		EquipNo = value;
	}

	public int StaN;

	public final int getStaN() {
		return StaN;
	}

	public final void setStaN(int value) {
		StaN = value;
	}

	public String EquipNm;

	public final String getEquipNm() {
		return EquipNm;
	}

	public final void setEquipNm(String value) {
		EquipNm = value;
	}

	public String EquipDetail;

	public final String getEquipDetail() {
		return EquipDetail;
	}

	public final void setEquipDetail(String value) {
		EquipDetail = value;
	}

	public int AccCyc;

	public final int getAccCyc() {
		return AccCyc;
	}

	public final void setAccCyc(int value) {
		AccCyc = value;
	}

	public String RelatedPic;

	public final String getRelatedPic() {
		return RelatedPic;
	}

	public final void setRelatedPic(String value) {
		RelatedPic = value;
	}

	public String ProcAdvice;

	public final String getProcAdvice() {
		return ProcAdvice;
	}

	public final void setProcAdvice(String value) {
		ProcAdvice = value;
	}

	public String OutOfContact;

	public final String getOutOfContact() {
		return OutOfContact;
	}

	public final void setOutOfContact(String value) {
		OutOfContact = value;
	}

	public String Contacted;

	public final String getContacted() {
		return Contacted;
	}

	public final void setContacted(String value) {
		Contacted = value;
	}

	public String EventWav;

	public final String getEventWav() {
		return EventWav;
	}

	public final void setEventWav(String value) {
		EventWav = value;
	}

	public String CommunicationDrv;

	public final String getCommunicationDrv() {
		return CommunicationDrv;
	}

	public final void setCommunicationDrv(String value) {
		CommunicationDrv = value;
	}

	public String LocalAddr;

	public final String getLocalAddr() {
		return LocalAddr;
	}

	public final void setLocalAddr(String value) {
		LocalAddr = value;
	}

	public String EquipAddr;

	public final String getEquipAddr() {
		return EquipAddr;
	}

	public final void setEquipAddr(String value) {
		EquipAddr = value;
	}

	public String CommunicationParam;

	public final String getCommunicationParam() {
		return CommunicationParam;
	}

	public final void setCommunicationParam(String value) {
		CommunicationParam = value;
	}

	public String CommunicationTimeParam;

	public final String getCommunicationTimeParam() {
		return CommunicationTimeParam;
	}

	public final void setCommunicationTimeParam(String value) {
		CommunicationTimeParam = value;
	}

	public int RawEquipNo;

	public final int getRawEquipNo() {
		return RawEquipNo;
	}

	public final void setRawEquipNo(int value) {
		RawEquipNo = value;
	}

	public String Tabname;

	public final String getTabname() {
		return Tabname;
	}

	public final void setTabname(String value) {
		Tabname = value;
	}

	public int AlarmScheme;

	public final int getAlarmScheme() {
		return AlarmScheme;
	}

	public final void setAlarmScheme(int value) {
		AlarmScheme = value;
	}

	public int Attrib;

	public final int getAttrib() {
		return Attrib;
	}

	public final void setAttrib(int value) {
		Attrib = value;
	}

	public String StaIp;

	public final String getStaIp() {
		return StaIp;
	}

	public final void setStaIp(String value) {
		StaIp = value;
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

	public String Backup;

	public final String getBackup() {
		return Backup;
	}

	public final void setBackup(String value) {
		Backup = value;
	}

	public ArrayList<Ycp> Ycps = new ArrayList<Ycp>();

	public final ArrayList<Ycp> getYcps() {
		return Ycps;
	}

	public final void setYcps(ArrayList<Ycp> value) {
		Ycps = value;
	}

	public ArrayList<Yxp> Yxps = new ArrayList<Yxp>();

	public final ArrayList<Yxp> getYxps() {
		return Yxps;
	}

	public final void setYxps(ArrayList<Yxp> value) {
		Yxps = value;
	}

	public ArrayList<SetParm> SetParms = new ArrayList<SetParm>();

	public final ArrayList<SetParm> getSetParms() {
		return SetParms;
	}

	public final void setParms(ArrayList<SetParm> value) {
		SetParms = value;
	}
}