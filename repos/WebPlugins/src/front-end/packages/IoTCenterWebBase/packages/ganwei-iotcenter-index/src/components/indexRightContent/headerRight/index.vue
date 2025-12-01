<template>
    <div class="header-right">
        <fullScreen v-if="config?.showTopBtn?.fullScreen || config?.showTopBtn?.fullScreen === undefined"/>
        <contentFullScreen v-if="config?.showTopBtn?.contentFullScreen || config?.showTopBtn?.contentFullScreen === undefined"/>
        <unReadMessages v-if="config?.showTopBtn?.notify || config?.showTopBtn?.notify === undefined"/>
        <switchTheme v-if="config?.showTopBtn?.themes || config?.showTopBtn?.themes === undefined" />
        <userInfo v-if="config?.showTopBtn?.user || config?.showTopBtn?.user === undefined"/>
    </div>
</template>

<script src="./index.js"></script>
<style src="./index.scss" scoped></style>
