
var path = require('path')
var webpack = require('webpack')
const merge = require('webpack-merge');
const components = require('./components.json')

const ProgressBarPlugin = require('progress-bar-webpack-plugin');
// const VueLoaderPlugin = require('vue-loader/lib/plugin');
// const TerserPlugin = require('terser-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');


const basePath = path.resolve(__dirname, '../')
let entries = {}
Object.keys(components).forEach(key => {
    entries[key] = path.join(basePath, '', components[key])
})

module.exports = merge({}, {
    entry: entries,
    context: path.resolve(__dirname, '../'),
    output: {
        path: path.resolve(process.cwd(), './dist/static/lib'),
        publicPath: '/dist/',
        filename: '[name]/[name].js',
        chunkFilename: '[id].js',
        libraryTarget: 'umd',
        libraryExport: 'default',
        library: '[name]',
        umdNamedDefine: true
    },

    externals: {
        vue: 'Vue',
        'element-ui': 'ELEMENT',
        'vue-router': 'VueRouter',
        axios: 'axios',
        vuex: 'Vuex',
        'vue-i18n': 'VueI18n',
        echarts: 'echarts',
        'jsencrypt/bin/jsencrypt.min.js': 'JSEncrypt',
        '@aspnet/signalr': 'signalR',
        jquery: 'jQuery'
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                loader: 'babel-loader',
                exclude: /(node_modules)/
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader',
                options: {
                    compilerOptions: {
                        preserveWhitespace: false
                    }
                }
            },
            {
                test: /\.css$/,
                use: ExtractTextPlugin.extract({
                    fallback: "style-loader",
                    use: "css-loader"
                })
            },
            {
                test: /\.scss$/,
                use: ExtractTextPlugin.extract({
                    fallback: "style-loader",
                    use: [{
                        loader: "css-loader"
                    }, {
                        loader: "sass-loader"
                    }]
                })
            },
            {
                test: /\.less$/,
                use: ExtractTextPlugin.extract({
                    fallback: "style-loader",
                    use: [{
                        loader: "css-loader"
                    }, {
                        loader: "less-loader"
                    }]
                })
            }
        ]
    },
    plugins: [
        new ProgressBarPlugin(),
        new ExtractTextPlugin('[name]/[name].css')
        // new VueLoaderPlugin()
    ]


});
