# 用法

模块 js 中引入
import gwTree from 'gw-base-components/tree'
components:{gwTree}


```
vue 文件
<myTree
ref='myTree'
:data="equipList"
nodeKey="key"           // 节点惟一标志
:props="defaultProps"     //节点参数
@current-change="getItem"     //单个节点点击事件，能通过getItem接收参数（点击的节点信息）:用法：getItem(node){console.log(node)}
default-expand-all         // 是否默认展开所有父节点
:height="100"             // 展示的高度，一般100% 
show-count                //是否显示分组中设备数量
show-status              // 是否展示设备状态
show-checkbox           // 是否展示选框(获取所选择的节点：this.$refs.myTree.getCheckedNodes())
:loading="loading"      //loading效果
:currentNodeKey='currentNodeKey'   // 当前选中行，该字段为节点唯一标志，如设备号,如果是测点，则需要传格式：设备号-测点号，如11134-1
:emptyText='emptyText'       //当无数据时提示语
// 如果是设备管理有操作按钮

></myTree>

```

```
注:equipList(Array)：列表数组
nodeKey(String)：节点唯一 id(绑定的是树状结构每个节点的唯一值，一般为 id)
props(Object)：子节点字段名、节点展示名字段,默认
  defaultProps: {
    children: 'children',
    label: 'title'
  },


```
