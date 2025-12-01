package gwdatacenter;

import java.io.*;
import java.nio.file.*;
import java.time.*;
import java.util.Arrays;
import java.util.Collections;
import java.util.regex.Pattern;

public final class General
{

	public static String CfgValue1 = "MicrosoftWPF4.0"; //密钥
	public static String CfgValue2 = "MicrosoftWCF4.0"; //初始向量

	public static LocalDateTime Convert2DT(LocalDateTime DT)
	{
		var dt = LocalDateTime.of(DT.getYear(), DT.getMonthValue(), DT.getDayOfMonth(), DT.getHour(), DT.getMinute(), DT.getSecond());
		return dt;
	}

	/** 
	 正则表达式验证 字符串是否符合预期格式
	 
	 @param s 正则表达式
	 @param s1 目标字符串
	 @return 
	*/

	public static boolean VerifyStringFormat(String s, String s1)
	{
		if (Pattern.matches(s, s1))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/** 
	 获取抛出异常的相关信息
	 
	 @param e
	 @return 
	*/
	public static String GetExceptionInfo(RuntimeException e)
	{
		String msg;
		msg = e.getMessage() + e.getCause() + e.getStackTrace();
		/*
		msg = msg.Replace('\'', '*');
		msg = msg.Replace('\"', '*');
		if (msg.Length > 250)
		{
		    msg = msg.Substring(0, 250);
		    msg +=  "...";
		}
		 */
		return msg;
	}

	public static String GetApplicationRootPath()
	{
		String applicationRootPath = null;
		try {
			String classPath = General.class.getProtectionDomain().getCodeSource().getLocation().getPath();
			File file = new File(classPath);
			applicationRootPath = file.getParentFile().getParent();
		} catch (Exception e) {
			// 忽略异常
		}
		return applicationRootPath;
	}

	public static String GetExecutingAssemblyFileName()
	{
		try {
			String classPath = General.class.getProtectionDomain().getCodeSource().getLocation().getPath();
			File file = new File(classPath);
			return file.getName();
		} catch (Exception e) {
			return "";
		}
	}

	public static int GetDayOfWeek(LocalDateTime t)
	{
		switch (t.getDayOfWeek())
		{
			case DayOfWeek.SUNDAY:
				return 1;
			case DayOfWeek.MONDAY:
				return 2;
			case DayOfWeek.TUESDAY:
				return 3;
			case DayOfWeek.WEDNESDAY:
				return 4;
			case DayOfWeek.THURSDAY:
				return 5;
			case DayOfWeek.FRIDAY:
				return 6;
			case DayOfWeek.SATURDAY:
				return 7;
			default:
				return 1;
		}
	}


	public static String GetString1(String MacAddr)
	{
		//网卡序列号
		String strID = "AlarmCenter1";

		strID += MacAddr;
		strID = strID.replace(':', '8');
		strID = strID.replace(' ', 'F');

		char[] charArray = strID.toCharArray();
		Collections.reverse(Arrays.asList(charArray));
		strID = (new String(charArray)).substring(0, 8);
		return strID;
	}

}