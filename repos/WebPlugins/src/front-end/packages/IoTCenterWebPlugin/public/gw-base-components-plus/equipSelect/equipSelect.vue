<template>
    <div class="equipSelectDialog">
        <el-dialog :title="$t('login.framePro.label.equipSelect')" class="menuDialog" width="500px" append-to-body
            :visible.sync="dialogVisible" :close-on-click-modal="false" center custom-class="equipSelect"
            :show-close="false">
            <span slot="title" class="dialog-footer dialogTitle">
                <span :class="{ STTreeMenu: showSetParm }">{{ $t('login.framePro.label.equipSelect') }}</span>
                <span v-if="showSetParm">{{ $t('login.framePro.tips.selControlEquip') }}</span>
            </span>

            <div :class="{ treeMenuBox: true, STTreeMenu: showSetParm }">
                <el-input :placeholder="$t('login.framePro.input.inputEquipName')" v-model="searchName"
                    @keyup.enter.native="onSearch" clearable>
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
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

            <span slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click="selectedChange([])">{{ $t('login.framePro.button.cancel')
                }}</el-button>
                <el-button type="primary" @click="sendData">{{ $t('login.framePro.button.confirm') }}</el-button>
            </span>
        </el-dialog>
    </div>
</template>

<style lang="scss" src="./equipSelect.scss" scoped></style>
<script src="./equipSelect.js"></script>
