<template>
    <el-dialog :title="$t('templateManage.button.downProduct')" width="750px" :visible.sync="isShowDialog" @close="closeDialog('showDeleteEquipDialog')" :close-on-click-modal="false" center class="deleteGroupEquip">
        <div class="exportContainer">
            <div class="exportSearch">
                <el-input v-model="exportSearch" :placeholder="$t('templateManage.input.searchDeviceTip')"
                    @keyup.enter.native="exportSearchFun" clearable >
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
                </el-input>
            </div>
            <div class="exportContent">

                <el-checkbox :indeterminate="isIndeterminate" v-model="checkAll" @change="handleCheckAllChange">{{$t('templateManage.publics.button.selectAll')}}</el-checkbox>
                <div class="exportList" >
                    <el-checkbox-group v-model="checkedList" @change="handleCheckedListChange">
                        <el-checkbox v-for="item in list" :label="item.equipNo" :key="item.equipNo">{{item.equipNm}}</el-checkbox>
                    </el-checkbox-group>
                </div>
            </div>
            <div class="exportPagination">
                <el-pagination class="mobilePagination" small background @size-change="exportSizeChange"
                    @current-change="exportCurrentChange" :pager-count="7" :page-size="exportSize"
                    :current-page="exportPage" layout="prev, pager, next, total" :total="exportTotal">
                </el-pagination>
            </div>
        </div>
        <span slot="footer" class="dialog-footer">
            <el-button type="primary" plain size="small" @click="closeDialog('showDeleteEquipDialog')">{{$t('templateManage.publics.button.cancel')}}</el-button>
            <el-button type="primary" size="small" @click="exportExcel" :loading="saveLoading" :disabled="checkedList.length == 0">{{$t('templateManage.publics.button.confirm')}}</el-button>
        </span>
    </el-dialog>
</template>

<script>
import index from './index.js'
export default index
</script>
<style lang='sass' src='./index.scss'>
</style>
