/* eslint-disable @typescript-eslint/no-non-null-assertion */
import type { ComputedRef, Ref } from 'vue'
import { onBeforeUnmount, onMounted, watchEffect } from 'vue'

export const isNumber = (val: any): val is number => typeof val === 'number'
export const isString = (val: any): val is string => typeof val === 'string'
export const isStringNumber = (val: string): boolean => {
    if (!isString(val)) {
        return false
    }
    return !Number.isNaN(Number(val))
}
export function addUnit(value?: string | number, defaultUnit = 'px') {
    if (value === undefined) return ''
    if (isNumber(value) || isStringNumber(value)) {
        return `${value}${defaultUnit}`
    } else if (isString(value)) {
        return value
    }
}

export const useDraggable = (
    targetRef: Ref<HTMLElement | undefined>,
    dragLeftRef: Ref<HTMLElement | undefined>,
    dragRightRef: Ref<HTMLElement | undefined>,
    topRef: Ref<HTMLElement | undefined>,
    draggable: ComputedRef<boolean>,
    onSelectChange: (context: { leftPercent: number, widthPercent: number }) => void
) => {
    const transform = {
        leftOffsetX: 0,
        rightOffsetX: 0,
    }
    const onMousedown = (reverse: boolean) => {
        return (e: MouseEvent) => {
            const downX = e.clientX
            const offsetX = transform[reverse ? 'rightOffsetX' : 'leftOffsetX']
            const dragRef = reverse ? dragRightRef : dragLeftRef

            const targetRect = targetRef.value!.getBoundingClientRect()

            const minLeft = 0
            const maxLeft = targetRect.width

            const onMousemove = (e: MouseEvent) => {
                let moveX = (e.clientX - downX)

                if (reverse) {
                    moveX = offsetX - moveX
                    moveX = Math.min(Math.max(moveX, minLeft), maxLeft)
                    transform['rightOffsetX'] = moveX

                    dragRef.value!.style.transform = `translate(${addUnit(
                        -transform['rightOffsetX']
                    )}, 0)`
                    if (maxLeft - transform['rightOffsetX'] < transform['leftOffsetX']) {
                        topRef.value!.style['left'] = `${addUnit(maxLeft - transform['rightOffsetX'])}`
                        topRef.value!.style['right'] = `${addUnit(maxLeft - transform['leftOffsetX'])}`
                    } else {
                        topRef.value!.style['right'] = `${addUnit(transform['rightOffsetX'])}`
                    }
                } else {
                    moveX = offsetX + moveX
                    moveX = Math.min(Math.max(moveX, minLeft), maxLeft)
                    transform['leftOffsetX'] = moveX
                    dragRef.value!.style.transform = `translate(${addUnit(
                        transform['leftOffsetX']
                    )}, 0)`
                    if (maxLeft - transform['leftOffsetX'] < transform['rightOffsetX']) {
                        topRef.value!.style['left'] = `${addUnit(maxLeft - transform['rightOffsetX'])}`
                        topRef.value!.style['right'] = `${addUnit(maxLeft - transform['leftOffsetX'])}`
                    } else {
                        topRef.value!.style['left'] = `${addUnit(transform['leftOffsetX'])}`
                    }
                }
            }

            const onMouseup = () => {
                onSelectChange({
                    leftPercent: transform.leftOffsetX / maxLeft * 100,
                    widthPercent: (maxLeft - transform.leftOffsetX - transform.rightOffsetX) / maxLeft * 100
                })
                document.removeEventListener('mousemove', onMousemove)
                document.removeEventListener('mouseup', onMouseup)
            }

            document.addEventListener('mousemove', onMousemove)
            document.addEventListener('mouseup', onMouseup)
        }
    }
    const leftHandleMousedown = onMousedown(false)
    const rightHandleMousedown = onMousedown(true)
    const topHandleMousedown = (e: MouseEvent) => {
        const downX = e.clientX
        const { leftOffsetX, rightOffsetX } = transform
        const targetRect = targetRef.value!.getBoundingClientRect()
        const minLeft = 0
        const maxLeft = targetRect.width
        const onMousemove = (e: MouseEvent) => {
            let leftMoveX = leftOffsetX + e.clientX - downX
            leftMoveX = Math.min(Math.max(leftMoveX, minLeft), maxLeft)
            let rightMoveX = rightOffsetX - (e.clientX - downX)
            rightMoveX = Math.min(Math.max(rightMoveX, minLeft), maxLeft)

            if (!leftMoveX || !rightMoveX) {
                return
            }

            transform['leftOffsetX'] = leftMoveX
            dragLeftRef.value!.style.transform = `translate(${addUnit(
                transform['leftOffsetX']
            )}, 0)`
            if (maxLeft - transform['leftOffsetX'] < transform['rightOffsetX']) {
                topRef.value!.style['left'] = `${addUnit(maxLeft - transform['rightOffsetX'])}`
                topRef.value!.style['right'] = `${addUnit(maxLeft - transform['leftOffsetX'])}`
            } else {
                topRef.value!.style['left'] = `${addUnit(transform['leftOffsetX'])}`
            }

            transform['rightOffsetX'] = rightMoveX
            dragRightRef.value!.style.transform = `translate(${addUnit(
                -transform['rightOffsetX']
            )}, 0)`
            if (maxLeft - transform['rightOffsetX'] < transform['leftOffsetX']) {
                topRef.value!.style['left'] = `${addUnit(maxLeft - transform['rightOffsetX'])}`
                topRef.value!.style['right'] = `${addUnit(maxLeft - transform['leftOffsetX'])}`
            } else {
                topRef.value!.style['right'] = `${addUnit(transform['rightOffsetX'])}`
            }
        }

        const onMouseup = () => {
            document.removeEventListener('mousemove', onMousemove)
            document.removeEventListener('mouseup', onMouseup)
        }
        document.addEventListener('mousemove', onMousemove)
        document.addEventListener('mouseup', onMouseup)
    }

    const onDraggable = () => {
        if (dragLeftRef.value && targetRef.value) {
            dragLeftRef.value.addEventListener('mousedown', leftHandleMousedown)
        }
        if (dragRightRef.value && targetRef.value) {
            dragRightRef.value.addEventListener('mousedown', rightHandleMousedown)
        }
        if (topRef.value && targetRef.value) {
            topRef.value.addEventListener('mousedown', topHandleMousedown)
        }
    }

    const offDraggable = () => {
        if (dragLeftRef.value && targetRef.value) {
            dragLeftRef.value.removeEventListener('mousedown', leftHandleMousedown)
        }
        if (dragRightRef.value && targetRef.value) {
            dragRightRef.value.removeEventListener('mousedown', rightHandleMousedown)
        }
    }

    function setLeftHandleOffsetX(offsetX: number, unit: 'px' | '%' = 'px') {
        if (unit === '%') {
            offsetX = targetRef.value!.getBoundingClientRect().width * offsetX / 100
        }
        transform.leftOffsetX = offsetX
        if (dragLeftRef.value) {
            dragLeftRef.value.style.transform = `translate(${addUnit(
                offsetX
            )}, 0)`
            topRef.value!.style['left'] = `${addUnit(offsetX)}`
        }
    }

    function setRightHandleOffsetX(offsetX: number, unit: 'px' | '%' = 'px') {
        if (unit === '%') {
            offsetX = targetRef.value!.getBoundingClientRect().width * offsetX / 100
        }
        transform.rightOffsetX = offsetX
        if (dragRightRef.value) {
            dragRightRef.value.style.transform = `translate(${addUnit(
                -offsetX
            )}, 0)`
            topRef.value!.style['right'] = `${addUnit(offsetX)}`
        }
    }

    onMounted(() => {
        watchEffect(() => {
            if (draggable.value) {
                onDraggable()
            } else {
                offDraggable()
            }
        })
    })

    onBeforeUnmount(() => {
        offDraggable()
    })

    return {
        setRightHandleOffsetX,
        setLeftHandleOffsetX
    }
}

export const useSelectDraggable = (
    containerRef: Ref<HTMLElement | undefined>,
    selectRef: Ref<HTMLElement | undefined>,
    onSelectChange: (context: { left: number, width: number, right: number, leftPercent: number, widthPercent: number, rightPercent: number }) => void
) => {
    const context = {
        left: 0,
        width: 0,
        right: 0
    }
    const onMousedown = (e: MouseEvent) => {
        const downX = e.clientX
        const targetRect = containerRef.value!.getBoundingClientRect()

        context.left = downX - targetRect.left
        selectRef.value!.style.left = `${context.left}px`

        const onMousemove = (e: MouseEvent) => {
            const moveX = (e.clientX - downX)
            context.width = Math.abs(moveX)
            context.right = targetRect.width - context.width - context.left
            selectRef.value!.style.width = `${context.width}px`
            if (moveX < 0) {
                context.left = downX - targetRect.left + moveX
                selectRef.value!.style.left = `${context.left}px`
            }
        }

        const onMouseup = () => {
            onSelectChange({
                ...context,
                leftPercent: context.left / targetRect.width * 100,
                rightPercent: context.left / targetRect.width * 100,
                widthPercent: context.width / targetRect.width * 100
            })
            selectRef.value!.style.width = '0px'
            document.removeEventListener('mousemove', onMousemove)
            document.removeEventListener('mouseup', onMouseup)
        }

        document.addEventListener('mousemove', onMousemove)
        document.addEventListener('mouseup', onMouseup)
    }

    const onDraggable = () => {
        if (selectRef.value && containerRef.value) {
            containerRef.value.addEventListener('mousedown', onMousedown)
        }
    }

    const offDraggable = () => {
        if (selectRef.value && containerRef.value) {
            containerRef.value.removeEventListener('mousedown', onMousedown)
        }
    }

    onMounted(() => {
        onDraggable()
    })

    onBeforeUnmount(() => {
        offDraggable()
    })
}

export const useHandleDraggable = (
    containerRef: Ref<HTMLElement | undefined>,
    dragRef: Ref<HTMLElement | undefined>,
    scale: number,
    onSelectChange: (context: { left: number, leftPercent: number }, type: string) => void,
) => {
    const { promise, resolve } = Promise.withResolvers()
    const context = {
        left: 0,
        leftPercent: 0,
        maxLeft: null
    }
    const onMousedown = (e: MouseEvent) => {
        const downX = e.clientX

        const targetRect = containerRef.value!.getBoundingClientRect()
        const minLeft = -targetRect.width
        const maxLeft = 0
        const offsetX = context.left

        const onMousemove = (e: MouseEvent) => {
            let moveX = (e.clientX - downX)
            moveX = offsetX + moveX
            moveX = Math.min(Math.max(moveX, minLeft * scale), maxLeft)
            context.left = moveX
            context.leftPercent = context.left / minLeft * 100
            dragRef.value!.style.transform = `translate(${addUnit(
                context['left']
            )}, 0)`

            onSelectChange({
                left: context.left,
                leftPercent: context.leftPercent,
            }, 'move')
        }

        const onMouseup = () => {
            onSelectChange({
                left: context.left,
                leftPercent: context.leftPercent,
            }, 'up')
            document.removeEventListener('mousemove', onMousemove)
            document.removeEventListener('mouseup', onMouseup)
        }

        document.addEventListener('mousemove', onMousemove)
        document.addEventListener('mouseup', onMouseup)
    }

    const onDraggable = () => {
        if (dragRef.value && containerRef.value) {
            dragRef.value.addEventListener('mousedown', onMousedown)
        }
    }

    const offDraggable = () => {
        if (dragRef.value && containerRef.value) {
            dragRef.value.removeEventListener('mousedown', onMousedown)
        }
    }

    const resetPosition = () => {
        context.left = 0;
        context.leftPercent = 0
        if (dragRef.value) {
            dragRef.value.style.transform = `translate(${addUnit(
                context['left']
            )}, 0)`
        }
    }

    async function setOffset(offsetX: number, unit: 'px' | '%' = 'px') {
        await promise
        if (unit === '%') {
            offsetX = -containerRef.value!.getBoundingClientRect().width * offsetX / 100
        }
        context.left = offsetX
        const minLeft = -containerRef.value!.getBoundingClientRect().width
        context.leftPercent = context.left / minLeft * 100
        if (dragRef.value) {
            dragRef.value.style.transform = `translate(${addUnit(
                context['left']
            )}, 0)`
        }
    }

    onMounted(() => {
        resolve(true)
        onDraggable()
    })

    onBeforeUnmount(() => {
        offDraggable()
    })

    return {
        resetPosition, setOffset
    }
}
