<template>
    <div class="barEchart" v-loading='loading' :key="reflesh">
       <div :id='echartId'>
    </div>
    </div>
</template>
<script>
import barEchart from './barEchart.js';
export default barEchart;
</script>
<style lang="scss" src="./barEchart.scss" scoped></style>