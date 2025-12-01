
export default {
    props: {
        tableDataYx: {
            type: Array,
            default: () => []
        },
        loading:{
            type:Boolean,
            default:false
        },
    },
    inject:['getColor'],
    data() {
        return {
            isShow: ['0','1','2','3','4','5']
        }
    },
    mounted () {
        let _this = this
        this.$bus.off('equipListsIot-yxp')
        this.$bus.on('equipListsIot-yxp', function(arry){
            _this.isShow = arry
        })
    },
    methods: {
        showRealTimeCurve(type, item) {
            this.$emit('showRealTimeCurve', type, item)
        },
        showYcYxList(type, title, ycyxNum, ycyxName, showTimeSelect) {
            this.$emit('showYcYxList', type, title, ycyxNum, ycyxName, showTimeSelect)
        },
        showHistoryCurve(type, item) {
            this.$emit('showHistoryCurve', type, item)
        }
    }
}
