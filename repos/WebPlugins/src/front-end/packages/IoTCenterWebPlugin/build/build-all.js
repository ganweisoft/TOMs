let _env = require('../config/prod.env')
process.env.PAGE_ENV = JSON.parse(_env.PAGE_ENV)
process.env.PAGES_ENV = JSON.parse(_env.PAGES_ENV)
const path = require('path')
const execFileSync = require('child_process').execFileSync
const moduleList = require('./module-conf').moduleList || []
const buildFile = path.join(__dirname, 'build.js')

for (const module of moduleList) {
    console.log('正在编译:', module)
    if(module !== 'ganwei-iotcenter-login')
    execFileSync('node', [buildFile, module, 'separate'], {})  // 异步执行构建文件，并传入两个参数，module：当前打包模块，separate：当前打包模式（分开打包）
}
