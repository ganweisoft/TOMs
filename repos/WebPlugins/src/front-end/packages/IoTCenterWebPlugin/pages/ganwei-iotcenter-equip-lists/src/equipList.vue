<template>
    <div id="equipLists" ref="equipLists">
        <aside class="equipList" id="equipListId">
            <div class="equipList_header iconinline">
                <div class="equipList_header_mian">
                    <p>{{ $t('equipListsIot.title.leftHeaderTitle') }} ({{ equipNumber }})</p>
                    <el-popover placement="bottom" trigger="hover" >
                        <el-button-group class="new-button-group setModule buttons">
                            <el-button type="text" size="small" @click="exportEquip"> {{ $t('equipListsIot.button.exportDevice') }}</el-button>
                            <el-button type="text" size="small" @click="exportCurve"> {{ $t('equipListsIot.button.exportCurve') }}</el-button>
                        </el-button-group>
                        <span slot="reference">
                            <i class="iconfont icon-gw-icon-zc-daochu"></i>
                        </span>
                    </el-popover>
                </div>
                <div class="searchEquipSide">
                    <el-input v-model="searchEquipSideCon" clearable :placeholder="$t('equipListsIot.input.inputSearchEquip')" @keyup.enter.native="onSearchSide">
                        <i slot="prefix" class="el-input__icon el-icon-search"></i>
                    </el-input>
                </div>
            </div>
            <div class="equipList_main">
                <myTree ref="mytree" @currentDelete="currentDelete" @statusChange="statusChange" :colorConfig="colorConfig" @node-click="getItem" showStatus @getTotal="getTotal"> </myTree>
            </div>
            <widthSetting leftSide="equipListId" rightSide="equipCon_mainId" :minWidth="370"></widthSetting>
        </aside>

        <section class="equipCon_main" id="equipCon_mainId">
            <nav class="equipCon_nav">
                <div class="equipCon_nav_top">
                    <p class="equipConNm" :title="currentSelect.equipName">
                        <span class="equipListLeftTopName">
                            {{ currentSelect.equipName }}
                        </span>

                        <span v-if="currentSelect.equipName && equipConState != 6" class="yd" :style="{ 'background-color': returnStatus(equipConState) }"></span>
                        <svg
                            v-if="currentSelect.equipName && equipConState == 6"
                            t="1681887306865"
                            class="icon"
                            viewBox="0 0 1024 1024"
                            version="1.1"
                            xmlns="http://www.w3.org/2000/svg"
                            p-id="17559"
                            xmlns:xlink="http://www.w3.org/1999/xlink"
                            width="200"
                            height="200"
                        >
                            <path
                                d="M993.581176 699.873882v75.113412c-0.240941 137.216-215.883294 248.350118-481.701647 248.350118-265.999059 0-481.460706-111.254588-481.761882-248.350118V699.934118c89.871059 98.725647 269.854118 164.743529 481.761882 164.743529s391.710118-66.017882 481.701647-164.743529zM30.117647 479.292235c23.009882 25.178353 52.103529 48.188235 85.895529 68.668236l3.855059 2.048c12.528941 7.408941 25.901176 14.576941 39.755294 21.202823 6.866824 3.312941 14.155294 6.384941 21.323295 9.517177a643.915294 643.915294 0 0 0 53.910588 20.841411c14.215529 4.818824 28.912941 9.035294 43.971764 13.071059 5.240471 1.385412 10.360471 2.891294 15.721412 4.276706 18.672941 4.577882 38.068706 8.553412 57.825883 11.866353a1032.613647 1032.613647 0 0 0 68.909176 8.794353c6.866824 0.602353 13.793882 1.264941 20.781177 1.807059 22.889412 1.686588 46.200471 2.770824 69.933176 2.770823 23.853176 0 46.983529-1.084235 69.872941-2.770823 7.047529-0.421647 13.974588-1.084235 20.841412-1.807059 16.263529-1.505882 32.105412-3.252706 47.826823-5.601882 7.047529-1.144471 14.095059-1.927529 20.961883-3.192471 19.877647-3.312941 39.454118-7.288471 58.187294-11.866353 5.240471-1.204706 10.059294-2.770824 15.179294-4.096 15.058824-4.035765 29.816471-8.312471 44.152471-13.131294 7.710118-2.590118 15.119059-5.421176 22.588235-8.131765 10.782118-3.975529 21.082353-8.252235 31.322353-12.649412 7.168-3.192471 14.456471-6.264471 21.323294-9.517176a537.298824 537.298824 0 0 0 39.755294-21.263059l3.674353-2.048 16.564706-10.36047a356.171294 356.171294 0 0 0 69.270588-58.307765v75.113412c0 29.936941-10.962824 58.428235-29.81647 85.11247a212.992 212.992 0 0 1-15.299765 19.034353l-1.92753 2.048c-20.841412 22.588235-47.887059 43.128471-80.293647 61.31953l-6.625882 3.734588c-6.445176 3.433412-13.251765 6.746353-19.937882 10.059294-3.734588 1.686588-7.348706 3.493647-11.023059 5.12a521.878588 521.878588 0 0 1-33.792 13.914353c-6.384941 2.409412-12.649412 4.818824-19.275294 7.047529-6.384941 2.228706-13.131294 4.156235-19.757177 6.204236a670.418824 670.418824 0 0 1-52.946823 14.215529c-5.12 1.084235-10.480941 2.048-15.841883 3.132235a613.315765 613.315765 0 0 1-42.044235 7.469177c-8.854588 1.385412-17.648941 2.469647-26.624 3.614117l-16.685176 1.807059c-9.938824 0.903529-19.877647 1.626353-29.936942 2.288941-4.818824 0.301176-9.637647 0.722824-14.45647 0.963765a824.018824 824.018824 0 0 1-90.955294 0c-4.879059-0.240941-9.697882-0.662588-14.516706-0.963765-10.059294-0.662588-20.118588-1.385412-29.936941-2.349176-5.662118-0.542118-11.143529-1.084235-16.685177-1.807059-8.975059-1.084235-17.769412-2.288941-26.624-3.614118a613.376 613.376 0 0 1-42.044235-7.408941 644.638118 644.638118 0 0 1-15.841882-3.132235 822.814118 822.814118 0 0 1-53.007059-14.215529c-6.565647-2.048-13.191529-3.975529-19.636706-6.204236-6.625882-2.228706-13.010824-4.698353-19.275294-7.047529a433.573647 433.573647 0 0 1-33.852236-13.914353 540.190118 540.190118 0 0 1-31.021176-15.179294c-2.349176-1.204706-4.397176-2.469647-6.625882-3.734588-32.346353-18.191059-59.392-38.731294-80.233412-61.31953a214.618353 214.618353 0 0 1-15.420235-18.733176l-1.686589-2.409412C41.020235 613.195294 30.117647 584.523294 30.117647 554.586353zM511.879529 0c266.059294 0 481.701647 111.254588 481.701647 248.591059H993.882353v85.534117c0 29.876706-10.842353 58.428235-29.756235 85.052236-0.602353 0.662588-1.144471 1.505882-1.686589 2.288941a212.992 212.992 0 0 1-13.613176 16.685176l-1.987765 2.108236c-20.781176 22.588235-47.826824 43.128471-80.233412 61.319529-2.168471 1.204706-4.216471 2.529882-6.625882 3.734588-6.445176 3.433412-13.251765 6.746353-19.998118 10.059294l-10.842352 5.12c-6.505412 2.891294-13.010824 5.782588-19.757177 8.553412l-14.034823 5.421177c-6.505412 2.409412-12.830118 4.939294-19.456 7.107764-6.324706 2.048-12.950588 4.035765-19.576471 6.02353-6.023529 1.807059-11.866353 3.794824-18.070588 5.421176l-0.662589 0.120471c-1.807059 0.421647-3.855059 0.843294-5.662117 1.385412-9.396706 2.469647-18.913882 4.999529-28.672 7.228235-2.228706 0.361412-4.276706 0.903529-6.324706 1.325176-1.686588 0.421647-3.312941 0.722824-4.818824 0.963765a480.376471 480.376471 0 0 1-30.057411 5.782588c-2.951529 0.421647-5.842824 1.144471-8.734118 1.566118l-8.432941 1.084235c-8.673882 1.204706-17.468235 2.349176-26.322824 3.433412l-10.601411 1.385412-6.625883 0.602353c-9.637647 0.903529-19.395765 1.626353-29.334588 2.288941-3.855059 0.301176-7.469176 0.662588-11.324235 0.843294-1.204706 0.120471-2.349176 0.120471-3.614118 0.120471a975.570824 975.570824 0 0 1-90.413176 0c-1.204706-0.120471-2.349176-0.120471-3.614118-0.120471-3.855059-0.180706-7.408941-0.602353-11.264-0.843294a974.727529 974.727529 0 0 1-29.394824-2.349177l-6.625882-0.542117-10.601412-1.385412a807.152941 807.152941 0 0 1-26.322823-3.433412c-2.770824-0.421647-5.662118-0.722824-8.432941-1.084235a1175.250824 1175.250824 0 0 1-33.912471-6.324706c-1.626353-0.481882-3.312941-0.722824-4.939294-1.024l-4.577883-0.783059c-2.168471-0.421647-4.216471-1.024-6.324705-1.385411-9.818353-2.228706-19.275294-4.698353-28.672-7.228236a67.764706 67.764706 0 0 1-5.662118-1.325176c-0.301176 0-0.421647-0.180706-0.722824-0.180706-6.204235-1.626353-11.986824-3.614118-18.070588-5.360941a405.865412 405.865412 0 0 1-38.972235-13.251765c-4.698353-1.807059-9.517176-3.433412-14.095059-5.360941-6.746353-2.770824-13.251765-5.662118-19.696941-8.553412a540.069647 540.069647 0 0 1-30.900706-15.179294l-6.625882-3.674353c-32.346353-18.251294-59.392-38.791529-80.233412-61.379765A214.618353 214.618353 0 0 1 61.560471 421.647059l-1.686589-2.409412C41.020235 392.854588 30.117647 364.182588 30.117647 334.245647V248.591059C30.117647 111.254588 245.76 0 511.879529 0z"
                                :fill="colorConfig.BackUp"
                                p-id="17560"
                            ></path>
                        </svg>
                        <span class="equipNumberLabel" v-if="currentSelect.equipNo"> {{ $t('equipListsIot.table.listTitleYc.equipNo') }}： {{ currentSelect.equipNo }} </span>
                    </p>
                    <aside class="main_search" :class="{ searchEquipMain_active: inputStyleMain }">
                        <el-input
                            :clearable="true"
                            v-model="mainSearchCon"
                            @blur="
                                () => {
                                    mainSearchCon = mainSearchCon.trim()
                                }
                            "
                            :placeholder="$t(searchActiveTabName)"
                            @keyup.enter.native="onSearchHeader"
                        >
                            <i slot="prefix" class="el-input__icon el-icon-search"></i>
                        </el-input>
                    </aside>
                </div>
            </nav>

            <section class="equipment_list">
                <gw-tab :tabNames="tabNames()" @change="onTabNav" :loading="loading">
                    <!-- 遥测遥信：showYcYxList(Function):列表展示（如遥测数据类型是一元祖时以列表展示）
                                    showRealTimeCurve(Function):展示实时曲线（仅当遥测数据类型是双精度值、遥信数据类型是bool型）
                                    showHistoryCurve(Function):展示历史曲线（仅当遥测数据类型是双精度值、遥信数据类型是bool型）-->
                    <template v-slot:1>
                        <yc-table :tableDataYc="tableDataYc" @showYcYxList="showYcYxList" :loading="loading" @showRealTimeCurve="showRealTimeCurve" @showHistoryCurve="showHistoryCurve"></yc-table>
                    </template>
                    <template v-slot:2>
                        <yx-table :tableDataYx="tableDataYx" @showYcYxList="showYcYxList" :loading="loading" @showRealTimeCurve="showRealTimeCurve" @showHistoryCurve="showHistoryCurve"></yx-table>
                    </template>
                    <template v-slot:3>
                        <!-- 设置:
                                    equipConState(Number):设备状态、tableDataSet(Array)：表格数据-->
                        <set-list :equipConState="equipConState" :loading="loading" :tableDataSet="tableDataSet"> </set-list>
                    </template>
                </gw-tab>
            </section>
            <!-- 分页 -->
            <div class="anologueEquipPaging">
                <el-pagination
                    small
                    background
                    @size-change="handleSizeChangeRight"
                    @current-change="handleCurrentChangeRight"
                    :pager-count="7"
                    :page-sizes="[20, 50, 100]"
                    :page-size="pageSizeRight"
                    layout="sizes, prev, pager, next, jumper,total"
                    :total="totalRight"
                    ref="pagination"
                >
                </el-pagination>
            </div>
        </section>

        <!-- 设备导出 -->
        <exportEquipDialog :showDialog="showExportEquipDialog" @closeDialog="closeDialog" />

        <!-- 历史曲线导出 selectedChange（function）-->
        <exportCurveDialog
            v-if="showExportCurveDialog"
            :equipList="equipList"
            :topTilte="$t('equipListsIot.title.exportCurve')"
            :confirm="toExportCurve"
            :selectedMode="0"
            :showInput="false"
            :showDatePicker="true"
            :confirmButton="$t('equipListsIot.button.confirm')"
            :exportLoading="exportCurveLoading"
        />

        <exportTypeSelectDialog :showDialog="showExportTypeSelectDialog" @closeDialog="closeDialog" @confirm="confirmExportCurve" />

        <!-- 数据以列表的形式展示：
                showTimeSelect（Bool）:展示时间选择器, realTimeData(Array)：实时值，
                currentSelect(Object)：当前选中的设备号、遥测、遥信等详细信息-->
        <yxyxList v-if="showYcyxListDialog" :showTimeSelect="showTimeSelect" :realTimeData="realTimeData" :showDialog="showYcyxListDialog" :currentSelect="currentSelect" :closeDialog="closeDialog" />

        <!-- 实时曲线：
            realTimeData(Array)：实时值，
            currentSelect(Object)：当前选中的设备号、遥测、遥信等详细信息-->
        <realtimeCurveDialog v-if="showRealtimeCurveDialog" :showDialog="showRealtimeCurveDialog" @closeDialog="closeDialog" :currentSelect="currentSelect" :realTimeData="realTimeData" />

        <!-- 历史曲线 -->
        <historyCurveDialog v-if="showHistoryDialog" :theme="theme" :showDialog="showHistoryDialog" :currentSelect="currentSelect" @closeDialog="closeDialog" @showHistoryCurve="showHistoryCurve" />
    </div>
</template>
<script>
import equipList from './js/equipList'
export default equipList
</script>
<style lang="scss" src="./css/equipLists.scss" scoped></style>
