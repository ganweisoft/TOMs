import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";

const name = 'ElementPlusAdapter'

import path from "path";

export default defineConfig({
    build: {
        emptyOutDir: true,
        target: 'modules',
        outDir: path.resolve(__dirname, '../dist'),
        minify: false,
        sourcemap: true,
        cssCodeSplit: false,
        rollupOptions: {
            external: ["vue", "element-plus"],
            output: {
                assetFileNames: name + "[extname]",
                globals: {
                    vue: "Vue",
                    "element-plus": "ElementPlus",
                },
            }
        },
        lib: {
            entry: path.resolve(__dirname, "./index.ts"),
            name: "ElementPlusAdapter",
            formats: ["es", "cjs", "iife"],
            fileName: name
        },
    },
    plugins: [vue()]
})