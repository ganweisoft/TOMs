// 设备联动-场景编辑
<template>
    <div class="sceneSetting">
        <div class="title">
            <div class="left">
                <span>{{ $t('sceneSetting.title.headName') }}({{ page.total }})</span>
                <el-icon @click="showAdd" ><CirclePlus /></el-icon>
            </div>
            <div class="right">
                <el-input clearable v-model="keyword" @change="getSceneList()" :placeholder="$t('sceneSetting.input.searchTriggerEquip')">
                    <template v-slot:prefix>
                        <el-icon><Search></Search></el-icon>
                    </template>
                </el-input>
            </div>
        </div>
        <div class="content" v-loading="listLoad">
            <div class="noDataTips" :data-noData="$t('sceneSetting.label.noRelatedContent')"
                v-if="!listLoad && page.total == 0">
            </div>
            <el-row :gutter="20" class="taskList" v-else>
                <el-col :xs="12" :sm="8" :md="6" :lg="4" :xl="4" v-for="(item, index) in sceneRepository" :key="index"
                    data-type="repository" data-choice="0" class="repositoryItem">
                    <div @click="showEdit(item)">
                        <p>{{ item.sceneName }}</p>
                        <p>{{ item.sum }}{{ $t('sceneSetting.label.unnamed') }}</p>
                        <el-popover placement="bottom" trigger="hover" :width="55">
                            <el-button @click="confirmDelete(item)" link type="danger" size="small" >{{ $t('sceneSetting.button.del')}}</el-button>
                            <template v-slot:reference>
                                <i class="iconfont icon-tubiao20_gengduo icon-tubiao20_gengduo" ></i>
                            </template>
                        </el-popover>
                    </div>
                </el-col>
            </el-row>
        </div>

        <el-dialog v-model="addVisible" :title="$t('sceneSetting.dialog.title')" :close-on-click-modal="false" width="900px" top="15vh" @close="clossDialog">
            <div class="dialog-body">
                <div class="dialog-body-xh-right">
                    <el-form ref="nameFormRef" :model="xhForm" class="demo-form-inline" :rules="xhRule"
                        :label-position="$t('sceneSetting.input.inputScenenName').length > 12 ? 'top' : 'right'">
                        <el-form-item :label="$t('sceneSetting.dialog.leftTitle') + ' :'" class="paddingRight"
                            prop="name">
                            <el-input v-model="xhForm.name" :placeholder="$t('sceneSetting.input.inputScenenName')" clearable
                                maxLength="64"></el-input>
                        </el-form-item>
                    </el-form>
                </div>
                <div class="dialog-body-xh-left" id="dialog-body-xh-left-id">
                    <p>
                        {{ $t('sceneSetting.dialog.taskControlSchedul') }}
                        <!-- <img id="contentFullScreen" src="/static/images/index-contentfull.svg"
                            :title="$t('sceneSetting.dialog.fullscreen')"
                            @click.stop.prevent="getContentFullscreen(true)" /> -->
                    </p>
                    <div>
                        <div class="dialog-body-xh-left-leftContent">
                            <span>
                                <a @click="tabChange(tabNames[0])" :class="{active: isActive(tabNames[0])}">
                                    {{ $t('sceneSetting.dialog.rightTitle') }}
                                </a>
                                <a @click="tabChange(tabNames[1])" :class="{active: isActive(tabNames[1])}">
                                    {{ $t('sceneSetting.label.lenTime') }}
                                </a>
                            </span>

                            <el-form ref="xhEqpFormRef" :model="xhForm" :rules="xhRule" label-position="top" class="eqp-info"
                                v-if="xhControlItem === tabNames[0]">
                                <el-form-item prop="equip" >
                                    <div class="list-box">
                                        <el-input :placeholder="$t('sceneSetting.dialog.enterEqpName')"
                                            v-model="searchEquip" @change="changeSearchEquip" clearable>
                                            <template v-slot:prefix>
                                                <el-icon><Search/></el-icon>
                                            </template>
                                        </el-input>
                                        <div class="main">
                                            <myTree v-if="addVisible" showSettings ref="myTreeRef" @node-click="handleNodeClick"></myTree>
                                        </div>
                                    </div>
                                </el-form-item>
                            </el-form>
                            <el-form ref="xhTimeFormRef" :model="xhForm" :rules="xhRule" label-position="top" class="time-info"
                                v-if="xhControlItem === tabNames[1]">
                                <el-form-item prop="conTime">
                                    <el-row :gutter="10" class="timeInterval">
                                        <el-col :span="24">
                                            <el-input v-model="xhForm.conTime" :placeholder="$t('sceneSetting.input.inputTime')"
                                                oninput="value=value.replace(/[^\d]/g,'')" maxLength="9">
                                                <template v-slot:suffix>
                                                    <span>ms</span>
                                                </template>
                                            </el-input>
                                        </el-col>
                                    </el-row>
                                </el-form-item>
                            </el-form>
                        </div>
                        <div class="dialog-body-xh-left-centerContent">
                            <i class="iconfont icon-tubiao24_youbian" @click="addConcatControlItem"></i>
                        </div>
                        <div class="dialog-body-xh-left-rightContent">
                            <el-form ref="cycleTaskContentRef" :model="xhForm" :rules="xhRule">
                                <el-form-item prop="cycleTaskContent">
                                    <header>
                                        <el-checkbox :indeterminate="indeterminate" v-model="checkAll" @change="checkAllChange">
                                            {{ $t('sceneSetting.dialog.centerTitle') }}
                                        </el-checkbox>
                                        <el-button type="text" class="danger" size="small" @click="deleteXhFormCycleTaskContent(checkedModel)">
                                            <el-icon><Delete /></el-icon>
                                            {{ $t('sceneSetting.button.del') }}
                                        </el-button>
                                    </header>
                                    <div class="base-info">
                                        <el-checkbox-group v-model="checkedModel" @change="checkChange" item-key="id"
                                            v-if="xhForm.cycleTaskContent.length > 0">
                                            <draggable v-bind="dragOptions" v-model="xhForm.cycleTaskContent" class="ctr-list">
                                                <template #item="{element}">
                                                    <div>
                                                        <span class="timeLine" :style="{ backgroundColor: element.color }">
                                                            <i :class="element.icon"></i>
                                                        </span>
                                                        <el-checkbox :label="element.id" v-if="element.type === 'E'">
                                                            {{ element.equipName + '-' + element.setNm }}
                                                            <p v-if="element.attribute == 'yc'" class="ctr-list-setValue">
                                                                {{ $t('sceneSetting.label.setValue') }}
                                                                <el-input v-model="element.value" class="setYcValue" clearable></el-input>
                                                            </p>
                                                        </el-checkbox>
                                                        <el-checkbox :label="element.id" v-if="element.type === 'T'">
                                                            {{ $t('sceneSetting.dialog.intervalTime') }}
                                                            <p class="ctr-list-setValue">
                                                                <el-input v-model="element.conTime"
                                                                    :placeholder="$t('sceneSetting.input.inputTime')"
                                                                    oninput="value=value.replace(/[^\d]/g,'')"
                                                                    maxLength="9">
                                                                </el-input>
                                                                <span style="margin-right: 2px">
                                                                    ms
                                                                </span>
                                                            </p>
                                                        </el-checkbox>
                                                    </div>
                                                </template>
                                            </draggable>
                                        </el-checkbox-group>
                                        <div v-else class="noDataTips" :data-nodata="$t('sceneSetting.label.noRelatedContent')">
                                        </div>
                                    </div>
                                </el-form-item>
                            </el-form>
                        </div>
                    </div>
                </div>
            </div>
            <template v-slot:footer>
                    <div class="dialog-footer">
                        <el-button type="primary" plain @click="clossDialog">
                            {{ $t('sceneSetting.button.cancel') }}
                        </el-button>
                        <el-button type="primary" @click="confirmAddorEdit" :loading="loading"  v-if="isEdit">
                            {{ $t('sceneSetting.button.comfirmed')}}
                        </el-button>
                        <el-button type="primary" @click="confirmAddorEdit" :loading="loading"  v-else>
                            {{ $t('sceneSetting.button.comfirmed')}}
                        </el-button>
                    </div>
                </template>
        </el-dialog>

        <div class="pagination">
            <el-pagination background :pager-count="7" :page-sizes="[20, 50, 100]" :total="page.total"
                v-model:page-size="page.pageSize" v-model:current-page="page.pageNo"
                layout="sizes, prev, pager, next, jumper,total" ></el-pagination>
        </div>
    </div>
</template>

<script setup lang="ts">
import draggable from 'vuedraggable'

import myTree from '@components/@ganwei-pc/gw-base-components-plus/treeV2'

import AddSceneSetting from './js/AddSceneSetting';
import DelSceneSetting from './js/DelSceneSetting';
import GetSceneSettings, { UUID } from './js/GetSceneSettings';
import { useCheckBox, useTabs } from './js/useSettingPanel';
import useTree from './js/useTree';

const { sceneRepository, getSceneList, keyword, listLoad, page } = GetSceneSettings()
const {
    nameFormRef, xhEqpFormRef, xhTimeFormRef, cycleTaskContentRef, xhForm, xhRule, deleteXhFormCycleTaskContent,
    addVisible, showAdd, showEdit, closeAdd, confirmAddorEdit, loading, isEdit
} = AddSceneSetting(getSceneList)

const { confirmDelete } = DelSceneSetting(getSceneList)

const tabNames = ['2', '3']
const { activeTab: xhControlItem, tabChange, isActive } = useTabs(tabNames)
const { myTreeRef, searchEquip, changeSearchEquip} = useTree()

const dragOptions = {
    animation: 0,
    group: 'description',
    disabled: false,
    tag: "div"
}
const { indeterminate, checkedModel, checkAll, checkAllChange, checkChange, reset } = useCheckBox(() => xhForm.cycleTaskContent, 'id')

function clossDialog() {
    closeAdd()
    reset()
}

function addConcatControlItem() {
    if(xhControlItem.value === tabNames[1]) {
        return xhTimeFormRef.value?.validate().then(() => {
            xhForm.cycleTaskContent.push({
                type: 'T',
                icon: 'iconfont icon-gw-icon-menu-dingshibaobiao',
                color: '#2EBFDF',
                conTime: xhForm.conTime || '0',
                id: UUID()
            })
        })
    }
    return xhEqpFormRef.value?.validate().then(() => {
        xhForm.cycleTaskContent.push({
            type: 'E',
            equipNo: xhForm.equip.equipSetValue[0],
            setNo: xhForm.equip.equipSetValue[1],
            value: xhForm.equip.value || '',
            equipName: xhForm.equip.equipName,
            setNm: xhForm.equip.setNm,
            setType: xhForm.equip.setType,
            attribute: xhForm.equip.value ? 'yc' : 'yx',
            icon: 'iconfont icon-gw-icon-menu-dingshibaobiao',
            color: '#2EBFDF',
            id: UUID()
        })
    })
}

function handleNodeClick(data) {
    if (data.isSetting) {
        const noEdit = data.setType !== 'V' // 设置设置值是否可输入，选择遥测时可输入，选择遥信则不可输入
        xhForm.equip = {
            value: noEdit ? data.setValue || '' : '0',
            equipSetValue: [data.equipNo, data.setNo],
            defaultValue: data.value || '',
            setType: data.setType,
            equipName: data.equipName,
            setNm: data.setNm
        }
    }
}
</script>

<style scoped lang="scss" src="./css/sceneSetting.scss"></style>
