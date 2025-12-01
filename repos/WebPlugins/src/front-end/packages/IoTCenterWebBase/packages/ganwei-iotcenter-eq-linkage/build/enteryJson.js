const pageEntry = [{
    filename: 'index', // filename 默认是template文件名，就是index.html
    entry: `/src/main`,
    output: `../../dist`,
    template: `index.html`,
    chunks: ['chunk-vendors', 'chunk-common', 'index']
}];
export default pageEntry;

export const externalTag = [
    {
        href: `/static/css/element-plus/index.css`,
        mode: 'dev',
        tag: 'link',
        injectTo: "head-prepend"
    },
    {
        href: `/static/css/reset-6-1.css`,
        mode: 'dev',
        tag: 'link',
        injectTo: "head-prepend"
    },
    {
        src: `/static/themes/default-theme-6-1.js`,
        mode: 'dev',
        tag: "script",
        injectTo: "head"
    },
]

export const generateCssLinkTag = (href, tag = 'link', injectTo = 'head-prepend') => {
    return {
        tag,
        attrs: { "rel": "stylesheet", "type": "text/css", "href": href },
        injectTo
    }
}

export const generateScriptTag = (src, tag = 'script', injectTo = 'head') => {
    return {
        tag,
        attrs: { "src": src },
        injectTo
    }
}

export const generateHtmlTag = (option) => {
    if (option.tag === 'link') {
        const { href, injectTo, tag } = option
        return generateCssLinkTag(href, tag, injectTo)
    }
    if (option.tag === 'script') {
        const { src, injectTo, tag } = option
        return generateScriptTag(src, tag, injectTo)
    }
    return {}
}

export function externalTagPlugin (options) {
    let _options = options;
    return {
        name: 'external-css',
        transformIndexHtml: {
            enforce: 'post',
            transform (html, ctx) {
                return _options.map(option => generateHtmlTag(option))
            }
        }
    }
}
