<template>
    <el-popover ref="popover" placement="bottom" trigger="click" popper-class="equipList-table-popover" @show="getPageColumnListByPage" v-if="cities&&cities.length>0">
        <p>{{$t('equipListsIot.label.list')}}</p>
        <div :loading="contentLoading">
        <el-checkbox-group
            v-model="checkedCities"
            :min="1"
            class="equipList-table-group-checkbox"
            >
            <el-checkbox v-for="city in cities" :label="city" :key="city">{{city}}</el-checkbox>
        </el-checkbox-group>
    </div>
        <el-button-group class="new-button-group equipList-table-group-botton" >
            <el-button type="primary" size="small" class="elBtn" v-on:click.stop="allSelect" :loading="resetLoading">
                {{$t('equipListsIot.publics.button.reset')}}
            </el-button>
            <el-button type="primary" size="small" class="elBtn" v-on:click.stop="addPageColumn" :loading="loading">
                {{$t('equipListsIot.publics.button.confirm')}}
            </el-button>
        </el-button-group>
        <i slot="reference" class="el-icon-s-operation" style="font-size: 24px;position: absolute;right: 0;top: 0;cursor: pointer;" ></i>
    </el-popover>
</template>
<style lang="scss">

.equipList-table-group-checkbox{
    display: flex;
    flex-direction: column;
    justify-content: start;
    align-items: self-start;
}
.equipList-table-group-botton{
    display: flex;
    flex-direction: row!important;
    margin-top: 10px;
    // gap: 5px;
    .el-button{
        color: white!important;
        height: 32px;
        line-height: 32px;
        padding: 0 15px !important;
        border-radius: 6px!important;

    }
    .el-button:first-child{
        margin-right: 5px;
    }
}
</style>
<script src="./index.js"></script>
