class HtmlInjectPlugin {
    constructor(options) {
        if (!Array.isArray(options)) {
            throw new Error("options should be like [{inject: 'head|body', type: 'js|css', path: '/path-to-file/a.js|a.css'}]")
        }
        for (let item of options) {
            if (!item.inject || !item.type || !item.path) {
                throw new Error("options should be like [{inject: 'head|body', type: 'js|css', path: '/path-to-file/a.js|a.css'}]")
            }
        }
        this.options = options
    }

    apply(compiler) {
        compiler.plugin('entry-option', (param, context) => {
            if (process.env.NODE_ENV == 'development') {
                context().then(res => {
                    this.entry = res
                })
            } else {
                this.entry = context()
            }
        })

        compiler.plugin('compilation', compilation => {
            compilation.plugin('html-webpack-plugin-before-html-processing', (data, cb) => {
                // 解析标签插入点
                let [head, body, _] = data.html.split(/<\/head>|<\/body>/)
                this.options.forEach(item => {
                    if (item.inject == 'head') {
                        if (item.type == 'js') {
                            head += `\r\n    <script type="text/javascript" src="${item.path}"></script>`
                        } else if (item.type == 'css') {
                            head += `\r\n    <link rel="stylesheet" src="${item.path}"></link>`
                        } else {
                            throw new Error('Invalid type')
                        }
                    } else if (item.inject == 'body') {
                        body += `\r\n    <script type="text/javascript" src="${item.path}"></script>`
                    } else {
                        throw new Error('Invalid inject')
                    }
                })

                // 组装入口页面
                data.html = head + '</head>' + body + '</body>' + _

                // 传递页面
                cb(null, data)
            })
            if (process.env.NODE_ENV_ALL) {
                compilation.plugin('html-webpack-plugin-after-html-processing', (data, cb) => {
                    let chunkName
                    if (process.env.NODE_ENV == 'development') {
                        chunkName = Object.keys(data.assets.chunks)[0]
                    } else {
                        chunkName = Object.keys(this.entry)[0]
                    }
                    if (chunkName != 'app') {
                        data.html = data.html.replace(new RegExp(`(src="?${data.assets.chunks[chunkName].entry})`), `defer $1`)
                    }
                    // 传递页面
                    cb(null, data)
                })
            }
        })
    }
}

module.exports = HtmlInjectPlugin
