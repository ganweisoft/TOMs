<template>
    <el-dialog :title="$t('login.framePro.title.dialogSystemInfoTitle')" class="systemInformation_main"
        v-model="dialogVisible" :before-close="closeDialog" width="auto" top="6vh" center append-to="#index">
        <div class="systemInformation">
            <!-- <header>系统运行信息</header> -->
            <div class="information_box">
                <div v-for="(item, key) in systemInfo" :key="key">
                    <p class="inform_title">{{ item.title }}</p>
                    <div class="diviceLine"></div>
                    <div class="information_box_list flex-between-center">
                        <div class="information_box_list_item " v-for="(childItem, key) in item.childObject" :key="key">
                            <div class="information_box_list_item_box flex-between-center">
                                <div class="box_left" v-if="childItem.icon">
                                    <div class="icon">
                                        <i class="iconfont" :class="childItem.icon"></i>
                                    </div>
                                </div>
                                <div class="box_right">
                                    <div class="label">
                                        <span>{{ childItem.title }}</span>
                                        <el-tooltip effect="dark" popper-class="topToolTip" :content="childItem.toolTip"
                                            placement="top-start">
                                            <el-icon class="question_icon" v-if="childItem.toolTip">
                                                <Warning />
                                            </el-icon>
                                        </el-tooltip>
                                    </div>
                                    <div class="value" :style="{ color: getColor(childItem) }">
                                        {{ `${childItem.value}${childItem.unit || ''}` }}
                                    </div>
                                </div>
                                <div class="propotionEchart" v-if="childItem.showEchart">
                                    <div class="chart" :id="childItem.echartId"></div>
                                </div>
                            </div>
                            <div class="bottomTip" v-if="showBottomTip(childItem)">
                                {{ childItem.bottomTip }}
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </el-dialog>
</template>

<script src="./index.js"></script>

<style src="./index.scss"></style>
