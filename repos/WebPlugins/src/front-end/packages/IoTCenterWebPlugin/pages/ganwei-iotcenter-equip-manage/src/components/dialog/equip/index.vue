<template>
    <div>
        <el-dialog class='eqYcYxDialog' @close="closeDialog('showEquipDialog')"
            :title="page == 'template' ? $tt('dialog.product') : $tt('dialog.device')"
            :visible.sync="showDialog" v-if="showDialog" :close-on-click-modal="false">
            <div class="equipDetail editDetail" v-loading='dialogLoading'>
                <div class="equipDetailBasic">
                    <p class="equipDetailTitle">{{ $tt('dialog.BasicSettings') }}</p>
                    <el-form :inline="true" ref="equipInfoForm" :rules="equipInfoRules" :model="equipInfo"
                        :label-position="'right'">
                        <el-row :gutter="10">
                            <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                <el-form-item :label="$tt('equip.deviceName')" prop="equipNm">
                                    <el-input clearable v-model="equipInfo.equipNm"
                                        @blur='() => { equipInfo.equipNm = equipInfo.equipNm.trim() }' maxlength='64'
                                        size="small"
                                        :placeholder="getPlaceholder('equip.deviceName', 'input')">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                <el-form-item prop="communicationDrv">
                                    <div slot="label" class="label">
                                        <span>{{ $tt('equip.DriverFile') }}</span>
                                        <el-tooltip class="item" effect="dark"
                                            :content="$tt('equip.DriverFileTips')" placement="top-end">
                                            <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                        </el-tooltip>
                                    </div>
                                    <el-select clearable v-model="equipInfo.communicationDrv" filterable
                                        :no-data-text="$t('publics.noData')"
                                        :placeholder="getPlaceholder('equip.DriverFile', 'select')"
                                        size="small">
                                        <el-option v-for="(item, index) in communicationDrvList" :key="index"
                                            :label="item" :value="item"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                <el-form-item :label="$tt('equip.AddressOfDevice')">
                                    <el-input clearable v-model="equipInfo.equipAddr"
                                        :placeholder="getPlaceholder('equip.AddressOfDevice', 'input')"
                                        @blur='() => { equipInfo.equipAddr = equipInfo.equipAddr.trim() }'
                                        maxlength='128' size="small"></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="24" :sm="8" :md="8" :lg="6">
                                <el-form-item>
                                    <div slot="label" class="label">
                                        <span>{{ $tt('equip.ParameterOfCommunication') }}</span>
                                        <el-tooltip class="item" effect="dark"
                                            :content="$tt('equip.ParameterOfCommunicationTips')"
                                            placement="top-end">
                                            <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                        </el-tooltip>
                                    </div>
                                    <el-input clearable v-model="equipInfo.communicationParam"
                                        :placeholder="getPlaceholder('equip.ParameterOfCommunication', 'input')"
                                        @blur='() => { equipInfo.communicationParam = equipInfo.communicationParam.trim() }'
                                        size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="24" :sm="8" :md="8" :lg="6">
                                <el-form-item prop="communicationTimeParam">
                                    <div slot="label" class="label">
                                        <span>{{ $tt('equip.TimeParameterOfCommunication') }}</span>
                                        <el-tooltip class="item" effect="dark"
                                            :content="$tt('equip.TimeParameterOfCommunicationTips')"
                                            placement="top-end">
                                            <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                        </el-tooltip>
                                    </div>
                                    <el-input clearable v-model="equipInfo.communicationTimeParam"
                                        :placeholder="getPlaceholder('equip.TimeParameterOfCommunication', 'input')"
                                        @blur='() => { equipInfo.communicationTimeParam = equipInfo.communicationTimeParam.trim() }'
                                        maxlength='32' size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="24" :sm="8" :md="8" :lg="6" v-show="false">
                                <el-form-item prop="alarmRiseCycle">
                                    <div slot="label" class="label">
                                        <span>{{ $tt('equip.AlarmUpgradePeriod') }}</span>
                                        <el-tooltip class="item" effect="dark"
                                            :content="$tt('equip.AlarmUpgradePeriodTips')"
                                            placement="top-end">
                                            <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                        </el-tooltip>
                                    </div>
                                    <el-input-number v-model="equipInfo.alarmRiseCycle"
                                        @change="toInteger('alarmRiseCycle')" :min="minNum" :max="maxNum"
                                        controls-position="right" size="small"></el-input-number>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="8" :md="8" :lg="6" v-if="page == 'template'">
                                <el-form-item prop="equipconntype" :label="$tt('equip.equipType')">
                                    <el-select clearable v-model="equipInfo.equipconntype"
                                        :no-data-text="$t('publics.noData')"
                                        :placeholder="getPlaceholder('equip.equipType', 'select')"
                                        size="small" filterable>
                                        <el-option v-for="item in equipTypes" :key="item.value" :label="$tt(item.label)"
                                            :value="item.value">
                                        </el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                        </el-row>
                    </el-form>
                </div>

                <div class="equipDetailSenior">
                    <el-collapse v-model="activeNames">
                        <el-collapse-item :title="$tt('报警方式')" name="3">
                            <el-form :inline="true" ref="equipInfoAlarmForm" :rules="equipInfoRules" :model="equipInfo">
                                <el-row :gutter="10">
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item>
                                            <div slot="label" class="label">
                                                <span>{{ $tt('equip.DisplayAlarm') }}</span>
                                                <el-tooltip class="item" effect="dark"
                                                    :content="$tt('显示到实时快照')" placement="top-end">
                                                    <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                                </el-tooltip>
                                            </div>
                                            <el-select size="small" v-model="alarms.isAlarm"
                                                :placeholder="getPlaceholder('equip.DisplayAlarm', 'select')">
                                                <el-option v-for="(item, index) in swit" :key="index"
                                                    :label="$tt(item.label)" :value="item.value"></el-option>
                                            </el-select>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.RecordAlarm')">
                                            <div slot="label" class="label">
                                                <span>{{ $tt('equip.RecordAlarm') }}</span>
                                                <el-tooltip class="item" effect="dark"
                                                    :content="$tt('记录到数据库中存储')" placement="top-end">
                                                    <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                                </el-tooltip>
                                            </div>
                                            <el-select v-model="alarms.isMarkAlarm"
                                                :placeholder="getPlaceholder('equip.RecordAlarm', 'select')"
                                                size="small">
                                                <el-option v-for="(item, index) in swit" :key="index"
                                                    :label="$tt(item.label)" :value="item.value"></el-option>
                                            </el-select>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.EmailAlarm')">
                                            <el-select v-model="alarms.emailAlarm"
                                                :placeholder="$tt('dialog.select')" size="small">
                                                <el-option v-for="(item, index) in swit" :key="index"
                                                    :label="$tt(item.label)" :value="item.value"></el-option>
                                            </el-select>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.SMSAlarm')">
                                            <el-select v-model="alarms.messageAlarm"
                                                :placeholder="$tt('dialog.select')" size="small">
                                                <el-option v-for="(item, index) in swit" :key="index"
                                                    :label="$tt(item.label)" :value="item.value"></el-option>
                                            </el-select>
                                        </el-form-item>
                                    </el-col>
                                </el-row>
                            </el-form>
                        </el-collapse-item>
                        <el-collapse-item :title="$tt('报警设置')" name="4">
                            <el-form :inline="true" ref="equipInfoAlarmSettingForm" :rules="equipInfoRules" :model="equipInfo">
                                <el-row :gutter="10">
                                    <el-col :xs="24" :sm="8" :md="8" :lg="12">
                                        <el-form-item>
                                            <div slot="label" class="label">
                                                <span>{{ $tt('equip.CommunicationFaultHandlingSuggestion')
                                                    }}</span>
                                                <el-tooltip class="item" effect="dark"
                                                    :content="$tt('equip.CommunicationFaultHandlingSuggestionTips')"
                                                    placement="top-end">
                                                    <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                                </el-tooltip>
                                            </div>
                                            <el-input clearable v-model="equipInfo.procAdvice" maxlength='254'
                                                @blur='() => { equipInfo.procAdvice = equipInfo.procAdvice.trim() }'
                                                size="small"
                                                :placeholder="getPlaceholder('equip.CommunicationFaultHandlingSuggestion', 'input')">
                                            </el-input>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.BreakdownInfo')" prop="outOfContact">
                                            <el-input clearable v-model="equipInfo.outOfContact"
                                                :placeholder="getPlaceholder('equip.BreakdownInfo', 'input')"
                                                maxlength='64'
                                                @blur='() => { equipInfo.outOfContact = equipInfo.outOfContact.trim() }'
                                                size="small">
                                            </el-input>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.FailoverPrompt')">
                                            <el-input clearable v-model="equipInfo.contacted" maxlength='64'
                                                @blur='() => { equipInfo.contacted = equipInfo.contacted.trim() }'
                                                size="small"
                                                :placeholder="getPlaceholder('equip.FailoverPrompt', 'input')"></el-input>
                                        </el-form-item>
                                    </el-col>
                                </el-row>
                            </el-form>
                        </el-collapse-item>
                        <el-collapse-item :title="$tt('dialog.AdvancedSettings')" name="1" v-show="false">
                            <el-form :inline="true" ref="equipInfoSeniorForm" :rules="equipInfoRules"
                                :model="equipInfo">
                                <el-row :gutter="10">
                                    <el-col :xs="24" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.CommunicationRefreshPeriod')" prop="accCyc">
                                            <el-input-number v-model="equipInfo.accCyc" @change="toInteger('accCyc')" :min="1"
                                                :max="maxNum" controls-position="right" size="small"> </el-input-number>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6" v-if="page == 'equipInfo'">
                                        <el-form-item :label="$tt('equip.CommunicationPort')" prop="localAddr">
                                            <el-input clearable v-model="equipInfo.localAddr"
                                                :placeholder="getPlaceholder('equip.CommunicationPort', 'input')"
                                                @blur='() => { equipInfo.localAddr = equipInfo.localAddr.trim() }'
                                                maxlength='64' size="small"></el-input>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6" v-if="page == 'equipInfo'">
                                        <el-form-item :label="$tt('equip.equipStartStop')">
                                            <el-switch v-model="equipInfo.enableEquip"
                                                :active-text="$tt('dialog.selectYes')"
                                                :inactive-text="$tt('dialog.selectNo')">
                                            </el-switch>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="24" :sm="8" :md="8" :lg="6">
                                        <el-form-item class="security-period" prop="safeTime">
                                            <div slot="label" class="label">
                                                <span>{{ $tt('equip.SafetyPeriod') }}</span>
                                                <el-tooltip class="item"
                                                    :content="$tt('equip.SafetyPeriodTips')"
                                                    placement="top-end">
                                                    <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                                </el-tooltip>
                                            </div>
                                            <el-input clearable v-model="equipInfo.safeTime"
                                                :placeholder="getPlaceholder('equip.SafetyPeriod', 'input')"
                                                @blur='() => { equipInfo.safeTime = equipInfo.safeTime.trim() }'
                                                size="small">
                                            </el-input>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.DeviceProperties')">
                                            <el-input
                                                v-model="equipInfo.equipDetail"
                                                @blur="
                                                    () => {
                                                        equipInfo.equipDetail = equipInfo.equipDetail.trim()
                                                    }
                                                "
                                                maxlength="255"
                                                size="small"
                                            ></el-input>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item>
                                            <div slot="label" class="label">
                                                <span>{{ $tt('equip.NameOfAsset') }}</span>
                                                <el-tooltip class="item" effect="dark"
                                                    :content="$tt('equip.NameOfAssetTips')"
                                                    placement="top-end">
                                                    <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                                </el-tooltip>
                                            </div>
                                            <ziChanSelect
                                                :placeholder="getPlaceholder('equip.NameOfAsset', 'select')"
                                                :no-data-text="$t('publics.noData')"
                                                v-model="equipInfo.ziChanId" :currentSelect='currentSelectZiChan'>
                                            </ziChanSelect>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item>
                                            <div slot="label" class="label">
                                                <span>{{ $tt('equip.PlanNumber') }}</span>
                                                <el-tooltip class="item" effect="dark"
                                                    :content="$tt('equip.PlanNumberTips')"
                                                    placement="top-end">
                                                    <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                                </el-tooltip>
                                            </div>
                                            <el-select clearable v-model="equipInfo.planNo" filterable
                                                :placeholder="getPlaceholder('equip.PlanNumber', 'select')"
                                                :no-data-text="$t('publics.noData')" size="small">
                                                <el-option v-for="(item, index) in planNoDropdown" :key="index"
                                                    :label="item.label" :value="item.value"></el-option>
                                            </el-select>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.LinkageVideo')">
                                            <el-select clearable v-model="equipInfo.relatedVideo" filterable
                                                :no-data-text="$t('publics.noData')"
                                                :placeholder="getPlaceholder('equip.LinkageVideo', 'select')"
                                                size="small">
                                                <el-option v-for="(item, index) in videoDropdown" :key="index"
                                                    :label="item.channelName" :value="item.id"></el-option>
                                            </el-select>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="24" :sm="8" :md="8" :lg="6">
                                        <el-form-item>
                                            <div slot="label" class="label">
                                                <span>{{ $tt('equip.NameOfAttachedList') }}</span>
                                                <el-tooltip class="item" effect="dark"
                                                    :content="$tt('equip.NameOfAttachedListTips')"
                                                    placement="top-end">
                                                    <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                                </el-tooltip>
                                            </div>
                                            <el-input clearable v-model="equipInfo.tabName"
                                                :placeholder="getPlaceholder('equip.NameOfAttachedList', 'input')"
                                                @blur='() => { equipInfo.tabName = filterOtherWords(equipInfo.tabName) }'
                                                maxlength='15' size="small">
                                            </el-input>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.LinkagePage')">
                                            <el-input clearable v-model="equipInfo.relatedPic"
                                                :placeholder="getPlaceholder('equip.LinkagePage', 'input')"
                                                @blur='() => { equipInfo.relatedPic = equipInfo.relatedPic.trim() }'
                                                maxlength='255' size="small">
                                            </el-input>
                                        </el-form-item>
                                    </el-col>
                                    <el-col :xs="12" :sm="8" :md="8" :lg="6">
                                        <el-form-item :label="$tt('equip.HotStandby')">
                                            <el-input clearable v-model="equipInfo.backup"
                                                :placeholder="getPlaceholder('equip.HotStandby', 'input')"
                                                @blur='() => { equipInfo.backup = equipInfo.backup.trim() }'
                                                maxlength='255' size="small">
                                            </el-input>
                                        </el-form-item>
                                    </el-col>
                                </el-row>
                            </el-form>
                        </el-collapse-item>
                        <el-collapse-item :title="$tt('common.customAttribute')" name="2" v-show="false">
                            <div class="parameters">
                                <el-button type="primary" size="small" icon="el-icon-plus" @click="addParameter">
                                    {{ $tt('common.addParameter') }}</el-button>
                                <el-form :model="equipInfo" label-position="left" ref="customAttribute">
                                    <div class="parameter" v-for="(item, index) in equipInfo.parameters" :key="index">
                                        <el-form-item class="inputBox" :label="$tt('common.parameterLabel')"
                                            :prop="`parameters[${index}][key]`" :rules="customAttributeRules.keyRules">
                                            <el-input size="small"
                                                :placeholder="getPlaceholder('common.parameterLabel', 'input')"
                                                v-model="item.key" @blur="inputKeyBlur(item)" clearable></el-input>
                                        </el-form-item>
                                        <el-form-item :label="$tt('common.parameterValue')"
                                            :prop="`parameters[${index}][value]`"
                                            :rules="customAttributeRules.valueRules">
                                            <el-input size="small"
                                                :placeholder="getPlaceholder('common.parameterValue', 'input')"
                                                @blur='() => { item.value = item.value.trim() }' clearable
                                                v-model="item.value"></el-input>
                                        </el-form-item>
                                        <div class="delete" @click="deleteParameter(index)"><i
                                                class="el-icon-delete"></i>
                                        </div>
                                    </div>
                                </el-form>
                            </div>
                        </el-collapse-item>
                    </el-collapse>
                </div>
            </div>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click="closeDialog('showEquipDialog')" size="mini">{{
                $t('publics.button.cancel') }}
                </el-button>
                <el-button type="primary" @click="saveEquipData" :disabled="dialogLoading" size="mini"
                    :loading="saveLoading">
                    {{ $t('publics.button.confirm') }}</el-button>
                <el-button type="primary" @click="bactchSetEquip(0)" size="mini" v-if="!isNew && page == 'equipInfo'"
                    :disabled='batchEquip'>{{ $tt('dialog.BatchApplication') }}</el-button>
            </span>
        </el-dialog>
        <el-dialog class='batUseDialog' :title="$tt('dialog.BatchApplication')"
            v-if="showBatSetEquipDialog && page == 'equipInfo'" :visible.sync="showBatSetEquipDialog"
            @close="closeBatchSetEquip" :close-on-click-modal="false">
            <div class="set-equip">
                <div class="treeData equipList_main equipListMain">
                    <tree ref='myTrees' nodeKey="key" :showCheckbox='true'>
                    </tree>
                </div>
            </div>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" plain size="small" @click="showBatSetEquipDialog = false">
                    {{ $t('publics.button.cancel') }}</el-button>
                <el-button type="primary" size="small" @click="saveBatchSetEquip" :loading="loading">
                    {{ $t('publics.button.confirm') }}</el-button>
            </span>
        </el-dialog>
    </div>
</template>

<script>
import index from './index.js';
export default index
</script>

<style lang='sass' src='./index.scss'>
