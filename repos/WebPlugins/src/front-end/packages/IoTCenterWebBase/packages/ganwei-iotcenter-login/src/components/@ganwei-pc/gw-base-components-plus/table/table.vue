<template>
    <div class="gwTable wh100">
        <slot name="header">
            <header v-if="showHeader" class="flex-between">
                <div class="search flex-start">
                    <slot name=headerSearch>
                        <el-input suffix-icon="el-icon-search" @keyup.enter.native="getList" v-model="searchNameStr" :placeholder="inputPlaceHolder" />
                    </slot>
                </div>
                <div class="buttons">
                    <slot name='headerOperate'>
                        <template v-if="headerOperate">
                            <el-button type="primary" @click="showNewDialog">新增</el-button>
                            <el-button v-if="multiple" type="danger" @click="deleteModels">删除</el-button>
                        </template>
                    </slot>
                </div>
            </header>
        </slot>

        <slot name="table">
            <el-table ref="multipleTable" :data="list" tooltip-effect="dark" style="width: 100%;" @selection-change="handleSelectionChange">
                <template slot="empty">
                    <div class="noData wh100 flex-center">
                        <span v-if="!noDataIconfont">暂无数据</span>
                        <span v-else><i class='icon iconfont' :class='noDataIconfont'></i></span>
                    </div>
                </template>
                <el-table-column v-if="multiple" type="selection" width="55"></el-table-column>
                <el-table-column v-for="(item, index) in setttingList" :width="item.width" :min-width="item.minWidth" :show-overflow-tooltip="item.showOverflowTooltip" :key="index" :property="item.property" :label="item.label">
                    <template slot-scope="scope">
                        {{ scope.row[scope.column.property] }}
                    </template>
                </el-table-column>
                <slot name="tableOparate"></slot>
            </el-table>
        </slot>
    </div>
</template>

<script src="./gwTable.js"></script>
<style lang="scss" src="./gwTable.scss" scoped></style>
