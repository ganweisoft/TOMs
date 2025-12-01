<template>
    <div class="equipSelectDialog">
        <el-dialog class="EquipSelectDialog" :title="$t('login.framePro.label.equipSelect')" width="500px"
            v-model="dialogVisible" :close-on-click-modal="false" center :show-close="false">
            <template v-slot:title>
                <span class="dialog-footer dialogTitle">
                    <span :class="{ STTreeMenu: showSetParm }">{{ $t('login.framePro.label.equipSelect') }}</span>
                    <span v-if="showSetParm">{{ $t('login.framePro.tips.selControlEquip') }}</span>
                </span>
            </template>

            <div :class="{ treeMenuBox: true, STTreeMenu: showSetParm }">
                <el-input :placeholder="$t('login.framePro.input.inputEquipName')" v-model="searchName"
                    @change="onSearch" clearable>
                    <template v-slot:prefix>
                        <el-icon><Search></Search></el-icon>
                    </template>
                </el-input>
                <div class="main">
                    <div class="treeMenu">
                        <myTree ref='treeV2' @node-click="getItem"></myTree>
                    </div>
                </div>
            </div>
            <div class="equipSelectRight" v-if="showSetParm">
                <div class="setParmMain">
                    <div :class="{
                        setParm: true,
                        selectedColor: item.setNo == setParmCurrentIndex,
                    }" v-for="(item, index) in setParmList" :key="index" @click="selectedSetParm(item)">
                        {{ item.setNm }}
                    </div>
                    <div class="noData" v-if="setParmList.length === 0">{{ noData }}</div>
                </div>
                <div class="pagination">
                    <el-pagination small :page-size="setParmPageSize" layout="prev, pager, next" :total="setParmTotal"
                        :current-page="setParmCurrentPage" @current-change="setParmHandleChange">
                    </el-pagination>
                </div>
            </div>

            <template v-slot:footer>
                <span class="dialog-footer">
                    <el-button type="primary" plain @click="selectedChange([])">{{ $t('login.framePro.button.cancel')
                        }}</el-button>
                    <el-button type="primary" @click="sendData">{{ $t('login.framePro.button.confirm') }}</el-button>
                </span>
            </template>
        </el-dialog>
    </div>
</template>

<style lang="scss" src="./equipSelect.scss" scoped></style>
<script src="./equipSelect.js"></script>
