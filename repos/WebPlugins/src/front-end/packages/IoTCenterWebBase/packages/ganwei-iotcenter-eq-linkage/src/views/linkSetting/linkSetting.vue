// 设备联动-联动配置
<template>
    <div class="linkSetting">
        <div class="left">
            <!-- 顶部标题 -->
            <header>
                <div class="title_left">
                    <span>{{ $t('linkSetting.title.headName') }} ({{ page.total }})</span>
                    <el-popover placement="bottom" trigger="hover" popper-class="linkSetting-popover" >
                        <el-button-group class="new-button-group">
                            <el-button type="text" size="small" class="elBtn" v-on:click.stop="showAdd(1)">
                                <i class="export iconfont icon-tubiao20_tianjiatupian"></i>
                                {{$t('linkSetting.dialog.newTitleName')}}
                            </el-button>
                            <el-button type="text" size="small" class="elBtn" v-on:click.stop="showAdd(2)">
                                <i class="export iconfont icon-tubiao20_tianjiatupian"></i>
                                {{$t('linkSetting.dialog.newIfTitleName')}}
                            </el-button>
                        </el-button-group>
                        <template #reference >
                            <el-icon >
                                <CirclePlus />
                            </el-icon>
                        </template>
                    </el-popover>
                    <i @click="openFilterDialog" class="iconfont icon-gw-icon-shaixuan1 filter" ></i>
                </div>
                <div class="title_right">
                    <el-input clearable @change="search" v-model="equipName"
                        :placeholder="$t('linkSetting.input.searchTriggerEquip')">
                        <template v-slot:prefix>
                            <el-icon>
                                <Search />
                            </el-icon>
                        </template>
                    </el-input>
                </div>
            </header>

            <!-- 中间内容块 -->
            <section>
                <div id="eqTable">
                    <el-table :data="tableData" v-loading="linkLoad" border>
                        <template v-slot:empty>
                            <div class="noDataTips" :data-noData="$t('linkSetting.label.noRelatedContent')"></div>
                        </template>
                        <el-table-column prop="toggleEqName" :label="$t('linkSetting.table.triggerEquip')" min-width="10%"
                            show-overflow-tooltip>
                        </el-table-column>
                        <el-table-column prop="toggleTypeName" class-name="tableToggleType"
                            :label="$t('linkSetting.table.triggerType')" width="140px" show-overflow-tooltip>
                        </el-table-column>
                        <el-table-column prop="ToggleDotName" :label="$t('linkSetting.table.triggerPoint')" min-width="10%"
                            show-overflow-tooltip></el-table-column>
                        <el-table-column prop="delay" :label="$t('linkSetting.table.delayTime')" min-width="10%"
                            show-overflow-tooltip></el-table-column>
                        <el-table-column prop="linkEqName" :label="$t('linkSetting.table.linkEquip')" min-width="10%"
                            show-overflow-tooltip></el-table-column>
                        <el-table-column prop="linkOrder.setName" :label="$t('linkSetting.table.linkCommand')" min-width="10%"
                            show-overflow-tooltip></el-table-column>
                        <el-table-column prop="value" :label="$t('linkSetting.table.commandInstructions')"
                            min-width="10%" show-overflow-tooltip></el-table-column>
                        <el-table-column prop="remark" :label="$t('linkSetting.table.remarksInfo')" min-width="10%"
                            show-overflow-tooltip></el-table-column>
                        <el-table-column prop="enable" :label="$t('linkSetting.input.enable')" min-width="10%"
                            show-overflow-tooltip>
                            <template v-slot="scope">
                                {{ scope.row.enable ? $t('linkSetting.input.yes') : $t('linkSetting.input.no')}}
                            </template>
                        </el-table-column>
                        <el-table-column prop="control" width="120" :label="$t('linkSetting.table.operation')"
                            show-overflow-tooltip fixed="right">
                            <template v-slot="scope">
                                <i class="iconfont icon-jieguo" @click="showEdit(scope.row,false)" ></i>
                                <i class="iconfont icon-gw-icon-bianji" @click="showEdit(scope.row, true)" ></i>
                                <i class="iconfont icon-gw-icon-shangchu" @click="delData(scope.row.toggleEqName, scope.row.toggleEq, scope.row.id, scope.row.isConditionLink)" ></i>
                            </template>
                        </el-table-column>
                    </el-table>
                </div>

                <!-- 分页 -->
                <div class="pagination mobilePagination">
                    <el-pagination background ref="pagination" :pager-count="7" :page-sizes="[20, 50, 100]"
                        v-model:page-size="page.pageSize" v-model:current-page="page.pageNo"
                        layout="sizes, prev, pager, next, jumper,total" :total="page.total"></el-pagination>
                </div>
            </section>

            <!-- 新建弹窗 -->
            <el-dialog :title="dialogTitle" v-model="addVisible"
                @close="resetAdd" width="840px" :close-on-click-modal="false" :modal-append-to-body="false">
                <div v-if="addVisible" class="formDetail" :class="{formDetailDisable: !isView}">
                    <el-form :model="newLinkInfo" ref="linkInfoNew" :rules="addRules" class="demo-form-inline"
                        :label-position="$t('linkSetting.table.triggerEquip').length > 12 ? 'top' : 'right'"
                        label-width="110px">
                        <el-row :gutter="10" v-if="addType == 1">
                            <el-col :span="12">
                                <el-form-item :label="$t('linkSetting.table.triggerEquip')" prop="toggleEqName">
                                    <el-input :placeholder="$t('linkSetting.input.triggerDevice')" v-model="newLinkInfo.toggleEqName"
                                        @click="showEquipSelect" readonly></el-input>
                                    </el-form-item>
                                    <equipSelect v-if="equipSelectVisible" :selectedChange="triggerSelectedChange" :selectedMode="1"/>
                            </el-col>
                            <el-col :span="12">
                                <el-form-item :label="$t('linkSetting.table.triggerType')" prop="toggleType">
                                    <el-select v-model="newLinkInfo.toggleType"
                                        :placeholder="$t('linkSetting.input.triggerType')" @change="triggerTypeChange" filterable>
                                        <el-option v-for="(item, index) in addTriggerTypes" :key="index" :label="item.label"
                                            :value="item.value"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <el-row :gutter="10">
                            <el-col :span="12" v-if="addType == 1">
                                <el-form-item :label="newLinkInfo.toggleType != 'evt' ? $t('linkSetting.table.triggerPoint') : $t('linkSetting.table.eventExpression')"
                                    prop="toggleDot">
                                    <el-select v-if="['c', 'C'].includes(newLinkInfo.toggleType)" v-model="newLinkInfo.toggleDot"
                                        :placeholder="$t('linkSetting.input.triggerPoint')" filterable>
                                        <el-option v-for="(item, index) in triggerPoints" :key="index" :label="item.ycNm"
                                            :value="String(item.ycNo)"></el-option>
                                    </el-select>
                                    <el-select v-else-if="['x', 'X'].includes(newLinkInfo.toggleType)"
                                        v-model="newLinkInfo.toggleDot" :placeholder="$t('linkSetting.label.pleaseSel')" filterable>
                                        <el-option v-for="(item, index) in triggerPoints" :key="index" :label="item.yxNm"
                                            :value="String(item.yxNo)"></el-option>
                                    </el-select>
                                    <el-input v-else v-model="newLinkInfo.toggleDot" :placeholder="$t('linkSetting.label.pleaseInput')"></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :span="12" v-if="addType == 2">
                                <el-form-item :label="$t('linkSetting.table.procName')" prop="procName">
                                    <el-input :placeholder="$t('linkSetting.input.newLinkInfoProcName')" v-model="newLinkInfo.procName" clearable></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :span="12" >
                                <el-form-item :label="$t('linkSetting.table.delayTime')" prop="delay">
                                    <el-input-number v-model="newLinkInfo.delay" :min="0"
                                        :max="21474123647"></el-input-number>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <!-- 条件联动 start -->
                        <el-row :gutter="10" class="controlEquipContainer" v-if="addType == 2">
                           <el-col :span="24" class="controlEquip-add">
                              <div>{{$t('linkSetting.tips.linkIf')}}</div>
                              <div class="modalSelect" >
                                <label>(
                                    {{$t('linkSetting.tips.linkIfTipsStart')}}
                                    <el-switch inline-prompt class="orAnd" style="--el-switch-on-color: #ff4949; --el-switch-off-color: #ff4949" v-model="newLinkInfo.conditionRelation" :active-text="$t('linkSetting.input.or')" :inactive-text="$t('linkSetting.input.and')"></el-switch>
                                    {{$t('linkSetting.tips.linkIfTipsEnd')}}
                                    )
                                </label>
                              </div>
                              <span>
                                  <el-icon><CirclePlus @click="addItem"/></el-icon>
                              </span>
                           </el-col>
                           <el-col :span="24" class="controlEquip-list">
                                    <dl v-for="(item, index) in newLinkInfo.ifLinkEquipList" :key="index">
                                        <dt>
                                            <el-form-item :label="$t('linkSetting.table.triggerEquip')" :prop="`ifLinkEquipList[${index}].iequipNm`" :rules="[{ required: true, message: $t('linkSetting.input.triggerDevice'), trigger: 'change' }]">
                                                    <el-input :placeholder="$t('linkSetting.input.triggerDevice')" v-model="item.iequipNm"
                                                        @click="showEquipSelect(index)" readonly></el-input>
                                            </el-form-item>
                                            <equipSelect v-if="equipSelectVisible" :selectedChange="triggerSelectedChange" :selectedMode="1"/>
                                            <span>
                                                <el-icon><CirclePlus @click="addChildrenItem(index)"/></el-icon>
                                                <el-icon><Remove @click="removeItem(index)"/></el-icon>
                                            </span>

                                        </dt>
                                        <dd v-for="(itemdd, indexdd) in item.iYcYxItems" :key="'dd-'+index+'-'+indexdd" >
                                            <el-form-item :prop="`ifLinkEquipList[${index}].iYcYxItems[${indexdd}].iycyxNo`" :rules="[{ required: true, message: $t('linkSetting.input.selectLinkIf'), trigger: 'change' }]">
                                                
                                                <el-select
                                                    v-model="itemdd.iycyxNo"
                                                    clearable
                                                    :placeholder="$t('linkSetting.input.selectLinkIf')"
                                                    @change="selectLinkIf(itemdd);itemdd.condition = 0;itemdd.iycyxValue = '';"
                                                    filterable
                                                >
                                                    <el-option
                                                    v-for="item1 in item.controlCommand"
                                                    :key="item1.ycyxNo"
                                                    :label="item1.ycyxName"
                                                    :value="item1.ycyxNo+'-'+item1.ycyxType"
                                                    :a="item1.ycyxNo+'-'+item1.ycyxType"
                                                    />

                                                </el-select>
                                            </el-form-item>
                                            <el-form-item >
                                                <el-select
                                                    v-model="itemdd.condition"
                                                    clearable
                                                    @click="selectLinkIf(itemdd)"
                                                    :placeholder="$t('linkSetting.input.selectJudgeIf')"
                                                >
                                                    <el-option
                                                    v-for="item in currentIfOptions"
                                                    :key="item.value"
                                                    :label="item.label"
                                                    :value="item.value"
                                                    />
                                                </el-select>
                                            </el-form-item>
                                            <el-form-item v-if="isEquipStatus(itemdd.iycyxNo,item.controlCommand)" :prop="`ifLinkEquipList[${index}].iYcYxItems[${indexdd}].iycyxValue`" :rules="[{ required: true, message: $t('linkSetting.input.selectEquipStatus'), trigger: 'change' }]">
                                                <el-select
                                                    v-model="itemdd.iycyxValue"
                                                    clearable
                                                    :placeholder="$t('linkSetting.input.selectEquipStatus')"
                                                    filterable >
                                                    <el-option
                                                    v-for="item in ifCommandOptions"
                                                    :key="item.value"
                                                    :label="item.label"
                                                    :value="item.value"
                                                    />
                                                </el-select>
                                            </el-form-item>
                                            <el-form-item  v-else :prop="`ifLinkEquipList[${index}].iYcYxItems[${indexdd}].iycyxValue`" :rules="[{ required: true, message: $t('linkSetting.input.commandInstructions'), trigger: 'blur' }]">
                                                <el-input :placeholder="$t('linkSetting.input.commandInstructions')"
                                                    v-model="itemdd.iycyxValue" maxlength="255" clearable></el-input>
                                            </el-form-item>
                                            <el-icon><Remove @click="removeChildrenItem(index,indexdd)"/></el-icon>
                                        </dd>
                                    </dl>
                           </el-col>
                        </el-row>
                        <!-- 条件联动 end -->
                        <el-row :gutter="10">
                            <el-col :span="12">
                                <el-form-item :label="$t('linkSetting.table.linkEquip')" prop="linkEqName">
                                    <el-input readonly :placeholder="$t('linkSetting.input.linkEquip')"
                                        suffix-icon="el-icon-arrow-down" v-model="newLinkInfo.linkEqName"
                                        @click.stop="showLinkEquipSelect"></el-input>
                                    </el-form-item>
                                    <equipSelect v-if="linkEquipSelectVisible" :selectedChange="linkSelectedChange" :selectedMode="1"/>
                            </el-col>
                            <el-col :span="12">
                                <el-form-item :label="$t('linkSetting.table.linkCommand')" prop="linkOrder">
                                    <el-select v-model="newLinkInfo.linkOrder" @change="getOrder" value-key="setNo"
                                        :placeholder="$t('linkSetting.input.linkCommand')" filterable>
                                        <el-option v-for="(item, index) in setParams" :key="item.setNo"
                                            :label="item.setNm" :value="item"></el-option>
                                    </el-select>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <el-row :gutter="10">
                            <el-col :span="12">
                                <el-form-item :label="$t('linkSetting.table.commandInstructions')" prop="value">
                                    <el-input :disabled="newLinkInfo.linkOrder.setType != 'V'" :placeholder="$t('linkSetting.input.commandInstructions')"
                                        v-model="newLinkInfo.value" maxlength="255"></el-input>
                                </el-form-item>
                            </el-col>
                            <el-col :span="12">
                                <el-form-item :label="$t('linkSetting.input.enable')" style="text-align: left;">
                                    <el-switch inline-prompt v-model="newLinkInfo.enable" :active-text="$t('linkSetting.input.yes')" :inactive-text="$t('linkSetting.input.no')"></el-switch>
                                </el-form-item>
                            </el-col>
                        </el-row>
                        <el-row :gutter="10" class="marginTop">
                            <el-col :span="24">
                                <el-form-item :label="$t('linkSetting.table.remarksInfo')">
                                    <el-input type="textarea" v-model="newLinkInfo.remark" maxlength="255"
                                        :placeholder="$t('linkSetting.input.remarksInfo')"></el-input>
                                </el-form-item>
                            </el-col>
                        </el-row>
                    </el-form>
                </div>
                <template v-slot:footer>
                    <span class="dialog-footer" v-if="isView">
                        <el-button type="primary" plain @click="addVisible = false">{{ $t('linkSetting.button.cancel')}}</el-button>
                        <el-button type="primary" @click="confirmAdd" :loading="isEdit?(addType == 1?editLoading:editIfLoading):(addType == 1?addLoading:addIfLoading)">{{ $t('linkSetting.button.comfirmed') }}</el-button>
                    </span>
                </template>
            </el-dialog>

            <!-- 过滤窗体 -->
            <el-dialog :title="$t('linkSetting.title.popoverTitle')" v-model="filterVisible" width="840px"
                :close-on-click-modal="false" :modal-append-to-body="false">
                <div class="formDetail">
                    <el-row :gutter="0" class="content">
                        <el-col :span="$t('linkSetting.table.triggerEquip').length > 12 ? 24 : 12" class="eqInfo"
                            :class="$t('linkSetting.table.triggerEquip').length > 12 ? 'labelWidth' : ''">
                            <span>{{ $t('linkSetting.table.triggerEquip') }}</span>
                            <el-select v-model="filterValue.triggerEquip" filterable
                                :placeholder="$t('linkSetting.input.triggerDevice')" multiple value-key="equipNm">
                                <el-option v-for="(item, index) in equipList?.triggerEquips" :key="index"
                                    :label="item.equipNm" :value="item.equipNo"> </el-option>
                            </el-select>
                        </el-col>
                        <el-col :span="$t('linkSetting.table.triggerEquip').length > 12 ? 24 : 12" class="eqInfo"
                            :class="$t('linkSetting.table.triggerEquip').length > 12 ? 'labelWidth' : ''">
                            <span>{{ $t('linkSetting.table.triggerType') }}</span>
                            <el-select v-model="filterValue.triggerType" filterable
                                :placeholder="$t('linkSetting.input.triggerType')" multiple value-key="equipNm">
                                <el-option v-for="(item, index) in triggerTypes" :key="index" :label="item.label"
                                    :value="item.value">
                                </el-option>
                            </el-select>
                        </el-col>

                        <el-col :span="$t('linkSetting.table.triggerEquip').length > 12 ? 24 : 12" class="eqInfo"
                            :class="$t('linkSetting.table.triggerEquip').length > 12 ? 'labelWidth' : ''">
                            <span>{{ $t('linkSetting.table.linkEquip') }}</span>
                            <el-select v-model="filterValue.linkEquip" filterable
                                :placeholder="$t('linkSetting.input.linkEquip')" multiple @change="changeFilterLinkEquip" value-key="equipNm">
                                <el-option v-for="(item, index) in equipList?.linkEquips" :key="index"
                                    :label="item.equipNm" :value="item.equipNo"> </el-option>
                            </el-select>
                        </el-col>
                        <el-col :span="$t('linkSetting.table.triggerEquip').length > 12 ? 24 : 12" class="eqInfo"
                            :class="$t('linkSetting.table.triggerEquip').length > 12 ? 'labelWidth' : ''">
                            <span>{{ $t('linkSetting.table.linkCommand') }}</span>
                            <el-select v-model="filterValue.linkEquipCommand" filterable
                                :placeholder="$t('linkSetting.input.linkCommand')" multiple value-key="name">
                                <el-option v-for="(item, index) in linkCommands" :key="index" :label="item.name"
                                    :value="item"> </el-option>
                            </el-select>
                        </el-col>
                       <!-- 
                        <el-col :span="$t('linkSetting.table.triggerEquip').length > 12 ? 24 : 12" class="eqInfo"
                            :class="$t('linkSetting.table.triggerEquip').length > 12 ? 'labelWidth' : ''">
                            <span>{{ $t('linkSetting.table.delayTime') }}</span>
                            <div class="input">
                                <el-input-number v-model="filterValue.triggerTimeMin" :placeholder="$t('linkSetting.label.minimum')" :min="0" :max="21474123647"></el-input-number>
                                <p>{{ $t('linkSetting.label.to') }}</p>
                                <el-input-number v-model="filterValue.triggerTimeMax" :placeholder="$t('linkSetting.label.highest')" :min="filterValue.triggerTimeMin" :max="21474123647"></el-input-number>
                            </div>
                        </el-col> -->
                    </el-row>
                </div>
                <template v-slot:footer>
                    <span class="dialog-footer">
                        <el-button type="primary" plain @click="filterVisible = false">{{ $t('linkSetting.button.cancel') }}</el-button>
                        <el-button type="primary" @click="filter"
                            :loading="linkLoad">{{ $t('linkSetting.button.comfirmed') }}</el-button>
                        <el-button type="primary" @click="reset">{{ $t('linkSetting.button.reset') }}</el-button>
                    </span>
                </template>
            </el-dialog>
        </div>
    </div>
</template>

<script setup lang="ts">
// import {computed, reactive, ref} from 'vue';
import { useI18n } from 'vue-i18n';

import equipSelect from '@components/@ganwei-pc/gw-base-components-plus/equipSelect/equipSelect.vue'

import AddLinkSetting from './js/addLinkSetting';
import deleteLinkSetting from './js/deleteLinkSetting';
import getEquipLinkList from './js/getEquipLinkList';
import useFilterDialog from './js/useFilterDialog';

const $t = useI18n().t
let currentIfOptions: any[] = []
const { filterValue, filterVisible, resetFilter, openFilterDialog, equipList, triggerTypes, changeFilterLinkEquip, linkCommands } = useFilterDialog()
const { equipName, linkLoad, page, tableData, search } = getEquipLinkList(filterValue)

const {
    dialogTitle, showAdd, showEdit,
    getOrder, addVisible, resetAdd, addForm: newLinkInfo, addRules, addorEditLinking, linkInfoNew, addLoading,
    equipSelectVisible, triggerSelectedChange, showEquipSelect, addTriggerTypes, triggerPoints, triggerTypeChange,
    linkEquipSelectVisible, linkSelectedChange, showLinkEquipSelect, setParams,
    isEdit, isView, editLoading, editIfLoading, addIfLoading, ifOptions, ifCommandOptions, addType, addItem, removeItem, addChildrenItem, removeChildrenItem
} = AddLinkSetting()
currentIfOptions = ifOptions

const { delData } = deleteLinkSetting(search)

function filter() {
    search().finally(() => {
        filterVisible.value = false
    })
}

function reset() {
    resetFilter()
    filter()
}

function confirmAdd() {
    addorEditLinking().then(() => {
        addVisible.value = false
        search()
    })
}

function isEquipStatus(val:any, arry:any){
    let isStatus = false;
    if(Array.isArray(arry) && arry.length > 0) {
        let dt = arry.filter(item=>(item.ycyxNo + "-" + item.ycyxType) === val)
        if(dt.length > 0 && ['E', 'e'].includes(dt[0].ycyxType)) {
         isStatus = true
       } else {
         isStatus = false
       }
    }
    return isStatus;
}

function selectLinkIf(item: any){
    currentIfOptions = ifOptions;
    if(item.iycyxNo.indexOf('-') == -1) { return; }
    item.iycyxType = item.iycyxNo.split('-')[1]
    if(['E', 'e', 'X', 'x'].includes(item.iycyxNo.split('-')[1])){
        currentIfOptions = [ifOptions[0], ifOptions[1]]
    }
}
</script>

<style lang="scss" src="./css/linkSetting.scss" scoped></style>

<style lang="scss">
.linkSetting-popover{
    padding: 10px 0!important;
    width: auto !important;
    background-color: var(--popper-button-background)!important;
    .el-button-group{
        display: flex;
        flex-direction: column;
        background-color: var(--popper-button-background)!important;
    }
    .el-popper__arrow::before{
        background-color: var(--popper-button-background)!important;
    }
}

.linkSetting-popover .el-button{
   
    display: flex;
    justify-content: start;
    padding-left: 20px;
    min-width: 150px;
    height: 32px;

    line-height: 32px;
    color: var(--popper-button-color);
    border-radius: 6px !important;
}
.linkSetting-popover .el-button:hover{
    background-color: var(--popover-button-background__hover);
    color: white !important;
}
.formDetailDisable{
    form{
        cursor: not-allowed;
        pointer-events:none;
        user-select: none;
    }
}
.controlEquip-add{
    display: flex!important;
    .modalSelect{
        .el-switch{
            padding-bottom: 4px;
        }
    }
}
.orAnd .el-switch__core{
    border-color: var(--switch-core-background__active)!important;
    background-color: var(--switch-core-background__active)!important;
}

.linkSetting{
    .el-dialog__header{
        .el-dialog__title{
            padding: 0;
        }
    }
}
</style>
