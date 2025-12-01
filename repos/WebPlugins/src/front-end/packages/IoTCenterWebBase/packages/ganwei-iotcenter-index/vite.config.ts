
import vue from '@vitejs/plugin-vue'
import path, { resolve } from 'path'
import { visualizer } from "rollup-plugin-visualizer"
import AutoImport from 'unplugin-auto-import/vite'
import { defineConfig, loadEnv } from 'vite'
import eslintPlugin from 'vite-plugin-eslint' //导入包
import { createHtmlPlugin } from 'vite-plugin-html'

import getEntryPath from "./build/enteryJson.js";
import viteAddInfoHtml from './build/vite-addInfo-html'
import stats from "./build/vite-plugin-stats-html"
import { name } from "./package.json"
const htmlParams = {
    minify: true,
    pages: getEntryPath,
}

import indexPort from '../../configuration/moduleConfiguration.json'
const VITE_APP_INDEX_PORT = indexPort.addressMapping['ganwei-iotcenter-index']
export default ({ mode }) => {
    const VITE_APP_TARGET_URL = indexPort.proxyTarget || loadEnv(mode, process.cwd()).VITE_APP_TARGET_URL
    const VITE_APP_THIS_URL = loadEnv(mode, process.cwd()).VITE_APP_THIS_URL
    const port = Number(indexPort.addressMapping?.[name].split(':').pop());
    const VITE_APP_PORT = port || Number(loadEnv(mode, process.cwd()).VITE_APP_PORT)

    return defineConfig({
        root: __dirname, // 项目根目录（index.html 文件所在的位置）,
        base: '',
        mode: 'development', // 模式
        publicDir: 'public', // 静态资源服务的文件夹
        // cacheDir: path.resolve(__dirname, '/node_modules/.vite'), // 存储缓存文件的目录
        define: {
            _INTLIFY_JIT_COMPILATION__: true,
            'process.env.VITE_APP_INDEX_PORT': `"${VITE_APP_INDEX_PORT}"`
        },
        plugins: [
            vue(),
            visualizer({
                open: false, //注意这里要设置为true，否则无效
                gzipSize: true,
            }),
            //plus按需引入
            AutoImport(),
            // 增加下面的配置项,这样在运行时就能检查eslint规范
            eslintPlugin({
                include: ['src/**/*.js', 'src/**/*.vue', 'src/*.js', 'src/*.vue'],
                exclude: ['./node_modules/**', './src/types/**', '/static/**', './src/components/qrcodeReader/**', './src/components/vue-qrcode-reader/**'],
                cache: false
            }),
            createHtmlPlugin(htmlParams),
            stats({
                filename: "stats.html",
            }),
            viteAddInfoHtml()
        ],
        resolve: {
            alias: [
                {
                    find: "@",
                    replacement: resolve(__dirname, "src")
                },
                {
                    find: "@components",
                    replacement: resolve(__dirname, "src/components")
                },
                {
                    find: "@static",
                    replacement: resolve(__dirname, "static")
                },
                {
                    find: "@assets",
                    replacement: resolve(__dirname, "src/assets")
                },
                {
                    find: "@views",
                    replacement: resolve(__dirname, "src/views")
                }
            ],
            // 忽略后缀名的配置选项, 添加 .vue 选项时要记得原本默认忽略的选项也要手动写入
            extensions: ["ts", '.mjs', '.js', '.json', '.vue']
        },
        // transpileDependencies: ["webrtc-adapter"],
        css: {
            // postcss: {
            //     plugins: [
            //         autoprefixer({
            //             overrideBrowserslist: ['Android 4.1', 'iOS 7.1', 'Chrome > 31', 'ff > 31', 'ie >= 8'],
            //         }),
            //         // postCssPxToRem({
            //         //   rootValue: 75, // 75表示750设计稿，37.5表示375设计稿
            //         //   propList: ['*'], // 需要转换的属性，这里选择全部都进行转换
            //         //   selectorBlackList: ['norem'], // 过滤掉norem-开头的class，不进行rem转换
            //         // }),
            //     ],
            // },
            preprocessorOptions: {
                // 全局样式引入
                // scss: {
                //     additionalData: '@use "./src/assets/css/style.scss" as *;',
                //     javascriptEnabled: true
                // }
            }
        },
        server: {
            headers: {
                "Access-Control-Allow-Origin": "*"
            },
            cors: {
                credentials: true,
                methods: "PUT,POST,GET,DELETE,OPTIONS",
                origin: VITE_APP_INDEX_PORT
            },
            host: VITE_APP_THIS_URL,        // 指定服务器应该监听哪个 IP 地址
            port: VITE_APP_PORT,               // 端口
            strictPort: true, // 若端口已被占用,尝试下移一格端口
            https: false, // 启用 TLS + HTTP/2
            open: false,
            proxy: {
                '/api': {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    rewrite: (path) => path.replace(/^\/api/, '/api'),
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
                '/APP': {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
                '/IoT': {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    rewrite: (path) => path.replace(/^\/IoT/, '/IoT'),
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
                '/monitor': {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    ws: true,
                    rewrite: (path) => path.replace(/^\/monitor/, '/monitor'),
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
                '/downFileNotify': {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    ws: true,
                    rewrite: (path) => path.replace(/^\/downFileNotify/, '/downFileNotify'),
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
                '/workOrder': {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    ws: true,
                    rewrite: (path) => path.replace(/^\/workOrder/, '/workOrder'),
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
                '/DownloadFile': {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    ws: true,
                    rewrite: (path) => path.replace(/^\/DownloadFile/, '/DownloadFile'),
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
                '/jdsso': {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    ws: true,
                    rewrite: (path) => path.replace(/^\/jdsso/, '/jdsso'),
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
                '/eGroupNotify': {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    ws: true,
                    rewrite: (path) => path.replace(/^\/eGroupNotify/, '/eGroupNotify'),
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
                "/realTimeChange": {
                    target: VITE_APP_TARGET_URL,
                    changeOrigin: true,
                    secure: false,
                    ws: true,
                    rewrite: path => path.replace(/^\/realTimeChange/, "/realTimeChange"),
                    headers: {
                        Referer: VITE_APP_TARGET_URL
                    }
                },
            },
            force: true, // 强制使依赖预构建
            hmr: { // 禁用或配置 HMR 连接

            },
            watch: {},
            fs: {
                strict: false, // 限制为工作区 root 路径以外的文件的访问
                allow: [], // 限制哪些文件可以通过 /@fs/ 路径提供服务
                deny: ['.env', '.env.*', '*.{pem,crt}'], // 用于限制 Vite 开发服务器提供敏感文件的黑名单
            }
        },
        build: {
            sourcemap: false, // 构建后是否生成 source map 文件
            chunkSizeWarningLimit: 1500, // 规定触发警告的 chunk(文件块) 大小
            cssCodeSplit: false,
            // brotliSize: false,  // 禁用压缩大小报告
            minify: 'esbuild',
            terserOptions: {
                compress: {
                    drop_console: true, // 生产环境去除console
                    drop_debugger: true, // 生产环境去除debugger
                },
            },
            rollupOptions: {  // 自定义底层的 Rollup 打包配置  ('[name]'.includes("ganwe")?'[name]/':'')
                output: {
                    // dir: path.resolve(__dirname, `../../dist/${name}`),
                    dir: path.resolve(__dirname, `../../dist/`),
                    assetFileNames: (assetInfo) => {
                        if (!assetInfo.name) {
                            throw Error("assetInfo.name is empty")
                        }
                        const info = assetInfo.name.split(".");
                        let extType = info[info.length - 1];
                        if (/png|jpe?g|svg|gif|tiff|bmp|ico/i.test(extType)) {
                            extType = 'images';
                        }
                        return `assets/${extType}/[name]-[hash][extname]`;
                    },
                    chunkFileNames: 'assets/js/[hash].js',
                    entryFileNames: 'assets/js/[name]-[hash].js',
                    compact: false,
                    manualChunks(id) {
                        if (id.includes('node_modules')) {
                            return id.toString().split('node_modules/')[1].split('/')[0].toString();
                        }
                    }

                },
            },
            emptyOutDir: false,
        }
    })
}
