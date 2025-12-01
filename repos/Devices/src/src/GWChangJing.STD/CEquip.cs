// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using System.Collections.Concurrent;

namespace GWChangJing.STD
{
    /// <summary>
    /// 设备管理类，负责处理设备参数设置和数据刷新
    /// </summary>
    public class CEquip : CEquipBase, IDisposable
    {
        private readonly ConcurrentQueue<SetItem> _setItemQueue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Task _refreshTask;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private const int ThreadInterval = 100;
        private bool _disposed = false;

        public CEquip()
        {
            // 使用Task.Run代替Thread，提供更好的线程管理
            _refreshTask = Task.Run(RefreshAsync, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// 添加设置项到队列
        /// </summary>
        /// <param name="item">要添加的设置项</param>
        /// <exception cref="ArgumentNullException">当item为null时抛出</exception>
        /// <exception cref="ObjectDisposedException">当对象已释放时抛出</exception>
        public void AddSetItem(SetItem item)
        {
            ThrowIfDisposed();

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // ConcurrentQueue是线程安全的，不需要额外的锁
            // 使用Contains检查可能很昂贵，考虑是否真的需要去重
            _setItemQueue.Enqueue(item);
        }

        /// <summary>
        /// 从队列中获取设置项
        /// </summary>
        /// <returns>设置项，如果队列为空则返回null</returns>
        /// <exception cref="ObjectDisposedException">当对象已释放时抛出</exception>
        public SetItem GetSetItem()
        {
            ThrowIfDisposed();

            return _setItemQueue.TryDequeue(out SetItem item) ? item : null;
        }

        /// <summary>
        /// 获取设备数据
        /// </summary>
        /// <param name="pEquip">设备对象</param>
        /// <returns>通信状态</returns>
        public override CommunicationState GetData(CEquipBase pEquip)
        {
            ThrowIfDisposed();
            return base.GetData(pEquip);
        }

        /// <summary>
        /// 设置设备参数
        /// </summary>
        /// <param name="mainInstruct">主指令</param>
        /// <param name="minorInstruct">次指令</param>
        /// <param name="value">参数值</param>
        /// <returns>是否成功添加到队列</returns>
        /// <exception cref="ArgumentException">当参数为空或null时抛出</exception>
        /// <exception cref="ObjectDisposedException">当对象已释放时抛出</exception>
        public override bool SetParm(string mainInstruct, string minorInstruct, string value)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(mainInstruct))
                throw new ArgumentException("主指令不能为空", nameof(mainInstruct));

            if (string.IsNullOrWhiteSpace(minorInstruct))
                throw new ArgumentException("次指令不能为空", nameof(minorInstruct));

            try
            {
                var item = new SetItem(
                    equipitem.iEquipno,
                    mainInstruct,
                    minorInstruct,
                    value,
                    SetParmExecutor,
                    false
                );

                AddSetItem(item);
                return true;
            }
            catch (Exception)
            {
                // 记录日志或处理异常
                return false;
            }
        }

        /// <summary>
        /// 异步刷新处理方法
        /// </summary>
        private async Task RefreshAsync()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var item = GetSetItem();

                    if (item != null)
                    {
                        // 使用Task.Run异步处理，避免阻塞主循环
                        _ = Task.Run(() => ProcessSetItem(item), _cancellationTokenSource.Token);
                    }

                    // 使用异步延迟，允许其他任务运行
                    await Task.Delay(ThreadInterval, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消，不需要处理
            }
            catch (Exception ex)
            {
                // 记录异常日志
                Console.WriteLine($"RefreshAsync异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理设置项
        /// </summary>
        /// <param name="item">要处理的设置项</param>
        private void ProcessSetItem(SetItem item)
        {
            try
            {
                // 使用信号量限制并发处理的数量
                _semaphore.Wait(_cancellationTokenSource.Token);

                try
                {
                    //TODO
                    //GWDataCenter.DataCenter.DoCJ(item);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消，不需要处理
            }
            catch (Exception ex)
            {
                // 记录异常日志
                Console.WriteLine($"ProcessSetItem异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查对象是否已释放
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CEquip));
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源的实现
        /// </summary>
        /// <param name="disposing">是否正在释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _cancellationTokenSource?.Cancel();

                try
                {
                    _refreshTask?.Wait(TimeSpan.FromSeconds(5));
                }
                catch (AggregateException)
                {
                    // 超时或任务异常，继续清理
                }

                _cancellationTokenSource?.Dispose();
                _semaphore?.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~CEquip()
        {
            Dispose(false);
        }
    }
}