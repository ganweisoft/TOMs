/**
 * @fileOverview 开发和生产环境的基本配置文件
 */

'use strict'

// Template version: 1.3.1// see http://vuejs-templates.github.io/webpack for documentation.

// 引入nodejs路径模块，用于操作路径
const path = require('path')

// 多模块打包新增配置
const MODULE = process.env.MODULE_ENV || 'undefined'

// 入口模板路径
// const htmlTemplate = `./src/modules/${MODULE}/ .html`

const hostIP = 'https://127.0.0.1:44380/'

module.exports = {
    // 开发环境的一些基本配置
    dev: {
        // Paths
        // 编译输出的二级目录
        assetsSubDirectory: 'static',

        // 编译发布的根目录，可配置为资源服务器域名或者cdn域名
        assetsPublicPath: '/',

        // 需要使用proxyTable代理的接口(用于配置跨域)
        proxyTable: {
            '/api': {
                target: hostIP,
                changeOrigin: true,
                secure: false,
                pathRewrite: {
                    '^/api': '/api'
                },
                headers: {
                    Referer: hostIP
                }
            },
            '/IoT': {
                target: hostIP,
                changeOrigin: true,
                secure: false,
                pathRewrite: {
                    '^/IoT': '/IoT'
                },
                headers: {
                    Referer: hostIP
                }
            },
            '/monitor': {
                target: hostIP,
                changeOrigin: true,
                secure: false,
                ws: true,
                pathRewrite: {
                    '^/monitor': '/monitor'
                },
                headers: {
                    Referer: hostIP
                }
            },
            '/downFileNotify': {
                target: hostIP,
                changeOrigin: true,
                secure: false,
                ws: true,
                pathRewrite: {
                    '^/downFileNotify': '/downFileNotify'
                },
                headers: {
                    Referer: hostIP
                }
            },
            '/workOrder': {
                target: hostIP,
                changeOrigin: true,
                secure: false,
                ws: true,
                pathRewrite: {
                    '^/workOrder': '/workOrder'
                },
                headers: {
                    Referer: hostIP
                }
            },
            '/DownloadFile': {
                target: hostIP,
                changeOrigin: true,
                secure: false,
                ws: true,
                pathRewrite: {
                    '^/DownloadFile': '/DownloadFile'
                },
                headers: {
                    Referer: hostIP
                }
            },
            '/jdsso': {
                target: hostIP,
                changeOrigin: true,
                secure: false,
                ws: true,
                pathRewrite: {
                    '^/jdsso': '/jdsso'
                },
                headers: {
                    Referer: hostIP
                }
            },
            '/eGroupNotify': {
                target: hostIP,
                changeOrigin: true,
                secure: false,
                ws: true,
                pathRewrite: {
                    '^/eGroupNotify': '/eGroupNotify'
                },
                headers: {
                    Referer: hostIP
                }
            },
            '/static/fontList': {
                target: hostIP,
                changeOrigin: true,
                secure: false,
                ws: true,
                pathRewrite: {
                    '^/static/fontList': '/static/fontList'
                },
                headers: {
                    Referer: hostIP
                }
            }
        },

        // Various Dev Server settings
        // 开发时候的访问域名。可以通过环境变量自己设置。
        host: '127.0.0.1',

        // 开发时候的端口。可以通过环境变量PORT设定。如果端口被占用了，会随机分配一个未被使用的端口
        port: 7010,

        // 是否自动打开浏览器
        autoOpenBrowser: true,

        // 下面两个都是浏览器展示错误的方式
        // 在浏览器是否展示错误蒙层
        errorOverlay: true,

        // 是否展示错误的通知
        notifyOnErrors: true,

        // 这个是webpack-dev-servr的watchOptions的一个选项，指定webpack检查文件的方式
        // 因为webpack使用文件系统去获取文件改变的通知。在有些情况下，这个可能不起作用。例如，当使用NFC的时候，
        // vagrant也会在这方面存在很多问题，在这些情况下，使用poll选项（以轮询的方式去检查文件是否改变）可以设定为true
        // 或者具体的数值，指定文件查询的具体周期。
        poll: false,

        // 是否使用eslint loader去检查代码
        useEslint: false,

        // 如果设置为true，在浏览器中，eslint的错误和警告会以蒙层的方式展现。
        showEslintErrorsInOverlay: false,

        /**
         * Source Maps
         */
        // source maps的格式
        devtool: 'cheap-module-eval-source-map', // 'cheap-module-eval-source-map',

        // 指定是否通过在文件名称后面添加一个查询字符串来创建source map的缓存
        cacheBusting: true,

        // 关闭css的source map
        cssSourceMap: true
    },

    // 生产编译环境下的一些基本配置
    build: {
        // html文件的生成的地方,
        index: process.env.NODE_ENV_ALL === 'proAll' ? path.resolve(__dirname, '../dist', MODULE, 'index.html') : path.resolve(__dirname, '../dist/index.html'),
        indexRoute: process.env.NODE_ENV_ALL === 'proAll' ? path.resolve(__dirname, '../src/views/pages/', MODULE, 'index.html') : path.resolve(__dirname, '../index.html'),

        // Paths
        // 编译生成的文件的目录path.resolve(__dirname, '../dist'),
        assetsRoot: process.env.NODE_ENV_ALL === 'proAll' ? path.resolve(__dirname, '../dist', MODULE) : path.resolve(__dirname, '../dist/'),

        // 编译生成的静态文件的目录
        assetsSubDirectory: 'static',

        // 编译发布的根目录，可配置为资源服务器域名或者cdn域名
        assetsPublicPath: './',

        /**
         * Source Maps
         */

        // 下面定义是否生成生产环境的sourcmap，sourcmap是用来debug编译后文件的，通过映射到编译前文件来实现
        productionSourceMap: false,

        // source maps的格式
        devtool: 'cheap-module-source-map',

        // Gzip off by default as many popular static hosts such as
        // Surge or Netlify already gzip all static assets for you.
        // Before setting to `true`, make sure to:
        // npm install --save-dev compression-webpack-plugin
        // 是否开启生产环境的gzip压缩
        productionGzip: true,

        // 开启gzip压缩的文件的后缀名称
        productionGzipExtensions: ['js', 'css'],

        // Run the build command with an extra argument to
        // View the bundle analyzer report after build finishes:
        // `npm run build --report`
        // Set to `true` or `false` to always turn it on or off
        // 如果这个选项是true的话，那么则会在build后，会在浏览器中生成一份bundler报告
        bundleAnalyzerReport: process.env.npm_config_report
    }
}
