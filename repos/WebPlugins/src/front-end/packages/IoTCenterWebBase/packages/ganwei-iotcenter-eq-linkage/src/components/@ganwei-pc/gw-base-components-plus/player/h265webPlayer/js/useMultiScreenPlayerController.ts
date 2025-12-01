import { inject } from 'vue'

import { AsyncPlayerSignalRController } from '../../signalr/AsyncPlayerSignalRController'
import { MultiScreenPlayerControllerKey } from '../../signalr/MultiScreenPlayerController'

export default function useMultiScreenPlayerController() {
    let multiScreenPlayerController = inject(MultiScreenPlayerControllerKey)
    if (multiScreenPlayerController === undefined) {
        multiScreenPlayerController = {
            addScreen(index, args) {
                return new AsyncPlayerSignalRController(...args)
            },
        }
    }

    return multiScreenPlayerController
}
