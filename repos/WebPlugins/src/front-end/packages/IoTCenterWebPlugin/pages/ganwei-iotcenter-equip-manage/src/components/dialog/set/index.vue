<template>
    <el-dialog class="setDialog" id="setDialog" v-if="showDialog" :visible.sync="showDialog" :show-close="false" :close-on-click-modal="false" center @close="closeDialog('showSetDialog')">
        <div class="title" slot="title">
            <div>{{ $tt('dialog.set') }}</div>
            <div class="close" @click="closeDialog('showSetDialog')">×</div>
        </div>
        <div class="setDetail editDetail">
            <div class="equipDetailBasic">
                <p class="equipDetailTitle">{{ $tt('dialog.BasicSettings') }}</p>
                <el-form :inline="true" ref="setInfoForm" :class="{ otherLanguage: $i18n.locale != 'zh-CN' }" :rules="setInfoRules" :model="setInfo">
                    <el-row :gutter="16">
                        <el-col :xs="12" :sm="8" :md="8" :lg="6" v-if="!isNew">
                            <el-form-item :label="$tt('set.settingID')" prop="setNo">
                                <el-input clearable size="small" :placeholder="getPlaceholder('set.settingID', 'input')" v-model="setInfo.setNo" controls-position="right" disabled> </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="8" :lg="6">
                            <el-form-item :label="$tt('set.setCode')" prop="setCode">
                                <el-input clearable :placeholder="getPlaceholder('set.setCode', 'input')" size="small" v-model="setInfo.setCode"></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="8" :lg="6">
                            <el-form-item :label="$tt('set.settingName')" prop="setNm">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('set.settingName', 'input')"
                                    v-model="setInfo.setNm"
                                    @blur="
                                        () => {
                                            setInfo.setNm = setInfo.setNm.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="8" :lg="6">
                            <el-form-item :label="$tt('set.value')" prop="value">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('set.value', 'input')"
                                    v-model="setInfo.value"
                                    @blur="
                                        () => {
                                            setInfo.value = setInfo.value.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="8" :lg="6">
                            <el-form-item :label="$tt('set.OperatingCommand')" prop="mainInstruction">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('set.OperatingCommand', 'input')"
                                    v-model="setInfo.mainInstruction"
                                    @blur="
                                        () => {
                                            setInfo.mainInstruction = setInfo.mainInstruction.trim()
                                        }
                                    "
                                    maxlength="64"
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="8" :lg="6">
                            <el-form-item :label="$tt('set.OperatingParameter')" prop="minorInstruction">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('set.OperatingParameter', 'input')"
                                    v-model="setInfo.minorInstruction"
                                    @blur="
                                        () => {
                                            setInfo.minorInstruction = setInfo.minorInstruction.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="8" :lg="6">
                            <el-form-item :label="$tt('set.typeOfSetting')" prop="setType">
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="setInfo.setType"
                                    :no-data-text="$t('publics.noData')"
                                    :placeholder="getPlaceholder('set.typeOfSetting', 'select')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in setTypeOptions" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="8" :lg="6">
                            <el-form-item :label="$tt('set.offline')" v-if="page == 'equipInfo'">
                                <el-switch v-model="setInfo.setOffline" :active-text="$tt('dialog.selectYes')" :inactive-text="$tt('dialog.selectNo')"> </el-switch>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="12" :sm="8" :md="8" :lg="6">
                            <el-form-item :label="$tt('set.enable')" v-if="page == 'equipInfo'">
                                <el-switch v-model="setInfo.enableSetParm" :active-text="$tt('dialog.selectYes')" :inactive-text="$tt('dialog.selectNo')"> </el-switch>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form>
            </div>

            <div class="equipDetailSenior">
                <el-collapse v-model="activeNames">
                    <el-collapse-item :title="$tt('dialog.AdvancedSettings')" name="1" v-show="false">
                        <el-form :inline="true" :class="{ otherLanguage: $i18n.locale != 'zh-CN' }" ref="setInfoSeniorForm" :rules="setInfoRules" :model="setInfo">
                            <el-row :gutter="16">
                                <el-col :xs="12" :sm="12" :md="8" :lg="6">
                                    <el-form-item :label="$tt('set.record')">
                                        <el-select size="small" v-model="setInfo.record" :placeholder="$tt('dialog.select')">
                                            <el-option v-for="(item, index) in swit1" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="12" :md="8" :lg="6">
                                    <el-form-item :label="$tt('set.VoiceControlOrNot')">
                                        <el-select size="small" v-model="setInfo.enableVoice">
                                            <el-option v-for="(item, index) in swit1" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="12" :md="8" :lg="6">
                                    <el-form-item :label="$tt('set.action')" prop="action">
                                        <el-input
                                            clearable
                                            size="small"
                                            :placeholder="getPlaceholder('set.action', 'input')"
                                            v-model="setInfo.action"
                                            @blur="
                                                () => {
                                                    setInfo.action = setInfo.action.trim()
                                                }
                                            "
                                            maxlength="16"
                                        >
                                        </el-input>
                                    </el-form-item>
                                </el-col>
                                <el-col :xs="12" :sm="12" :md="8" :lg="6">
                                    <el-form-item :label="$tt('set.VoiceControlCharacter')" prop="voiceKeys">
                                        <el-input
                                            clearable
                                            size="small"
                                            :placeholder="getPlaceholder('set.VoiceControlCharacter', 'input')"
                                            v-model="setInfo.voiceKeys"
                                            @blur="
                                                () => {
                                                    setInfo.voiceKeys = setInfo.voiceKeys.trim()
                                                }
                                            "
                                        ></el-input>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </el-collapse-item>
                    <el-collapse-item :title="$tt('common.customAttribute')" name="2" v-show="false">
                        <div class="parameters">
                            <el-button type="primary" size="small" icon="el-icon-plus" @click="addParameter"> {{ $tt('common.addParameter') }}</el-button>
                            <el-form :model="setInfo" label-position="left" ref="customAttribute">
                                <div class="parameter" v-for="(item, index) in setInfo.parameters" :key="index">
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
            <el-button type="primary" plain @click="closeDialog('showSetDialog')" size="mini">{{ $t('publics.button.cancel') }}</el-button>
            <el-button type="primary" @click="saveSet" size="mini" :loading="saveLoading">
                {{ $t('publics.button.confirm') }}
            </el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>

<style lang="sass" src="./index.scss" />
