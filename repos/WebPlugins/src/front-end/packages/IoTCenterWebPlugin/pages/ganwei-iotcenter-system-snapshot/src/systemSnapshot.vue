<template>
    <div id="systemSnapshot">
        <p class="title">
            {{ $t('systemSnapshot.headerTitle') }}
            <el-tooltip :content="$t('systemSnapshot.exportSnapshots')" placement="bottom" effect="dark" v-if="showExportSnapshots">
                <i class="iconfont icon-gw-icon-zc-daochu" @click="confirmExport"></i>
            </el-tooltip>
        </p>
        <div class="container" id="container" ref="container">
            <header class="containerHeader" id="containerHeader">
                <el-row :gutter="10">
                    <el-col v-for="(item, index) in tabTopList" :key="index" :xs="12" :sm="8" :md="8" :lg="4" :xl="4">
                        <div @click="onTabTop(index, item)" class="aside_top_nav" :class="{ aside_top_nav_active: item.check }">
                            <div class="aside_top_nav_inner">
                                <el-image :src="item.img">
                                    <div slot="error" class="image-slot">
                                        <img width="40" :src="getDefaultTypeImage(index)" alt />
                                    </div>
                                </el-image>
                                <span class="tab_btn_title">
                                    <p class="title">{{ $t(item.title) }}</p>
                                    <p class="value">
                                        {{ item.value }}
                                        <span>{{ $t('systemSnapshot.slip') }}</span>
                                    </p>
                                </span>
                            </div>
                        </div>
                    </el-col>
                </el-row>
            </header>
            <div class="content" id="content">
                <section id="table">
                    <el-table v-loading="loading" :data="allSnapshoot" :height="tableHeight" style="width: 100%" border>
                        <template slot="empty">
                            <div class="noDataTips" v-if="!loading" :data-noData="$t('systemSnapshot.publics.noData')"></div>
                            <div v-else></div>
                        </template>
                        <el-table-column prop="typeSz" :label="$t('systemSnapshot.mainHeaderTab.type')" min-width="100">
                            <template slot-scope="scope">
                                <span class="typeIco">
                                    <el-image :src="scope.row.typeIcon">
                                        <div slot="error" class="image-slot">
                                            <img width="40" :src="getDefaultTypeImage(scope.row.typeIndex)" alt />
                                        </div>
                                    </el-image>
                                    <!-- <img :src="scope.row.typeIcon" alt="" /> -->
                                    <span>{{ scope.row.typeSz }}</span>
                                </span>
                            </template>
                        </el-table-column>
                        <el-table-column prop="time" :label="$t('systemSnapshot.mainHeaderTab.time')" min-width="180"> </el-table-column>
                        <el-table-column prop="level" :label="$t('systemSnapshot.mainHeaderTab.level')" min-width="180"> </el-table-column>
                        <el-table-column prop="eventMsg" :label="$t('systemSnapshot.mainHeaderTab.details')" min-width="180" show-overflow-tooltip>
                            <template slot-scope="scope">
                                <span v-if="scope.row.eventMsg">
                                    {{ scope.row.eventMsg }}
                                </span>
                                <div class="crossBar" v-else>
                                    <span class="crossBar"></span>
                                </div>
                            </template>
                        </el-table-column>
                        <el-table-column prop="procAdviceMsg" :label="$t('systemSnapshot.mainHeaderTab.suggestion')" min-width="180">
                            <template slot-scope="scope">
                                <span v-if="scope.row.procAdviceMsg">
                                    {{ scope.row.procAdviceMsg }}
                                </span>
                                <div class="crossBar" v-else>
                                    <span class="crossBar"></span>
                                </div>
                            </template>
                        </el-table-column>
                        <el-table-column prop="procAdviceMsg" :label="$t('systemSnapshot.mainHeaderTab.location')" min-width="180">
                            <template slot-scope="scope">
                                <span v-if="scope.row.relatedPic">
                                    <el-popover placement="bottom" trigger="hover">
                                        <el-button @click="goLocation(scope.row, '/topology-gis/#/viewgis')">{{ $t('systemSnapshot.tips.GIS') }}</el-button>
                                        <el-button @click="goLocation(scope.row, '/topology-gis/#/viewgd')">{{ $t('systemSnapshot.tips.Amap') }}</el-button>
                                        <el-button @click="goLocation(scope.row, '/topology/#/view')">{{ $t('systemSnapshot.tips.topology') }}</el-button>
                                        <i class="iconfont icon-dingwei" slot="reference"></i>
                                    </el-popover>
                                </span>
                                <div class="crossBar" v-else>
                                    <span class="crossBar"></span>
                                </div>
                            </template>
                        </el-table-column>
                        <el-table-column prop="planNo" :label="$t('systemSnapshot.mainHeaderTab.plan')" min-width="180">
                            <template slot-scope="scope">
                                <span v-if="scope.row.planNo">
                                    <!-- <i class="iconfont icon-dingwei" slot="reference"></i> -->
                                    <el-button type="text" @click="goPlan(scope.row.planNo)">{{ $t('systemSnapshot.tips.viewPlan') }}</el-button>
                                </span>
                                <div class="crossBar" v-else>
                                    <span class="crossBar"></span>
                                </div>
                            </template>
                        </el-table-column>
                        <el-table-column :label="$t('systemSnapshot.mainHeaderTab.status')" min-width="180">
                            <template slot-scope="scope">
                                <span @click="onTBC(scope.row)" class="comfirStatus">
                                    <div title :class="{ confirmedColor: scope.row.bConfirmed, spanTextColor_gz: !scope.row.bConfirmed, 'en-button-width': languageSet == 'en-US' }">
                                        {{ scope.row.bConfirmed ? $t('systemSnapshot.tips.confirmed') : $t('systemSnapshot.tips.toBeConfirmed') }}
                                    </div>
                                </span>
                            </template>
                        </el-table-column>
                    </el-table>
                </section>
                <footer class="anologueEquipPaging">
                    <el-pagination
                        small
                        background
                        @size-change="handleSizeChange"
                        @current-change="handleCurrentChange"
                        :pager-count="7"
                        :page-sizes="[20, 50, 100]"
                        :page-size="pageSize"
                        layout="sizes, prev, pager, next, jumper,total"
                        :total="total"
                        ref="pagination"
                    >
                    </el-pagination>
                </footer>
            </div>
        </div>

        <el-dialog class="video-main" :visible.sync="videoShow" :close-on-click-modal="false" @close="onCloseVideo" width="800px" height="550px" top="10vh" center>
            <p class="video-title">{{ videoName }}</p>
            <!-- <GWVideoPlaybackCom :GWVideoPlaybackComValue="videoValue" class="primary-icon" @click.native="onBtnClose()"> </GWVideoPlaybackCom> -->
        </el-dialog>

        <el-dialog :title="$t('systemSnapshot.tips.toBeConfirmed') + ': ' + popType" class="confirmed" :visible.sync="confirmedNo" @close="closeEquipDialog" width="840px" top="10vh" center>
            <el-form label-width="100px" class="form" :label-position="$t('systemSnapshot.tips.misdeclara').length >= 8 ? 'top' : 'right'">
                <el-form-item :label="$t('systemSnapshot.time')">
                    <span><i class="el-icon-date" style="margin-right: 5px"></i> {{ popTime }} </span>
                </el-form-item>
                <el-form-item :label="$t('systemSnapshot.event')">
                    <el-input size="small" v-model="popIncident" disabled></el-input>
                </el-form-item>
                <el-form-item :label="$t('systemSnapshot.tips.misdeclara')">
                    <el-switch v-model="radio1" :active-text="$t('systemSnapshot.tips.yes')" :inactive-text="$t('systemSnapshot.tips.no')" active-value="1" inactive-value="0"></el-switch>
                    <!-- <el-radio v-model="radio1" @change="radioVla()" label="1" border size="mini">
                        {{$t('systemSnapshot.tips.yes')}}</el-radio>
                    <el-radio v-model="radio1" @change="radioVla()" label="0" border size="mini">
                        {{$t('systemSnapshot.tips.no')}}</el-radio> -->
                </el-form-item>
                <el-form-item :label="$t('systemSnapshot.mainHeaderTab.suggestion')">
                    <el-input class="textarea" type="textarea" :placeholder="$t('systemSnapshot.messageType.input')" v-model="handlingSuggestion" maxlength="255" show-word-limit> </el-input>
                </el-form-item>
            </el-form>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click="closeEquipDialog">{{ $t('systemSnapshot.publics.button.cancel') }}</el-button>
                <el-button type="primary" :loading="!onAffirmShow" @click="onAffirm">{{ $t('systemSnapshot.publics.button.confirm') }}</el-button>
                <!-- <el-button type="primary"  v-else>{{$t('systemSnapshot.tips.confirming')}}</el-button> -->
            </span>
        </el-dialog>

        <el-dialog
            :title="$t('systemSnapshot.tips.confirmed') + ': ' + popType"
            class="confirmed"
            :visible.sync="confirmedOk"
            @close="closeEquipDialog"
            width="500px"
            top="10vh"
            :close-on-click-modal="false"
            center
        >
            <el-form label-width="100px" class="form" :label-position="$t('systemSnapshot.tips.misdeclara').length >= 8 ? 'top' : 'right'">
                <el-form-item :label="$t('systemSnapshot.time')">
                    <span><i class="el-icon-date" style="margin-right: 5px"></i> {{ popTime }} </span>
                </el-form-item>
                <el-form-item :label="$t('systemSnapshot.event')">
                    <el-input size="small" v-model="popIncident" disabled></el-input>
                </el-form-item>
                <el-form-item :label="$t('systemSnapshot.mainHeaderTab.suggestion')">
                    <el-input disabled class="textarea" type="textarea" v-model="popConfirmremark" maxlength="255" show-word-limit> </el-input>
                </el-form-item>
                <el-form-item :label="$t('systemSnapshot.tips.confirmer')">
                    <span> {{ popAdmin }} </span>
                </el-form-item>
                <el-form-item :label="$t('systemSnapshot.tips.confirmTime')">
                    <span> {{ acknowledgingTime }} </span>
                </el-form-item>
            </el-form>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click="() => (this.confirmedOk = false)">{{ $t('systemSnapshot.publics.button.cancel') }}</el-button>
                <el-button type="primary" :loading="!onAffirmShow" @click="() => (this.confirmedOk = false)">{{ $t('systemSnapshot.publics.button.confirm') }}</el-button>
                <!-- <el-button type="primary"  v-else>{{$t('systemSnapshot.tips.confirming')}}</el-button> -->
            </span>
        </el-dialog>

        <el-drawer title="" :visible.sync="drawer" @closed="closedDrawer" size="80%" direction="btt" :show-close="false">
            <div class="drawer-header" slot="title">
                <i class="el-icon-arrow-left" @click="drawer = false"></i>
                <p>{{ $t('systemSnapshot.mainHeaderTab.type') }} · {{ form.popType }}</p>
                <el-button v-if="!formReadOnly" type="text" @click="onAffirmDraw">{{ $t('systemSnapshot.publics.button.confirm') }} </el-button>
                <el-button v-else type="text"></el-button>
            </div>
            <div class="drawer-content">
                <el-form :model="form" :gutter="16">
                    <el-row>
                        <el-col :span="24">
                            <el-form-item :label="$t('systemSnapshot.mainHeaderTab.time')">
                                <el-input v-model="form.popTime" readonly></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :span="24">
                            <el-form-item :label="$t('systemSnapshot.mainHeaderTab.details')">
                                <el-input v-model="form.popIncident" type="textarea" rows="3" readonly></el-input>
                            </el-form-item>
                        </el-col>
                        <el-col :span="24">
                            <el-form-item :label="$t('systemSnapshot.mainHeaderTab.suggestion')" class="noDisabled">
                                <el-input class="textarea" type="textarea" rows="3" :readonly="formReadOnly" v-model="form.popConfirmremark" maxlength="255" show-word-limit> </el-input>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form>
            </div>
        </el-drawer>

        <el-dialog :title="topologyName" class="confirmed topology" :visible="showTopology" @close="showTopology = false" width="90%" top="5vh" fullscreen :close-on-click-modal="false" center>
            <iframe :src="topology" frameborder="0"></iframe>
        </el-dialog>
    </div>
</template>
<script>
import systemSnapshot from './js/systemSnapshot.js'
export default systemSnapshot
</script>
<style lang="scss" src="./css/systemSnapshot.scss" scoped></style>
