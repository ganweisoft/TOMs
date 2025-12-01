// https://eslint.org/docs/user-guide/configuring

module.exports = {
    //此项是用来告诉eslint找当前配置文件不能往父级查找
    root: true,
    //此项是用来指定javaScript语言类型和风格，sourceType用来指定js导入的方式，默认是script，此处设置为module，指某块导入方式
    parserOptions: {
        //此项是用来指定eslint解析器的，解析器必须符合规则，babel-eslint解析器是对babel解析器的包装使其与ESLint解析
        parser: 'babel-eslint'
    },
    // 此项指定环境的全局变量，下面的配置指定为浏览器环境
    env: {
        browser: true,
        jquery: true
    },
    // 此项是用来配置标准的js风格，就是说写代码的时候要规范的写
    extends: [
        // https://github.com/vuejs/eslint-plugin-vue#priority-a-essential-error-prevention
        // consider switching to `plugin:vue/strongly-recommended` or `plugin:vue/recommended` for stricter rules.
        'plugin:vue/essential',
        // https://github.com/standard/standard/blob/master/docs/RULES-en.md
        'standard' //扩展，可以通过字符串或者一个数组来扩展规则
    ],
    // required to lint *.vue files
    // 此项是用来提供插件的，插件名称省略了eslint-plugin-，下面这个配置是用来规范html的
    plugins: ['vue'],
    // add your custom rules here
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
        // debugger使用
        //'no-debugger': process.env.NODE_ENV === 'production' ? 2 : 0,
        // 开发模式允许使用console
        'no-console': 0,
        // 条件语句中复制操作符需要用圆括号括起来
        'no-cond-assign': [2, 'except-parens'],
        // 允许使用条件表达式使用常量
        'no-constant-condition': 0,
        // 单行可忽略大括号，多行不可忽略
        curly: [2, 'multi-line'],
        // 不允许使用var变量
        'no-var': 2,
        // 不允许出现多个空格
        'no-multi-spaces': ['error', { ignoreEOLComments: true }],
        camelcase: 0,
        // 对象字面量的键值空格风格
        'key-spacing': 2,
        // if语句包含一个return语句， else就多余
        'no-else-return': 2,
        // 建议将经常出现的数字提取为变量
        'no-magic-numbers': [0, { ignoreArrayIndexes: true }],
        // 不允许重复声明变量
        'no-redeclare': [2, { builtinGlobals: true }],
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
        'brace-style': [2, '1tbs', { allowSingleLine: true }],
        // 统一逗号周围空格风格
        'comma-spacing': [2, { before: false, after: true }],
        // 禁止出现多个空行
        'no-multiple-empty-lines': [2, { max: 1, maxEOF: 2 }],
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
        ]
    }
}
