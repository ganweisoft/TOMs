export default {
  props: {
    showHeader: {
      type: Boolean,
      default: false, //是否需要展示顶部
    },
    headerOperate: {
      type: Boolean, // 是否展示顶部操作按钮
      default: true,
    },
    inputPlaceHolder: {
      type: String,
      required: true, //input框placeHolder
    },
    list: {
      type: Array,
      required: true, //需要展示的数组
    },
    setttingList: {
      type: Array,
      required: true, //每一列中设置,包括列名、对应的属性名、宽度、最小宽度、是否hover显示全  example:[{label:'名字',property:'name',width:100,minWidth:100,showOverflowTooltip:true}]
    },
    multiple: {
      type: Boolean, // 是否多选
      default: true,
    },
    noDataIconfont: {
      // 当没有数据时展示的图标
      type: String,
      default: '',
    },
  },
  data() {
    return {
      renderList: [],
      searchNameStr: '',
      labels: [],
      renderList: [], //数据
      renderLabel: [], //表头
      renderColunm: [],
      deleteList: [],
    }
  },
  watch: {
    searchNameStr: function (val) {
      if (!val) {
        this.getList()
      }
    },
  },
  mounted() {
    let objectArr = Array.from(
      document.getElementsByClassName('el-table__body-wrapper'),
    )
    objectArr.forEach((topItem) => {
      topItem.addEventListener('scroll', function () {
        let arr = Array.from(
          document.getElementsByClassName('el-tooltip__popper'),
        )
        arr.forEach((item) => {
          item.style.display = 'none'
        })
      })
    })
  },

  methods: {
    handleSelectionChange(val) {
      this.deleteList = []
      if (val.length > 0) {
        val.forEach((item) => {
          this.deleteList.push(item.id)
        })
      }
      this.$emit('handleSelectionChange', val)
    },
    showNewDialog() {
      this.$emit('add', {})
    },
    deleteModels() {
      this.$emit('delete', this.deleteList)
    },
    getList() {
      this.$emit('getList', this.searchNameStr)
    },
  },
}
