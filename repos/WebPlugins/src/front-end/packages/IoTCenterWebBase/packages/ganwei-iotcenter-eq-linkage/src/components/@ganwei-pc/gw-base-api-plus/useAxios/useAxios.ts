
import { reactive, ref, shallowRef, watch } from 'vue';
import { AxiosResponse } from 'axios';

import { PageResponse } from '@components/@ganwei-pc/gw-base-api-plus/apiFunction';
import { useMessage } from '@components/@ganwei-pc/gw-base-utils-plus/notification';

import { BeforeEachContext, UseAxiosOptions, useAxiosReturn, usePageAxiosOptions } from '.';

interface Response<T> {
    code: number;
    data?: T;
    message: string;
}

type RequestFunctionWithoutPage<T, R> = (data: T) => Promise<R>

type ExcludeUndefined<T> = T extends undefined ? never : T

/**
 *
 *
 * @export
 * @template Data data.value 根据afterEach返回值确定，默认为`R`
 * @template Req 接口请求参数
 * @template Res 接口返回响应
 * @param {RequestFunctionWithoutPage<Req, Res>} request
 * @param {UseAxiosOptions<Data, Req, Res>} [useAxiosOptions={}]
 * @returns {useAxiosReturn<Data, Req, Res>}
 */
export function useAxios<Req, Res extends AxiosResponse<Response<any>, any>, Data = ExcludeUndefined<Res>>(request: RequestFunctionWithoutPage<Req, Res>, useAxiosOptions: UseAxiosOptions<Data, Req, Res> = {}): useAxiosReturn<Data, Req> {
    const $message = useMessage()
    const options: UseAxiosOptions<Data, Req, Res> = {
        immediate: false,
        isPage: false,
        pageChangeAutoRequest: false,
        ...useAxiosOptions,
    }

    // @ts-ignore
    options.__proto__ = useAxios.globalOptions

    const page = reactive({
        pageNo: 1,
        pageSize: 20,
        total: 0
    })

    const isFinished = ref(false)
    const isFetching = ref(false)
    const data = options.deep ? ref<Data>(options.initialData) : shallowRef<Data>(options.initialData)

    const loading = (isLoading: boolean) => {
        isFetching.value = isLoading
        isFinished.value = !isLoading
    }

    const execute = async (params?: Req) => {
        loading(true)

        if (options.validate) {
            let validate = true;
            try {
                validate = await options.validate();
            } catch (err) {
                validate = false
            }
            if (!validate) {
                loading(false)
                return Promise.reject('validate Failed')
            }
        }

        const context: BeforeEachContext<Req> = {
            page,
            params
        }
        const beforeEach = options.beforeEach && (await options.beforeEach(context)) || {} as Req
        context.params = { ...(params || {}), ...beforeEach }

        const promise = request(context.params).then(async res => {
            if (options.afterEach) {
                data.value = await options.afterEach(res, context)
            } else {
                data.value = res
            }
            if (res.data && res.data.code !== 200) {
                return Promise.reject(res)
            }
            if (options.setTotal) {
                options.setTotal(res, context)
            }
            if (options.autoMessage) $message.success(res.data.message)
            return data.value
        }).finally(() => {
            loading(false)
        })
        if (options.customErrorCatch) {
            return promise
        }

        return promise.catch((err) => {
            $message.error(err.data, err);
            return Promise.reject(err)
        })
    }

    if (options.immediate) {
        execute()
    }

    if (options.pageChangeAutoRequest && options.isPage) {
        watch([() => page.pageNo, () => page.pageSize], () => {
            if (!isFetching.value) {
                execute()
            }
        })
    }

    return {
        data,
        loading: isFetching,
        execute,
        page,
    } as useAxiosReturn<Data, Req>
}

useAxios.globalOptions = {
    autoMessage: false
}

export function usePageAxios<Req, Res extends AxiosResponse<Response<PageResponse<any>>, any>, Data = ExcludeUndefined<Res>>(request: RequestFunctionWithoutPage<Req, Res>, useAxiosOptions: usePageAxiosOptions<Data, Req, Res> = {}): useAxiosReturn<Data, Req> {
    return useAxios<Req, Res, Data>(request, {
        isPage: true,
        pageChangeAutoRequest: true,
        ...useAxiosOptions,
        beforeEach({ page }) {
            return {
                pageNo: page.pageNo,
                pageSize: page.pageSize,
                ...(useAxiosOptions.beforeEach?.() || {})
            } as Req
        },
        setTotal(res, { page }) {
            page.total = res.data.data?.total || 0
        }
    })
}
