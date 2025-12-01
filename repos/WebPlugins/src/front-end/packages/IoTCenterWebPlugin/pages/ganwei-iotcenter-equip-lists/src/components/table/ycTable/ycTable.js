
import preview from '../../valuePreview/index.vue'
export default {
    components: {
        preview
    },
    props: {
        tableDataYc: {
            type: Array,
            default: () => []
        },
        loading: {
            type: Boolean,
            default: false
        },
    },
    inject: ['getColor'],
    data () {
        return {
            isShow: ['0','1','2','3','4','5']
        }
    },
    mounted () {
        let _this = this
        this.$bus.off('equipListsIot-ycp')
        this.$bus.on('equipListsIot-ycp', function (arry) {
            _this.isShow = arry
        })
    },
    methods: {
        // 字符转化DOM连接
        domConversion (str) {
            if (!str) {
                return false
            }
            str = str.toString().toLowerCase()
            if (
                str.indexOf('ftp') != -1 ||
                str.indexOf('http') != -1 ||
                str.indexOf('https') != -1
            ) {
                return true
            }
            return false
        },
        showRealTimeCurve (type, item) {
            this.$emit('showRealTimeCurve', type, item)
        },
        showHistoryCurve (type, item) {
            this.$emit('showHistoryCurve', type, item)
        },
        showYcYxList (type, title, ycyxNum, ycyxName, showTimeSelect) {
            this.$emit('showYcYxList', type, title, ycyxNum, ycyxName, showTimeSelect)
        },

        getRender (value) {
            return value ? value : `<div class="crossBar"><span class="crossBar"></span></div>`
        },
    }
}
