<template>
    <div class="analogueEquip">
        <div class="anologueEquipHeader">
            <div class="anologueEquipHeaderTop">
                <div class="anologueEquipHeaderTopLeft">
                    <span class="selectEquipNm" :title="selecteItem.equipName" v-if="selecteItem.equipName">{{ selecteItem.equipName }}</span>
                    <span class="equipNumberLabel" v-if="selecteItem.equipNo"> {{ $tt('equip.deviceID') }}： {{ selecteItem.equipNo }} </span>
                </div>
                <div class="anologueEquipHeaderTopRight">
                    <div class="header-right">
                        <el-input
                            size="small"
                            v-model="searchTableEquip"
                            @keyup.enter.native="changeTableList"
                            :placeholder="$tt(searchActiveTabName)"
                            @clear="changeTableList"
                            @input="changeTableList"
                            clearable
                        >
                            <i slot="prefix" class="el-input__icon el-icon-search"></i>
                        </el-input>
                    </div>
                </div>
            </div>
            <div class="anologueEquipHeaderBottom">
                <div class="tab_btn">
                    <span class="tab" @click="tabClick(1)" :class="{ onTabNavClass: activeYcYxSet == 'first' }">
                        <span class="leftBox"></span>
                        <span class="content">
                            {{ $tt(ycName) + ' (' + ycNum + ')' }}
                        </span>
                        <span class="rightBox"></span>
                    </span>
                    <span class="tab" @click="tabClick(2)" :class="{ onTabNavClass: activeYcYxSet == 'second' }">
                        <span class="leftBox"></span>
                        <span class="content">
                            {{ $tt(yxName) + ' (' + yxNum + ')' }}
                        </span>
                        <span class="rightBox"></span>
                    </span>

                    <span class="tab" @click="tabClick(3)" :class="{ onTabNavClass: activeYcYxSet == 'third' }">
                        <span class="leftBox"></span>
                        <span class="content">
                            {{ $tt(setName) + ' (' + setNum + ')' }}
                        </span>
                        <span class="rightBox"></span>
                    </span>
                </div>
                <div class="header-right">
                    <div v-if="selecteItem.equipName">
                        <span class="icon" @click.stop="editEquip">
                            <i class="el-icon-edit-outline"></i>
                            <span>
                                {{ $t('publics.button.edit') }}
                            </span>
                        </span>
                        <span class="icon" v-on:click.stop="deleteEquip">
                            <i class="el-icon-delete"></i>
                            <span>
                                {{ $t('publics.button.deletes') }}
                            </span>
                        </span>
                    </div>

                    <el-button type="primary" size="mini" @click="newYc" v-if="activeYcYxSet == 'first'" icon="el-icon-circle-plus-outline">{{ $tt(ycName) }}</el-button>
                    <el-button type="primary" size="mini" @click="newYx" v-else-if="activeYcYxSet == 'second'" icon="el-icon-circle-plus-outline">{{ $tt(yxName) }}</el-button>
                    <el-button type="primary" size="mini" @click="newSet" v-else-if="activeYcYxSet == 'third'" icon="el-icon-circle-plus-outline">{{ $tt(setName) }}</el-button>
                </div>
            </div>
        </div>

        <div class="anologueEquipContent" id="eqTable">
            <el-table :data="ycTable" v-loading="loading" v-show="activeYcYxSet == 'first'" :height="tableHeight" border>
                <template slot="empty">
                    <div class="noDataTips" :data-noData="$t('publics.noData')" v-if="!loading"></div>
                    <div v-else></div>
                </template>
                <el-table-column :show-overflow-tooltip="true" prop="ycNo" :label="$tt('yc.TelemeteringID')" min-width="100"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="ycNm" :label="$tt('yc.TelemeteringName')" min-width="100"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="valMin" :label="$tt('yc.LowerLimitingValue')" min-width="140"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="restoreMin" :label="$tt('yc.ReplyLowerLimitingValue')" min-width="120"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="restoreMax" :label="$tt('yc.ReplyUpperLimitValue')" min-width="120"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="valMax" :label="$tt('yc.UpperLimitValue')" min-width="120"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" :label="$tt('yc.Unit')" min-width="80">
                    <template slot-scope="scope">
                        <div v-if="scope.row.unit">
                            {{ scope.row.unit }}
                        </div>
                        <div class="crossBar" v-else>
                            <span class="crossBar"></span>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column :show-overflow-tooltip="true" :label="$tt('yc.LinkagePage')" min-width="120">
                    <template slot-scope="scope">
                        <div v-if="scope.row.relatedPic">
                            {{ scope.row.relatedPic }}
                        </div>
                        <div class="crossBar" v-else>
                            <span class="crossBar"></span>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="relatedVideoName" :label="$tt('yc.LinkageVideo')" width="120">
                    <template slot-scope="scope">
                        <div v-if="scope.row.relatedVideoName">
                            {{ scope.row.relatedVideoName }}
                        </div>
                        <div class="crossBar" v-else>
                            <span class="crossBar"></span>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column fixed="right" :label="$tt('table.operation')" width="100">
                    <template slot-scope="scope">
                        <el-tooltip  class="item" effect="dark" :content="$t('publics.button.edit')" placement="top">
                            <span @click="editYc(scope.row)" class="iconfont icon-gw-icon-bianji"></span>
                        </el-tooltip>
                        <el-tooltip class="item" effect="dark" :content="$t('publics.button.deletes')" placement="top">
                            <span @click="deleteYc(scope.row)" class="iconfont icon-gw-icon-shangchu"></span>
                        </el-tooltip>
                    </template>
                </el-table-column>
            </el-table>

            <el-table :data="yxTable" v-loading="loading" v-show="activeYcYxSet == 'second'" :height="tableHeight" style="width: 100%" border>
                <template slot="empty">
                    <div class="noDataTips" :data-noData="$t('publics.noData')" v-if="!loading"></div>
                    <div v-else></div>
                </template>
                <el-table-column :show-overflow-tooltip="true" prop="yxNo" :label="$tt('yx.TeleindicationID')" width="120"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="yxNm" :label="$tt('yx.TeleindicationName')" min-width="100"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="evt01" :label="$tt('yx.zeroToOneEcent')" min-width="120"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="evt10" :label="$tt('yx.oneToZeroEvent')" min-width="120"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" :label="$tt('yx.LinkagePage')" min-width="120">
                    <template slot-scope="scope">
                        <div v-if="scope.row.relatedPic">
                            {{ scope.row.relatedPic }}
                        </div>
                        <div class="crossBar" v-else>
                            <span class="crossBar"></span>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column :show-overflow-tooltip="true" :label="$tt('yx.LinkageVideo')" min-width="120">
                    <template slot-scope="scope">
                        <div v-if="scope.row.relatedVideoName">
                            {{ scope.row.relatedVideoName }}
                        </div>
                        <div class="crossBar" v-else>
                            <span class="crossBar"></span>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column :show-overflow-tooltip="true" :label="$tt('yx.NameOfAsset')" min-width="120">
                    <template slot-scope="scope">
                        <div v-if="scope.row.ziChanId">
                            {{ scope.row.ziChanId }}
                        </div>
                        <div class="crossBar" v-else>
                            <span class="crossBar"></span>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column :show-overflow-tooltip="true" :label="$tt('yx.PlanNumber')" min-width="120">
                    <template slot-scope="scope">
                        <div v-if="scope.row.planNo">
                            {{ scope.row.planNo }}
                        </div>
                        <div class="crossBar" v-else>
                            <span class="crossBar"></span>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column :show-overflow-tooltip="true" :label="$tt('yx.CurveRecord')" min-width="120">
                    <template slot-scope="scope">
                        <span>{{ $tt(scope.row.curveRcdStr) }}</span>
                    </template>
                </el-table-column>
                <el-table-column fixed="right" :label="$tt('table.operation')" width="120">
                    <template slot-scope="scope">
                        <el-tooltip  class="item" effect="dark" :content="$t('publics.button.edit')" placement="top">
                            <span @click="editYx(scope.row)" class="iconfont icon-gw-icon-bianji"></span>
                        </el-tooltip>
                        <el-tooltip class="item" effect="dark" :content="$t('publics.button.deletes')" placement="top">
                            <span @click="deleteYx(scope.row)" class="iconfont icon-gw-icon-shangchu"></span>
                        </el-tooltip>
                    </template>
                </el-table-column>
            </el-table>

            <el-table :data="setTable" v-loading="loading" v-show="activeYcYxSet == 'third'" :height="tableHeight" style="width: 100%" border>
                <template slot="empty">
                    <div class="noDataTips" :data-noData="$t('publics.noData')" v-if="!loading"></div>
                    <div v-else></div>
                </template>
                <el-table-column :show-overflow-tooltip="true" prop="setNo" :label="$tt('set.settingID')" width="120"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="setNm" :label="$tt('set.settingName')" min-width="100"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="setType" :label="$tt('set.typeOfSetting')" min-width="120">
                    <template slot-scope="scope">
                        <div v-if="scope.row.setType">
                            <span>{{ $tt(getSetTypeName(scope.row.setType)) }}</span>
                        </div>
                        <div class="crossBar" v-else>
                            <span class="crossBar"></span>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="mainInstruction" :label="$tt('set.OperatingCommand')" min-width="150"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="minorInstruction" :label="$tt('set.OperatingParameter')" width="100"> </el-table-column>
                <el-table-column :show-overflow-tooltip="true" :label="$tt('set.record')" min-width="100">
                    <template slot-scope="scope">
                        <span>{{ $tt(scope.row.recordStr) }}</span>
                    </template>
                </el-table-column>
                <el-table-column :show-overflow-tooltip="true" :label="$tt('set.action')" min-width="150">
                    <template slot-scope="scope">
                        <div v-if="scope.row.action">
                            {{ scope.row.action }}
                        </div>
                        <div class="crossBar" v-else>
                            <span class="crossBar"></span>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column :show-overflow-tooltip="true" prop="value" :label="$tt('set.value')" min-width="100"> </el-table-column>
                <el-table-column fixed="right" :label="$tt('table.operation')" min-width="120">
                    <template slot-scope="scope">
                        <el-tooltip  class="item" effect="dark" :content="$t('publics.button.edit')" placement="top">
                            <span @click="editSet(scope.row)" class="iconfont icon-gw-icon-bianji"></span>
                        </el-tooltip>
                        <el-tooltip class="item" effect="dark" :content="$t('publics.button.deletes')" placement="top">
                            <span @click="deleteSet(scope.row)" class="iconfont icon-gw-icon-shangchu"></span>
                        </el-tooltip>
                    </template>
                </el-table-column>
            </el-table>
        </div>

        <div class="anologueEquipPaging">
            <el-pagination
                class="mobilePagination"
                background
                @size-change="handleSizeChange"
                @current-change="handleCurrentChange"
                :pager-count="7"
                :page-sizes="[20, 50, 100]"
                :page-size="pagination.pageSize"
                :current-page="pagination.pageNo"
                layout="sizes, prev, pager, next, jumper,total"
                :total="pagination.totalPage"
            >
            </el-pagination>
        </div>

        <!-- 遥测、遥信、设置 -->
        <!-- showEquipDialog(Boolean):弹窗控制字符；closeDialog(Function):关闭弹窗方法; info(Object):弹窗数据显示; isNew(Boolean):是否是新建; saveData(Function):数据保存; saveLoading(Boolean):保存按钮loading;-->
        <yc :showDialog="showYcDialog" :closeDialog="closeDialog" :info="copyObject" :isNew="isYcNew" :saveData="saveYcYxSet" :saveLoading="saveLoading" />
        <yx :showDialog="showYxDialog" :closeDialog="closeDialog" :info="copyObject" :isNew="isYxNew" :saveData="saveYcYxSet" :saveLoading="saveLoading" />
        <set :showDialog="showSetDialog" :closeDialog="closeDialog" :info="copyObject" :isNew="isSetNew" :saveData="saveYcYxSet" :saveLoading="saveLoading" />
    </div>
</template>

<script>
import index from './index.js'
export default index
</script>

<style lang="sass" src="./index.scss"></style>
