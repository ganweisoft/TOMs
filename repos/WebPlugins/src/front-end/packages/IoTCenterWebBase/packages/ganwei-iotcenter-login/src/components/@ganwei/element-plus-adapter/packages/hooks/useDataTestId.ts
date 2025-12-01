import { useAttrs } from 'vue'

const UI_AUTOMATION_TOKEN = `data-testid`

export default function () {
    const attrs = useAttrs()
    if (attrs[UI_AUTOMATION_TOKEN] === undefined || attrs[UI_AUTOMATION_TOKEN] === null || typeof attrs[UI_AUTOMATION_TOKEN] !== 'string') {
        console.warn(`${UI_AUTOMATION_TOKEN} is required to UI Automation`)
    }
}