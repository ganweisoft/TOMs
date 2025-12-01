<template>
    <div>
        <template v-if="Array.isArray(content)">
            <template v-for="(item, index) in content">
                <p :style="{ 'text-indent': indent + 'px' }" :level="level" v-if="item.title">{{ item.title }}
                </p>
                <declarationPacts :content="item.content" :level="item.title ? level + 1 : level">
                </declarationPacts>
            </template>
        </template>
        <template v-else>
            <p :style="{ 'text-indent': indent + 'px' }" :level="level">{{ content }}</p>
        </template>
    </div>
</template>

<script>
export default {
    name: 'declarationPacts',
    props: {
        content: {
            type: [Array, String],
            default: () => []
        },
        level: {
            type: Number,
            default: 0
        }
    },
    data () {
        return {
        }
    },
    computed: {
        indent () {
            return 30 + (this.level) * 16;
        }
    },
    mounted () {
    }
}
</script>