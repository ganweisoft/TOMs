<template>
    <el-dialog class="eqYcYxDialog" v-if="showDialog" :visible.sync="showDialog" :show-close="false" :close-on-click-modal="false" center @close="closeDialog('showYxDialog')">
        <div class="title" slot="title">
            <div>{{ $tt('dialog.yx') }}</div>
            <div class="close" @click="closeDialog('showYxDialog')">×</div>
        </div>
        <div class="yxDetail editDetail">
            <div class="equipDetailBasic">
                <p class="equipDetailTitle">{{ $tt('dialog.BasicSettings') }}</p>
                <el-form :inline="true" ref="yxInfoForm" :rules="yxInfoRules" :model="yxInfo">
                    <el-row :gutter="10">
                        <el-col :xs="12" :sm="8" :md="6" :lg="6" v-if="!isNew">
                            <el-form-item :label="$tt('yx.TeleindicationID')" prop="yxNo">
                                <el-input :placeholder="getPlaceholder('yx.TeleindicationID', 'input')" clearable v-model="yxInfo.yxNo" controls-position="right" disabled size="small"> </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yx.yxCode')" prop="yxCode">
                                <el-input :placeholder="getPlaceholder('yx.yxCode', 'input')" clearable size="small" v-model="yxInfo.yxCode" maxlength="80"></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yx.TeleindicationName')" prop="yxNm">
                                <el-input
                                    clearable
                                    v-model="yxInfo.yxNm"
                                    @blur="
                                        () => {
                                            yxInfo.yxNm = yxInfo.yxNm.trim()
                                        }
                                    "
                                    maxlength="80"
                                    size="small"
                                    :placeholder="getPlaceholder('yx.TeleindicationName', 'input')"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yx.OperatingCommand')" prop="mainInstruction">
                                <el-input
                                    clearable
                                    v-model="yxInfo.mainInstruction"
                                    :placeholder="getPlaceholder('yx.OperatingCommand', 'input')"
                                    @blur="
                                        () => {
                                            yxInfo.mainInstruction = yxInfo.mainInstruction.trim()
                                        }
                                    "
                                    maxlength="255"
                                    size="small"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yx.OperatingParameter')" prop="minorInstruction">
                                <el-input
                                    clearable
                                    v-model="yxInfo.minorInstruction"
                                    :placeholder="getPlaceholder('yx.OperatingParameter', 'input')"
                                    @blur="
                                        () => {
                                            yxInfo.minorInstruction = yxInfo.minorInstruction.trim()
                                        }
                                    "
                                    maxlength="255"
                                    size="small"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yx.zeroToOneEcent')" prop="evt01">
                                <el-input
                                    clearable
                                    v-model="yxInfo.evt01"
                                    :placeholder="getPlaceholder('yx.zeroToOneEcent', 'input')"
                                    @blur="
                                        () => {
                                            yxInfo.evt01 = yxInfo.evt01.trim()
                                        }
                                    "
                                    maxlength="64"
                                    size="small"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yx.oneToZeroEvent')" prop="evt10">
                                <el-input
                                    clearable
                                    v-model="yxInfo.evt10"
                                    :placeholder="getPlaceholder('yx.oneToZeroEvent', 'input')"
                                    @blur="
                                        () => {
                                            yxInfo.evt10 = yxInfo.evt10.trim()
                                        }
                                    "
                                    maxlength="64"
                                    size="small"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('dataType.title')">
                                <el-select v-model="yxInfo.dataType" :no-data-text="$t('publics.noData')" :placeholder="getPlaceholder('dataType.title', 'select')" clearable size="small" filterable>
                                    <el-option v-for="(item, index) in dataTypeOption" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form>
            </div>

            <div class="equipDetailSenior">
                <el-collapse v-model="activeNames">
                    <el-collapse-item :title="$tt('报警方式')" name="3">
                        <el-form :inline="true" ref="yxInfoAlarmForm" :rules="yxInfoRules" :model="yxInfo">
                            <el-row :gutter="10">
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.DisplayAlarm') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('显示到实时快照')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-select clearable v-model="alarms.isAlarm" :placeholder="$tt('dialog.select')" size="small">
                                            <el-option v-for="(item, i) in swit" :key="i" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.RecordAlarm') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('记录到数据库中存储')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-select clearable v-model="alarms.isMarkAlarm" :placeholder="$tt('dialog.select')" size="small">
                                            <el-option v-for="(item, i) in swit" :key="i" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.EmailAlarm')">
                                        <el-select v-model="alarms.emailAlarm" :placeholder="$tt('dialog.select')" size="small">
                                            <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.SMSAlarm')">
                                        <el-select v-model="alarms.messageAlarm" :placeholder="$tt('dialog.select')" size="small">
                                            <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.isNewWorkOrder')">
                                        <el-select v-model="alarms.isGenerateWO" :placeholder="$tt('dialog.select')" size="small">
                                            <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('曲线设置')" name="4">
                        <el-form :inline="true" ref="yxInfoCurveForm" :rules="yxInfoRules" :model="yxInfo">
                            <el-row :gutter="10">
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.CurveRecord')">
                                        <el-select clearable v-model="yxInfo.curveRcd" :placeholder="$tt('dialog.select')" size="small">
                                            <el-option v-for="(item, i) in swit1" :key="i" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('报警设置')" name="5">
                        <el-form :inline="true" ref="yxInfoAlarmSettingForm" :rules="yxInfoRules" :model="yxInfo">
                            <el-row :gutter="10">
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.SuggestionForZeroOne') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yx.SuggestionForZeroOneTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input
                                            clearable
                                            v-model="yxInfo.procAdviceR"
                                            :placeholder="getPlaceholder('yx.SuggestionForZeroOne', 'input')"
                                            @blur="
                                                () => {
                                                    yxInfo.procAdviceR = yxInfo.procAdviceR.trim()
                                                }
                                            "
                                            maxlength="255"
                                            size="small"
                                        >
                                        </el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.SuggestionForOneZero')">
                                        <el-input
                                            clearable
                                            v-model="yxInfo.procAdviceD"
                                            :placeholder="getPlaceholder('yx.SuggestionForOneZero', 'input')"
                                            @blur="
                                                () => {
                                                    yxInfo.procAdviceD = yxInfo.procAdviceD.trim()
                                                }
                                            "
                                            maxlength="255"
                                            size="small"
                                        >
                                        </el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="12" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.levelOfZeroOne') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yx.levelOfZeroOneTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <div class="LevelOfAlarmBox">
                                            <el-select
                                                clearable
                                                v-model="yxInfo.levelR"
                                                filterable
                                                :no-data-text="$t('publics.noData')"
                                                :placeholder="getPlaceholder('yx.levelOfZeroOne', 'select')"
                                                size="small"
                                            >
                                                <el-option v-for="(item, i) in alarmDropdown" :key="i" :label="item.name" :value="item.value"></el-option>
                                            </el-select>
                                        </div>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="12" :md="6" :lg="6">
                                    <el-form-item prop="levelD">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.levelOfOneZero') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yx.levelOfOneZeroTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>

                                        <div class="LevelOfAlarmBox">
                                            <el-select
                                                clearable
                                                v-model="yxInfo.levelD"
                                                filterable
                                                :no-data-text="$t('publics.noData')"
                                                :placeholder="getPlaceholder('yx.levelOfOneZeroTips', 'select')"
                                                size="small"
                                            >
                                                <el-option v-for="(item, i) in alarmDropdown" :key="i" :label="item.name" :value="item.value"></el-option>
                                            </el-select>
                                        </div>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('防误报设置')" name="6">
                        <el-form :inline="true" ref="yxInfoPreventAlarmVForm" :rules="yxInfoRules" :model="yxInfo">
                            <el-row :gutter="10">
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.RestoreDelayTime')" prop="restoreAcceptableTime">
                                        <el-input-number
                                            v-model="yxInfo.restoreAcceptableTime"
                                            @change="toInteger('restoreAcceptableTime')"
                                            :min="minNum"
                                            :max="maxNum"
                                            controls-position="right"
                                            size="small"
                                        ></el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item prop="alarmAcceptableTime">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.alarmDelayTime') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yx.alarmDelayTimeTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number
                                            v-model="yxInfo.alarmAcceptableTime"
                                            @change="toInteger('alarmAcceptableTime')"
                                            :min="minNum"
                                            :max="maxNum"
                                            controls-position="right"
                                            size="small"
                                        ></el-input-number>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('dialog.AdvancedSettings')" name="1" v-show="false">
                        <el-form :inline="true" ref="yxInfoSeniorForm" :rules="yxInfoRules" :model="yxInfo">
                            <el-row :gutter="16">
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.ReverseOrNot')">
                                        <el-select clearable v-model="yxInfo.inversion" :placeholder="$tt('dialog.select')" size="small">
                                            <el-option v-for="(item, i) in swit1" :key="i" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.RepeatedAlarmTime')" prop="alarmRepeatTime">
                                        <el-input-number
                                            v-model="yxInfo.alarmRepeatTime"
                                            @change="toInteger('alarmRepeatTime')"
                                            :min="minNum"
                                            :max="maxNum"
                                            controls-position="right"
                                            size="small"
                                        ></el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6" class="alarmWeek">
                                    <el-form-item prop="alarmRiseCycle">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.AlarmUpgradePeriod') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yx.AlarmUpgradePeriodTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number
                                            v-model="yxInfo.alarmRiseCycle"
                                            @change="toInteger('alarmRiseCycle')"
                                            :min="minNum"
                                            :max="maxNum"
                                            controls-position="right"
                                            size="small"
                                        ></el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.AlarmBlock') }}</span>
                                            <el-tooltip class="item" :content="$tt('yx.AlarmBlockTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input
                                            clearable
                                            v-model="yxInfo.alarmShield"
                                            :placeholder="getPlaceholder('yx.AlarmBlock', 'input')"
                                            @blur="
                                                () => {
                                                    yxInfo.alarmShield = yxInfo.alarmShield.trim()
                                                }
                                            "
                                            size="small"
                                        ></el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item class="security-period" prop="safeTime">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.SafetyPeriod') }}</span>
                                            <el-tooltip class="item" :content="$tt('yx.SafetyPeriodTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input
                                            clearable
                                            v-model="yxInfo.safeTime"
                                            :placeholder="getPlaceholder('yx.SafetyPeriod', 'input')"
                                            @blur="
                                                () => {
                                                    yxInfo.safeTime = yxInfo.safeTime.trim()
                                                }
                                            "
                                            size="small"
                                        >
                                        </el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="24" :md="12" :lg="12" v-if="page == 'equipInfo'">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('common.expression') }}</span>
                                            <el-tooltip class="item" popper-class="expression" effect="dark" placement="top-end">
                                                <div slot="content" v-html="expressionTips"></div>
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input
                                            clearable
                                            v-model="yxInfo.expression"
                                            :placeholder="getPlaceholder('common.expression', 'input')"
                                            @blur="
                                                () => {
                                                    yxInfo.expression = yxInfo.expression.trim()
                                                }
                                            "
                                            size="small"
                                        ></el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.NameOfAsset') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yx.NameOfAssetTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <ziChanSelect
                                            clearable
                                            v-model="yxInfo.ziChanId"
                                            :no-data-text="$t('publics.noData')"
                                            :placeholder="getPlaceholder('yx.NameOfAsset', 'select')"
                                            :currentSelect="currentSelectZiChan"
                                        >
                                        </ziChanSelect>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yx.PlanNumber') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yx.PlanNumberTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-select
                                            clearable
                                            v-model="yxInfo.planNo"
                                            filterable
                                            :no-data-text="$t('publics.noData')"
                                            :placeholder="getPlaceholder('yx.PlanNumber', 'select')"
                                            size="small"
                                        >
                                            <el-option v-for="(item, i) in planNoDropdown" :key="i" :label="item.label" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.LinkageVideo')">
                                        <el-select
                                            clearable
                                            v-model="yxInfo.relatedVideoName"
                                            :no-data-text="$t('publics.noData')"
                                            filterable
                                            :placeholder="getPlaceholder('yx.LinkageVideo', 'select')"
                                            @change="yxVideoChange"
                                            size="small"
                                        >
                                            <el-option v-for="(item, i) in videoDropdown" :key="i" :label="item.channelName" :value="item"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yx.LinkagePage')">
                                        <el-input
                                            clearable
                                            v-model="yxInfo.relatedPic"
                                            :placeholder="getPlaceholder('yx.LinkagePage', 'input')"
                                            @blur="
                                                () => {
                                                    yxInfo.relatedPic = yxInfo.relatedPic.trim()
                                                }
                                            "
                                            maxlength="255"
                                            size="small"
                                        ></el-input>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('common.customAttribute')" name="2" v-show="false">
                        <div class="parameters">
                            <el-button type="primary" size="small" icon="el-icon-plus" @click="addParameter"> {{ $tt('common.addParameter') }}</el-button>
                            <el-form :model="yxInfo" label-position="left" ref="customAttribute">
                                <div class="parameter" v-for="(item, index) in yxInfo.parameters" :key="index">
                                    <el-form-item class="inputBox" :label="$tt('common.parameterLabel')" :prop="`parameters[${index}][key]`" :rules="customAttributeRules.keyRules">
                                        <el-input size="small" :placeholder="getPlaceholder('common.parameterLabel', 'input')" v-model="item.key" @blur="inputKeyBlur(item)" clearable></el-input>
                                    </el-form-item>
                                    <el-form-item :label="$tt('common.parameterValue')" :prop="`parameters[${index}][value]`" :rules="customAttributeRules.valueRules">
                                        <el-input
                                            size="small"
                                            :placeholder="getPlaceholder('common.parameterValue', 'input')"
                                            @blur="
                                                () => {
                                                    item.value = item.value.trim()
                                                }
                                            "
                                            clearable
                                            v-model="item.value"
                                        ></el-input>
                                    </el-form-item>
                                    <div class="delete" @click="deleteParameter(index)"><i class="el-icon-delete"></i></div>
                                </div>
                            </el-form>
                        </div>
                    </el-collapse-item>
                </el-collapse>
            </div>
        </div>

        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain @click="closeDialog('showYxDialog')" size="mini">{{ $t('publics.button.cancel') }}</el-button>
            <el-button type="primary" @click="saveYx" size="mini" :loading="saveLoading">
                {{ $t('publics.button.confirm') }}
            </el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
<style src="./index.scss" lang="sass"></style>
