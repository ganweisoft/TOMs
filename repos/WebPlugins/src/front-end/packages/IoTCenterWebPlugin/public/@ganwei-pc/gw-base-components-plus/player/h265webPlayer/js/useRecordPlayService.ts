import { inject, InjectionKey } from 'vue';

import { RecordPlayService } from './AsyncRecordPlaySignalRController';

export interface IRecordPlayAsyncOperation {
    recordStopPlay(streamId?: string): Promise<any>
    recordSpeedPlay(speed: number): Promise<any>
    recordPlayResume(): Promise<any>
    recordPlayPause(): Promise<any>
}

export const RecordPlayServiceKey: InjectionKey<RecordPlayService> = Symbol('RecordPlayServiceKey');

export const asyncOperationKey: InjectionKey<IRecordPlayAsyncOperation> = Symbol('asyncOperation');

export default function () {
    return inject(RecordPlayServiceKey);
}
