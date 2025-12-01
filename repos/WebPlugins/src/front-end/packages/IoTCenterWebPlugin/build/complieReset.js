const sass = require('node-sass')
const fs = require('fs')
const path = require('path')
const zlib = require('zlib')
const { pipeline } = require('stream')

const source = path.resolve(__dirname, '../static/css/reset.scss')
const target = path.resolve(__dirname, '../dist/static/css/reset.css')
const targetGZ = path.resolve(__dirname, '../dist/static/css/reset.css.gz')
const gzip = zlib.createGzip()

const build = function (noPress) {
    sass.render(
        {
            file: source,
            outputStyle: 'compressed'
        },
        function (error, result) {
            if (!error) {
                makeDir(() => {
                    fs.writeFile(target, result.css, (err) => {
                        if (!noPress) {
                            pipeline(fs.createReadStream(target), gzip, fs.createWriteStream(targetGZ), err => {
                                if (err) {
                                    console.error('An error occurred:', err)
                                    process.exitCode = 1
                                }
                            })
                        }
                    })
                })
            }
        }
    )

    const makeDir = (cb) => {
        if (!fs.existsSync(path.resolve(`./dist`))) {
            fs.mkdirSync(path.resolve(`./dist`))
        }
        if (!fs.existsSync(path.resolve(`./dist/static`))) {
            fs.mkdirSync(path.resolve(`./dist/static`))
        }

        if (!fs.existsSync(path.resolve(`./dist/static/css`))) {
            fs.mkdirSync(path.resolve(`./dist/static/css`))
        }
        cb()
    }
}


module.exports = { build }
