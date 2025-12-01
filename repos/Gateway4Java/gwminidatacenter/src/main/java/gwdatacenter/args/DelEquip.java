package gwdatacenter.args;

import java.time.*;

public class DelEquip
{
	public int iStaNo;
	public int iEqpNo;
	public LocalDateTime DelTime = LocalDateTime.MIN;
}
/** 
 如果设备的local_addr 相同,则这些设备将归为一个SubEquipList 类，将使用同一个通讯线程通讯
 比如 使用COM4的所有设备将运行在一个线程
*/

