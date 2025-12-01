
<template>
    <div id="menu-loading-center">
        <div>
            <div id="loading-center-absolute">
                <div class="object"><img src="./index-loading.png" /></div>
            </div>
            <p class="menu-loading-text">{{ $t('login.tips.loading') }}…</p>
        </div>
    </div>
</template>

<script>
export default {
    data () {
        return {
            text: null,
        };
    },
    methods: {

    }
};
</script>
<style scoped lang="scss">
#menu-loading-center {
    height: auto;
    width: auto;
    position: absolute;
    z-index: 1;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    background: var(--loading-bgColor);
}

#menu-loading-center>div {
    display: inline-block;
    text-align: center;
    transform: scale(0.6)
}

.menu-loading-text {
    margin-top: 10px;
    color: var(--frame-main-color);
}

#loading-center-absolute {
    height: 70px;
    width: 70px;
    display: flex;
    justify-content: center;
    align-items: center;
}

.object {
    position: absolute;
}

.object img {
    width: 100px;
    height: auto;
}
</style>
