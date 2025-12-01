<template>
    <div class="logPreview">
        <div class="logPreviewHeader">
            <div class="headerLeft">
                <p>
                    {{ $t('logPreview.title.headName') }}
                </p>
            </div>
            <div class="headerRight">
                <label>{{ $t('logPreview.logType.title') }}</label>
                <el-select v-model="logType" :placeholder="$t('logPreview.input.pleaseSelect')" @change="selectChange">
                    <el-option v-for="item in logOptions" :key="item.value" :label="$t(item.label)" :value="item.value">
                    </el-option>
                </el-select>
            </div>
        </div>
        <div class="logPreviewMain" v-loading="pageLoading">
            <el-table border v-if="!pageLoading" :data="logDataList" :height="tableHeight"
                style="width: 100%; margin-bottom: 20px" row-key="key"
                :tree-props="{ children: 'childs', hasChildren: 'hasChildren' }">
                <el-table-column :label="$t('logPreview.table.name')" min-width="240">
                    <template slot-scope="scope">
                        <img src="./images/logpreview-folderopen.png"
                            v-show="scope.row.isDirectory && scope.row.expanded" />
                        <img src="./images/logpreview-folderclose.png"
                            v-if="scope.row.isDirectory && !scope.row.expanded" />
                        <img src="./images/logpreview-file.png" v-if="!scope.row.isDirectory" />
                        <span>{{ scope.row.name }}</span>
                    </template>
                </el-table-column>
                <el-table-column :label="$t('logPreview.table.size')" min-width="100">
                    <template slot-scope="scope">
                        <div class="crossBar" v-if="!scope.row.size"><span class="crossBar"></span></div>
                        <span v-else>{{ scope.row.size }}</span>
                    </template>
                </el-table-column>
                <el-table-column prop="modifyTime" :label="$t('logPreview.table.updateTime')" min-width="200">
                </el-table-column>
                <el-table-column :label="$t('logPreview.table.oparate')" min-width="140">
                    <template slot-scope="scope">
                        <div class="crossBar" v-if="scope.row.isDirectory"><span class="crossBar"></span></div>
                        <span v-else class="download">
                            <el-button v-if="!scope.row.isDirectory" icon="el-icon-download" :loading="scope.row.loading"
                                @click="getFile(scope.row)" >{{ $t(down) }}</el-button>
                        </span>
                    </template>
                </el-table-column>
                <template slot="empty">
                    <div class="noDataTips" :data-nodata="$t('logPreview.noData')"></div>
                </template>
            </el-table>
        </div>
        <el-dialog title="提示" :visible.sync="dialogVisible" width="1000px">
            <div ref="logContent" class="logContent"></div>
        </el-dialog>

        <div class="pagination">
            <el-pagination class="mobilePagination" background @size-change="handleSizeChange"
                @current-change="handleCurrentChange" :current-page.sync="pagination.pageNo"
                :page-size="pagination.pageSize" :page-sizes="[20, 50, 100]" layout="sizes,prev, pager, next,total"
                :total="pagination.total">
            </el-pagination>
        </div>
    </div>
</template>

<script src="./js/logPreview.js"></script>
<style lang="scss" src="./css/logPreview.scss" scoped></style>
