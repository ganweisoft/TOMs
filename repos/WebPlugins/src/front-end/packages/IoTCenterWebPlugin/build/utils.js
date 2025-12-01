/**
 * @file vue开发环境的wepack相关配置文件，主要用来生成css-loader和vue-style-loader配置
 */

'use strict'
// 引入nodejs路径模块，用于操作路径
const path = require('path')
// 引入config目录下的index.js配置文件，获取生产和开发环境的相关属性
const config = require('../config')
// 提取特定文件的插件，比如把css文件提取到一个文件中去
const ExtractTextPlugin = require('extract-text-webpack-plugin')
// 加载package.json文件
const packageConfig = require('../package.json')

// 生成编译输出的二级目录
exports.assetsPath = function (aPath) {
    // 如果是生产环境assetsSubDirectory就是'static'
    const assetsSubDirectory = process.env.NODE_ENV === 'production' ? config.build.assetsSubDirectory : config.dev.assetsSubDirectory

    // path.posix是path模块跨平台的实现（不同平台的路径表示是不一样的），用就是返回一个干净的相对根路径
    // path.join和path.posix.join的区别就是，前者返回的是完整的路径，后者返回的是完整路径的相对根路径
    // 也就是说path.join的路径是C:a/a/b/xiangmu/b，那么path.posix.join就是b
    return path.posix.join(assetsSubDirectory, aPath)
}

// 下面是导出cssLoaders的相关配置
// 为不同的css预处理器提供一个统一的生成方式，也就是统一处理各种css类型的打包问题。
// 这个是为在vue文件中的style中使用的css类型
exports.cssLoaders = function (options) {
    // options如果没值就是空对象
    options = options || {}

    // cssLoader的基本配置
    const cssLoader = {
        loader: 'css-loader',
        // options是loader的选项配置，是用来传递参数给loader的
        options: {
            // minimize表示压缩，如果是生产环境就压缩css代码
            minimize: process.env.NODE_ENV === 'production',
            // 是否开启cssmap，默认是false
            sourceMap: options.sourceMap
        }
    }

    // 编译postcss模块
    const postcssLoader = {
        // 使用postcss-loader来打包postcss模块
        loader: 'postcss-loader',
        // 配置sourcemap
        options: {
            sourceMap: options.sourceMap
        }
    }

    /**
     * 创建loader加载器字符串，结合extract text插件使用
     * @param {string} loader loader的名称
     * @param {string} loaderOptions loader对应的options配置对象
     */
    function generateLoaders(loader, loaderOptions) {
        // 通过usePostCSS 来标明是否使用了postcss
        const loaders = options.usePostCSS ? [cssLoader, postcssLoader] : [cssLoader]

        // 如果该函数传递了单独的loader就加到这个loaders数组里面，这个loader可能是less,sass之类的
        if (loader) {
            // 向loaders的数组中添加该loader对应的加载器
            // 一个很重要的地方就是，一个数组中的loader加载器，是从右向左执行的。
            loaders.push({
                // loader加载器的名称
                loader: loader + '-loader',
                // 对应的加载器的配置对象 Object.assign是es6的方法，主要用来合并对象的，浅拷贝
                options: Object.assign({}, loaderOptions, {
                    sourceMap: options.sourceMap
                })
            })
        }

        // Extract CSS when that option is specified. //在指定该选项时提取CSS
        // (which is the case during production build) //(这是在产品构建期间的情况)
        // 注意这个extract是自定义的属性，可以定义在options里面，
        // 主要作用就是当配置为true就把文件单独提取，false表示不单独提取，这个可以在使用的时候单独配置
        if (options.extract) {
            // fallback这个选项我们可以这样理解
            // webpack默认会按照loaders中的加载器从右向左调用编译各种css类型文件。如果一切顺利，在loaders中的
            // 各个加载器运行结束之后就会把css文件导入到规定的文件中去，如果不顺利，则继续使用vue-style-loader来处理css文件
            return ExtractTextPlugin.extract({
                use: loaders,
                // 加入相对位置，可以修正静态资源的url，不然background-imgage 的路径引入有问题
                publicPath: '../../',
                fallback: 'vue-style-loader'
            })
        } else {
            // 如果没有提取行为，则最后再使用vue-style-loader处理css
            return ['vue-style-loader'].concat(loaders)
        }
    }

    function generateSassResourceLoader() {
        var loaders = [
            cssLoader,
            'sass-loader',
            {
                loader: 'sass-resources-loader',
                options: {
                    // 多个文件时用数组的形式传入，单个文件时可以直接使用 path.resolve(__dirname, '../static/style/common.scss'
                    resources: path.resolve(__dirname, '../node_modules/gw-base-style-plus/style.scss')
                }
            }
        ]
        if (options.extract) {
            return ExtractTextPlugin.extract({
                use: loaders,
                fallback: 'vue-style-loader'
            })
        } else {
            return ['vue-style-loader'].concat(loaders)
        }
    }

    return {
        // css-loader
        css: generateLoaders(),
        // postcss-loader
        postcss: generateLoaders(),
        // less-loader
        less: generateLoaders('less'),
        // sass-loader 后面的选项表明sass使用的是缩进的愈发
        // sass: generateLoaders('sass', { indentedSyntax: true }),
        sass: generateSassResourceLoader(),
        scss: generateSassResourceLoader(),
        // scss-loader
        // scss: generateLoaders('sass'),
        // stylus-loader stylus文件有两种后缀名.stylus和styl
        stylus: generateLoaders('stylus'),
        // stylus-loader
        styl: generateLoaders('stylus')
    }
}

// Generate loaders for standalone style files (outside of .vue) //为独立样式文件创建加载器配置(.vue之外)
// 下面这个主要处理import这种方式导入的文件类型的打包，上面的exports.cssLoaders是为这一步服务的
exports.styleLoaders = function (options) {
    // 保存加载器配置的变量
    const output = []
    // 获取所有css文件类型的loaders
    const loaders = exports.cssLoaders(options)

    for (const extension in loaders) {
        // 把每一种文件的laoder都提取出来
        const loader = loaders[extension]
        // 生成对应的loader配置
        output.push({
            test: new RegExp('\\.' + extension + '$'),
            use: loader
        })
    }

    return output
}

// 当编译出错的时候，根据config.dev.notifyOnErrors来确定是否需要在桌面右上角显示错误通知框
exports.createNotifierCallback = () => {
    // node-notifier是一个跨平台的包，以类似浏览器的通知的形式展示信息。
    const notifier = require('node-notifier')

    return (severity, errors) => {
        // 只展示错误的信息
        if (severity !== 'error') {
            return
        }

        const error = errors[0]
        const filename = error.file && error.file.split('!').pop()

        // 需要展示的错误信息的内容
        notifier.notify({
            // 通知的标题
            title: packageConfig.name,
            // 通知的主体内容
            message: severity + ': ' + error.name,
            // 副标题
            subtitle: filename || '',
            // 通知展示的icon
            icon: path.join(__dirname, 'index-logo-src-small.png')
        })
    }
}
