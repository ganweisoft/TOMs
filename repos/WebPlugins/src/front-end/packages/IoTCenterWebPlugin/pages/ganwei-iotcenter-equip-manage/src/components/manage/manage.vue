<template>
    <div style="width: 100%;height: 100%;">
        <!-- selecteItem(Object):选中当前设备信息 -->
        <ycYxSet :selecteItem="selecteItem" ref="YcYxSet" id="equipCon_mainId" />
        <!-- 设备编辑弹窗 -->
        <!-- showEquipDialog(Boolean):弹窗控制字符；closeDialog(Function):关闭弹窗方法; info(Object):弹窗数据显示; isNew(Boolean):是否是新建; saveData(Function):数据保存; saveLoading(Boolean):保存按钮loading; batchEquip(Boolean):是否批量应用 -->
        <equipDialog :showDialog="showEquipDialog" :closeDialog="closeDialog" :info="copyObject" :isNew="isEquipNew"
            :saveData="saveEquip" :saveLoading="saveLoading" :batchEquip="batchEquip" />
        <!-- 设备删除 -->
        <deleteEquip :showDialog="showDeleteEquipDialog" :deleteItem="selecteItem" :closeDialog="closeDialog"
            :deleteEquip="confirmDeleteEquip" :saveLoading="saveLoading" />
    </div>
</template>

<script>
import manage from './manage.js'
export default manage
</script>

<style lang="sass" src="./manage.scss"></style>