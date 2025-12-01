<template>
  <div class="video-info" v-if="isPlayInfo">
    <p v-for="(item, index) in props.playInfo" class="susscessed">{{item}}</p>
  </div>
</template>
<script  setup lang="ts">
// 基础
const props = defineProps({
    playInfo: Array,
    isPlayInfo: Boolean
})

</script>

<style lang="scss" scoped>
.video-info{
    position: absolute;
    left: 10px;
    bottom: 55px;
    z-index: 2001;
    p{
        opacity: 0.2;
        font-size: 12px;
        padding: 5px 0;
    }
    .susscessed{
        opacity: 0.5;
    }
}
</style>
