export interface auth {
    add: number, edit: number, delete: number, view: number, export: number, import: number, print: number, other: number
}

export interface permission {
    packageId: string,
    pluginName: string,
    menuName: string,
    auth: string
}