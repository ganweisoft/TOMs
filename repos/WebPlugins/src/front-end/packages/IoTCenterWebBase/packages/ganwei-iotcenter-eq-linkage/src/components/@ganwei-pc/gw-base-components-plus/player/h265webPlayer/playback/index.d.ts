export interface TimeRange {
    startTime: string,
    endTime: string
}
export interface ITimeBarProps {
    ranges: TimeRange[],
    date: Date
}
export interface IPlayBackPlayerProps {
    id: string,
    url: string
}
export interface IPlayBackProps extends IPlayBackPlayerProps, ITimeBarProps {

}
