<template>
    <el-dialog :title="$tt('dialog.deleteEquip')" :visible.sync="showDeleteEquipDialog"
        @close="closeDialog('showDeleteEquipDialog')" :close-on-click-modal="false" center class="deleteGroupEquip">
        <div>
            <span class="flex-start-center"><i class="el-icon-warning"></i>{{ $tt('dialog.whetherToDeleteGroup') }}
                {{ deleteItem.equipName
                }}</span>
            <span>{{ $tt('dialog.afterDelete') }}<span class='tips'>{{ $tt('dialog.canNotFind')
            }}</span>{{ $tt('dialog.addAgain') }}</span>
        </div>
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain size="small" @click="closeDialog('showDeleteEquipDialog')">{{
                $t('publics.button.cancel') }}</el-button>
            <el-button type="primary" size="small" @click="deleteEquip" :loading="saveLoading">{{
                $t('publics.button.confirm') }}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
<style lang='sass' src='./index.scss'>
</style>