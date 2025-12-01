<template>
    <el-dialog :title="$t('equipInfo.dialog.batEdit')" :visible.sync="showBatEditYcDialog" @close="resetAndClose" class="batEditDialog batEditEqYcYxSet" :close-on-click-modal="false" center>
        <div class="editDetail">
            <div class="equipDetailBasic">
                <p class="equipDetailTitle">{{ $t('equipInfo.dialog.modificationItem') }}</p>
                <el-form ref="form" :inline="true" :model="batchYcSelect" :rules="rules">
                    <el-form-item :label="$t('equipInfo.yc.YcParameters')" prop="selectVal">
                        <el-select
                            clearable
                            size="small"
                            v-model="batchYcSelect.selectVal"
                            :no-data-text="$t('equipInfo.publics.noData')"
                            :placeholder="getPlaceholder('equipInfo.yc.YcParameters', 'input')"
                            multiple
                            filterable
                        >
                            <el-option v-for="(item, index) in ycSelectList" :key="index" :label="$t(item.label)" :value="item.value"></el-option>
                        </el-select>
                    </el-form-item>
                </el-form>
                <p class="equipDetailTitle" v-show="batchYcSelect.selectVal.length != 0">
                    {{ $t('equipInfo.dialog.modifyValue') }}
                </p>
                <el-form :inline="true" :model="batchYcForm" class="batchForm">
                    <el-row :gutter="16">
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('dataType') != -1">
                            <el-form-item :label="$t('equipInfo.dataType.title')">
                                <el-select
                                    clearable
                                    v-model="batchYcForm.dataType"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.dataType.title', 'select')"
                                    size="small"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in dataTypeOption" :key="index" :label="$t(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('valMin') != -1">
                            <el-form-item prop="valMin">
                                <span slot="label">
                                    {{ $t('equipInfo.yc.LowerLimitingValue') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.LowerLimitingValueTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number size="small" v-model="batchYcForm.valMin" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right"> </el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('restoreMin') != -1">
                            <el-form-item prop="restoreMin">
                                <span slot="label">
                                    {{ $t('equipInfo.yc.ReplyLowerLimitingValue') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.ReplyLowerLimitingValueTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number size="small" v-model="batchYcForm.restoreMin" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right"> </el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('restoreMax') != -1">
                            <el-form-item prop="restoreMax">
                                <span slot="label">
                                    {{ $t('equipInfo.yc.ReplyUpperLimitValue') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.ReplyUpperLimitValueTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number size="small" v-model="batchYcForm.restoreMax" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right"> </el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('valMax') != -1">
                            <el-form-item prop="valMax">
                                <span slot="label">
                                    {{ $t('equipInfo.yc.UpperLimitValue') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.UpperLimitValueTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number size="small" v-model="batchYcForm.valMax" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right"> </el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('unit') != -1">
                            <el-form-item :label="$t('equipInfo.yc.Unit')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yc.Unit', 'input')"
                                    maxlength="50"
                                    v-model="batchYcForm.unit"
                                    @blur="
                                        () => {
                                            batchYcForm.unit = batchYcForm.unit.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('relatedPic') != -1">
                            <el-form-item :label="$t('equipInfo.yc.LinkagePage')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yc.LinkagePage', 'input')"
                                    maxlength="255"
                                    v-model="batchYcForm.relatedPic"
                                    @blur="
                                        () => {
                                            batchYcForm.relatedPic = batchYcForm.relatedPic.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('relatedVideo') != -1">
                            <el-form-item :label="$t('equipInfo.yc.LinkageVideo')">
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchYcForm.relatedVideo"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.yc.LinkageVideo', 'select')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in videoDropdown" :key="index" :label="item.channelName" :value="item.id"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('ziChanId') != -1">
                            <el-form-item :label="$t('equipInfo.yc.NameOfAsset')">
                                <ziChanSelect
                                    clearable
                                    v-model="batchYcForm.ziChanId"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.yc.NameOfAsset', 'select')"
                                    :currentSelect="currentSelectZiChan"
                                >
                                </ziChanSelect>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('planNo') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.yc.PlanNumber') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.PlanNumberTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchYcForm.planNo"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.yc.PlanNumber', 'select')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in planNoDropdown" :key="index" :label="item.label" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('curveRcd') != -1">
                            <el-form-item :label="$t('equipInfo.yc.CurveRecord')">
                                <el-select size="small" v-model="batchYcForm.curveRcd" :placeholder="$t('equipInfo.dialog.select')" filterable>
                                    <el-option v-for="(item, index) in swit1" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('curveLimit') != -1">
                            <el-form-item :label="$t('equipInfo.yc.CurveRecordThreshold')">
                                <el-input-number size="small" v-model="batchYcForm.curveLimit" @change="toInteger('curveLimit')" :min="minNum" :max="maxNum" controls-position="right">
                                </el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('outmaxEvt') != -1">
                            <el-form-item :label="$t('equipInfo.yc.OverTheCeilingEvent')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yc.OverTheCeilingEvent', 'input')"
                                    maxlength="64"
                                    v-model="batchYcForm.outmaxEvt"
                                    @blur="
                                        () => {
                                            batchYcForm.outmaxEvt = batchYcForm.outmaxEvt.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('outminEvt') != -1">
                            <el-form-item :label="$t('equipInfo.yc.OfflineIncident')">
                                <el-input
                                    clearable
                                    :placeholder="getPlaceholder('equipInfo.yc.OfflineIncident', 'input')"
                                    size="small"
                                    maxlength="64"
                                    v-model="batchYcForm.outminEvt"
                                    @blur="
                                        () => {
                                            batchYcForm.outminEvt = batchYcForm.outminEvt.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yc.DisplayAlarm')">
                                <el-select size="small" v-model="alarms.isAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yc.RecordAlarm')">
                                <el-select size="small" v-model="alarms.isMarkAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('lvlLevel') != -1">
                            <el-form-item :label="$t('equipInfo.yc.LevelOfAlarm')">
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchYcForm.lvlLevel"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.yc.LevelOfAlarm', 'input')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in alarmDropdown" :key="index" :label="item.name" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>

                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yc.SMSAlarm')">
                                <el-select size="small" v-model="alarms.messageAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yc.EmailAlarm')">
                                <el-select size="small" v-model="alarms.emailAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yc.isNewWorkOrder')">
                                <el-select size="small" v-model="alarms.isGenerateWO">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('mapping') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.yc.ScalingTransformation') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.ScalingTransformationTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-select size="small" v-model="batchYcForm.mapping">
                                    <el-option v-for="(item, index) in swit1" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('ycMin') != -1">
                            <el-form-item prop="ycMin">
                                <span slot="label">
                                    {{ $t('equipInfo.yc.ActualMinimum') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.ActualMinimumTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number size="small" v-model="batchYcForm.ycMin" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right"> </el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('ycMax') != -1">
                            <el-form-item prop="ycMax">
                                <span slot="label">
                                    {{ $t('equipInfo.yc.ActualMaximum') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.ActualMaximumTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number size="small" v-model="batchYcForm.ycMax" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right"> </el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('physicMin') != -1">
                            <el-form-item :label="$t('equipInfo.yc.Minimum')" prop="physicMin">
                                <el-input-number size="small" v-model="batchYcForm.physicMin" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right"> </el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('physicMax') != -1">
                            <el-form-item :label="$t('equipInfo.yc.Maximum')" prop="physicMax">
                                <el-input-number size="small" v-model="batchYcForm.physicMax" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right"> </el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('mainInstruction') != -1">
                            <el-form-item :label="$t('equipInfo.yc.OperatingCommand')" prop="mainInstruction">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yc.OperatingCommand', 'input')"
                                    maxlength="255"
                                    v-model="batchYcForm.mainInstruction"
                                    @blur="
                                        () => {
                                            batchYcForm.mainInstruction = batchYcForm.mainInstruction.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('minorInstruction') != -1">
                            <el-form-item :label="$t('equipInfo.yc.OperatingParameter')" prop="minorInstruction">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yc.OperatingParameter', 'input')"
                                    maxlength="255"
                                    v-model="batchYcForm.minorInstruction"
                                    @blur="
                                        () => {
                                            batchYcForm.minorInstruction = batchYcForm.minorInstruction.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('alarmAcceptableTime') != -1">
                            <el-form-item prop="alarmAcceptableTime">
                                <span slot="label">
                                    {{ $t('equipInfo.yc.ExcessDelayTime') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.ExcessDelayTimeTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number
                                    size="small"
                                    v-model="batchYcForm.alarmAcceptableTime"
                                    @change="toInteger('alarmAcceptableTime')"
                                    :min="minNum"
                                    :max="maxNum"
                                    controls-position="right"
                                ></el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('restoreAcceptableTime') != -1">
                            <el-form-item :label="$t('equipInfo.yc.RestoreDelayTime')" prop="restoreAcceptableTime">
                                <el-input-number
                                    size="small"
                                    v-model="batchYcForm.restoreAcceptableTime"
                                    @change="toInteger('restoreAcceptableTime')"
                                    :min="minNum"
                                    :max="maxNum"
                                    controls-position="right"
                                ></el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('alarmRepeatTime') != -1">
                            <el-form-item :label="$t('equipInfo.yc.RepeatedAlarmTime')" prop="alarmRepeatTime">
                                <el-input-number
                                    size="small"
                                    v-model="batchYcForm.alarmRepeatTime"
                                    @change="toInteger('alarmRepeatTime')"
                                    :min="minNum"
                                    :max="maxNum"
                                    controls-position="right"
                                ></el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" class="alarmWeek" v-show="batchYcSelect.selectVal.indexOf('alarmRiseCycle') != -1">
                            <el-form-item prop="alarmRiseCycle">
                                <span slot="label">
                                    {{ $t('equipInfo.yc.AlarmUpgradePeriod') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yc.AlarmUpgradePeriodTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number
                                    size="small"
                                    v-model="batchYcForm.alarmRiseCycle"
                                    @change="toInteger('alarmRiseCycle')"
                                    :min="minNum"
                                    :max="maxNum"
                                    controls-position="right"
                                ></el-input-number>
                            </el-form-item>
                        </el-col>

                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('alarmShield') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.yc.AlarmBlock') }}
                                    <el-tooltip class="item" :content="$t('equipInfo.yc.AlarmBlockTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input
                                    clearable
                                    :placeholder="getPlaceholder('equipInfo.yc.AlarmBlock', 'input')"
                                    size="small"
                                    v-model="batchYcForm.alarmShield"
                                    @blur="
                                        () => {
                                            batchYcForm.alarmShield = batchYcForm.alarmShield.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('safeTime') != -1">
                            <el-form-item prop="safeTime">
                                <span slot="label">
                                    {{ $t('equipInfo.yc.SafetyPeriod') }}
                                    <el-tooltip class="item" :content="$t('equipInfo.yc.SafetyPeriodTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yc.SafetyPeriod', 'input')"
                                    v-model="batchYcForm.safeTime"
                                    @blur="
                                        () => {
                                            batchYcForm.safeTime = batchYcForm.safeTime.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYcSelect.selectVal.indexOf('expression') != -1">
                            <el-form-item>
                                <div slot="label" class="label">
                                    <span>{{ $t('equipInfo.common.expression') }}</span>
                                    <el-tooltip class="item" popper-class="expression" effect="dark" placement="top-end">
                                        <div slot="content" v-html="expressionTips"></div>
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </div>
                                <el-input
                                    clearable
                                    v-model="batchYcForm.expression"
                                    :placeholder="getPlaceholder('equipInfo.common.expression', 'input')"
                                    @blur="
                                        () => {
                                            batchYcForm.expression = batchYcForm.expression.trim()
                                        }
                                    "
                                    size="small"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form>

                <p class="equipDetailTitle" v-show="batchYcSelect.selectVal.indexOf('parameters') != -1">
                    {{ $t('equipInfo.common.customAttribute') }}
                </p>
                <el-row v-if="batchYcSelect.selectVal.indexOf('parameters') != -1">
                    <customAttribute v-model="batchYcForm.parameters" :getCustomPropData="getCustomPropData"></customAttribute>
                </el-row>
            </div>
        </div>
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain size="small" @click="resetAndClose"> {{ $t('equipInfo.publics.button.cancel') }}</el-button>
            <el-button type="primary" size="small" @click="saveBatchYc" :loading="saveLoading"> {{ $t('equipInfo.publics.button.confirm') }}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
