/**
 * @fileOverview 生产环境的核心配置文件
 */

'use strict'
// 引入nodejs路径模块，用于操作路径
const path = require('path')

// 加载utils配置文件，用来解决css相关文件loader
const utils = require('./utils')
// 引入webpack模块
const webpack = require('webpack')
// 引入config目录下的index.js配置文件，获取生产和开发环境的相关属性
const config = require('../config')
// 引入webpack-merge模块。这个模块用于把多个webpack配置合并成一个配置，后面的配置会覆盖前面的配置。
const merge = require('webpack-merge')
// 引入webpack的基本设置，这个设置文件包含了开发环境和生产环境的一些公共配置
const baseWebpackConfig = require('./webpack.base.conf')

// 这个模块主要用于在webpack中拷贝文件和文件夹
const CopyWebpackPlugin = require('copy-webpack-plugin')
// 用于生成html文件的插件
const HtmlWebpackPlugin = require('html-webpack-plugin')
const BuildInfo = require('./explanatory.note.js')
// 这个插件主要是用于将入口中所有的chunk，移到独立的分离的css文件中
const ExtractTextPlugin = require('extract-text-webpack-plugin')
// 这个插件主要是用于压缩css模块的
const OptimizeCSSPlugin = require('optimize-css-assets-webpack-plugin')
// 这个插件主要是用于压缩js文件的
const UglifyJsPlugin = require('uglifyjs-webpack-plugin')
// // 往html增加版本信息
// const addHtmlInfo = require('./addHtmlInfo.js')

// 将scss转成css
const resetCssPlugin = require('./reset-css-plugin')

const HtmlWebpackTagsPlugin = require('html-webpack-tags-plugin')
const moduleList = require('./module-conf');
// // 往html增加版本信息
// if (process.env.NODE_ENV_ALL == 'proFrame') {
//     addHtmlInfo(path.resolve(`./index.html`))
// } else {
//     addHtmlInfo(path.resolve(`${process.env.PAGE_ENV}/${moduleList.getModuleToBuild()[0]}/index.html`))
// }
let arr = [];
arr.push(
    new HtmlWebpackPlugin({
        filename: `index.html`,
        template: process.env.NODE_ENV_ALL == 'proAll' ? `${process.env.PAGE_ENV}/${moduleList.getModuleToBuild()[0]}/index.html` : 'index.html',
        inject: true,
        minify: {
            removeComments: false,
            collapseWhitespace: true,
            removeAttributeQuotes: true
        },
        chunks: ['manifest', 'vendor', 'app', process.env.NODE_ENV_ALL == 'proAll' ? moduleList.getModuleToBuild()[0] : ''],
        chunksSortMode: 'dependency',
        buildInfo: JSON.stringify(BuildInfo)
    })
);

// 引入用于生产环境的一些基本变量
const env = require('../config/prod.env')
env.NODE_ENV_ALL = `'${process.env.NODE_ENV_ALL}'`

const staticCopy = new CopyWebpackPlugin([
    {
        // 定义要拷贝的资源的源目录
        from: path.resolve(__dirname, '../static'),
        // 定义要拷贝的资源的目标目录
        to: config.build.assetsSubDirectory,
        ignore: ['.git', '*.scss']
    }])

// 合并公共配置和生产环境独有的配置并返回一个用于生产环境的webpack配置文件
const webpackConfig = merge(baseWebpackConfig, {
    // 用于生产环境的一些loader配置
    module: {
        rules: utils.styleLoaders({
            sourceMap: config.build.productionSourceMap,
            // 在生产环境中使用extract选项，这样就会把thunk中的css代码抽离到一份独立的css文件中去
            extract: true,
            usePostCSS: true
        })
    },
    // 配置生产环境中使用的source map的形式。在这里，生产环境使用的是#source map的形式
    devtool: config.build.productionSourceMap ? config.build.devtool : false,
    output: {
        // 加入相对位置，可以修正静态资源的url
        publicPath: './',
        // 打包后的文件放在dist目录里面
        path: config.build.assetsRoot,
        // 文件名称使用 static/js/[name].[chunkhash].js, 其中name就是main,chunkhash就是模块的hash值，长度为20，是用于浏览器缓存的
        filename: utils.assetsPath('js/[name].[chunkhash].js'),
        // chunkFilename是非入口模块文件，也就是说filename文件中引用了chunckFilename
        chunkFilename: utils.assetsPath('js/[name].[chunkhash].js')
    },
    externals: {
        "gw-base-components-plus/treeV2": 'treeV2'
    },
    plugins: [
        new resetCssPlugin(false),
        // http://vuejs.github.io/vue-loader/en/workflow/production.html
        new webpack.DefinePlugin({
            'process.env': env
        }),
        // // 压缩javascript的插件
        new UglifyJsPlugin({
            // 压缩js的时候的一些基本配置
            uglifyOptions: {
                // 配置压缩的行为
                compress: {
                    // 在删除未使用的变量等时，显示警告信息，默认就是false
                    warnings: false
                }
            },
            // 使用 source map 将错误信息的位置映射到模块（这会减慢编译的速度）
            // 而且这里不能使用cheap-source-map
            sourceMap: config.build.productionSourceMap,
            // 使用多进程并行运行和文件缓存来提高构建速度
            parallel: true
        }),
        // 提取css文件到一个独立的文件中去
        new ExtractTextPlugin({
            // 提取之后css文件存放的地方
            // 其中[name]和[contenthash]都是占位符
            // [name]就是指模块的名称
            // [contenthash]根据提取文件的内容生成的 hash
            filename: utils.assetsPath('css/[name].[contenthash].css'),

            // 从所有额外的 chunk(additional chunk) 提取css内容
            // （默认情况下，它仅从初始chunk(initial chunk) 中提取）
            // 当使用 CommonsChunkPlugin 并且在公共 chunk 中有提取的 chunk（来自ExtractTextPlugin.extract）时
            // 这个选项需要设置为true
            allChunks: true
        }),
        // 使用这个插件压缩css，优化、最小化css代码，如果只简单使用extract-text-plugin可能会造成css重复
        new OptimizeCSSPlugin({
            // 这个选项的所有配置都会传递给cssProcessor
            // cssProcessor使用这些选项决定压缩的行为
            cssProcessorOptions: config.build.productionSourceMap ? { safe: true, map: { inline: false } } : { safe: true }
        }),
        // 创建一个html文件
        ...arr,
        // 根据模块的相对路径生成一个四位数的hash作为模块id
        new webpack.HashedModuleIdsPlugin(),
        // webpack2处理过的每一个模块都会使用一个函数进行包裹
        // 这样会带来一个问题：降低浏览器中JS执行效率，这主要是闭包函数降低了JS引擎解析速度。
        // webpack3中，通过下面这个插件就能够将一些有联系的模块，
        // 放到一个闭包函数里面去，通过减少闭包函数数量从而加快JS的执行速度。
        new webpack.optimize.ModuleConcatenationPlugin(),
        // 这个插件用于提取多入口chunk的公共模块
        // 通过将公共模块提取出来之后，最终合成的文件能够在最开始的时候加载一次
        // 然后缓存起来供后续使用，这会带来速度上的提升。
        new webpack.optimize.CommonsChunkPlugin({
            // 这是 common 模块 的名称
            name: 'vendor',
            // 把所有从node_modules中引入的文件提取到vendor中，即抽取库文件
            minChunks (module) {
                // any required modules inside node_modules are extracted to vendor
                return module.resource && /\.js$/.test(module.resource) && module.resource.indexOf(path.join(__dirname, '../node_modules')) === 0 && !module.resource.includes('gw-base-frame-config')
            }
        }),
        // 为了将项目中的第三方依赖代码抽离出来，官方文档上推荐使用这个插件，当我们在项目里实际使用之后，
        // 发现一旦更改了 app.js 内的代码，vendor.js 的 hash 也会改变，那么下次上线时，
        // 用户仍然需要重新下载 vendor.js 与 app.js——这样就失去了缓存的意义了。所以第二次new就是解决这个问题的
        // 参考：https://github.com/DDFE/DDFE-blog/issues/10
        new webpack.optimize.CommonsChunkPlugin({
            name: 'manifest',
            minChunks: Infinity
        }),
        // This instance extracts shared chunks from code splitted chunks and bundles them
        // in a separate chunk, similar to the vendor chunk
        // see: https://webpack.js.org/plugins/commons-chunk-plugin/#extra-async-commons-chunk
        new webpack.optimize.CommonsChunkPlugin({
            name: 'index',
            async: 'vendor-async',
            children: true,
            minChunks: 3
        }),

        // copy custom static assets
        // 拷贝静态资源到build文件夹中 process.env.NODE_ENV_ALL === 'proAll'?'':
        process.env.NODE_ENV_ALL === 'proAll' ? new CopyWebpackPlugin() : staticCopy,

        new HtmlWebpackTagsPlugin({
            usePublicPath: false,
            links: [
                // '/static/themes/reset.css',
            ],
            append: false
        }),
    ]
})

// 如果开启了生产环境的gzip，则利用插件将构建后的产品文件进行压缩
if (config.build.productionGzip) {
    // 一个用于压缩的webpack插件
    const CompressionWebpackPlugin = require('compression-webpack-plugin')

    webpackConfig.plugins.push(
        new CompressionWebpackPlugin({
            // 目标资源的名称
            // [path]会被替换成原资源路径
            // [query]会被替换成原查询字符串
            asset: '[path].gz[query]',
            //filename: '[path].gz[query]',
            // gzip算法
            // 这个选项可以配置成zlib模块中的各个算法
            // 也可以是(buffer, cb) => cb(buffer)
            algorithm: 'gzip',
            // 处理所有匹配此正则表达式的资源
            test: new RegExp('\\.(' + config.build.productionGzipExtensions.join('|') + ')$'),
            // 只处理比这个值大的资源
            threshold: 10240,
            // 只有压缩率比这个值小的资源才会被处理
            minRatio: 0.8
        })
    )
}

// 如果启动了report，则通过插件给出webpack构建打包后的产品文件分析报告
if (config.build.bundleAnalyzerReport) {
    // 打包编译后的文件打印出详细的文件信息，vue-cli默认把这个禁用了，可以自行配置
    const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin
    webpackConfig.plugins.push(new BundleAnalyzerPlugin())
}

module.exports = webpackConfig