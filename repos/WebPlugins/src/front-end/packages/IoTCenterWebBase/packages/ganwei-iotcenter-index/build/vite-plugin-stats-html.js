import Y from 'fs'
import L from 'path'
var q = (t, e) => () => (e || t((e = { exports: {} }).exports, e), e.exports);
var G = q((f, F) => {
  const V = (t, e) => () => {
    let i = "";
    for (let n = 0; n < e; n++)
      i += t[Math.random() * t.length | 0];
    return i;
  }, _ = V("codercao1234", 4), A = _();
  let X = 0;
  const w = () => `${A}-${X++}`, I = class {
    // 定义节点部分和节点元数据
    // 节点部分以模块的 UID 为键，存储模块部分的长度信息
    // 节点元数据以模块的 ID 为键，存储模块的 UID 和元数据
    // 元数据包括模块 ID、模块部分、导入模块和被导入模块的 ID 集合等信息
    constructor(e) {
      this.projectRoot = e, this.nodeParts = {}, this.nodeMetas = {};
    }
    // 去除模块 ID 中的项目根路径前缀
    trimProjectRootId(e) {
      return typeof this.projectRoot == "string" && e.startsWith(this.projectRoot) ? e.slice(this.projectRoot.length) : e.replace(this.projectRoot, "");
    }
    // 获取模块 ID 对应的 UID
    getModuleUid(e) {
      return e in this.nodeMetas || (this.nodeMetas[e] = {
        uid: w(),
        meta: {
          id: this.trimProjectRootId(e),
          moduleParts: {},
          imported: /* @__PURE__ */ new Set(),
          importedBy: /* @__PURE__ */ new Set()
        }
      }), this.nodeMetas[e].uid;
    }
    // 获取 bundle ID 和模块 ID 对应的 UID
    getBundleModuleUid(e, i) {
      return i in this.nodeMetas || (this.nodeMetas[i] = {
        uid: w(),
        meta: {
          id: this.trimProjectRootId(i),
          moduleParts: {},
          imported: /* @__PURE__ */ new Set(),
          importedBy: /* @__PURE__ */ new Set()
        }
      }), e in this.nodeMetas[i].meta.moduleParts || (this.nodeMetas[i].meta.moduleParts[e] = w()), this.nodeMetas[i].meta.moduleParts[e];
    }
    // 设置节点部分
    setNodePart(e, i, n) {
      const s = this.getBundleModuleUid(e, i);
      if (s in this.nodeParts)
        throw new Error(
          `Override module: bundle id ${e}, module id ${i}, value ${JSON.stringify(
            n
          )}, existing value: ${JSON.stringify(this.nodeParts[s])}`
        );
      return this.nodeParts[s] = { ...n, metaUid: this.getModuleUid(i) }, s;
    }
    // 设置节点元数据
    setNodeMeta(e, i) {
      this.getModuleUid(e), this.nodeMetas[e].meta.isEntry = i.isEntry, this.nodeMetas[e].meta.isExternal = i.isExternal;
    }
    // 检查是否存在节点部分
    hasNodePart(e, i) {
      return !(!(i in this.nodeMetas) || !(e in this.nodeMetas[i].meta.moduleParts) || !(this.nodeMetas[i].meta.moduleParts[e] in this.nodeParts));
    }
    // 获取节点部分
    getNodeParts() {
      return this.nodeParts;
    }
    // 获取节点元数据
    getNodeMetas() {
      const e = {};
      for (const { uid: i, meta: n } of Object.values(this.nodeMetas))
        e[i] = {
          ...n,
          imported: [...n.imported].map((s) => {
            const [l, r] = s.split(","), o = { uid: l };
            return r === "true" && (o.dynamic = !0), o;
          }),
          importedBy: [...n.importedBy].map((s) => {
            const [l, r] = s.split(","), o = { uid: l };
            return r === "true" && (o.dynamic = !0), o;
          })
        };
      return e;
    }
    // 添加被导入模块链接
    addImportedByLink(e, i) {
      const n = this.getModuleUid(i);
      this.getModuleUid(e), this.nodeMetas[e].meta.importedBy.add(n);
    }
    // 添加导入模块链接
    addImportedLink(e, i, n = !1) {
      const s = this.getModuleUid(i);
      this.getModuleUid(e), this.nodeMetas[e].meta.imported.add(String([s, n]));
    }
  }, b = (t) => "children" in t, R = (t, e, i, n) => {
    if (i.length === 0)
      throw new Error(`Error adding node to path ${t}`);
    const [s, ...l] = i;
    if (l.length === 0) {
      e.children.push({ ...n, name: s });
      return;
    } else {
      let r = e.children.find(
        (o) => o.name === s && b(o)
      );
      r || (r = { name: s, children: [] }, e.children.push(r)), R(t, r, l, n);
      return;
    }
  }, S = (t) => {
    if (t.children.length === 1) {
      const e = t.children[0], i = `${t.name}/${e.name}`;
      return b(e) ? (t.name = i, t.children = e.children, S(t)) : {
        name: i,
        uid: e.uid,
        value: e.value
      };
    } else
      return t.children = t.children.map((e) => b(e) ? S(e) : e), t;
  }, K = (t, e, i) => {
    const n = {
      name: t,
      children: []
    };
    for (const { id: s, renderedLength: l } of e) {
      const r = i.setNodePart(t, s, {
        renderedLength: l
      }), o = i.trimProjectRootId(s), p = o.split(/\\|\//).filter((h) => h !== "");
      R(o, n, p, {
        uid: r,
        value: l
      });
    }
    return n.children = n.children.map((s) => b(s) ? S(s) : s), n;
  }, Q = (t) => `
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<!-- 引入样式 -->
<link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
<style>
body {
  background-color: rgb(246, 247, 251);
}
.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}
.container {
  width: 1280px;
  margin: 0px auto;
  background-color: rgb(255, 255, 255);
  border: 1px solid rgb(225, 223, 221);
  border-radius: 2px;
  padding: 0 20px 20px 20px;
}
.header-wrap {
  min-height: 56px;
  display: flex;
  -webkit-box-align: center;
  align-items: center;
  padding: 12px 20px 0px;
  font-size: 16px;
  line-height: 24px;
  font-weight: 600;
  color: rgb(27, 26, 25);
}
.bundle-info {
  display: flex;
  border-radius: 4px;
  border: 1px solid rgb(225, 223, 221);
  padding: 12px 16px;
  justify-content: space-between;
  margin-bottom: 20px;
}
.bundle-info-title {
  font-size: 14px;
  font-weight: 600;
}
.bundle-overview {
  flex-wrap: wrap;
  width: auto;
  overflow: visible;
  height: auto;
  display: flex;
  justify-content: space-between;
}
.bundle-left {
  width: 60%;
  height: 300px;
  border-radius: 4px;
  display: flex;
  justify-content: space-between;
  align-content: space-between;
  flex-wrap: wrap;
}
.bundle-left-item {
  width: calc(30% - 20px);
  height: calc(45% - 20px);
  border-radius: 4px;
  border: 1px solid rgb(225, 223, 221);
  padding: 12px 16px;
  text-align: left;
}
.kb {
  font-size: 14px;
  font-weight: 400;
}
.bundle-left-item-title {
  text-align: left;
  font-size: 14px;
  font-weight: 600;
  line-height: 22px;
}
.bundle-left-item-value {
  font-size: 28px;
  font-weight: 600;
  line-height: 100px;
}
.bundle-right {
  width: calc(40% - 20px);
  border-radius: 4px;
  height: 300px;
  border: 1px solid rgb(225, 223, 221);
}
.pie {
  position: relative;
  width: 100%;
  height: 100%;
  padding: 20px;
}
.tablist {
  margin-top: 20px;
}
.visualization {
    width: 1248px;
    height: 500px;
    background: #eebe77;
  }
</style>
<script src="https://unpkg.com/vue@2"><\/script>
<script src="https://unpkg.com/element-ui/lib/index.js"><\/script>
<script src="https://cdn.jsdelivr.net/npm/echarts@5.2.2/dist/echarts.min.js"><\/script>
</head>
<body>
<div class="app" id="app">
<div class="container">
  <div class="header-wrap">${t.title}</div>
  <div class="bundle-info">
    <div>
      <span class="bundle-info-title">project URL：</span
      >{{ bundleObj.projectRoot }}
    </div>
    <div>
      <span class="bundle-info-title">built in：</span> {{ bundleObj.time }}
    </div>
    <div>
      <span class="bundle-info-title">built Date:</span>
      {{ bundleObj.startTime }}
    </div>
  </div>
  <div class="bundle-overview">
    <div class="bundle-left">
      <div
        class="bundle-left-item"
        v-for="item in bundleList"
        :key="item.key"
      >
        <div class="bundle-left-item-title" v-if="item.content">
          <el-popover
            :ref="item.key"
            :title="item.content.title"
            width="200"
            trigger="hover"
            :content="item.content.content"
          >
          </el-popover>
          <span v-popover="item.key">{{ item.title }}</span>
        </div>
        <div v-else class="bundle-left-item-title">{{ item.title }}</div>
        <div class="bundle-left-item-value">
          {{ bundleObj[item.key] }}
          <span v-if="!item.content" class="kb">kb</span>
        </div>
      </div>
    </div>
    <div class="bundle-right">
      <div class="pie" id="pie"></div>
    </div>
  </div>
  <el-tabs type="border-card" class="tablist">
    <el-tab-pane label="Visualization">
      <div id="visualization" class="visualization"></div>
    </el-tab-pane>
    <el-tab-pane label="Assets Statistics">
      <el-table :data="tableData" height="400" stripe style="width: 100%">
        <el-table-column prop="file" sortable label="file" width="500px">
        </el-table-column>
        <el-table-column prop="type" sortable label="type"> </el-table-column>
        <el-table-column prop="size" sortable label="size(kb)"> </el-table-column>
        <el-table-column prop="dependencyCount" sortable label="dependencyCount">
        </el-table-column>
      </el-table>
    </el-tab-pane>
  </el-tabs>
</div>
</div>
</body>


<script>
var app = new Vue({
  el: '#app',
  data: {
    bundleObj:${JSON.stringify(t.bundleObj)},
    tableData:${JSON.stringify(t.tableData)},
    bundleList:[
        {
          key: 'totalSize',
          title: 'Bundle Size',
          content: '',
        },
        {
          key: 'jsSize',
          title: 'Initial JS Size',
          content: '',
        },
        {
          key: 'cssSize',
          title: 'Initial CSS Size',
          content: '',
        },
        {
          key: 'assetCount',
          title: 'Assets Count',
          content: {
            title: 'Webpack ouput assets',
            content:
              'Files emitted by webpack.<br/> Including any JavaScript CSS font Image files which processed by webpack.',
          },
        },
        {
          key: 'chunkCount',
          title: 'Chunks Count',
          content: {
            title: 'Chunks',
            content:
              '(1)initial is the main chunk for the entry point. This chunk contains all the modules and its dependencies that you specify for an entry point.initial is the main chunk for the entry point. This chunk contains all the modules and its dependencies that you specify for an entry point.non-initial is a chunk that may be lazy-loaded. It may appear when dynamic import or SplitChunksPlugin is being used.',
          },
        },
        {
          key: 'packageCount',
          title: 'Packages Count',
          content: {
            title: 'node_modules',
            content: 'Third part packages count in node_modules.',
          },
        },
      ]
  },
  methods: {
    setPieChart(){
        // 基于准备好的dom，初始化echarts实例
        var myChart = echarts.init(document.getElementById('pie'))
        // 绘制图表
        myChart.setOption({
          title: {
            text: 'Bundle Overview',
          },
          tooltip: {
            trigger: 'item',
          },
          legend: {
            orient: 'vertical',
            left: 'left',
            top: '30%',
          },
          series: [
            {
              name: 'Bundle Overview',
              type: 'pie',
              radius: '50%',
              data: [
                { value: ${t.bundleObj.jsSize}, name: 'JS' },
                { value: ${t.bundleObj.cssSize}, name: 'CSS' },
                { value: ${t.bundleObj.imageSize}, name: 'Image' },
                { value: ${t.bundleObj.htmlSize}, name: 'Font' },
                { value: ${t.bundleObj.fontSize}, name: 'Html' },
              ],
              emphasis: {
                itemStyle: {
                  shadowBlur: 10,
                  shadowOffsetX: 0,
                  shadowColor: 'rgba(0, 0, 0, 0.5)',
                },
              },
            },
          ],
        })
    },
    getLevelOption() {
        return [
          {
            itemStyle: {
              borderColor: '#777',
              borderWidth: 0,
              gapWidth: 1
            },
            upperLabel: {
              show: false
            }
          },
          {
            itemStyle: {
              borderColor: '#555',
              borderWidth: 5,
              gapWidth: 1
            },
            emphasis: {
              itemStyle: {
                borderColor: '#ddd'
              }
            }
          },
          {
            colorSaturation: [0.35, 0.5],
            itemStyle: {
              borderWidth: 5,
              gapWidth: 1,
              borderColorSaturation: 0.6
            }
          }
        ];
      },
    setTreeChart(){
        // 基于准备好的dom，初始化echarts实例
        var myChart = echarts.init(document.getElementById('visualization'))
        // 绘制图表
        myChart.setOption({
          title: {
              text: 'visualization',
              left: 'center'
          },
          tooltip: {
              formatter: function(info) {
                  var value = info.value;
                  var treePathInfo = info.treePathInfo;
                  var treePath = [];
                  for (var i = 1; i < treePathInfo.length; i++) {
                      treePath.push(treePathInfo[i].name);
                  }
                  return [
                      '<div class="tooltip-title">' +
                      echarts.format.encodeHTML(treePath.join('/')) +
                      '</div>',
                      'size: ' + echarts.format.addCommas(value) + ' KB'
                  ].join('');
              }
          },
          series: [{
              name: 'root',
              type: 'treemap',
              visibleMin: 300,
              label: {
                  show: true,
                  formatter: '{b}'
              },
              upperLabel: {
                  show: true,
                  height: 30
              },
              itemStyle: {
                  borderColor: '#fff'
              },
              levels: this.getLevelOption(),
              data: ${JSON.stringify(t.treeData)},
          }]
      })
    }
  },
  mounted(){
    this.setPieChart()
    this.setTreeChart()
  }
})
<\/script>
</html>
  `, E = (t = {}) => {
    let e;
    return {
      name: "visualizer",
      buildStart() {
        e = Date.now();
      },
      async generateBundle(i, n) {
        var B, T;
        const s = t.output || "stats.html", l = t.title ?? "Vite Plugin Status Html", r = t.projectRoot ?? process.cwd(), o = [], p = new I(r), h = async ({ id: c, renderedLength: a, code: u }) => {
          const d = u == null || u == "";
          return {
            id: c,
            renderedLength: d ? a : Buffer.byteLength(u, "utf-8")
          };
        };
        let k = 0, j = 0, z = 0, M = 0, P = 0, C = 0, O = 0, N = 0, U = 0, $ = [];
        for (const [c, a] of Object.entries(n)) {
          let u = L.extname(a.fileName).slice(1), d = ((B = a == null ? void 0 : a.code) == null ? void 0 : B.length) || ((T = a == null ? void 0 : a.source) == null ? void 0 : T.length);
          switch (u) {
            case "js":
              P += d;
              break;
            case "css":
              C += d;
              break;
            case "jpg":
            case "jpeg":
            case "png":
            case "gif":
            case "svg":
              O += d;
              break;
            case "html":
              N += d;
              break;
            case "woff":
            case "woff2":
            case "ttf":
            case "otf":
              U += d;
              break;
          }
          const g = Object.keys(a.modules ?? []).length;
          if (M += d, k++, $.push({
            file: a.fileName,
            type: u,
            size: Number(d / 1e3).toFixed(2),
            dependencyCount: g
          }), a.type !== "chunk")
            continue;
          z += g;
          let v;
          const D = await Promise.all(
            Object.entries(a.modules).map(
              ([y, { renderedLength: m, code: x }]) => h({ id: y, renderedLength: m, code: x })
            )
          );
          if (v = K(c, D, p), v.children.length === 0) {
            const y = await h({
              id: c,
              renderedLength: a.code.length,
              code: a.code
            }), m = a.facadeModuleId ?? `${c}-unknown`, x = p.setNodePart(
              c,
              m,
              y
            );
            p.setNodeMeta(m, { isEntry: !0 });
            const H = { name: c, uid: x };
            o.push(H);
          } else
            o.push(v);
        }
        j = Object.keys(n).length;
        let J = {
          bundleObj: {
            title: l,
            projectRoot: r,
            time: (Date.now() - e) / 1e3 + "s",
            startTime: (/* @__PURE__ */ new Date()).toLocaleString(),
            totalSize: Number(M / 1e3).toFixed(2),
            assetCount: k,
            chunkCount: j,
            packageCount: z,
            jsSize: Number(P / 1e3).toFixed(2),
            cssSize: Number(C / 1e3).toFixed(2),
            imageSize: Number(O / 1e3).toFixed(2),
            htmlSize: Number(N / 1e3).toFixed(2),
            fontSize: Number(U / 1e3).toFixed(2)
          },
          tableData: $,
          treeData: o
        };
        const W = Q(J);
        await Y.writeFileSync(L.join("./", s), W);
      }
    };
  };
  F.exports = E;
  f.visualizer = E;
  f.default = f.visualizer;
});
export default G();
