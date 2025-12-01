<template>
    <div class="equipSelectDialog">
        <el-dialog class="menuDialog" append-to-body :visible.sync="dialogVisible" :close-on-click-modal="false" center
            custom-class="equipSelect" @close='confirm(null)'>
            <span slot="title" class="dialog-footer dialogTitle">
                <span :class="{ STTreeMenu: showSetParm }">{{ topTilte }}</span>
                <span v-if="showSetParm">{{ $t(subTitle) }}</span>
            </span>

            <div :class="{ treeMenuBox: true, STTreeMenu: showSetParm }" class='contentBox'>
                <div class="datePicker">
                    <el-date-picker @change='timeChange' size="small" v-model="timeSelect" :picker-options="pickerOptions"
                        type="datetimerange" value-format="yyyy-MM-dd HH:mm" format="yyyy-MM-dd HH:mm" range-separator="-"
                        :start-placeholder="$t('equipListsIot.input.startTime')"
                        :end-placeholder="$t('equipListsIot.input.endTime')">
                    </el-date-picker>
                </div>

                <div class="content">
                    <div class="title">
                        <span>{{ $t('equipListsIot.title.selectDevice') }}</span>
                    </div>
                    <div class="main">
                        <div class="treeMenu">
                            <treeV2 ref="treeV2" v-if="showTree" :filterData="filterData" alias="noPoints" showCheckbox
                                show-count @onCheck='getChecked'>
                            </treeV2>
                        </div>
                    </div>
                </div>

            </div>
            <div class='contentBox'>
                <el-input :placeholder="$t('equipListsIot.input.searchYc')" v-model="ycpSearch"
                    @blur="() => { ycpSearch = ycpSearch.trim() }" clearable @keyup.enter.native="searchList('ycps')">
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
                </el-input>
                <div class="content">
                    <div class="title">
                        <el-checkbox v-model="ycpAllSelect" :indeterminate="ycHalfSelect"
                            @change='changeAllSelect("ycpAllSelect", "yc")'>
                        </el-checkbox>
                        <span>{{ $t('equipListsIot.title.selectYc') }}</span>
                    </div>
                    <div class="main" v-loading='getYxYcLoading'>
                        <div class="points">
                            <el-checkbox-group v-model="ycSelect">
                                <el-checkbox v-for="item in ycps" :label="item" :key="item">{{ item }}</el-checkbox>
                            </el-checkbox-group>
                        </div>
                    </div>
                </div>

            </div>
            <div class='contentBox'>
                <el-input :placeholder="$t('equipListsIot.input.searchYx')" v-model="yxpSearch"
                    @blur="() => { yxpSearch = yxpSearch.trim() }" clearable @keyup.enter.native="searchList('yxps')">
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
                </el-input>
                <div class="content">
                    <div class="title">
                        <el-checkbox v-model="yxpAllSelect" :indeterminate="yxHalfSelect"
                            @change='changeAllSelect("yxpAllSelect", "yx")'>
                        </el-checkbox>
                        <span>{{ $t('equipListsIot.title.selectYx') }}</span>
                    </div>
                    <div class="main" v-loading='getYxYcLoading'>
                        <div class="points">
                            <el-checkbox-group v-model="yxSelect">
                                <el-checkbox v-for="item in yxps" :label="item" :key="item">{{ item }}</el-checkbox>
                            </el-checkbox-group>
                        </div>
                    </div>
                </div>
            </div>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click="confirm(null)">
                    {{ $t('equipListsIot.publics.button.cancel') }}</el-button>
                <el-button type="primary" @click="sendData" :loading='exportLoading'>{{ $t(confirmButton) }}</el-button>
            </span>
        </el-dialog>

    </div>
</template>

<style lang="scss" src="./equipSelect.scss"></style>
<script src="./equipSelect.js"></script>