/**
 * @file 设备列表插件
 */

import myTree from '../treeV2'
export default {
    components: {
        myTree
    },
    props: {
        selectedChange: {
            type: Function,
            default: () => { }
        },
        selectedMode: {
            type: Number,
            default: 0
        },
        equipSelected: {
            type: Array,
            default: () => []
        },
        showSetParm: {
            type: Boolean,
            default: false
        }
    },
    data () {
        return {
            equipLoading: false, // 设备加载loading
            equipList: [], // 设备列表
            searchName: '', // 搜索框
            selectedList: [],
            total: 10,
            currentPage: 1,
            pageSize: 5,
            dialogVisible: true,
            noData: this.$t('login.framePro.label.noData'),
            equipNo: -1,
            setParmList: [],
            setParmCurrentPage: 1,
            setParmPageSize: 8,
            setParmTotal: 0,
            setParmCurrentIndex: -1,
            setParmData: {
                setNo: -1,
                setName: ''
            },
            searchEquipName: '',
            disabled: true

        };
    },
    mounted () {

        // 缓存 ，搜索用API请求
        this.equipLoading = true;
    },
    watch: {
        searchName (val) {
            if (!val) {
                this.$refs.treeV2.filterMethod()
                // this.searchEquipName = val
            }
        }

    },
    methods: {
        setParmHandleChange (val) {
            this.setParmCurrentPage = val;
            this.getItem();
        },
        getItem (data) {

            // 当需要选择设备控制项的时候传equipNo
            if (!data.isGroup) {
                this.equipNo = data.key;
                this.selectedList = [
                    {
                        equipNo: data.key,
                        equipName: data.title,
                        staNo: data.staNo
                    }]

            }

        },
        selectedSetParm (item) {
            this.setParmCurrentIndex = item.setNo;
            this.setParmData.setNo = item.setNo;
            this.setParmData.setName = item.setNm;
        },
        sendData () {
            let equipNoList = this.selectedList;
            if (equipNoList.length === 0) {
                this.$message({
                    title: this.$t('login.framePro.tips.selEquip'),
                    type: 'warning'
                });
            } else if (this.showSetParm && this.setParmData.setNo === -1) {
                this.$title({
                    message: this.$t('login.framePro.tips.selControlEquip'),
                    type: 'warning'
                });
            } else {
                this.filterData(equipNoList);
            }
        },
        filterData (data) {
            let Arr = [];
            let ArrList = [];
            data.forEach((item) => {
                if (Arr.indexOf(item.equipNo) === -1) {
                    Arr.push(item.equipNo);
                    ArrList.push(item);
                }
            });
            if (this.showSetParm) {
                this.selectedChange(ArrList, this.setParmData);
            } else {
                this.selectedChange(ArrList);
            }
        },
        handleCurrentChange (val) {
            this.currentPage = val;
            this.searchEquips();
        },
        inputSearch () {
            // if (this.searchName == '')
            // this.onSearch();
        },
        onSearch () {
            this.searchName = this.searchName.trim()
            this.$refs.treeV2.filterMethod(this.searchName)
        },

        // 获取左侧设备列表
        searchEquips () {
            this.equipLoading = true;
            this.$api
                .getGroup()
                .then((rt) => {

                    // 搜索请求结果
                    let str = this.$t('login.framePro.tips.noSearchResult');
                    if (rt.data.message === '无查找结果' || rt.data.message === str) {
                        this.equipList = [];
                        return;
                    }
                    if (rt.data.code === 200) {
                        this.equipList = rt.data.data
                    }
                    this.equipLoading = false;
                })
                .catch((err) => {
                    this.equipLoading = false;
                    console.log('err---', err);
                });

        }
    }
};
