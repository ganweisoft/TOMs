package gwdatacenter.database;

import gwdatacenter.mqservice.*;
import gwdatacenter.*;
import java.util.*;
import java.util.function.Function;

import static java.util.stream.Collectors.toList;

public class GWDbProvider
{
	public GWDbProvider()
	{
	}
	public static GWDbProvider Instance = new GWDbProvider();
	public final void Init() throws InterruptedException {
		//TODO: 从MQTT服务器获取json数据并做处理
		MqttProvider.Instance.EquipInited.addListener((sender, args) -> this.setInitCompleted(true));
		MqttProvider.Instance.Init();
		while (!this.getInitCompleted())
		{
			Thread.sleep(500);
		}
	}

	private boolean InitCompleted;
	public final boolean getInitCompleted()
	{
		return InitCompleted;
	}
	public final void setInitCompleted(boolean value)
	{
		InitCompleted = value;
	}

	public final ArrayList<EquipTableRow> GetEquipTableList()
	{
		ArrayList<EquipTableRow> temp = new ArrayList<EquipTableRow>();
		// 从MQTT服务器获取到的json数据进行转换
		for (var equip : MqttProvider.Instance.EquipTableRows)
		{
			EquipTableRow tempVar = new EquipTableRow();
			tempVar.setStaN(equip.getStaN());
			tempVar.setEquipNo(equip.getEquipNo());
			tempVar.setEquipNm(equip.getEquipNm());
			tempVar.setEquipDetail(equip.getEquipDetail());
			tempVar.setAccCyc(equip.getAccCyc());
			tempVar.setRelatedPic(equip.getRelatedPic());
			tempVar.setProcAdvice(equip.getProcAdvice());
			tempVar.setOutOfContact(equip.getOutOfContact());
			tempVar.setContacted(equip.getContacted());
			tempVar.setEventWav(equip.getEventWav());
			tempVar.setCommunicationDrv(equip.getCommunicationDrv());
			tempVar.setLocalAddr(equip.getLocalAddr());
			tempVar.setEquipAddr(equip.getEquipAddr());
			tempVar.setCommunicationParam(equip.getCommunicationParam());
			tempVar.setCommunicationTimeParam(equip.getCommunicationTimeParam());
			tempVar.setRawEquipNo(equip.getRawEquipNo());
			tempVar.setTabname(equip.getTabname());
			tempVar.setAlarmScheme(equip.getAlarmScheme());
			tempVar.setAttrib(equip.getAttrib());
			tempVar.setStaIP(equip.getStaIp());
			tempVar.setAlarmRiseCycle(equip.getAlarmRiseCycle());
			tempVar.setReserve1(equip.getReserve1());
			tempVar.setReserve2(equip.getReserve2());
			tempVar.setReserve3(equip.getReserve3());
			tempVar.setRelatedVideo(equip.getRelatedVideo());
			tempVar.setZiChanID(equip.getZiChanId());
			tempVar.setPlanNo(equip.getPlanNo());
			tempVar.setSafeTime(equip.getSafeTime());
			tempVar.setBackup(equip.getBackup());
			temp.add(tempVar);
		}

		return temp;
	}
	public final ArrayList<YcpTableRow> GetYcpTableList()
	{
		ArrayList<YcpTableRow> temp = new ArrayList<>();
		// 从MQTT服务器获取到的json数据进行转换
		for (var equip : MqttProvider.Instance.EquipTableRows)
		{
			temp.addAll(equip.getYcps().stream().map(ycp -> {
				YcpTableRow tempVar = new YcpTableRow();
				tempVar.setStaN(ycp.getStaN());
				tempVar.setEquipNo(ycp.getEquipNo());
				tempVar.setYcNo(ycp.getYcNo());
				tempVar.setYcNm(ycp.getYcNm());
				tempVar.setMapping(ycp.getMapping() != null ? ycp.getMapping() : false);
				tempVar.setYcMin(ycp.getYcMin());
				tempVar.setYcMax(ycp.getYcMax());
				tempVar.setPhysicMin(ycp.getPhysicMin());
				tempVar.setPhysicMax(ycp.getPhysicMax());
				tempVar.setValMin(ycp.getValMin());
				tempVar.setRestoreMin(ycp.getRestoreMin());
				tempVar.setRestoreMax(ycp.getRestoreMax());
				tempVar.setValMax(ycp.getValMax());
				tempVar.setValTrait(ycp.getValTrait());
				tempVar.setMainInstruction(ycp.getMainInstruction());
				tempVar.setMinorInstruction(ycp.getMinorInstruction());
				tempVar.setAlarmAcceptableTime(ycp.getAlarmAcceptableTime());
				tempVar.setRestoreAcceptableTime(ycp.getRestoreAcceptableTime());
				tempVar.setAlarmRepeatTime(ycp.getAlarmRepeatTime());
				tempVar.setProcAdvice(ycp.getProcAdvice());
				tempVar.setLvlLevel(ycp.getLvlLevel());
				tempVar.setOutminEvt(ycp.getOutminEvt());
				tempVar.setOutmaxEvt(ycp.getOutmaxEvt());
				tempVar.setWaveFile(ycp.getWaveFile());
				tempVar.setRelatedPic(ycp.getRelatedPic());
				tempVar.setAlarmScheme(ycp.getAlarmScheme());
				tempVar.setCurveRcd(ycp.getCurveRcd());
				tempVar.setCurveLimit(ycp.getCurveLimit());
				tempVar.setAlarmShield(ycp.getAlarmShield());
				tempVar.setUnit(ycp.getUnit());
				tempVar.setAlarmRiseCycle(ycp.getAlarmRiseCycle());
				tempVar.setReserve1(ycp.getReserve1());
				tempVar.setReserve2(ycp.getReserve2());
				tempVar.setReserve3(ycp.getReserve3());
				tempVar.setRelatedVideo(ycp.getRelatedVideo());
				tempVar.setZiChanID(ycp.getZiChanId());
				tempVar.setPlanNo(ycp.getPlanNo());
				tempVar.setSafeTime(ycp.getSafeTime());
				tempVar.setGWValue(ycp.getGWValue());
				tempVar.setGWTime(ycp.getGWTime());
				tempVar.setDatatype(ycp.getDataType());
				tempVar.setYcCode(ycp.getYcCode());

				return tempVar;
			}).toList());
		}

		return temp;
	}
	public final ArrayList<YxpTableRow> GetYxpTableList()
	{
		ArrayList<YxpTableRow> temp = new ArrayList<YxpTableRow>();
		// 从MQTT服务器获取到的json数据进行转换
		for (var equip : MqttProvider.Instance.EquipTableRows)
		{
			temp.addAll(equip.getYxps().stream().map(yxp -> {
				YxpTableRow tempVar = new YxpTableRow();
				tempVar.setYxNo(yxp.getYxNo());
				tempVar.setYxNm(yxp.getYxNm());
				tempVar.setProcAdviceR(yxp.getProcAdviceR());
				tempVar.setProcAdviceD(yxp.getProcAdviceD());
				tempVar.setLevelR(yxp.getLevelR());
				tempVar.setLevelD(yxp.getLevelD());
				tempVar.setEvt_01(yxp.getEvt01());
				tempVar.setEvt_10(yxp.getEvt10());
				tempVar.setStaN(yxp.getStaN());
				tempVar.setEquipNo(yxp.getEquipNo());
				tempVar.setValTrait(yxp.getValTrait());
				tempVar.setMainInstruction(yxp.getMainInstruction());
				tempVar.setMinorInstruction(yxp.getMinorInstruction());
				tempVar.setAlarmAcceptableTime(yxp.getAlarmAcceptableTime());
				tempVar.setRestoreAcceptableTime(yxp.getRestoreAcceptableTime());
				tempVar.setAlarmRepeatTime(yxp.getAlarmRepeatTime());
				tempVar.setWaveFile(yxp.getWaveFile());
				tempVar.setRelatedPic(yxp.getRelatedPic());
				tempVar.setAlarmScheme(yxp.getAlarmScheme());
				tempVar.setCurveRcd(yxp.getCurveRcd());
				tempVar.setAlarmShield(yxp.getAlarmShield());
				tempVar.setAlarmRiseCycle(yxp.getAlarmRiseCycle());
				tempVar.setReserve1(yxp.getReserve1());
				tempVar.setReserve2(yxp.getReserve2());
				tempVar.setReserve3(yxp.getReserve3());
				tempVar.setRelatedVideo(yxp.getRelatedVideo());
				tempVar.setZiChanID(yxp.getZiChanId());
				tempVar.setPlanNo(yxp.getPlanNo());
				tempVar.setSafeTime(yxp.getSafeTime());
				tempVar.setGWValue(yxp.getGWValue());
				tempVar.setGWTime(yxp.getGWTime());
				tempVar.setDatatype(yxp.getDataType());
				tempVar.setInversion(yxp.getInversion());
				tempVar.setInitval(yxp.getInitval());
				tempVar.setYxCode(yxp.getYxCode());

				return tempVar;
			}).toList());
		}
		return temp;
	}

	public final ArrayList<SetParmTableRow> GetSetParmTableList() {
		ArrayList<SetParmTableRow> temp = new ArrayList<SetParmTableRow>();
		//TODO: 从MQTT服务器获取到的json数据进行转换
		for (var equip : MqttProvider.Instance.EquipTableRows) {
			temp.addAll(equip.getSetParms().stream().map(set -> {
				SetParmTableRow tempVar = new SetParmTableRow();
				tempVar.setSetNo(set.getSetNo());
				tempVar.setSetNm(set.getSetNm());
				tempVar.setSetType(set.getSetType());
				tempVar.setRecord(set.getRecord());
				tempVar.setAction(set.getAction());
				tempVar.setValue(set.getValue());
				tempVar.setCanexecution(set.getCanexecution());
				tempVar.setVoiceKeys(set.getVoiceKeys());
				tempVar.setEnableVoice(set.getEnableVoice());
				tempVar.setQrEquipNo(set.getQrEquipNo());
				tempVar.setSetCode(set.getSetCode());
				tempVar.setStaN(set.getStaN());
				tempVar.setEquipNo(set.getEquipNo());
				tempVar.setMainInstruction(set.getMainInstruction());
				tempVar.setMainInstruction(set.getMinorInstruction());
				tempVar.setReserve1(set.getReserve1());
				tempVar.setReserve2(set.getReserve2());
				tempVar.setReserve3(set.getReserve3());
				return tempVar;
			}).toList());
		}
		return temp;
	}

}