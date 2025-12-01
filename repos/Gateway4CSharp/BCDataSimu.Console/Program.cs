// See https://aka.ms/new-console-template for more information

CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
ManualResetEvent shutdownEvent = new ManualResetEvent(false);
var cancellationToken = cancellationTokenSource.Token;

// 注册 Ctrl+C 事件处理
Console.CancelKeyPress += (sender, eventArgs) =>
{
    eventArgs.Cancel = true; // 防止默认的终止行为
    cancellationTokenSource.Cancel(); // 触发取消令牌
};

Console.WriteLine("程序已启动，按 Ctrl+C 来优雅关闭...");

// 启动一个后台线程模拟程序运行中的工作
Thread workerThread = new Thread(WorkerMethod);
workerThread.IsBackground = true;
workerThread.Start(cancellationToken);

// 等待关闭事件或取消令牌
WaitHandle.WaitAny(new[] { shutdownEvent, cancellationToken.WaitHandle });

// 执行清理操作
GracefulShutdown();

static void WorkerMethod(object? o)
{
    try
    {
        var cancellationToken = (CancellationToken)(o ?? throw new ArgumentException(nameof(o)));
        
        while (!cancellationToken.IsCancellationRequested)
        {
            GWDataCenter.DataCenter.Start();
        }
    }
    catch (ThreadInterruptedException ex)
    {
        // 捕获线程中断异常
    }
    catch (Exception ex)
    {
        // 捕获线程中断异常
    }
}

static void GracefulShutdown()
{
    Console.WriteLine("检测到 Ctrl+C，开始优雅关闭程序...");
    // 在这里执行清理操作，比如关闭文件、释放资源等
    Thread.Sleep(2000); // 模拟清理操作延迟
    Console.WriteLine("清理操作已完成。");
    Console.WriteLine("程序已关闭。");
    Environment.Exit(0); // 优雅退出程序
}