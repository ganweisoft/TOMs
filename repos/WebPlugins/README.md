<p align="left" dir="auto">
  <a href="https://opensource.ganweicloud.com" rel="nofollow">
    <img width="130" height="130" src="src/logo.jpg">
  </a>
</p>

[![GitHub license](https://camo.githubusercontent.com/5eaf3ed8a7e8ccb15c21d967b8635ac79e8b1865da3a5ccf78d2572a3e10738a/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f646f746e65742f6173706e6574636f72653f636f6c6f723d253233306230267374796c653d666c61742d737175617265)](https://github.com/ganweisoft/WebPlugins/blob/main/LICENSE) [![Build Status](https://github.com/ganweisoft/TOMs/actions/workflows/build.yml/badge.svg)](https://github.com/ganweisoft/TOMs/actions) ![https://v2.vuejs.org/](https://img.shields.io/badge/Vue-3.5.13-%2394c20c?labelColor=#94c20c) ![https://www.webpackjs.com](https://img.shields.io/badge/vite-4.5.5-%234ec428?labelColor=#5a5a5a) ![https://www.axios-http.cn/docs/intro](https://img.shields.io/badge/Axios-1.7.9-%2397c424?labelColor=#5a5a5a) ![https://next.router.vuejs.org](https://img.shields.io/badge/vueRouter-4.5.0-%23d6604a?labelColor=#5a5a5a) ![https://element.eleme.io/#/zh-CN](https://img.shields.io/badge/ElementUI-2.9.1-%23097abb?labelColor=#5a5a5a) ![](https://img.shields.io/badge/join-discord-infomational)

English | [简体中文](README-CN.md)

WebPlugins is a modular and pluggable application framework based on ASP.NET Core and VUE. Built on the design principles of loose coupling and high cohesion, it provides an extensible and maintainable application framework. By completely decoupling core logic from functional components, it enables secondary development.

Here is the translated English version of your front-end development guide, maintaining the original structure and technical terminology:

# [Front‐End Development Guide](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide)
- [1. Environment Configuration & Project Initialization](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#1-environment-configuration--project-initialization)
  - [1.1 Development Environment Setup](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#1-install-using-nvm)
  - [1.2 Project Architecture Overview](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#2-project-structure-description)

- [2. Sub-Application Development Standards](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#3-sub-application-development)
  - [2.1 Template Engineering System](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#31-example-template-file-structure-device-linkage)
  - [2.2 Menu System Configuration](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#32-configuration-menu)
  - [2.3 Business Code Development Standards](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#33-code-development)
  - [2.4 Startup Process Explanation](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#34-startup-project)

- [3. Interface Theme System](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#35-theme-configuration)
  - [3.1 Theme Resource Referencing Mechanism](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#351-topic-citation)
  - [3.2 Dynamic Theme Switching Implementation](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#352-theme-switching)

- [4. Build & Deployment](https://github.com/ganweisoft/WebPlugins/wiki/Front%E2%80%90End-Development-Guide#4-build--deployment)

# [Backend Development Guide](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide)
- [1.1 Create a Plugin Template Solution](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#11-create-a-plugin-template-solution)
  - [1.1.1 Download the Template Engine](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#111-download-the-template-engine)
  - [1.1.2 Command Line to Create a Plugin Template Solution](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#112-command-line-to-create-a-plugin-template-solution)
  - [1.1.3 Visual Studio to Create a Plugin Template Solution](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#113-visual-studio-to-create-a-plugin-template-solution)

- [1.2 Explanation of the Plugin Template Solution](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#12-explination-of-the-plugin-template-solution)
  - [1.2.1 Solution Items Solution Folder](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#121-solution-items-solution-folder)
  - [1.2.2 docs Solution Folder](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#122-docs-solution-folder)
  - [1.2.3 build Solution Folder](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#123-build-solution-folder)
  - [1.2.4 Plugin Project](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#124-plugin-project)

- [1.3 Context Operation Instructions](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#13-context-operation-instructions)
  - [1.3.1 Model Mapping Norms](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#131-model-mapping-norms)
  - [1.3.2 Context Naming Conventions](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#132-context-naming-conventions)
  - [1.3.3 Basic Explanation of Context](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#133-basic-explanation-of-context)

- [1.4 Interface Definition and Invocation](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#14-interface-definition-and-invocation)
  - [1.4.1 Service Interface Definition](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#141-service-interface-definition)
  - [1.4.2 Service Interface Implementation](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#142-service-interface-implementation)
  - [1.4.3 Service Interface Registration](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#143-service-interface-registration)
  - [1.4.4 Controller Inheritance](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#144-controller-inheritance)
  - [1.4.5 Local Debugging Interface](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#145-local-debugging-interface)
  - [1.4.6 Interface Debugging Tools](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#146-interface-debugging-tools)

- [1.5 Log Management](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#15-log-management)
  - [1.5.1 Platform Log Output](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#151-platform-log-output)
  - [1.5.2 Plugin Log Output](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#152-plugin-log-output)

- [1.6 Common Helpful Extensions](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#16-common-helpful-extensions)
  - [1.6.1 String Extensions](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#161-string-extensions)
  - [1.6.2 Date Extensions](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#162-date-extensions)
  - [1.6.3 Serialization Extensions](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#163-serialization-extensions)
  - [1.6.4 Enumeration Extensions](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#164-enumeration-extensions)
  - [1.6.5 Key-Value Pair Objects](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#165-key-value-pair-objects)

- [1.7 Session Information](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#17-session-information)
- [1.8 Reading Configuration](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#18-reading-configuration)
- [1.9 Scheduled Tasks](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#19-scheduled-tasks)
  - [1.9.1 Example Usage](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#191-example-usage)
  - [1.9.2 Usage Notes](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#192-usage-notes)
- [1.10 Data Transfer Object (DTO)](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#110-data-transfer-object-dto)
  - [1.10.1 Mapping Configuration](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#1101-mapping-configuration)
  - [1.10.2 Mapping Method (One)](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#1102-mapping-method-one)
  - [1.10.3 Mapping Method (Two)](https://github.com/ganweisoft/WebPlugins/wiki/Backend-Development-Guide#1103-mapping-method-two)
