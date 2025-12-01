// 模板管理
<template>
    <div id="templateManage">
        <div class="equipList">
            <div class="equipListHeader">
                <div class="equipListTitle">
                    <p>{{ $t('templateManage.title.modelTitle') }}</p>
                    <div class="operateList">
                        <el-tooltip class="item" :content="$t('templateManage.button.importProduct')" placement="top-start">
                            <el-upload class="upload-demo" :on-change="fileListChange" :file-list="uploadForm" :auto-upload="false" accept=".xlsx,.xls">
                                <svg
                                    t="1704939928061"
                                    style="width: 18px; height: 18px"
                                    class="icon"
                                    viewBox="0 0 1024 1024"
                                    version="1.1"
                                    xmlns="http://www.w3.org/2000/svg"
                                    p-id="28134"
                                    width="200"
                                    height="200"
                                >
                                    <path
                                        d="M435.565714 46.299429a32.914286 32.914286 0 0 1 5.924572 65.243428l-5.924572 0.585143H122.660571v786.285714h791.917715V574.610286c0-16.164571 11.702857-29.622857 27.062857-32.402286l5.997714-0.512c16.164571 0 29.622857 11.629714 32.402286 26.989714l0.585143 5.851429v356.644571a32.914286 32.914286 0 0 1-27.062857 32.402286l-5.924572 0.512H89.673143a32.987429 32.987429 0 0 1-32.475429-26.989714l-0.512-5.851429V79.140571c0-16.164571 11.702857-29.622857 27.062857-32.402285l5.924572-0.512h345.892571z"
                                        fill="#8B8F97"
                                        p-id="28135"
                                    ></path>
                                    <path
                                        d="M484.644571 589.677714a33.060571 33.060571 0 0 1-50.761142-41.691428l3.730285-4.534857L923.062857 52.736a33.060571 33.060571 0 0 1 50.761143 41.691429l-3.803429 4.608-485.376 490.642285z"
                                        fill="#8B8F97"
                                        p-id="28136"
                                    ></path>
                                    <path
                                        d="M417.28 641.097143a32.987429 32.987429 0 0 1-32.475429-26.989714l-0.512-5.851429v-305.737143a32.914286 32.914286 0 0 1 65.462858-5.851428l0.512 5.851428v272.822857h273.408c16.237714 0 29.696 11.556571 32.475428 26.916572l0.512 5.924571a32.914286 32.914286 0 0 1-27.062857 32.402286l-5.924571 0.512H417.28z"
                                        fill="#8B8F97"
                                        p-id="28137"
                                    ></path>
                                </svg>
                            </el-upload>
                        </el-tooltip>
                        <el-tooltip class="item" :content="$t('templateManage.button.downProduct')" placement="top-start">
                            <svg
                                t="1704939954741"
                                style="width: 18px; height: 18px"
                                @click="batchExport"
                                class="icon"
                                viewBox="0 0 1024 1024"
                                version="1.1"
                                xmlns="http://www.w3.org/2000/svg"
                                p-id="28270"
                                width="200"
                                height="200"
                            >
                                <path
                                    d="M416.329143 46.299429a32.914286 32.914286 0 0 1 5.851428 65.243428l-5.851428 0.585143H103.424v786.285714h791.917714V574.610286c0-16.164571 11.702857-29.622857 27.062857-32.402286l5.924572-0.512c16.237714 0 29.696 11.629714 32.475428 26.989714l0.585143 5.851429v356.644571a32.914286 32.914286 0 0 1-27.062857 32.402286l-5.997714 0.512H70.436571a32.987429 32.987429 0 0 1-32.475428-26.989714l-0.512-5.851429V79.140571c0-16.164571 11.702857-29.622857 27.062857-32.402285L70.436571 46.299429h345.892572z"
                                    fill="#8B8F97"
                                    p-id="28271"
                                ></path>
                                <path
                                    d="M859.940571 94.354286a33.060571 33.060571 0 0 1 50.761143 41.691428l-3.803428 4.608L421.522286 631.222857a33.060571 33.060571 0 0 1-50.761143-41.691428L374.491429 585.142857l485.376-490.715428z"
                                    fill="#8B8F97"
                                    p-id="28272"
                                ></path>
                                <path
                                    d="M927.305143 43.008c16.164571 0 29.622857 11.629714 32.402286 26.989714l0.585142 5.851429v305.737143a32.914286 32.914286 0 0 1-65.462857 5.851428l-0.585143-5.851428v-272.822857H620.836571a32.987429 32.987429 0 0 1-32.402285-26.989715l-0.585143-5.851428c0-16.164571 11.702857-29.622857 27.062857-32.402286l5.924571-0.512h306.468572z"
                                    fill="#8B8F97"
                                    p-id="28273"
                                ></path>
                            </svg>
                        </el-tooltip>
                        <el-tooltip class="item" :content="$t('templateManage.button.downTemplate')" placement="top-start">
                            <svg t="1704940365977" @click="exportFile" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="28540" width="200" height="200">
                                <path
                                    d="M833.4848 745.472c0 25.0368-14.3872 47.616-37.632 59.0336l-2.8672 1.3824V345.344a27.4944 27.4944 0 0 0-8.192-19.5072L621.312 165.376a27.2384 27.2384 0 0 0-19.0976-7.7824H291.1744l1.5872-2.9696c11.8272-22.1696 34.816-35.9424 59.904-35.9424h392.8064a88.2176 88.2176 0 0 1 88.064 88.0128V745.472z m-139.6736-429.4656v1.9968H634.112a14.336 14.336 0 0 1-14.336-14.1312V240.5376l75.4688 74.0352-1.4336 1.3824z m44.4928 523.0592c0 36.5056-30.464 66.2528-67.8912 66.2528H257.5872c-37.4272 0-67.8912-29.696-67.8912-66.2528V278.528c0-36.5568 30.464-66.2528 67.8912-66.2528H565.0944v91.5456A69.12 69.12 0 0 0 634.1632 372.736h104.1408v466.3808zM745.472 64h-392.704c-28.0064 0-55.3984 9.6256-77.1584 27.136a121.3952 121.3952 0 0 0-42.4448 67.7376l-0.256 1.2288-1.28 0.256a123.4944 123.4944 0 0 0-68.9664 41.7792c-17.8176 21.504-27.648 48.64-27.648 76.3904v560.5376C135.0144 905.728 190.0032 960 257.5872 960h412.8256c28.0576 0 55.4496-9.6256 77.2096-27.136 21.4528-17.2032 36.5056-41.2672 42.3936-67.6864l0.3072-1.28 1.2288-0.256a123.4944 123.4944 0 0 0 69.0176-41.7792c17.8176-21.504 27.648-48.64 27.648-76.3904V206.6944A142.848 142.848 0 0 0 745.5232 64z"
                                    fill="#080203"
                                    p-id="28541"
                                ></path>
                            </svg>
                        </el-tooltip>
                        <el-tooltip class="item" :content="$t('templateManage.button.newProduct')" placement="top-start">
                            <svg viewBox="0 0 20 20" version="1.1" xmlns="http://www.w3.org/2000/svg" @click="newequip" xmlns:xlink="http://www.w3.org/1999/xlink">
                                <g id="页面-1" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                                    <g id="编组-65" fill-rule="nonzero">
                                        <g id="图标" transform="translate(0.400000, 0.400000)">
                                            <path
                                                d="M9.6,1.2 C4.9608081,1.2 1.2,4.9608081 1.2,9.6 C1.2,14.2391919 4.9608081,18 9.6,18 C14.2391919,18 18,14.2391919 18,9.6 C18,4.9608081 14.2391919,1.2 9.6,1.2 Z M9.6,2.4 C13.5764502,2.4 16.8,5.6235498 16.8,9.6 C16.8,13.5764502 13.5764502,16.8 9.6,16.8 C5.6235498,16.8 2.4,13.5764502 2.4,9.6 C2.4,5.6235498 5.6235498,2.4 9.6,2.4 Z"
                                                id="矩形"
                                            ></path>
                                            <path
                                                d="M9.6,5.4 C9.91187845,5.4 10.1681795,5.63795523 10.1972534,5.942216 L10.2,6 L10.199,8.999 L13.2,9 C13.5313708,9 13.8,9.26862915 13.8,9.6 C13.8,9.91187845 13.5620448,10.1681795 13.257784,10.1972534 L13.2,10.2 L10.199,10.199 L10.2,13.2 C10.2,13.5313708 9.93137085,13.8 9.6,13.8 C9.28812155,13.8 9.03182053,13.5620448 9.00274663,13.257784 L9,13.2 L8.999,10.199 L6,10.2 C5.66862915,10.2 5.4,9.93137085 5.4,9.6 C5.4,9.28812155 5.63795523,9.03182053 5.942216,9.00274663 L6,9 L8.999,8.999 L9,6 C9,5.66862915 9.26862915,5.4 9.6,5.4 Z"
                                                id="形状结合"
                                            ></path>
                                        </g>
                                    </g>
                                </g>
                            </svg>
                        </el-tooltip>
                    </div>
                </div>
                <el-input v-model="searchEquip" :placeholder="$t('templateManage.input.searchDeviceTip')" @keyup.enter.native="changeEquipList" clearable>
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
                </el-input>
            </div>

            <div class="equipListMain" id="equipListMain" v-loading="equipLoading">
                <el-container id="equipContainer">
                    <div
                        class="equipListItem"
                        :class="{ 'is-checked': selectEquipNo == item.equipNo, overScroll: scrollWidth }"
                        v-for="(item, index) in nowEquipList"
                        :key="index"
                        @click="select(item)"
                    >
                        <div class="index">{{ index + 1 }}</div>
                        <div class="equipName">
                            <p>{{ item.equipNm }}</p>
                            <div class="operations">
                                <el-tooltip class="item" effect="dark" :content="$t('templateManage.tips.copy')" placement="top">
                                    <i class="iconfont el-icon-copy-document" @click.stop.prevent="copyEquip(item)"></i>
                                </el-tooltip>
                                <el-tooltip class="item" effect="dark" :content="$t('templateManage.publics.button.edit')" placement="top">
                                    <i class="iconfont icon-gw-icon-bianji" @click.stop.prevent="editEquip(item)"></i>
                                </el-tooltip>
                                <el-tooltip class="item" effect="dark" :content="$t('templateManage.publics.button.deletes')" placement="top">
                                    <i class="iconfont icon-gw-icon-shangchu" @click.stop.prevent="deleteEquip(item)"></i>
                                </el-tooltip>
                            </div>
                        </div>
                    </div>
                    <div v-if="!nowEquipList.length && !equipLoading" class="leftNoDataTips">
                        <p class="tip">{{ $t('templateManage.publics.noData') }}</p>
                    </div>
                </el-container>
            </div>

            <div class="equipPaging">
                <el-pagination
                    class="mobilePagination"
                    small
                    background
                    @current-change="equipCurrentChange"
                    :pager-count="5"
                    layout="prev, pager, next"
                    :page-size="pageSizeEquip"
                    :current-page="currentPageEquip"
                    :total="totalPageEquip"
                >
                </el-pagination>
            </div>

            <div class="dragLine"></div>
        </div>
        <div class="analogueEquip">
            <div class="anologueEquipHeader">
                <div class="anologueEquipHeaderTop">
                    <div class="anologueEquipHeaderTopLeft">
                        <div class="left">
                            <el-tooltip :disabled="selectEquipNm.length > 10" :content="selectEquipNm" placement="bottom">
                                <span class="selectEquipNm" v-if="selectEquipNm">
                                    {{ selectEquipNm }}
                                    <label>
                                        {{ $t('templateManage.equip.template') + 'Id: ' + selectEquipNo }}
                                    </label>
                                </span>
                            </el-tooltip>
                        </div>
                    </div>
                    <div class="header-right">
                        <el-input
                            v-show="activeYcYxSet !== 'four'"
                            v-model="searchTableEquip"
                            @keyup.enter.native="changeTableList"
                            :placeholder="$t(searchActiveTabName)"
                            icon="el-icon-circle-plus-outline"
                            clearable
                            @clear="changeTableList"
                            ><i slot="prefix" class="el-input__icon el-icon-search"></i
                        ></el-input>
                    </div>
                </div>
                <div class="anologueEquipHeaderTop">
                    <div class="tab_btn">
                        <span class="tab" @click="tabClick(1)" :class="{ onTabNavClass: activeYcYxSet == 'first' }">
                            <span class="leftBox"></span>
                            <span class="content">
                                {{ $t(ycName) + ' (' + ycNum + ')' }}
                            </span>
                            <span class="rightBox"></span>
                        </span>
                        <span class="tab" @click="tabClick(2)" :class="{ onTabNavClass: activeYcYxSet == 'second' }">
                            <span class="leftBox"></span>
                            <span class="content">
                                {{ $t(yxName) + ' (' + yxNum + ')' }}
                            </span>
                            <span class="rightBox"></span>
                        </span>

                        <span class="tab" @click="tabClick(3)" :class="{ onTabNavClass: activeYcYxSet == 'third' }">
                            <span class="leftBox"></span>
                            <span class="content">
                                {{ $t(setName) + ' (' + setNum + ')' }}
                            </span>
                            <span class="rightBox"></span>
                        </span>
                    </div>

                    <el-button type="primary" size="mini" @click="newYc" v-if="activeYcYxSet == 'first'" icon="el-icon-circle-plus-outline">{{ $t(ycName) }}</el-button>
                    <el-button type="primary" size="mini" @click="newYx" v-else-if="activeYcYxSet == 'second'" icon="el-icon-circle-plus-outline">{{ $t(yxName) }}</el-button>
                    <el-button type="primary" size="mini" @click="newSet" v-else-if="activeYcYxSet == 'third'" icon="el-icon-circle-plus-outline">{{ $t(setName) }}</el-button>
                    <div v-else-if="activeYcYxSet == 'four'"></div>
                </div>
            </div>

            <div class="anologueEquipContent" id="eqTable">
                <el-table v-show="activeYcYxSet == 'first'" :height="tableHeight" :data="ycTable" v-loading="dataLoading" style="width: 100%" border>
                    <template slot="empty">
                        <div class="noDataTips" :data-noData="$t('templateManage.publics.noData')" v-if="!dataLoading"></div>
                        <div v-else></div>
                    </template>
                    <el-table-column :show-overflow-tooltip="true" prop="ycNo" :label="$t('templateManage.yc.TelemeteringID')" width="120"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" prop="ycNm" :label="$t('templateManage.yc.TelemeteringName')" min-width="100"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" prop="valMin" :label="$t('templateManage.yc.LowerLimitingValue')" min-width="100"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" prop="restoreMin" :label="$t('templateManage.yc.ReplyLowerLimitingValue')" min-width="100"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" prop="restoreMax" :label="$t('templateManage.yc.ReplyUpperLimitValue')" min-width="120"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" prop="valMax" :label="$t('templateManage.yc.UpperLimitValue')" min-width="120"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" prop="unit" :label="$t('templateManage.yc.Unit')" min-width="80"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.yc.LinkagePage')" min-width="120">
                        <template slot-scope="scope">
                            <div v-if="scope.row.relatedPic">
                                {{ scope.row.relatedPic }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.yc.LinkageVideo')" min-width="120">
                        <template slot-scope="scope">
                            <div v-if="scope.row.relatedVideoName">
                                {{ scope.row.relatedVideoName }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column fixed="right" :label="$t('templateManage.table.operation')" width="120">
                        <template slot-scope="scope">
                            <el-tooltip class="item" effect="dark" :content="$t('templateManage.tips.copy')" placement="top">
                                <i class="iconfont el-icon-copy-document" @click.stop.prevent="copyYcp(scope.row)"></i>
                            </el-tooltip>
                            <el-tooltip class="item" effect="dark" :content="$t('templateManage.publics.button.edit')" placement="top">
                                <span @click="editYc(scope.row)" class="iconfont icon-gw-icon-bianji"></span>
                            </el-tooltip>
                            <el-tooltip class="item" effect="dark" :content="$t('templateManage.publics.button.deletes')" placement="top">
                                <span @click="deleteYc(scope.row)" class="iconfont icon-gw-icon-shangchu"></span>
                            </el-tooltip>
                        </template>
                    </el-table-column>
                </el-table>

                <el-table v-show="activeYcYxSet == 'second'" :height="tableHeight" :data="yxTable" v-loading="dataLoading" style="width: 100%" border>
                    <template slot="empty">
                        <div class="noDataTips" :data-noData="$t('templateManage.publics.noData')" v-if="!dataLoading"></div>
                        <div v-else></div>
                    </template>
                    <el-table-column :show-overflow-tooltip="true" prop="yxNo" :label="$t('templateManage.yx.TeleindicationID')" width="120"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" prop="yxNm" :label="$t('templateManage.yx.TeleindicationName')" min-width="100"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.yx.zeroToOneEcent')" min-width="120">
                        <template slot-scope="scope">
                            <div v-if="scope.row.evt01">
                                {{ scope.row.evt01 }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.yx.oneToZeroEvent')" min-width="120">
                        <template slot-scope="scope">
                            <div v-if="scope.row.evt10">
                                {{ scope.row.evt10 }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.yx.LinkagePage')" min-width="120">
                        <template slot-scope="scope">
                            <div v-if="scope.row.relatedPic">
                                {{ scope.row.relatedPic }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.yx.LinkageVideo')" min-width="120">
                        <template slot-scope="scope">
                            <div v-if="scope.row.relatedVideoName">
                                {{ scope.row.relatedVideoName }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.yx.NameOfAsset')" min-width="100">
                        <template slot-scope="scope">
                            <div v-if="scope.row.ziChanId">
                                {{ scope.row.ziChanId }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" prop="planNo" :label="$t('templateManage.yx.PlanNumber')" min-width="100">
                        <template slot-scope="scope">
                            <div v-if="scope.row.planNo">
                                {{ scope.row.planNo }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.yx.CurveRecord')" min-width="100">
                        <template slot-scope="scope">
                            <span>{{ $t(scope.row.curveRcdStr) }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column fixed="right" :label="$t('templateManage.table.operation')" width="120">
                        <template slot-scope="scope">
                            <el-tooltip class="item" effect="dark" :content="$t('templateManage.tips.copy')" placement="top">
                                <i class="iconfont el-icon-copy-document" @click.stop.prevent="copyYxp(scope.row)"></i>
                            </el-tooltip>
                            <el-tooltip class="item" effect="dark" :content="$t('templateManage.publics.button.edit')" placement="top">
                                <span @click="editYx(scope.row)" class="iconfont icon-gw-icon-bianji"></span>
                            </el-tooltip>
                            <el-tooltip class="item" effect="dark" :content="$t('templateManage.publics.button.deletes')" placement="top">
                                <span @click="deleteYx(scope.row)" class="iconfont icon-gw-icon-shangchu"></span>
                            </el-tooltip>
                        </template>
                    </el-table-column>
                </el-table>

                <el-table v-show="activeYcYxSet == 'third'" :height="tableHeight" :data="setTable" v-loading="dataLoading" style="width: 100%" border>
                    <template slot="empty">
                        <div class="noDataTips" :data-noData="$t('templateManage.publics.noData')" v-if="!dataLoading"></div>
                        <div v-else></div>
                    </template>
                    <el-table-column :show-overflow-tooltip="true" prop="setNo" :label="$t('templateManage.set.settingID')" width="120"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" prop="setNm" :label="$t('templateManage.set.settingName')" min-width="130"> </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.set.typeOfSetting')" width="130">
                        <template slot-scope="scope">
                            <div v-if="scope.row.setType">
                                {{ $t(getSetTypeName(scope.row.setType)) }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.set.OperatingCommand')" width="200">
                        <template slot-scope="scope">
                            <div v-if="scope.row.mainInstruction">
                                {{ scope.row.mainInstruction }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.set.OperatingParameter')" width="100">
                        <template slot-scope="scope">
                            <div v-if="scope.row.minorInstruction">
                                {{ scope.row.minorInstruction }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.set.record')" width="100">
                        <template slot-scope="scope">
                            <span>{{ $t(scope.row.recordStr) }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.set.action')" width="100">
                        <template slot-scope="scope">
                            <div v-if="scope.row.action">
                                {{ scope.row.action }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column :show-overflow-tooltip="true" :label="$t('templateManage.set.value')" width="100">
                        <template slot-scope="scope">
                            <div v-if="scope.row.value">
                                {{ scope.row.value }}
                            </div>
                            <div class="crossBar" v-else>
                                <span class="crossBar"></span>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column fixed="right" :label="$t('templateManage.table.operation')" width="120">
                        <template slot-scope="scope">
                            <el-tooltip class="item" effect="dark" :content="$t('templateManage.tips.copy')" placement="top">
                                <i class="iconfont el-icon-copy-document" @click.stop.prevent="copySetparm(scope.row)"></i>
                            </el-tooltip>
                            <el-tooltip class="item" effect="dark" :content="$t('templateManage.publics.button.edit')" placement="top">
                                <span @click="editSet(scope.row)" class="iconfont icon-gw-icon-bianji"></span>
                            </el-tooltip>
                            <el-tooltip class="item" effect="dark" :content="$t('templateManage.publics.button.deletes')" placement="top">
                                <span @click="deleteSet(scope.row)" class="iconfont icon-gw-icon-shangchu"></span>
                            </el-tooltip>
                        </template>
                    </el-table-column>
                </el-table>
                <el-tab-pane :label="$t(productAttrName)" v-show="activeYcYxSet == 'four'">
                    <div style="width: 100%; height: calc(98vh - 200px); overflow-y: scroll; display: flex; justify-content: center">
                        <el-form ref="productAttrInfoForm" :model="productAttrInfo" label-width="200px" style="width: 80%">
                            <el-form-item
                                v-for="(item, index) in productAttrInfo.attr"
                                :key="item.key"
                                :label="$t(item.name)"
                                :prop="'attr.' + index + '.value'"
                                :rules="
                                    index === 0
                                        ? {
                                              required: true,
                                              message: $t(item.name) + $t('templateManage.tips.empty'),
                                              trigger: 'blur'
                                          }
                                        : null
                                "
                            >
                                <el-input v-if="index <= 5" v-model="item.value"></el-input>
                                <el-input v-else type="textarea" :rows="5" v-model="item.value"></el-input>
                            </el-form-item>
                        </el-form>
                    </div>
                </el-tab-pane>
            </div>

            <div class="anologueEquipPaging" v-if="activeYcYxSet != 'four'">
                <el-pagination
                    class="mobilePagination"
                    small
                    background
                    @size-change="handleSizeChange"
                    @current-change="handleCurrentChange"
                    :pager-count="7"
                    :page-sizes="[20, 50, 100]"
                    :page-size="pageSize"
                    :current-page="pageNo"
                    layout="sizes, prev, pager, next, jumper,total"
                    :total="totalPage"
                >
                </el-pagination>
            </div>
            <div class="anologueEquipPaging" v-if="activeYcYxSet == 'four'">
                <el-button type="primary" plain @click="cancelProductProperty">{{ $t('templateManage.publics.button.cancel') }}</el-button>
                <el-button type="primary" @click="setProductProperty">{{ $t('templateManage.publics.button.save') }}</el-button>
            </div>
        </div>

        <equipDialog :showDialog="showEquipDialog" :closeDialog="closeDialog" :info="copyObject" :isNew="isEquipNew" :saveData="saveEquip" :saveLoading="saveLoading" page="template" />
        <yc :showDialog="showYcDialog" :closeDialog="closeDialog" :info="copyObject" :isNew="isYcNew" :saveData="saveYcYxSet" :saveLoading="saveLoading" page="template" />
        <yx :showDialog="showYxDialog" :closeDialog="closeDialog" :info="copyObject" :isNew="isYxNew" :saveData="saveYcYxSet" :saveLoading="saveLoading" page="template" />
        <set :showDialog="showSetDialog" :closeDialog="closeDialog" :info="copyObject" :isNew="isSetNew" :saveData="saveYcYxSet" :saveLoading="saveLoading" page="template" />
        <batchExport ref="batchExport"></batchExport>
    </div>
</template>
<script>
import templateManage from './js/templateManage.js'
export default templateManage
</script>
<style lang="scss" src="./css/templateManage.scss" scoped></style>
