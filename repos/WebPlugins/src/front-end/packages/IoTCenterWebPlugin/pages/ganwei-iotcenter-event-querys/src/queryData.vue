<template>
    <div class="queryData">
        <div class="left">
            <header>
                <span>{{ $t('queryData.headerTitle') }}</span>
                <el-button size="small" type="primary" @click="searchData">{{ $t('queryData.publics.button.search') }}</el-button>
            </header>
            <div class="content">
                <div class="elRows">
                    <div class="row">
                        <div class="col title">
                            <span> {{ $t('queryData.tips.selectSystem') }}</span>
                            <el-popover placement="bottom" trigger="hover" popper-class="conditionAnalysePopper">
                                <div class="conditionAnalyse">
                                    <el-row v-for="item in ['like', 'equal', 'in']" :key="item">
                                        <el-col :span="7">{{ $t(`queryData.analysisHeader.${item}`) }}</el-col>
                                        <el-col :span="17"> {{ $t(`queryData.analysisContent.${item}`) }}</el-col>
                                    </el-row>
                                </div>
                                <span slot="reference" class="analysis">
                                    {{ $t('queryData.tips1.parse') }}
                                    <i class="el-icon-question"></i>
                                </span>
                            </el-popover>
                        </div>
                        <div class="col">
                            <el-select v-model="dataBaseSelect" filterable :placeholder="$t('queryData.placeholder')" value-key="label" @change="dataBaseSelectChange">
                                <el-option v-for="(item, index) in dataBaseOptions" :key="index" :label="item.label" :value="item"> </el-option>
                            </el-select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col title">
                            <span>{{ $t('queryData.tips.selectHeader') }}</span>
                        </div>
                        <div class="col">
                            <el-select v-model="baseItemSelect" multiple class="multySelect" filterable value-key="code" @change="baseItemSelectChange" :class="{ optionDisplayNone: noMatch }">
                                <el-option v-for="(item, index) in baseItemOptions" :key="index" :label="item.name" :value="item"> </el-option>
                            </el-select>
                        </div>
                    </div>
                    <div class="row" v-if="baseItemOptions.length > 0">
                        <el-popover :title="$t('queryData.titles.basic')" trigger="click" popper-class="editBaseCheckBox">
                            <el-autocomplete class="inline-input" v-model="popperSearchContent" :fetch-suggestions="popperSearchChange" :placeholder="$t('queryData.placeholder')" :trigger-on-focus="false" @select="handleSelect"></el-autocomplete>

                            <el-checkbox-group v-model="editBaseCheckBox" @change="editBaseItemsChange">
                                <el-checkbox v-for="(item, index) in baseItemOptions" :label="item.name" :key="index" @change="editCheckboxChange($event, item)"></el-checkbox>
                            </el-checkbox-group>
                            <el-divider content-position="center" slot="reference">
                                <i class="el-icon-plus"></i>
                                {{ $t('queryData.tips1.addScreen') }}
                            </el-divider>
                        </el-popover>
                    </div>
                    <div class="queryList">
                        <div class="list">
                            <div class="row" v-for="(item, index) in editBaseItems" :key="index">
                                <div class="col title">
                                    <span>{{ item.name }}</span>
                                    <div class="connection" v-if="typeConnectSelect == 2 && index != 0">
                                        <span>{{ $t('queryData.connection') }}</span>
                                        <el-select filterable v-model="item.link" :placeholder="$t('queryData.placeholder')" @change="forceUpdate">
                                            <el-option v-for="(item, index) in relationOptions" :key="index" :label="item.label" :value="item.value"> </el-option>
                                        </el-select>
                                    </div>
                                </div>
                                <div class="editBox">
                                    <div class="typeBox">
                                        <template>
                                            <el-select filterable v-model="item.typeOption" value-key="value" v-if="item.type == 'string' && item.isNull" @change="typeOptionChange($event, item, index)">
                                                <el-option v-for="(item, indexs) in stringOparateOptions" :key="indexs" :label="item.label" :value="item"> </el-option>
                                            </el-select>
                                            <el-select v-model="item.typeOption" value-key="value" v-if="item.type == 'string' && !item.isNull" @change="typeOptionChange($event, item, index)">
                                                <el-option v-for="(item, indexs) in stringNoNullOparateOptions" :key="indexs" :label="item.label" :value="item"> </el-option>
                                            </el-select>
                                        </template>

                                        <template>
                                            <el-select v-model="item.typeOption" value-key="value" v-if="item.type == 'number' && item.isNull" @change="typeOptionChange($event, item, index)">
                                                <el-option v-for="(item, indexe) in numberOparateOptions" :key="indexe" :label="item.label" :value="item"> </el-option>
                                            </el-select>
                                            <el-select v-model="item.typeOption" value-key="value" v-if="item.type == 'number' && !item.isNull" @change="typeOptionChange($event, item, index)">
                                                <el-option v-for="(item, indexe) in numberNoNullOparateOptions" :key="indexe" :label="item.label" :value="item"> </el-option>
                                            </el-select>
                                        </template>

                                        <template>
                                            <el-select v-model="item.typeOption" value-key="value" v-if="item.type == 'dateTime' && item.isNull" @change="typeOptionChange($event, item, index)">
                                                <el-option v-for="(item, indexr) in dateOparateOptions" :key="indexr" :label="item.label" :value="item"> </el-option>
                                            </el-select>
                                            <el-select v-model="item.typeOption" value-key="value" v-if="item.type == 'dateTime' && !item.isNull" @change="typeOptionChange($event, item, index)">
                                                <el-option v-for="(item, indexr) in dateNoNullOparateOptions" :key="indexr" :label="item.label" :value="item"> </el-option>
                                            </el-select>
                                        </template>

                                        <template>
                                            <el-select v-model="item.typeOption" value-key="value" v-if="item.type == 'bool' && item.isNull" @change="typeOptionChange($event, item, index)">
                                                <el-option v-for="(item, indexr) in boolOparationOptions" :key="indexr" :label="item.label" :value="item"> </el-option>
                                            </el-select>
                                            <el-select v-model="item.typeOption" value-key="value" v-if="item.type == 'bool' && !item.isNull" @change="typeOptionChange($event, item, index)">
                                                <el-option v-for="(item, indexr) in boolNoNullOparationOptions" :key="indexr" :label="item.label" :value="item"> </el-option>
                                            </el-select>
                                        </template>
                                    </div>

                                    <div class="inputBox" v-if="item.typeOption.inputType">
                                        <el-input v-if="item.typeOption.inputType == 'string'" @blur="
                                                () => {
                                                    ;(item.inputValue1 = item.inputValue1.trim()), forceUpdate()
                                                }
                                            " v-model="item.inputValue1" @input="forceUpdate" :placeholder="$t('queryData.placeholder')"></el-input>
                                        <!-- <el-select v-if="item.typeOption.inputType=='multiString'" v-model="item.inputValue1" @change="forceUpdate($event,'multi')" multiple filterable allow-create default-first-option placeholder="请输入内容回车">
                                    </el-select> -->
                                        <div class="mySelect" v-if="item.typeOption.inputType == 'multiString'" @click="focusOnMySelect($event, index)">
                                            <!-- <span v-if="item.tags.length==0" class='tips'>{{$t('queryData.tips1.inputEnter')}}</span> -->
                                            <span class="tag" v-for="(childItem, childIndex) in item.tags" :key="childIndex" :class="{ isExist: existIndex == index.toString() + childIndex }" :num="index.toString() + childIndex">
                                                <span :number="index + childIndex">{{ childItem }}</span>
                                                <span class="deleteChildItem" @click.stop.prevent="deleteChildItem(childIndex, item, index)">×</span></span>
                                            <el-input :ref="'myRefs' + [index]" v-model="temUse[index]" @change="mySelectChange($event, item, index)" @input="mySelectInput($event, index)" :placeholder="item.tags.length == 0 ? $t('queryData.tips1.inputEnter') : ''"></el-input>
                                        </div>
                                        <el-input v-if="item.typeOption.inputType == 'number'" @blur="
                                                () => {
                                                    item.inputValue1 = item.inputValue1.trim()
                                                    item.inputValue1 = item.inputValue1.replace(/[^\d]/g, '')
                                                    forceUpdate()
                                                }
                                            " v-model="item.inputValue1" @input="forceUpdate" :placeholder="$t('queryData.placeholder')"></el-input>

                                        <el-date-picker v-if="item.typeOption.inputType == 'dateTime'" v-model="times[index]" @change="timeSelectChange($event, item, index)" type="datetime" :placeholder="$t('queryData.placeholder')">
                                        </el-date-picker>
                                        <el-date-picker v-if="item.typeOption.inputType == 'dateRange'" @change="dateRangeChange($event, item, index)" v-model="timeRange[index]" type="datetimerange" range-separator="—" :start-placeholder="$t('queryData.date.start')" :end-placeholder="$t('queryData.date.end')">
                                        </el-date-picker>

                                        <el-select v-if="item.typeOption.inputType == 'bool'" v-model="item.inputValue1" @change="forceUpdate" :placeholder="$t('queryData.placeholder')">
                                            <el-option v-for="item in boolOptions" :key="item.value" :label="item.label" :value="item.value"> </el-option>
                                        </el-select>
                                    </div>
                                    <div class="durationInput">
                                        <el-input v-if="item.typeOption.inputType == 'numberRange'" v-model="item.inputValue1" @blur="
                                                () => {
                                                    item.inputValue1 = item.inputValue1.trim()
                                                    item.inputValue1 = item.inputValue1.replace(/[^\d]/g, '')
                                                    forceUpdate()
                                                }
                                            " @input="forceUpdate" :placeholder="$t('queryData.placeholder')"></el-input>
                                        <span v-if="item.typeOption.inputType == 'numberRange'" class="separate">—</span>
                                        <el-input v-if="item.typeOption.inputType == 'numberRange'" v-model="item.inputValue2" @blur="
                                                () => {
                                                    item.inputValue2 = item.inputValue2.trim()
                                                    item.inputValue2 = item.inputValue2.replace(/[^\d]/g, '')
                                                    forceUpdate()
                                                }
                                            " @input="forceUpdate" :placeholder="$t('queryData.placeholder')"></el-input>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row returnTips" v-if="editBaseItems && editBaseItems.length > 1">
                            <div class="col title">{{ $t('queryData.tips1.returnInCondition') }}</div>
                            <div class="col">
                                <el-select v-model="typeConnectSelect" default-first-option @change="forceUpdate">
                                    <el-option v-for="items in typeConnectOptions" :key="items.value" :label="items.label" :value="items.value"> </el-option>
                                </el-select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="right">
            <div class="header">
                <el-button class="noDownload" :loading="exportLoading" type="primary" @click="exportData">{{ $t('queryData.buttons.exportAll') }}</el-button>
            </div>
            <div class="content" v-loading="pageLoading">
                <el-table ref="table" v-if="tableData.length > 0 && tableHeaderTemUse.length < 5" :data="tableData" border style="width: 100%">
                    <el-table-column min-width="55" v-for="(item, index) in tableHeaderTemUse" :label="item.name" :prop="item.code" :key="index"> </el-table-column>
                </el-table>
                <el-table ref="table" v-if="tableData.length > 0 && tableHeaderTemUse.length >= 5" :data="tableData" border tooltip-effect="dark" style="width: 100%">
                    <el-table-column min-width="100" :label="tableHeaderTemUse[0].name" :prop="tableHeaderTemUse[0].code"> </el-table-column>
                    <el-table-column width="300" v-for="(item, index) in temporaryUse" :label="item.name" :prop="item.code" :key="index"> </el-table-column>
                </el-table>
                <div class="noData" v-if="tableData.length == 0">
                    <div class="noDataTips" :data-noData="$t('queryData.publics.noData')"></div>
                </div>
            </div>

            <div class="pagination">
                <el-pagination background @current-change="handleCurrentChange" @size-change="handleCurrentChanges" :page-sizes="[20, 50, 100]" :page-size="pagination.pageSize" layout="sizes, prev, pager, next, jumper,total" :total="pagination.total">
                </el-pagination>
            </div>
        </div>
    </div>
</template>
<script src="./js/queryData.js"></script>
<style lang="scss" src="./css/queryData.scss"></style>
