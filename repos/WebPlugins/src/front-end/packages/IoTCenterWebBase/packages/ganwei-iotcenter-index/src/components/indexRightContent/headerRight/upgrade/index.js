
export default {
    data () {
        return {
            drawer: false,
            upgradeList: [],
            upgradeLoading: false,
            upgradeBtnLoading: ''
        }
    },
    mounted () {
        this.init();
    },
    methods: { 
        init () {
            let data = {
                "whereCause": "",
                "pageIndex": 1,
                "pageSize": 9999,
                "searchType": 6,
                "searchPluginType": 0,
                "isPublic": true
              }
            this.$api.getUpgradePluginList(data).then(rt => {
                    if (rt?.data?.code == 200) {
                        this.upgradeList = rt?.data?.data?.releaseEntityResponse || []
                    }
                })
                .catch(e => {
                    console.log(e)
                }).finally((r) => {
                    console.log(r)
                })
        },
        upgradePlugin(dt){
            let data;
            if(!dt) {
                this.upgradeLoading = true
                data = this.dataHandle();
            } else {
                this.upgradeBtnLoading = dt?.id
                data = [{
                    "pluginId": dt?.id,
                    "pluginVersionId": dt?.lastAssest?.id
                }]
            }
            this.$api.upgradePlugin(data).then(rt => {
                if (rt?.data?.code == 200) {
                    this.$message.success(rt?.data?.data)
                } else {
                    this.$message.warning(rt?.data?.message)
                }

            })
            .catch(e => {
                console.log(e)
            }).finally((r) => {
                this.upgradeBtnLoading = ''
                this.upgradeLoading = false
                this.init()
                console.log(r)
            })
        },
        dataHandle(){
            let arry = [], data = this.upgradeList
            if(Array.isArray(data) && data.length > 0) {
                data.forEach(item => {
                    arry.push({
                        pluginId: item?.id,
                        pluginVersionId: item?.lastAssest?.id
                    })
                }) 
            }
            return arry
        },
        openElDrawer() {
            this.drawer = true
        },
        formatName(name) {
            return name.slice(0, 1);
        }
    }
}