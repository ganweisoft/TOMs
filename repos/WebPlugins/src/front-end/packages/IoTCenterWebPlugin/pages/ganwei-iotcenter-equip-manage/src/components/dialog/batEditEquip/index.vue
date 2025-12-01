<template>
    <el-dialog :title="$t('equipInfo.dialog.batEdit')" :visible.sync="showBatEditEquipDialog" @close="resetAndClose" class="batEditDialog batEditEqYcYxSet" :close-on-click-modal="false" center>
        <div class="editDetail">
            <div class="equipDetailBasic">
                <p class="equipDetailTitle">{{ $t('equipInfo.dialog.modificationItem') }}</p>
                <el-form ref="form" :inline="true" :model="batchEquipSelect" :rules="rules">
                    <el-form-item :label="$t('equipInfo.poverTips.deviceParameters')" prop="selectVal">
                        <el-select
                            size="small"
                            v-model="batchEquipSelect.selectVal"
                            clearable
                            :no-data-text="$t('equipInfo.publics.noData')"
                            :placeholder="getPlaceholder('equipInfo.poverTips.deviceParameters', 'select')"
                            multiple
                            filterable
                        >
                            <el-option v-for="(item, index) in equipSelectList" :key="index" :label="$t(item.label)" :value="item.value"></el-option>
                        </el-select>
                    </el-form-item>
                </el-form>
                <p class="equipDetailTitle" v-show="batchEquipSelect.selectVal.length != 0">
                    {{ $t('equipInfo.dialog.modifyValue') }}
                </p>
                <el-form :inline="true" :model="batchEquipForm" class="batchForm">
                    <el-row :gutter="16">
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('equipDetail') != -1">
                            <el-form-item :label="$t('equipInfo.equip.DeviceProperties')">
                                <el-input
                                    v-model="batchEquipForm.equipDetail"
                                    @blur="
                                        () => {
                                            batchEquipForm.equipDetail = batchEquipForm.equipDetail.trim()
                                        }
                                    "
                                    maxlength="255"
                                    size="small"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('relatedVideo') != -1">
                            <el-form-item :label="$t('equipInfo.equip.LinkageVideo')">
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchEquipForm.relatedVideo"
                                    :placeholder="getPlaceholder('equipInfo.equip.LinkageVideo', 'select')"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in videoDropdown" :key="index" :label="item.channelName" :value="item.id"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('ziChanId') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.equip.NameOfAsset') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.equip.NameOfAssetTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <ziChanSelect
                                    clearable
                                    v-model="batchEquipForm.ziChanId"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.equip.NameOfAsset', 'select')"
                                    :currentSelect="currentSelectZiChan"
                                >
                                </ziChanSelect>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('planNo') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.equip.PlanNumber') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.equip.PlanNumberTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchEquipForm.planNo"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.equip.PlanNumber', 'select')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in planNoDropdown" :key="index" :label="item.label" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('relatedPic') != -1">
                            <el-form-item :label="$t('equipInfo.equip.LinkagePage')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.LinkagePage', 'input')"
                                    v-model="batchEquipForm.relatedPic"
                                    @blur="
                                        () => {
                                            batchEquipForm.relatedPic = batchEquipForm.relatedPic.trim()
                                        }
                                    "
                                    maxlength="255"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.equip.DisplayAlarm')">
                                <el-select size="small" v-model="alarms.isAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.equip.RecordAlarm')">
                                <el-select size="small" v-model="alarms.isMarkAlarm" :placeholder="$t('equipInfo.dialog.select')" filterable>
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('backup') != -1">
                            <el-form-item :label="$t('equipInfo.equip.HotStandby')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.HotStandby', 'input')"
                                    v-model="batchEquipForm.backup"
                                    @blur="
                                        () => {
                                            batchEquipForm.backup = batchEquipForm.backup.trim()
                                        }
                                    "
                                    maxlength="255"
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>

                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.equip.SMSAlarm')">
                                <el-select size="small" v-model="alarms.messageAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.equip.EmailAlarm')">
                                <el-select size="small" v-model="alarms.emailAlarm" filterable>
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('outOfContact') != -1">
                            <el-form-item :label="$t('equipInfo.equip.BreakdownInfo')" prop="outOfContact">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.BreakdownInfo', 'input')"
                                    v-model="batchEquipForm.outOfContact"
                                    @blur="
                                        () => {
                                            batchEquipForm.outOfContact = batchEquipForm.outOfContact.trim()
                                        }
                                    "
                                    maxlength="64"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('localAddr') != -1">
                            <el-form-item :label="$t('equipInfo.equip.CommunicationPort')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.CommunicationPort', 'input')"
                                    v-model="batchEquipForm.localAddr"
                                    @blur="
                                        () => {
                                            batchEquipForm.localAddr = batchEquipForm.localAddr.trim()
                                        }
                                    "
                                    maxlength="64"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('procAdvice') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.equip.CommunicationFaultHandlingSuggestion') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.equip.CommunicationFaultHandlingSuggestionTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.CommunicationFaultHandlingSuggestion', 'input')"
                                    v-model="batchEquipForm.procAdvice"
                                    @blur="
                                        () => {
                                            batchEquipForm.procAdvice = batchEquipForm.procAdvice.trim()
                                        }
                                    "
                                    maxlength="254"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('accCyc') != -1">
                            <el-form-item :label="$t('equipInfo.equip.CommunicationRefreshPeriod')" prop="accCyc">
                                <el-input-number size="small" v-model="batchEquipForm.accCyc" @change="toInteger('accCyc')" :min="1" :max="maxNum" controls-position="right"> </el-input-number>
                            </el-form-item>
                        </el-col>

                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('contacted') != -1">
                            <el-form-item :label="$t('equipInfo.equip.FailoverPrompt')" prop="contacted">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.FailoverPrompt', 'input')"
                                    v-model="batchEquipForm.contacted"
                                    @blur="
                                        () => {
                                            batchEquipForm.contacted = batchEquipForm.contacted.trim()
                                        }
                                    "
                                    maxlength="64"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('communicationDrv') != -1">
                            <el-form-item prop="communicationDrv">
                                <span slot="label">
                                    {{ $t('equipInfo.equip.DriverFile') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.equip.DriverFileTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-select
                                    size="small"
                                    clearable
                                    v-model="batchEquipForm.communicationDrv"
                                    :placeholder="getPlaceholder('equipInfo.equip.DriverFile', 'select')"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in communicationDrvList" :key="index" :label="item" :value="item"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>

                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('equipAddr') != -1">
                            <el-form-item :label="$t('equipInfo.equip.AddressOfDevice')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.AddressOfDevice', 'input')"
                                    v-model="batchEquipForm.equipAddr"
                                    @blur="
                                        () => {
                                            batchEquipForm.equipAddr = batchEquipForm.equipAddr.trim()
                                        }
                                    "
                                    maxlength="128"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('safeTime') != -1">
                            <el-form-item prop="safeTime">
                                <span slot="label">
                                    {{ $t('equipInfo.equip.SafetyPeriod') }}
                                    <el-tooltip class="item" :content="$t('equipInfo.equip.SafetyPeriodTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input
                                    size="small"
                                    v-model="batchEquipForm.safeTime"
                                    :placeholder="getPlaceholder('equipInfo.equip.SafetyPeriod', 'input')"
                                    @blur="
                                        () => {
                                            batchEquipForm.safeTime = batchEquipForm.safeTime.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('communicationParam') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.equip.ParameterOfCommunication') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.equip.ParameterOfCommunicationTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.ParameterOfCommunication', 'input')"
                                    v-model="batchEquipForm.communicationParam"
                                    @blur="
                                        () => {
                                            batchEquipForm.communicationParam = batchEquipForm.communicationParam.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('communicationTimeParam') != -1">
                            <el-form-item prop="communicationTimeParam">
                                <span slot="label">
                                    {{ $t('equipInfo.equip.TimeParameterOfCommunication') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.equip.TimeParameterOfCommunicationTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.TimeParameterOfCommunication', 'input')"
                                    v-model="batchEquipForm.communicationTimeParam"
                                    @blur="
                                        () => {
                                            batchEquipForm.communicationTimeParam = batchEquipForm.communicationTimeParam.trim()
                                        }
                                    "
                                    maxlength="32"
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('alarmRiseCycle') != -1">
                            <el-form-item prop="alarmRiseCycle">
                                <span slot="label">
                                    {{ $t('equipInfo.equip.AlarmUpgradePeriod') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.equip.AlarmUpgradePeriodTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number
                                    size="small"
                                    v-model="batchEquipForm.alarmRiseCycle"
                                    @change="toInteger('alarmRiseCycle')"
                                    :min="minNum"
                                    :max="maxNum"
                                    controls-position="right"
                                ></el-input-number>
                            </el-form-item>
                        </el-col>

                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('tabname') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.equip.NameOfAttachedList') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.equip.NameOfAttachedListTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.equip.NameOfAttachedList', 'input')"
                                    v-model="batchEquipForm.tabname"
                                    @blur="
                                        () => {
                                            batchEquipForm.tabname = batchEquipForm.tabname.trim()
                                        }
                                    "
                                    maxlength="15"
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchEquipSelect.selectVal.indexOf('enableEquip') != -1">
                            <el-form-item :label="$t('equipInfo.equip.equipStartStop')">
                                <el-switch v-model="batchEquipForm.enableEquip" :active-text="$t('equipInfo.dialog.selectYes')" :inactive-text="$t('equipInfo.dialog.selectNo')"> </el-switch>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form>
                <p class="equipDetailTitle" v-show="batchEquipSelect.selectVal.indexOf('parameters') != -1">
                    {{ $t('equipInfo.common.customAttribute') }}
                </p>
                <el-row v-if="batchEquipSelect.selectVal.indexOf('parameters') != -1">
                    <customAttribute v-model="batchEquipForm.parameters" :getCustomPropData="getCustomPropData"></customAttribute>
                </el-row>
            </div>
        </div>
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain size="small" @click="resetAndClose"> {{ $t('equipInfo.publics.button.cancel') }}</el-button>
            <el-button type="primary" size="small" @click="saveBatchEquip" :loading="saveLoading"> {{ $t('equipInfo.publics.button.confirm') }}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
