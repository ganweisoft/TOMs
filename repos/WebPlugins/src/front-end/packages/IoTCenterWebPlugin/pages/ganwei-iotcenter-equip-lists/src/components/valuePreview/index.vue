<template>
    <span class="preview" :class="{ clickable: clickable }">
        <span @click="showPreview">
            <slot></slot>
        </span>
        <el-dialog custom-class="preview-dialog" :title="data.ycNm" :visible.sync="dialogVisible" v-if="dialogVisible" append-to-body>
            <div class="preview-container">
                <!-- <h265webContainer v-if="isVideo" :protocol="data.code" :url="value" :panelControl="panelControl"></h265webContainer> -->
                <el-image v-if="isImage" :src="value">
                    <div slot="error" class="image-slot">
                        <i class="el-icon-picture-outline"></i>
                    </div>
                </el-image>
            </div>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" @click="dialogVisible = false">关闭</el-button>
            </span>
        </el-dialog>
    </span>
</template>

<script>
export default {

    props: {
        data: {
            type: Object,
            default: () => ({
                code: '',
                value: ''
            })
        },
        value: {
            type: [String, Number],
            default: ''
        }
    },
    data() {
        return {
            isImage: false,
            isVideo: false,
            dialogVisible: false,
            panelControl: {
                beforePlayInfo: true,
                panelControl: true,
                volume: true,
                close: true,
                fullScreen: true
            }
        }
    },
    computed: {
        clickable() {
            return this.isVideo || this.isImage
        }
    },
    created() {
        if (['webrtc', 'rtmp', 'fmp4'].includes(this.data.code)) {
            this.isVideo = true
        }
        const urlParts = String(this.value).toLowerCase().split('.')
        const extension = urlParts[urlParts.length - 1]
        if (['jpg', 'jpeg', 'png', 'gif', 'webp', 'bmp'].includes(extension)) {
            this.isImage = true
        }
    },
    methods: {
        showPreview() {
            if (!this.value) return
            if (this.clickable) this.dialogVisible = true
        }
    }
}
</script>

<style lang="scss" scoped>
.preview {
    &.clickable:hover {
        cursor: pointer;
        text-decoration: underline;
    }
}
</style>

<style lang="scss">
.preview-dialog {
    .preview-container {
        min-height: 500px;
    }
}
</style>
