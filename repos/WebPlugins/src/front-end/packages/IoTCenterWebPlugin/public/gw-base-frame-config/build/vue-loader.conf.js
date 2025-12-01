/**
 * @fileOverview 处理.vue文件的配置文件
 */

'use strict'
// 加载utils配置文件，用来解决css相关文件loader
const utils = require('./utils')
// 引入config目录下的index.js配置文件，获取生产和开发环境的相关属性
const config = require('../config')
// 判断当前是否生产环境
const isProduction = process.env.NODE_ENV === 'production'
// 根据不同的环境，引入不同的source map配置文件
const sourceMapEnabled = isProduction ? config.build.productionSourceMap : config.dev.cssSourceMap

module.exports = {
    // vue文件中的css loader配置
    loaders: utils.cssLoaders({
        sourceMap: sourceMapEnabled,
        // 这一项是自定义配置项，设置为true表示生成单独样式文件；生产环境下就会把css文件抽取到一个独立的文件中
        extract: isProduction
    }),
    // css source map文件的配置
    cssSourceMap: sourceMapEnabled,
    // css source map文件缓存控制变量
    cacheBusting: config.dev.cacheBusting,
    transformToRequire: {
        video: ['src', 'poster'],
        source: 'src',
        img: 'src',
        image: 'xlink:href'
    }
}
