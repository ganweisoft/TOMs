using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GWDataCenter
{
    /// <summary>
    /// 当有更新的时候，实现该接口，就不用重新启动服务
    /// </summary>
	interface ICanReset
	{
        bool ResetWhenDBChanged(params object[] o);
	}
}
