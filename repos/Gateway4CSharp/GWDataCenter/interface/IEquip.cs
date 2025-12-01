using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GWDataCenter
{
    public enum CommunicationState
    {
        /// <summary>
        /// 通讯失败
        /// </summary>
        fail,
        /// <summary>
        /// 通讯成功
        /// </summary>
        ok,
        /// <summary>
        /// 因为有设置而中断数据采集
        /// </summary>
        setreturn,
        /// <summary>
        /// 通讯重试中
        /// </summary>
        retry
    }

    public class EquipEvent
    {
        public string msg;//显示到实时快照的内容
        public string msg4Linkage;//联动传入的内容，如果为空就传入msg
        public MessageLevel level;
        public DateTime dt;
        public int iEquipNo;

        public EquipEvent(string Msg,MessageLevel Level,DateTime Dt)
        {
            msg = Msg;
            level = Level;
            dt = Dt;
        }
        public EquipEvent(string Msg, string Msg4Linkage, MessageLevel Level, DateTime Dt)
        {
            msg = Msg;
            msg4Linkage = Msg4Linkage;
            level = Level;
            dt = Dt;
        }
    }
    public interface IEquip
    {
        int m_sta_no
        {
            get;
            set;
        }
        int m_equip_no
        {
            get;
            set;
        }
        int m_retrytime//通讯失败重复次数
        {
            get;
            set;
        }
        //YC点
        Dictionary<int, object> YCResults
        {
            get;
        }
        //YX点
        Dictionary<int, object> YXResults
        {
            get;
        }
        //事件列表
        List<EquipEvent> EquipEventList
        {
            get;
        }

        bool RunSetParmFlag
        {
            get;
            set;
        }

        bool ResetFlag
        {
            get;
            set;
        }

        EquipItem equipitem
        {
            get;
            set;
        }

        bool bCanConfirm2NormalState
        {
            get;
            set;
        }

        string SetParmExecutor
        {
            get;
            set;
        }
        bool init(EquipItem eqpitm);
        CommunicationState GetData(CEquipBase p);
        bool SetParm(string cmd1,string cmd2,string value);

        bool Confirm2NormalState(string sYcYxType, int iYcYxNo);
        bool CloseCommunication();//用于释放通讯资源
    }
}
