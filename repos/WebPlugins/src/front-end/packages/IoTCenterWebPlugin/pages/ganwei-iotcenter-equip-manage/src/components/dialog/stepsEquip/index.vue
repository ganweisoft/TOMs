<template>
    <div>
        <div class="equipDetail editDetail">
            <div class="equipDetailBasic">
                <section v-show="active == 2">
                    <el-form ref="equipInfoForm1" :rules="equipInfoRules" :model="equipInfo" :label-position="'right'"
                        label-width="110px" class="stepsAdd">
                        <p class="equipDetailTitle">基础信息</p>
                        <el-row :gutter="10">
                            <el-col :xs="24" :sm="12" :md="12" :lg="12" v-if="isNew && page == 'equipInfo'">
                                <el-form-item :label="$t('equipInfo.equip.group')">
                                    <el-input v-model="equipInfo.groupName" disabled size="small"></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.deviceName')" prop="equipNm">
                                    <el-input v-model="equipInfo.equipNm"
                                        @blur='() => { equipInfo.equipNm = equipInfo.equipNm.trim() }' maxlength='64'
                                        size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.DeviceProperties')">
                                    <el-input v-model="equipInfo.equipDetail"
                                        @blur='() => { equipInfo.equipDetail = equipInfo.equipDetail.trim() }'
                                        maxlength='255' size="small"></el-input>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <p class="equipDetailTitle">解码器配置</p>
                        <el-row :gutter="10">
                            <el-col :xs="12" :sm="12" :md="12" :lg="24">
                                <el-form-item :label="$t('equipInfo.equip.AddressOfDevice')">
                                    <el-input v-model="equipInfo.equipAddr"
                                        @blur='() => { equipInfo.equipAddr = equipInfo.equipAddr.trim() }'
                                        maxlength='128' size="small"></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.CommunicationPort')" prop="localAddr">
                                    <el-input v-model="equipInfo.localAddr"
                                        @blur='() => { equipInfo.localAddr = equipInfo.localAddr.trim() }'
                                        maxlength='64' size="small"></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="24" :sm="12" :md="12" :lg="12">
                                <el-form-item>
                                    <div slot="label" class="label">
                                        <span>{{ $t('equipInfo.equip.ParameterOfCommunication') }}</span>
                                        <el-tooltip class="item" effect="dark"
                                            :content="$t('equipInfo.equip.ParameterOfCommunicationTips')"
                                            placement="top-end">
                                            <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                        </el-tooltip>
                                    </div>
                                    <el-input v-model="equipInfo.communicationParam"
                                        @blur='() => { equipInfo.communicationParam = equipInfo.communicationParam.trim() }'
                                        size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="24" :sm="12" :md="12" :lg="12">
                                <el-form-item prop="communicationTimeParam">
                                    <div slot="label" class="label">
                                        <span>{{ $t('equipInfo.equip.TimeParameterOfCommunication') }}</span>
                                        <el-tooltip class="item" effect="dark"
                                            :content="$t('equipInfo.equip.TimeParameterOfCommunicationTips')"
                                            placement="top-end">
                                            <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                        </el-tooltip>
                                    </div>
                                    <el-input v-model="equipInfo.communicationTimeParam"
                                        @blur='() => { equipInfo.communicationTimeParam = equipInfo.communicationTimeParam.trim() }'
                                        maxlength='32' size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="24" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.CommunicationRefreshPeriod')" prop="accCyc">
                                    <el-input-number v-model="equipInfo.accCyc" @change="toInteger('accCyc')" :min="1"
                                        :max="maxNum" size="small"> </el-input-number>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="24" :sm="12" :md="12" :lg="12">
                                <el-form-item>
                                    <div slot="label" class="label">
                                        <span>通讯处理意见</span>
                                        <el-tooltip class="item" effect="dark"
                                            :content="$t('equipInfo.equip.CommunicationFaultHandlingSuggestionTips')"
                                            placement="top-end">
                                            <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                        </el-tooltip>
                                    </div>
                                    <el-input v-model="equipInfo.procAdvice" maxlength='254'
                                        @blur='() => { equipInfo.procAdvice = equipInfo.procAdvice.trim() }'
                                        size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item prop="communicationDrv">
                                    <div slot="label" class="label">
                                        <span>{{ $t('equipInfo.equip.DriverFile') }}</span>
                                    </div>
                                    <el-select v-model="equipInfo.communicationDrv"
                                        :no-data-text="$t('equipInfo.publics.noData')" filterable
                                        :placeholder="$t('equipInfo.dialog.select')" size="small">
                                        <el-option v-for="(item, index) in communicationDrvList" :key="index"
                                            :label="item" :value="item"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                        </el-row>
                    </el-form>
                </section>

                <section v-show="active == 3">
                    <el-form ref="equipInfoForm2" :rules="equipInfoRules" :model="equipInfo" :label-position="'right'"
                        label-width="110px" class="stepsAdd">
                        <p class="equipDetailTitle">报警相关</p>
                        <el-row :gutter="10">
                            <el-col :xs="12" :sm="8" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.BreakdownInfo')" prop="outOfContact">
                                    <el-input v-model="equipInfo.outOfContact" maxlength='64'
                                        @blur='() => { equipInfo.outOfContact = equipInfo.outOfContact.trim() }'
                                        size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="8" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.FailoverPrompt')">
                                    <el-input v-model="equipInfo.contacted" maxlength='64'
                                        @blur='() => { equipInfo.contacted = equipInfo.contacted.trim() }'
                                        size="small"></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.AlarmVoiceFile')">
                                    <el-input v-model="equipInfo.eventWav"
                                        @blur='() => { equipInfo.eventWav = equipInfo.eventWav.trim() }' maxlength='64'
                                        size="small"></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="24" :sm="12" :md="12" :lg="12">
                                <el-form-item prop="alarmRiseCycle">
                                    <div slot="label" class="label">
                                        <span>报警升级周期</span>
                                    </div>
                                    <el-input-number v-model="equipInfo.alarmRiseCycle"
                                        @change="toInteger('alarmRiseCycle')" :min="minNum" :max="maxNum"
                                        controls-position="right" size="small"></el-input-number>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <p class="equipDetailTitle">报警通知</p>
                        <el-row :gutter="10">
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.DisplayAlarm')">
                                    <el-select v-model="alarms.isAlarm" :placeholder="$t('equipInfo.dialog.select')">
                                        <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)"
                                            :value="item.value" size="small"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.EmailAlarm')">
                                    <el-select v-model="alarms.emailAlarm" :placeholder="$t('equipInfo.dialog.select')"
                                        size="small">
                                        <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)"
                                            :value="item.value"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.SMSAlarm')">
                                    <el-select v-model="alarms.messageAlarm"
                                        :placeholder="$t('equipInfo.dialog.select')" size="small">
                                        <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)"
                                            :value="item.value"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.RecordAlarm')">
                                    <el-select v-model="alarms.isMarkAlarm" :placeholder="$t('equipInfo.dialog.select')"
                                        size="small">
                                        <el-option v-for="(item, index) in swit" :key="index" :label="$tt(item.label)"
                                            :value="item.value"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="24" :sm="12" :md="12" :lg="12">
                                <el-form-item class="security-period" prop="safeTime">
                                    <div slot="label" class="label">
                                        <span>{{ $t('equipInfo.equip.SafetyPeriod') }}</span>
                                    </div>
                                    <el-input v-model="equipInfo.safeTime"
                                        @blur='() => { equipInfo.safeTime = equipInfo.safeTime.trim() }' size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <p class="equipDetailTitle">报警关联</p>
                        <el-row :gutter="10">
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.LinkageVideo')">
                                    <el-select v-model="equipInfo.relatedVideo" filterable
                                        :placeholder="$t('equipInfo.dialog.select')" size="small">
                                        <el-option v-for="(item, index) in videoDropdown" :key="index"
                                            :label="item.channelName" :value="item.id"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item>
                                    <div slot="label" class="label">
                                        <span>{{ $t('equipInfo.equip.NameOfAsset') }}</span>
                                    </div>
                                    <el-select v-model="equipInfo.ziChanId" filterable
                                        :placeholder="$t('equipInfo.dialog.select')" size="small">
                                        <el-option v-for="(item, index) in zcDropdown" :key="index"
                                            :label="item.ziChanName" :value="item.ziChanId"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item>
                                    <div slot="label" class="label">
                                        <span>{{ $t('equipInfo.equip.PlanNumber') }}</span>
                                    </div>
                                    <el-select v-model="equipInfo.planNo" filterable
                                        :placeholder="$t('equipInfo.dialog.select')" size="small">
                                        <el-option v-for="(item, index) in planNoDropdown" :key="index"
                                            :label="item.label" :value="item.value"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.LinkagePage')">
                                    <el-input v-model="equipInfo.relatedPic"
                                        @blur='() => { equipInfo.relatedPic = equipInfo.relatedPic.trim() }'
                                        maxlength='255' size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                        </el-row>
                    </el-form>
                </section>

                <section v-show="active == 4">
                    <el-form ref="equipInfoForm3" :rules="equipInfoRules" :model="equipInfo" :label-position="'right'"
                        label-width="110px" class="stepsAdd">
                        <p class="equipDetailTitle">其他配置</p>
                        <el-row :gutter="10">
                            <el-col :xs="24" :sm="12" :md="12" :lg="12">
                                <el-form-item>
                                    <div slot="label" class="label">
                                        <span>{{ $t('equipInfo.equip.NameOfAttachedList') }}</span>
                                    </div>
                                    <el-input v-model="equipInfo.tabName"
                                        @blur='() => { equipInfo.tabName = equipInfo.tabName.trim() }' maxlength='15'
                                        size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :xs="12" :sm="12" :md="12" :lg="12">
                                <el-form-item :label="$t('equipInfo.equip.HotStandby')">
                                    <el-input v-model="equipInfo.backup"
                                        @blur='() => { equipInfo.backup = equipInfo.backup.trim() }' maxlength='255'
                                        size="small">
                                    </el-input>
                                </el-form-item>
                            </el-col>
                        </el-row>
                    </el-form>
                </section>

            </div>
        </div>
    </div>
</template>

<script>
import index from './index.js';
export default index
</script>

<style lang='sass' src='./index.scss'>