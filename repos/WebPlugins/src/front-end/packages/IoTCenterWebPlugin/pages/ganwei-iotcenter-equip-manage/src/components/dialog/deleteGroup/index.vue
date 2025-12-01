<template>
    <el-dialog :title="$t('equipInfo.dialog.DeleteGroup')" :visible.sync="showDeleteGroup"
        @close="closeDialog('showDeleteGroup')" :close-on-click-modal="false" center class="deleteGroupDialog">
        <div>
            <span class="el-icon-warning"></span>
            <span>{{ $t('equipInfo.dialog.whetherToDeleteGroup') }}</span>
            <span class="tips">{{ info.groupName }}?</span>
            <div class="deleteTip">{{ $t('equipInfo.dialog.deleteGroupTips[0]') }}</div>
        </div>
        <!-- <div>
            <el-checkbox v-model="info.deleteChild" />
            <span>{{ $t('equipInfo.dialog.deleteGroupTips[0]') }}</span>
        </div> -->
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain size="small" @click="closeDialog('showDeleteGroup')">{{
                $t('equipInfo.publics.button.cancel') }}</el-button>
            <el-button type="danger" size="small" @click="confirmDeleteGroup" :loading="loading">{{
                $t('equipInfo.publics.button.confirm') }}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>