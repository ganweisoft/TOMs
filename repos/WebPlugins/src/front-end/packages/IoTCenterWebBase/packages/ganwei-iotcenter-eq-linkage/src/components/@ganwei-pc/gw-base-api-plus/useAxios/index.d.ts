import { Reactive, Ref } from 'vue';

import { AxiosResponse } from 'axios';

interface Response<T> {
    code: number;
    data?: T;
    message: string;
}

type RequestFunctionWithoutPage<T, R> = (data: T) => Promise<R>

export type ExcludeUndefined<T> = T extends undefined ? never : T

interface Pagination {
    pageNo: number;
    pageSize: number;
    total: number;
}

export interface useAxiosReturn<Data, Req> {

    /**
     * Indicates if the fetch request has finished
     */
    loading: Ref<boolean>;

    /**
     * The fetch response body on success, may either be JSON or text
     */
    data: Ref<ExcludeUndefined<Data>>;

    page: Reactive<Pagination>

    /**
     * Manually call the fetch
     * (default not throwing error)
     */
    execute: (params?: Req) => Promise<Data>;
}

export interface UseAxiosOptions<Data, Req, Res> {

    /**
     * Will automatically run fetch when `useFetch` is used
     *
     * @default true
     */
    immediate?: boolean;

    deep?: boolean;

    pageChangeAutoRequest?: boolean

    validate?: () => Promise<boolean>

    isPage?: boolean

    beforeEach?: (context: BeforeEachContext<Req>) => Promise<Req | undefined> | Req | undefined

    afterEach?: (res: Res, BeforeEachContext: BeforeEachContext<Req>) => Data

    setTotal?: (data: Res, BeforeEachContext: BeforeEachContext<Req>) => void;

    /**
     * Initial data before the request finished
     *
     * @default null
     */
    initialData?: any;

    customErrorCatch?: boolean,

    autoMessage?: boolean
}

export interface usePageAxiosOptions<Data, Req, Res> extends Omit<UseAxiosOptions<Data, Req, Res>, 'beforeEach'> {
    beforeEach?: () => Omit<Req, keyof Pagination>
}

export interface BeforeEachContext<Req> {
    page: Reactive<Pagination>,
    params?: Req
}
