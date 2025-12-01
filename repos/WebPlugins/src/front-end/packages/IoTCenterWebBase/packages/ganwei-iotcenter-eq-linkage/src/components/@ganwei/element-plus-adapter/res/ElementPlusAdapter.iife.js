var ElementPlusAdapter = function(exports, elementPlus, vue) {
  "use strict";
  const _sfc_main$m = /* @__PURE__ */ vue.defineComponent({
    ...{
      inheritAttrs: false
    },
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      const loading = vue.ref(false);
      const handleClick = async (evt) => {
        const res = attrs.onClick(evt);
        if (res instanceof Promise) {
          loading.value = true;
          res.finally(() => {
            loading.value = false;
          });
        }
        return res;
      };
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElButton), vue.normalizeProps(vue.guardReactiveProps({ type: "primary", loading: loading.value, ...vue.unref(attrs), onClick: handleClick })), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$k = "";
  const _sfc_main$l = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElCascader), vue.normalizeProps(vue.guardReactiveProps(vue.unref(attrs))), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$j = "";
  const _sfc_main$k = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElCheckbox), vue.normalizeProps(vue.guardReactiveProps(vue.unref(attrs))), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$i = "";
  const _sfc_main$j = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElCollapse), null, vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1024);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$h = "";
  const _sfc_main$i = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    props: {
      defaultTime: {},
      valueFormat: {}
    },
    setup(__props) {
      const props = __props;
      const attrs = vue.useAttrs();
      const defaultTime = parseDate(props.defaultTime);
      const valueFormat = props.valueFormat || "YYYY-MM-DD HH:mm:ss";
      function parseDate(defaultTime2) {
        if (!defaultTime2) {
          return void 0;
        }
        if (Array.isArray(defaultTime2)) {
          if (defaultTime2[0] && defaultTime2[1]) {
            return [parseDate(defaultTime2[0]), parseDate(defaultTime2[1])];
          }
          return [new Date(2e3, 1, 1, 0, 0, 0), new Date(2e3, 1, 1, 23, 59, 59)];
        }
        if (defaultTime2 instanceof Date) {
          return defaultTime2;
        }
        const [h = 0, m = 0, s = 0] = defaultTime2.split(":");
        return new Date(2e3, 1, 1, Number(h), Number(m), Number(s));
      }
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElDatePicker), vue.mergeProps(vue.unref(attrs), {
          "default-time": vue.unref(defaultTime),
          "value-format": vue.unref(valueFormat)
        }), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040, ["default-time", "value-format"]);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$g = "";
  const _sfc_main$h = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElDialog), vue.normalizeProps(vue.guardReactiveProps(_ctx.$attrs)), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$f = "";
  const UI_AUTOMATION_TOKEN = `data-testid`;
  function useDataTestId() {
    const attrs = vue.useAttrs();
    if (attrs[UI_AUTOMATION_TOKEN] === void 0 || attrs[UI_AUTOMATION_TOKEN] === null || typeof attrs[UI_AUTOMATION_TOKEN] !== "string") {
      console.warn(`${UI_AUTOMATION_TOKEN} is required to UI Automation`);
    }
  }
  const _sfc_main$g = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      useDataTestId();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElDrawer), vue.normalizeProps(vue.guardReactiveProps(vue.unref(attrs))), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$e = "";
  const _sfc_main$f = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      useDataTestId();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElDropdown), vue.normalizeProps(vue.guardReactiveProps(vue.unref(attrs))), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$d = "";
  function useExpose(proxyElement) {
    const expose = {};
    vue.onMounted(() => {
      Object.assign(expose, proxyElement.value);
    });
    return expose;
  }
  const _sfc_main$e = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props, { expose: __expose }) {
      const proxyElement = vue.ref();
      const expose = useExpose(proxyElement);
      __expose(expose);
      const attrs = vue.useAttrs();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElForm), vue.mergeProps(vue.unref(attrs), {
          ref_key: "proxyElement",
          ref: proxyElement
        }), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$c = "";
  /*! Element Plus Icons Vue v2.3.1 */
  var picture_vue_vue_type_script_setup_true_lang_default = /* @__PURE__ */ vue.defineComponent({
    name: "Picture",
    __name: "picture",
    setup(__props) {
      return (_ctx, _cache) => (vue.openBlock(), vue.createElementBlock("svg", {
        xmlns: "http://www.w3.org/2000/svg",
        viewBox: "0 0 1024 1024"
      }, [
        vue.createElementVNode("path", {
          fill: "currentColor",
          d: "M160 160v704h704V160zm-32-64h768a32 32 0 0 1 32 32v768a32 32 0 0 1-32 32H128a32 32 0 0 1-32-32V128a32 32 0 0 1 32-32"
        }),
        vue.createElementVNode("path", {
          fill: "currentColor",
          d: "M384 288q64 0 64 64t-64 64q-64 0-64-64t64-64M185.408 876.992l-50.816-38.912L350.72 556.032a96 96 0 0 1 134.592-17.856l1.856 1.472 122.88 99.136a32 32 0 0 0 44.992-4.864l216-269.888 49.92 39.936-215.808 269.824-.256.32a96 96 0 0 1-135.04 14.464l-122.88-99.072-.64-.512a32 32 0 0 0-44.8 5.952z"
        })
      ]));
    }
  });
  var picture_default = picture_vue_vue_type_script_setup_true_lang_default;
  const _hoisted_1 = { class: "error-slot" };
  const _sfc_main$d = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      return (_ctx, _cache) => {
        const _component_el_icon = vue.resolveComponent("el-icon");
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElImage), {
          fit: "contain",
          lazy: ""
        }, {
          error: vue.withCtx(() => [
            vue.createElementVNode("div", _hoisted_1, [
              vue.createVNode(_component_el_icon, null, {
                default: vue.withCtx(() => [
                  vue.createVNode(vue.unref(picture_default))
                ]),
                _: 1
              })
            ])
          ]),
          _: 1
        });
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$b = "";
  const _sfc_main$c = /* @__PURE__ */ vue.defineComponent({
    __name: "ElInput",
    props: {
      "modelValue": {
        default: ""
      },
      "modelModifiers": {}
    },
    emits: /* @__PURE__ */ vue.mergeModels(["change"], ["update:modelValue"]),
    setup(__props, { emit: __emit }) {
      const model = vue.useModel(__props, "modelValue");
      let preChangeVal = "";
      const emits = __emit;
      function changeHandler(val) {
        if (preChangeVal === val)
          return;
        preChangeVal = val;
        emits("change", val);
      }
      const input = vue.ref();
      function inputHandler(val) {
        if (model.value !== "" && val.trim() === "") {
          changeHandler("");
        }
        model.value = val.trim();
      }
      const attrs = vue.useAttrs();
      useDataTestId();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElInput), vue.mergeProps({
          ref_key: "input",
          ref: input,
          modelValue: model.value,
          "onUpdate:modelValue": _cache[0] || (_cache[0] = ($event) => model.value = $event)
        }, vue.unref(attrs), {
          onInput: inputHandler,
          onChange: changeHandler
        }), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040, ["modelValue"]);
      };
    }
  });
  const ElInput_vue_vue_type_style_index_0_lang = "";
  const index$2 = "";
  const vLoading = {
    mounted(el, binding, vnode, prevVnode) {
      var _a, _b;
      const vueInstance = vnode.ctx.proxy;
      el.setAttribute(`element-loading-text`, (vueInstance == null ? void 0 : vueInstance.$t("publics.tips.loading")) + "...");
      el.setAttribute(`element-loading-spinner`, " ");
      el.setAttribute(`element-loading-background`, "customLoading");
      (_b = (_a = elementPlus.vLoading).mounted) == null ? void 0 : _b.call(_a, el, binding, vnode, prevVnode);
    },
    updated(el, binding, vnode, prevVnode) {
      var _a, _b;
      (_b = (_a = elementPlus.vLoading).updated) == null ? void 0 : _b.call(_a, el, binding, vnode, prevVnode);
    },
    unmounted(el, binding, vnode, prevVnode) {
      var _a, _b;
      (_b = (_a = elementPlus.vLoading).unmounted) == null ? void 0 : _b.call(_a, el, binding, vnode, prevVnode);
    }
  };
  const _sfc_main$b = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      useDataTestId();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElMenu), vue.normalizeProps(vue.guardReactiveProps(vue.unref(attrs))), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$a = "";
  const index$1 = "";
  const index = "";
  const install = elementPlus.ElNotification.install;
  const defaultOption = {
    duration: 2e3
  };
  const ElNotification = elementPlus.ElNotification;
  const notificationTypes = [
    "success",
    "info",
    "warning",
    "error"
  ];
  notificationTypes.forEach((type) => {
    ElNotification[type] = (options) => {
      if (typeof options === "string") {
        return elementPlus.ElNotification[type](options);
      }
      if (vue.isVNode(options)) {
        return elementPlus.ElNotification[type](options);
      }
      let data = {
        ...defaultOption,
        ...options,
        customClass: type,
        type
      };
      return ElNotification(data);
    };
  });
  ElNotification.install = (app) => {
    install == null ? void 0 : install(app);
    app.config.globalProperties.$notify = ElNotification;
  };
  const _sfc_main$a = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      useDataTestId();
      const defaultAttrs = {
        pageSizes: [20, 50, 100],
        layout: "sizes,prev, pager, next,total"
      };
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElPagination), vue.normalizeProps(vue.guardReactiveProps({ ...defaultAttrs, ...vue.unref(attrs) })), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$9 = "";
  const _sfc_main$9 = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElPopover), null, vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1024);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$8 = "";
  const _sfc_main$8 = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElRadio), vue.normalizeProps(vue.guardReactiveProps(vue.unref(attrs))), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$7 = "";
  const _sfc_main$7 = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    props: {
      round: {
        type: Boolean,
        default: false
      }
    },
    setup(__props) {
      const attrs = vue.useAttrs();
      useDataTestId();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElRadioGroup), vue.mergeProps(vue.unref(attrs), {
          class: { round: __props.round }
        }), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040, ["class"]);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$6 = "";
  const _sfc_main$6 = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props, { expose: __expose }) {
      const proxyElement = vue.ref();
      const expose = useExpose(proxyElement);
      __expose(expose);
      const attrs = vue.useAttrs();
      useDataTestId();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElSelect), vue.mergeProps(vue.unref(attrs), {
          ref_key: "proxyElement",
          ref: proxyElement
        }), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$5 = "";
  const _sfc_main$5 = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      useDataTestId();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElSwitch), vue.normalizeProps(vue.guardReactiveProps(vue.unref(attrs))), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$4 = "";
  function useResizeTable() {
    const instance = vue.getCurrentInstance();
    const height = vue.ref();
    let el = null;
    vue.onMounted(() => {
      resizeTable();
      window.addEventListener("resize", resizeTable);
    });
    vue.onBeforeUnmount(() => {
      window.removeEventListener("resize", resizeTable);
    });
    function getPxNumber(value) {
      if (value.endsWith("px")) {
        return Number(value.slice(0, -2));
      }
      return 0;
    }
    function resizeTable() {
      var _a, _b;
      el = (_b = (_a = instance == null ? void 0 : instance.proxy) == null ? void 0 : _a.$el) == null ? void 0 : _b.parentElement;
      if (el) {
        const stylesheet = getComputedStyle(el);
        height.value = el.clientHeight - getPxNumber(stylesheet.paddingTop) - getPxNumber(stylesheet.paddingBottom);
      }
    }
    return height;
  }
  const _sfc_main$4 = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    props: {
      height: {
        type: Number,
        default: null
      }
    },
    setup(__props) {
      const height = useResizeTable();
      useDataTestId();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElTable), { height: vue.unref(height) }, vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1032, ["height"]);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$3 = "";
  const _sfc_main$3 = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      const attrs = vue.useAttrs();
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElTimePicker), vue.normalizeProps(vue.guardReactiveProps(vue.unref(attrs))), vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1040);
      };
    }
  });
  const _sfc_main$2 = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElTransfer));
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$2 = "";
  const _sfc_main$1 = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props, { expose: __expose }) {
      const tree = vue.ref();
      const expose = useExpose(tree);
      __expose(expose);
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElTree), {
          ref_key: "tree",
          ref: tree
        }, vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1536);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang$1 = "";
  const _sfc_main = /* @__PURE__ */ vue.defineComponent({
    __name: "index",
    setup(__props) {
      return (_ctx, _cache) => {
        return vue.openBlock(), vue.createBlock(vue.unref(elementPlus.ElUpload), { class: "upload_wrapper" }, vue.createSlots({ _: 2 }, [
          vue.renderList(_ctx.$slots, (value, key) => {
            return {
              name: key,
              fn: vue.withCtx((slotProps) => [
                vue.renderSlot(_ctx.$slots, key, vue.mergeProps(slotProps, { key }))
              ])
            };
          })
        ]), 1024);
      };
    }
  });
  const index_vue_vue_type_style_index_0_lang = "";
  const Adapter = {
    ElInput: _sfc_main$c,
    ElDropdown: _sfc_main$f,
    ElMenu: _sfc_main$b,
    ElRadioGroup: _sfc_main$7,
    ElTable: _sfc_main$4,
    ElSelect: _sfc_main$6,
    ElPagination: _sfc_main$a,
    ElButton: _sfc_main$m,
    ElForm: _sfc_main$e,
    ElCascader: _sfc_main$l,
    ElRadio: _sfc_main$8,
    ElCheckbox: _sfc_main$k,
    ElDatePicker: _sfc_main$i,
    ElTimePicker: _sfc_main$3,
    ElDialog: _sfc_main$h,
    ElSwitch: _sfc_main$5,
    ElDrawer: _sfc_main$g,
    ElPopover: _sfc_main$9,
    ElTransfer: _sfc_main$2,
    ElCollapse: _sfc_main$j,
    ElImage: _sfc_main$d,
    ElUpload: _sfc_main,
    ElTree: _sfc_main$1
  };
  function adapterInstall(app) {
    for (const [key, component] of Object.entries(Adapter)) {
      app.component(key, component);
    }
    adapterLoading(app);
    app.use(elementPlus.ElMessageBox);
    app.use(ElNotification);
  }
  function adapterLoading(app) {
    app.directive("loading", vLoading);
  }
  exports.adapterInstall = adapterInstall;
  exports.adapterLoading = adapterLoading;
  exports.default = Adapter;
  Object.defineProperties(exports, { __esModule: { value: true }, [Symbol.toStringTag]: { value: "Module" } });
  return exports;
}({}, ElementPlus, Vue);
//# sourceMappingURL=ElementPlusAdapter.iife.js.map
