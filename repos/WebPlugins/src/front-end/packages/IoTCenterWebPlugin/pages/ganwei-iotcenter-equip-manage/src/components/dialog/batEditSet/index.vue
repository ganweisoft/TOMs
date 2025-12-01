<template>
    <el-dialog :title="$t('equipInfo.dialog.batEdit')" class="batEditDialog batEditEqYcYxSet" :visible.sync="showBatEditSetDialog" @close="resetAndClose" :close-on-click-modal="false">
        <div class="editDetail">
            <div class="equipDetailBasic">
                <p class="equipDetailTitle">{{ $t('equipInfo.dialog.modificationItem') }}</p>
                <el-form ref="form" :inline="true" :model="batchSetSelect" :rules="rules">
                    <el-form-item :label="$t('equipInfo.set.setParameters')" prop="selectVal">
                        <el-select
                            clearable
                            size="small"
                            v-model="batchSetSelect.selectVal"
                            :no-data-text="$t('equipInfo.publics.noData')"
                            :placeholder="getPlaceholder('equipInfo.set.setParameters', 'select')"
                            multiple
                            filterable
                        >
                            <el-option v-for="(item, index) in setSelectList" :key="index" :label="$t(item.label)" :value="item.value"></el-option>
                        </el-select>
                    </el-form-item>
                </el-form>
                <p class="equipDetailTitle" v-show="batchSetSelect.selectVal.length != 0">
                    {{ $t('equipInfo.dialog.modifyValue') }}
                </p>
                <el-form :model="batchSetForm" class="batchForm" label-position="top">
                    <el-row :gutter="16">
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('value') != -1">
                            <el-form-item :label="$t('equipInfo.set.value')">
                                <el-input
                                    clearable
                                    size="small"
                                    v-model="batchSetForm.value"
                                    :placeholder="getPlaceholder('equipInfo.set.value', 'input')"
                                    @blur="
                                        () => {
                                            batchSetForm.value = batchSetForm.value.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('setType') != -1">
                            <el-form-item :label="$t('equipInfo.set.typeOfSetting')">
                                <el-select
                                    clearable
                                    size="small"
                                    v-model="batchSetForm.setType"
                                    :no-data-text="$t('equipInfo.publics.noData')"
                                    :placeholder="getPlaceholder('equipInfo.set.typeOfSetting', 'select')"
                                    filterable
                                >
                                    <el-option v-for="(item, index) in setTypeOptions" :key="index" :label="$t(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('action') != -1">
                            <el-form-item :label="$t('equipInfo.set.action')">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.set.action', 'input')"
                                    maxlength="16"
                                    v-model="batchSetForm.action"
                                    @blur="
                                        () => {
                                            batchSetForm.action = batchSetForm.action.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('mainInstruction') != -1">
                            <el-form-item :label="$t('equipInfo.set.OperatingCommand')" prop="mainInstruction">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.set.OperatingCommand', 'input')"
                                    maxlength="64"
                                    v-model="batchSetForm.mainInstruction"
                                    @blur="
                                        () => {
                                            batchSetForm.mainInstruction = batchSetForm.mainInstruction.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('minorInstruction') != -1">
                            <el-form-item :label="$t('equipInfo.set.OperatingParameter')" prop="minorInstruction">
                                <el-input
                                    clearable
                                    size="small"
                                    :placeholder="getPlaceholder('equipInfo.set.OperatingParameter', 'input')"
                                    v-model="batchSetForm.minorInstruction"
                                    @blur="
                                        () => {
                                            batchSetForm.minorInstruction = batchSetForm.minorInstruction.trim()
                                        }
                                    "
                                >
                                </el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('record') != -1">
                            <el-form-item :label="$t('equipInfo.set.record')">
                                <el-select size="small" v-model="batchSetForm.record">
                                    <el-option v-for="(item, index) in swit1" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('voiceKeys') != -1">
                            <el-form-item :label="$t('equipInfo.set.VoiceControlCharacter')">
                                <el-input
                                    clearable
                                    size="small"
                                    v-model="batchSetForm.voiceKeys"
                                    :placeholder="getPlaceholder('equipInfo.set.VoiceControlCharacter', 'input')"
                                    @blur="
                                        () => {
                                            batchSetForm.voiceKeys = batchSetForm.voiceKeys.trim()
                                        }
                                    "
                                ></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('enableVoice') != -1">
                            <el-form-item :label="$t('equipInfo.set.VoiceControlOrNot')">
                                <el-select size="small" v-model="batchSetForm.enableVoice">
                                    <el-option v-for="(item, index) in swit1" :key="index" :label="$tt(item.label)" :value="item.value"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('offline') != -1">
                            <el-form-item :label="$t('equipInfo.set.offline')">
                                <el-switch size="small" v-model="batchSetForm.offline"></el-switch>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="24" :md="12" :lg="12" v-show="batchSetSelect.selectVal.indexOf('enableSetParm') != -1">
                            <el-form-item :label="$t('equipInfo.set.enable')">
                                <el-switch size="small" v-model="batchSetForm.enableSetParm"></el-switch>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form>
                <p class="equipDetailTitle" v-show="batchSetSelect.selectVal.indexOf('parameters') != -1">
                    {{ $t('equipInfo.common.customAttribute') }}
                </p>
                <el-row v-if="batchSetSelect.selectVal.indexOf('parameters') != -1">
                    <customAttribute v-model="batchSetForm.parameters" :getCustomPropData="getCustomPropData"></customAttribute>
                </el-row>
            </div>
        </div>
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain size="small" @click="resetAndClose"> {{ $t('equipInfo.publics.button.cancel') }}</el-button>
            <el-button type="primary" size="small" @click="saveBatchSet" :loading="saveLoading"> {{ $t('equipInfo.publics.button.confirm') }}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
