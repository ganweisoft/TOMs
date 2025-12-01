/* eslint-disable @typescript-eslint/naming-convention */
module.exports = {
    globals: {
        myJavaFun: 'readonly'
    },
    env: {
        browser: true,
        es2021: true,
        node: true
    },
    extends: [
        'eslint:recommended',
        'plugin:vue/vue3-essential',
        'plugin:@typescript-eslint/recommended'
        // 'plugin:prettier/recommended' // 解决ESlint和Prettier冲突
    ],
    overrides: [],
    parser: 'vue-eslint-parser',
    parserOptions: {
        ecmaVersion: 'latest',
        sourceType: 'module',
        parser: '@typescript-eslint/parser',
        ecmaFeatures: {
            jsx: true
        }
    },
    plugins: ['vue', '@typescript-eslint', 'simple-import-sort'],

    /*
   下面这些rules是用来设置从插件来的规范代码的规则，使用必须去掉前缀eslint-plugin-
    主要有如下的设置规则，可以设置字符串也可以设置数字，两者效果一致
    "off" -> 0 关闭规则
    "warn" -> 1 开启警告规则
    "error" -> 2 开启错误规则
  */
    // 参数说明：
    // 参数1 ： 错误等级
    // 参数2 ： 处理方式
    rules: {
        "simple-import-sort/imports": [
            "error",
            {
                groups: [
                    // node_modulse packages
                    ["^vue", "^element", "^@(?!ganwei)\\w", '^@?\\w'],
                    // @ganwei private packages
                    ["^@ganwei"],
                    // alias or relative path
                    ["^@/", "'^\\.\\.(?!/?$)', '^\\.\\./?$'"]
                ]
            }
        ],
        "simple-import-sort/exports": "error",
        // allow async-await
        'generator-star-spacing': 'off',
        // allow debugger during development
        'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',

        'prefer-promise-reject-errors': 0,
        'space-unary-ops': 0,
        'no-unused-expressions': 0,
        'no-useless-return': 0,
        'standard/no-callback-literal': 0,
        'import/first': 0,
        'import/export': 0,
        'no-mixed-operators': 0,
        'no-use-before-define': 0,
        'no-useless-escape': 0,
        // 允许使用分号
        semi: [0, 'never'],
        // 取消文件结尾处需要换行符
        'eol-last': 0,
        // 允许使用==
        eqeqeq: 0,
        // 缩进使用不做限制
        indent: 0,
        // 允许使用tab
        'no-tabs': 0,
        // 函数圆括号之前没有空格
        //'space-before-function-paren': [2, "never"],
        'space-before-function-paren': 0, //这句话表示在函数后可以不加空格
        // 不要求块内空格填充格式
        'padded-blocks': 0,
        // 不限制变量一起声明
        'one-var': 0,
        // 开发模式允许使用console
        // 'no-console': process.env.NODE_ENV === 'production' ? 2 : 0,
        // 条件语句中复制操作符需要用圆括号括起来
        'no-cond-assign': [2, 'except-parens'],
        // 允许使用条件表达式使用常量
        'no-constant-condition': 0,
        // 单行可忽略大括号，多行不可忽略
        curly: [2, 'multi-line'],
        // 不允许使用var变量
        'no-var': 2,
        // 不允许出现多个空格
        'no-multi-spaces': [
            'error',
            {
                ignoreEOLComments: true
            }
        ],
        camelcase: 0,
        // 对象字面量的键值空格风格
        'key-spacing': 2,
        // if语句包含一个return语句， else就多余
        'no-else-return': 2,
        // 建议将经常出现的数字提取为变量
        'no-magic-numbers': [
            0,
            {
                ignoreArrayIndexes: true
            }
        ],
        // 不允许重复声明变量
        'no-redeclare': [
            2,
            {
                builtinGlobals: true
            }
        ],
        // 立即执行函数风格
        'wrap-iife': [2, 'inside'],
        // 不允许圆括号中出现空格
        'space-in-parens': [2, 'never'],
        // 确保运算符周围有空格
        'space-infix-ops': 2,
        // 强制点号与属性同一行
        'dot-location': [2, 'property'],
        // 强制单行代码使用空格
        'block-spacing': [2, 'always'],
        // 约束for-in使用hasOwnProperty判断
        'guard-for-in': 0,
        // 采用one true brace style大括号风格
        'brace-style': [
            2,
            '1tbs',
            {
                allowSingleLine: true
            }
        ],
        // 统一逗号周围空格风格
        'comma-spacing': [
            2,
            {
                before: false,
                after: true
            }
        ],
        // 禁止出现多个空行
        'no-multiple-empty-lines': [
            2,
            {
                max: 1,
                maxEOF: 2
            }
        ],
        // 允许箭头函数不使用圆括号
        'arrow-parens': 0,
        // 规范generator函数的使用
        //'generator-star-spacing': [2, {'before': false, 'after': true}],
        // 要求在块级
        'lines-around-comment': [
            2,
            {
                beforeBlockComment: true,
                afterBlockComment: false,
                beforeLineComment: false,
                afterLineComment: false
            }
        ],
        "@typescript-eslint/no-this-alias": ["off"],
        'vue/multi-word-component-names': 'off', //关闭组件命名规则,
        'new-cap': ['error', { "newIsCap": true, capIsNew: false }]
    }
}
