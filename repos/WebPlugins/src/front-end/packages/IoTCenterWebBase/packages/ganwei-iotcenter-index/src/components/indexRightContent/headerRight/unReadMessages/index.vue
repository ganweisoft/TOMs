<template>
    <unread :messageList="messageList" @updateMsgList="updateMsgList" @jump="toRouter" :showSound="showSound"
        @openSpeak="openSpeak" :theme="theme" @closeSpeak="closeSpeak">
    </unread>
</template>

<script src="./index.js"></script>
<style src="./index.scss"></style>