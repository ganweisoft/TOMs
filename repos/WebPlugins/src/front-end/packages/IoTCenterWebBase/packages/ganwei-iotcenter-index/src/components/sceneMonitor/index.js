import gwSignalr from '@components/@ganwei-pc/gw-base-components-plus/equipProcessing/gwSignalr.js'
export default {
    data () {
        return {
            monitor: null
        }
    },
    mounted () {
        this.connectMonitor()
    },
    methods: {
        connectMonitor () {
            // eslint-disable-next-line new-cap
            new gwSignalr('/monitor').openConnect().then(res => {
                this.monitor = res
                this.monitor.on('connectionSucceeded', () => {
                    this.monitor.invoke('AddUserToGroup').then().catch(er => {
                        console.log('signalR', er);
                    }).catch(err => console.error(err));
                })
                this.monitor.on('OnOpenWindow', signalrData => {
                    this.signalrMonitorScene(signalrData)
                })
                this.monitor.onclose(() => {
                    this.connectMonitor()
                })
            })
        },
        signalrMonitorScene (signalrData) {
            let pageArr = signalrData.replace(/"/g, '').split('%');
            if (pageArr[0].toUpperCase() === sessionStorage.userName.toUpperCase()) {
                let changePath = '';
                let judgeArr = [];
                if (pageArr[1].includes('clickId')) {
                    let newQuery = this.getQueryClickId(pageArr[1]);
                    changePath = newQuery.newHref;
                    judgeArr = newQuery.newParames;
                } else {
                    changePath = pageArr[1];
                }

                let callback = () => this.$bus.emit('contentFullScreen', false)
                if (changePath.includes('fullscreen=true')) {
                    callback = () => this.$bus.emit('contentFullScreen', true)
                }

                // 当预跳转路径与当前路径相同时不进行二次跳转
                if (changePath) {
                    if (changePath.includes('?')) {
                        changePath = `${changePath}&refreshTab=true`
                    } else {
                        changePath = `${changePath}?refreshTab=true`
                    }
                    this.$bus.emit('openPage', { changePath, callback })
                }
            }
        },
        getQueryClickId (variable) {
            let href = variable;
            let param = href.indexOf('&') !== -1 ? href.split('&') : href.split('?');
            let newHref = '';
            let newParames = [];
            for (let i = 0; i < param.length; i++) {
                let pair;
                if (param[i].indexOf('?') != -1) {
                    pair = param[i].split('?')[1].split('=');
                } else {
                    pair = param[i].split('=');
                }
                if (pair[0] == 'clickId') {
                    window.sessionStorage.setItem('clickId', pair[1]);
                    continue;
                }
                newHref += (i == 0 ? '' : '&') + param[i];
                newParames.push(param[i]);
            }
            return { newHref: newHref, newParames: newParames };
        },
    }
}
