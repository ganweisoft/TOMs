<template>
    <el-dialog :title="$t('equipInfo.title.newDevice')" width="520px" :visible.sync="showDialog" class="addPanel"
        :before-close="closeDialog" :close-on-click-modal="false">
        <div class="editDetail">
            <div class="tempEquipForm">
                <el-form ref="form" :model="tempEquipForm" :rules="rules" label-position="right" label-width="110px">
                    <el-form-item :label="$t('equipInfo.label.addToGroup')" prop="groupId">
                        <el-select v-model="tempEquipForm.groupId" size="small"
                            :no-data-text="$t('equipInfo.publics.noData')"
                            :placeholder="$t('equipInfo.addFormRules.addToGroup')" clearable filterable>
                            <el-option v-for="(item, index) in groupList" :key="index" :label="item.groupName"
                                :value="item.groupId"></el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item :label="$t('equipInfo.dialog.selectedTemplate')" prop="equipNo">
                        <selectV2 clearable v-model="tempEquipForm.equipNo" dataLabel="equipNm"
                            :popper-append-to-body="true" dataValue="equipNo" size="small" :data="tempList"
                            @loadmore="loadmore" @remoteSearch="remoteSearch"
                            :placeholder="$t('equipInfo.tips.pleaseSelectTemplate')">
                        </selectV2>
                    </el-form-item>
                    <el-form-item :label="$t('equipInfo.dialog.deviceNum')" prop="num">
                        <el-input-number v-model="tempEquipForm.num" :min="1" :max="10000" size="small"> </el-input-number>
                    </el-form-item>
                </el-form>
            </div>
        </div>
        <span class="dialog-footer" slot="footer">
            <el-button type="primary" plain class="b_btn operationBtn" size="small" @click="closeDialog">{{
                $t('equipInfo.publics.button.cancel') }}</el-button>
            <el-button type="primary" :loading="saveLoading" class="b_btn operationBtn" @click="saveTempAddEquip">{{
                $t('equipInfo.publics.button.confirm') }}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
<style lang="scss">
.addPanel {
    .el-dialog {
        .tempEquipForm {
            form {

                .el-form-item {

                    .el-input-number {

                        .el-input-number__decrease {
                            border-right: 1px solid !important;
                            border-color: var(--btn-borderColor2) !important;
                        }
                    }
                }
            }
        }
    }
}
</style>
