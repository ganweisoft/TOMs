import { inject, InjectionKey } from 'vue'

import { AbstractPlayer } from '../../classDefintion/Player'

export interface IPlayerContext {
    player: AbstractPlayer | null,
    deviceId: number,
    nvrChannelId: number,
    ptzControl: boolean,
}

export const PlayerContextKey: InjectionKey<IPlayerContext> =
    Symbol('PlayerContext')

export default function () {
    return inject(PlayerContextKey)
}
