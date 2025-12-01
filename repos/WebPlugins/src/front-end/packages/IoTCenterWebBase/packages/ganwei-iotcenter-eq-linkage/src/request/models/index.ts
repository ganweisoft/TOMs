export interface IPageRequest {
    pageNo: number
    pageSize: number
}

export interface IEquipLinkListRequest extends IPageRequest {
    equipName: string,
    iequipNos: number[];
    iycyxTypes: string;
    equipSetLists: Array<{ equipNo: number, setNos: number[] }>;
    // maxDelay: number;
    // minDelay: number;
}

export interface IEquipLinkListResponse {
    id: number;
    iequipNo: number;
    iequipNm: string;
    iycyxNo: number;
    ycYxName: string;
    iycyxType: string;
    delay: number;
    oequipNo: number;
    oequipNm: string;
    osetNo: number;
    setNm: string;
    setType: string;
    value: string;
    procDesc: string;
    enable: boolean;
    editEnable: boolean;
    isConditionLink: boolean;
    conditionRelation: number; // 0: AND 1: OR
}

export interface IEquipListResponse {
    iList: Array<{
        equipNm: string;
        equipNo: number
        equipType: string
    }>
    oList: Array<{
        equipNm: string;
        equipNo: number
        equipType: string
    }>
}

export interface IEquipCommandsRequest {
    equipList: number[]
}

export interface IEquipCommandsResponse {
    equipNo: number
    setParmList: Array<IEquipCommandResult>
}

export interface IEquipCommandResult {
    setNo: number;
    staN: number;
    equipNo: number;
    setNm: string;
    setType: string;
    mainInstruction: string;
    minorInstruction: string;
    record: boolean;
    action: string;
    value: string;
    canexecution: boolean;
    voiceKeys: string;
    enableVoice: boolean;
    qrEquipNo: number;
    reserve1: string;
    reserve2: string;
    reserve3: string;
    equip: any;
    setCode: string;
}

export interface IEquipNo {
    equipNo: number
}

export interface IEquipYcYxSetNumResponse {
    ycNum: number
    yxNum: number
    setNum: number
}

export interface IYcpYxpBaseData {
    dataType: null;
    valMax: number;
    valMin: number;
    curveRcd: boolean;
    code: string;
    staN: number;
    equipNo: number;
    procAdvice: string;
    relatedPic: string;
    relatedVideo: string;
    ziChanID: string;
    planNo: string;
    state: boolean;
    value: string;
    unit: string;
}

export interface IYcpResponse extends IYcpYxpBaseData {
    ycNm: string;
    ycNo: number;
}

export interface IYxpResponse extends IYcpYxpBaseData {
    yxNo: number;
    yxNm: string;
}
export interface ISetParamResponse {
    staN: number;
    equipNo: number;
    setNo: number;
    setNm: string;
    setType: string;
    value: string;
    mainInstruction: string;
    enableSetParm: boolean;
}

export interface IAddEquipLinkRequest {
    iequipNo: number;
    iycyxNo: number;
    iycyxType: string;
    delay: number;
    oequipNo: number;
    osetNo: number;
    value: string;
    procdesc: string;
}

export interface ISceneLinkRequest extends IPageRequest {
    searchName: string
}
export interface ISceneLinkResponse {
    rows: ISceneLinkData[]
    totalCount: number
}

export interface ISceneLinkData {
    staNo: number;
    equipNo: number;
    equipNm: string;
    setNo: number;
    setNm: string;
    setType: string;
    value: string;
    list: ISceneLinkSettingData[],
}
export interface ISceneLinkSettingData {
    id: string,
    sceneType: string;
    equipNo: string;
    equipNm: string;
    setNo: string;
    setNm: string;
    setType: string;
    value: string;
    timeValue: string;
}

export interface IEditSceneLinkRequest {
    setNm: string;
    setNo: number;
    equipNo: number;
    list: Array<IEditSceneLinkData>,
}
export type IEditSceneLinkData = {
    id: string;
    sceneType: 'E';
    equipNm: string;
    equipNo: string;
    setType: string;
    setNo: string;
    setNm: string;
    value: string;
} | {
    id: string;
    sceneType: 'T';
    timeValue: string;
}

export interface IDeleteSceneLinkRequest {
    equipno: number;
    setNo: number;
}

export interface IAddSceneLinkRequest {
    setNm: string;
    list: Array<IEditSceneLinkData>,
}

export interface ISGetConditionLinkByAutoProcIdResponse {
    id: number;
    procName: string;
    delay: number;
    iConditionItems: any;
    oequipNo: number;
    osetNo: number;
    value: string;
    procDesc: string;
    enable: boolean;
}
