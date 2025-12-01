<template>
    <el-image fit="contain" lazy>
        <template #error>
          <div class="error-slot">
            <el-icon><icon-picture /></el-icon>
          </div>
        </template>
    </el-image>
</template>

<script setup lang="ts">

import { ElImage } from 'element-plus'
import { Picture as IconPicture } from '@element-plus/icons-vue'
    
</script>

<style lang="scss">
.el-image {
    .error-slot {
        color: var(--frame-main-color);
    }
}
</style>