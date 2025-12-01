/**
 * @fileOverview 生产环境构建脚本，也就是打包的时候需要的一些包的引入配置信息
 */

'use strict'
let _env = require('../config/prod.env')
process.env.PAGE_ENV = JSON.parse(_env.PAGE_ENV)
process.env.PAGES_ENV = JSON.parse(_env.PAGES_ENV)
require('./check-versions')()

process.env.NODE_ENV = 'production'
const chalk = require('chalk')

if (process.env.NODE_ENV_ALL === 'proAll') {
    // MODULE_ENV用来记录当前打包的模块名称
    process.env.MODULE_ENV = process.argv[2]
    // MODE_ENV用来记录当前打包的模式，total代表整体打包（静态资源在同一个目录下，可以复用重复的文件），separate代表分开打包（静态资源按模块名称分别独立打包，不能复用重复的文件）
    process.env.MODE_ENV = process.argv[3]
    // 如果有传参时，对传入的参数进行检测，如果参数非法，那么停止打包操作
    const checkModule = require('./module-conf').checkModule
    if (process.env.MODULE_ENV !== 'undefined' && !checkModule()) {
        return
    }
}

// 打包开始提示对cli进行输出一个带spinner的文案，告诉用户正在打包中
const ora = require('ora')
// 去除先前的打包,这个模块是用来清除之前的打的包，
// 因为在vue-cli中每次打包会生成不同的hash,每次打包都会生成新的文件，那就不对了，
// 我们要复盖原先的文件，因为hash不同复盖不了，所以要清除
const rm = require('rimraf')
const path = require('path')

// 把webpack模块包给加进入
const webpack = require('webpack')
const config = require('../config')
const webpackConfig = require('./webpack.prod.conf')
// 对cli进行输出一个带spinner的文案，告诉用户正在打包中也可以这样设置多个值
const spinner = ora({
    color: 'green',
    text: 'building for production...正在打包'
})
spinner.start()

rm(path.join(config.build.assetsRoot, config.build.assetsSubDirectory), err => {
    if (err) throw err
    webpack(webpackConfig, (err, stats) => {
        spinner.stop()
        if (err) throw err
        process.stdout.write(
            stats.toString({
                colors: true,
                modules: false,
                children: false, // If you are using ts-loader, setting this to true will make TypeScript errors show up during build.
                chunks: false,
                chunkModules: false
            }) + '\n\n'
        )

        if (stats.hasErrors()) {
            console.log(chalk.red('  Build failed with errors...构建失败，出现错误\n'))
            process.exit(1)
        }

        console.log(chalk.cyan('  Build complete...构建完成\n'))
        console.log(chalk.yellow('  Tip: built files are meant to be served over an HTTP server.\n' + "  Opening index.html over file:// won't work.\n"))
    })
})
