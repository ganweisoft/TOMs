<template>
    <el-container id="index">
        <div class="topIndexHeader"></div>
        <!-- 左侧区域 -->
        <div :class="{ isCollapse: isCollapse, noIsCollapse: !isCollapse, transition: transition }">
            <!-- 全量菜单 -->
            <div class="maxW maxActive" :class="{ transition: transition }" id="menuRef">
                <div class="aside-header">
                    <div class="aside-header-box">
                        <a style="cursor: pointer">
                            <img :src="theme.logo" class="header-top-img" alt="" />
                        </a>
                    </div>
                </div>
                <div class="left-nav" @click.stop>
                    <el-row class="list">
                        <div class="max">
                            <el-menu ref="menu" :router="true" :default-active="menuActive" @select="onRouters"
                                :collapse="false" :collapse-transition="false" unique-opened>
                                <asideMenu @func="getName" v-for="item in menu" :data="item" :key="item.resourceId">
                                </asideMenu>
                            </el-menu>
                        </div>
                    </el-row>
                    <div class="fold">
                        <div @click.stop.prevent="onAsideListShow()" :class="menuOverflow">
                            <el-button>
                                <i
                                    :class="isCollapse ? 'iconfont icon-caidan_zhankai cacelmargin' : 'iconfont icon-caidan_zhankai isopen'"></i>
                            </el-button>
                            <span>收缩</span>
                        </div>
                    </div>
                </div>

                <widthSetting custom @resizeEnd="resizeEnd" v-if="!isCollapse" leftSide="menuRef"
                    rightSide="contentRef"> </widthSetting>
            </div>

            <!-- 缩放菜单 -->
            <div class="maxW minActive" :class="{ transition: transition }">
                <div class="topIndexHeader"></div>
                <div class="minActiveContent">
                    <div class="aside-header">
                        <div class="aside-header-box">
                            <a style="cursor: pointer">
                                <img class="min-img" :src="config.img.indexSmallImg" alt />
                            </a>
                        </div>
                    </div>
                    <div class="left-nav" @click.stop>
                        <!-- 新版导航菜单 -->
                        <el-row class="list">
                            <div class="max">
                                <el-menu ref="menu" :router="true" :default-active="menuActive" @select="onRouters"
                                    :collapse="true" :collapse-transition="false" unique-opened>
                                    <asideMenu :isCollapse="isCollapse" @func="getName" v-for="item in menu"
                                        :data="item" :key="item.resourceId"> </asideMenu>
                                </el-menu>
                            </div>
                        </el-row>
                        <div class="fold">
                            <div @click.stop.prevent="onAsideListShow()" :class="menuOverflow">
                                <el-button>
                                    <i
                                        :class="isCollapse ? 'iconfont icon-caidan_zhankai cacelmargin' : 'iconfont icon-caidan_zhankai isopen'"></i>
                                </el-button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 右侧区域 -->
        <el-container id="contentRef">
            <!-- 顶部内容块 -->
            <el-header class="indexHeader">
                <div class="header-right displayNone">
                    <div>
                        <img v-if="n % 2 == 0" :src="theme.fullScreen" alt="" @click.stop.prevent="getFullCreeen()" />
                        <img v-else :src="theme.outFullScreeen" @click.stop.prevent="getFullCreeen()" alt="" />
                    </div>

                    <div>
                        <img :src="theme.contentFullScreen" alt="" @click.stop.prevent="getContentFullscreen()" />
                    </div>

                    <el-dropdown class="index-header-right">
                        <el-avatar :src="theme.user" shape="square"></el-avatar>
                        <i class="iconfont icon-xialasanjiao"></i>
                        <el-dropdown-menu style="padding: 10px" slot="dropdown">
                            <div class="user">
                                <el-avatar :src="theme.user" shape="square"></el-avatar>
                                <span>{{ loginUsername }}</span>
                            </div>
                            <p @click.stop.prevent="modal2 = true" type="default" style="cursor: pointer">
                                <i class="iconfont icon-tuichudenglu"></i>
                                {{ $t('login.framePro.button.exitLogin') }}
                            </p>
                        </el-dropdown-menu>
                    </el-dropdown>
                </div>
            </el-header>

            <!-- 底部内容块 -->
            <template v-if="getConfig">
                <div class="breadcrumb" v-if="!config.labelPage">
                    <el-breadcrumb separator-class="el-icon-arrow-right">
                        <el-breadcrumb-item>{{ $t('login.framePro.label.home') }}</el-breadcrumb-item>
                        <el-breadcrumb-item v-for="(item, i) in breadcrumbList" :key="i" :to="{ path: item.item }">{{
                            $t(item.title) }}</el-breadcrumb-item>
                    </el-breadcrumb>
                </div>

                <el-main class="container-main" v-if="!config.labelPage">
                    <router-view :key="$route.fullPath"></router-view>
                </el-main>

                <el-main class="container-main" v-if="config.labelPage">
                    <router-view :key="$route.fullPath" v-if="isError"></router-view>
                    <el-tabs @contextmenu.prevent.native="openContextMenu($event)" v-model="editableTabsValue"
                        type="card" closable @tab-remove="removeTab" @tab-click="tabClick" v-else>
                        <el-tab-pane v-for="(item, index) in editableTabs" :key="item.name" :label="item.title"
                            :disabled="item.disabled" :name="item.name" :route="item.route" :data-va="index">
                            <template slot="label">
                                <span class="el-icon-refresh-right" v-if="!item.disabled" @click="refreshTab(index)"
                                    :class="{ active: isActive }"></span>
                                <span v-else class="el-icon-loading"></span>
                                <span>{{ $t(item.title) }}</span>
                            </template>

                            <router-view :key="item.route" :editableTabs="editableTabs"
                                :editableTabsValue="editableTabsValue"></router-view>
                        </el-tab-pane>
                    </el-tabs>s
                </el-main>
            </template>
        </el-container>

        <!-- 系统运行信息 -->
        <el-dialog :title="$t('login.framePro.title.dialogSystemInfoTitle')" class="systemInformation_main"
            :visible.sync="curveShowCurve" @close="onCloseCurve" width="600px" top="30vh" center>
            <div class="systemInformation">
                <!-- <header>系统运行信息</header> -->
                <div class="information_box">
                    <div v-for="(item, index) in infoList" :key="index">
                        <p class="inform_title">{{ item.title }}</p>
                        <div class="inform_msg">
                            <p class="inform_link" v-for="(items, indexs) in item.value" :key="indexs">
                                <span class="inform_nm">{{ items.Key }}</span>
                                <span :title="items.Value" class="inform_value">{{ items.Value }}</span>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </el-dialog>

        <!-- 密码编辑 -->
        <el-dialog class="editPassword" @close="inspectState" @closed="clearInput" :visible.sync="modal1" width="480px"
            top="28vh" :close-on-click-modal="false" :show-close="false" center>
            <div slot="title">
                <span class="el-dialog__title" style="line-height: 24px; font-size: 18px">{{
                    $t('login.framePro.title.dialogChangePasswordTitle') }}</span>
                <i style="position: absolute; top: 20px; right: 20px; line-height: 20px; font-size: 14px; color: #909399; cursor: pointer"
                    class="iconfont iconguanbi .el-dialog__headerbtn" @click.stop.prevent="modal1 = false"></i>
            </div>
            <el-form :model="changePwd" label-position="left"
                :label-width="$t('login.framePro.title.dialogChangePasswordTitle').length > 5 ? '140px' : '80px'"
                :rules="rules" ref="changePasswordForm">
                <el-form-item :label="$t('login.framePro.label.originalPassword')" prop="oldPwd">
                    <el-input v-model="changePwd.oldPwd" v-if="modal1" show-password></el-input>
                </el-form-item>
                <el-form-item :label="$t('login.framePro.label.newPassword')" prop="newPwd">
                    <el-input v-model="changePwd.newPwd" v-if="modal1" show-password @change="pwdInputChange">
                    </el-input>
                </el-form-item>
                <el-form-item :label="$t('login.framePro.label.confirmPassword')" prop="confPwd">
                    <el-input v-model="changePwd.confPwd" v-if="modal1" show-password></el-input>
                </el-form-item>
            </el-form>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click.stop.prevent="modal1 = false">{{
                    $t('login.publics.button.cancel') }}</el-button>
                <el-button type="primary" @click.stop.prevent="changeCode()">{{ $t('login.publics.button.confirm') }}
                </el-button>
            </span>
        </el-dialog>

        <!-- 退出 -->
        <el-dialog id="quit" :title="$t('login.framePro.tips.sureToLogOut')" :visible.sync="modal2" width="350px"
            top="35vh" center>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" plain @click.stop.prevent="modal2 = false">{{
                    $t('login.publics.button.cancel') }}</el-button>
                <el-button @click.stop.prevent="quitLogin" type="danger" style="color: #ffffff"> {{
                    $t('login.publics.button.confirm') }}</el-button>
            </span>
        </el-dialog>
    </el-container>
</template>

<script>
    const asideMenu = () => import('gw-base-components-plus/asideMenu/menu.vue')
    import ContextMenu from 'gw-base-components-plus/contextMenu'
    import topNav from 'gw-base-components-plus/asideMenu/topNav.vue'

    import unread from 'gw-base-components-plus/unreadMsg/unread.vue'
    import keyEvent from 'gw-base-utils-plus/keyEvent'
    // import frameLanguage from 'gw-base-utils-plus/frameLanguage'
    import exportHistory from 'gw-base-utils-plus/historyExport'
    import screenfull from 'screenfull'
    import widthSetting from 'gw-base-components-plus/widthSetting/widthSetting.vue'
    import Speaktts from 'gw-base-utils-plus/speaktts.js'
    import getMenuesList from "./menu"
    // import equipSignlr from 'gw-base-components-plus/treev2/mixin/signlrLoadEquip.js'
    // import equipCache from 'gw-base-components-plus/equipProcessing/gwEquipCache.js';
    // equipSignlr
    export default {
        mixins: [{ methods: keyEvent.methods }, exportHistory, Speaktts],
        components: {
            asideMenu,
            topNav,
            widthSetting,
            unread
        },
        data () {
            return {
                showSound: false,
                speakttsRange: '',
                speakttsArray: [],
                speakttsIs: true,
                speakttsTime: null,
                speakttsText: true,
                noFirst: true,
                getConfig: false,
                navTopActive: 0,
                navTopOpt: [],
                routesMap: {},
                routesToName: {},
                menuOverflow: '',
                langOptions: window.localStorage.langOptions ? JSON.parse(window.localStorage.langOptions) : [],
                config: {
                    showSwitchTheme: false,
                    img: {
                        indexSmallImg: ''
                    },
                    theme: {
                        default: '',
                        supportThemes: []
                    }
                },
                theme: '',
                currentDate: '2021-06-01',
                menu: [],
                allMenu: [],
                openeds: [],
                historicalList: [],
                isCollapseText: true,
                dialogVisible: false,

                defaultProps: {
                    children: 'children',
                    label: 'text'
                },

                changePwd: {
                    oldPwd: '',
                    newPwd: '',
                    confPwd: ''
                },

                modal1: false,
                modal2: false,

                // 全屏/不全屏
                n: 0,
                childIndex: 0,

                // 导航显示隐藏
                asideListShow: true,

                // 系统信息显示隐藏
                curveShowCurve: false,
                infoList: [
                    {
                        title: '系统运行平台',
                        value: [
                            {
                                Key: '网关服务版本号',
                                Value: 'x.x.x'
                            },
                            {
                                Key: 'Web程序版本号',
                                Value: 'x.x.x'
                            }
                        ]
                    }
                ],

                // 登录者姓名
                loginUsername: window.sessionStorage.userName,
                loginUn: '',

                // 登陆者权限模块
                loginRoleList: [],

                isCollapse: false,

                maxWActive: 'maxW-active',

                notifyPromise: Promise.resolve(),

                // 消息提醒定时器
                intevalObj: null,

                // 是否初次调用消息提醒
                isFirstMsg: true,

                // 是否有实时快照权限
                iaHaveSnapshot: false,

                // 面包屑导航菜单
                breadcrumbList: [], // 面包屑数据
                menuActive: '', // 导航菜单高亮

                // 该参数用于保证请求完iam的token后才加载子路由
                isRouter: false,
                flagl: false,

                // 修改密码校验规则
                regPwdLength: /./,
                regPwd1: /./,
                regPwd2: new RegExp('[\\u4E00-\\u9FFF]+', 'g'),
                regPwd3: /^[\S]*$/,

                regPwdLengthMsg: '',
                regPwdMsg: '',
                pwdHaveName: undefined,
                minCharacters: 0,
                pwdCharacters: 0,
                pwdMinCharactersMsg: undefined,
                delayTime: 5000,
                exportSignalrConnection: null,
                noPermission: false,

                // 标签页数组和当前选中值
                editableTabsValue: '',
                editableTabs: [],

                // 接口是否报错
                isError: false,

                // 刷新按钮状态
                isActive: false,

                // 全屏参数
                isEnableFullScreen: false,
                FullScreenHome: [],
                contentIsFullScreen: false,
                overdue: false,

                // 是否使用标签页
                // isTab: false,

                // 新消息数量

                totalMessage: 0,
                messageList: [],

                firstMenuItem: {
                    name: '',
                    route: ''
                },

                packageId: '',
                showLangSelect: false,
                languageSelected: sessionStorage.languageType || '中文',

                // 安全模式2023-02-23
                safeMode: 1,
                safeTipsTime: 10000,
                safeTipsText: 'login.tips.safeLevel.high',
                transition: false,
                eventType: {},
                snapshotColorClass: ['xinxi', 'shezhi', 'zichan', 'guzhang', 'jinggao'],
            }
        },

        computed: {
            rules () {
                return {
                    oldPwd: [
                        {
                            type: 'string',
                            required: true,
                            validator: (rule, value, callback) => {
                                if (!value) {
                                    callback(new Error(this.$t('login.framePro.tips.enterOriginPas')))
                                }
                                callback()
                            },

                            // message: this.$t('login.framePro.tips.enterOriginPas'),
                            trigger: 'blur'
                        }
                    ],
                    newPwd: [
                        {
                            validator: this.validatePass,
                            required: true,
                            trigger: 'blur'
                        }
                    ],
                    confPwd: [
                        {
                            validator: this.validateConfPass,
                            required: true,
                            trigger: 'blur'
                        }
                    ]
                }
            }
        },

        created () {
            this.showSound = sessionStorage.showSound || false
            this.myUtils
                .configInfoData(this)
                .then(data => {
                    this.config = data
                    document.title = data.titleConfig.platName
                    this.speakttsRange = data.speaktts || ''
                    if (data.img.platLogo) {
                        let link = document.createElement('link')
                        link.setAttribute('rel', 'icon')
                        link.href = data.img.platLogo
                        link.setAttribute('type', 'image/x-icon')
                        window.document.head.prepend(link)
                    }

                    // tab标签
                    sessionStorage.isTab = data.labelPage

                    // 主题
                    if (!localStorage.theme) {
                        localStorage.setItem('theme', this.config.theme.default)
                    }
                    window.document.documentElement.setAttribute('data-theme', localStorage.theme)
                    this.theme = JSON.parse(JSON.stringify(this.config.theme.supportThemes.find(item => item.value === localStorage.theme)))

                    // 中英文
                    this.showLangSelect = data.showLangSelect
                    if (!this.showLangSelect || !window.sessionStorage.languageType) {
                        window.sessionStorage.languageType = 'zh-CN'
                    }
                })
                .catch(err => {
                    console.log(err)
                })
                .finally(() => {
                    this.getConfig = true
                })

            // 初始化选中菜单
            if (sessionStorage.menuActive) {
                this.menuActive = sessionStorage.menuActive
            }
            this.init()
            this.getUserInfo()

        },
        mounted () {
            this.getRealTimeEventTypeConfig()

            if (this.$route.path.indexOf('/systemSnapshot') == -1) {
                this.intevalObj = setTimeout(() => {
                    this.getRealTimeDataInfo()
                }, this.delayTime)
            }

            // this.setSafeMode()
            if (!this.config.labelPage) {
                if (localStorage.breadcrumbList) {
                    this.breadcrumbList = JSON.parse(localStorage.breadcrumbList)
                }
            }
            if (localStorage.historicalList) {
                this.historicalList = JSON.parse(localStorage.historicalList)
            }

            // document.title = this.$t('login.documentTitle');
            if (window.sessionStorage.passwordPolicy && window.sessionStorage.passwordPolicy == 1) {
                this.modal1 = true
            }

            // window.document.documentElement.setAttribute('data-theme', this.theme.value)

            window.addEventListener('message', e => {
                // 设备列表历史曲线导出
                this.curveSignalR(e, true)
                if (e.data && typeof e.data == 'object' && 'setMenu' in e.data) {
                    // 传递被删除的菜单项的route到Index组件中
                    // this.$emit("setMenu", { route: e.data.route, setMenu: e.data.setMenu });
                    this.resetMenu({ route: e.data.route, setMenu: e.data.setMenu })
                }
            })

            // new equipCache().Init();

            document.onkeyup = this.keyEvent

        },
        beforeDestroy () {
            clearTimeout(this.intevalObj)
            this.intevalObj = null
        },
        destroyed () {
            clearTimeout(this.intevalObj)
            this.intevalObj = null
        },
        watch: {
            editableTabs (val) {
                window.sessionStorage.editableTabs = JSON.stringify(this.editableTabs)
            },
            $route (to, from) {
                sessionStorage.menuActive = this.menuActive = to.fullPath
                if (to.path.includes('noAccess')) {
                    this.menu = []
                    this.overdue = true
                } else {
                    this.navTopActive = this.routesMap[this.menuActive]
                }
                this.$nextTick(() => {
                    // let activeDom = document.querySelector('.el-menu-item.is-active');
                    // let title = activeDom ? activeDom.getAttribute('name') : '';
                    let title = this.routesToName[to.path]

                    if (!this.config.labelPage) {
                        this.breadcrumbList = [
                            {
                                title: title,
                                path: sessionStorage.menuActive
                            }
                        ]
                        localStorage.breadcrumbList = JSON.stringify(this.breadcrumbList)
                    }
                    if (title && title != '') {
                        this.setTableTabs(title)
                    }

                    // 设置标签页选中项状态和名称
                    let len = this.editableTabs.length
                    for (let i = 0; i < len; i++) {
                        if (this.$route.fullPath == this.editableTabs[i].route) {
                            this.editableTabsValue = this.editableTabs[i].name
                            sessionStorage.menuActiveName = this.editableTabs[i].title
                            break
                        }
                    }
                })

                clearTimeout(this.intevalObj)
                this.intevalObj = null

                let that = this
                if (to.path.indexOf('/systemSnapshot') == -1) {
                    this.intevalObj = setTimeout(() => {
                        this.getRealTimeDataInfo()
                    }, that.delayTime)
                }
            },
            navTopActive (newVal) {
                this.menu = this.config.showTopNav ? [...this.allMenu[this.navTopActive].children] : this.menu
                function getFirstRoute (menus) {
                    for (let m of menus) {
                        if (m.route) {
                            return [m.route, m.name]
                        }
                        let [route, name] = getFirstRoute(m.children)
                        if (route) return [route, name]
                    }
                    return ['', '']
                }
                if (this.config.showTopNav) {
                    let [firstRoute, name] = getFirstRoute(this.menu)
                    sessionStorage.menuActiveName = name
                    firstRoute && this.$router.push(firstRoute)
                }
            }
        },
        methods: {
            openContextMenu (e) {
                function getTabName (element) {
                    if (!e.currentTarget.contains(element)) {
                        return ''
                    }
                    if (element.classList.contains('el-tabs__item')) {
                        return element.id.split('-')[1]
                    }
                    return getTabName(element.parentElement)
                }
                let targetName = getTabName(e.target)
                if (targetName === '') return
                let startIndex = this.editableTabs.findIndex(i => i.name == targetName)
                ContextMenu({
                    width: '200',
                    root: e.currentTarget,
                    left: e.clientX,
                    top: e.clientY,
                    commands: [
                        {
                            name: 'closeCurrent',
                            label: this.$t('login.contextMenu.close'),
                            icon: 'el-icon-close',
                            onclick: () => {
                                this.removeTab(targetName)
                            },
                            disabled: this.editableTabs.length <= 1
                        },
                        {
                            name: 'closeOther',
                            label: this.$t('login.contextMenu.closeOther'),
                            onclick: () => {

                                this.editableTabsValue = targetName
                                let toBeCloses = this.editableTabs.filter(i => i.name != targetName).map(i => i.name)
                                toBeCloses.forEach(i => {
                                    this.removeTab(i)
                                })
                            },
                            disabled: this.editableTabs.length <= 1
                        },
                        {
                            name: 'closeLeft',
                            label: this.$t('login.contextMenu.closeLeft'),
                            onclick: () => {
                                this.editableTabsValue = targetName
                                let toBeCloses = this.editableTabs.slice(0, startIndex).map(i => i.name)
                                toBeCloses.forEach(i => {
                                    this.removeTab(i)
                                })
                            },
                            disabled: startIndex == 0
                        },
                        {
                            name: 'closeRight',
                            label: this.$t('login.contextMenu.closeRight'),
                            onclick: () => {

                                this.editableTabsValue = targetName
                                let toBeCloses = this.editableTabs.slice(startIndex + 1).map(i => i.name)
                                toBeCloses.forEach(i => {
                                    this.removeTab(i)
                                })
                            },
                            disabled: startIndex == this.editableTabs.length - 1
                        },
                        {
                            name: 'reload',
                            label: this.$t('login.contextMenu.reload'),
                            icon: 'el-icon-refresh-right',
                            onclick: () => {
                                this.refreshTab(startIndex)
                            },
                            disabled: false
                        }
                    ]
                })
            },
            resizeEnd (width) {
                if (this.$refs.TopNav) {
                    this.$refs.TopNav.resize(width)
                }
            },

            // 改变内容区域全屏的状态
            contentFullscreenChange () {
                this.contentIsFullScreen = screenfull.isFullscreen
            },

            // 全屏
            getContentFullscreen () {
                if (!screenfull.isEnabled) {
                    return false
                }
                let contentFrame = document.getElementById(`pane-${this.editableTabsValue}`)
                screenfull.toggle(contentFrame)
            },

            resetMenu (data) {
                // 判断是否删除标签页
                if (data.route) {
                    // 找到标签页中被删除的菜单
                    this.editableTabs.forEach(item => {
                        if (data.route == item.route) {
                            this.removeTab(item.name)
                        }
                    })
                }

                // 判断是否重新获取菜单
                if (data.setMenu) {
                    this.getUserInfo()
                }
            },

            tabClick (event) {
                let route = event.$attrs.route
                sessionStorage.menuActive = this.menuActive = route
                this.$router.push(route)
            },

            // 设置标签页
            setTableTabs (title) {
                let len = this.editableTabs.length

                // 判断标签页数组是否存在当前选中导航项
                let isHave = this.editableTabs.some(item => {
                    return item.route == this.$route.fullPath
                })

                // 如果不存在则增加一项进数组，存在则只做标签页选中
                if (!isHave) {
                    this.editableTabs.push({
                        title: title,
                        route: this.$route.fullPath,
                        name: len ? String(Number(this.editableTabs[len - 1].name) + 1) : '1',
                        disabled: false
                    })
                    len = this.editableTabs.length
                    this.editableTabsValue = this.editableTabs[len - 1].name

                    if (len > 6) {
                        this.removeTab(this.editableTabs[0].name)

                        // this.editableTabs.shift();
                        len--
                    }
                } else {
                    for (let i = 0; i < len; i++) {
                        if (this.$route.fullPath == this.editableTabs[i].route) {
                            this.editableTabsValue = this.editableTabs[i].name
                            break
                        }
                    }
                }

                sessionStorage.menuActiveName = title
            },

            // 关闭标签页
            removeTab (targetName) {
                let tabs = this.editableTabs
                let activeName = this.editableTabsValue
                tabs.forEach((tab, index) => {
                    if (tab.name === targetName) {
                        if (activeName === targetName) {
                            let nextTab = tabs[index + 1] || tabs[index - 1]
                            if (nextTab) {
                                activeName = nextTab.name
                                sessionStorage.menuActiveName = nextTab.title
                                sessionStorage.menuActive = this.menuActive = nextTab.route
                                this.$router.push(nextTab.route)
                            }
                        }
                        this.editableTabs[index].disabled = true

                        let arr = document.getElementsByClassName('jumpIframe')
                        let iframe = arr[index]
                        if (iframe) {
                            let clearTree = iframe.contentWindow.clearTree
                            for (let item in clearTree) {
                                clearTree[item]()
                            }
                        }
                        setTimeout(() => {
                            let index = tabs.findIndex(i => i.name === targetName)
                            this.editableTabs.splice(index, 1)
                        }, 800)
                    }
                })
            },

            // 刷新标签页
            refreshTab (index, tabName) {
                if (tabName) {
                    let iframe = document.getElementById(tabName).getElementsByClassName('jumpIframe')[0]
                    iframe.contentWindow.location.reload()
                } else {
                    let iframe = document.getElementsByClassName('jumpIframe')
                    iframe[index].contentWindow.location.reload()
                }

                this.isActive = true
                setTimeout(() => {
                    this.isActive = false
                }, 500)
            },

            getRule () {
                this.$api
                    .GetAccountPasswordRule()
                    .then(res => {
                        if (res.data.code === 200) {
                            if (res.data.data) {
                                window.sessionStorage.accountRule = JSON.stringify(res.data.data)
                                this.setRule()
                            }
                        } else {
                            this.$message.error(res.data.message)
                        }
                    })
                    .catch(er => {
                        console.log(er)
                    })
            },

            setRule () {
                let rule = JSON.parse(window.sessionStorage.accountRule)
                let regArr = ['(?=.*[a-z])', '(?=.*[A-Z])', '(?=.*[0-9])', '(?=.*[`~!@#$%^&*()_+={}|:;"\'<>,.?/-])']
                let msgArr = [
                    this.$t('login.framePro.tips.lowercaseLetter'),
                    this.$t('login.framePro.tips.capitalize'),
                    this.$t('login.framePro.tips.number'),
                    this.$t('login.framePro.tips.specialSymbols')
                ]

                // 密码规则
                if (rule.password.enabled === true) {
                    // 密码长度
                    this.regPwdLength = new RegExp('^.{' + rule.password.length + ',32}$')
                    this.regPwdLengthMsg = this.$t('login.framePro.tips.userNameLength') + `${rule.password.length}-32` + this.$t('login.framePro.tips.characterNumber')

                    // 密码中必须包含的类型
                    let regStr = '^'
                    let newMsgArr = []
                    for (let item of rule.password.elements) {
                        regStr += regArr[item]
                        newMsgArr.push(msgArr[item])
                    }
                    this.regPwd1 = new RegExp(regStr)
                    this.regPwdMsg = this.$t('login.framePro.tips.userHas') + newMsgArr.join('、')

                    if (!rule.password.allowedUserName) {
                        this.pwdHaveName = this.$t('login.framePro.tips.passwordNoHasUser')
                    } else {
                        this.pwdHaveName = undefined
                    }

                    if (rule.password.minCharacters != 0) {
                        this.minCharacters = rule.password.minCharacters
                        this.pwdMinCharactersMsg = new Error(this.$t('login.framePro.tips.containAtLeast') + `${rule.password.minCharacters}` + this.$t('login.framePro.tips.differentCharacters'))
                    } else {
                        this.pwdMinCharactersMsg = undefined
                    }
                } else {
                    this.regPwdLength = /./
                    this.regPwd1 = /./
                }
            },

            validatePass (rule, value, callback) {
                if (value === '') {
                    callback(new Error(this.$t('login.framePro.tips.inputPassword')))
                } else if (!this.regPwdLength.test(value)) {
                    callback(new Error(this.regPwdLengthMsg))
                } else if (value != '' && !this.regPwd1.test(value)) {
                    callback(new Error(this.regPwdMsg))
                } else if (this.regPwd2.test(value)) {
                    callback(new Error(this.$t('login.framePro.tips.noChinese')))
                } else if (!this.regPwd3.test(value)) {
                    callback(new Error(this.$t('login.framePro.tips.noSpaces')))
                } else if (this.pwdCharacters < this.minCharacters) {
                    callback(this.pwdMinCharactersMsg)
                } else if (value == this.changePwd.oldPwd) {
                    callback(new Error(this.$t('login.framePro.tips.newAndOldPwd')))
                } else if (value.indexOf(window.sessionStorage.userName) != -1 && window.sessionStorage.userName != '') {
                    callback(this.pwdHaveName)
                } else {
                    callback()
                }
            },

            validateConfPass (rule, value, callback) {
                if (value === '') {
                    callback(new Error(this.$t('login.framePro.tips.enterConfirmPas')))
                } else if (value != this.changePwd.newPwd) {
                    callback(new Error(this.$t('login.framePro.tips.towPasAreNoSame')))
                } else {
                    callback()
                }
            },

            // // 中英文切换方法
            changeLang () {
                this.$i18n.locale == 'zh-CN' ? (this.$i18n.locale = 'en') : (this.$i18n.locale = 'zh-CN') // 设置中英文模式
                localStorage.setItem('languageSet', this.$i18n.locale) // 将用户设置存储到localStorage以便用户下次打开时使用此设置
            },

            getQueryParm (name) {
                return (
                    decodeURIComponent(
                        (new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(
                            location.href
                            // eslint-disable-next-line no-sparse-arrays
                        ) || [, ''])[1].replace(/\+/g, '%20')
                    ) || null
                )
            },

            // 截取路由
            splitRouter (val) {
                let url = val.split('/')
                return url[url.length - 1]
            },

            // 历史菜单-最大10个记录
            historical (data) {
                let index = -1
                if ((index = this.historicalList.findIndex(v => v.route == data.route)) > -1) {
                    this.historicalList.splice(index, 1)
                    this.historicalList.unshift(data)
                } else {
                    this.historicalList.unshift(data)
                }
                if (this.historicalList.length > 10) {
                    this.historicalList.pop()
                }

                // localStorage.historicalList = JSON.stringify(this.historicalList);
            },

            // 菜单点击事件
            onRouters (type, data, el) {

            },

            getName (name) {
                sessionStorage.menuActiveName = name
            },

            handleOpen (key, keyPath) {

            },

            handleClose (key, keyPath) {
                // 禁止折叠菜单
                this.$refs.menus.open(keyPath)
            },

            toRouter () {
                this.menuActive = '/Index/jumpIframe/ganwei-iotcenter-system-snapshot/systemSnapshot'
                parent.vm.$router.push({ path: '/Index/jumpIframe/ganwei-iotcenter-system-snapshot/systemSnapshot' })
            },

            getRealTimeEventTypeConfig () {
                this.$api
                    .getRealTimeEventTypeConfig()
                    .then(res => {
                        if (res.data.code == 200) {
                            let typeList = res.data.data
                            typeList.forEach(item => {
                                this.eventType[`${item.snapshotLevelMin}-${item.snapshotLevelMax}`] = {
                                    iconRes: item.iconRes,
                                    snapshotName: item.snapshotName
                                }
                            })
                        } else {
                            this.$message.error(res.data.message, res)
                            this.eventType = {}
                        }
                    })
                    .catch(err => {
                        this.$message.error(err.data, err)
                        console.log(err)
                        this.eventType = {}
                    })
            },
            getRandomInt (min, max) {
                return Math.floor(Math.random() * (max - min + 1)) + min
            },
            getSnapshotColorClass (src) {
                let className = this.snapshotColorClass[0]
                this.snapshotColorClass.forEach(item => {
                    if (src.includes(item)) {
                        className = item
                    }
                })
                return className
            },
            getEventType (level) {
                let keys = Object.keys(this.eventType)
                let key = keys[0]
                keys.forEach(item => {
                    let arr = item.split('-');
                    if (Number(level) >= Number(arr[0]) && Number(level) <= Number(arr[1])) {
                        key = item
                    }
                })
                let obj = {}
                obj = JSON.parse(JSON.stringify(this.eventType[key]))

                return obj
            },


            inSpeakttsArray (level) {
                let key = ''
                if (this.speakttsRange) {
                    this.speakttsRange.forEach(item => {
                        let arr = item.split('-');
                        if (Number(level) >= Number(arr[0]) && Number(level) <= Number(arr[1])) {
                            key = item
                        }
                    })
                }
                if (key) {
                    return true
                }
                return false
            },

            getRealTimeDataInfo () {
                let that = this
                this.$api
                    .getRealTimeData()
                    .then(res => {
                        if (!that.noPermission) {
                            clearTimeout(that.intevalObj)
                            that.intevalObj = null
                            that.intevalObj = setTimeout(() => {
                                that.getRealTimeDataInfo()
                            }, that.delayTime)
                        }

                        if (res.data.code == 200) {
                            let data = res.data.data.Result

                            // data = [{ 'RelatedVideo': '', 'ZiChanID': '', 'PlanNo': '', 'Level': 3, 'EventMsg': '温湿度#11156-遥信1:报警', 'ProcAdviceMsg': '请处理', 'RelatedPic': '', 'Wavefile': 'YX62_1_1.wav', 'Equipno': 11156, 'Type': 'X', 'Ycyxno': 1, 'Time': '2023-06-06T03:32:08', 'bConfirmed': false, 'UserConfirm': null, 'DTConfirm': '1970-01-01T00:00:00', 'GUID': 'dbcb947f-7201-426b-b9f4-5a22219a1040' }]
                            data.forEach(item => {
                                let eventType = this.getEventType(item.Level)
                                item.snapshotName = eventType.snapshotName
                                item.iconRes = eventType.iconRes
                                item.Time = this.$moment(item.Time).format('YYYY-MM-DD HH:mm:ss')
                                item.className = this.getSnapshotColorClass(item.iconRes)

                                // 是否在语音播报列表中
                                if (this.inSpeakttsArray(item.Level)) {
                                    if (this.showSound) {
                                        this.openSpeak()
                                        if (this.noFirst) {
                                            this.speechPlay('即将进行语音播报', false)
                                        }
                                        this.speechPlay(item.EventMsg, false)
                                    } else {
                                        this.closeSpeak()
                                    }

                                    this.noFirst = false
                                }
                            })

                            this.messageList.unshift(...data)
                        }
                    })
                    .catch(err => {
                        if (err.status == 401 || err.status == 403) {
                            clearTimeout(that.intevalObj)
                            that.intevalObj = null
                            that.noPermission = true
                        } else {
                            clearTimeout(that.intevalObj)
                            that.intevalObj = null
                            that.intevalObj = setTimeout(() => {
                                that.getRealTimeDataInfo()
                            }, that.delayTime)
                        }
                        console.log(err)
                    })
            },

            setSpeak (obj) {
                this.speech.init(obj).then(() => {
                    console.log("success");
                })
            },

            closeSpeak () {
                this.speech.init({ volume: 0, rate: 2 }).then(() => {
                    console.log("success");
                })
                this.showSound = false
            },
            openSpeak () {
                this.speech.init({ volume: 1, rate: 2 }).then(() => {
                    console.log("success");
                })
                this.showSound = true
            },

            topNavClick (index) {
                this.navTopActive = index
            },

            traverse (data, index) {
                if (!data.route) {
                    if (data.children && data.children.length > 0) {
                        for (let item of data.children) {
                            this.traverse(item, index)
                        }
                    }
                } else {
                    this.routesMap[data.route] = index
                    this.routesToName[data.route] = data.name

                    // 保存第一项菜单
                    this.firstMenuItem.route = this.firstMenuItem.route || data.route
                    this.firstMenuItem.name = this.firstMenuItem.name || data.name
                }
            },

            getUserInfo () {
                let res = getMenuesList;
                            this.isError = false
                            let withoutGroup = []
                            this.navTopOpt = []
                            this.routesMap = []
                            this.allMenu = res
                            this.allMenu.forEach((group, index) => {
                                const { name, resourceId: id } = group
                                this.navTopOpt.push({ name, id, index })
                                if(group.children)
                                   withoutGroup.push(...group.children)
                                this.traverse(group, index)
                            })
                            if (this.menuActive) {
                                this.navTopActive = this.routesMap[this.menuActive]
                            }

                            if (this.config.showTopNav) {
                                this.menu = [...this.allMenu[this.navTopActive].children]
                                window.sessionStorage.asideList = JSON.stringify(withoutGroup)
                            } else {
                                this.menu = JSON.parse(JSON.stringify(res))
                                window.sessionStorage.asideList = JSON.stringify(this.allMenu)
                            }
                            if (this.menu.length > 0) {
                                this.iaHaveSnapshot = false
                                if (window.sessionStorage.asideList && (this.$route.path == '/Index' || this.$route.path == '/index/')) {
                                    this.$router.push(this.firstMenuItem.route)

                                    sessionStorage.menuActiveName = this.firstMenuItem.name
                                }

                                // 初始化第一个标签页
                                if (this.editableTabs.length == 0) {
                                    this.editableTabs.push({
                                        title: sessionStorage.menuActiveName,
                                        route: sessionStorage.menuActive || this.firstMenuItem.route,
                                        name: '1'
                                    })
                                    this.editableTabsValue = '1'
                                }
                            }
            },

            // 系统运行信息
            onSystemOperationInfor () {
                this.curveShowCurve = true
                this.getSystemInfo()
            },

            onCloseCurve () {
                this.curveShowCurve = false
            },

            // 获取系统运行信息
            getSystemInfo () {
                this.$api.getSystemInfo().then(rt => {
                    if (rt.status == 200) {
                        let infoList = [
                            {
                                title: JSON.parse(rt.data.data.systemPlatformInfo).Item1,
                                value: JSON.parse(rt.data.data.systemPlatformInfo).Item2.slice(-2)
                            }
                        ]
                        this.infoList = infoList
                    }
                })
            },

            inspectState () {
                if (window.sessionStorage.passwordPolicy && window.sessionStorage.passwordPolicy == 1) {
                    this.modal1 = true
                }
                window.addEventListener('message', e => {
                    // 设备列表历史曲线导出
                    this.curveSignalR(e, true)
                    if (e.data && typeof e.data == 'object' && 'setMenu' in e.data) {
                        // 传递被删除的菜单项的route到Index组件中
                        // this.$emit("setMenu", { route: e.data.route, setMenu: e.data.setMenu });
                        this.resetMenu({ route: e.data.route, setMenu: e.data.setMenu })
                    }
                })

                document.onkeyup = this.keyEvent
            },

            // 清空输入的密码
            clearInput () {
                this.changePwd.oldPwd = ''
                this.changePwd.newPwd = ''
                this.changePwd.confPwd = ''
                this.$refs['changePasswordForm'].resetFields()
            },

            pwdInputChange (val) {
                if (window.sessionStorage.accountRule) {
                    let rule = JSON.parse(window.sessionStorage.accountRule)
                    if (rule.password.minCharacters != 0) {
                        let len = val.length
                        let arr = []
                        for (let i = 0; i < len; i++) {
                            if (arr.indexOf(val[i]) == -1) {
                                arr.push(val[i])
                            }
                            this.pwdCharacters = arr.length
                            if (arr.length >= rule.password.minCharacters) {
                                break
                            }
                        }
                    }
                }
            },

            showEditPwd () {
                // 获取最新规则，防止其他人更新后没有获取到最新规则
                this.getRule()
                this.modal1 = true
            },

            // 修改密码
            changeCode () {
                this.$refs['changePasswordForm'].validate(async valid => {
                    if (valid) {
                        let data = {
                            userName: window.sessionStorage.userName,
                            oldPassword: await this.$getCode.RSAEncrypt(this.changePwd.oldPwd),
                            newPassword: await this.$getCode.RSAEncrypt(this.changePwd.newPwd)
                        }
                        this.$api.getUpdUserInfoData(data).then(rt => {
                            let code = rt.data.code
                            if (code == 200) {
                                window.sessionStorage.removeItem('passwordPolicy')
                                this.modal1 = false
                                this.$message.success(this.$t('login.framePro.tips.changeSuccess'))
                                this.quitLogin()
                            } else if (code == 5001) {
                                this.$message.warning(rt.data.message)
                                return
                            } else if (code == -100) {
                                this.$message.error(rt.data.message)
                                setTimeout(() => {
                                    this.quitLogin()
                                }, 1000)
                            } else {
                                this.$message.error(
                                    rt.data.message
                                )
                            }
                        })
                    }
                })
            },

            // 退出登录
            quitLogin () {
                this.modal2 = false
                let isSsoLogin = window.sessionStorage.getItem('isSsoLogin')

                if (isSsoLogin) {
                    this.$router.replace('/jumpIframeLogin/ganwei-iotcenter-login/ssoLogout')
                    window.sessionStorage.clear()
                    return
                }
                this.$api.loginOut().then(rt => {
                    if (rt.data.code == 200) {
                        window.sessionStorage.clear()
                        this.$message.success(this.$t('login.framePro.tips.loggedOut'))
                        try {
                            // eslint-disable-next-line
                            myJavaFun.OpenLocalUrl('login')
                        } catch (e) {
                            if (isSsoLogin == null || isSsoLogin == undefined || !isSsoLogin) {
                                // this.$router.replace('/');
                                window.location.href = window.location.origin + '/'
                            } else {
                                // window.location.href = '/loginOut.html';
                                this.$router.replace('/jumpIframeLogin/ganwei-iotcenter-login/ssoLogout')
                            }
                        }
                    } else {
                        this.$message.error(this.$t('login.framePro.tips.logOutFail'))
                    }
                })
            },
            init () {
                this.loginUn = this.loginUsername ? this.loginUsername.substr(0, 1).toUpperCase() : ''
            },
            onListIndex (index, type) {
                if (type === 'child') {
                    this.childIndex = index
                    return
                }
                this.childIndex = 0
            },

            // 导航菜单折叠显示
            onAsideListShow () {
                if (this.transition) {
                    return
                }
                this.transition = true
                this.isCollapse = !this.isCollapse
                setTimeout(() => {
                    this.transition = false
                }, 600)
            },

            // 全屏、不全屏
            getFullCreeen () {
                this.n++
                this.n % 2 == 0 ? this.outFullCreeen(document) : this.inFullCreeen(document.documentElement)
            },
            inFullCreeen (element) {
                let el = element
                let rfs = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullScreen
                if (typeof rfs != 'undefined' && rfs) {
                    rfs.call(el)
                } else if (typeof window.ActiveXObject != 'undefined') {
                    // eslint-disable-next-line
                    let wscript = new ActiveXObject('WScript.Shell')
                    wscript.SendKeys('{F11}')
                }
            },
            outFullCreeen (element) {
                let el = element
                let cfs = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullScreen
                if (typeof cfs != 'undefined' && cfs) {
                    cfs.call(el)
                } else if (typeof window.ActiveXObject != 'undefined') {
                    // eslint-disable-next-line
                    let wscript = new ActiveXObject('WScript.Shell')
                    wscript.SendKeys('{F11}')
                }
            },
            checkStyle (theme) {
                localStorage.setItem('theme', theme)
                this.theme = JSON.parse(JSON.stringify(this.config.theme.supportThemes.find(item => item.value === localStorage.theme)))
                window.document.documentElement.setAttribute('data-theme', theme)

                let iframe = document.getElementsByClassName('jumpIframe')
                for (let item of iframe) {
                    item.contentWindow.document.documentElement.setAttribute('data-theme', theme)
                    item.contentWindow.postMessage({ theme: theme })

                    // item.contentWindow.location.reload()
                }
                this.myUtils.changeStyle()
            },
            langChange (val) {
                window.document.documentElement.setAttribute('data-languagetype', val)
            },

            // 安全模式2023-02-23
            setSafeMode () {
                let that = this
                this.$api
                    .getSafeLevelByGateway()
                    .then(rt => {
                        if (rt.data.code == 200) {
                            that.safeMode = rt.data.data
                            if (that.safeMode == 1) {
                                that.safeTipsText = 'login.tips.safeLevel.high'
                            } else if (that.safeMode == 2) {
                                that.safeTipsText = 'login.tips.safeLevel.middle'
                            } else {
                                that.safeTipsText = 'login.tips.safeLevel.low'
                            }
                            setTimeout(function () {
                                that.safeTipsText = ''
                            }, that.safeTipsTime)
                        } else {
                        }
                    })
                    .catch(e => { })
            }
        }
    }
</script>

<style lang="scss" src="gw-base-style-plus/index.scss" scoped></style>
