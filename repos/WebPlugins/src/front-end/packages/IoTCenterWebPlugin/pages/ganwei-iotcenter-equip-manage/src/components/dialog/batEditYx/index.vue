<template>
    <el-dialog :title="$t('equipInfo.dialog.batEdit')" :visible.sync="showBatEditYxDialog" @close="resetAndClose" class="batEditDialog batEditEqYcYxSet" :close-on-click-modal="false" center>
        <div class="editDetail">
            <div class="equipDetailBasic">
                <p class="equipDetailTitle">{{ $t('equipInfo.dialog.modificationItem') }}</p>
                <el-form ref="form" :inline="true" :model="batchYxSelect" :rules="rules">
                    <el-form-item :label="$t('equipInfo.yx.YxParameters')" prop="selectVal">
                        <el-select
                            clearable
                            size="small"
                            v-model="batchYxSelect.selectVal"
                            :no-data-text="$t('equipInfo.publics.noData')"
                            :placeholder="getPlaceholder('equipInfo.yx.YxParameters', 'select')"
                            multiple
                            filterable
                        >
                            <el-option v-for="(item, index) in yxSelectList" :key="index" :label="$t(item.label)" :value="item.value"></el-option>
                        </el-select>
                    </el-form-item>
                </el-form>
                <p class="equipDetailTitle" v-show="batchYxSelect.selectVal.length != 0">
                    {{ $t('equipInfo.dialog.modifyValue') }}
                </p>
                <el-form :inline="true" :model="batchYxForm" class="batchForm">
                    <el-row :gutter="16">
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('dataType') != -1">
                            <el-form-item :label="$t('equipInfo.dataType.title')">
                                <el-select
                                    clearable
                                    v-model="batchYxForm.dataType"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.dataType.title', 'select')"
                                    size="small"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in dataTypeOption" :key="index" :label="$t(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('evt01') != -1">
                            <el-form-item :label="$t('equipInfo.yx.zeroToOneEcent')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yx.zeroToOneEcent', 'input')"
                                    maxlength="64"
                                    v-model="batchYxForm.evt01"
                                    @blur="
                                        () => {
                                            batchYxForm.evt01 = batchYxForm.evt01.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('evt10') != -1">
                            <el-form-item :label="$t('equipInfo.yx.oneToZeroEvent')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yx.oneToZeroEvent', 'input')"
                                    maxlength="64"
                                    v-model="batchYxForm.evt10"
                                    @blur="
                                        () => {
                                            batchYxForm.evt10 = batchYxForm.evt10.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('relatedPic') != -1">
                            <el-form-item :label="$t('equipInfo.yx.LinkagePage')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yx.LinkagePage', 'input')"
                                    maxlength="255"
                                    v-model="batchYxForm.relatedPic"
                                    @blur="
                                        () => {
                                            batchYxForm.relatedPic = batchYxForm.relatedPic.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('relatedVideo') != -1">
                            <el-form-item :label="$t('equipInfo.yx.LinkageVideo')">
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchYxForm.relatedVideo"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.yx.LinkageVideo', 'select')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in videoDropdown" :key="index" :label="item.channelName" :value="item.id"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('ziChanId') != -1">
                            <el-form-item :label="$t('equipInfo.yx.NameOfAsset')">
                                <ziChanSelect
                                    clearable
                                    v-model="batchYxForm.ziChanId"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.yx.NameOfAsset', 'select')"
                                    :currentSelect="currentSelectZiChan"
                                >
                                </ziChanSelect>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('planNo') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.yx.PlanNumber') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yx.PlanNumberTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchYxForm.planNo"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.yx.PlanNumber', 'select')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in planNoDropdown" :key="index" :label="item.label" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('curveRcd') != -1">
                            <el-form-item :label="$t('equipInfo.yx.CurveRecord')">
                                <el-select size="small" v-model="batchYxForm.curveRcd">
                                    <el-option v-for="(item, index) in swit1" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yx.DisplayAlarm')">
                                <el-select size="small" v-model="alarms.isAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yx.RecordAlarm')">
                                <el-select size="small" v-model="alarms.isMarkAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>

                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('inversion') != -1">
                            <el-form-item :label="$t('equipInfo.yx.ReverseOrNot')">
                                <el-select clearable size="small" v-model="batchYxForm.inversion">
                                    <el-option v-for="(item, i) in swit1" :key="i" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('procAdviceR') != -1">
                            <el-form-item :label="$t('equipInfo.yx.SuggestionForZeroOne')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yx.SuggestionForZeroOne', 'input')"
                                    maxlength="255"
                                    v-model="batchYxForm.procAdviceR"
                                    @blur="
                                        () => {
                                            batchYxForm.procAdviceR = batchYxForm.procAdviceR.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('procAdviceD') != -1">
                            <el-form-item :label="$t('equipInfo.yx.SuggestionForOneZero')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yx.SuggestionForOneZero', 'input')"
                                    maxlength="255"
                                    v-model="batchYxForm.procAdviceD"
                                    @blur="
                                        () => {
                                            batchYxForm.procAdviceD = batchYxForm.procAdviceD.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('levelR') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.yx.levelOfZeroOne') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yx.levelOfZeroOneTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchYxForm.levelR"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.yx.levelOfZeroOne', 'select')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in alarmDropdown" :key="index" :label="item.name" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('levelD') != -1">
                            <el-form-item :label="$t('equipInfo.yx.levelOfOneZero')">
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchYxForm.levelD"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.yx.levelOfOneZero', 'select')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in alarmDropdown" :key="index" :label="item.name" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>

                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('mainInstruction') != -1">
                            <el-form-item :label="$t('equipInfo.yx.OperatingCommand')" prop="mainInstruction">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yx.OperatingCommand', 'input')"
                                    maxlength="255"
                                    v-model="batchYxForm.mainInstruction"
                                    @blur="
                                        () => {
                                            batchYxForm.mainInstruction = batchYxForm.mainInstruction.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('minorInstruction') != -1">
                            <el-form-item :label="$t('equipInfo.yx.OperatingParameter')" prop="minorInstruction">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yx.OperatingParameter', 'input')"
                                    maxlength="255"
                                    v-model="batchYxForm.minorInstruction"
                                    @blur="
                                        () => {
                                            batchYxForm.minorInstruction = batchYxForm.minorInstruction.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('alarmAcceptableTime') != -1">
                            <el-form-item prop="alarmAcceptableTime">
                                <span slot="label">
                                    {{ $t('equipInfo.yx.alarmDelayTime') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yx.alarmDelayTimeTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number
                                    size="small"
                                    v-model="batchYxForm.alarmAcceptableTime"
                                    @change="toInteger('alarmAcceptableTime')"
                                    :min="minNum"
                                    :max="maxNum"
                                    controls-position="right"
                                ></el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('restoreAcceptableTime') != -1">
                            <el-form-item :label="$t('equipInfo.yx.RestoreDelayTime')" prop="restoreAcceptableTime">
                                <el-input-number
                                    size="small"
                                    v-model="batchYxForm.restoreAcceptableTime"
                                    @change="toInteger('restoreAcceptableTime')"
                                    :min="minNum"
                                    :max="maxNum"
                                    controls-position="right"
                                ></el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('alarmRepeatTime') != -1">
                            <el-form-item :label="$t('equipInfo.yx.RepeatedAlarmTime')" prop="alarmRepeatTime">
                                <el-input-number
                                    size="small"
                                    v-model="batchYxForm.alarmRepeatTime"
                                    @change="toInteger('alarmRepeatTime')"
                                    :min="minNum"
                                    :max="maxNum"
                                    controls-position="right"
                                ></el-input-number>
                            </el-form-item>
                        </el-col>

                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('alarmShield') != -1">
                            <el-form-item>
                                <span slot="label">
                                    {{ $t('equipInfo.yx.AlarmBlock') }}
                                    <el-tooltip class="item" :content="$t('equipInfo.yx.AlarmBlockTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yx.AlarmBlock', 'input')"
                                    v-model="batchYxForm.alarmShield"
                                    @blur="
                                        () => {
                                            batchYxForm.alarmShield = batchYxForm.alarmShield.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" class="alarmWeek" v-show="batchYxSelect.selectVal.indexOf('alarmRiseCycle') != -1">
                            <el-form-item prop="alarmRiseCycle">
                                <span slot="label">
                                    {{ $t('equipInfo.yx.AlarmUpgradePeriod') }}
                                    <el-tooltip class="item" effect="dark" :content="$t('equipInfo.yx.AlarmUpgradePeriodTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input-number
                                    size="small"
                                    v-model="batchYxForm.alarmRiseCycle"
                                    @change="toInteger('alarmRiseCycle')"
                                    :min="minNum"
                                    :max="maxNum"
                                    controls-position="right"
                                ></el-input-number>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yx.SMSAlarm')">
                                <el-select size="small" v-model="alarms.messageAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yx.EmailAlarm')">
                                <el-select size="small" v-model="alarms.emailAlarm">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('alarmScheme') != -1">
                            <el-form-item :label="$t('equipInfo.yx.isNewWorkOrder')">
                                <el-select size="small" v-model="alarms.isGenerateWO">
                                    <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('safeTime') != -1">
                            <el-form-item prop="safeTime">
                                <span slot="label">
                                    {{ $t('equipInfo.yx.SafetyPeriod') }}
                                    <el-tooltip class="item" :content="$t('equipInfo.yx.SafetyPeriodTips')" placement="top-end">
                                        <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                    </el-tooltip>
                                </span>
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.yx.SafetyPeriod', 'input')"
                                    v-model="batchYxForm.safeTime"
                                    @blur="
                                        () => {
                                            batchYxForm.safeTime = batchYxForm.safeTime.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchYxSelect.selectVal.indexOf('expression') != -1">
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
                                    v-model="batchYxForm.expression"
                                    :placeholder="getPlaceholder('equipInfo.common.expression', 'input')"
                                    @blur="
                                        () => {
                                            batchYxForm.expression = batchYxForm.expression.trim()
                                        }
                                    "
                                    size="small"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form>
                <p class="equipDetailTitle" v-show="batchYxSelect.selectVal.indexOf('parameters') != -1">
                    {{ $t('equipInfo.common.customAttribute') }}
                </p>
                <el-row v-if="batchYxSelect.selectVal.indexOf('parameters') != -1">
                    <customAttribute v-model="batchYxForm.parameters" :getCustomPropData="getCustomPropData"></customAttribute>
                </el-row>
            </div>
        </div>
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain size="small" @click="resetAndClose"> {{ $t('equipInfo.publics.button.cancel') }}</el-button>
            <el-button type="primary" size="small" @click="saveBatchYx" :loading="saveLoading"> {{ $t('equipInfo.publics.button.confirm') }}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
