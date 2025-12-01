<template>
    <el-dialog class="eqYcYxDialog" v-if="showDialog" :visible.sync="showDialog" :show-close="false" :close-on-click-modal="false" center @close="closeDialog('showYcDialog')">
        <div class="title" slot="title">
            <div>{{ $tt('dialog.yc') }}</div>
            <div class="close" @click="closeDialog('showYcDialog')">×</div>
        </div>
        <div class="ycDetail editDetail">
            <div class="equipDetailBasic">
                <p class="equipDetailTitle">{{ $tt('dialog.BasicSettings') }}</p>
                <el-form :inline="true" ref="ycInfoForm" :rules="ycInfoRules" :model="ycInfo">
                    <el-row :gutter="10">
                        <el-col :xs="12" :sm="8" :md="6" :lg="6" v-if="!isNew">
                            <el-form-item :label="$tt('yc.TelemeteringID')" prop="ycNo">
                                <el-input :placeholder="getPlaceholder('yc.TelemeteringID', 'input')" clearable v-model="ycInfo.ycNo" controls-position="right" disabled size="small"> </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yc.ycCode')" prop="ycCode">
                                <el-input clearable size="small" v-model="ycInfo.ycCode" maxlength="80" :placeholder="getPlaceholder('yc.ycCode', 'input')"></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yc.TelemeteringName')" prop="ycNm">
                                <el-input
                                    clearable
                                    v-model="ycInfo.ycNm"
                                    @blur="
                                        () => {
                                            ycInfo.ycNm = ycInfo.ycNm.trim()
                                        }
                                    "
                                    maxlength="80"
                                    size="small"
                                    :placeholder="getPlaceholder('yc.TelemeteringName', 'input')"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yc.Unit')">
                                <el-input
                                    clearable
                                    v-model="ycInfo.unit"
                                    @blur="
                                        () => {
                                            ycInfo.unit = ycInfo.unit.trim()
                                        }
                                    "
                                    maxlength="50"
                                    size="small"
                                    :placeholder="getPlaceholder('yc.Unit', 'input')"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yc.OperatingCommand')" prop="mainInstruction">
                                <el-input
                                    clearable
                                    v-model="ycInfo.mainInstruction"
                                    :placeholder="getPlaceholder('yc.OperatingCommand', 'input')"
                                    @blur="
                                        () => {
                                            ycInfo.mainInstruction = ycInfo.mainInstruction.trim()
                                        }
                                    "
                                    maxlength="255"
                                    size="small"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('yc.OperatingParameter')" prop="minorInstruction">
                                <el-input
                                    clearable
                                    v-model="ycInfo.minorInstruction"
                                    @blur="
                                        () => {
                                            ycInfo.minorInstruction = ycInfo.minorInstruction.trim()
                                        }
                                    "
                                    maxlength="255"
                                    size="small"
                                    :placeholder="getPlaceholder('yc.OperatingParameter', 'input')"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="8" :md="6" :lg="6">
                            <el-form-item :label="$tt('dataType.title')">
                                <el-select v-model="ycInfo.dataType" clearable filterable :no-data-text="$t('publics.noData')" :placeholder="getPlaceholder('dataType.title', 'select')" size="small">
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
                        <el-form :inline="true" ref="ycInfoAlarmForm" :rules="ycInfoRules" :model="ycInfo">
                            <el-row :gutter="10">
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.DisplayAlarm')">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.DisplayAlarm') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('显示到实时快照')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-select v-model="alarms.isAlarm" :placeholder="getPlaceholder('yc.DisplayAlarm', 'select')" size="small">
                                            <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.RecordAlarm') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('记录到数据库中存储')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-select v-model="alarms.isMarkAlarm" :placeholder="getPlaceholder('yc.RecordAlarm', 'select')" size="small">
                                            <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.EmailAlarm')">
                                        <el-select v-model="alarms.emailAlarm" size="small">
                                            <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.SMSAlarm')">
                                        <el-select v-model="alarms.messageAlarm" size="small">
                                            <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.isNewWorkOrder')">
                                        <el-select v-model="alarms.isGenerateWO" size="small">
                                            <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('曲线设置')" name="4">
                        <el-form :inline="true" ref="ycInfoCurveForm" :rules="ycInfoRules" :model="ycInfo">
                            <el-row :gutter="10">
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.CurveRecord')">
                                        <el-select v-model="ycInfo.curveRcd" size="small">
                                            <el-option v-for="(item, i) in swit1" :key="i" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.CurveRecordThreshold')" prop="curveLimit">
                                        <el-input-number v-model="ycInfo.curveLimit" @change="toInteger('curveLimit')" :min="minNum" :max="maxNum" controls-position="right" size="small">
                                        </el-input-number>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('数据比例变化')" name="5">
                        <el-form :inline="true" ref="ycInfoMappingForm" :rules="ycInfoRules" :model="ycInfo">
                            <el-row :gutter="10">
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.ScalingTransformation') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.ScalingTransformationTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-select v-model="ycInfo.mapping" size="small">
                                            <el-option v-for="(item, index) in swit1" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.Maximum')" prop="physicMax">
                                        <el-input-number v-model="ycInfo.physicMax" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right" size="small"> </el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.Minimum')" prop="physicMin">
                                        <el-input-number v-model="ycInfo.physicMin" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right" size="small"> </el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item prop="ycMax">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.ActualMaximum') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.ActualMaximumTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number v-model="ycInfo.ycMax" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right" size="small"> </el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item prop="ycMin">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.ActualMinimum') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.ActualMinimumTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number v-model="ycInfo.ycMin" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right" size="small"> </el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item prop="ycMin">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.初始值') }}</span>
                                        </div>
                                        <el-input-number v-model="ycInfo.valTrait" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right" size="small"> </el-input-number>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('报警阈值配置')" name="6">
                        <el-form :inline="true" ref="ycInfoAlarmThresholdForm" :rules="ycInfoRules" :model="ycInfo">
                            <el-row :gutter="10">
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item prop="valMax">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.UpperLimitValue') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.UpperLimitValueTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number v-model="ycInfo.valMax" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right" size="small"> </el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.OverTheCeilingEvent') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.OverTheCeilingEventTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input
                                            clearable
                                            v-model="ycInfo.outmaxEvt"
                                            :placeholder="getPlaceholder('yc.OverTheCeilingEvent', 'input')"
                                            @blur="
                                                () => {
                                                    ycInfo.outmaxEvt = ycInfo.outmaxEvt.trim()
                                                }
                                            "
                                            maxlength="64"
                                            size="small"
                                        ></el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item prop="restoreMax">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.ReplyUpperLimitValue') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.ReplyUpperLimitValueTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number v-model="ycInfo.restoreMax" :min="otherMinNum" :max="maxNum" :precision="2" controls-position="right" size="small"> </el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item prop="valMin">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.LowerLimitingValue') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.LowerLimitingValueTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number v-model="ycInfo.valMin" :precision="2" :min="otherMinNum" :max="maxNum" controls-position="right" size="small"> </el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.OfflineIncident') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.OfflineIncidentTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input
                                            clearable
                                            v-model="ycInfo.outminEvt"
                                            :placeholder="getPlaceholder('yc.OfflineIncident', 'input')"
                                            @blur="
                                                () => {
                                                    ycInfo.outminEvt = ycInfo.outminEvt.trim()
                                                }
                                            "
                                            maxlength="64"
                                            size="small"
                                        ></el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item prop="restoreMin">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.ReplyLowerLimitingValue') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.ReplyLowerLimitingValueTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number v-model="ycInfo.restoreMin" :min="otherMinNum" :max="maxNum" :precision="2" controls-position="right" size="small"> </el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.LevelOfAlarm') }}</span>
                                            <el-tooltip class="item" :content="$tt('yc.LevelOfAlarmTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <div class="LevelOfAlarmBox">
                                            <el-select
                                                clearable
                                                v-model="ycInfo.lvlLevel"
                                                filterable
                                                :no-data-text="$t('publics.noData')"
                                                :placeholder="getPlaceholder('yc.LevelOfAlarm', 'select')"
                                                size="small"
                                            >
                                                <el-option v-for="(item, index) in alarmDropdown" :key="index" :label="item.name" :value="item.value"></el-option>
                                            </el-select>
                                        </div>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="24" :md="12" :lg="12">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('equip.告警处理意见') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('equip.CommunicationFaultHandlingSuggestionTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input
                                            clearable
                                            v-model="ycInfo.procAdvice"
                                            :placeholder="getPlaceholder('equip.CommunicationFaultHandlingSuggestion', 'input')"
                                            @blur="
                                                () => {
                                                    ycInfo.procAdvice = ycInfo.procAdvice.trim()
                                                }
                                            "
                                            size="small"
                                        ></el-input>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('防误报设置')" name="7">
                        <el-form :inline="true" ref="ycInfoPreventAlarmVForm" :rules="ycInfoRules" :model="ycInfo">
                            <el-row :gutter="10">
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item prop="alarmAcceptableTime">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.ExcessDelayTime') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.ExcessDelayTimeTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number
                                            v-model="ycInfo.alarmAcceptableTime"
                                            @change="toInteger('alarmAcceptableTime')"
                                            :min="minNum"
                                            :max="maxNum"
                                            controls-position="right"
                                            size="small"
                                        ></el-input-number>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.RestoreDelayTime')" prop="restoreAcceptableTime">
                                        <el-input-number
                                            v-model="ycInfo.restoreAcceptableTime"
                                            @change="toInteger('restoreAcceptableTime')"
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
                        <el-form :inline="true" ref="ycInfoSeniorForm" :rules="ycInfoRules" :model="ycInfo">
                            <el-row :gutter="10">
                                <el-col :xs="24" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.RepeatedAlarmTime')" prop="alarmRepeatTime">
                                        <el-input-number
                                            v-model="ycInfo.alarmRepeatTime"
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
                                            <span>{{ $tt('yc.AlarmUpgradePeriod') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.AlarmUpgradePeriodTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input-number
                                            v-model="ycInfo.alarmRiseCycle"
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
                                            <span>{{ $tt('yc.AlarmBlock') }}</span>
                                            <el-tooltip class="item" :content="$tt('yc.AlarmBlockTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input
                                            clearable
                                            v-model="ycInfo.alarmShield"
                                            @blur="
                                                () => {
                                                    ycInfo.alarmShield = ycInfo.alarmShield.trim()
                                                }
                                            "
                                            size="small"
                                            :placeholder="getPlaceholder('yc.AlarmBlock', 'input')"
                                        ></el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="12" :md="6" :lg="6">
                                    <el-form-item class="security-period" prop="safeTime">
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.SafetyPeriod') }}</span>
                                            <el-tooltip class="item" :content="$tt('yc.SafetyPeriodTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-input
                                            clearable
                                            v-model="ycInfo.safeTime"
                                            :placeholder="getPlaceholder('yc.SafetyPeriod', 'input')"
                                            @blur="
                                                () => {
                                                    ycInfo.safeTime = ycInfo.safeTime.trim()
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
                                            v-model="ycInfo.expression"
                                            :placeholder="getPlaceholder('common.expression', 'input')"
                                            @blur="
                                                () => {
                                                    ycInfo.expression = ycInfo.expression.trim()
                                                }
                                            "
                                            size="small"
                                        ></el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.NameOfAsset') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.NameOfAssetTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <ziChanSelect
                                            clearable
                                            v-model="ycInfo.ziChanId"
                                            :no-data-text="$t('publics.noData')"
                                            :placeholder="getPlaceholder('yc.NameOfAsset', 'select')"
                                            :currentSelect="currentSelectZiChan"
                                        >
                                        </ziChanSelect>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item>
                                        <div slot="label" class="label">
                                            <span>{{ $tt('yc.PlanNumber') }}</span>
                                            <el-tooltip class="item" effect="dark" :content="$tt('yc.PlanNumberTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </div>
                                        <el-select
                                            clearable
                                            v-model="ycInfo.planNo"
                                            filterable
                                            :no-data-text="$t('publics.noData')"
                                            :placeholder="getPlaceholder('yc.PlanNumber', 'select')"
                                            size="small"
                                        >
                                            <el-option v-for="(item, i) in planNoDropdown" :key="i" :label="item.label" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="8" :md="6" :lg="6">
                                    <el-form-item :label="$tt('yc.LinkageVideo')">
                                        <el-select
                                            clearable
                                            v-model="ycInfo.relatedVideoName"
                                            filterable
                                            :no-data-text="$t('publics.noData')"
                                            :placeholder="getPlaceholder('yc.LinkageVideo', 'select')"
                                            @change="ycVideoChange"
                                            size="small"
                                        >
                                            <el-option v-for="(item, i) in videoDropdown" :key="i" :label="item.channelName" :value="item"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="24" :sm="24" :md="12" :lg="12">
                                    <el-form-item :label="$tt('yc.LinkagePage')">
                                        <el-input
                                            clearable
                                            v-model="ycInfo.relatedPic"
                                            :placeholder="getPlaceholder('yc.LinkagePage', 'input')"
                                            @blur="
                                                () => {
                                                    ycInfo.relatedPic = ycInfo.relatedPic.trim()
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
                            <el-form :model="ycInfo" label-position="left" ref="customAttribute">
                                <div class="parameter" v-for="(item, index) in ycInfo.parameters" :key="index">
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
            <el-button type="primary" plain @click="closeDialog('showYcDialog')" size="mini">{{ $t('publics.button.cancel') }}</el-button>
            <el-button type="primary" @click="saveYc" size="mini" :loading="saveLoading">
                {{ $t('publics.button.confirm') }}
            </el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>

<style lang="scss" src="./index.scss"></style>
