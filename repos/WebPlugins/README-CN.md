<p align="left" dir="auto">
  <a href="https://opensource.ganweicloud.com" rel="nofollow">
    <img width="130" height="130" src="src/logo.jpg">
  </a>
</p>

[![GitHub license](https://camo.githubusercontent.com/5eaf3ed8a7e8ccb15c21d967b8635ac79e8b1865da3a5ccf78d2572a3e10738a/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f646f746e65742f6173706e6574636f72653f636f6c6f723d253233306230267374796c653d666c61742d737175617265)](https://github.com/ganweisoft/WebPlugins/blob/main/LICENSE) [![Build Status](https://github.com/ganweisoft/TOMs/actions/workflows/build.yml/badge.svg)](https://github.com/ganweisoft/TOMs/actions) ![https://v2.vuejs.org/](https://img.shields.io/badge/Vue-3.5.13-%2394c20c?labelColor=#94c20c) ![https://www.webpackjs.com](https://img.shields.io/badge/vite-4.5.5-%234ec428?labelColor=#5a5a5a) ![https://www.axios-http.cn/docs/intro](https://img.shields.io/badge/Axios-1.7.9-%2397c424?labelColor=#5a5a5a) ![https://next.router.vuejs.org](https://img.shields.io/badge/vueRouter-4.5.0-%23d6604a?labelColor=#5a5a5a) ![https://element.eleme.io/#/zh-CN](https://img.shields.io/badge/ElementUI-2.9.1-%23097abb?labelColor=#5a5a5a) ![](https://img.shields.io/badge/join-discord-infomational)

简体中文 | [English](README.md)

WebPlugins 是一个基于ASP.NET Core和VUE的模块化和插件化应用程序框架，基于松耦合、高内聚的设计理念，构建了一个可扩展、易维护的应用框架，通过将核心逻辑与功能组件完全解耦，可进行二次开发

## [前端开发指南](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN)
- [1. 环境配置与项目初始化](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#1-%E7%8E%AF%E5%A2%83%E9%85%8D%E7%BD%AE%E4%B8%8E%E9%A1%B9%E7%9B%AE%E5%88%9D%E5%A7%8B%E5%8C%96)
  - [1.1 开发环境搭建](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#11-%E5%BC%80%E5%8F%91%E7%8E%AF%E5%A2%83%E6%90%AD%E5%BB%BA)
  - [1.2 项目架构解析](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#12-%E9%A1%B9%E7%9B%AE%E6%9E%B6%E6%9E%84%E8%A7%A3%E6%9E%90)

- [2. 子应用开发规范](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#2-%E5%AD%90%E5%BA%94%E7%94%A8%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83)
  - [2.1 模板工程体系](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#21-%E6%A8%A1%E6%9D%BF%E5%B7%A5%E7%A8%8B%E4%BD%93%E7%B3%BB)
  - [2.2 菜单系统配置](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#22-%E8%8F%9C%E5%8D%95%E7%B3%BB%E7%BB%9F%E9%85%8D%E7%BD%AE)
  - [2.3 业务代码开发规范](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#23-%E4%B8%9A%E5%8A%A1%E4%BB%A3%E7%A0%81%E5%BC%80%E5%8F%91%E8%A7%84%E8%8C%83)
  - [2.4 启动流程说明](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#24-%E5%90%AF%E5%8A%A8%E6%B5%81%E7%A8%8B%E8%AF%B4%E6%98%8E)

- [3. 界面主题系统](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#3-%E7%95%8C%E9%9D%A2%E4%B8%BB%E9%A2%98%E7%B3%BB%E7%BB%9F)
  - [3.1 主题资源引用机制](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#31-%E4%B8%BB%E9%A2%98%E8%B5%84%E6%BA%90%E5%BC%95%E7%94%A8%E6%9C%BA%E5%88%B6)
  - [3.2 动态主题切换实现](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#32-%E5%8A%A8%E6%80%81%E4%B8%BB%E9%A2%98%E5%88%87%E6%8D%A2%E5%AE%9E%E7%8E%B0)

- [4. 构建与部署](https://github.com/ganweisoft/WebPlugins/wiki/front%E2%80%90end%E2%80%90CN#4-%E6%9E%84%E5%BB%BA%E4%B8%8E%E9%83%A8%E7%BD%B2)


## [后端开发指南](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end.README.zh%E2%80%90cn)
- [1. 插件工程架构](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#%E6%8F%92%E4%BB%B6%E5%B7%A5%E7%A8%8B%E6%9E%B6%E6%9E%84)
  - [1.1 工程化解决方案](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#11-%E5%B7%A5%E7%A8%8B%E5%8C%96%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88)
    - [1.1.1 下载模板引擎](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#111-%E4%B8%8B%E8%BD%BD%E6%A8%A1%E6%9D%BF%E5%BC%95%E6%93%8E)
    - [1.1.2 命令行创建插件模板解决方案](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#112-%E5%91%BD%E4%BB%A4%E8%A1%8C%E5%88%9B%E5%BB%BA%E6%8F%92%E4%BB%B6%E6%A8%A1%E6%9D%BF%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88)
    - [1.1.3 VisualStudio创建插件模板解决方案](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#113-visualstudio%E5%88%9B%E5%BB%BA%E6%8F%92%E4%BB%B6%E6%A8%A1%E6%9D%BF%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88)
  - [1.2 解决方案结构](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#12-%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88%E7%BB%93%E6%9E%84)
    - [1.2.1 Solution Items解决方案文件夹](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#121-solution-items%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88%E6%96%87%E4%BB%B6%E5%A4%B9)
    - [1.2.2 docs解决方案文件夹](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#122-docs%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88%E6%96%87%E4%BB%B6%E5%A4%B9)
    - [1.2.3 build解决方案文件夹](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#123-build%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88%E6%96%87%E4%BB%B6%E5%A4%B9)
    - [1.2.4 插件项目](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#124-%E6%8F%92%E4%BB%B6%E9%A1%B9%E7%9B%AE)
- [2. 上下文管理规范](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#2-%E4%B8%8A%E4%B8%8B%E6%96%87%E7%AE%A1%E7%90%86%E8%A7%84%E8%8C%83)
  - [2.1 模型映射规范](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#21-%E6%A8%A1%E5%9E%8B%E6%98%A0%E5%B0%84%E8%A7%84%E8%8C%83)
  - [2.2 上下文命名规范](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#22-%E4%B8%8A%E4%B8%8B%E6%96%87%E5%91%BD%E5%90%8D%E8%A7%84%E8%8C%83)
  - [2.3 上下文基本说明](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#23-%E4%B8%8A%E4%B8%8B%E6%96%87%E5%9F%BA%E6%9C%AC%E8%AF%B4%E6%98%8E)
  - [2.4 跨插件模型操作说明](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#24-%E8%B7%A8%E6%8F%92%E4%BB%B6%E6%A8%A1%E5%9E%8B%E6%93%8D%E4%BD%9C%E8%AF%B4%E6%98%8E)
- [3. 接口定义及调用](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#3-%E6%8E%A5%E5%8F%A3%E5%AE%9A%E4%B9%89%E5%8F%8A%E8%B0%83%E7%94%A8)
  - [3.1 服务接口定义](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#31-%E6%9C%8D%E5%8A%A1%E6%8E%A5%E5%8F%A3%E5%AE%9A%E4%B9%89)
  - [3.2 服务接口实现](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#32-%E6%9C%8D%E5%8A%A1%E6%8E%A5%E5%8F%A3%E5%AE%9E%E7%8E%B0)
  - [3.3 服务接口注册](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#33-%E6%9C%8D%E5%8A%A1%E6%8E%A5%E5%8F%A3%E6%B3%A8%E5%86%8C)
  - [3.4 控制器继承](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#34-%E6%8E%A7%E5%88%B6%E5%99%A8%E7%BB%A7%E6%89%BF)
  - [3.5 本地调试接口](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#35-%E6%9C%AC%E5%9C%B0%E8%B0%83%E8%AF%95%E6%8E%A5%E5%8F%A3)
  - [3.6 接口调试工具](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#36-%E6%8E%A5%E5%8F%A3%E8%B0%83%E8%AF%95%E5%B7%A5%E5%85%B7)
- [4. 日志管理](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#4-%E6%97%A5%E5%BF%97%E7%AE%A1%E7%90%86)
  - [4.1 平台日志输出](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#41-%E5%B9%B3%E5%8F%B0%E6%97%A5%E5%BF%97%E8%BE%93%E5%87%BA)
  - [4.2 插件日志输出](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#42-%E6%8F%92%E4%BB%B6%E6%97%A5%E5%BF%97%E8%BE%93%E5%87%BA)
- [5. 常用帮助扩展](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#5-%E5%B8%B8%E7%94%A8%E5%B8%AE%E5%8A%A9%E6%89%A9%E5%B1%95)
  - [5.1 字符串扩展](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#51-%E5%AD%97%E7%AC%A6%E4%B8%B2%E6%89%A9%E5%B1%95)
  - [5.2 日期扩展](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#52-%E6%97%A5%E6%9C%9F%E6%89%A9%E5%B1%95)
  - [5.3 序列化扩展](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#53-%E5%BA%8F%E5%88%97%E5%8C%96%E6%89%A9%E5%B1%95)
  - [5.4 枚举扩展](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#54-%E6%9E%9A%E4%B8%BE%E6%89%A9%E5%B1%95)
  - [5.5 键值对对象](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#55-%E9%94%AE%E5%80%BC%E5%AF%B9%E5%AF%B9%E8%B1%A1)
- [6. 会话信息](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#6-%E4%BC%9A%E8%AF%9D%E4%BF%A1%E6%81%AF)
- [7. 读取配置](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#7-%E8%AF%BB%E5%8F%96%E9%85%8D%E7%BD%AE)
- [8. 定时作业](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#8-%E5%AE%9A%E6%97%B6%E4%BD%9C%E4%B8%9A)
  - [8.1 使用示例](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#81-%E4%BD%BF%E7%94%A8%E7%A4%BA%E4%BE%8B)
  - [8.2 使用注意](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#82-%E4%BD%BF%E7%94%A8%E6%B3%A8%E6%84%8F)
- [9. 数据转换对象(DTO)](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#9-%E6%95%B0%E6%8D%AE%E8%BD%AC%E6%8D%A2%E5%AF%B9%E8%B1%A1dto)
  - [9.1 映射配置](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#91-%E6%98%A0%E5%B0%84%E9%85%8D%E7%BD%AE)
  - [9.2 映射方式（一）](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#92-%E6%98%A0%E5%B0%84%E6%96%B9%E5%BC%8F%E4%B8%80)
  - [9.3 映射方式（二）](https://github.com/ganweisoft/WebPlugins/wiki/back%E2%80%90end%E2%80%90CN#93-%E6%98%A0%E5%B0%84%E6%96%B9%E5%BC%8F%E4%BA%8C)
