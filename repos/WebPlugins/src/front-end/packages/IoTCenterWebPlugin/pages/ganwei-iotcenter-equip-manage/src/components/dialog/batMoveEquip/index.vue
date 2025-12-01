<template>
    <el-dialog class="addGroup" :title="$t('equipInfo.dialog.selectGroup')" :visible.sync="showSelectGroupDialog"
        :close-on-click-modal="false" @close="resetAndClose" center>
        <div class="addEquipBox">
            <div class="addEquipAndFenZu">
                <div class="addEquipAndFenZu_main">
                    <div class="nmEquip_link">
                        <span>{{ $t('equipInfo.dialog.groupName') }}</span>
                        <el-form ref="form" :model="form" :rules="rules">
                            <el-form-item prop="newGroupId">
                                <el-select clearable v-model="form.newGroupId" filterable
                                    :placeholder="$t('equipInfo.tips.selectGroupName')"
                                    :no-data-text="$t('equipInfo.publics.noData')" size="small">
                                    <el-option v-for="(item, index) in parentSeleceed" :key="index" :label="item.groupName"
                                        :value="item.groupId"></el-option>
                                </el-select>
                            </el-form-item>
                        </el-form>
                    </div>
                </div>
            </div>
        </div>
        <div class="dialog-footer" slot="footer">
            <el-button class="b_btn" type="primary" plain size="small" @click="resetAndClose">{{
                $t('equipInfo.publics.button.cancel') }}</el-button>
            <el-button class="b_btn el-button--primary" :loading="btnLoading" @click="ConfirmMove()">{{
                $t('equipInfo.publics.button.confirm') }}</el-button>
        </div>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
