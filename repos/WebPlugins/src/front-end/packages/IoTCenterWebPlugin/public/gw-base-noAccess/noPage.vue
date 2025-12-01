<template>
    <div class="noPage noPageTips" :data-noData="$t('login.noAccess.noPage')">
        <div>
            <p>{{msg}}</p>
        </div>
    </div>
</template>
<script>
    export default {
        data () {
            return {
                msg: ''
            }
        },
        mounted () {
            this.msg = this.$router.currentRoute.query.tips;
        }
    }
</script>
<style scoped>
    .noPage {
        width: 100%;
        height: 100%;
        display: flex;
        justify-content: center;
        color: white;
    }

    .noPageTips>div {
        width: 380px;
        height: 380px;
        background-size: 100% 100%;
        background-repeat: no-repeat;
        position: absolute;
        left: 50%;
        top: 40%;
        transform: translate(-50%, -50%);
        background-image: url(./Image/frame-nopage.png);
    }

    .noPageTips>div p {
        font-size: 14px;
        position: absolute;
        width: 100%;
        bottom: -24px;
        left: 0;
        text-align: center;
        margin-left: 10px;
    }

    [data-theme="dark"] .noPageTips>div {
        background-image: url(./Image/frame-nopage.png);
    }

    [data-theme="dark"] .noPageTips>div p {
        color: #818EA9;
    }

    [data-theme="light"] .noPageTips>div {
        background-image: url(./Image/frame-nopage-light.png);
    }

    [data-theme="light"] .noPageTips>div p {
        color: #818EA9;
    }
</style>