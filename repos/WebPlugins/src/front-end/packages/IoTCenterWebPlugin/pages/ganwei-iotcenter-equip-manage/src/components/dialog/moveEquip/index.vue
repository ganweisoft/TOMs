<template>
    <el-dialog :title="$t('equipInfo.dialog.moveEquipTo')" :visible.sync="showMoveGroupsDialog"
        @close="closeDialog('showMoveGroupsDialog')" :close-on-click-modal="false" center class="moveEquip">
        <div>
            <span class="tips">{{ $t('equipInfo.dialog.selectGroup') }}</span>
            <el-select clearable size="small" v-model='moveGroupId' :no-data-text="$t('equipInfo.publics.noData')"
                :placeholder="$t('equipInfo.tips.selectGroupName')" filterable>
                <el-option v-for="(item, index) in moveGroups" :key="index" :label="item.groupName"
                    :value="item.groupId"></el-option>
            </el-select>
        </div>
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain size="small" @click="closeDialog('showMoveGroupsDialog')">{{
                $t('equipInfo.publics.button.cancel') }}</el-button>
            <el-button type="primary" size="small" @click="saveMoveData" :loading="saveLoading">{{
                $t('equipInfo.publics.button.confirm') }}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js';
export default index
</script>