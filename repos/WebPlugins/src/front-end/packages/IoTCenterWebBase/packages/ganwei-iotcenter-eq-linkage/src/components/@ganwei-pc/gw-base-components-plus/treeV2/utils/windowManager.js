export class WindowManager {
    constructor() {
        this.window = window;
        try {
            window.top.equipGroup
            this.window = window.top
        } catch (error) {
            this.window = window
        }
    }
}

export const mixinWindow = {
    data () {
        return {
            window: null
        }
    },
    created () {
        this.window = window

        try {
            window.top.equipGroup
            this.window = window.top
        } catch (error) {
            this.window = window
        }
    }
}
