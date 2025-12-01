<template>
    <div id="taskRepository">
        <el-header class="task-header">
            <div class="header-left">
                <h1>{{ $t('taskRepository.main.title') }}</h1>
                <el-popover class="choice" popper-class="task-popover" placement="bottom-start" width=" 288" trigger="click" v-model="visible">
                    <div class="taskChoice">
                        <span class="title">{{ $t('taskRepository.main.addMsg') }}</span>

                        <div class="choice">
                            <div @click="addPtTask()">
                                <img src="./images/task-normalTask-normal.png" />
                                <span>{{ $t('taskRepository.main.normalTask') }}</span>
                            </div>

                            <div @click="addXhTask()">
                                <img src="./images/task-circleTask-normal.png" />
                                <span>{{ $t('taskRepository.main.circleTask') }}</span>
                            </div>
                        </div>
                    </div>
                    <el-button slot="reference" icon="el-icon-circle-plus-outline" size="mini" circle ></el-button>
                </el-popover>
            </div>

            <div class="header-right">
                <el-input v-model="searchName" @keyup.enter.native="searchList" clearable :placeholder="$t('taskRepository.main.searchInput')" size="small">
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
                </el-input>
            </div>
        </el-header>

        <!-- 任务列表 -->
        <el-row :gutter="8" class="task-list" v-loading="taskLoad" >
            <div class="noDataTips" :data-nodata="$t('taskRepository.publics.noData')" v-if="!taskLoad && taskRepository.length == 0"></div>
            <el-col :xs="12" :sm="8" :md="6" :lg="4" :xl="4" v-for="item in taskRepository" :key="item.tableId" >
                <div class="task-item" @click="showEdit(item.tableId, item.tableType)" >
                    <div class="task-list-name" :title="item.tableName">
                        <span class="sign" :class="{ circleSign: item.tableType === 'C' }"></span>
                        <span>
                            <p>{{ item.tableName }}</p>
                            <p>{{ item.remark }}</p>
                        </span>
                    </div>

                    <el-popover placement="bottom" trigger="hover">
                        <el-button size="small" @click="delData(item.tableName, item.tableId, item.tableType)" class="el-button--danger" icon="iconfont icon-gw-icon-shanchu" >{{
                            $t('taskRepository.publics.button.deletes')
                        }}</el-button>
                        <i @click.stop class="iconfont icon-tubiao20_gengduo" slot="reference" >
                        </i>
                    </el-popover>

                </div>
            </el-col>
        </el-row>

        <footer>
            <div class="equipPaging">
                <el-pagination
                    small
                    background
                    @size-change="sizeChange"
                    @current-change="currentChange"
                    :pager-count="7"
                    :page-sizes="[20, 50, 100]"
                    :page-size="pageSize"
                    :current-page="pageNo"
                    layout="sizes, prev, pager, next, jumper,total"
                    :total="total"
                ></el-pagination>
            </div>
        </footer>

        <el-dialog width="500px" top="0" :visible.sync="treeDialog" :close-on-click-modal="false" @close="cancelTree" @closed="closedTree">
            <p slot="title">{{ $t('taskRepository.dialog.selectEqpItem') }}</p>
            <div class="list-box">
                <el-input :placeholder="$t('taskRepository.dialog.enterEqpName')" v-model="searchTreeName" @keyup.enter.native="inputChange" size="small" clearable>
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
                </el-input>
                <div class="main">
                    <myTree v-if="treeDialog" showSettings ref="myTree" @node-click="handleNodeClick"></myTree>
                </div>
            </div>

            <div slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click="treeDialog = false" size="small">{{ $t('taskRepository.publics.button.cancel') }}</el-button>
                <el-button type="primary" @click="confirmTree" size="small" :loading="ptLoading" >{{ $t('taskRepository.publics.button.confirm') }}</el-button>
            </div>
        </el-dialog>

        <!-- 普通任务框 -->
        <el-dialog width="930px" custom-class="pt-dialog" top="0" :close-on-click-modal="false" :visible.sync="ptTaskDetail" @closed="closePtDetail">
            <p slot="title">{{ $t('taskRepository.main.normalTask') }}</p>

            <div class="dialog-body flex-column" v-loading="loading">
                <el-form ref="ptForm" :model="ptForm" :rules="ptRule" label-position="top" class="base-info" :validate-on-rule-change="false">
                    <div class="dialog-body-pt-top">
                        <el-form-item :label="$t('taskRepository.dialog.taskName') + ':'" prop="tableName">
                            <el-input v-model="ptForm.tableName" :placeholder="$t('taskRepository.main.pleaseEnter')" size="small"></el-input>
                        </el-form-item>
                        <el-form-item :label="$t('taskRepository.dialog.taskComment') + ':'">
                            <el-input v-model="ptForm.comment" maxlength="256" type="textarea" :placeholder="$t('taskRepository.dialog.taskCommentPlaceholder')"></el-input>
                        </el-form-item>
                    </div>

                    <div class="dialog-body-pt-bottom">
                        <el-form-item prop="procTaskSys-" :label="$t('taskRepository.dialog.taskSys')">
                            <div class="task-sys-list dialog-body-item">
                                <header class="dialog-body-title">
                                    <span class="body-item-check">{{ $t('taskRepository.dialog.selected') }}({{ this.sysSelection.length }})</span>
                                    <el-button @click="addPtTaskSys" icon="el-icon-plus" class="new-btn" size="small" round >{{ $t('taskRepository.dialog.addControl') }}</el-button>
                                    <el-button
                                        @click="setSelectTaskStatus(true)"
                                        icon="el-icon-video-play"
                                        class="new-btn"
                                        size="small"
                                        :class="{ noTaskSelect: !sysSelection.length }"
                                        :disabled="!sysSelection.length"
                                        round

                                        >{{ $t('taskRepository.publics.button.start') }}</el-button
                                    >
                                    <el-button
                                        @click="setSelectTaskStatus(false)"
                                        icon="el-icon-video-pause"
                                        :class="{ noTaskSelect: !sysSelection.length }"
                                        :disabled="!sysSelection.length"
                                        class="new-btn"
                                        size="small"
                                        round

                                        >{{ $t('taskRepository.publics.button.stop') }}</el-button
                                    >
                                    <el-button
                                        @click="deletePtTaskSys"
                                        :class="{ noTaskSelect: !sysSelection.length }"
                                        :disabled="!sysSelection.length"
                                        icon="el-icon-delete"
                                        class="del-btn"
                                        size="small"
                                        round

                                        >{{ $t('taskRepository.publics.button.deletes') }}</el-button
                                    >
                                </header>

                                <div class="task-table">
                                    <el-form ref="procTaskSys" :model="ptForm" :rules="ptRule" label-width="0px">
                                        <el-table
                                            ref="taskSysTable"
                                            id="taskSysTable"
                                            label-position="top"
                                            :data="ptForm.procTaskSys"
                                            style="width: 100%"
                                            :cell-style="cellStyle"
                                            @selection-change="sysSelectionChange"
                                        >
                                            <el-table-column type="selection" width="32"></el-table-column>

                                            <el-table-column :label="$t('taskRepository.dialog.procType')" width="150">
                                                <template slot-scope="scope">
                                                    <el-select @change="removeId(scope.row)" v-model="scope.row.procType" :placeholder="$t('taskRepository.dialog.selectCtr')" filterable size="mini">
                                                        <template #prefix>
                                                            <i class="el-icon-open"></i>
                                                        </template>
                                                        <el-option v-for="item in typeList" :key="$t(item.label)" :label="$t(item.label)" :value="item.value"></el-option>
                                                    </el-select>
                                                </template>
                                            </el-table-column>

                                            <el-table-column :label="$t('taskRepository.dialog.taskTime')" width="135">
                                                <template slot-scope="scope">
                                                    <el-form-item prop="time">
                                                        <el-time-picker v-model="scope.row.time" size="mini" :clearable="false"></el-time-picker>
                                                    </el-form-item>
                                                </template>
                                            </el-table-column>

                                            <el-table-column :label="$t('taskRepository.dialog.taskCtrName')" min-width="300">
                                                <template slot-scope="scope">
                                                    <el-form-item
                                                        :prop="`procTaskSys.${scope.$index}.linkValue`"
                                                        :rules="[{ validator: validateName, trigger: 'change' }]"
                                                        v-if="scope.row.procType == 's'"
                                                    >
                                                        <el-cascader :options="sysControlTable" v-model="scope.row.linkValue" @change="val => linkChange(val, scope.$index)"></el-cascader>
                                                    </el-form-item>
                                                    <el-form-item
                                                        :prop="`procTaskSys.${scope.$index}.equipSetName`"
                                                        class="setEquipAttribute"
                                                        :rules="[{ validator: validateName, trigger: 'change' }]"
                                                        v-if="scope.row.procType == 'e'"
                                                    >
                                                        <el-input
                                                            v-model="scope.row.equipSetName"
                                                            prefix-icon="el-icon-suitcase"
                                                            @focus="focusCascader(scope.$index + 1)"
                                                            :ref="scope.$index + 1"
                                                            size="mini"
                                                            :placeholder="$t('taskRepository.dialog.selectEqpItem')"
                                                        />
                                                        <el-input
                                                            v-model="scope.row.value"
                                                            autocomplete="off"
                                                            type="text"
                                                            :ref="scope.$index + 'value'"
                                                            prefix-icon="el-icon-postcard"
                                                            :maxlength="255"
                                                            @blur="replaceInput(scope.row.value, scope.$index)"
                                                            clearable
                                                            size="mini"
                                                            :disabled="scope.row.noEdit"
                                                            :placeholder="$t('taskRepository.dialog.setValue')"
                                                        />
                                                    </el-form-item>
                                                </template>
                                            </el-table-column>

                                            <el-table-column :label="$t('taskRepository.dialog.taskTimeDur')" width="135">
                                                <template slot-scope="scope">
                                                    <el-form-item prop="timeDur">
                                                        <el-time-picker v-model="scope.row.timeDur" size="mini" :clearable="false"></el-time-picker>
                                                    </el-form-item>
                                                </template>
                                            </el-table-column>

                                            <el-table-column :label="$t('taskRepository.dialog.openStatus')" min-width="130" >
                                                <template slot-scope="scope" >
                                                    <el-switch
                                                        v-model="scope.row.isCanRun"
                                                        :active-text="$t('taskRepository.publics.button.start')"
                                                        :inactive-text="$t('taskRepository.publics.button.stop')"
                                                        class="switchStatus"

                                                    >
                                                    </el-switch>
                                                </template>
                                            </el-table-column>
                                        </el-table>
                                    </el-form>
                                </div>
                            </div>
                        </el-form-item>
                    </div>
                </el-form>
            </div>

            <div slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click="ptTaskDetail = false" size="small">{{ $t('taskRepository.publics.button.cancel') }}</el-button>
                <el-button type="primary" @click="savePtTask" size="small" :loading="ptLoading" >{{ $t('taskRepository.publics.button.confirm') }}</el-button>
            </div>
        </el-dialog>

        <!-- 循环任务框 -->
        <el-dialog width="900px" custom-class="xhTaskDialog" top="0" :visible.sync="xhTaskDetail" @closed="closeXhDetail" :close-on-click-modal="false">
            <div slot="title">
                <p>{{ $t('taskRepository.main.circleTask') }}</p>
            </div>

            <div class="dialog-body" v-loading="loading">
                <div class="dialog-body-xh-right" :class="$t('taskRepository.dialog.taskImplement').length > 12 ? 'setLabelWidth' : ''">
                    <el-form ref="xhForm" :model="xhForm" :rules="xhRule" class="base-info" :validate-on-rule-change="false" :label-position="left">
                        <el-form-item :label="$t('taskRepository.dialog.taskName') + ':'" prop="tableName">
                            <el-input v-model="xhForm.tableName" :placeholder="$t('taskRepository.dialog.alarmMsgName')" size="small"></el-input>
                        </el-form-item>

                        <el-form-item :label="$t('taskRepository.dialog.taskIntervalTime') + ':'" prop="cycleTask.intervalTime">
                            <el-time-picker
                                is-range
                                v-model="xhForm.cycleTask.intervalTime"
                                :rules="xhRule.cycleTask.intervalTime"
                                :start-placeholder="$t('taskRepository.dialog.startTimePlaceholder')"
                                :end-placeholder="$t('taskRepository.dialog.endTimePlaceholder')"
                                size="small"
                                :clearable="false"
                            >
                            </el-time-picker>
                        </el-form-item>

                        <el-form-item :label="$t('taskRepository.dialog.taskImplement') + ':'" :class="xhForm.cycleTask.implement == '3' ? 'el-select-50' : ''">
                            <el-select v-model="xhForm.cycleTask.implement" filterable :placeholder="$t('taskRepository.dialog.taskImplement')" size="small">
                                <el-option :key="zx1" :label="$t('taskRepository.dialog.startNow')" value="1"></el-option>
                                <el-option :key="zx2" :label="$t('taskRepository.dialog.startsOnTheHour')" value="2"></el-option>
                                <el-option :key="zx3" :label="$t('taskRepository.dialog.specifyTheStart')" value="3"> </el-option>
                            </el-select>
                            <el-time-picker v-model="xhForm.cycleTask.zhidingTime" size="small" :clearable="false" v-if="xhForm.cycleTask.implement == '3'"></el-time-picker>
                        </el-form-item>

                        <el-form-item :label="$t('taskRepository.dialog.taskLoop') + ':'" class="cycle-box">
                            <el-checkbox v-model="xhForm.cycleTask.isMaxCycle" size="small"
                                >{{ $t('taskRepository.dialog.taskIsMaxCycle') }}
                                <el-input-number
                                    v-model="xhForm.cycleTask.maxCycleNum"
                                    :disabled="!xhForm.cycleTask.isMaxCycle"
                                    :placeholder="$t('taskRepository.dialog.order')"
                                    size="small"
                                ></el-input-number>
                            </el-checkbox>
                            <el-checkbox v-model="xhForm.cycleTask.cycleMustFinish" :label="$t('taskRepository.dialog.taskMustFinish')" size="small"></el-checkbox>
                        </el-form-item>

                        <el-form-item :label="$t('taskRepository.dialog.taskComment') + ':'" class="taskTextarea">
                            <el-input
                                type="textarea"
                                maxlength="400"
                                clearable
                                show-word-limit
                                v-model="xhForm.comment"
                                :placeholder="$t('taskRepository.dialog.taskCommentPlaceholder')"
                                size="small"
                            ></el-input>
                        </el-form-item>
                    </el-form>
                </div>
                <div class="dialog-body-xh-left" id="dialog-body-xh-left">
                    <p>
                        {{ $t('taskRepository.dialog.taskControlSchedul') }}
                        <img id="contentFullScreen" src="/static/images/index-contentfull.svg" :title="$t('taskRepository.dialog.fullscreen')" @click.stop.prevent="getContentFullscreen(true)" />
                    </p>
                    <div>
                        <div class="dialog-body-xh-left-leftContent">
                            <span>
                                <a @click="xhControlItem = 2" :class="xhControlItem == 2 ? 'active' : ''">{{ $t('taskRepository.dialog.taskEqp') }}</a>
                                <a @click="initSysSelected" :class="xhControlItem == 1 ? 'active' : ''">{{ $t('taskRepository.dialog.taskSys') }}</a>
                                <a @click="xhControlItem = 3" :class="xhControlItem == 3 ? 'active' : ''">{{ $t('taskRepository.dialog.taskTimeItem') }}</a>
                            </span>
                            <el-form ref="xhSysForm" :model="xhSysForm" :label-position="top" class="sys-info" v-if="xhControlItem == 1">
                                <el-form-item prop="control" :rules="xhRule.control">
                                    <!-- <el-select v-model="xhSysForm.control" filterable :placeholder="$t('taskRepository.dialog.selectCtr')" size="small">
                                        <el-option v-for="item in sysList" :key="item.procCode" :label="item.cmdNm" :value="item.cmdNm + 'Gw2004' + item.procCode"></el-option>
                                    </el-select> -->

                                    <el-cascader-panel :options="sysControlTable" v-model="xhSysForm.control"></el-cascader-panel>
                                </el-form-item>
                            </el-form>
                            <el-form ref="xhEqpForm" :model="xhEqpForm" :label-position="top" class="eqp-info" v-if="xhControlItem == 2">
                                <el-form-item prop="control" :rules="xhRule.eqControl">
                                    <!-- <el-row :gutter="10">
                                        <el-col :span="10">
                                            <el-input
                                                v-model="xhEqpForm.controlName"
                                                @focus="focusCascader"
                                                ref="xhCascader"
                                                size="small"
                                                :placeholder="$t('taskRepository.dialog.selectEqpItem')"
                                            ></el-input>
                                        </el-col>
                                        <el-col :span="10">
                                            <el-input
                                                v-model="xhEqpForm.value"
                                                autocomplete="off"
                                                type="text"
                                                :maxlength="255"
                                                @blur="replaceInput(xhEqpForm.value)"
                                                size="small"
                                                clearable
                                                :disabled="cascaderXh"
                                                :placeholder="$t('taskRepository.dialog.setValue')"
                                            />
                                        </el-col>
                                    </el-row> -->
                                    <p slot="title">{{ $t('taskRepository.dialog.selectEqpItem') }}</p>
                                    <div class="list-box">
                                        <el-input :placeholder="$t('taskRepository.dialog.enterEqpName')" v-model="searchTreeName" @keyup.enter.native="inputChange" size="small" clearable>
                                            <i slot="prefix" class="el-input__icon el-icon-search"></i>
                                        </el-input>
                                        <div class="main">
                                            <myTree showSettings ref="myTree" @node-click="handleNodeClick"></myTree>
                                        </div>
                                    </div>
                                </el-form-item>
                            </el-form>
                            <el-form ref="xhTimeForm" :model="xhTimeForm" :label-position="top" class="time-info" v-if="xhControlItem == 3">
                                <el-form-item prop="totalSeconds" :rules="xhRule.totalSeconds">
                                    <el-row :gutter="10" class="timeInterval">
                                        <el-col :span="24">
                                            <el-input-number
                                                :min="0"
                                                v-model="xhTimeForm.day"
                                                @change="inputNumber('day')"
                                                size="small"
                                                :placeholder="$t('taskRepository.dialog.day')"
                                            ></el-input-number>
                                        </el-col>
                                        <el-col :span="24">
                                            <el-input-number
                                                :min="0"
                                                v-model="xhTimeForm.hour"
                                                @change="inputNumber('hour')"
                                                size="small"
                                                :placeholder="$t('taskRepository.dialog.time')"
                                            ></el-input-number>
                                        </el-col>
                                        <el-col :span="24">
                                            <el-input-number
                                                :min="0"
                                                v-model="xhTimeForm.min"
                                                @change="inputNumber('min')"
                                                size="small"
                                                :placeholder="$t('taskRepository.dialog.minutes')"
                                            ></el-input-number>
                                        </el-col>
                                        <el-col :span="24">
                                            <el-input-number
                                                :min="0"
                                                v-model="xhTimeForm.second"
                                                @change="inputNumber('second')"
                                                size="small"
                                                :placeholder="$t('taskRepository.dialog.second')"
                                            ></el-input-number>
                                        </el-col>
                                    </el-row>
                                    <!-- <el-button @click="addTimeItem" icon="el-icon-plus" class="new-btn" size="small" round>{{ $t('taskRepository.dialog.addControl') }}</el-button> -->
                                </el-form-item>
                            </el-form>
                        </div>
                        <div class="dialog-body-xh-left-centerContent">
                            <i class="iconfont icon-tubiao24_youbian" @click="addConcatControlItem"></i>
                        </div>
                        <div class="dialog-body-xh-left-rightContent">
                            <el-form ref="cycleTaskContent" :model="xhForm" :rules="xhRule" :validate-on-rule-change="true">
                                <el-form-item prop="cycleTaskContent">
                                    <header>
                                        <el-checkbox :indeterminate="isIndeterminate" v-model="checkAll" @change="handleCheckAllChange">
                                            {{ $t('taskRepository.dialog.taskCtr') }}
                                            <el-tooltip class="item" effect="dark" :content="$t('taskRepository.dialog.controlTips')" placement="top-end">
                                                <i class="iconfont icon-gw-icon-wenhao icons"></i>
                                            </el-tooltip>
                                        </el-checkbox>
                                        <el-button type="text" class="danger" size="small" @click="delItem" v-if="checkedArr.length > 0"
                                            ><i class="el-icon-delete"></i>{{ $t('taskRepository.main.delete') }}</el-button
                                        >
                                    </header>
                                    <div class="base-info">
                                        <el-checkbox-group v-model="checkedArr" @change="handleCheckedChange" v-if="xhForm.cycleTaskContent.length > 0">
                                            <draggable tag="div" v-bind="dragOptions" v-model="xhForm.cycleTaskContent" class="ctr-list">
                                                <transition-group type="transition">
                                                    <div v-for="(item, index) in xhForm.cycleTaskContent" :key="'SET' + index">
                                                        <!--时间线  -->
                                                        <span class="timeLine" :style="{ backgroundColor: item.color }">
                                                            <i :class="item.icon"></i>
                                                        </span>
                                                        <!-- 控制项 -->
                                                        <el-checkbox :label="item" :key="'S' + index" v-if="item.type === 'S'">{{ item.firstLevelDirectory + '/' + item.cmdNm }}</el-checkbox>
                                                        <el-checkbox :label="item" :key="'E' + index" v-if="item.type === 'E'">
                                                            {{ item.equipSetNm }}
                                                            <p v-if="item.attribute == 'yc'" class="ctr-list-setValue">
                                                                {{ $t('taskRepository.dialog.setValue') }}
                                                                <el-input v-model="item.value" size="small" class="setYcValue" clearable @blur="setEquipValue(index)"></el-input>
                                                            </p>
                                                        </el-checkbox>
                                                        <el-checkbox :label="item" :key="'T' + index" v-if="item.type === 'T'">
                                                            <el-input v-model="item.day" size="small" class="setDateInput" @input="value => setTimeValue(value, index, 'day')"></el-input>
                                                            {{ $t('taskRepository.dialog.d') }}
                                                            <el-input v-model="item.hour" size="small" class="setDateInput" @input="value => setTimeValue(value, index, 'hour')"></el-input>
                                                            {{ $t('taskRepository.dialog.h') }}
                                                            <el-input v-model="item.minutes" size="small" class="setDateInput" @input="value => setTimeValue(value, index, 'minutes')"></el-input>
                                                            {{ $t('taskRepository.dialog.m') }}
                                                            <el-input v-model="item.second" size="small" class="setDateInput" @input="value => setTimeValue(value, index, 'second')"></el-input>
                                                            {{ $t('taskRepository.dialog.s') }}
                                                        </el-checkbox>
                                                    </div>
                                                </transition-group>
                                            </draggable>
                                        </el-checkbox-group>
                                        <div class="noDataTips" :data-nodata="$t('taskRepository.publics.noData')" v-else></div>
                                    </div>
                                </el-form-item>
                            </el-form>
                        </div>
                    </div>
                </div>
            </div>

            <div slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click="xhTaskDetail = false" size="small">{{ $t('taskRepository.publics.button.cancel') }}</el-button>
                <el-button type="primary" @click="saveXhTask" size="small" :loading="xhLoading" >{{ $t('taskRepository.publics.button.confirm') }}</el-button>
            </div>
        </el-dialog>
    </div>
</template>
<script>
import taskRepository from './js/taskRepository.js'

export default taskRepository
</script>
<style scoped lang="scss" src="./css/taskRepository.scss"></style>
