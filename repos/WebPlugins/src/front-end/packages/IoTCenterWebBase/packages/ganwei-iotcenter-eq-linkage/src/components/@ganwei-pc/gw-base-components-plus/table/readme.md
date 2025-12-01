# 用法：

```
import gwTable from 'gw-base-components/gwTable/gwTable'
components:{gwTable}

data(){
    return{
        list:[], //接口中的数据
        pagination:{     //分页
           pageSize:25,  // 分页大小，一页显示多少个
           pageNo:1,     // 当前页
           total:0
        },
        setttingList:[
            {
                label: '名称',       //展示的表头名
                property: 'name',    // 对应的属性名
                width: 200,          //宽度
                showOverflowTooltip: true  //是否hover弹窗显示
            },
            {
                label: '描述',
                property: 'description',
                width: 500
            },
            {
                label: '服务',
                property: 'services',
                showOverflowTooltip: true
            }
        ],
        noDataIconfont:''
    }
}
methods:{
        add(){}
        delete(arr){}
        searchChange(val){
        this.pagination.pageNo=1;
        this.pagination.total=0;
        this.getList(val)
        },
        handleSelectionChange(arr){},
        getList(val){
            // api请求列表,记得加val参数
        }
}

//参数说明
<!-- show-header(bool) //展示顶部，包含搜索框、操作按钮,不传默认false    multiple(bool):是否多选-->

<gwTable
:list='list'
@add='add'               //新增按钮触发事件
@delete='delete'         // 删除触发事件 接收参数：选中的列表数组
@getList='searchChange'  //搜索触发事件 注意处理分页问题
@handleSelectionChange='handleSelectionChange'   //当multiple多选时,选中触发，常用于批量操作，如批量修改
:setttingList="setttingList"    //每一列中设置,包括列名、对应的属性名、宽度、最小宽度、是否hover显示全  example:[{label:'名字',property:'name',width:100,minWidth:100,showOverflowTooltip:true}]
:noDataIconfont='noDataIconfont'   //当表格没有数据时展示的图标
show-header     // 展示顶部 默认false
multiple        // 多选
>

 //支持自定义渲染
 <template v-slot:header></template> //顶部输入框、操作按钮自定义渲染
 <template v-slot:headerSearch></template> //顶部输入框自定义
 <template v-slot:headerOperate></template> //顶部操作按钮自定义
 <template v-slot:table></template> //表格自定义渲染
 <template v-slot:tableOparate></template> //表格自定义操作按钮,默认无


//example:如果表格按钮有操作功能，如查看详情
 <template v-slot:tableOparate>
   <el-table-column label="操作" width="100">
        <template slot-scope="scope">
            <el-button @click="myClick" type="text" size="small">
               查看详情
           </el-button>
       </template>
   </el-table-column>
 </template>

</gwTable>

```
