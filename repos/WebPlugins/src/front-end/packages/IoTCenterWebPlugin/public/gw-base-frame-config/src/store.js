import Vue from 'vue'
import Vuex from 'vuex'
import * as types from './types'

Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        gwToken: '',
        loginMsg: '',
        curPage: {
            isHome: false,
            name: '',
            childName: null
        },
        curEquip: {
            equipNo: ''
        },

        overEquipNmae: '',

        // 视频请求方式
        realProtocol: '',

        // 视频请求地址
        realHost: '',

        // 这里开始是h5视频流
        xzvalue: new Date(),
        usingEq: [],
        user: {},
        token: null,
        title: '',
        lang: 'en',
        rtc: '',
        watermarkstring: 'linkingvision',
        watermarktoggle: '',
        equipNoPush: '888,555,111,222', // 用于数据推送的设备号参数
        ycpDataList: '', // 存储推过来的实时数据
        yxpDataList: '', // 存储推过来的实时数据
        urlRoute: '', // 监听URL改变参数
        homeImg: false,
        videoNm: [],
        videoList: [],
        equipInfoSelected: [],
        isCollapse: false, // 导航菜单折叠显示
        languageSet: 'zh-CN', // 中英文切换,
        network: true, // true代表正常连接，false代表网络连接不上
        noAccessLoading: true
    },
    mutations: {
        isCollapseFun(state, value) {
            state.isCollapse = value
        },
        languageSetFun(state, value) {
            state.languageSet = value
        },
        getOverEquipNmae(state, value) {
            state.overEquipNmae = value
        },
        getStorageToken(state) {
            let token = window.sessionStorage.getItem('accessToken')
            state.gwToken = token
        },
        setEquipNo(state, nom) {
            state.curEquip.equipNo = nom
        },
        setCurpage(state, obj) {
            state.curPage.isHome = obj.isHome
            state.curPage.name = obj.name
            state.curPage.childName = obj.childName || null
        },
        getVideoList(state, value) {
            state.videoList = value
        },
        getVideoNm(state, value) {
            state.videoNm = value
        },
        getHomeImg(state, value) {
            state.homeImg = value
        },
        getUrlRoute(state, value) {
            state.urlRoute = value
        },
        getEquipNoPush(state, value) {
            state.equipNoPush = value
        },

        getYcpDataList(state, value) {
            state.ycpDataList = value
        },

        getYxpDataList(state, value) {
            state.yxpDataList = value
        },
        getStorageLoginMsg(state) {
            state.loginMsg = window.localStorage.getItem('login_msg')
        },
        setSelectedEquip(state, value) {
            state.selectedEquip = value
        },
        setEquipInfo(state, Selected) {
            state.equipInfoSelected = Selected
        },
        saveNetwork(state, value) {
            state.network = value
        },
        saveNoAccessLoading(state, value) {
            state.noAccessLoading = value
        },

        // 从这里开始是h5视频流
        [types.WATERMARKTOGGLE]: (state, data) => {
            localStorage.h5watermarktoggle = data
            state.watermarktoggle = data
        },
        [types.WATERMARKSTRING]: (state, data) => {
            localStorage.h5watermarkstring = data
            state.watermarkstring = data
        },
        [types.RTCSW]: (state, data) => {
            localStorage.h5rtcsw = data
            state.rtc = data
        },
        [types.LOGIN]: (state, data) => {
            localStorage.h5stoken = data
            state.token = data
        },
        [types.LOGOUT]: state => {
            localStorage.removeItem('h5stoken')
            state.token = null
        },
        [types.TITLE]: (state, data) => {
            state.title = data
        },
        [types.LANG]: (state, data) => {
            localStorage.h5slang = data
            state.lang = data
        }
    },

    actions: {
        commitNetwork(context, value) {
            context.commit('saveNetwork', value)
        },
        commitAccessLoading(context, value) {
            context.commit('saveNoAccessLoading', value)
        },
        isCollapseFun(context, value) {
            context.commit('isCollapseFun', value)
        },
        languageSetFun(context, value) {
            context.commit('languageSetFun', value)
        },
        getOverEquipNmae(context, value) {
            context.commit('getOverEquipNmae', value)
        },
        getVideoList(context, value) {
            context.commit('getVideoList', value)
        },
        getVideoNm(context, value) {
            context.commit('getVideoNm', value)
        },
        getHomeImg(context, value) {
            context.commit('getHomeImg', value)
        },
        getUrlRoute(context, value) {
            context.commit('getUrlRoute', value)
        },
        getEquipNoPush(context, value) {
            context.commit('getEquipNoPush', value)
        },
        getYcpDataList(context, value) {
            context.commit('getYcpDataList', value)
        },
        getYxpDataList(context, value) {
            context.commit('getYxpDataList', value)
        },
        reflashSet({ commit }) {
            commit('getStorageToken')
            commit('getStorageLoginMsg')
        }
    }
})
