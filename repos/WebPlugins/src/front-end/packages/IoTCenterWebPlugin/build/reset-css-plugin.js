const toCss = require('./complieReset.js')
class resetCssPlugin {
    constructor(noPress) {
        this.noPress = noPress
        this.time = 1
    }
    apply (compiler) {
        try {
            const emit = (compilation, cb) => {
                toCss.build(this.noPress)
                cb();
            }
            if (compiler.hooks) {
                compiler.hooks.emit.tap('resetCssPlugin', (compilation) => {
                    toCss.build(this.noPress)
                })
            } else {
                compiler.plugin('emit', emit);
            }
            compiler.plugin('watch-run', (watching, callback) => {
                Object.keys(watching.compiler.watchFileSystem.watcher.mtimes).forEach(fileName => {

                    if (fileName.includes('reset.scss')) {
                        toCss.build(this.noPress)
                        // this.changeResetCSS(compiler)
                        // compiler.apply('emit', compiler)
                    }
                })
                callback()
            });
        } catch (error) {
            console.log(error)

        }

    }
    changeResetCSS (compiler) {
        const emit = (compilation, cb) => {
            toCss.build(this.noPress)
            cb();
        }

        if (compiler.hooks) {
            compiler.hooks.emit.tap('emit', (compilation) => {
                toCss.build(this.noPress)
            })
        } else {
            compiler.plugin('emit', emit);
        }
    }
}

module.exports = resetCssPlugin