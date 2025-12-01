<template>
    <div>
        <el-tooltip popper-class="topToolTip" :content="$t('login.framePro.tips.contentFullScreen')" placement="bottom">
            <img id="contentFullScreen" :src="theme.contentFullScreen" alt=""
                @click.stop.prevent="getContentFullscreen(true)" />
        </el-tooltip>
    </div>
</template>

<script src="./index.js"></script>
