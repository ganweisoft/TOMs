<template>
    <div id="app">
        <router-view />
    </div>
</template>

<script>
export default {
    name: 'App',
    mounted () {
        let style = window.localStorage.theme;
        window.document.documentElement.setAttribute('data-theme', style || 'dark')
    }
};
</script>

<style>
html,
body,
#app {
    width: 100%;
    height: 100%;
    padding: 0!important;
}
</style>
