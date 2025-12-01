<template>
    <div class="operates">
        <el-popover placement="bottom" trigger="hover">
            <el-button-group class="new-button-group setModule">
                <el-button type="text" size="small" class="elBtn" icon=" iconfont icon-tubiao20_bianji"
                    @click.stop="groupEditAndNew(false, source)">
                    {{ $t('equipInfo.poverTips.rename') }}
                </el-button>
                <el-button type="text" size="small" class="elBtn" icon=" iconfont icon-gw-icon-tianjia1"
                    @click="groupEditAndNew(true, source)">
                    {{ $t('equipInfo.poverTips.newChildGroup') }}
                </el-button>
                <el-button type="danger" size="small" @click.stop="deleteGroup(source)"
                    icon=" iconfont icon-tubiao20_shanchu" v-if="source.level != 1">
                    {{ $t('publics.button.deletes') }}
                </el-button>
            </el-button-group>
            <i class="el-icon-more" @click.stop slot="reference"></i>
        </el-popover>
    </div>
</template>

<script>
export default {
    props: {
        source: {
            type: Object,
            default: () => { }
        },
        groupEditAndNew: {
            type: Function,
            default: () => { }
        },
        deleteGroup: {
            type: Function,
            default: () => { }
        }
    }
}
</script>
