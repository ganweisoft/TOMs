<!--
 * @Description:
 * @Version: v1
 * @Author: zkx
 * @LastEditTime: 2023-10-09 13:37:02
 * Copyright (c) 2020-2025 Shenzhen Ganwei Software Technology Co., Ltd
-->
<template>
    <div class="top-tip" v-if="showDebugInfo">
        <img class="top-img" src="./warning.png">
        {{ $t('login.tips.debugging') }}
    </div>
</template>
<script>
export default {
    data () {
        return {
            showDebugInfo: false
        }
    },
    mounted () {
        this.$api.getServiceStatus().then(res => {
            if (res.data.code == 200 && 'data' in res.data) {
                let data = res.data.data;
                if (data.isDebug) {
                    this.showDebugInfo = true;
                }
            } else {
                this.showDebugInfo = false
            }
        })
            .catch(err => {
                this.showDebugInfo = false
            });
    }
}
</script>
<style lang="scss" scoped>
.top-tip {
    position: absolute;
    top: 65px;
    right: 1px;
    height: 38px;
    color: #FF0024;
    padding: 0 15px;
    background: rgba(43, 19, 9, 0.7);
    border: 1px solid rgba(255, 192, 82, 0.4);
    display: flex;
    align-items: center;
    z-index: 99;

    .top-img {
        width: 22px;
        margin-right: 16px;
    }
}
</style>
