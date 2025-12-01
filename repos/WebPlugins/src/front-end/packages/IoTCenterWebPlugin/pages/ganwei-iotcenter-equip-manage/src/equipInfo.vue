<!-- 设备管理-->
<template>
    <div id="equipInfo">
        <div class="equipList" id="equipListId">
            <div class="equipListHeader">
                <div class="equipListTitle">
                    <p>{{ $t('equipInfo.title.headName') }}({{ equipNumber }})</p>

                    <div class="equipListTitleBtn">
                        <el-tooltip class="item" :content="$t('equipInfo.button.newDevice')" placement="top">
                            <svg viewBox="0 0 20 20" version="1.1" xmlns="http://www.w3.org/2000/svg" @click="addEquips" xmlns:xlink="http://www.w3.org/1999/xlink">
                                <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                                    <g fill-rule="nonzero">
                                        <g transform="translate(0.400000, 0.400000)">
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
                        <!-- 导出 -->
                        <el-tooltip class="item" :content="$t('equipInfo.button.downDevice')" placement="top">
                            <svg viewBox="0 0 20 20" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" @click="exportFile">
                                <g id="页面-1" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                                    <g id="切图备份" transform="translate(-252.000000, -760.000000)">
                                        <g id="编组-68备份" transform="translate(252.000000, 760.000000)">
                                            <g id="编组-36备份-3" transform="translate(2.684138, 2.775000)" fill="#F0F4FF" fill-rule="nonzero">
                                                <path
                                                    d="M5.84085716,0.0500124264 C6.17222801,0.0500124264 6.44085716,0.318641576 6.44085716,0.650012426 C6.44085716,0.948246191 6.22326754,1.19565918 5.93818027,1.24215944 L5.84085716,1.25001243 L1.2,1.25 L1.2,12.994 L13,12.994 L13,8.17695733 C13,7.87872357 13.2175896,7.63131058 13.5026769,7.58481032 L13.6,7.57695733 C13.8982338,7.57695733 14.1456468,7.79454694 14.192147,8.07963421 L14.2,8.17695733 L14.2,13.5942897 C14.2,13.8925235 13.9824104,14.1399365 13.6973231,14.1864367 L13.6,14.1942897 L0.6,14.1942897 C0.301766235,14.1942897 0.054353247,13.9767001 0.00785298705,13.6916128 L0,13.5942897 L0,0.650012426 C0,0.351778661 0.217589612,0.104365673 0.502676883,0.0578654134 L0.6,0.0500124264 L5.84085716,0.0500124264 Z"
                                                    id="路径-13"
                                                ></path>
                                                <path
                                                    d="M12.4918747,0.810739938 C12.7246163,0.574862889 13.1045068,0.572321115 13.3403838,0.805062728 C13.5500523,1.01194416 13.5753571,1.33509137 13.41493,1.57001176 L13.346061,1.65357187 L5.99099305,9.10772549 C5.75825144,9.34360254 5.37836095,9.34614431 5.1424839,9.1134027 C4.93281542,8.90652127 4.9075106,8.58337406 5.06793768,8.34845367 L5.13680669,8.26489356 L12.4918747,0.810739938 Z"
                                                    id="路径-14"
                                                ></path>
                                                <path
                                                    d="M13.5834538,0 C13.8816876,0 14.1291006,0.217589612 14.1756008,0.502676883 L14.1834538,0.6 L14.1834538,5.24285714 C14.1834538,5.57422799 13.9148247,5.84285714 13.5834538,5.84285714 C13.2852201,5.84285714 13.0378071,5.62526753 12.9913068,5.34018026 L12.9834538,5.24285714 L12.983,1.2 L8.94059668,1.2 C8.64236291,1.2 8.39494992,0.982410388 8.34844966,0.697323117 L8.34059668,0.6 C8.34059668,0.301766235 8.55818629,0.054353247 8.84327356,0.00785298705 L8.94059668,0 L13.5834538,0 Z"
                                                    id="路径-17"
                                                ></path>
                                            </g>
                                        </g>
                                    </g>
                                </g>
                            </svg>
                        </el-tooltip>

                        <!-- 移动 -->
                        <el-tooltip class="item" :content="$t('equipInfo.poverTips.moveEquip')" placement="top">
                            <svg @click="moveClick" viewBox="0 0 20 20" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
                                <g id="页面-1" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                                    <g id="编组-14" fill="#F0F4FF" fill-rule="nonzero">
                                        <path
                                            d="M9.89052742,5.59504582 L7.2333723,2.92904582 L2.16952742,2.92904582 C1.7718824,2.92904582 1.44952742,3.2514008 1.44952742,3.64904582 L1.44952742,16.4090458 C1.44952742,16.8066908 1.7718824,17.1290458 2.16952742,17.1290458 L16.9295274,17.1290458 C17.3271724,17.1290458 17.6495274,16.8066908 17.6495274,16.4090458 L17.6495274,6.31542946 L17.6429547,6.21772973 C17.5952787,5.86629719 17.2940354,5.59542946 16.9295274,5.59542946 L9.89052742,5.59504582 Z M2.64952742,4.12904582 L6.73452742,4.12904582 L9.39188141,6.79542946 L16.4495274,6.79504582 L16.4495274,15.9290458 L2.64952742,15.9290458 L2.64952742,4.12904582 Z"
                                            id="路径-5"
                                        ></path>
                                        <g id="编组-13" transform="translate(5.862161, 8.579782)">
                                            <path
                                                d="M5.525,1.00073593 L8.19926407,3.675 C8.43357864,3.90931458 8.43357864,4.28921356 8.19926407,4.52352814 C7.99098445,4.73180776 7.66767517,4.75494994 7.4338334,4.59295468 L7.35073593,4.52352814 L5.52473972,2.69826407 L3.69926407,4.52352814 C3.49098445,4.73180776 3.16767517,4.75494994 2.9338334,4.59295468 L2.85073593,4.52352814 C2.64245631,4.31524852 2.61931413,3.99193923 2.78130939,3.75809747 L2.85073593,3.675 L5.525,1.00073593 Z"
                                                id="路径-37"
                                                transform="translate(5.525000, 2.850000) rotate(90.000000) translate(-5.525000, -2.850000) "
                                            ></path>
                                            <path
                                                d="M3.125,-0.150735931 C3.42323376,-0.150735931 3.67064675,0.0668536803 3.71714701,0.351940952 L3.725,0.449264069 L3.725,5.49926407 C3.725,5.83063492 3.45637085,6.09926407 3.125,6.09926407 C2.82676624,6.09926407 2.57935325,5.88167446 2.53285299,5.59658719 L2.525,5.49926407 L2.525,0.449264069 C2.525,0.117893219 2.79362915,-0.150735931 3.125,-0.150735931 Z"
                                                id="路径-38"
                                                transform="translate(3.125000, 2.974264) rotate(90.000000) translate(-3.125000, -2.974264) "
                                            ></path>
                                        </g>
                                    </g>
                                </g>
                            </svg>
                        </el-tooltip>

                        <!-- 编辑 -->
                        <el-popover placement="bottom" trigger="hover" style="padding: 1px" width="178">
                            <el-button-group class="new-button-group leftMenuMultiple">
                                <el-button type=" text" size="small" @click="batchEditEquip" icon="iconfont icon-gw-icon-shebeixinxilian"> {{ $t('equipInfo.poverTips.deviceParameters') }}</el-button>
                                <el-button type=" text" size="small" @click="batchEditYc" icon="iconfont icon-gw-icon-yaocelian"> {{ $t('equipInfo.tab.yc') }}</el-button>
                                <el-button type=" text" size="small" @click="batchEditYx" icon="iconfont icon-gw-icon-yaoxinlian">
                                    {{ $t('equipInfo.tab.yx') }}
                                    <!-- <i class="iconfont icon-tubiao14_yaoxinliang"></i> -->
                                </el-button>
                                <el-button type=" text" size="small" @click="batchEditSet" icon="iconfont icon-gw-icon-shezhi"> {{ $t('equipInfo.tab.set') }}</el-button>
                                <el-button type="danger text" @click="batchDelEquip" class="del-btn" size="small" style="width: 100%" icon="iconfont icon-gw-icon-shanchu">{{
                                    $t('equipInfo.publics.button.deletes')
                                }}</el-button>
                            </el-button-group>
                            <svg slot="reference" viewBox="0 0 20 20" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
                                <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                                    <g transform="translate(-332.000000, -760.000000)" fill="#F0F4FF" fill-rule="nonzero">
                                        <path
                                            d="M340.524995,762.82886 C340.856366,762.82886 341.124995,763.097489 341.124995,763.42886 C341.124995,763.727093 340.907405,763.974506 340.622318,764.021007 L340.524995,764.02886 L335.884138,764.02886 L335.884138,775.77286 L347.684138,775.77286 L347.684138,770.955804 C347.684138,770.657571 347.901727,770.410158 348.186815,770.363657 L348.284138,770.355804 C348.582372,770.355804 348.829785,770.573394 348.876285,770.858481 L348.884138,770.955804 L348.884138,776.373137 C348.884138,776.671371 348.666548,776.918784 348.381461,776.965284 L348.284138,776.973137 L335.284138,776.973137 C334.985904,776.973137 334.738491,776.755547 334.691991,776.47046 L334.684138,776.373137 L334.684138,763.42886 C334.684138,763.130626 334.901727,762.883213 335.186815,762.836713 L335.284138,762.82886 L340.524995,762.82886 Z"
                                            id="路径-13"
                                        ></path>
                                        <path
                                            d="M347.178842,763.586739 C347.413156,763.352424 347.793055,763.352424 348.02737,763.586739 C348.235649,763.795019 348.258792,764.118328 348.096796,764.35217 L348.02737,764.435267 L339.152108,773.310529 C338.917793,773.544843 338.537894,773.544843 338.30358,773.310529 C338.0953,773.102249 338.072158,772.77894 338.234153,772.545098 L338.30358,772.462001 L347.178842,763.586739 Z"
                                            id="路径-14"
                                        ></path>
                                    </g>
                                </g>
                            </svg>
                        </el-popover>
                    </div>
                </div>
                <el-input v-model="searchEquip" :placeholder="$t('equipInfo.input.searchDeviceTip')" @keyup.enter.native="changeEquipList()" clearable>
                    <i slot="prefix" class="el-input__icon el-icon-search"></i>
                </el-input>
            </div>

            <div class="treeData equipList_main equipListMain">
                <tree
                    ref="myTree"
                    treeType="_manageMent"
                    @getTotal="getTotal"
                    showSelectNum
                    :loading="equipLoading"
                    show-operate
                    nodeKey="key"
                    :needAdd="false"
                    @groupEditAndNew="groupEditAndNew"
                    @deleteGroup="deleteGroup"
                    @node-click="getItem"
                    showCheckbox
                    :showAdd="showAdd"
                >
                </tree>
            </div>

            <div class="dragLine"></div>

            <widthSetting leftSide="equipListId" rightSide="equipCon_mainId" :minWidth="370"></widthSetting>
        </div>

        <!-- selecteItem(Object):选中当前设备信息 -->
        <ycYxSet :selecteItem="selecteItem" ref="YcYxSet" id="equipCon_mainId" />
        <!-- 设备编辑弹窗 -->
        <!-- showEquipDialog(Boolean):弹窗控制字符；closeDialog(Function):关闭弹窗方法; info(Object):弹窗数据显示; isNew(Boolean):是否是新建; saveData(Function):数据保存; saveLoading(Boolean):保存按钮loading; batchEquip(Boolean):是否批量应用 -->
        <equipDialog :showDialog="showEquipDialog" :closeDialog="closeDialog" :info="copyObject" :isNew="isEquipNew" :saveData="saveEquip" :saveLoading="saveLoading" :batchEquip="batchEquip" />
        <!-- 设备移动 -->
        <moveEquip :showDialog="showMoveGroupsDialog" :closeDialog="closeDialog" :saveData="confirmMoveEquip" :saveLoading="saveLoading" />
        <!-- 设备删除 -->
        <deleteEquip :showDialog="showDeleteEquipDialog" :deleteItem="selecteItem" :closeDialog="closeDialog" :deleteEquip="confirmDeleteEquip" :saveLoading="saveLoading" />

        <!-- 右侧管理模块 -->
        <div style="width: calc(100% - 370px)">
            <manage :selecteItem.sync="selecteItem" />
        </div>
        <!-- 模板形式添加设备 -->
        <modelNew :showDialog="showModelNewDialog" @closeDialog="closeDialog('showModelNewDialog')" :groupList="parentSeleceed" />
        <!-- 设备分组管理--增加、编辑 -->
        <group :showDialog="showGroupDialog" :closeDialog="closeDialog" :isGroupNew="isGroupNew" :info="groupData" />
        <!--  设备分组删除-->
        <deleteGroup :showDialog="showDeleteGroup" :closeDialog="closeDialog" :info="deleteGroupInfo" :callback="deleteGroupCallBack" />

        <!-- 批量移动设备 -->
        <batMoveEquip :showDialog="showSelectGroupDialog" :closeDialog="closeDialog" :parentSeleceed="parentSeleceed" :info="moveEquips" />
        <!-- 批量修改设备信息 -->
        <batEditEquip :showDialog="showBatEditEquipDialog" :closeDialog="closeDialog" :saveData="saveData" :saveLoading="saveLoading" />
        <!-- 批量修改遥测 -->
        <batEditYc :showDialog="showBatEditYcDialog" :closeDialog="closeDialog" :saveData="saveData" :saveLoading="saveLoading" />
        <!-- 批量修改遥信 -->
        <batEditYx :showDialog="showBatEditYxDialog" :closeDialog="closeDialog" :saveData="saveData" :saveLoading="saveLoading" />
        <!-- 批量修改设置 -->
        <batEditSet :showDialog="showBatEditSetDialog" :closeDialog="closeDialog" :saveData="saveData" :saveLoading="saveLoading" />
    </div>
</template>
<script>
import equipInfo from './js/equipInfo.js'
export default equipInfo
</script>
<style lang="scss" src="./css/equipInfo.scss" scoped></style>
