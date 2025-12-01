<template>
  <div class="historyEchart">
    <div :id="echartId" v-if="historyValue.length > 0" :key="reflesh"></div>
    <!-- <div v-else class="noData">{{$t('publics.noData')}}</div> -->
    <div
      class="noDataTips"
      :data-noData="$t('publics.noData')"
      v-if="historyValue.length == 0"
    ></div>
  </div>
</template>
<script>
import historyEchart from './historyEchart.js'
export default historyEchart
</script>
<style lang="scss" src="./historyEchart.scss" scoped></style>
