(function webpackUniversalModuleDefinition(root, factory) {
	if(typeof exports === 'object' && typeof module === 'object')
		module.exports = factory(require("signalR"), require("Vue"));
	else if(typeof define === 'function' && define.amd)
		define("treeV2", ["signalR", "Vue"], factory);
	else if(typeof exports === 'object')
		exports["treeV2"] = factory(require("signalR"), require("Vue"));
	else
		root["treeV2"] = factory(root["signalR"], root["Vue"]);
})(typeof self !== 'undefined' ? self : this, function(__WEBPACK_EXTERNAL_MODULE_32__, __WEBPACK_EXTERNAL_MODULE_30__) {
return /******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "/dist/";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 17);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports) {

function _classCallCheck(instance, Constructor) {
  if (!(instance instanceof Constructor)) {
    throw new TypeError("Cannot call a class as a function");
  }
}
module.exports = _classCallCheck, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

var toPropertyKey = __webpack_require__(11);
function _defineProperties(target, props) {
  for (var i = 0; i < props.length; i++) {
    var descriptor = props[i];
    descriptor.enumerable = descriptor.enumerable || false;
    descriptor.configurable = true;
    if ("value" in descriptor) descriptor.writable = true;
    Object.defineProperty(target, toPropertyKey(descriptor.key), descriptor);
  }
}
function _createClass(Constructor, protoProps, staticProps) {
  if (protoProps) _defineProperties(Constructor.prototype, protoProps);
  if (staticProps) _defineProperties(Constructor, staticProps);
  Object.defineProperty(Constructor, "prototype", {
    writable: false
  });
  return Constructor;
}
module.exports = _createClass, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 2 */
/***/ (function(module, exports, __webpack_require__) {

var arrayWithoutHoles = __webpack_require__(21);
var iterableToArray = __webpack_require__(22);
var unsupportedIterableToArray = __webpack_require__(13);
var nonIterableSpread = __webpack_require__(23);
function _toConsumableArray(arr) {
  return arrayWithoutHoles(arr) || iterableToArray(arr) || unsupportedIterableToArray(arr) || nonIterableSpread();
}
module.exports = _toConsumableArray, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 3 */
/***/ (function(module, exports) {

/* globals __VUE_SSR_CONTEXT__ */

// IMPORTANT: Do NOT use ES2015 features in this file.
// This module is a runtime utility for cleaner component module output and will
// be included in the final webpack user bundle.

module.exports = function normalizeComponent (
  rawScriptExports,
  compiledTemplate,
  functionalTemplate,
  injectStyles,
  scopeId,
  moduleIdentifier /* server only */
) {
  var esModule
  var scriptExports = rawScriptExports = rawScriptExports || {}

  // ES6 modules interop
  var type = typeof rawScriptExports.default
  if (type === 'object' || type === 'function') {
    esModule = rawScriptExports
    scriptExports = rawScriptExports.default
  }

  // Vue.extend constructor export interop
  var options = typeof scriptExports === 'function'
    ? scriptExports.options
    : scriptExports

  // render functions
  if (compiledTemplate) {
    options.render = compiledTemplate.render
    options.staticRenderFns = compiledTemplate.staticRenderFns
    options._compiled = true
  }

  // functional template
  if (functionalTemplate) {
    options.functional = true
  }

  // scopedId
  if (scopeId) {
    options._scopeId = scopeId
  }

  var hook
  if (moduleIdentifier) { // server build
    hook = function (context) {
      // 2.3 injection
      context =
        context || // cached call
        (this.$vnode && this.$vnode.ssrContext) || // stateful
        (this.parent && this.parent.$vnode && this.parent.$vnode.ssrContext) // functional
      // 2.2 with runInNewContext: true
      if (!context && typeof __VUE_SSR_CONTEXT__ !== 'undefined') {
        context = __VUE_SSR_CONTEXT__
      }
      // inject component styles
      if (injectStyles) {
        injectStyles.call(this, context)
      }
      // register component module identifier for async chunk inferrence
      if (context && context._registeredComponents) {
        context._registeredComponents.add(moduleIdentifier)
      }
    }
    // used by ssr in case component is cached and beforeCreate
    // never gets called
    options._ssrRegister = hook
  } else if (injectStyles) {
    hook = injectStyles
  }

  if (hook) {
    var functional = options.functional
    var existing = functional
      ? options.render
      : options.beforeCreate

    if (!functional) {
      // inject component registration as beforeCreate hook
      options.beforeCreate = existing
        ? [].concat(existing, hook)
        : [hook]
    } else {
      // for template-only hot-reload because in that case the render fn doesn't
      // go through the normalizer
      options._injectStyles = hook
      // register for functioal component in vue file
      options.render = function renderWithStyleInjection (h, context) {
        hook.call(context)
        return existing(h, context)
      }
    }
  }

  return {
    esModule: esModule,
    exports: scriptExports,
    options: options
  }
}


/***/ }),
/* 4 */
/***/ (function(module, exports, __webpack_require__) {

var toPropertyKey = __webpack_require__(11);
function _defineProperty(obj, key, value) {
  key = toPropertyKey(key);
  if (key in obj) {
    Object.defineProperty(obj, key, {
      value: value,
      enumerable: true,
      configurable: true,
      writable: true
    });
  } else {
    obj[key] = value;
  }
  return obj;
}
module.exports = _defineProperty, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 5 */
/***/ (function(module, exports) {

function asyncGeneratorStep(gen, resolve, reject, _next, _throw, key, arg) {
  try {
    var info = gen[key](arg);
    var value = info.value;
  } catch (error) {
    reject(error);
    return;
  }
  if (info.done) {
    resolve(value);
  } else {
    Promise.resolve(value).then(_next, _throw);
  }
}
function _asyncToGenerator(fn) {
  return function () {
    var self = this,
      args = arguments;
    return new Promise(function (resolve, reject) {
      var gen = fn.apply(self, args);
      function _next(value) {
        asyncGeneratorStep(gen, resolve, reject, _next, _throw, "next", value);
      }
      function _throw(err) {
        asyncGeneratorStep(gen, resolve, reject, _next, _throw, "throw", err);
      }
      _next(undefined);
    });
  };
}
module.exports = _asyncToGenerator, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 6 */
/***/ (function(module, exports) {

function _typeof(o) {
  "@babel/helpers - typeof";

  return (module.exports = _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (o) {
    return typeof o;
  } : function (o) {
    return o && "function" == typeof Symbol && o.constructor === Symbol && o !== Symbol.prototype ? "symbol" : typeof o;
  }, module.exports.__esModule = true, module.exports["default"] = module.exports), _typeof(o);
}
module.exports = _typeof, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 7 */
/***/ (function(module, exports, __webpack_require__) {

// TODO(Babel 8): Remove this file.

var runtime = __webpack_require__(24)();
module.exports = runtime;

try {
  regeneratorRuntime = runtime;
} catch (accidentalStrictMode) {
  if (typeof globalThis === "object") {
    globalThis.regeneratorRuntime = runtime;
  } else {
    Function("r", "regeneratorRuntime = r")(runtime);
  }
}


/***/ }),
/* 8 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return utils; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_defineProperty__ = __webpack_require__(4);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_defineProperty___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_defineProperty__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass__);




function ownKeys(e, r) { var t = Object.keys(e); if (Object.getOwnPropertySymbols) { var o = Object.getOwnPropertySymbols(e); r && (o = o.filter(function (r) { return Object.getOwnPropertyDescriptor(e, r).enumerable; })), t.push.apply(t, o); } return t; }
function _objectSpread(e) { for (var r = 1; r < arguments.length; r++) { var t = null != arguments[r] ? arguments[r] : {}; r % 2 ? ownKeys(Object(t), !0).forEach(function (r) { __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_defineProperty___default()(e, r, t[r]); }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(e, Object.getOwnPropertyDescriptors(t)) : ownKeys(Object(t)).forEach(function (r) { Object.defineProperty(e, r, Object.getOwnPropertyDescriptor(t, r)); }); } return e; }
function _createForOfIteratorHelper(o, allowArrayLike) { var it = typeof Symbol !== "undefined" && o[Symbol.iterator] || o["@@iterator"]; if (!it) { if (Array.isArray(o) || (it = _unsupportedIterableToArray(o)) || allowArrayLike && o && typeof o.length === "number") { if (it) o = it; var i = 0; var F = function F() {}; return { s: F, n: function n() { if (i >= o.length) return { done: true }; return { done: false, value: o[i++] }; }, e: function e(_e) { throw _e; }, f: F }; } throw new TypeError("Invalid attempt to iterate non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); } var normalCompletion = true, didErr = false, err; return { s: function s() { it = it.call(o); }, n: function n() { var step = it.next(); normalCompletion = step.done; return step; }, e: function e(_e2) { didErr = true; err = _e2; }, f: function f() { try { if (!normalCompletion && it.return != null) it.return(); } finally { if (didErr) throw err; } } }; }
function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }
function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) arr2[i] = arr[i]; return arr2; }
var utils = /*#__PURE__*/function () {
  function utils() {
    __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck___default()(this, utils);
  }
  __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass___default()(utils, null, [{
    key: "formateList",
    value: function formateList(list, level) {
      var arr = [];
      var _iterator = _createForOfIteratorHelper(list),
        _step;
      try {
        for (_iterator.s(); !(_step = _iterator.n()).done;) {
          var item = _step.value;
          var dataItem = {};
          dataItem.count = 0; //分组下挂载的设备总数(包含子孙分组所挂载的设备)
          dataItem.equipSelectCount = 0; // 设备选中数量（包含子孙分组设备选中数量）
          dataItem.controlSelectCount = 0; // 控制项选中数量（包含子孙分组设备选中数量）
          dataItem.equipCount = item.equipCount || 0; //分组下挂载的设备数量（仅当前分组，不包含子孙分组所挂载设备）
          dataItem.title = item.name;
          dataItem.key = item.id;
          dataItem.isGroup = true; //是否是分组
          dataItem.children = [];
          dataItem.status = 1; //分组状态
          dataItem.level = level || 1; //分组层级，按照层级缩进
          dataItem.expand = !level; //是否展开
          dataItem.equips = []; //设备存储区,展开每次从缓存中拿，关闭分组即清空equips
          dataItem.groupId = item.parentId; //父级分组id
          dataItem.groups = []; //子分组节点存储区（不包含孙分组节点）
          dataItem.alarmCounts = 0; //当前分组设备报警数量(包含子孙分组的报警数量累加)
          dataItem.backUpCounts = 0; //当前分组双机热备数量(包含子孙分组的双机热备数量累加)
          dataItem.indeterminate = false;
          dataItem.checked = false;
          dataItem.visible = true;
          dataItem.nodeEquipSelectCount = 0; //当前节点设备选中数量

          dataItem.checkedEquips = [];
          dataItem.halfCheckedEquips = [];
          dataItem.selectControlCount = 0;
          if (item.children && item.children.length > 0) {
            dataItem.groups = __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray___default()(this.formateList(item.children, level ? level + 1 : 2));
          }
          arr.push(dataItem);
        }
      } catch (err) {
        _iterator.e(err);
      } finally {
        _iterator.f();
      }
      return arr;
    }
  }, {
    key: "deepClone",
    value: function deepClone(source, level, needsSettings, parentId, equipCheckObject, equipStatusObject) {
      var arr = [];
      if (source) {
        for (var i = 0, length = source.length; i < length; i++) {
          arr.push({
            isGroup: needsSettings,
            key: "".concat(parentId, "-").concat(source[i].id),
            status: equipStatusObject[source[i].id] || 0,
            title: source[i].title,
            level: level,
            expand: false,
            isEquip: true,
            loading: false,
            indeterminate: equipCheckObject[source[i].equipNo] && equipCheckObject[source[i].equipNo].indeterminate || false,
            checked: equipCheckObject[source[i].equipNo] && equipCheckObject[source[i].equipNo].checked || false,
            groupId: parentId,
            equipNo: source[i].id,
            visible: true,
            settings: []
          });
        }
      }
      return arr;
    }
  }, {
    key: "copyOrigin",
    value: function copyOrigin(list) {
      var arr = [];
      list.forEach(function (item) {
        arr.push(_objectSpread({}, item));
      });
      return arr;
    }

    // 获取分组位置
  }, {
    key: "getPosition",
    value: function getPosition(key, visibleList) {
      var index = 0;
      for (var i = 0, length = visibleList.length; i < length; i++) {
        if (visibleList[i].key == key) {
          index = i - 30;
          break;
        }
      }
      return index;
    }

    // 扁平化
  }, {
    key: "flattern",
    value: function flattern(data, groupNodeObject) {
      var _this = this;
      data.forEach(function (item) {
        if (item.isGroup) {
          groupNodeObject["".concat(item.key)] = null;
          groupNodeObject["".concat(item.key)] = item;
        }
        if (item.groups && item.groups.length) {
          _this.flattern(item.groups, groupNodeObject);
        }
      });
    }

    /**
      * @description 生成唯一Id
      * @param {}  不用传参
      * @return {string}
    */
  }, {
    key: "generateUUID",
    value: function generateUUID() {
      var d = new Date().getTime();
      if (window.performance && typeof window.performance.now === 'function') {
        d += performance.now(); // use high-precision timer if available
      }
      var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c === 'x' ? r : r & 0x3 | 0x8).toString(16);
      });
      return uuid;
    }

    /**
       * @description  将普通列表转换为树结构的列表
       * @param {list} 可构建树形结构的普通列表
       * @return {string}
    */
  }, {
    key: "listToTreeList",
    value: function listToTreeList(list) {
      //
      var data = list;
      var result = [];
      var map = {};
      data.forEach(function (item) {
        map[item.id] = item;
      });
      data.forEach(function (item) {
        var parent = map[item.parentId];
        if (parent) {
          (parent.children || (parent.children = [])).push(item);
        } else {
          result.push(item);
        }
      });
      return result;
    }
  }]);
  return utils;
}();


/***/ }),
/* 9 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__components_virtualList_virtualList_vue__ = __webpack_require__(19);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__utils_utils_js__ = __webpack_require__(8);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__equipProcessing_gwEquipCache__ = __webpack_require__(44);
//
//
//




/* harmony default export */ __webpack_exports__["a"] = ({
  name: "treeV2",
  components: {
    tree: __WEBPACK_IMPORTED_MODULE_0__components_virtualList_virtualList_vue__["a" /* default */]
  },
  data: function data() {
    return {
      list: [],
      refId: __WEBPACK_IMPORTED_MODULE_1__utils_utils_js__["a" /* default */].generateUUID(),
      treeKey: null,
      listKey: null,
      hasBuildTree: false
    };
  },
  props: {
    treeType: {
      type: String,
      default: ''
    }
  },
  mounted: function mounted() {
    var _this = this;
    this.treeKey = "equipGroup".concat(this.treeType);
    this.listKey = "groupList".concat(this.treeType);
    if (window.top[this.treeKey]) {
      this.list = __WEBPACK_IMPORTED_MODULE_1__utils_utils_js__["a" /* default */].formateList(JSON.parse(JSON.stringify(window.top[this.treeKey])));
    } else if (window.top[this.listKey]) {
      this.buildTree();
    }
    if (!window.executeQueue) {
      window.executeQueues = {};
    }
    window.executeQueues[this.refId] = this.destroyTree;
    window.addEventListener('message', function (res) {
      if (res && res.data && res.data.type) {
        _this[res.data.type] && _this[res.data.type]();
      }
    });
    try {
      if (!window.top.hasIframe) {
        this.selfRequest();
      }
    } catch (error) {
      this.selfRequest();
    }
  },
  methods: {
    selfRequest: function selfRequest() {
      var equipCache = new __WEBPACK_IMPORTED_MODULE_2__equipProcessing_gwEquipCache__["a" /* default */]();
      equipCache.Init();
    },
    GetEquipGroupTreeWidthTreeType: function GetEquipGroupTreeWidthTreeType() {
      if (this.treeType && window.top[this.listKey]) {
        this.buildTree();
      }
    },
    GetEquipGroupTree: function GetEquipGroupTree() {
      if (!this.treeType && window.top[this.listKey]) {
        this.buildTree();
      }
    },
    buildTree: function buildTree() {
      if (window.top[this.listKey]) {
        var treeData = __WEBPACK_IMPORTED_MODULE_1__utils_utils_js__["a" /* default */].listToTreeList(JSON.parse(JSON.stringify(window.top[this.listKey])));
        this.list = __WEBPACK_IMPORTED_MODULE_1__utils_utils_js__["a" /* default */].formateList(JSON.parse(JSON.stringify(treeData)));
        window.top["equipGroup".concat(this.treeType)] = treeData;
        this.hasBuildTree = true;
      }
    },
    filterMethod: function filterMethod(searchName) {
      this.$refs[this.refId].filterMethod(searchName);
    },
    resetCheckedStatus: function resetCheckedStatus() {
      this.$refs[this.refId].resetCheckedStatus();
    },
    getEquipSelectd: function getEquipSelectd() {
      return this.$refs[this.refId].getEquipSelectd();
    },
    getControlSelected: function getControlSelected() {
      return this.$refs[this.refId].getControlSelected();
    },
    destroyTree: function destroyTree() {
      this.$refs[this.refId].destroyTree();
    }
  }
});

/***/ }),
/* 10 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_asyncToGenerator__ = __webpack_require__(5);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_asyncToGenerator___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_asyncToGenerator__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_defineProperty__ = __webpack_require__(4);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_defineProperty___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_defineProperty__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_toConsumableArray__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_toConsumableArray___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_toConsumableArray__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator__ = __webpack_require__(7);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__treeNode_treeNode_vue__ = __webpack_require__(25);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_vue_virtual_scroll_list__ = __webpack_require__(29);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_vue_virtual_scroll_list___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_5_vue_virtual_scroll_list__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__utils_utils_js__ = __webpack_require__(8);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__mixin_equipStatusManage__ = __webpack_require__(31);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__mixin_cacheManage__ = __webpack_require__(33);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__mixin_searchManage__ = __webpack_require__(34);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__mixin_equipNumManage__ = __webpack_require__(35);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__mixin_requestManage__ = __webpack_require__(36);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__mixin_checkStatusManage__ = __webpack_require__(42);




function ownKeys(e, r) { var t = Object.keys(e); if (Object.getOwnPropertySymbols) { var o = Object.getOwnPropertySymbols(e); r && (o = o.filter(function (r) { return Object.getOwnPropertyDescriptor(e, r).enumerable; })), t.push.apply(t, o); } return t; }
function _objectSpread(e) { for (var r = 1; r < arguments.length; r++) { var t = null != arguments[r] ? arguments[r] : {}; r % 2 ? ownKeys(Object(t), !0).forEach(function (r) { __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_defineProperty___default()(e, r, t[r]); }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(e, Object.getOwnPropertyDescriptors(t)) : ownKeys(Object(t)).forEach(function (r) { Object.defineProperty(e, r, Object.getOwnPropertyDescriptor(t, r)); }); } return e; }
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//










/* harmony default export */ __webpack_exports__["a"] = ({
  components: {
    VirtualList: __WEBPACK_IMPORTED_MODULE_5_vue_virtual_scroll_list___default.a
  },
  data: function data() {
    return {
      itemComponent: __WEBPACK_IMPORTED_MODULE_4__treeNode_treeNode_vue__["a" /* default */],
      //节点组件

      visibleList: [],
      //虚拟滚动所需要的列表

      currentSelect: -1,
      //当前选中的节点（不包含分组节点）

      equipStatusManage: null,
      //设备状态管理工具
      cacheManage: null,
      //内存管理工具
      searchManage: null,
      //搜索管理工具
      equipNumManage: null,
      //设备数量管理工具
      requestManage: null,
      //请求管理工具
      checkStatusManage: null,
      //请求管理工具

      groupNodeObject: {},
      //所有分组扁平化所保存的存储的地方

      nodesMap: {},
      //所有实例化对象映射

      equipCheckObject: {},
      //设备选中状态记录 equipCheckObject：{xxx设备号：{indeterminate: false,checked: false,groupId:xxx}}
      equipStatusObject: {},
      //记录设备状态,从后台推送的初始化全量状态,增量更新需要实时维护

      controlObject: {},
      //展开的控制项,controlObject:{xxx设备号：{groupId:xxx}}用于设置当前展开设置选中，如果依赖与equipControlObject筛选设置选中，性能差

      isSearchStatus: false,
      //是否是搜索状态

      equipControllObject: {},
      //绑定所有的已选中的设备控制项

      expandGroup: [],
      //记录已展开的分组
      updateJob: null
    };
  },
  props: {
    data: {
      //分组列表
      type: Array
    },
    selectEquips: {
      //设备选中列表，用于回显，如权限管理
      type: Array,
      default: function _default() {
        return [];
      }
    },
    controllList: {
      //控制项选中列表，用于回显，如权限管理
      type: Array,
      default: function _default() {
        return [];
      }
    },
    showSettings: {
      //是否需要加载设备控制项，如权限管理
      type: Boolean,
      default: false
    },
    showSelectNum: {
      //是否在树形结构顶部展示已选设备数量，如设备管理
      type: Boolean,
      default: function _default() {
        return false;
      }
    },
    showCheckbox: {
      //是否展示checkbox
      type: Boolean,
      default: false
    },
    showStatus: {
      //是否展示设备状态
      type: Boolean,
      default: false
    },
    showOperate: {
      // 是否展示操作按钮
      type: Boolean,
      default: false
    },
    defaultExpandAll: {
      type: Boolean,
      default: false
    },
    currentNodeKey: [String, Number],
    colorConfig: {
      type: Object,
      default: function _default() {
        return {
          "noComm": "#a0a0a0",
          "normal": "#63e03f",
          "alarm": "#f22433",
          "lsSet": "#bebcaa",
          "initialize": "#289ac0",
          "withdraw": "#ffc0cb",
          "BackUp": "#f8901c"
        };
      }
    },
    treeType: {
      type: String,
      default: ''
    },
    buildTree: {
      type: Function,
      default: function _default() {}
    },
    filterData: {
      type: Function,
      default: function _default() {}
    },
    alias: {
      type: String,
      default: ''
    }
  },
  watch: {
    'data': function data(val) {
      if (val && val.length) {
        // 将对象扁平化到this.groupNodeObject
        __WEBPACK_IMPORTED_MODULE_6__utils_utils_js__["a" /* default */].flattern(this.data, this.groupNodeObject);
        // 增加分组映射
        this.cacheManage.addNodesMap(Object.values(this.groupNodeObject).map(function (item) {
          return item;
        }));
        this.init();
      }
    },
    'controllList': function controllList(val) {
      this.updateCheckedStatusWithControls();
    },
    'selectEquips': function selectEquips(val) {
      this.updateCheckedStatusWithEquips();
    }
  },
  computed: {
    aliasName: function aliasName() {
      return this.alias ? "-".concat(this.alias) : '';
    }
  },
  created: function created() {
    // 实例化复选框管理工具
    if (this.showCheckbox) {
      this.checkStatusManage = new __WEBPACK_IMPORTED_MODULE_12__mixin_checkStatusManage__["a" /* default */](this.groupNodeObject, this.nodesMap, this.equipControllObject, this.controlObject, this.equipCheckObject, this.aliasName);
    }

    // 实例化缓存管理工具
    this.cacheManage = new __WEBPACK_IMPORTED_MODULE_8__mixin_cacheManage__["a" /* default */](this.groupNodeObject, this.nodesMap, this.controlObject, this.equipCheckObject);

    // 实例化搜索管理工具
    this.searchManage = new __WEBPACK_IMPORTED_MODULE_9__mixin_searchManage__["a" /* default */](this.groupNodeObject, this.showSettings, this.aliasName);

    // 实例化分组所挂载的设备数量管理工具
    this.equipNumManage = new __WEBPACK_IMPORTED_MODULE_10__mixin_equipNumManage__["a" /* default */](this.groupNodeObject, this.aliasName);

    // 实例化数据请求管理工具
    this.requestManage = new __WEBPACK_IMPORTED_MODULE_11__mixin_requestManage__["a" /* default */](this.nodesMap, this.equipControllObject);
  },
  mounted: function mounted() {
    var _this = this;
    this.updateCheckedStatusWithControls();
    this.updateCheckedStatusWithEquips();
    window.addEventListener('message', function (e) {
      if (e.data.type) {
        _this[e.data.type] && _this[e.data.type](e.data.data);
      }
    });
  },
  methods: {
    // 初始化
    init: function init() {
      var _this2 = this;
      this.data[0].expand = true;
      this.updateTreeList();
      setTimeout(function () {
        Object.values(_this2.groupNodeObject).forEach(function (group) {
          if (!group.expand && _this2.expandGroup.includes(group.key)) {
            group.expand = true;
          }
          _this2.filterWithAlias(group.key);
          _this2.updateGroupEquips(group.key, true);
        });
        _this2.equipNumManage.resetGroupNum(_this2.isSearchStatus);
        // 实例化设备状态管理工具
        if (_this2.showStatus && !_this2.equipStatusManage) {
          _this2.equipStatusManage = new __WEBPACK_IMPORTED_MODULE_7__mixin_equipStatusManage__["a" /* default */](_this2.nodesMap, _this2.equipStatusObject, _this2.groupNodeObject, _this2.statusChange, _this2.aliasName);
        }
      }, 100);
    },
    // 根据条件过滤
    filterWithAlias: function filterWithAlias(groupKey) {
      if (this.aliasName) {
        var arr = window.top["group-".concat(groupKey)] || [];
        window.top["group-".concat(groupKey).concat(this.aliasName)] = this.filterData(arr);
      }
    },
    // 更新当前选中
    updateCurrentSelect: function updateCurrentSelect() {
      if (window.top.equipCache && window.top.equipCache[this.currentNodeKey]) {
        this.currentSelect = "".concat(window.top.equipCache[this.currentNodeKey].groupId, "-").concat(this.currentNodeKey);
      }
    },
    // 设备状态变化
    statusChange: function statusChange(groupId, equipNo, status) {
      if (this.currentSelect == "".concat(groupId, "-").concat(equipNo)) {
        this.$emit('statusChange', equipNo, status);
      }
    },
    // 更新checkbox选中状态
    updateCheckWidthJob: function updateCheckWidthJob() {
      if (this.showCheckbox) {
        this.checkStatusManage.resetCheckedStatus();
        this.updateCheckedStatusWithControls();
        this.updateCheckedStatusWithEquips();
      }
    },
    // 通过外框更新设备
    updateGroupEquips: function updateGroupEquips(key, isInit) {
      if (!this.isSearchStatus && this.groupNodeObject[key]) {
        var total = this.equipNumManage.getAllEquipsNum();
        this.$emit('getTotal', total);
      }
      if (isInit) {
        var arr = window.top["group-".concat(key).concat(this.aliasName)] || [];
        this.equipNumManage.setGroupNum(key, arr.length);
      }
      if (this.groupNodeObject[key].expand) {
        this.updateList(key, this.groupNodeObject[key].level, null, false);
      }
    },
    // 展开的时候从缓存中拿设备数据到equips中并更新视图
    updateList: function updateList(key) {
      // 当前为分组
      if (this.groupNodeObject[key]) {
        var arr = [];
        if (this.isSearchStatus) {
          arr = window.top["group-".concat(key, "-search")];
          this.groupNodeObject[key].equips = __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_toConsumableArray___default()(__WEBPACK_IMPORTED_MODULE_6__utils_utils_js__["a" /* default */].deepClone(arr, Number(this.groupNodeObject[key].level) + 1, this.showSettings, key, this.equipCheckObject, this.equipStatusObject));
        } else {
          arr = window.top["group-".concat(key).concat(this.aliasName)];
          this.groupNodeObject[key].equips = __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_toConsumableArray___default()(__WEBPACK_IMPORTED_MODULE_6__utils_utils_js__["a" /* default */].deepClone(arr, Number(this.groupNodeObject[key].level) + 1, this.showSettings, key, this.equipCheckObject, this.equipStatusObject));
        }

        // 增加设备节点映射
        this.cacheManage.addNodesMap(this.groupNodeObject[key].equips);

        // 关闭兄弟节点
        this.cacheManage.closeBrotherNode(key);
        this.visibleList = [];
        this.updateTreeList(this.data);
        if (this.showStatus && this.equipStatusManage) {
          this.equipStatusManage.updateGroupStatus(key);
        }
        this.updateCurrentSelect();
      }
    },
    // 更新整个树形结构
    updateTreeList: function updateTreeList(data) {
      var _this3 = this;
      if (data) {
        data.forEach(function (item) {
          _this3.visibleList.push(item);
          if (item.expand) {
            if (item.isEquip) {
              if (!item.children) {
                item.children = [];
              }
              if (item.settings) {
                item.children = __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_toConsumableArray___default()(item.settings || []);
              }
            } else {
              item.children = [].concat(__WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_toConsumableArray___default()(item.equips || []), __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_toConsumableArray___default()(item.groups || []));
            }
          } else {
            item.children = [];
          }
          _this3.updateTreeList(item.children || []);
        });
      }
    },
    // 节点点击事件
    nodeClick: function nodeClick(node, nodeIndex, level, checked) {
      this.$emit('node-click', _objectSpread(_objectSpread({}, node), {}, {
        key: node.isEquip ? node.equipNo : node.key
      }));
      if (node.isGroup) {
        if (!node.isEquip) {
          this.groupClick(node, nodeIndex, checked);
        } else {
          this.equipClick(node, nodeIndex, checked);
        }
      } else {
        this.currentSelect = node.key;
      }
    },
    groupClick: function groupClick(node, nodeIndex, checked) {
      if (node.expand) {
        this.updateList(node.key, node.level, nodeIndex, checked);
      } else {
        this.cacheManage.recycleGroupCache(node.key);
        this.visibleList = [];
        this.updateTreeList(this.data);
      }
    },
    equipClick: function equipClick(node, nodeIndex, checked) {
      var _this4 = this;
      return __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_asyncToGenerator___default()( /*#__PURE__*/__WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator___default.a.mark(function _callee() {
        var _this4$visibleList;
        return __WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator___default.a.wrap(function _callee$(_context) {
          while (1) switch (_context.prev = _context.next) {
            case 0:
              if (!_this4.showSettings) {
                _context.next = 15;
                break;
              }
              if (!node.expand) {
                _context.next = 11;
                break;
              }
              node.loading = true;
              _context.next = 5;
              return _this4.requestManage.getSetting(node.key, node.title, node.level, checked);
            case 5:
              _this4.controlObject[node.equipNo] = {
                groupId: node.groupId
              };
              node.loading = false;
              (_this4$visibleList = _this4.visibleList).splice.apply(_this4$visibleList, [nodeIndex + 1, 0].concat(__WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_toConsumableArray___default()(_this4.nodesMap[node.key].settings)));
              _this4.cacheManage.addNodesMap(_this4.nodesMap[node.key].settings);
              _context.next = 15;
              break;
            case 11:
              _this4.visibleList.splice(nodeIndex + 1, _this4.nodesMap[node.key].settings.length);
              _this4.cacheManage.removeNodesMap(_this4.nodesMap[node.key].settings);
              _this4.nodesMap[node.key].settings = [];
              delete _this4.controlObject[node.equipNo];
            case 15:
            case "end":
              return _context.stop();
          }
        }, _callee);
      }))();
    },
    onChecked: function onChecked(node) {
      this.checkStatusManage && this.checkStatusManage.onChecked(node, this.isSearchStatus);
      this.$emit('onCheck', node);
    },
    groupEditAndNew: function groupEditAndNew(isGroupNew, node) {
      this.$emit('groupEditAndNew', {
        isGroupNew: isGroupNew,
        node: node
      });
    },
    deleteGroup: function deleteGroup(node) {
      this.$emit('deleteGroup', node);
    },
    updateCheckedStatusWithControls: function updateCheckedStatusWithControls() {
      if (this.controllList && this.controllList.length && this.checkStatusManage) {
        this.checkStatusManage && this.checkStatusManage.updateCheckedStatusWithControls(this.controllList);
      }
    },
    updateCheckedStatusWithEquips: function updateCheckedStatusWithEquips() {
      if (this.selectEquips && this.selectEquips.length && this.checkStatusManage) {
        this.checkStatusManage && this.checkStatusManage.updateCheckedStatusWithEquips(this.selectEquips);
      }
    },
    resetCheckedStatus: function resetCheckedStatus() {
      this.checkStatusManage && this.checkStatusManage.resetCheckedStatus();
    },
    /**
     * @description 获取选择的设备
     *  @return Array
     */
    getEquipSelectd: function getEquipSelectd() {
      return this.checkStatusManage && this.checkStatusManage.getEquipSelectd();
    },
    /**
     * @description 获取选择的设备控制项
     *  @return Array
     */
    getControlSelected: function getControlSelected() {
      return this.checkStatusManage && this.checkStatusManage.getControlSelected();
    },
    /**
     * @description 记录已展开的分组
     *  @return 无
     */
    recordExpandGroup: function recordExpandGroup() {
      var _this5 = this;
      this.expandGroup = [];
      Object.values(this.groupNodeObject).forEach(function (group) {
        if (group.expand) {
          _this5.expandGroup.push(group.key);
        }
      });
    },
    /**
     * @description 清除已展开的分组
     *  @return
     */
    removeExpandGroup: function removeExpandGroup() {
      var _this6 = this;
      this.$nextTick(function () {
        _this6.expandGroup = [];
      });
    },
    /**
     * @description 搜索设备
     *    1：清除所有节点缓存
          2: 设置搜索状态为true,搜索状态下所有操作全部不更新
          3: 非搜索状态，需要清除搜索缓存
     * @param {searchName}  搜索名称
     *  @return 无返回值
     */
    filterMethod: function filterMethod(searchName) {
      var _this7 = this;
      this.visibleList = [];
      this.cacheManage.recycleAllNodeCache();
      if (searchName) {
        this.isSearchStatus = true;
        this.searchManage.filterMethod(searchName);
      } else {
        this.isSearchStatus = false;
      }
      this.init();
      this.$nextTick(function () {
        _this7.$refs.virtualList.scrollToIndex(0);
      });
      this.equipNumManage.resetGroupNum(this.isSearchStatus);
      if (this.showCheckbox) {
        this.checkStatusManage && this.checkStatusManage.reComputedCheckNum(this.isSearchStatus);
        this.checkStatusManage && this.checkStatusManage.updateGroupCheckStatus();
      }
    },
    // 重新构建树
    rebuildTree: function rebuildTree() {
      var _this8 = this;
      this.recordExpandGroup();
      this.checkStatusManage && this.checkStatusManage.resetCheckedStatus();
      this.cacheManage.recycleAllNodeCache(true);
      this.$nextTick(function () {
        _this8.buildTree();
        _this8.removeExpandGroup();
      });
    },
    // 树形结构销毁
    destroyTree: function destroyTree() {
      this.cacheManage.recycleAllNodeCache(true);
      this.visibleList = [];
      this.updateTreeList(this.data);
    },
    // 通过外框事件更新树形结构数据--start
    /**
     * @description 获取分组设备
     * @param {data}  {分组Id}
     *  @return 无返回值
     */
    GetGroupEquips: function GetGroupEquips(data) {
      var _this9 = this;
      if (data.groupId && this.groupNodeObject[data.groupId] && !this.isSearchStatus) {
        this.updateGroupEquips(data.groupId, true);
        if (this.updateJob) {
          clearTimeout(this.updateJob);
        }
        this.updateJob = setTimeout(function () {
          _this9.updateCheckWidthJob();
        }, 500);
      }
    },
    /**
      * @description 删除设备
      * @param {data}  {groupId,equips}
      *  @return 无返回值
    */
    DeleteEquip: function DeleteEquip(data) {
      var _this10 = this;
      var _ref = data || {},
        groupId = _ref.groupId,
        equips = _ref.equips;
      if (groupId && this.groupNodeObject[groupId]) {
        this.updateGroupEquips(data.groupId);
        this.equipNumManage.resetGroupNum();
        this.checkStatusManage && this.checkStatusManage.resetCheckedStatus();
      }
      if (equips.length) {
        equips.forEach(function (item) {
          var node = _this10.nodesMap["".concat(groupId, "-").concat(item.id)];
          if (node) {
            if (_this10.currentSelect.toString().includes(item.id)) {
              _this10.$emit('currentDelete');
            }
            if (_this10.showStatus) {
              if (node.status == 2 || node.status == 0) {
                _this10.equipStatusManage.setGroupStatus(groupId, false, 2);
              } else if (node.status == 6) {
                _this10.equipStatusManage.setGroupStatus(groupId, false, 6);
              }
            }
            delete _this10.nodesMap["".concat(groupId, "-").concat(item.id)];
            delete _this10.equipCheckObject[item.id];
          }
          delete _this10.equipStatusObject[item.id];
        });
      }
    },
    /**
      * @description 移动设备
      * @param {data} {updateGroups} 需要更新的分组列表
      *  @return 无返回值
    */
    moveEquips: function moveEquips(data) {
      var _this11 = this;
      var _ref2 = data || {},
        updateGroups = _ref2.updateGroups,
        buildTree = _ref2.buildTree;
      if (!buildTree && updateGroups) {
        updateGroups.forEach(function (groupId) {
          if (groupId && _this11.groupNodeObject[groupId]) {
            _this11.updateGroupEquips(groupId);
            _this11.equipNumManage.resetGroupNum();
            _this11.checkStatusManage && _this11.checkStatusManage.resetCheckedStatus();
          }
        });
      } else if (buildTree) {
        this.rebuildTree();
      }
    },
    /**
       * @description 删除分组
       * @param {data}  {groupId,parentGroupId}
       * @return 无返回值
    */
    DeleteEquipGroup: function DeleteEquipGroup(data) {
      var _this12 = this;
      data.forEach(function (group) {
        if (group.groupId && _this12.nodesMap[group.groupId]) {
          if (!_this12.isSearchStatus) {
            _this12.rebuildTree();
          }
        }
      });
    },
    /**
       * @description 新增分组
       * @param {data}  {groupId,parentGroupId}
       * @return 无返回值
    */
    AddEquipGroup: function AddEquipGroup(data) {
      var _ref3 = data || {},
        parentGroupId = _ref3.parentGroupId,
        groupId = _ref3.groupId;
      if (groupId && this.nodesMap[parentGroupId] && this.treeType) {
        if (!this.isSearchStatus) {
          this.rebuildTree();
        }
      }
    },
    /** 编辑分组
       * @description 新增分组
       * @param {data}  {groupId,parentGroupId}
       * @return 无返回值
    */
    EditEquipGroup: function EditEquipGroup(data) {
      var groupId = data.groupId,
        groupName = data.groupName;
      if (this.nodesMap[groupId]) {
        this.nodesMap[groupId].title = groupName;
      }
    },
    /**
     * @description 新增分组设备
     * @param {data}  {分组Id,设备}
     *  @return 无返回值
     */
    AddEquip: function AddEquip(data) {
      var _ref4 = data || {},
        groupId = _ref4.groupId;
      if (groupId) {
        if (!this.isSearchStatus) {
          this.rebuildTree();
        }
      }
    },
    /**
        * @description 新增分组设备
        * @param {data}  {分组Id,设备}
        * @return 无返回值
    */
    EditEquip: function EditEquip(data) {
      var _ref5 = data || {},
        equipNo = _ref5.equipNo,
        groupId = _ref5.groupId,
        equipName = _ref5.equipName;
      if (this.nodesMap["".concat(groupId, "-").concat(equipNo)]) {
        if (!this.isSearchStatus) {
          this.nodesMap["".concat(groupId, "-").concat(equipNo)].title = equipName;
        }
      }
    } // 通过外框事件更新树形结构数据--end
  }
});

/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

var _typeof = __webpack_require__(6)["default"];
var toPrimitive = __webpack_require__(20);
function toPropertyKey(t) {
  var i = toPrimitive(t, "string");
  return "symbol" == _typeof(i) ? i : i + "";
}
module.exports = toPropertyKey, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 12 */
/***/ (function(module, exports) {

function _arrayLikeToArray(arr, len) {
  if (len == null || len > arr.length) len = arr.length;
  for (var i = 0, arr2 = new Array(len); i < len; i++) arr2[i] = arr[i];
  return arr2;
}
module.exports = _arrayLikeToArray, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 13 */
/***/ (function(module, exports, __webpack_require__) {

var arrayLikeToArray = __webpack_require__(12);
function _unsupportedIterableToArray(o, minLen) {
  if (!o) return;
  if (typeof o === "string") return arrayLikeToArray(o, minLen);
  var n = Object.prototype.toString.call(o).slice(8, -1);
  if (n === "Object" && o.constructor) n = o.constructor.name;
  if (n === "Map" || n === "Set") return Array.from(o);
  if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return arrayLikeToArray(o, minLen);
}
module.exports = _unsupportedIterableToArray, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 14 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__oparate_oparate_vue__ = __webpack_require__(26);
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//


/* harmony default export */ __webpack_exports__["a"] = ({
  name: 'ElTreeVirtualNode',
  componentName: 'ElTreeVirtualNode',
  props: {
    searchName: {
      type: String,
      default: ''
    },
    expanding: {
      type: Boolean,
      default: false
    },
    aleadyLoadAll: {
      type: Boolean,
      default: false
    },
    groupLoading: {
      type: Boolean,
      default: false
    },
    source: {
      default: function _default() {
        return {};
      }
    },
    showCheckbox: {
      type: Boolean,
      default: false
    },
    showStatus: {
      type: Boolean,
      default: false
    },
    showOperate: {
      type: Boolean,
      default: false
    },
    currentSelect: {
      type: Number | String,
      default: -1
    },
    nodeClick: {
      type: Function
    },
    onChecked: {
      type: Function
    },
    groupEditAndNew: {
      type: Function
    },
    deleteGroup: {
      type: Function
    },
    colorConfig: {
      type: Object,
      default: function _default() {}
    },
    index: {
      type: Number,
      default: -1
    }
  },
  components: {
    oparate: __WEBPACK_IMPORTED_MODULE_0__oparate_oparate_vue__["a" /* default */]
  },
  computed: {
    getColor: function getColor() {
      return function (state) {
        var color;
        switch (state) {
          case 0:
            color = this.colorConfig['noComm'];
            break;
          case 1:
            color = this.colorConfig['normal'];
            break;
          case 2:
            color = this.colorConfig['alarm'];
            break;
          case 3:
            color = this.colorConfig['lsSet'];
            break;
          case 4:
            color = this.colorConfig['initialize'];
            break;
          case 5:
            color = this.colorConfig['withdraw'];
            break;
          case 6:
            color = this.colorConfig['BackUp'];
            break;
          default:
            color = this.colorConfig['noComm'];
            break;
        }
        return color;
      };
    }
  },
  data: function data() {
    return {
      indent: 13
    };
  },
  methods: {
    clickFunction: function clickFunction() {
      this.source.expand = !this.source.expand;
      this.nodeClick(this.source, this.index, this.source.level, this.source.checked);
    },
    checkedChange: function checkedChange(val) {
      this.onChecked(this.source);
    }
  }
});

/***/ }),
/* 15 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//

/* harmony default export */ __webpack_exports__["a"] = ({
  props: {
    source: {
      type: Object,
      default: function _default() {}
    },
    groupEditAndNew: {
      type: Function,
      default: function _default() {}
    },
    deleteGroup: {
      type: Function,
      default: function _default() {}
    }
  }
});

/***/ }),
/* 16 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return Signalr; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_asyncToGenerator__ = __webpack_require__(5);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_asyncToGenerator___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_asyncToGenerator__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_classCallCheck__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_classCallCheck___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_classCallCheck__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_createClass__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_createClass___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_createClass__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator__ = __webpack_require__(7);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__aspnet_signalr__ = __webpack_require__(32);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__aspnet_signalr___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4__aspnet_signalr__);





var Signalr = /*#__PURE__*/function () {
  function Signalr(path, connectionId, equipNo) {
    __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_classCallCheck___default()(this, Signalr);
    this.url = path;
    this.connectionId = connectionId;
    this.equipNo = equipNo;
    this.signalr = null;
  }
  __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_createClass___default()(Signalr, [{
    key: "openConnect",
    value: function openConnect() {
      var _this = this;
      // 停止连接
      if (this.signalr) {
        this.signalr.stop();
        this.signalr = null;
      }

      //  连接
      this.signalr = new __WEBPACK_IMPORTED_MODULE_4__aspnet_signalr__["HubConnectionBuilder"]().withUrl(this.url).build();
      this.signalr.serverTimeoutInMilliseconds = 500000000;
      this.signalr.keepaliveintervalinmilliseconds = 500000000;
      return new Promise(function (resolve) {
        _this.signalr.start() // 启动连接
        .then(function () {
          if (_this.connectionId) {
            _this.send();
          }
          resolve(_this.signalr);
        }).catch(function (e) {
          console.log(e);
        });
      });
    }
  }, {
    key: "send",
    value: function () {
      var _send = __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_asyncToGenerator___default()( /*#__PURE__*/__WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator___default.a.mark(function _callee() {
        return __WEBPACK_IMPORTED_MODULE_3__babel_runtime_regenerator___default.a.wrap(function _callee$(_context) {
          while (1) switch (_context.prev = _context.next) {
            case 0:
              _context.prev = 0;
              _context.next = 3;
              return this.signalr.invoke(this.connectionId, this.equipNo).catch(function (error) {
                console.log('send 发送失败' + error);
              });
            case 3:
              _context.next = 8;
              break;
            case 5:
              _context.prev = 5;
              _context.t0 = _context["catch"](0);
              console.log('connectHub 连接失败' + _context.t0);
            case 8:
            case "end":
              return _context.stop();
          }
        }, _callee, this, [[0, 5]]);
      }));
      function send() {
        return _send.apply(this, arguments);
      }
      return send;
    }()
  }]);
  return Signalr;
}();


/***/ }),
/* 17 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__treev2_vue__ = __webpack_require__(18);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__tree_scss__ = __webpack_require__(46);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__tree_scss___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__tree_scss__);



/* istanbul ignore next */
__WEBPACK_IMPORTED_MODULE_0__treev2_vue__["a" /* default */].install = function (Vue) {
  Vue.component(__WEBPACK_IMPORTED_MODULE_0__treev2_vue__["a" /* default */].name, __WEBPACK_IMPORTED_MODULE_0__treev2_vue__["a" /* default */]);
};
/* harmony default export */ __webpack_exports__["default"] = (__WEBPACK_IMPORTED_MODULE_0__treev2_vue__["a" /* default */]);

/***/ }),
/* 18 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_loader_node_modules_vue_loader_lib_selector_type_script_index_0_treev2_vue__ = __webpack_require__(9);
/* unused harmony namespace reexport */
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__node_modules_vue_loader_lib_template_compiler_index_id_data_v_10382e54_hasScoped_false_buble_transforms_node_modules_vue_loader_lib_selector_type_template_index_0_treev2_vue__ = __webpack_require__(45);
var disposed = false
var normalizeComponent = __webpack_require__(3)
/* script */


/* template */

/* template functional */
var __vue_template_functional__ = false
/* styles */
var __vue_styles__ = null
/* scopeId */
var __vue_scopeId__ = null
/* moduleIdentifier (server only) */
var __vue_module_identifier__ = null
var Component = normalizeComponent(
  __WEBPACK_IMPORTED_MODULE_0__babel_loader_node_modules_vue_loader_lib_selector_type_script_index_0_treev2_vue__["a" /* default */],
  __WEBPACK_IMPORTED_MODULE_1__node_modules_vue_loader_lib_template_compiler_index_id_data_v_10382e54_hasScoped_false_buble_transforms_node_modules_vue_loader_lib_selector_type_template_index_0_treev2_vue__["a" /* default */],
  __vue_template_functional__,
  __vue_styles__,
  __vue_scopeId__,
  __vue_module_identifier__
)
Component.options.__file = "gw-base-components-plus/treeV2/treev2.vue"

/* hot reload */
if (false) {(function () {
  var hotAPI = require("vue-hot-reload-api")
  hotAPI.install(require("vue"), false)
  if (!hotAPI.compatible) return
  module.hot.accept()
  if (!module.hot.data) {
    hotAPI.createRecord("data-v-10382e54", Component.options)
  } else {
    hotAPI.reload("data-v-10382e54", Component.options)
  }
  module.hot.dispose(function (data) {
    disposed = true
  })
})()}

/* harmony default export */ __webpack_exports__["a"] = (Component.exports);


/***/ }),
/* 19 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_loader_node_modules_vue_loader_lib_selector_type_script_index_0_virtualList_vue__ = __webpack_require__(10);
/* unused harmony namespace reexport */
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__node_modules_vue_loader_lib_template_compiler_index_id_data_v_47259c44_hasScoped_false_buble_transforms_node_modules_vue_loader_lib_selector_type_template_index_0_virtualList_vue__ = __webpack_require__(43);
var disposed = false
var normalizeComponent = __webpack_require__(3)
/* script */


/* template */

/* template functional */
var __vue_template_functional__ = false
/* styles */
var __vue_styles__ = null
/* scopeId */
var __vue_scopeId__ = null
/* moduleIdentifier (server only) */
var __vue_module_identifier__ = null
var Component = normalizeComponent(
  __WEBPACK_IMPORTED_MODULE_0__babel_loader_node_modules_vue_loader_lib_selector_type_script_index_0_virtualList_vue__["a" /* default */],
  __WEBPACK_IMPORTED_MODULE_1__node_modules_vue_loader_lib_template_compiler_index_id_data_v_47259c44_hasScoped_false_buble_transforms_node_modules_vue_loader_lib_selector_type_template_index_0_virtualList_vue__["a" /* default */],
  __vue_template_functional__,
  __vue_styles__,
  __vue_scopeId__,
  __vue_module_identifier__
)
Component.options.__file = "gw-base-components-plus/treeV2/components/virtualList/virtualList.vue"

/* hot reload */
if (false) {(function () {
  var hotAPI = require("vue-hot-reload-api")
  hotAPI.install(require("vue"), false)
  if (!hotAPI.compatible) return
  module.hot.accept()
  if (!module.hot.data) {
    hotAPI.createRecord("data-v-47259c44", Component.options)
  } else {
    hotAPI.reload("data-v-47259c44", Component.options)
  }
  module.hot.dispose(function (data) {
    disposed = true
  })
})()}

/* harmony default export */ __webpack_exports__["a"] = (Component.exports);


/***/ }),
/* 20 */
/***/ (function(module, exports, __webpack_require__) {

var _typeof = __webpack_require__(6)["default"];
function toPrimitive(t, r) {
  if ("object" != _typeof(t) || !t) return t;
  var e = t[Symbol.toPrimitive];
  if (void 0 !== e) {
    var i = e.call(t, r || "default");
    if ("object" != _typeof(i)) return i;
    throw new TypeError("@@toPrimitive must return a primitive value.");
  }
  return ("string" === r ? String : Number)(t);
}
module.exports = toPrimitive, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 21 */
/***/ (function(module, exports, __webpack_require__) {

var arrayLikeToArray = __webpack_require__(12);
function _arrayWithoutHoles(arr) {
  if (Array.isArray(arr)) return arrayLikeToArray(arr);
}
module.exports = _arrayWithoutHoles, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 22 */
/***/ (function(module, exports) {

function _iterableToArray(iter) {
  if (typeof Symbol !== "undefined" && iter[Symbol.iterator] != null || iter["@@iterator"] != null) return Array.from(iter);
}
module.exports = _iterableToArray, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 23 */
/***/ (function(module, exports) {

function _nonIterableSpread() {
  throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.");
}
module.exports = _nonIterableSpread, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 24 */
/***/ (function(module, exports, __webpack_require__) {

var _typeof = __webpack_require__(6)["default"];
function _regeneratorRuntime() {
  "use strict";
  module.exports = _regeneratorRuntime = function _regeneratorRuntime() {
    return e;
  }, module.exports.__esModule = true, module.exports["default"] = module.exports;
  var t,
    e = {},
    r = Object.prototype,
    n = r.hasOwnProperty,
    o = Object.defineProperty || function (t, e, r) {
      t[e] = r.value;
    },
    i = "function" == typeof Symbol ? Symbol : {},
    a = i.iterator || "@@iterator",
    c = i.asyncIterator || "@@asyncIterator",
    u = i.toStringTag || "@@toStringTag";
  function define(t, e, r) {
    return Object.defineProperty(t, e, {
      value: r,
      enumerable: !0,
      configurable: !0,
      writable: !0
    }), t[e];
  }
  try {
    define({}, "");
  } catch (t) {
    define = function define(t, e, r) {
      return t[e] = r;
    };
  }
  function wrap(t, e, r, n) {
    var i = e && e.prototype instanceof Generator ? e : Generator,
      a = Object.create(i.prototype),
      c = new Context(n || []);
    return o(a, "_invoke", {
      value: makeInvokeMethod(t, r, c)
    }), a;
  }
  function tryCatch(t, e, r) {
    try {
      return {
        type: "normal",
        arg: t.call(e, r)
      };
    } catch (t) {
      return {
        type: "throw",
        arg: t
      };
    }
  }
  e.wrap = wrap;
  var h = "suspendedStart",
    l = "suspendedYield",
    f = "executing",
    s = "completed",
    y = {};
  function Generator() {}
  function GeneratorFunction() {}
  function GeneratorFunctionPrototype() {}
  var p = {};
  define(p, a, function () {
    return this;
  });
  var d = Object.getPrototypeOf,
    v = d && d(d(values([])));
  v && v !== r && n.call(v, a) && (p = v);
  var g = GeneratorFunctionPrototype.prototype = Generator.prototype = Object.create(p);
  function defineIteratorMethods(t) {
    ["next", "throw", "return"].forEach(function (e) {
      define(t, e, function (t) {
        return this._invoke(e, t);
      });
    });
  }
  function AsyncIterator(t, e) {
    function invoke(r, o, i, a) {
      var c = tryCatch(t[r], t, o);
      if ("throw" !== c.type) {
        var u = c.arg,
          h = u.value;
        return h && "object" == _typeof(h) && n.call(h, "__await") ? e.resolve(h.__await).then(function (t) {
          invoke("next", t, i, a);
        }, function (t) {
          invoke("throw", t, i, a);
        }) : e.resolve(h).then(function (t) {
          u.value = t, i(u);
        }, function (t) {
          return invoke("throw", t, i, a);
        });
      }
      a(c.arg);
    }
    var r;
    o(this, "_invoke", {
      value: function value(t, n) {
        function callInvokeWithMethodAndArg() {
          return new e(function (e, r) {
            invoke(t, n, e, r);
          });
        }
        return r = r ? r.then(callInvokeWithMethodAndArg, callInvokeWithMethodAndArg) : callInvokeWithMethodAndArg();
      }
    });
  }
  function makeInvokeMethod(e, r, n) {
    var o = h;
    return function (i, a) {
      if (o === f) throw Error("Generator is already running");
      if (o === s) {
        if ("throw" === i) throw a;
        return {
          value: t,
          done: !0
        };
      }
      for (n.method = i, n.arg = a;;) {
        var c = n.delegate;
        if (c) {
          var u = maybeInvokeDelegate(c, n);
          if (u) {
            if (u === y) continue;
            return u;
          }
        }
        if ("next" === n.method) n.sent = n._sent = n.arg;else if ("throw" === n.method) {
          if (o === h) throw o = s, n.arg;
          n.dispatchException(n.arg);
        } else "return" === n.method && n.abrupt("return", n.arg);
        o = f;
        var p = tryCatch(e, r, n);
        if ("normal" === p.type) {
          if (o = n.done ? s : l, p.arg === y) continue;
          return {
            value: p.arg,
            done: n.done
          };
        }
        "throw" === p.type && (o = s, n.method = "throw", n.arg = p.arg);
      }
    };
  }
  function maybeInvokeDelegate(e, r) {
    var n = r.method,
      o = e.iterator[n];
    if (o === t) return r.delegate = null, "throw" === n && e.iterator["return"] && (r.method = "return", r.arg = t, maybeInvokeDelegate(e, r), "throw" === r.method) || "return" !== n && (r.method = "throw", r.arg = new TypeError("The iterator does not provide a '" + n + "' method")), y;
    var i = tryCatch(o, e.iterator, r.arg);
    if ("throw" === i.type) return r.method = "throw", r.arg = i.arg, r.delegate = null, y;
    var a = i.arg;
    return a ? a.done ? (r[e.resultName] = a.value, r.next = e.nextLoc, "return" !== r.method && (r.method = "next", r.arg = t), r.delegate = null, y) : a : (r.method = "throw", r.arg = new TypeError("iterator result is not an object"), r.delegate = null, y);
  }
  function pushTryEntry(t) {
    var e = {
      tryLoc: t[0]
    };
    1 in t && (e.catchLoc = t[1]), 2 in t && (e.finallyLoc = t[2], e.afterLoc = t[3]), this.tryEntries.push(e);
  }
  function resetTryEntry(t) {
    var e = t.completion || {};
    e.type = "normal", delete e.arg, t.completion = e;
  }
  function Context(t) {
    this.tryEntries = [{
      tryLoc: "root"
    }], t.forEach(pushTryEntry, this), this.reset(!0);
  }
  function values(e) {
    if (e || "" === e) {
      var r = e[a];
      if (r) return r.call(e);
      if ("function" == typeof e.next) return e;
      if (!isNaN(e.length)) {
        var o = -1,
          i = function next() {
            for (; ++o < e.length;) if (n.call(e, o)) return next.value = e[o], next.done = !1, next;
            return next.value = t, next.done = !0, next;
          };
        return i.next = i;
      }
    }
    throw new TypeError(_typeof(e) + " is not iterable");
  }
  return GeneratorFunction.prototype = GeneratorFunctionPrototype, o(g, "constructor", {
    value: GeneratorFunctionPrototype,
    configurable: !0
  }), o(GeneratorFunctionPrototype, "constructor", {
    value: GeneratorFunction,
    configurable: !0
  }), GeneratorFunction.displayName = define(GeneratorFunctionPrototype, u, "GeneratorFunction"), e.isGeneratorFunction = function (t) {
    var e = "function" == typeof t && t.constructor;
    return !!e && (e === GeneratorFunction || "GeneratorFunction" === (e.displayName || e.name));
  }, e.mark = function (t) {
    return Object.setPrototypeOf ? Object.setPrototypeOf(t, GeneratorFunctionPrototype) : (t.__proto__ = GeneratorFunctionPrototype, define(t, u, "GeneratorFunction")), t.prototype = Object.create(g), t;
  }, e.awrap = function (t) {
    return {
      __await: t
    };
  }, defineIteratorMethods(AsyncIterator.prototype), define(AsyncIterator.prototype, c, function () {
    return this;
  }), e.AsyncIterator = AsyncIterator, e.async = function (t, r, n, o, i) {
    void 0 === i && (i = Promise);
    var a = new AsyncIterator(wrap(t, r, n, o), i);
    return e.isGeneratorFunction(r) ? a : a.next().then(function (t) {
      return t.done ? t.value : a.next();
    });
  }, defineIteratorMethods(g), define(g, u, "Generator"), define(g, a, function () {
    return this;
  }), define(g, "toString", function () {
    return "[object Generator]";
  }), e.keys = function (t) {
    var e = Object(t),
      r = [];
    for (var n in e) r.push(n);
    return r.reverse(), function next() {
      for (; r.length;) {
        var t = r.pop();
        if (t in e) return next.value = t, next.done = !1, next;
      }
      return next.done = !0, next;
    };
  }, e.values = values, Context.prototype = {
    constructor: Context,
    reset: function reset(e) {
      if (this.prev = 0, this.next = 0, this.sent = this._sent = t, this.done = !1, this.delegate = null, this.method = "next", this.arg = t, this.tryEntries.forEach(resetTryEntry), !e) for (var r in this) "t" === r.charAt(0) && n.call(this, r) && !isNaN(+r.slice(1)) && (this[r] = t);
    },
    stop: function stop() {
      this.done = !0;
      var t = this.tryEntries[0].completion;
      if ("throw" === t.type) throw t.arg;
      return this.rval;
    },
    dispatchException: function dispatchException(e) {
      if (this.done) throw e;
      var r = this;
      function handle(n, o) {
        return a.type = "throw", a.arg = e, r.next = n, o && (r.method = "next", r.arg = t), !!o;
      }
      for (var o = this.tryEntries.length - 1; o >= 0; --o) {
        var i = this.tryEntries[o],
          a = i.completion;
        if ("root" === i.tryLoc) return handle("end");
        if (i.tryLoc <= this.prev) {
          var c = n.call(i, "catchLoc"),
            u = n.call(i, "finallyLoc");
          if (c && u) {
            if (this.prev < i.catchLoc) return handle(i.catchLoc, !0);
            if (this.prev < i.finallyLoc) return handle(i.finallyLoc);
          } else if (c) {
            if (this.prev < i.catchLoc) return handle(i.catchLoc, !0);
          } else {
            if (!u) throw Error("try statement without catch or finally");
            if (this.prev < i.finallyLoc) return handle(i.finallyLoc);
          }
        }
      }
    },
    abrupt: function abrupt(t, e) {
      for (var r = this.tryEntries.length - 1; r >= 0; --r) {
        var o = this.tryEntries[r];
        if (o.tryLoc <= this.prev && n.call(o, "finallyLoc") && this.prev < o.finallyLoc) {
          var i = o;
          break;
        }
      }
      i && ("break" === t || "continue" === t) && i.tryLoc <= e && e <= i.finallyLoc && (i = null);
      var a = i ? i.completion : {};
      return a.type = t, a.arg = e, i ? (this.method = "next", this.next = i.finallyLoc, y) : this.complete(a);
    },
    complete: function complete(t, e) {
      if ("throw" === t.type) throw t.arg;
      return "break" === t.type || "continue" === t.type ? this.next = t.arg : "return" === t.type ? (this.rval = this.arg = t.arg, this.method = "return", this.next = "end") : "normal" === t.type && e && (this.next = e), y;
    },
    finish: function finish(t) {
      for (var e = this.tryEntries.length - 1; e >= 0; --e) {
        var r = this.tryEntries[e];
        if (r.finallyLoc === t) return this.complete(r.completion, r.afterLoc), resetTryEntry(r), y;
      }
    },
    "catch": function _catch(t) {
      for (var e = this.tryEntries.length - 1; e >= 0; --e) {
        var r = this.tryEntries[e];
        if (r.tryLoc === t) {
          var n = r.completion;
          if ("throw" === n.type) {
            var o = n.arg;
            resetTryEntry(r);
          }
          return o;
        }
      }
      throw Error("illegal catch attempt");
    },
    delegateYield: function delegateYield(e, r, n) {
      return this.delegate = {
        iterator: values(e),
        resultName: r,
        nextLoc: n
      }, "next" === this.method && (this.arg = t), y;
    }
  }, e;
}
module.exports = _regeneratorRuntime, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 25 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_loader_node_modules_vue_loader_lib_selector_type_script_index_0_treeNode_vue__ = __webpack_require__(14);
/* unused harmony namespace reexport */
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__node_modules_vue_loader_lib_template_compiler_index_id_data_v_1d1d4eb0_hasScoped_false_buble_transforms_node_modules_vue_loader_lib_selector_type_template_index_0_treeNode_vue__ = __webpack_require__(28);
var disposed = false
var normalizeComponent = __webpack_require__(3)
/* script */


/* template */

/* template functional */
var __vue_template_functional__ = false
/* styles */
var __vue_styles__ = null
/* scopeId */
var __vue_scopeId__ = null
/* moduleIdentifier (server only) */
var __vue_module_identifier__ = null
var Component = normalizeComponent(
  __WEBPACK_IMPORTED_MODULE_0__babel_loader_node_modules_vue_loader_lib_selector_type_script_index_0_treeNode_vue__["a" /* default */],
  __WEBPACK_IMPORTED_MODULE_1__node_modules_vue_loader_lib_template_compiler_index_id_data_v_1d1d4eb0_hasScoped_false_buble_transforms_node_modules_vue_loader_lib_selector_type_template_index_0_treeNode_vue__["a" /* default */],
  __vue_template_functional__,
  __vue_styles__,
  __vue_scopeId__,
  __vue_module_identifier__
)
Component.options.__file = "gw-base-components-plus/treeV2/components/treeNode/treeNode.vue"

/* hot reload */
if (false) {(function () {
  var hotAPI = require("vue-hot-reload-api")
  hotAPI.install(require("vue"), false)
  if (!hotAPI.compatible) return
  module.hot.accept()
  if (!module.hot.data) {
    hotAPI.createRecord("data-v-1d1d4eb0", Component.options)
  } else {
    hotAPI.reload("data-v-1d1d4eb0", Component.options)
  }
  module.hot.dispose(function (data) {
    disposed = true
  })
})()}

/* harmony default export */ __webpack_exports__["a"] = (Component.exports);


/***/ }),
/* 26 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_loader_node_modules_vue_loader_lib_selector_type_script_index_0_oparate_vue__ = __webpack_require__(15);
/* unused harmony namespace reexport */
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__node_modules_vue_loader_lib_template_compiler_index_id_data_v_1824beb2_hasScoped_false_buble_transforms_node_modules_vue_loader_lib_selector_type_template_index_0_oparate_vue__ = __webpack_require__(27);
var disposed = false
var normalizeComponent = __webpack_require__(3)
/* script */


/* template */

/* template functional */
var __vue_template_functional__ = false
/* styles */
var __vue_styles__ = null
/* scopeId */
var __vue_scopeId__ = null
/* moduleIdentifier (server only) */
var __vue_module_identifier__ = null
var Component = normalizeComponent(
  __WEBPACK_IMPORTED_MODULE_0__babel_loader_node_modules_vue_loader_lib_selector_type_script_index_0_oparate_vue__["a" /* default */],
  __WEBPACK_IMPORTED_MODULE_1__node_modules_vue_loader_lib_template_compiler_index_id_data_v_1824beb2_hasScoped_false_buble_transforms_node_modules_vue_loader_lib_selector_type_template_index_0_oparate_vue__["a" /* default */],
  __vue_template_functional__,
  __vue_styles__,
  __vue_scopeId__,
  __vue_module_identifier__
)
Component.options.__file = "gw-base-components-plus/treeV2/components/oparate/oparate.vue"

/* hot reload */
if (false) {(function () {
  var hotAPI = require("vue-hot-reload-api")
  hotAPI.install(require("vue"), false)
  if (!hotAPI.compatible) return
  module.hot.accept()
  if (!module.hot.data) {
    hotAPI.createRecord("data-v-1824beb2", Component.options)
  } else {
    hotAPI.reload("data-v-1824beb2", Component.options)
  }
  module.hot.dispose(function (data) {
    disposed = true
  })
})()}

/* harmony default export */ __webpack_exports__["a"] = (Component.exports);


/***/ }),
/* 27 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
var render = function() {
  var _vm = this
  var _h = _vm.$createElement
  var _c = _vm._self._c || _h
  return _c(
    "div",
    { staticClass: "operates" },
    [
      _c(
        "el-popover",
        { attrs: { placement: "bottom", trigger: "hover" } },
        [
          _c(
            "el-button-group",
            { staticClass: "new-button-group setModule" },
            [
              _c(
                "el-button",
                {
                  staticClass: "elBtn",
                  attrs: {
                    type: "text",
                    size: "small",
                    icon: " iconfont icon-tubiao20_bianji"
                  },
                  on: {
                    click: function($event) {
                      $event.stopPropagation()
                      return _vm.groupEditAndNew(false, _vm.source)
                    }
                  }
                },
                [
                  _vm._v(
                    "\n                " +
                      _vm._s(_vm.$t("equipInfo.poverTips.rename")) +
                      "\n            "
                  )
                ]
              ),
              _vm._v(" "),
              _c(
                "el-button",
                {
                  staticClass: "elBtn",
                  attrs: {
                    type: "text",
                    size: "small",
                    icon: " iconfont icon-gw-icon-tianjia1"
                  },
                  on: {
                    click: function($event) {
                      return _vm.groupEditAndNew(true, _vm.source)
                    }
                  }
                },
                [
                  _vm._v(
                    "\n                " +
                      _vm._s(_vm.$t("equipInfo.poverTips.newChildGroup")) +
                      "\n            "
                  )
                ]
              ),
              _vm._v(" "),
              _vm.source.level != 1
                ? _c(
                    "el-button",
                    {
                      attrs: {
                        type: "danger",
                        size: "small",
                        icon: " iconfont icon-tubiao20_shanchu"
                      },
                      on: {
                        click: function($event) {
                          $event.stopPropagation()
                          return _vm.deleteGroup(_vm.source)
                        }
                      }
                    },
                    [
                      _vm._v(
                        "\n                " +
                          _vm._s(_vm.$t("publics.button.deletes")) +
                          "\n            "
                      )
                    ]
                  )
                : _vm._e()
            ],
            1
          ),
          _vm._v(" "),
          _c("i", {
            staticClass: "el-icon-more",
            attrs: { slot: "reference" },
            on: {
              click: function($event) {
                $event.stopPropagation()
              }
            },
            slot: "reference"
          })
        ],
        1
      )
    ],
    1
  )
}
var staticRenderFns = []
render._withStripped = true
var esExports = { render: render, staticRenderFns: staticRenderFns }
/* harmony default export */ __webpack_exports__["a"] = (esExports);
if (false) {
  module.hot.accept()
  if (module.hot.data) {
    require("vue-hot-reload-api")      .rerender("data-v-1824beb2", esExports)
  }
}

/***/ }),
/* 28 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
var render = function() {
  var _vm = this
  var _h = _vm.$createElement
  var _c = _vm._self._c || _h
  return _c(
    "div",
    {
      staticClass: "el-tree-node",
      class: {
        parent_tag: _vm.source.isGroup,
        child_tags: !_vm.source.isGroup,
        selectedColor:
          _vm.currentSelect === _vm.source.key && !_vm.source.isGroup
      },
      on: { click: _vm.clickFunction }
    },
    [
      _c("div", { staticClass: "el-tree-node__content" }, [
        _c("span", {
          staticClass: "el-tree__indent",
          style: { width: (_vm.source.level - 1) * _vm.indent + "px" },
          attrs: { "aria-hidden": "true" }
        }),
        _vm._v(" "),
        _vm.source.isGroup
          ? _c("span", {
              class: [
                {
                  "is-leaf": !_vm.source.isGroup,
                  expanded: _vm.source.isGroup && _vm.source.expand
                },
                "el-tree-node__expand-icon",
                "el-icon-arrow-right"
              ]
            })
          : _vm._e(),
        _vm._v(" "),
        _c(
          "div",
          { staticClass: "nodeContent" },
          [
            _vm.showCheckbox
              ? _c("el-checkbox", {
                  attrs: { indeterminate: _vm.source.indeterminate },
                  on: { change: _vm.checkedChange },
                  nativeOn: {
                    click: function($event) {
                      $event.stopPropagation()
                    }
                  },
                  model: {
                    value: _vm.source.checked,
                    callback: function($$v) {
                      _vm.$set(_vm.source, "checked", $$v)
                    },
                    expression: "source.checked"
                  }
                })
              : _vm._e(),
            _vm._v(" "),
            _vm.showStatus
              ? _c("span", { staticClass: "circle" }, [
                  _vm.source.status != 6
                    ? _c("span", {
                        staticClass: "yd",
                        style: {
                          backgroundColor: _vm.getColor(_vm.source.status)
                        }
                      })
                    : _c("i", {
                        staticClass: "iconfont icon-gw-icon-beiji2",
                        style: { color: _vm.colorConfig.BackUp }
                      })
                ])
              : _vm._e(),
            _vm._v(" "),
            _c("span", { staticClass: "label" }, [
              _vm._v(" " + _vm._s(_vm.source.title))
            ]),
            _vm._v(" "),
            (_vm.source.isGroup && !_vm.source.isEquip) ||
            (_vm.source.isGroup && !_vm.source.isEquip)
              ? _c("span", { staticClass: "equipNumber" }, [
                  _vm._v(
                    "\n                " +
                      _vm._s(_vm.source.count) +
                      "\n            "
                  )
                ])
              : _vm._e(),
            _vm._v(" "),
            _vm.source.isGroup && _vm.showOperate
              ? _c("oparate", {
                  attrs: {
                    source: _vm.source,
                    groupEditAndNew: _vm.groupEditAndNew,
                    deleteGroup: _vm.deleteGroup
                  }
                })
              : _vm._e(),
            _vm._v(" "),
            _vm.source.loading
              ? _c("span", {
                  staticClass: "el-tree-node__loading-icon el-icon-loading"
                })
              : _vm._e()
          ],
          1
        )
      ])
    ]
  )
}
var staticRenderFns = []
render._withStripped = true
var esExports = { render: render, staticRenderFns: staticRenderFns }
/* harmony default export */ __webpack_exports__["a"] = (esExports);
if (false) {
  module.hot.accept()
  if (module.hot.data) {
    require("vue-hot-reload-api")      .rerender("data-v-1d1d4eb0", esExports)
  }
}

/***/ }),
/* 29 */
/***/ (function(module, exports, __webpack_require__) {

/*!
 * vue-virtual-scroll-list v2.3.4
 * open source under the MIT license
 * https://github.com/tangbc/vue-virtual-scroll-list#readme
 */

(function (global, factory) {
   true ? module.exports = factory(__webpack_require__(30)) :
  typeof define === 'function' && define.amd ? define(['vue'], factory) :
  (global = global || self, global.VirtualList = factory(global.Vue));
}(this, (function (Vue) { 'use strict';

  Vue = Vue && Object.prototype.hasOwnProperty.call(Vue, 'default') ? Vue['default'] : Vue;

  function ownKeys(object, enumerableOnly) {
    var keys = Object.keys(object);
    if (Object.getOwnPropertySymbols) {
      var symbols = Object.getOwnPropertySymbols(object);
      enumerableOnly && (symbols = symbols.filter(function (sym) {
        return Object.getOwnPropertyDescriptor(object, sym).enumerable;
      })), keys.push.apply(keys, symbols);
    }
    return keys;
  }
  function _objectSpread2(target) {
    for (var i = 1; i < arguments.length; i++) {
      var source = null != arguments[i] ? arguments[i] : {};
      i % 2 ? ownKeys(Object(source), !0).forEach(function (key) {
        _defineProperty(target, key, source[key]);
      }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)) : ownKeys(Object(source)).forEach(function (key) {
        Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key));
      });
    }
    return target;
  }
  function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  }
  function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
      var descriptor = props[i];
      descriptor.enumerable = descriptor.enumerable || false;
      descriptor.configurable = true;
      if ("value" in descriptor) descriptor.writable = true;
      Object.defineProperty(target, descriptor.key, descriptor);
    }
  }
  function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) _defineProperties(Constructor.prototype, protoProps);
    if (staticProps) _defineProperties(Constructor, staticProps);
    Object.defineProperty(Constructor, "prototype", {
      writable: false
    });
    return Constructor;
  }
  function _defineProperty(obj, key, value) {
    if (key in obj) {
      Object.defineProperty(obj, key, {
        value: value,
        enumerable: true,
        configurable: true,
        writable: true
      });
    } else {
      obj[key] = value;
    }
    return obj;
  }
  function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread();
  }
  function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) return _arrayLikeToArray(arr);
  }
  function _iterableToArray(iter) {
    if (typeof Symbol !== "undefined" && iter[Symbol.iterator] != null || iter["@@iterator"] != null) return Array.from(iter);
  }
  function _unsupportedIterableToArray(o, minLen) {
    if (!o) return;
    if (typeof o === "string") return _arrayLikeToArray(o, minLen);
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if (n === "Object" && o.constructor) n = o.constructor.name;
    if (n === "Map" || n === "Set") return Array.from(o);
    if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen);
  }
  function _arrayLikeToArray(arr, len) {
    if (len == null || len > arr.length) len = arr.length;
    for (var i = 0, arr2 = new Array(len); i < len; i++) arr2[i] = arr[i];
    return arr2;
  }
  function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.");
  }

  /**
   * virtual list core calculating center
   */

  var DIRECTION_TYPE = {
    FRONT: 'FRONT',
    // scroll up or left
    BEHIND: 'BEHIND' // scroll down or right
  };

  var CALC_TYPE = {
    INIT: 'INIT',
    FIXED: 'FIXED',
    DYNAMIC: 'DYNAMIC'
  };
  var LEADING_BUFFER = 0;
  var Virtual = /*#__PURE__*/function () {
    function Virtual(param, callUpdate) {
      _classCallCheck(this, Virtual);
      this.init(param, callUpdate);
    }
    _createClass(Virtual, [{
      key: "init",
      value: function init(param, callUpdate) {
        // param data
        this.param = param;
        this.callUpdate = callUpdate;

        // size data
        this.sizes = new Map();
        this.firstRangeTotalSize = 0;
        this.firstRangeAverageSize = 0;
        this.fixedSizeValue = 0;
        this.calcType = CALC_TYPE.INIT;

        // scroll data
        this.offset = 0;
        this.direction = '';

        // range data
        this.range = Object.create(null);
        if (param) {
          this.checkRange(0, param.keeps - 1);
        }

        // benchmark test data
        // this.__bsearchCalls = 0
        // this.__getIndexOffsetCalls = 0
      }
    }, {
      key: "destroy",
      value: function destroy() {
        this.init(null, null);
      }

      // return current render range
    }, {
      key: "getRange",
      value: function getRange() {
        var range = Object.create(null);
        range.start = this.range.start;
        range.end = this.range.end;
        range.padFront = this.range.padFront;
        range.padBehind = this.range.padBehind;
        return range;
      }
    }, {
      key: "isBehind",
      value: function isBehind() {
        return this.direction === DIRECTION_TYPE.BEHIND;
      }
    }, {
      key: "isFront",
      value: function isFront() {
        return this.direction === DIRECTION_TYPE.FRONT;
      }

      // return start index offset
    }, {
      key: "getOffset",
      value: function getOffset(start) {
        return (start < 1 ? 0 : this.getIndexOffset(start)) + this.param.slotHeaderSize;
      }
    }, {
      key: "updateParam",
      value: function updateParam(key, value) {
        var _this = this;
        if (this.param && key in this.param) {
          // if uniqueIds change, find out deleted id and remove from size map
          if (key === 'uniqueIds') {
            this.sizes.forEach(function (v, key) {
              if (!value.includes(key)) {
                _this.sizes["delete"](key);
              }
            });
          }
          this.param[key] = value;
        }
      }

      // save each size map by id
    }, {
      key: "saveSize",
      value: function saveSize(id, size) {
        this.sizes.set(id, size);

        // we assume size type is fixed at the beginning and remember first size value
        // if there is no size value different from this at next comming saving
        // we think it's a fixed size list, otherwise is dynamic size list
        if (this.calcType === CALC_TYPE.INIT) {
          this.fixedSizeValue = size;
          this.calcType = CALC_TYPE.FIXED;
        } else if (this.calcType === CALC_TYPE.FIXED && this.fixedSizeValue !== size) {
          this.calcType = CALC_TYPE.DYNAMIC;
          // it's no use at all
          delete this.fixedSizeValue;
        }

        // calculate the average size only in the first range
        if (this.calcType !== CALC_TYPE.FIXED && typeof this.firstRangeTotalSize !== 'undefined') {
          if (this.sizes.size < Math.min(this.param.keeps, this.param.uniqueIds.length)) {
            this.firstRangeTotalSize = _toConsumableArray(this.sizes.values()).reduce(function (acc, val) {
              return acc + val;
            }, 0);
            this.firstRangeAverageSize = Math.round(this.firstRangeTotalSize / this.sizes.size);
          } else {
            // it's done using
            delete this.firstRangeTotalSize;
          }
        }
      }

      // in some special situation (e.g. length change) we need to update in a row
      // try goiong to render next range by a leading buffer according to current direction
    }, {
      key: "handleDataSourcesChange",
      value: function handleDataSourcesChange() {
        var start = this.range.start;
        if (this.isFront()) {
          start = start - LEADING_BUFFER;
        } else if (this.isBehind()) {
          start = start + LEADING_BUFFER;
        }
        start = Math.max(start, 0);
        this.updateRange(this.range.start, this.getEndByStart(start));
      }

      // when slot size change, we also need force update
    }, {
      key: "handleSlotSizeChange",
      value: function handleSlotSizeChange() {
        this.handleDataSourcesChange();
      }

      // calculating range on scroll
    }, {
      key: "handleScroll",
      value: function handleScroll(offset) {
        this.direction = offset < this.offset || offset === 0 ? DIRECTION_TYPE.FRONT : DIRECTION_TYPE.BEHIND;
        this.offset = offset;
        if (!this.param) {
          return;
        }
        if (this.direction === DIRECTION_TYPE.FRONT) {
          this.handleFront();
        } else if (this.direction === DIRECTION_TYPE.BEHIND) {
          this.handleBehind();
        }
      }

      // ----------- public method end -----------
    }, {
      key: "handleFront",
      value: function handleFront() {
        var overs = this.getScrollOvers();
        // should not change range if start doesn't exceed overs
        if (overs > this.range.start) {
          return;
        }

        // move up start by a buffer length, and make sure its safety
        var start = Math.max(overs - this.param.buffer, 0);
        this.checkRange(start, this.getEndByStart(start));
      }
    }, {
      key: "handleBehind",
      value: function handleBehind() {
        var overs = this.getScrollOvers();
        // range should not change if scroll overs within buffer
        if (overs < this.range.start + this.param.buffer) {
          return;
        }
        this.checkRange(overs, this.getEndByStart(overs));
      }

      // return the pass overs according to current scroll offset
    }, {
      key: "getScrollOvers",
      value: function getScrollOvers() {
        // if slot header exist, we need subtract its size
        var offset = this.offset - this.param.slotHeaderSize;
        if (offset <= 0) {
          return 0;
        }

        // if is fixed type, that can be easily
        if (this.isFixedType()) {
          return Math.floor(offset / this.fixedSizeValue);
        }
        var low = 0;
        var middle = 0;
        var middleOffset = 0;
        var high = this.param.uniqueIds.length;
        while (low <= high) {
          // this.__bsearchCalls++
          middle = low + Math.floor((high - low) / 2);
          middleOffset = this.getIndexOffset(middle);
          if (middleOffset === offset) {
            return middle;
          } else if (middleOffset < offset) {
            low = middle + 1;
          } else if (middleOffset > offset) {
            high = middle - 1;
          }
        }
        return low > 0 ? --low : 0;
      }

      // return a scroll offset from given index, can efficiency be improved more here?
      // although the call frequency is very high, its only a superposition of numbers
    }, {
      key: "getIndexOffset",
      value: function getIndexOffset(givenIndex) {
        if (!givenIndex) {
          return 0;
        }
        var offset = 0;
        var indexSize = 0;
        for (var index = 0; index < givenIndex; index++) {
          // this.__getIndexOffsetCalls++
          indexSize = this.sizes.get(this.param.uniqueIds[index]);
          offset = offset + (typeof indexSize === 'number' ? indexSize : this.getEstimateSize());
        }
        return offset;
      }

      // is fixed size type
    }, {
      key: "isFixedType",
      value: function isFixedType() {
        return this.calcType === CALC_TYPE.FIXED;
      }

      // return the real last index
    }, {
      key: "getLastIndex",
      value: function getLastIndex() {
        return this.param.uniqueIds.length - 1;
      }

      // in some conditions range is broke, we need correct it
      // and then decide whether need update to next range
    }, {
      key: "checkRange",
      value: function checkRange(start, end) {
        var keeps = this.param.keeps;
        var total = this.param.uniqueIds.length;

        // datas less than keeps, render all
        if (total <= keeps) {
          start = 0;
          end = this.getLastIndex();
        } else if (end - start < keeps - 1) {
          // if range length is less than keeps, corrent it base on end
          start = end - keeps + 1;
        }
        if (this.range.start !== start) {
          this.updateRange(start, end);
        }
      }

      // setting to a new range and rerender
    }, {
      key: "updateRange",
      value: function updateRange(start, end) {
        this.range.start = start;
        this.range.end = end;
        this.range.padFront = this.getPadFront();
        this.range.padBehind = this.getPadBehind();
        this.callUpdate(this.getRange());
      }

      // return end base on start
    }, {
      key: "getEndByStart",
      value: function getEndByStart(start) {
        var theoryEnd = start + this.param.keeps - 1;
        var truelyEnd = Math.min(theoryEnd, this.getLastIndex());
        return truelyEnd;
      }

      // return total front offset
    }, {
      key: "getPadFront",
      value: function getPadFront() {
        if (this.isFixedType()) {
          return this.fixedSizeValue * this.range.start;
        } else {
          return this.getIndexOffset(this.range.start);
        }
      }

      // return total behind offset
    }, {
      key: "getPadBehind",
      value: function getPadBehind() {
        var end = this.range.end;
        var lastIndex = this.getLastIndex();
        if (this.isFixedType()) {
          return (lastIndex - end) * this.fixedSizeValue;
        }
        return (lastIndex - end) * this.getEstimateSize();
      }

      // get the item estimate size
    }, {
      key: "getEstimateSize",
      value: function getEstimateSize() {
        return this.isFixedType() ? this.fixedSizeValue : this.firstRangeAverageSize || this.param.estimateSize;
      }
    }]);
    return Virtual;
  }();

  /**
   * props declaration for default, item and slot component
   */

  var VirtualProps = {
    dataKey: {
      type: [String, Function],
      required: true
    },
    dataSources: {
      type: Array,
      required: true
    },
    dataComponent: {
      type: [Object, Function],
      required: true
    },
    keeps: {
      type: Number,
      "default": 30
    },
    extraProps: {
      type: Object
    },
    estimateSize: {
      type: Number,
      "default": 50
    },
    direction: {
      type: String,
      "default": 'vertical' // the other value is horizontal
    },

    start: {
      type: Number,
      "default": 0
    },
    offset: {
      type: Number,
      "default": 0
    },
    topThreshold: {
      type: Number,
      "default": 0
    },
    bottomThreshold: {
      type: Number,
      "default": 0
    },
    pageMode: {
      type: Boolean,
      "default": false
    },
    rootTag: {
      type: String,
      "default": 'div'
    },
    wrapTag: {
      type: String,
      "default": 'div'
    },
    wrapClass: {
      type: String,
      "default": ''
    },
    wrapStyle: {
      type: Object
    },
    itemTag: {
      type: String,
      "default": 'div'
    },
    itemClass: {
      type: String,
      "default": ''
    },
    itemClassAdd: {
      type: Function
    },
    itemStyle: {
      type: Object
    },
    headerTag: {
      type: String,
      "default": 'div'
    },
    headerClass: {
      type: String,
      "default": ''
    },
    headerStyle: {
      type: Object
    },
    footerTag: {
      type: String,
      "default": 'div'
    },
    footerClass: {
      type: String,
      "default": ''
    },
    footerStyle: {
      type: Object
    },
    itemScopedSlots: {
      type: Object
    }
  };
  var ItemProps = {
    index: {
      type: Number
    },
    event: {
      type: String
    },
    tag: {
      type: String
    },
    horizontal: {
      type: Boolean
    },
    source: {
      type: Object
    },
    component: {
      type: [Object, Function]
    },
    slotComponent: {
      type: Function
    },
    uniqueKey: {
      type: [String, Number]
    },
    extraProps: {
      type: Object
    },
    scopedSlots: {
      type: Object
    }
  };
  var SlotProps = {
    event: {
      type: String
    },
    uniqueKey: {
      type: String
    },
    tag: {
      type: String
    },
    horizontal: {
      type: Boolean
    }
  };

  var Wrapper = {
    created: function created() {
      this.shapeKey = this.horizontal ? 'offsetWidth' : 'offsetHeight';
    },
    mounted: function mounted() {
      var _this = this;
      if (typeof ResizeObserver !== 'undefined') {
        this.resizeObserver = new ResizeObserver(function () {
          _this.dispatchSizeChange();
        });
        this.resizeObserver.observe(this.$el);
      }
    },
    // since componet will be reused, so disptach when updated
    updated: function updated() {
      // this.dispatchSizeChange()
      this.resizeObserver.observe(this.$el);
    },
    beforeDestroy: function beforeDestroy() {
      if (this.resizeObserver) {
        this.resizeObserver.disconnect();
        this.resizeObserver = null;
      }
    },
    methods: {
      getCurrentSize: function getCurrentSize() {
        return this.$el ? this.$el[this.shapeKey] : 0;
      },
      // tell parent current size identify by unqiue key
      dispatchSizeChange: function dispatchSizeChange() {
        this.$parent.$emit(this.event, this.uniqueKey, this.getCurrentSize(), this.hasInitial);
      }
    }
  };

  // wrapping for item
  var Item = Vue.component('virtual-list-item', {
    mixins: [Wrapper],
    props: ItemProps,
    render: function render(h) {
      var tag = this.tag,
        component = this.component,
        _this$extraProps = this.extraProps,
        extraProps = _this$extraProps === void 0 ? {} : _this$extraProps,
        index = this.index,
        source = this.source,
        _this$scopedSlots = this.scopedSlots,
        scopedSlots = _this$scopedSlots === void 0 ? {} : _this$scopedSlots,
        uniqueKey = this.uniqueKey,
        slotComponent = this.slotComponent;
      var props = _objectSpread2(_objectSpread2({}, extraProps), {}, {
        source: source,
        index: index
      });
      return h(tag, {
        key: uniqueKey,
        attrs: {
          role: 'listitem'
        }
      }, [slotComponent ? slotComponent({
        item: source,
        index: index,
        scope: props
      }) : h(component, {
        props: props,
        scopedSlots: scopedSlots
      })]);
    }
  });

  // wrapping for slot
  var Slot = Vue.component('virtual-list-slot', {
    mixins: [Wrapper],
    props: SlotProps,
    render: function render(h) {
      var tag = this.tag,
        uniqueKey = this.uniqueKey;
      return h(tag, {
        key: uniqueKey,
        attrs: {
          role: uniqueKey
        }
      }, this.$slots["default"]);
    }
  });

  /**
   * virtual list default component
   */
  var EVENT_TYPE = {
    ITEM: 'item_resize',
    SLOT: 'slot_resize'
  };
  var SLOT_TYPE = {
    HEADER: 'thead',
    // string value also use for aria role attribute
    FOOTER: 'tfoot'
  };
  var VirtualList = Vue.component('virtual-list', {
    props: VirtualProps,
    data: function data() {
      return {
        range: null
      };
    },
    watch: {
      'dataSources.length': function dataSourcesLength() {
        this.virtual.updateParam('uniqueIds', this.getUniqueIdFromDataSources());
        this.virtual.handleDataSourcesChange();
      },
      keeps: function keeps(newValue) {
        this.virtual.updateParam('keeps', newValue);
        this.virtual.handleSlotSizeChange();
      },
      start: function start(newValue) {
        this.scrollToIndex(newValue);
      },
      offset: function offset(newValue) {
        this.scrollToOffset(newValue);
      }
    },
    created: function created() {
      this.isHorizontal = this.direction === 'horizontal';
      this.directionKey = this.isHorizontal ? 'scrollLeft' : 'scrollTop';
      this.installVirtual();

      // listen item size change
      this.$on(EVENT_TYPE.ITEM, this.onItemResized);

      // listen slot size change
      if (this.$slots.header || this.$slots.footer) {
        this.$on(EVENT_TYPE.SLOT, this.onSlotResized);
      }
    },
    activated: function activated() {
      // set back offset when awake from keep-alive
      this.scrollToOffset(this.virtual.offset);
      if (this.pageMode) {
        document.addEventListener('scroll', this.onScroll, {
          passive: false
        });
      }
    },
    deactivated: function deactivated() {
      if (this.pageMode) {
        document.removeEventListener('scroll', this.onScroll);
      }
    },
    mounted: function mounted() {
      // set position
      if (this.start) {
        this.scrollToIndex(this.start);
      } else if (this.offset) {
        this.scrollToOffset(this.offset);
      }

      // in page mode we bind scroll event to document
      if (this.pageMode) {
        this.updatePageModeFront();
        document.addEventListener('scroll', this.onScroll, {
          passive: false
        });
      }
    },
    beforeDestroy: function beforeDestroy() {
      this.virtual.destroy();
      if (this.pageMode) {
        document.removeEventListener('scroll', this.onScroll);
      }
    },
    methods: {
      // get item size by id
      getSize: function getSize(id) {
        return this.virtual.sizes.get(id);
      },
      // get the total number of stored (rendered) items
      getSizes: function getSizes() {
        return this.virtual.sizes.size;
      },
      // return current scroll offset
      getOffset: function getOffset() {
        if (this.pageMode) {
          return document.documentElement[this.directionKey] || document.body[this.directionKey];
        } else {
          var root = this.$refs.root;
          return root ? Math.ceil(root[this.directionKey]) : 0;
        }
      },
      // return client viewport size
      getClientSize: function getClientSize() {
        var key = this.isHorizontal ? 'clientWidth' : 'clientHeight';
        if (this.pageMode) {
          return document.documentElement[key] || document.body[key];
        } else {
          var root = this.$refs.root;
          return root ? Math.ceil(root[key]) : 0;
        }
      },
      // return all scroll size
      getScrollSize: function getScrollSize() {
        var key = this.isHorizontal ? 'scrollWidth' : 'scrollHeight';
        if (this.pageMode) {
          return document.documentElement[key] || document.body[key];
        } else {
          var root = this.$refs.root;
          return root ? Math.ceil(root[key]) : 0;
        }
      },
      // set current scroll position to a expectant offset
      scrollToOffset: function scrollToOffset(offset) {
        if (this.pageMode) {
          document.body[this.directionKey] = offset;
          document.documentElement[this.directionKey] = offset;
        } else {
          var root = this.$refs.root;
          if (root) {
            root[this.directionKey] = offset;
          }
        }
      },
      // set current scroll position to a expectant index
      scrollToIndex: function scrollToIndex(index) {
        // scroll to bottom
        if (index >= this.dataSources.length - 1) {
          this.scrollToBottom();
        } else {
          var offset = this.virtual.getOffset(index);
          this.scrollToOffset(offset);
        }
      },
      // set current scroll position to bottom
      scrollToBottom: function scrollToBottom() {
        var _this = this;
        var shepherd = this.$refs.shepherd;
        if (shepherd) {
          var offset = shepherd[this.isHorizontal ? 'offsetLeft' : 'offsetTop'];
          this.scrollToOffset(offset);

          // check if it's really scrolled to the bottom
          // maybe list doesn't render and calculate to last range
          // so we need retry in next event loop until it really at bottom
          setTimeout(function () {
            if (_this.getOffset() + _this.getClientSize() + 1 < _this.getScrollSize()) {
              _this.scrollToBottom();
            }
          }, 3);
        }
      },
      // when using page mode we need update slot header size manually
      // taking root offset relative to the browser as slot header size
      updatePageModeFront: function updatePageModeFront() {
        var root = this.$refs.root;
        if (root) {
          var rect = root.getBoundingClientRect();
          var defaultView = root.ownerDocument.defaultView;
          var offsetFront = this.isHorizontal ? rect.left + defaultView.pageXOffset : rect.top + defaultView.pageYOffset;
          this.virtual.updateParam('slotHeaderSize', offsetFront);
        }
      },
      // reset all state back to initial
      reset: function reset() {
        this.virtual.destroy();
        this.scrollToOffset(0);
        this.installVirtual();
      },
      // ----------- public method end -----------
      installVirtual: function installVirtual() {
        this.virtual = new Virtual({
          slotHeaderSize: 0,
          slotFooterSize: 0,
          keeps: this.keeps,
          estimateSize: this.estimateSize,
          buffer: Math.round(this.keeps / 3),
          // recommend for a third of keeps
          uniqueIds: this.getUniqueIdFromDataSources()
        }, this.onRangeChanged);

        // sync initial range
        this.range = this.virtual.getRange();
      },
      getUniqueIdFromDataSources: function getUniqueIdFromDataSources() {
        var dataKey = this.dataKey;
        return this.dataSources.map(function (dataSource) {
          return typeof dataKey === 'function' ? dataKey(dataSource) : dataSource[dataKey];
        });
      },
      // event called when each item mounted or size changed
      onItemResized: function onItemResized(id, size) {
        this.virtual.saveSize(id, size);
        this.$emit('resized', id, size);
      },
      // event called when slot mounted or size changed
      onSlotResized: function onSlotResized(type, size, hasInit) {
        if (type === SLOT_TYPE.HEADER) {
          this.virtual.updateParam('slotHeaderSize', size);
        } else if (type === SLOT_TYPE.FOOTER) {
          this.virtual.updateParam('slotFooterSize', size);
        }
        if (hasInit) {
          this.virtual.handleSlotSizeChange();
        }
      },
      // here is the rerendering entry
      onRangeChanged: function onRangeChanged(range) {
        this.range = range;
      },
      onScroll: function onScroll(evt) {
        var offset = this.getOffset();
        var clientSize = this.getClientSize();
        var scrollSize = this.getScrollSize();

        // iOS scroll-spring-back behavior will make direction mistake
        if (offset < 0 || offset + clientSize > scrollSize + 1 || !scrollSize) {
          return;
        }
        this.virtual.handleScroll(offset);
        this.emitEvent(offset, clientSize, scrollSize, evt);
      },
      // emit event in special position
      emitEvent: function emitEvent(offset, clientSize, scrollSize, evt) {
        this.$emit('scroll', evt, this.virtual.getRange());
        if (this.virtual.isFront() && !!this.dataSources.length && offset - this.topThreshold <= 0) {
          this.$emit('totop');
        } else if (this.virtual.isBehind() && offset + clientSize + this.bottomThreshold >= scrollSize) {
          this.$emit('tobottom');
        }
      },
      // get the real render slots based on range data
      // in-place patch strategy will try to reuse components as possible
      // so those components that are reused will not trigger lifecycle mounted
      getRenderSlots: function getRenderSlots(h) {
        var slots = [];
        var _this$range = this.range,
          start = _this$range.start,
          end = _this$range.end;
        var dataSources = this.dataSources,
          dataKey = this.dataKey,
          itemClass = this.itemClass,
          itemTag = this.itemTag,
          itemStyle = this.itemStyle,
          isHorizontal = this.isHorizontal,
          extraProps = this.extraProps,
          dataComponent = this.dataComponent,
          itemScopedSlots = this.itemScopedSlots;
        var slotComponent = this.$scopedSlots && this.$scopedSlots.item;
        for (var index = start; index <= end; index++) {
          var dataSource = dataSources[index];
          if (dataSource) {
            var uniqueKey = typeof dataKey === 'function' ? dataKey(dataSource) : dataSource[dataKey];
            if (typeof uniqueKey === 'string' || typeof uniqueKey === 'number') {
              slots.push(h(Item, {
                props: {
                  index: index,
                  tag: itemTag,
                  event: EVENT_TYPE.ITEM,
                  horizontal: isHorizontal,
                  uniqueKey: uniqueKey,
                  source: dataSource,
                  extraProps: extraProps,
                  component: dataComponent,
                  slotComponent: slotComponent,
                  scopedSlots: itemScopedSlots
                },
                style: itemStyle,
                "class": "".concat(itemClass).concat(this.itemClassAdd ? ' ' + this.itemClassAdd(index) : '')
              }));
            } else {
              console.warn("Cannot get the data-key '".concat(dataKey, "' from data-sources."));
            }
          } else {
            console.warn("Cannot get the index '".concat(index, "' from data-sources."));
          }
        }
        return slots;
      }
    },
    // render function, a closer-to-the-compiler alternative to templates
    // https://vuejs.org/v2/guide/render-function.html#The-Data-Object-In-Depth
    render: function render(h) {
      var _this$$slots = this.$slots,
        header = _this$$slots.header,
        footer = _this$$slots.footer;
      var _this$range2 = this.range,
        padFront = _this$range2.padFront,
        padBehind = _this$range2.padBehind;
      var isHorizontal = this.isHorizontal,
        pageMode = this.pageMode,
        rootTag = this.rootTag,
        wrapTag = this.wrapTag,
        wrapClass = this.wrapClass,
        wrapStyle = this.wrapStyle,
        headerTag = this.headerTag,
        headerClass = this.headerClass,
        headerStyle = this.headerStyle,
        footerTag = this.footerTag,
        footerClass = this.footerClass,
        footerStyle = this.footerStyle;
      var paddingStyle = {
        padding: isHorizontal ? "0px ".concat(padBehind, "px 0px ").concat(padFront, "px") : "".concat(padFront, "px 0px ").concat(padBehind, "px")
      };
      var wrapperStyle = wrapStyle ? Object.assign({}, wrapStyle, paddingStyle) : paddingStyle;
      return h(rootTag, {
        ref: 'root',
        on: {
          '&scroll': !pageMode && this.onScroll
        }
      }, [
      // header slot
      header ? h(Slot, {
        "class": headerClass,
        style: headerStyle,
        props: {
          tag: headerTag,
          event: EVENT_TYPE.SLOT,
          uniqueKey: SLOT_TYPE.HEADER
        }
      }, header) : null,
      // main list
      h(wrapTag, {
        "class": wrapClass,
        attrs: {
          role: 'group'
        },
        style: wrapperStyle
      }, this.getRenderSlots(h)),
      // footer slot
      footer ? h(Slot, {
        "class": footerClass,
        style: footerStyle,
        props: {
          tag: footerTag,
          event: EVENT_TYPE.SLOT,
          uniqueKey: SLOT_TYPE.FOOTER
        }
      }, footer) : null,
      // an empty element use to scroll to bottom
      h('div', {
        ref: 'shepherd',
        style: {
          width: isHorizontal ? '0px' : '100%',
          height: isHorizontal ? '100%' : '0px'
        }
      })]);
    }
  });

  return VirtualList;

})));


/***/ }),
/* 30 */
/***/ (function(module, exports) {

module.exports = __WEBPACK_EXTERNAL_MODULE_30__;

/***/ }),
/* 31 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return equipStatusManage; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__equipProcessing_gwSignalr_js__ = __webpack_require__(16);



var equipStatusManage = /*#__PURE__*/function () {
  function equipStatusManage(nodesMap, equipStatusObject, groupNodeObject, statusChange, aliasName) {
    __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck___default()(this, equipStatusManage);
    this.equipStatusObject = equipStatusObject;
    this.groupNodeObject = groupNodeObject;
    this.aleadyUpdateStatus = {};
    this.isGetAllEquipStatus = false;
    this.nodesMap = nodesMap;
    this.statusChange = statusChange;
    this.openSignlr();
    this.aliasName = aliasName;
  }
  __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass___default()(equipStatusManage, [{
    key: "openSignlr",
    value: function openSignlr() {
      var _this = this;
      this.equipStatusSignlr = new __WEBPACK_IMPORTED_MODULE_2__equipProcessing_gwSignalr_js__["a" /* default */]('/equipStatusMonitor', '', '');
      this.equipStatusSignlr.openConnect().then(function (rt) {
        // 通知后台推送全量设备状态
        try {
          rt.invoke('GetAllEquipStatus');
        } catch (error) {
          console.log(error);
        }
        _this.subscribeTo(rt, 'GetAllEquipStatus');
        // 通知后台推送差异设备状态
        try {
          rt.invoke('GetEquipChangeStatus');
        } catch (error) {
          console.log(error);
        }
        rt.onclose(function () {
          _this.openSignlr();
        });
        _this.subscribeTo(rt, 'GetEquipChangeStatus');
      });
    }
  }, {
    key: "subscribeTo",
    value: function subscribeTo(signalr, func) {
      var _this2 = this;
      signalr.off(func);
      signalr.on(func, function (res) {
        if (res && res.isSuccess && res.data) {
          if (_this2[func]) {
            _this2[func](res.data);
          } else {
            _this2.notice({
              func: func,
              data: res.data,
              key: res.groupId
            });
          }
        }
      });
    }
  }, {
    key: "GetAllEquipStatus",
    value: function GetAllEquipStatus(data) {
      var _this3 = this;
      this.isGetAllEquipStatus = true;
      this.resetGroupStatus();
      Object.keys(data).forEach(function (equipNo) {
        _this3.equipStatusObject[equipNo] = data[equipNo];
      });
      Object.keys(this.groupNodeObject).forEach(function (key) {
        _this3.updateByGroup(key);
      });
    }
  }, {
    key: "updateByGroup",
    value: function updateByGroup(key) {
      var _this4 = this;
      if (!this.aleadyUpdateStatus[key]) {
        var list = window.top["group-".concat(key).concat(this.aliasName)] || [];
        if (list.length) {
          this.aleadyUpdateStatus[key] = true;
          list.forEach(function (equip) {
            if (_this4.equipStatusObject[equip.equipNo] == 0 || _this4.equipStatusObject[equip.equipNo] == 2) {
              _this4.setGroupStatus(key, true, 2);
            } else if (_this4.equipStatusObject[equip.equipNo] == 6) {
              _this4.setGroupStatus(key, true, 6);
            }
            if (_this4.nodesMap["".concat(key, "-").concat(equip.equipNo)]) {
              _this4.nodesMap["".concat(key, "-").concat(equip.equipNo)].status = _this4.equipStatusObject[equip.equipNo];
            }
          });
        }
      }
    }
  }, {
    key: "updateGroupStatus",
    value: function updateGroupStatus(key) {
      if (this.isGetAllEquipStatus) {
        this.updateByGroup(key);
      }
    }
  }, {
    key: "GetEquipChangeStatus",
    value: function GetEquipChangeStatus(data) {
      this.setStatus(data);
    }

    // 设置分组状态 key:扁平化数据中节点索引；type:类型（增加，减少）；status:状态（报警2、双机热备6）
  }, {
    key: "setGroupStatus",
    value: function setGroupStatus(key, type, status) {
      if (type) {
        if (status == 2) {
          this.nodesMap[key].alarmCounts = this.nodesMap[key].alarmCounts + 1;
          if (this.nodesMap[key].alarmCounts > 0) {
            this.nodesMap[key].status = 2;
          }
        } else {
          this.nodesMap[key].backUpCounts = this.nodesMap[key].backUpCounts + 1;
          if (this.nodesMap[key].alarmCounts == 0 && this.nodesMap[key].backUpCounts > 0) {
            this.nodesMap[key].status = 6;
          }
        }
      } else {
        if (status == 2) {
          this.nodesMap[key].alarmCounts = this.nodesMap[key].alarmCounts - 1;
          if (this.nodesMap[key].alarmCounts == 0) {
            this.nodesMap[key].status = 1;
          }
        } else {
          this.nodesMap[key].backUpCounts = this.nodesMap[key].backUpCounts - 1;
          if (this.nodesMap[key].alarmCounts == 0) {
            if (this.nodesMap[key].backUpCounts == 0) {
              this.nodesMap[key].status = 1;
            } else {
              this.nodesMap[key].status = 6;
            }
          }
        }
      }
      if (this.nodesMap[key].groupId) {
        this.setGroupStatus(this.nodesMap[key].groupId, type, status);
      }
    }
  }, {
    key: "setStatus",
    value: function setStatus(data) {
      var status = this.equipStatusObject[data.equipNo];
      var groupId = window.top.equipCache && window.top.equipCache[data.equipNo] && window.top.equipCache[data.equipNo].groupId;
      if (data.status != 3 && groupId) {
        switch (data.status) {
          case 0:
            // 由非离线转离线，告警+1
            if (status != 0) {
              this.setGroupStatus(groupId, true, 2);
            }
            if (status == 6) {
              // 由双机热备转离线，告警+1，双机热备-1
              this.setGroupStatus(groupId, false, 6);
            }
            break;
          case 1:
            // 由双机热备状态转正常，双机热备-1
            if (status == 6) {
              this.setGroupStatus(groupId, false, 6);
            } else if (status == 2 || status == 0) {
              // 由告警或离线状态转正常，告警-1
              this.setGroupStatus(groupId, false, 2);
            }
            break;
          case 2:
            // 由非告警状态转告警，告警+1
            if (status != 2) {
              this.setGroupStatus(groupId, true, 2);
            }
            if (status == 6) {
              // 由双机热备转告警，告警+1、双机热备-1
              this.setGroupStatus(groupId, false, 6);
            }
            break;
          case 6:
            if (status == 2 || status == 0) {
              // 由告警或离线转双机热备，告警-1，双机热备+1
              this.setGroupStatus(groupId, false, 2);
            }
            // 由非双机热备状态转双机热备,双机热备+1
            if (status != 6) {
              this.setGroupStatus(groupId, true, 6);
            }
            break;
        }
        // 更新缓存设备状态
        this.equipStatusObject[data.equipNo] = data.status;
        var equipKey = "".concat(groupId, "-").concat(data.equipNo);

        // 更新已展开的设备状态
        this.nodesMap[equipKey] && (this.nodesMap[equipKey].status = data.status);

        // 通知外层当前设备状态已经改变
        this.statusChange(groupId, data.equipNo, data.status);
      }
    }

    // 重置分组状态
  }, {
    key: "resetGroupStatus",
    value: function resetGroupStatus() {
      for (var item in this.groupNodeObject) {
        this.groupNodeObject[item].alarmCounts = 0;
        this.groupNodeObject[item].backUpCount = 0;
      }
    }
  }]);
  return equipStatusManage;
}();


/***/ }),
/* 32 */
/***/ (function(module, exports) {

module.exports = __WEBPACK_EXTERNAL_MODULE_32__;

/***/ }),
/* 33 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return cacheManage; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass__);


var cacheManage = /*#__PURE__*/function () {
  // 分组节点扁平化对象、设备扁平化对象、window缓存设备扁平化对象
  function cacheManage(groupNodeObject, nodesMap, controlObject) {
    __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck___default()(this, cacheManage);
    this.groupNodeObject = groupNodeObject;
    this.nodesMap = nodesMap;
    this.controlObject = controlObject;
  }

  // 增加映射
  __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass___default()(cacheManage, [{
    key: "addNodesMap",
    value: function addNodesMap(list) {
      var _this = this;
      if (list) {
        list.forEach(function (item) {
          _this.nodesMap[item.key] = item;
        });
      }
    }

    // 移除映射
  }, {
    key: "removeNodesMap",
    value: function removeNodesMap(list) {
      var _this2 = this;
      if (list) {
        list.forEach(function (item) {
          delete _this2.nodesMap[item.key];
        });
      }
    }

    // 回收所有分组节点缓存
  }, {
    key: "recycleAllNodeCache",
    value: function recycleAllNodeCache(isDelete) {
      for (var item in this.groupNodeObject) {
        this.groupNodeObject[item].expand = false;
        this.groupNodeObject[item].count = 0;
        this.recycleGroupCache(this.groupNodeObject[item].key);
        isDelete && delete this.groupNodeObject[item];
      }
    }

    // 回收分组节点内存
  }, {
    key: "recycleGroupCache",
    value: function recycleGroupCache(key) {
      if (this.groupNodeObject[key]) {
        this.groupNodeObject[key].children = [];
        this.removeNodesMap(this.groupNodeObject[key].equips);
        this.groupNodeObject[key].equips = [];
        this.groupNodeObject[key].equips.length = 0;
        this.groupNodeObject[key].expand = false;
        for (var i in this.groupNodeObject) {
          if (this.groupNodeObject[i].groupId == key) {
            this.recycleGroupCache(this.groupNodeObject[i].key);
          }
        }
      }
    }

    // 关闭兄弟分组节点及回收内存
  }, {
    key: "closeBrotherNode",
    value: function closeBrotherNode(key) {
      if (!key) return;
      if (this.groupNodeObject[key]) {
        var groupId = this.groupNodeObject[key].groupId;
        for (var item in this.groupNodeObject) {
          if (this.groupNodeObject[item].groupId == groupId && this.groupNodeObject[item].key != key) {
            if (this.groupNodeObject[item].expand) {
              this.groupNodeObject[item].expand = false;
              this.recycleGroupCache(this.groupNodeObject[item].key);
            }
          }
        }
      }
    }
  }]);
  return cacheManage;
}();


/***/ }),
/* 34 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return searchManage; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__utils_utils__ = __webpack_require__(8);



var searchManage = /*#__PURE__*/function () {
  // 分组节点扁平化对象、设备扁平化对象、window缓存设备扁平化对象
  function searchManage(groupNodeObject, showSettings, aliasName) {
    __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck___default()(this, searchManage);
    this.showSettings = showSettings;
    this.groupNodeObject = groupNodeObject;
    this.aliasName = aliasName;
  }

  // 触发搜索
  __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass___default()(searchManage, [{
    key: "filterMethod",
    value: function filterMethod(searchName) {
      for (var item in this.groupNodeObject) {
        if (searchName) {
          this.updateBySearch(item, searchName);
        }
      }
    }

    // 搜索状态将搜索的结果存放缓存中
  }, {
    key: "updateBySearch",
    value: function updateBySearch(item, searchName) {
      var arr = window.top["group-".concat(this.groupNodeObject[item].key).concat(this.aliasName)];
      if (arr) {
        arr = arr.filter(function (item) {
          return item.title.includes(searchName);
        });
        window.top["group-".concat(this.groupNodeObject[item].key, "-search")] = __WEBPACK_IMPORTED_MODULE_2__utils_utils__["a" /* default */].copyOrigin(arr);
      }
    }
  }]);
  return searchManage;
}();


/***/ }),
/* 35 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return equipNumManage; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass__);


var equipNumManage = /*#__PURE__*/function () {
  // 分组节点扁平化对象、设备扁平化对象、window缓存设备扁平化对象
  function equipNumManage(groupNodeObject, aliasName) {
    __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_classCallCheck___default()(this, equipNumManage);
    this.aliasName = aliasName;
    this.groupNodeObject = groupNodeObject;
  }

  // 重新计算分组设备数量
  __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_createClass___default()(equipNumManage, [{
    key: "resetGroupNum",
    value: function resetGroupNum(isSearchStatus) {
      this.clearAllEquipNum();
      for (var key in this.groupNodeObject) {
        var arr = [];
        if (isSearchStatus) {
          arr = window.top["group-".concat(key, "-search")];
        } else {
          arr = window.top["group-".concat(key).concat(this.aliasName)];
        }
        var num = arr ? arr.length : 0;
        if (num) {
          this.groupNodeObject[key].equipCount = num;
          this.setGroupNum(key, num);
        }
      }
    }
  }, {
    key: "clearAllEquipNum",
    value: function clearAllEquipNum() {
      for (var key in this.groupNodeObject) {
        this.groupNodeObject[key].equipCount = 0;
        this.groupNodeObject[key].count = 0;
      }
    }

    // 设置分组设备数量
  }, {
    key: "setGroupNum",
    value: function setGroupNum(key, num) {
      if (this.groupNodeObject[key]) {
        this.groupNodeObject[key].count = Number(this.groupNodeObject[key].count) + Number(num);
        this.setGroupNum(this.groupNodeObject[key].groupId, num);
      }
    }

    // 获取设备总数
  }, {
    key: "getAllEquipsNum",
    value: function getAllEquipsNum() {
      var total = 0;
      for (var item in this.groupNodeObject) {
        if (window.top["group-".concat(this.groupNodeObject[item].key).concat(this.aliasName)]) {
          total = total + window.top["group-".concat(this.groupNodeObject[item].key).concat(this.aliasName)].length;
        }
      }
      return total;
    }
  }]);
  return equipNumManage;
}();


/***/ }),
/* 36 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return requestManage; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_slicedToArray__ = __webpack_require__(37);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_slicedToArray___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_slicedToArray__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_asyncToGenerator__ = __webpack_require__(5);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_asyncToGenerator___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_asyncToGenerator__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_classCallCheck__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_classCallCheck___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_classCallCheck__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__babel_runtime_helpers_createClass__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__babel_runtime_helpers_createClass___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4__babel_runtime_helpers_createClass__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__babel_runtime_regenerator__ = __webpack_require__(7);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__babel_runtime_regenerator___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_5__babel_runtime_regenerator__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__request_api__ = __webpack_require__(41);







var requestManage = /*#__PURE__*/function () {
  function requestManage(nodesMap, equipControllObject) {
    __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_classCallCheck___default()(this, requestManage);
    this.nodesMap = nodesMap;
    this.equipControllObject = equipControllObject;
  }

  // 获取设备控制项
  __WEBPACK_IMPORTED_MODULE_4__babel_runtime_helpers_createClass___default()(requestManage, [{
    key: "getSetting",
    value: function () {
      var _getSetting = __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_asyncToGenerator___default()( /*#__PURE__*/__WEBPACK_IMPORTED_MODULE_5__babel_runtime_regenerator___default.a.mark(function _callee(key, name, level, checked) {
        var _this = this;
        var _String$split, _String$split2, groupId, equipNo, data;
        return __WEBPACK_IMPORTED_MODULE_5__babel_runtime_regenerator___default.a.wrap(function _callee$(_context) {
          while (1) switch (_context.prev = _context.next) {
            case 0:
              _String$split = String(key).split('-'), _String$split2 = __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_slicedToArray___default()(_String$split, 2), groupId = _String$split2[0], equipNo = _String$split2[1];
              data = {
                equipNo: equipNo
              };
              _context.next = 4;
              return __WEBPACK_IMPORTED_MODULE_6__request_api__["a" /* default */].getSetParm(data).then(function (res) {
                if (res.data.code == 200) {
                  var list = res && res.data && res.data.data && res.data.data.rows || [];
                  if (!_this.nodesMap[key].settings) {
                    _this.nodesMap[key].settings = [];
                  }
                  if (list && list.length > 0 && _this.nodesMap[key]) {
                    list.forEach(function (item) {
                      item.title = item.setNm;
                      item.key = "".concat(groupId, "-").concat(equipNo, "-").concat(item.setNo);
                      item.level = Number(level) + 1;
                      item.checked = checked || _this.equipControllObject[equipNo] && _this.equipControllObject[equipNo].includes(item.setNo);
                      item.isSetting = true;
                      item.equipNo = equipNo;
                      item.groupId = groupId;
                      item.setNo = item.setNo;
                      item.equipName = name;
                    });
                    _this.nodesMap[key].settings = __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray___default()(list);
                  }
                }
              });
            case 4:
            case "end":
              return _context.stop();
          }
        }, _callee);
      }));
      function getSetting(_x, _x2, _x3, _x4) {
        return _getSetting.apply(this, arguments);
      }
      return getSetting;
    }()
  }]);
  return requestManage;
}();


/***/ }),
/* 37 */
/***/ (function(module, exports, __webpack_require__) {

var arrayWithHoles = __webpack_require__(38);
var iterableToArrayLimit = __webpack_require__(39);
var unsupportedIterableToArray = __webpack_require__(13);
var nonIterableRest = __webpack_require__(40);
function _slicedToArray(arr, i) {
  return arrayWithHoles(arr) || iterableToArrayLimit(arr, i) || unsupportedIterableToArray(arr, i) || nonIterableRest();
}
module.exports = _slicedToArray, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 38 */
/***/ (function(module, exports) {

function _arrayWithHoles(arr) {
  if (Array.isArray(arr)) return arr;
}
module.exports = _arrayWithHoles, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 39 */
/***/ (function(module, exports) {

function _iterableToArrayLimit(r, l) {
  var t = null == r ? null : "undefined" != typeof Symbol && r[Symbol.iterator] || r["@@iterator"];
  if (null != t) {
    var e,
      n,
      i,
      u,
      a = [],
      f = !0,
      o = !1;
    try {
      if (i = (t = t.call(r)).next, 0 === l) {
        if (Object(t) !== t) return;
        f = !1;
      } else for (; !(f = (e = i.call(t)).done) && (a.push(e.value), a.length !== l); f = !0);
    } catch (r) {
      o = !0, n = r;
    } finally {
      try {
        if (!f && null != t["return"] && (u = t["return"](), Object(u) !== u)) return;
      } finally {
        if (o) throw n;
      }
    }
    return a;
  }
}
module.exports = _iterableToArrayLimit, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 40 */
/***/ (function(module, exports) {

function _nonIterableRest() {
  throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.");
}
module.exports = _nonIterableRest, module.exports.__esModule = true, module.exports["default"] = module.exports;

/***/ }),
/* 41 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
var treeRequest = {
  getSetParm: function getSetParm(data) {
    if (window.axios) {
      return window.axios({
        method: 'post',
        url: '/IoT/api/v3/EquipList/GetFullSetParmByEquipNo',
        params: data,
        data: data
      });
    }
  }
};
var api = Object.assign({}, treeRequest);
/* harmony default export */ __webpack_exports__["a"] = (api);

/***/ }),
/* 42 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return checkStatusManage; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_defineProperty__ = __webpack_require__(4);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_defineProperty___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_defineProperty__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass__);




function ownKeys(e, r) { var t = Object.keys(e); if (Object.getOwnPropertySymbols) { var o = Object.getOwnPropertySymbols(e); r && (o = o.filter(function (r) { return Object.getOwnPropertyDescriptor(e, r).enumerable; })), t.push.apply(t, o); } return t; }
function _objectSpread(e) { for (var r = 1; r < arguments.length; r++) { var t = null != arguments[r] ? arguments[r] : {}; r % 2 ? ownKeys(Object(t), !0).forEach(function (r) { __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_defineProperty___default()(e, r, t[r]); }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(e, Object.getOwnPropertyDescriptors(t)) : ownKeys(Object(t)).forEach(function (r) { Object.defineProperty(e, r, Object.getOwnPropertyDescriptor(t, r)); }); } return e; }
var checkStatusManage = /*#__PURE__*/function () {
  function checkStatusManage(groupNodeObject, nodesMap, equipControllObject, controlObject, equipCheckObject, aliasName) {
    __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck___default()(this, checkStatusManage);
    // 设备控制项，以设备号为键，绑定已选择控制项列表，如：this.equipControllObject.xxx设备=[控制项]
    this.equipControllObject = equipControllObject;
    //分组节点对象，树形结构的扁平化对象
    this.groupNodeObject = groupNodeObject;
    // 已展开的分组下所挂载的设备对象
    this.nodesMap = nodesMap;

    // 所展开的设备控制项
    this.controlObject = controlObject;
    //设备选中状态记录 equipCheckObject：{xxx设备号：{indeterminate: false,checked: false,groupId:xxx}}
    this.equipCheckObject = equipCheckObject;
    // 搜索状态下
    this.isSearchStatus = false;

    // 搜索状态下分组数据保存原有数据,选择节点时需要更新,非搜索状态下,清空
    this.searchStatusGroupObject = {};
    this.aliasName = aliasName;
  }
  __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass___default()(checkStatusManage, [{
    key: "resetCheckedStatus",
    value: function resetCheckedStatus() {
      var _this = this;
      // 重置所有实例化节点选中效果
      Object.values(this.nodesMap).forEach(function (item) {
        _this.setCheckStatus(item.key, false, false);
        if (item.isGroup && !item.isEquip) {
          // 清除选中设备
          _this.clearCheckedEquips(item.key);

          // 清除之前选中的数量
          _this.updateEquipSelectCount(item.key, true);
        }
        item.halfCheckedEquips && (item.halfCheckedEquips = []);
      });
      Object.keys(this.equipCheckObject).forEach(function (item) {
        _this.setEquipCheckObject(item, false, false, _this.equipCheckObject[item].groupId);
      });
    }

    // 搜索状态下,更新分组选中状态
    /**
     *  1:备份原有选中的设备数据量,父级分组Id
     *  2:清空之前分组的设备选中数量
     *  3:依照搜索结果,重新计算分组选中的设备数据量
     *  */
  }, {
    key: "reComputedCheckNum",
    value: function reComputedCheckNum(isSearchStatus) {
      var _this2 = this;
      this.isSearchStatus = isSearchStatus;
      if (this.isSearchStatus) {
        // 1备份原有数据,父级分组Id
        Object.values(this.groupNodeObject).forEach(function (item) {
          _this2.searchStatusGroupObject[item.key] = _objectSpread({
            groupId: item.groupId,
            equipSelectCount: item.equipSelectCount,
            halfCheckedEquips: item.halfCheckedEquips
          }, _this2.searchStatusGroupObject[item.key]);
        });
        // 2:清空之前分组的设备选中数量
        Object.values(this.groupNodeObject).forEach(function (item) {
          _this2.updateEquipSelectCount(item.key, true);
          item.halfCheckedEquips = [];
        });

        // 3:依照搜索结果,重新计算分组选中的设备数据量
        Object.values(this.groupNodeObject).forEach(function (item) {
          var list = window.top["group-".concat(item.key, "-search")] || [];
          list.forEach(function (equip) {
            if (_this2.equipCheckObject[equip.equipNo] && _this2.equipCheckObject[equip.equipNo].checked) {
              _this2.updateEquipSelectCount(item.key, false, 1);
            } else if (_this2.equipCheckObject[equip.equipNo] && _this2.equipCheckObject[equip.equipNo].indeterminate) {
              item.halfCheckedEquips.push(equip.equipNo);
            }
          });
        });
      } else {
        // 非搜索状态下,还原原有数据
        Object.values(this.groupNodeObject).forEach(function (item) {
          var data = _this2.searchStatusGroupObject[item.key];
          if (data) {
            item.equipSelectCount = data.equipSelectCount;
            item.halfCheckedEquips = [].concat(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray___default()(data.halfCheckedEquips), __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray___default()(item.halfCheckedEquips));
            item.halfCheckedEquips = __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray___default()(new Set(item.halfCheckedEquips));
          }
        });
        this.searchStatusGroupObject = {};
      }
    }

    // 根据传过来的设备选中数据、回显选中状态
    /**
     *  1:记录进全局设备选中状态
     *  2:设备设备选中状态
     *  3:更新分组选中的设备数量
     *  4:更新展开的设备控制项选中状态
     *  5:更新分组选中状态
     *  */
  }, {
    key: "updateCheckedStatusWithEquips",
    value: function updateCheckedStatusWithEquips(list) {
      var _this3 = this;
      window.setTimeout(function () {
        for (var index = 0, length = list.length; index < length; index++) {
          // 从缓存中看有没有该设备，防止是脏数据
          var data = window.top.equipCache && window.top.equipCache[list[index]];
          if (data && data.groupId) {
            // 1:记录进全局设备选中状态
            _this3.setEquipCheckObject(list[index], true, false, data.groupId);
            // 2:设备设备选中状态
            _this3.setCheckStatus("".concat(data.groupId, "-").concat(list[index]), true, false);
            // 3:更新分组选中的设备数量
            _this3.updateEquipSelectCount(data.groupId, false, 1);
            // 4:更新展开的设备控制项选中状态
            _this3.updateExpandControlCheckStatus(data.groupId, list[index]);
          }
        }
        // 5:更新分组选中状态
        _this3.updateGroupCheckStatus();
      }, 200);
    }

    // 更新展开的设备控制项选中状态
  }, {
    key: "updateExpandControlCheckStatus",
    value: function updateExpandControlCheckStatus(groupId, equipNo) {
      var _this4 = this;
      var equipNode = this.nodesMap["".concat(groupId, "-").concat(equipNo)];
      if (equipNode && equipNode.settings && equipNode.settings.length) {
        equipNode.settings.forEach(function (set) {
          if (_this4.nodesMap["".concat(groupId, "-").concat(equipNo, "-").concat(set.setNo)]) {
            _this4.setCheckStatus("".concat(groupId, "-").concat(equipNo, "-").concat(set.setNo), true, false);
          }
        });
      }
    }

    // 根据传过来的设备控制项，回显设备分组选中状态
    /**
     *  1:记录全局设备控制项选中
     *  2:设置设备和设备控制项选中状态
     *  3:记录分组的半选设备
     *  4:记录全局设备半选状态
     *  5:更新分组选中状态
     *  */
  }, {
    key: "updateCheckedStatusWithControls",
    value: function updateCheckedStatusWithControls(list) {
      var _this5 = this;
      window.setTimeout(function () {
        // 半选设备
        for (var index = 0, length = list.length; index < length; index++) {
          var arr = list[index].split('.');
          var equipNo = arr[0];
          var setNo = arr[1];
          if (!_this5.equipControllObject[equipNo]) {
            _this5.equipControllObject[equipNo] = [];
          }
          // 1:记录全局设备控制项选中
          _this5.equipControllObject[equipNo].push(Number(setNo));
          if (window.top.equipCache && window.top.equipCache[equipNo]) {
            var groupId = window.top.equipCache[equipNo].groupId;
            var setKey = "".concat(groupId, "-").concat(equipNo, "-").concat(setNo);
            // 2:设置设备和设备控制项选中状态
            if (_this5.nodesMap[setKey]) {
              _this5.setCheckStatus(setKey, true, false);
            }
            if (_this5.nodesMap["".concat(groupId, "-").concat(equipNo)]) {
              _this5.setCheckStatus("".concat(groupId, "-").concat(equipNo), false, true);
            }
            // 3:记录分组的半选设备
            _this5.nodesMap["".concat(groupId)].halfCheckedEquips && _this5.nodesMap["".concat(groupId)].halfCheckedEquips.push(equipNo);
            // 4:记录全局设备半选状态
            _this5.setEquipCheckObject(equipNo, false, true, groupId);
          }
        }

        //  5:更新分组选中状态
        _this5.updateGroupCheckStatus();
      }, 100);
    }

    // 更新分组选中状态
  }, {
    key: "updateGroupCheckStatus",
    value: function updateGroupCheckStatus() {
      var _this6 = this;
      Object.values(this.groupNodeObject).forEach(function (item) {
        if (item.count > 0 && item.count == item.equipSelectCount) {
          _this6.setCheckStatus(item.key, true, false);
        } else if (item.count && item.equipSelectCount && item.count > item.equipSelectCount || item.halfCheckedEquips.length) {
          _this6.setGroupHalfChecked(item.key);
        } else {
          _this6.setCheckStatus(item.key, false, false);
        }
      });
    }

    // 设置父级分组半选状态
  }, {
    key: "setGroupHalfChecked",
    value: function setGroupHalfChecked(key) {
      if (this.nodesMap[key]) {
        this.setCheckStatus(key, false, true);
      }
      if (this.nodesMap[key].groupId) {
        this.setGroupHalfChecked(this.nodesMap[key].groupId);
      }
    }

    // 设置设备控制项状态
  }, {
    key: "setControlStatus",
    value: function setControlStatus(setNos, groupId, equipNo, checked) {
      var _this7 = this;
      if (setNos) {
        setNos.forEach(function (item) {
          if (_this7.nodesMap["".concat(groupId, "-").concat(equipNo, "-").concat(item.setNo)]) {
            _this7.setCheckStatus("".concat(groupId, "-").concat(equipNo, "-").concat(item.setNo), checked, false);
          }
        });
      }
    }

    // 更新设备控制项选中状态
  }, {
    key: "updateControlCheckStatus",
    value: function updateControlCheckStatus() {
      var _this8 = this;
      Object.keys(this.controlObject).forEach(function (item) {
        var node = _this8.nodesMap["".concat(_this8.controlObject[item].groupId, "-").concat(item)];
        if (node) {
          if (node.checked) {
            var setNos = node.settings;
            _this8.setControlStatus(setNos, _this8.controlObject[item].groupId, item, true);
          } else if (!node.checked && !node.indeterminate) {
            var _setNos = node.settings;
            _this8.setControlStatus(_setNos, _this8.controlObject[item].groupId, item, false);
          } else {
            if (_this8.equipControllObject[item]) {
              _this8.equipControllObject[item].forEach(function (setNo) {
                var key = "".concat(_this8.controlObject[item].groupId, "-").concat(item, "-").concat(setNo);
                if (_this8.nodesMap[key]) {
                  _this8.setCheckStatus(key, true, false);
                }
              });
            }
          }
        }
      });
    }

    // 获取选中的分组
  }, {
    key: "getGroupChecked",
    value: function getGroupChecked() {
      var _this9 = this;
      var group = [];
      Object.keys(this.groupNodeObject).forEach(function (item) {
        if (_this9.groupNodeObject[item].checked) {
          group.push(item);
        }
      });
      return group;
    }

    // 获取选中设备
  }, {
    key: "getEquipSelectd",
    value: function getEquipSelectd() {
      var _this10 = this;
      var equips = [];
      Object.keys(this.equipCheckObject).forEach(function (equipNo) {
        if (_this10.equipCheckObject[equipNo].checked) {
          equips.push(Number(equipNo));
        }
      });
      return equips;
    }

    // 获取选中的设备控制箱
  }, {
    key: "getControlSelected",
    value: function getControlSelected() {
      var _this11 = this;
      var controls = [];
      Object.keys(this.equipControllObject).forEach(function (item) {
        controls.push.apply(controls, __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_toConsumableArray___default()(_this11.equipControllObject[item].map(function (child) {
          return "".concat(item, ".").concat(child);
        })));
      });
      return controls;
    }

    // 点击选中
    /**
     *  1:如果是分组选择，触发分组选中事件
     *  2:如果是设备选择，触发设备选中事件
     *  3:如果是设备控制项选择，触发设备控制项选中事件
     *  */
  }, {
    key: "onChecked",
    value: function onChecked(node, isSearchStatus) {
      this.isSearchStatus = isSearchStatus || false;
      if (node.isGroup && !node.isEquip) {
        this.selectGroup(node);
      } else if (node.isEquip) {
        this.selectEquip(node);
      } else if (node.isSetting) {
        this.selectControl(node);
      }
    }

    // 从全局映射中，设置节点选中状态
  }, {
    key: "setCheckStatus",
    value: function setCheckStatus(key, checked, indeterminate) {
      if (this.nodesMap[key]) {
        this.nodesMap[key].checked = checked;
        this.nodesMap[key].isGroup && (this.nodesMap[key].indeterminate = indeterminate);
      }
    }

    // 清除选中的设备
  }, {
    key: "clearCheckedEquips",
    value: function clearCheckedEquips(key) {
      var _this12 = this;
      if (this.groupNodeObject[key]) {
        this.groupNodeObject[key].equipSelectCount = 0;
        this.groupNodeObject[key].halfCheckedEquips && this.groupNodeObject[key].halfCheckedEquips.forEach(function (item) {
          if (_this12.equipControllObject[item]) {
            _this12.equipControllObject[item].forEach(function (child) {
              // 清除选中状态
              _this12.setCheckStatus("".concat(key, "-").concat(item, "-").concat(child), false, false);
            });
          }
        });
        this.groupNodeObject[key].halfCheckedEquips = [];
        if (this.groupNodeObject[key].groupId) {
          this.clearCheckedEquips(this.groupNodeObject[key].groupId);
        }
      }
    }

    // 搜索状态下，更新分组选中的设备数量
  }, {
    key: "updateSearchStatusObject",
    value: function updateSearchStatusObject(key, count) {
      if (this.searchStatusGroupObject[key]) {
        this.searchStatusGroupObject[key].equipSelectCount = this.searchStatusGroupObject[key].equipSelectCount + count;
        if (this.searchStatusGroupObject[key].groupId) {
          this.updateSearchStatusObject(this.searchStatusGroupObject[key].groupId, count);
        }
      }
    }

    // 更新分组设备选中数量
  }, {
    key: "updateEquipSelectCount",
    value: function updateEquipSelectCount(key, clear, count) {
      if (this.nodesMap[key]) {
        if (clear) {
          this.nodesMap[key].equipSelectCount = 0;
        } else {
          this.nodesMap[key].equipSelectCount = this.nodesMap[key].equipSelectCount + count;
        }
        if (this.nodesMap[key].groupId) {
          this.updateEquipSelectCount(this.nodesMap[key].groupId, clear, count);
        }
      }
    }

    // 保存全局设备选中状态
  }, {
    key: "setEquipCheckObject",
    value: function setEquipCheckObject(key, checked, indeterminate, groupId) {
      if (!this.equipCheckObject[key]) {
        this.equipCheckObject[key] = {};
      }
      this.equipCheckObject[key].checked = checked;
      this.equipCheckObject[key].indeterminate = indeterminate;
      this.equipCheckObject[key].groupId = groupId;
    }

    // 更新当前展开的分组所选中的设备、设置分组选中的设备数量、更新选中状态（已经实例化的节点）
  }, {
    key: "updateEquipSelect",
    value: function updateEquipSelect(key, list, checked, count) {
      var _this13 = this;
      if (this.nodesMap[key]) {
        if (list.length) {
          list.forEach(function (item) {
            if (checked) {
              _this13.setEquipCheckObject(item.equipNo, true, false, key);
              _this13.nodesMap["".concat(key, "-").concat(item.equipNo)] && _this13.setCheckStatus("".concat(key, "-").concat(item.equipNo), true, false);
            } else {
              _this13.setEquipCheckObject(item.equipNo, false, false, key);
              _this13.nodesMap["".concat(key, "-").concat(item.equipNo)] && _this13.setCheckStatus("".concat(key, "-").concat(item.equipNo), false, false);
            }
            _this13.equipControllObject[item.equipNo] && delete _this13.equipControllObject[item.equipNo];
            if (_this13.nodesMap[key].halfCheckedEquips.includes(item.equipNo)) {
              _this13.nodesMap[key].halfCheckedEquips = _this13.nodesMap[key].halfCheckedEquips.filter(function (equipNo) {
                return equipNo != item.equipNo;
              });
              if (_this13.searchStatusGroupObject[key]) {
                _this13.searchStatusGroupObject[key].halfCheckedEquips = _this13.nodesMap[key].halfCheckedEquips;
              }
            }
          });
        }
      }
    }

    // 根据分组更新选中的设备，全选分组或者取消选择分组时调用
    /**
     *  1:统计需要更新的设备数量
     *  2:把所有子孙分组设备全部统计，并且记录所有当前分组选中的设备，更新除分组之外的选中状态（已经实例化的节点）
     *  3:更新自身设备选择数量、向上级分组累加已经选中的设备数量
     *  4:更新搜索状态下分组副本数据
     *  */
    // list(Array):所要更新的分组，type(String):更新类型，add为新增，delete为删除;
  }, {
    key: "updateGroupSelect",
    value: function updateGroupSelect(list, checked) {
      var _this14 = this;
      list.forEach(function (item) {
        if (_this14.groupNodeObject[item]) {
          if (_this14.groupNodeObject[item].groups && _this14.groupNodeObject[item].groups.length > 0) {
            _this14.updateGroupSelect(_this14.groupNodeObject[item].groups.map(function (i) {
              return i.key;
            }), checked);
          }
          var _list = window.top["group-".concat(item).concat(_this14.aliasName)] || [];
          if (_this14.isSearchStatus) {
            _list = window.top["group-".concat(item, "-search")] || [];
          }

          // 1:统计需要更新的设备数
          var addCount = 0;
          var deleteCount = 0;
          _list.forEach(function (equip) {
            var node = _this14.equipCheckObject[equip.equipNo];
            if (!node || !node.checked) {
              addCount++;
            } else {
              deleteCount--;
            }
          });
          var count = checked ? addCount : deleteCount;

          // 1：把所有子孙分组设备全部统计，并且记录所有当前分组选中的设备、更新除分组之外的选中状态（已经实例化的节点）
          _this14.updateEquipSelect(item, _list, checked, count);

          // 3:更新自身设备选择数量、向上级分组累加已经选中的设备数量
          _this14.updateEquipSelectCount(item, false, count);

          // 4:更新搜索状态下分组副本数据
          _this14.updateSearchStatusObject(item, count);
        }
      });
    }

    // 清除分组挂载的半选设备
  }, {
    key: "clearHalfCheckedEquips",
    value: function clearHalfCheckedEquips(list) {
      var _this15 = this;
      list.forEach(function (groupId) {
        _this15.groupNodeObject[groupId].halfCheckedEquips && _this15.groupNodeObject[groupId].halfCheckedEquips.forEach(function (equip) {
          if (_this15.equipControllObject[equip]) {
            _this15.equipControllObject[equip].forEach(function (setNo) {
              // 清除选中状态
              _this15.setCheckStatus("".concat(groupId, "-").concat(equip, "-").concat(setNo), false, false);
            });
          }
        });
        _this15.groupNodeObject[groupId].halfCheckedEquips = [];
        if (_this15.groupNodeObject[groupId].groups && _this15.groupNodeObject[groupId].groups.length > 0) {
          _this15.clearHalfCheckedEquips(_this15.groupNodeObject[groupId].groups.map(function (i) {
            return i.key;
          }));
        }
      });
    }

    // 触发分组选择
    /**
     * 1:清除之前半选设备
    *  2:更新分组中设备选择
    *  3:更新所有分组选中状态
    *  4:更新控制项选中状态
    *  */
  }, {
    key: "selectGroup",
    value: function selectGroup(node) {
      // 1:清除之前半选设备
      this.clearHalfCheckedEquips([node.key]);
      // 2:更新分组中设备选择
      this.updateGroupSelect([node.key], node.checked);
      // 3:更新控制项选中状态
      this.updateControlCheckStatus();
      // 4:更新所有分组选中状态
      this.updateGroupCheckStatus();
    }

    // 触发设备选择
    /**
    *  1:更新分组选择的设备
    *  2:更新分组所选择的设备数量
    *  3:更新控制项选中状态
    *  4:更新分组状态
    *  */
  }, {
    key: "selectEquip",
    value: function selectEquip(node) {
      if (this.nodesMap[node.groupId]) {
        var list = [{
          equipNo: node.equipNo
        }];
        var count = (node.checked ? 1 : -1) * list.length;
        // 1:更新分组选择的设备
        this.updateEquipSelect(node.groupId, list, node.checked, count);
        // 2:更新分组所选择的设备数量
        this.updateEquipSelectCount(node.groupId, false, count);
        // 3:更新搜索状态下分组副本数据
        this.updateSearchStatusObject(node.groupId, count);
        // 4:更新控制项选中状态
        this.updateControlCheckStatus();
        // 5:更新分组状态
        this.updateGroupCheckStatus();
      }
    }

    // 更新设备控制项存储
  }, {
    key: "updateEquipControl",
    value: function updateEquipControl(equipNo, setNo, checked) {
      if (!this.equipControllObject[equipNo]) {
        this.equipControllObject[equipNo] = [];
      }
      if (checked) {
        this.equipControllObject[equipNo].push(setNo);
      } else {
        this.equipControllObject[equipNo] = this.equipControllObject[equipNo].filter(function (item) {
          return item != setNo;
        });
      }
    }

    // 触发控制项选择
    /**
    *  1:更新设备控制项
    *  2:更新设备选中状态
       (1)选择情况下：
            1）如果设备即将全选，清除分组中半选设备、清除设备控制项记录（equipControllObject），分组全选设备数组+1，半选设备数组中有则-1,设置设备全选
            2) 如果之前设备没选择情况下(非半选情况下)，分组中半选设备数组+1，设置设备半选
            3）如果设备是半选状态。不做处理
      （2）取消选择情况下
            1）如果设备是全选情况下、分组全选设备数组-1
                1）如果不仅有一个测点，设备半选分组+1，设备控制项（equipControllObject）记录剩余设备，设置设备半选
                2）如果仅有一个测点，设置设备不选
            2) 如果设备是半选情况下，如果还是半选，不做处理，如果从半选到不选情况下，分组挂载的半选分组-1，设备控制项有则-1，设置设备不选
    *  3:更新分组状态
    *  */
  }, {
    key: "selectControl",
    value: function selectControl(node) {
      var _this16 = this;
      var equipNode = this.nodesMap["".concat(node.groupId, "-").concat(node.equipNo)];
      if (equipNode) {
        this.updateEquipControl(node.equipNo, node.setNo, node.checked);
        if (node.checked) {
          // 如果设备所绑定的控制项与设备控制项记录（equipControllObject）长度相同情况下，则设备是全选
          // 要么是是半选,要么没选
          // 要么之前是半选(两个),要么只有一个
          // 如果之前是半选,现在还是半选,不处理
          if (equipNode.settings && equipNode.settings.length == this.equipControllObject[node.equipNo].length) {
            delete this.equipControllObject[node.equipNo];
            this.updateEquipSelectCount(node.groupId, false, 1);
            this.updateSearchStatusObject(node.groupId, 1);
            this.nodesMap[node.groupId].halfCheckedEquips = this.nodesMap[node.groupId].halfCheckedEquips.filter(function (equipNo) {
              return equipNo != node.equipNo;
            });
            this.setEquipCheckObject(node.equipNo, true, false, node.groupId);
            this.setCheckStatus("".concat(node.groupId, "-").concat(node.equipNo), true, false);
          } else if (equipNode && !equipNode.indeterminate) {
            this.nodesMap["".concat(node.groupId)].halfCheckedEquips.push(node.equipNo);
            this.setCheckStatus("".concat(node.groupId, "-").concat(node.equipNo), false, true);
            this.setEquipCheckObject(node.equipNo, false, true, node.groupId);
          }
        } else {
          if (equipNode.checked) {
            this.updateEquipSelectCount(node.groupId, false, -1);
            this.updateSearchStatusObject(node.groupId, -1);
            if (equipNode.settings.length > 1) {
              this.nodesMap[node.groupId].halfCheckedEquips.push(node.equipNo);
              var otherSetNos = equipNode.settings.map(function (item) {
                return item.setNo;
              }).filter(function (setNo) {
                return setNo != node.setNo;
              });
              otherSetNos.forEach(function (setNo) {
                _this16.updateEquipControl(node.equipNo, setNo, true);
              });
              this.setEquipCheckObject(node.equipNo, false, true, node.groupId);
              this.setCheckStatus("".concat(node.groupId, "-").concat(node.equipNo), false, true);
            } else {
              this.setEquipCheckObject(node.equipNo, false, false, node.groupId);
              this.setCheckStatus("".concat(node.groupId, "-").concat(node.equipNo), false, false);
            }
          } else if (equipNode.indeterminate) {
            if (this.equipControllObject[node.equipNo] && !this.equipControllObject[node.equipNo].length) {
              this.nodesMap[node.groupId].halfCheckedEquips = this.nodesMap[node.groupId].halfCheckedEquips.filter(function (equipNo) {
                return equipNo != node.equipNo;
              });
              this.setEquipCheckObject(node.equipNo, false, false, node.groupId);
              this.setCheckStatus("".concat(node.groupId, "-").concat(node.equipNo), false, false);
            }
          }
        }
        this.updateGroupCheckStatus();
      }
    }
  }]);
  return checkStatusManage;
}();


/***/ }),
/* 43 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
var render = function() {
  var _vm = this
  var _h = _vm.$createElement
  var _c = _vm._self._c || _h
  return _c("div", { staticClass: "gw-tree", style: { height: "100%" } }, [
    _c(
      "div",
      { staticClass: "tree", style: { height: "100%" } },
      [
        _c("virtual-list", {
          ref: "virtualList",
          staticClass: "virtualList",
          attrs: {
            "data-key": "key",
            "data-sources": _vm.visibleList,
            "data-component": _vm.itemComponent,
            keeps: 40,
            "extra-props": {
              currentSelect: _vm.currentSelect,
              nodeClick: _vm.nodeClick,
              showStatus: _vm.showStatus,
              showSettings: _vm.showSettings,
              showCheckbox: _vm.showCheckbox,
              onChecked: _vm.onChecked,
              showOperate: _vm.showOperate,
              groupEditAndNew: _vm.groupEditAndNew,
              deleteGroup: _vm.deleteGroup,
              colorConfig: _vm.colorConfig
            }
          }
        })
      ],
      1
    )
  ])
}
var staticRenderFns = []
render._withStripped = true
var esExports = { render: render, staticRenderFns: staticRenderFns }
/* harmony default export */ __webpack_exports__["a"] = (esExports);
if (false) {
  module.hot.accept()
  if (module.hot.data) {
    require("vue-hot-reload-api")      .rerender("data-v-47259c44", esExports)
  }
}

/***/ }),
/* 44 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return equipCache; });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_defineProperty__ = __webpack_require__(4);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_defineProperty___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_defineProperty__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__gwSignalr_js__ = __webpack_require__(16);




function _createForOfIteratorHelper(o, allowArrayLike) { var it = typeof Symbol !== "undefined" && o[Symbol.iterator] || o["@@iterator"]; if (!it) { if (Array.isArray(o) || (it = _unsupportedIterableToArray(o)) || allowArrayLike && o && typeof o.length === "number") { if (it) o = it; var i = 0; var F = function F() {}; return { s: F, n: function n() { if (i >= o.length) return { done: true }; return { done: false, value: o[i++] }; }, e: function e(_e) { throw _e; }, f: F }; } throw new TypeError("Invalid attempt to iterate non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); } var normalCompletion = true, didErr = false, err; return { s: function s() { it = it.call(o); }, n: function n() { var step = it.next(); normalCompletion = step.done; return step; }, e: function e(_e2) { didErr = true; err = _e2; }, f: function f() { try { if (!normalCompletion && it.return != null) it.return(); } finally { if (didErr) throw err; } } }; }
function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }
function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) arr2[i] = arr[i]; return arr2; }
function ownKeys(e, r) { var t = Object.keys(e); if (Object.getOwnPropertySymbols) { var o = Object.getOwnPropertySymbols(e); r && (o = o.filter(function (r) { return Object.getOwnPropertyDescriptor(e, r).enumerable; })), t.push.apply(t, o); } return t; }
function _objectSpread(e) { for (var r = 1; r < arguments.length; r++) { var t = null != arguments[r] ? arguments[r] : {}; r % 2 ? ownKeys(Object(t), !0).forEach(function (r) { __WEBPACK_IMPORTED_MODULE_0__babel_runtime_helpers_defineProperty___default()(e, r, t[r]); }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(e, Object.getOwnPropertyDescriptors(t)) : ownKeys(Object(t)).forEach(function (r) { Object.defineProperty(e, r, Object.getOwnPropertyDescriptor(t, r)); }); } return e; }

var equipCache = /*#__PURE__*/function () {
  function equipCache() {
    __WEBPACK_IMPORTED_MODULE_2__babel_runtime_helpers_classCallCheck___default()(this, equipCache);
    this.eGroupNotify = new __WEBPACK_IMPORTED_MODULE_4__gwSignalr_js__["a" /* default */]('/eGroupNotify', '', '');
    this.alreadyUpdate = {};
    this.notify = null;
  }
  __WEBPACK_IMPORTED_MODULE_3__babel_runtime_helpers_createClass___default()(equipCache, [{
    key: "stop",
    value: function stop() {
      if (this.notify) {
        this.notify.stop();
      }
    }
  }, {
    key: "Init",
    value: function Init() {
      var _this = this;
      //  打开signalr链接
      this.eGroupNotify.openConnect().then(function (rt) {
        _this.notify = rt;

        // 分组推送--start
        // 通知后台需要推送分组结构
        try {
          rt.invoke('GetEquipGroupTree');
          // 通知后台推送全量分组
          rt.invoke('GetAllEquipGroupTree');
        } catch (error) {
          console.log(error);
        }

        // 获取分组结构
        _this.subscribeTo(rt, 'GetEquipGroupTree');
        // 新增分组
        _this.subscribeTo(rt, 'AddEquipGroup');
        // 编辑分组
        _this.subscribeTo(rt, 'EditEquipGroup');
        // 删除分组
        _this.subscribeTo(rt, 'DeleteEquipGroup');
        // 分组推送--end

        // 设备推送--start
        // 通知后台需要推送分组结构

        try {
          rt.invoke('GetGroupEquips');
        } catch (error) {
          console.log(error);
        }
        // 订阅设备推送
        _this.subscribeTo(rt, 'GetGroupEquips');
        // 订阅分组结构推送
        _this.subscribeTo(rt, 'AddEquip');
        // 订阅分组结构推送
        _this.subscribeTo(rt, 'DeleteEquip');
        // 订阅分组结构推送
        _this.subscribeTo(rt, 'EditEquip');
        _this.subscribeTo(rt, 'moveEquips');

        // 获取全量分组
        _this.subscribeTo(rt, 'GetAllEquipGroupTree');
        // 设备推送--end

        rt.onclose(function (err) {
          try {
            _this.Init();
          } catch (error) {
            console.log(error);
          }
          console.log('重连', err);
        });
      }).catch(function (e) {
        console.error(e);
      });
    }
  }, {
    key: "subscribeTo",
    value: function subscribeTo(signalr, func) {
      var _this2 = this;
      signalr.off(func);
      signalr.on(func, function (res) {
        if (res && res.isSuccess) {
          if (_this2[func]) {
            _this2[func](res.data);
          } else {
            _this2.notice({
              func: func,
              data: res.data,
              key: res.groupId
            });
          }
        }
      });
    }

    // 获取分组---无权限管理的分组列表--空设备分组不展示
  }, {
    key: "GetEquipGroupTree",
    value: function GetEquipGroupTree(data) {
      if (!window.top.groupList) {
        window.top.groupList = data;
        this.notice({
          type: 'GetEquipGroupTree'
        });
      }
    }

    // 获取全量分组---设备管理使用
  }, {
    key: "GetAllEquipGroupTree",
    value: function GetAllEquipGroupTree(data) {
      console.log(8899, data);
      if (!window.top.groupList_manageMent) {
        window.top.groupList_manageMent = data;
        this.notice({
          type: 'GetEquipGroupTreeWidthTreeType'
        });
      }
    }

    // 新增分组
  }, {
    key: "AddEquipGroup",
    value: function AddEquipGroup(data) {
      var _ref = data || {},
        parentGroupId = _ref.parentGroupId,
        groupId = _ref.groupId,
        groupName = _ref.groupName;
      if (groupId) {
        if (!window.top.groupList_manageMent) {
          window.top.groupList_manageMent = [];
        }
        window.top.groupList_manageMent.push({
          parentId: parentGroupId,
          id: groupId,
          name: groupName,
          equipCount: 0
        });
        this.notice({
          type: 'AddEquipGroup',
          data: data
        });
      }
    }

    // 编辑分组
  }, {
    key: "EditEquipGroup",
    value: function EditEquipGroup(data) {
      // 更新window缓存
      var groupId = data.groupId,
        groupName = data.groupName;
      if (window.top.groupList) {
        window.groupList.forEach(function (item) {
          if (item.id == groupId) {
            item.name = groupName;
          }
        });
      }
      if (window.top.groupList_manageMent) {
        window.groupList_manageMent.forEach(function (item) {
          if (item.id == groupId) {
            item.name = groupName;
          }
        });
      }
      this.notice({
        type: 'EditEquipGroup',
        data: data
      });
    }
  }, {
    key: "deleteChildGroup",
    value: function deleteChildGroup(parentId, list) {
      var _this3 = this;
      var deleteGroups = [];
      list.forEach(function (item) {
        if (item.parentId == parentId) {
          deleteGroups.push(item.id);
        }
      });
      deleteGroups.forEach(function (groupId) {
        var index = list.findIndex(function (item) {
          return item.id == groupId;
        });
        index > -1 && list.splice(index, 1);
        _this3.deleteChildGroup(groupId, list);
      });
    }

    // 删除分组
  }, {
    key: "DeleteEquipGroup",
    value: function DeleteEquipGroup(data) {
      var _this4 = this;
      data.forEach(function (group) {
        if (window.top.groupList) {
          var index = window.top.groupList.findIndex(function (item) {
            return item.id == group.groupId;
          });
          index > -1 && window.top.groupList.splice(index, 1);
          _this4.deleteChildGroup(group.groupId, window.top.groupList);
        }
        if (window.top.groupList_manageMent) {
          var _index = window.top.groupList_manageMent.findIndex(function (item) {
            return item.id == group.groupId;
          });
          _index > -1 && window.top.groupList_manageMent.splice(_index, 1);
          _this4.deleteChildGroup(group.groupId, window.top.groupList_manageMent);
        }
      });
      this.notice({
        type: 'DeleteEquipGroup',
        data: data
      });
    }

    // 获取设备
  }, {
    key: "GetGroupEquips",
    value: function GetGroupEquips(data) {
      var _ref2 = data || {},
        groupId = _ref2.groupId,
        equips = _ref2.equips;
      console.log(this.alreadyUpdate[groupId]);
      if (!this.alreadyUpdate[groupId]) {
        if (!window.top.groupCache) {
          window.top.groupCache = {};
        }
        window.top.groupCache[groupId] = {};
        if (groupId && equips && equips instanceof Array) {
          equips.forEach(function (item) {
            item.title = item.name;
            item.groupId = groupId;
            item.equipNo = item.id;
            delete item.name;
            if (!window.equipCache) {
              window.top.equipCache = {};
            }
            // 找分组,设备编辑需要用到
            window.top.equipCache[item.id] = item;

            // 设备状态需要用到
            window.top.groupCache[groupId][item.id] = item;
          });
          window.top["group-".concat(groupId)] = equips;
          this.notice({
            type: 'GetGroupEquips',
            data: {
              groupId: groupId
            }
          });
          this.alreadyUpdate[groupId] = true;
        }
      }
    }

    // 新增设备
  }, {
    key: "AddEquip",
    value: function AddEquip(data) {
      var _ref3 = data || {},
        groupId = _ref3.groupId,
        equips = _ref3.equips;
      if (!window.top["group-".concat(groupId)]) {
        window.top["group-".concat(groupId)] = [];
      }
      if (!window.top.groupCache) {
        window.top.groupCache = {};
      }
      if (!window.top.groupCache[groupId]) {
        window.top.groupCache[groupId] = {};
      }
      if (!window.top.equipCache) {
        window.top.equipCache = {};
      }
      var length = window.top["group-".concat(groupId)].length;
      if (groupId && equips) {
        equips.forEach(function (equip, index) {
          window.top["group-".concat(groupId)].push({
            equipNo: equip.id,
            groupId: groupId,
            id: equip.id,
            title: equip.name
          });

          // 找分组,设备编辑需要用到
          window.top.equipCache[equip.id] = window.top["group-".concat(groupId)][length + index];

          // 设备状态需要用到
          window.top.groupCache[groupId][equip.id] = window.top["group-".concat(groupId)][length + index];
        });
        if (!this.exist(groupId, window.top.groupList)) {
          var _window$top$groupList;
          var list = this.findParentList(groupId, window.top.groupList_manageMent);
          (_window$top$groupList = window.top.groupList).push.apply(_window$top$groupList, __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray___default()(list));
        }
        this.notice({
          type: 'AddEquip',
          data: data
        });
      }
    }
  }, {
    key: "moveEquips",
    value: function moveEquips(data) {
      var _this5 = this;
      // 统计需要更新的分组Id
      var updateGroups = [];
      var buildTree = true;
      var needAddToGroupList = [];
      var _ref4 = data || {},
        sourceGroup = _ref4.sourceGroup,
        targetGroupId = _ref4.targetGroupId;
      targetGroupId && updateGroups.push(targetGroupId);
      sourceGroup.forEach(function (group) {
        updateGroups.push(group.groupId);
        if (!window.top["group-".concat(targetGroupId)] || !window.top["group-".concat(targetGroupId)].length) {
          window.top["group-".concat(targetGroupId)] = [];
          needAddToGroupList.push(targetGroupId);
        }
        if (!window.top.groupCache) {
          window.top.groupCache = {};
        }
        if (!window.top.groupCache[targetGroupId]) {
          window.top.groupCache[targetGroupId] = {};
        }
        group.equips.forEach(function (equip) {
          equip.title = equip.name;
          equip.groupId = targetGroupId;
          equip.equipNo = equip.id;
          if (window.top["group-".concat(group.groupId)]) {
            var index = window.top["group-".concat(group.groupId)].findIndex(function (item) {
              return item.id == equip.id;
            });
            index > -1 && window.top["group-".concat(group.groupId)].splice(index, 1);
          }
          if (!window.equipCache) {
            window.top.equipCache = {};
          }
          // 找分组,设备编辑需要用到
          window.top.equipCache[equip.id] = equip;

          // 设备状态需要用到
          window.top.groupCache[targetGroupId][equip.id] = equip;
          window.top["group-".concat(targetGroupId)].push(equip);
        });
        if (!window.top["group-".concat(group.groupId)].length) {
          var index = window.top.groupList.findIndex(function (item) {
            return item.id == group.groupId;
          });
          index > -1 && window.top.groupList.splice(index, 1);
          _this5.deleteChildGroup(group.groupId, window.top.groupList);
        }
      });
      needAddToGroupList.forEach(function (groupId) {
        if (!_this5.exist(groupId, window.top.groupList)) {
          var _window$top$groupList2;
          var list = _this5.findParentList(groupId, window.top.groupList_manageMent);
          (_window$top$groupList2 = window.top.groupList).push.apply(_window$top$groupList2, __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray___default()(list));
        }
      });
      this.notice({
        type: 'moveEquips',
        data: {
          updateGroups: updateGroups,
          buildTree: buildTree
        }
      });
    }
  }, {
    key: "exist",
    value: function exist(key, list) {
      if (list) {
        return list.some(function (item) {
          return item.id == key;
        });
      }
      return false;
    }
  }, {
    key: "findParentList",
    value: function findParentList(groupId, list) {
      var parentList = [];
      if (list) {
        var index = list.findIndex(function (group) {
          return group.id == groupId;
        });
        if (index > -1) {
          parentList.push(_objectSpread({}, list[index]));
        }
        if (list[index].parentId && !this.exist(list[index].parentId, window.top.groupList)) {
          parentList.push.apply(parentList, __WEBPACK_IMPORTED_MODULE_1__babel_runtime_helpers_toConsumableArray___default()(this.findParentList(list[index].parentId, window.top.groupList_manageMent)));
        }
      }
      return parentList;
    }

    // 删除设备
  }, {
    key: "DeleteEquip",
    value: function DeleteEquip(data) {
      var _ref5 = data || {},
        groupId = _ref5.groupId,
        equips = _ref5.equips;
      if (groupId && equips && equips instanceof Array) {
        if (window.top["group-".concat(groupId)]) {
          var _loop = function _loop(i) {
            var index = window.top["group-".concat(groupId)].findIndex(function (item) {
              return item.id == equips[i].id;
            });
            index > -1 && window.top["group-".concat(groupId)].splice(index, 1);
            //删除引用
            delete window.top.equipCache[equips[i].id];
            delete window.top.groupCache[groupId][equips[i].id];
          };
          for (var i = 0, length = equips.length; i < length; i++) {
            _loop(i);
          }
        }
        this.notice({
          type: 'DeleteEquip',
          data: data
        });
      }
    }

    // 编辑设备
  }, {
    key: "EditEquip",
    value: function EditEquip(data) {
      var _ref6 = data || {},
        equipNo = _ref6.equipNo,
        groupId = _ref6.groupId,
        equipName = _ref6.equipName;
      if (groupId && equipNo) {
        if (window["group-".concat(groupId)]) {
          window.top.equipCache[equipNo] = equipName;
        }
        this.notice({
          type: 'EditEquip',
          data: data
        });
      }
    }
  }, {
    key: "notice",
    value: function notice(data) {
      // 下发通知
      if (window.top.hasIframe) {
        var iframe = document.getElementsByTagName('iframe');
        var _iterator = _createForOfIteratorHelper(iframe),
          _step;
        try {
          for (_iterator.s(); !(_step = _iterator.n()).done;) {
            var item = _step.value;
            item.contentWindow.postMessage(data);
          }
        } catch (err) {
          _iterator.e(err);
        } finally {
          _iterator.f();
        }
      } else {
        window.postMessage(data, '*');
      }
    }
  }]);
  return equipCache;
}();


/***/ }),
/* 45 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
var render = function() {
  var _vm = this
  var _h = _vm.$createElement
  var _c = _vm._self._c || _h
  return _c(
    "tree",
    _vm._g(
      _vm._b(
        { ref: _vm.refId, attrs: { data: _vm.list, buildTree: _vm.buildTree } },
        "tree",
        Object.assign({}, _vm.$attrs, _vm.$props),
        false
      ),
      _vm.$listeners
    )
  )
}
var staticRenderFns = []
render._withStripped = true
var esExports = { render: render, staticRenderFns: staticRenderFns }
/* harmony default export */ __webpack_exports__["a"] = (esExports);
if (false) {
  module.hot.accept()
  if (module.hot.data) {
    require("vue-hot-reload-api")      .rerender("data-v-10382e54", esExports)
  }
}

/***/ }),
/* 46 */
/***/ (function(module, exports) {

// removed by extract-text-webpack-plugin

/***/ })
/******/ ])["default"];
});
