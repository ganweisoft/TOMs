<template>
    <div class="context">
        <!-- 标题 -->
        <div class="title" v-if="props.panelControl.title">
            <span class="title-text">{{playStreamInfo.playName}}</span>
        </div>
        <!-- 重播 -->
        <div :id="'replay'+id" class="replayVideo" v-if="replayStatus && !isPlay">
            <i class="iconfont icon-_bofang" @click="replayVideo" ></i>
            <p>{{tipsInfo}}</p>
        </div>
        <!-- 播放器 -->
        <div class="player-container" v-loading="loading">
            <div :id="'glplayer'+id" class="glplayer" :class="{transparent: isPlay}"></div>
        </div>
        <!-- 选择列表 -->
        <div class="stream-list" :id="'stream-list'+id" v-if="panelControl.streamList">
            <el-select v-model="playStreamInfo.id"  @change="streamTypeChange" v-if="playStreamIdList.length>0" :append-to="'#stream-list'+id">
                <template v-slot:prefix>
                    <i class="iconfont icon-gw-icon-diqiu"></i>
                </template>
                <el-option v-for="(item) in playStreamIdList" :label="item.name" :value="item.id" :key="item.protocol" > </el-option>
            </el-select>
        </div>
        <!-- 控制按钮 -->
        <videoControl ref="control" v-if="panelControl.panelControl && isPlayerReady" :isPlay="isPlay" :panelControl="panelControl" @fullScreenHandler="bottomControl.fullScreenHandler" @volumeHandler="bottomControl.volumeHandler" @closeHandler="closeStreamPlay" @ptzControlHandler="ptzControlHandler"></videoControl>
        <!-- 播放信息 -->
        <videoPlayInfo v-if="panelControl.beforePlayInfo" :playInfo="playInfo" :isPlayInfo="isPlayInfo"></videoPlayInfo>
    </div>
</template>

<script setup lang="ts">
    // 基础
    import { getCurrentInstance, onUnmounted, provide, reactive, ref} from 'vue'

    import { useMessage } from "@components/@ganwei-pc/gw-base-utils-plus/notification";

    import { createPlayer, SupportPlayerEnum } from "../classDefintion/Player"
    import { AsyncSignalRController } from '../signalr/AsyncSignalRController';
    import videoControl from '../videoControl/videoControl.vue'
    import videoPlayInfo from '../videoPlayInfo/videoPlayInfo.vue'
    import useMultiScreenPlayerController from './js/useMultiScreenPlayerController'
    import { IPlayerContext, PlayerContextKey } from './js/usePlayerContext'
    import useStreamList from './js/useStreamList'
    import { IStreamInfo, PTZCommandEnum, PTZControlTypeEnum } from './Models/index'

    export interface IH265WebProps {
        id: number,
        selected: number,
        panelControl: {
            title?: boolean,
            streamList?: boolean,
            panelControl?: boolean,
            beforePlayInfo?: boolean,
            record?: boolean,
            capture?: boolean,
            volume?: boolean,
            close?: boolean,
            fullScreen?: boolean,
        }
    }

    const control = ref()

    // 值传输
    const props = withDefaults(defineProps<IH265WebProps>(), {
        id: 0,
        selected: 0,
        panelControl: () => ({})
    })

    const $message = useMessage()

    const {proxy} = getCurrentInstance() as any

    // #region 播放器
    const loading = ref(false) // 缓冲
    const isPlay = ref(false) // 是否播放
    const replayStatus = ref(false) // 重播状态控制
    const tipsInfo = ref('播放时间较长，请重试!') // 提示信息
    const playStreamInfo = reactive({
        id: '', // 当前播放流id
        protocol: '', // 当前播放流协议，
        playName: '', // 播放设备名称
    })

    const playerContext: IPlayerContext = {
        deviceId: 0,
        nvrChannelId: 0,
        ptzControl: false,
        player: null
    }
    const isPlayerReady = ref(false)
    provide(PlayerContextKey, playerContext)
    //#endregion
    const { playStreamIdList, setPlayStreamIdList} = useStreamList() // 流列表

    const multiScreenPlayerController = useMultiScreenPlayerController()
    let asyncPlay: AsyncSignalRController | null = null // signalR  对象

    // #region 面板信息
    const playInfo = ref<string[]>([]) // 播放信息
    const isPlayInfo = ref(false) // 是否显示播放前信息
    let startTime = 0 // 开始播放时间
    let endTime = 0 // 准备就绪时间
    let playDuration = 0 // 播放时长
    //#endregion

    // #region 倒计时
    const delayTime = 30000
    let playInfoCancelControl = -1 // 播放信息取消定时器控制
    let setTimeoutFun = -1
    //#endregion

    // 左侧流获取监听
    proxy.$bus.off('H265web' + props?.id)
    proxy.$bus.on('H265web' + props?.id, (streamList: Array<IStreamInfo>) => {

        // 释放资源、重置状态
        dispose()
        clearPlayStreamInfo()
        control.value?.reset()
        if(!streamList.length) {
            console.error('没有流信息')
            return;
        }

        setPlayStreamIdList(streamList)

        // 左侧列表接口返回值提示
        if(streamList[0]?.tips) {
            infoTips(streamList[0]?.tips)
            return;
        }

        // 列表信息
        if(streamList[0]?.id) {
            playStreamInfo.playName = streamList[0].title
            playStreamInfo.id = streamList[0].id
            playStreamInfo.protocol = streamList[0].protocol
            playerContext.deviceId = streamList[0].deviceId
            playerContext.nvrChannelId = streamList[0].nvrChannelId
            playerContext.ptzControl = streamList[0].ptzControl === PTZControlTypeEnum.GB28181
            reset()
            createAsyncPlayerSignalRController(streamList[0].id)
        } else {
            let url = streamList[0].url
            if(url) {
                h265webStart(url)
            }
        }
    })

    //播放
    function h265webStart(playUrl: string){
        console.log('播放流地址：' + playUrl);
        countdownStopped() // 倒计时停止播放
        loading.value = true // 开启缓冲
        startTime = performance.now(); // 计时开始

        playerContext.player = createPlayer(SupportPlayerEnum.H265WebPlayer, {
            player: 'glplayer' + props.id
        })
        isPlayerReady.value = true
        try {
            playerContext.player.init(playUrl);
            playerContext.player.eventsOn(getEventListenerFunction())
            playerContext.player.start();
            playerContext.player.play();
        } catch (e) {
            console.log(e);
        }
    }

    // 重新播放
    function replayVideo(){
        let val = playStreamInfo.id
        if(val) {
            streamTypeChange(val)
        }
    }

    /*
        显示播放前日志--后端方法大写，保持同名应用
        60: 流可播放
        80: 播放成功
        90: 播放失败
        100: 流已存在
        result: 失败
    */
    function ShowLog(res: any, id: string){
        //#region 过滤历史信息
        if(!asyncPlay) {
            return
        }
        if(asyncPlay.connectionId === '') {
            console.error('当前窗口未连接成功')
            return ;
        }
        if(asyncPlay.connectionId !== id) {
            console.error('现在的ID： ', asyncPlay.connectionId);
            console.error('接收到的ID： ', id);
            return ;
        }
        //#endregion

        if(!res) {
            return;
        }
        // 关闭连接
        if(!res.result || res.playStatus == 80 || res.playStatus == 90) {
            asyncPlay.stop()
        }

        // 显示重新播放、提示
        if(!res.result || res.playStatus == 90) {
            infoTips("播放失败!")
        }

        // 记录总耗时
        if(playDuration < res.duration) {
            playDuration = res.duration
        }

        // 允许播放
        if((res.playStatus == 60 && res.protocol == playStreamInfo.protocol) || res.playStatus == 100){
            if(res.url) { //目标播放流不为空
                h265webStart(res.url)
            } else {
                infoTips("目标地址为空,播放失败!")
            }
        }

        // 信息记录
        if(res.message && res.duration) {
            if(res.playStatus == 60 && res.protocol != playStreamInfo.protocol) {
                // 舍弃入栈
            } else {
                playInfo.value.push(res.message + "(" + res.duration + "ms)")
            }
        }
    }

    // 切换流类型
    function streamTypeChange(val: string){
        if(val) {
            dispose()
            reset()
            createAsyncPlayerSignalRController(val)
        } else {
            infoTips("播放失败，流地址不存在!")
        }
    }

    function createAsyncPlayerSignalRController(val: string) {
        asyncPlay = multiScreenPlayerController.addScreen(props.id, [val, ShowLog])
        asyncPlay.start()
    }

    // 重播状态更改
    function openReplay(status: boolean){
        replayStatus.value = status
    }

    // 关闭流播放
    function closeStreamPlay(){
        infoTips("")
    }

    // 清理播放器
    function destroyVideo(){
        if(setTimeoutFun) { clearTimeout(setTimeoutFun) }
        if(playerContext.player) {
            console.log('关闭播放器')
            playerContext.player.destroy();
            playerContext.player = null
            isPlay.value = false
            isPlayerReady.value = false
        }
    }

    // 播放器方法监听
    function getEventListenerFunction(){
        return {
            onPlayState: (state: boolean) => {
                isPlay.value = state
            },
            onPerformance: (time: number) => {
                loading.value = false  // 取消缓冲效果
                isPlay.value = true  // 修改为播放状态
                const timeInterval = time
                playInfo.value.push("播放器加载完成时间: " + timeInterval.toFixed(4) + "ms")
                playInfo.value.push("播放总耗时: " + (playDuration + timeInterval).toFixed(4) + "ms")

                if(setTimeoutFun) {  // 播放成功取消倒计时
                    clearTimeout(setTimeoutFun)
                }

                // 播放成功后，倒计时清除播放前信息
                if(playInfoCancelControl) { clearTimeout(playInfoCancelControl) }
                playInfoCancelControl = window.setTimeout(()=>{
                    isPlayInfo.value = false
                    playInfoCancelControl = -1
                }, 5000)
            },
            onLoadFinish: () => {
                playerContext.player?.setVoice(0)
                let mediaInfo = playerContext.player?.instance.mediaInfo()
                let codecName = "h265";
                if (mediaInfo?.meta?.isHEVC === false) {
                    console.log("\r\nonLoadFinish is Not HEVC/H.265");
                    codecName = "h264";
                } else {
                    console.log("\r\nonLoadFinish is HEVC/H.265");
                }

            }
        }
    }

    // 倒计时停止播放
    function countdownStopped(){
        if(setTimeoutFun) { clearTimeout(setTimeoutFun) }
        setTimeoutFun = window.setTimeout(()=>{ // 倒计时30秒，未播放则取消loading、显示提示、销毁播放器
            if(!isPlay.value){
                infoTips("播放超时!")
            }
            setTimeoutFun = -1
        }, delayTime)
    }

    // 信息提示
    function infoTips(txt: string){
        openReplay(true) // 显示重播
        loading.value = false // 关闭缓冲
        tipsInfo.value = txt  // 无可播放内容内部展示
        dispose() // 回收播放器
    }

    // 右上角列表
    function clearPlayStreamInfo(){
        playStreamInfo.playName = ""
        playStreamInfo.id = ''
    }

    // 底部控制方法
    const bottomControl = {
        // 暂停
        pauseHandler: function(){
        if (playerContext.player?.instance?.isPlaying()) { playerContext.player?.pause(); }
        },
        // 播放
        resumeHandler: function(){
            playerContext.player?.play();
        },

        // 音量
        volumeHandler: function(val: number){
            playerContext.player?.setVoice(val)
        },

        // 全屏
        fullScreenHandler: function(){
            playerContext.player?.fullScreen();
        },

        // 截图
        snapshotHandler: function(){
            playerContext.player?.snapshot(); // snapshot to canvas
        }
    }

    function ptzControlHandler(cmd: PTZCommandEnum, args: object) {
        playerContext.player?.ptzControl(cmd, args).then(res => {
            $message.success('命令下发成功，请稍等')
        }).catch(err => {
            $message.error(err)
        })
    }

    function dispose () {
        destroyVideo()
        asyncPlay?.stop()
        asyncPlay = null
    }

    function reset() {
        loading.value = true
        playDuration = 0
        openReplay(false)
        // 信息展示初始化
        playInfo.value.length = 0
        isPlayInfo.value = true
        isPlay.value = false
    }

    // 生命周期
    onUnmounted(()=>{
        reset()
        dispose()
    })

</script>

<style scoped lang="scss">
i:hover{
    transition: all .3s ease-in-out;
    opacity: 1;
    transform: scale(1.2);
}
.context{
    width: 100%;
    height: 100%;
    position: relative;
    overflow: hidden;
    .title{
        position: absolute;
        right: 10px;
        top: 10px;
        z-index: 2001;
    }
    .player__button{
        width: 100%;
        display: flex;
        justify-content: space-between;
        position: absolute;
        bottom: 0;
    }
}
.header-line {
  height: 30px;
}

ul {
  margin: 0;
  padding: 0;
}
ul li {
  list-style-type: none;
  padding: 5px 0;
}
ul li a {
  text-decoration: none;
}

.player-container {
  width: 100%;
  height: 100%;
  position: relative;
}
.player-container .glplayer {
    width: 100% !important;
    height: 100% !important;

    &.transparent {
        background-color: transparent !important;
    }
}

.replayVideo{
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    position: absolute;
    z-index: 2008;
}

.replayVideo p{
    text-align: center;
    color: orange;
    opacity: 0.6;
    margin-top: 10px;
}

.replayVideo i{
    font-size: 40px;
    background: rgba(255,255,255,0.1);
    border-radius: 50%;
    display: flex;
    width: 60px;
    height: 60px;
    align-items: center;
    justify-content: center;
}

.player__button {
    border: 1px solid black;
    width: 80px;
    border-radius: 5px;
    text-align: center;
    margin: 10px 0;
    cursor: pointer;
}

.timeline__container {
    margin: 5px 0;
}

.timeline__container span:nth-child(2) {
    margin-left: 10px;
}

.stream-list {
  min-width: 120px;
  position: absolute;
  right: 10px;
  top: 35px;
  display: flex;
  align-items: center;
  color: white;
  place-content: stretch;
  z-index: 2009;
  opacity: 0;
  i{font-size: 24px;opacity: 0.6;}
}

.stream-list:hover{
    opacity: 1;
}

</style>

<style>

.right-context-container .el-loading-mask{
    visibility: visible;
}

video::-webkit-media-controls-panel {
    display: none;
}

video::-webkit-media-controls-mute-button {
    display: none;
}

video::-webkit-media-controls-timeline {
    display: none;
}

video::-webkit-media-controls-time-remaining-display {
    display: none;
}
video::-webkit-media-controls-volume-slider {
    display: none;
}

</style>

<style>
.stream-list .el-select-dropdown__wrap .el-select-dropdown__list li{
    padding: 0;
    span{
    padding-left: 10px;
    }
}
.player-container .glplayer video{
    width: 100% !important;
    height: 100% !important;
    object-fit: fill;
}
.player-container .glplayer canvas{
    width: 100%;
    height: 100%;
    margin: 0;
    display: none;
}

</style>
