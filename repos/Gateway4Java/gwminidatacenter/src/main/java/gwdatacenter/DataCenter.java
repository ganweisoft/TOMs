package gwdatacenter;

import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.JsonNodeFactory;
import com.fasterxml.jackson.databind.node.ObjectNode;
import gwdatacenter.database.*;

import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.time.format.DateTimeFormatter;
import java.util.*;
import java.io.*;
import java.nio.file.*;
import java.time.*;

public final class DataCenter
{
	private static HashMap<Integer, EquipItem> equipItemDict = new HashMap<Integer, EquipItem>();
	public static Object brunning = false;

	private static Object bsibok = false;

	private static Object s_StationOnlyMark = "";
	private static boolean bEncrypt = false;

	public static Object myo = true;
	public static HashMap<Integer, EquipItem> getEquipItemDict()
	{
		synchronized (myo)
		{
			return equipItemDict;
		}
	}

	public static EquipItem GetEquipItem(int iEquipNo)
	{
		if (getEquipItemDict().containsKey(iEquipNo))
		{
			return getEquipItemDict().get(iEquipNo);
		}
		return null;
	}

	/** 
	 写入错误日志文件
	 
	 @param input 写入内容
	*/
	public static void WriteLogFile(String input)
	{
		System.out.println(input);
		try
		{
			String rootDir = Paths.get(General.GetApplicationRootPath()).resolve("log").toString();
			File RootPathName = new File(rootDir);
			if(!RootPathName.isDirectory()){
				RootPathName.mkdir();
			}

			//如果没有发现指定文件，则生成新文件
			String FileName = Paths.get(rootDir).resolve("XLog.txt").toString();
			var wholeFilePath = new File(FileName);
			if (!(wholeFilePath.isFile()))
			{
                wholeFilePath.createNewFile();
            }
			else if(wholeFilePath.length() > 1024 * 1024 * 5) //文件存在
			{
				wholeFilePath.deleteOnExit();
			}

			try (FileOutputStream filestream = new FileOutputStream(FileName, true)){
				/////////
				//写入时间
				String sss = "服务端";

				String strNow = String.format("*******************************************%1$s-%2$s(%3$s)*******************************************%4$s", LocalDateTime.now().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME), LocalDateTime.now().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME), sss, System.lineSeparator());
				byte[] bs = strNow.getBytes();
				filestream.write(bs, 0, bs.length);

				//sw.Write(strNow);

				bs = input.getBytes();
				filestream.write(bs, 0, bs.length);
				//sw.Write(input);

				bs = System.lineSeparator().getBytes();
				filestream.write(bs, 0, bs.length);
				//sw.WriteLine(Environment.NewLine);

				String fg = "**********************************************************************************************";
				bs = fg.getBytes();
				filestream.write(bs, 0, bs.length);
				//sw.WriteLine(fg);

				//末尾换行
				bs = System.lineSeparator().getBytes();
				filestream.write(bs, 0, bs.length);
			}
		}
		catch (RuntimeException | IOException e)
		{
			e.printStackTrace();
			System.out.println(e);
		}
    }


	/**  
	 Base64加密 
	  
	 @param encode 加密采用的编码方式
	 @param source 待加密的明文 
	 @return  
	*/
	public static String EncodeBase64(Charset encode, String source)
	{
		String result;
		try
		{
			byte[] bytes = source.getBytes(encode);
			result = Base64.getEncoder().encodeToString(bytes);
		}
		catch (Exception e)
		{
			result = source;
		}

		return result;
	}

	/**  
	 Base64加密，采用utf8编码方式加密 
	  
	 @param source 待加密的明文 
	 @return 加密后的字符串 
	*/
	public static String EncodeBase64(String source)
	{
		return EncodeBase64(StandardCharsets.UTF_8, source);
	}

	/**  
	 Base64解密 
	  
	 @param encode 解密采用的编码方式，注意和加密时采用的方式一致
	 @param result 待解密的密文 
	 @return 解密后的字符串 
	*/
	public static String DecodeBase64(Charset encode, String result)
	{
		String decode = "";
		try
		{
			byte[] bytes = Base64.getDecoder().decode(result);
			decode = new String(bytes, encode);
		}
		catch (Exception e)
		{
			decode = result;
		}
		return decode;
	}

	/**  
	 Base64解密，采用utf8编码方式解密 
	  
	 @param result 待解密的密文 
	 @return 解密后的字符串 
	*/
	public static String DecodeBase64(String result)
	{
		return DecodeBase64(StandardCharsets.UTF_8, result);
	}

	public static void Start()
	{
		synchronized (brunning)
		{
			if (!(Boolean)brunning)
			{
				try
				{
					if (!StationItem.init())
					{
						new Scanner(System.in).nextLine();
						return;
					}
					for (Map.Entry<String, Object> entry : StationItem.EquipCategoryDict.entrySet())
					{
						//启动数据获取线程和设置扫描线程 
						SubEquipList EquipList = (SubEquipList)entry.getValue();
						EquipList.OldEquip = null;

						EquipList.StartRefreshThread();
						EquipList.StartSetParmThread();
					}

					brunning = true;
					System.out.println(LocalDateTime.now().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME) + ResourceService.GetString("AlarmCenter.DataCenter.Msg22", "程序运行中…"));

				}

				catch (RuntimeException e)
				{
					DataCenter.WriteLogFile(e.toString());
				}
			}
		}
	}

//C# TO JAVA CONVERTER TODO TASK: There is no preprocessor in Java:
		///#region 读写系统的配置文件
	/** 
	 读取系统的配置文件
	 
	 @param PropertyName 配置文件中的属性名称
	 @param NodeName 属性下面的节点名称
	 @param DefaultValue 如果没有配置该属性，则返回默认值
	 @return 
	*/
	public static String GetPropertyFromPropertyService(String PropertyName, String NodeName, String DefaultValue)
	{
		String strValue = "";
		try
		{
			if (sharpSystem.StringHelper.isNullOrEmpty(NodeName))
			{
				strValue = PropertyService.Get(PropertyName, DefaultValue);
				return strValue;
			}

			Properties properties = PropertyService.Get(PropertyName, new Properties());
			if (properties.Contains(NodeName))
			{
				strValue = properties.Get(NodeName, DefaultValue);
			}
			else
			{
				strValue = DefaultValue;
				properties.Set(NodeName, DefaultValue);
				PropertyService.Save();
			}
			return strValue;
		}
		catch (RuntimeException e)
		{
			DataCenter.WriteLogFile(e.toString());
		}
		return strValue;
	}
	/** 
	 修改系统的配置文件
	 
	 @param PropertyName 配置文件中的属性名称
	 @param NodeName 属性下面的节点名称
	 @param Value 属性或者节点的值
	 @return 
	*/
	public static void SetPropertyToPropertyService(String PropertyName, String NodeName, String Value)
	{
		try
		{
			if (sharpSystem.StringHelper.isNullOrEmpty(NodeName))
			{
				PropertyService.Set(PropertyName, Value);
				return;
			}

			Properties properties = PropertyService.Get(PropertyName, new Properties());
			if (properties.Contains(NodeName))
			{
				properties.Set(NodeName, Value);
				PropertyService.Save();
			}
		}
		catch (RuntimeException e)
		{
			DataCenter.WriteLogFile(e.toString());
		}
	}

	/** 
	 读取扩展字段里面的配置参数
	 
	 @param strValue 读到的参数值
	 @param TableName 表名称：目前支持equip、ycp、yxp、setparm这四个表
	 @param ReserveName 字段名称，仅支持Reserve1、Reserve2、Reserve3这三个字段名称
	 @param iEquipNo 设备号
	 @param iChedianNo 测点号，无需填入时就填0，比如读取equip表时
	 @param NodeName json的键名，支持多层深度，每个深度之间用#隔开，比如"GWDataCenter.dll#EnableEquip"
	 @return 读取成功返回true，否则为false
	*/
	public static boolean GetPropertyFromReserve(sharpSystem.OutObject<String> strValue, String TableName, String ReserveName, int iEquipNo, int iChedianNo, String NodeName)
	{
		String strReserve = "";
		strValue.outArgValue = "";
		try
		{
			switch (TableName.toLowerCase())
			{
				case "equip":
				{
						//EquipTableRow R = (from r in StationItem.getDbEqp() where r.equip_no == iEquipNo select r).FirstOrDefault();
						EquipTableRow R = StationItem.getDbEqp().stream().filter(r -> r.getEquipNo() == iEquipNo).findFirst().orElse(null);;
						if (R == null)
						{
							DataCenter.WriteLogFile(String.format("GetPropertyFromReserve 找不到设备号为%1$s的EquipTableRow！", iEquipNo));
							return false;
						}
						if (ReserveName.toLowerCase().equals("reserve1"))
						{
							strReserve = R.getReserve1();
						}
						else if (ReserveName.toLowerCase().equals("reserve2"))
						{
							strReserve = R.getReserve2();
						}
						else if (ReserveName.toLowerCase().equals("reserve3"))
						{
							strReserve = R.getReserve3();
						}
						break;
				}
				case "ycp":
				{
						//YcpTableRow R = (from r in StationItem.getDbYcp() where r.equip_no == iEquipNo && r.yc_no == iChedianNo select r).FirstOrDefault();
						YcpTableRow R = StationItem.getDbYcp().stream().filter(r -> r.getEquipNo() == iEquipNo && r.getYcNo() == iChedianNo).findFirst().orElse(null);
						if (R == null)
						{
							DataCenter.WriteLogFile(String.format("GetPropertyFromReserve 找不到设备号为%1$s测点号为%2$s的YcpTableRow！", iEquipNo, iChedianNo));
							return false;
						}
						if (ReserveName.toLowerCase().equals("reserve1"))
						{
							strReserve = R.getReserve1();
						}
						else if (ReserveName.toLowerCase().equals("reserve2"))
						{
							strReserve = R.getReserve2();
						}
						else if (ReserveName.toLowerCase().equals("reserve3"))
						{
							strReserve = R.getReserve3();
						}
						break;
				}
				case "yxp":
				{
						//YxpTableRow R = (from r in StationItem.getDbYxp() where r.equip_no == iEquipNo && r.yx_no == iChedianNo select r).FirstOrDefault();
						YxpTableRow R = StationItem.getDbYxp().stream().filter(r -> r.getEquipNo() == iEquipNo && r.getYxNo() == iChedianNo).findFirst().orElse(null);
						if (R == null)
						{
							DataCenter.WriteLogFile(String.format("GetPropertyFromReserve 找不到设备号为%1$s测点号为%2$s的YxpTableRow！", iEquipNo, iChedianNo));
							return false;
						}
						if (ReserveName.toLowerCase().equals("reserve1"))
						{
							strReserve = R.getReserve1();
						}
						else if (ReserveName.toLowerCase().equals("reserve2"))
						{
							strReserve = R.getReserve2();
						}
						else if (ReserveName.toLowerCase().equals("reserve3"))
						{
							strReserve = R.getReserve3();
						}
						break;
				}
				case "setparm":
				{
						//SetParmTableRow R = (from r in StationItem.getDbSetparm() where r.equip_no == iEquipNo && r.set_no == iChedianNo select r).FirstOrDefault();
						SetParmTableRow R = StationItem.getDbSetparm().stream().filter(r -> r.getEquipNo() == iEquipNo && r.getSetNo() == iChedianNo).findFirst().orElse(null);
						if (R == null)
						{
							DataCenter.WriteLogFile(String.format("GetPropertyFromReserve 找不到设备号为%1$s设置号为%2$s的SetParmTableRow！", iEquipNo, iChedianNo));
							return false;
						}
						if (ReserveName.toLowerCase().equals("reserve1"))
						{
							strReserve = R.getReserve1();
						}
						else if (ReserveName.toLowerCase().equals("reserve2"))
						{
							strReserve = R.getReserve2();
						}
						else if (ReserveName.toLowerCase().equals("reserve3"))
						{
							strReserve = R.getReserve3();
						}
						break;
				}
				default:
				{
						DataCenter.WriteLogFile(String.format("GetPropertyFromReserve 的参数%1$s不符合预期！", TableName));
						return false;
				}
			}
			//空字符串直接返回
			if (sharpSystem.StringHelper.isNullOrEmpty(strReserve) || sharpSystem.StringHelper.isNullOrWhiteSpace(strReserve))
			{
				return false;
			}

			// 解析 JSON 字符串
			ObjectMapper mapper = new ObjectMapper();
			JsonNode currentElement = mapper.readTree(strReserve);

			for (var segment : NodeName.split(java.util.regex.Pattern.quote("#"), -1))
			{
				JsonNode nextElement;
				if (currentElement.isObject() && currentElement.has(segment))
				{
					nextElement = currentElement.get(segment);
					currentElement = nextElement;
				}
				else
				{
					DataCenter.WriteLogFile(String.format("GetPropertyFromReserve 的参数%1$s中的节点%2$s没有找到！", NodeName, segment));
					return false;
				}
			}
			strValue.outArgValue = currentElement.toString();
			return true;
		}
		catch (RuntimeException e)
		{
			DataCenter.WriteLogFile(String.format("GetPropertyFromReserve 方法出错,TableName：%1$s,ReserveName：%2$s,iEquipNo：%3$s,iChedianNo：%4$s,NodeName：%5$s,异常内容：%6$s", TableName, ReserveName, iEquipNo, iChedianNo, NodeName, e.toString()));
		} catch (JsonMappingException e) {

        } catch (JsonProcessingException e) {

        }
        return false;
	}

	public static boolean GetPropertyFromReserveWithEquipTableRow(sharpSystem.OutObject<String> strValue, String ReserveName, String NodeName, EquipTableRow R)
	{
		String strReserve = "";
		strValue.outArgValue = "";
		try
		{
			if (R == null)
			{
				return false;
			}
			if (ReserveName.toLowerCase().equals("reserve1"))
			{
				strReserve = R.getReserve1();
			}
			else if (ReserveName.toLowerCase().equals("reserve2"))
			{
				strReserve = R.getReserve2();
			}
			else if (ReserveName.toLowerCase().equals("reserve3"))
			{
				strReserve = R.getReserve3();
			}


			//空字符串直接返回
			if (sharpSystem.StringHelper.isNullOrEmpty(strReserve) || sharpSystem.StringHelper.isNullOrWhiteSpace(strReserve))
			{
				return false;
			}
			// 解析 JSON 字符串
			ObjectMapper mapper = new ObjectMapper();
			JsonNode currentElement = mapper.readTree(strReserve);

			for (var segment : NodeName.split(java.util.regex.Pattern.quote("#"), -1))
			{
				JsonNode nextElement;
				if (currentElement.isObject() && currentElement.has(segment))
				{
					nextElement = currentElement.get(segment);
					currentElement = nextElement;
				}
				else
				{
					DataCenter.WriteLogFile(String.format("GetPropertyFromReserve 的参数%1$s中的节点%2$s没有找到！", NodeName, segment));
					return false;
				}
			}
			strValue.outArgValue = currentElement.toString();
			return true;
		}
		catch (RuntimeException e)
		{
			DataCenter.WriteLogFile(String.format("GetPropertyFromReserveWithEquipTableRow 方法出错,ReserveName：%1$s,iEquipNo：%2$s,NodeName：%3$s,异常内容：%4$s", ReserveName, R.getEquipNo(), NodeName, e.toString()));
		} catch (JsonProcessingException e) {
            throw new RuntimeException(e);
        }
        return false;
	}


	private static void CheckAndCreateNode(JsonNode node, String[] keys, String value)
	{
		CheckAndCreateNode(node, keys, value, 0);
	}

//C# TO JAVA CONVERTER NOTE: Java does not support optional parameters. Overloaded method(s) are created above:
//ORIGINAL LINE: static void CheckAndCreateNode(JsonObject node, string[] keys, string value, int index = 0)
	private static void CheckAndCreateNode(JsonNode node, String[] keys, String value, int index)
	{
		// 如果索引超过键的长度，结束递归
		if (index >= keys.length)
		{
			return;
		}

		// 检查当前节点是否包含键
		if (!node.has(keys[index]))
		{
			// 如果是最后一个键，添加键值对并打印消息
			if (index == keys.length - 1)
			{
				((ObjectNode)node).put(keys[index], value);
			}
			else
			{
				// 如果不是最后一个键，创建子节点并递归
				ObjectNode child = new ObjectNode(JsonNodeFactory.instance);
				((ObjectNode)node).set(keys[index], child);
				CheckAndCreateNode(child, keys, value, index + 1);
			}
		}
		else
		{
			// 如果是最后一个键，更新值
			if (index == keys.length - 1)
			{
				((ObjectNode)node).put(keys[index], value);
			}
			else
			{
				// 如果不是最后一个键，递归到下一个键
				CheckAndCreateNode(node.get(keys[index]), keys, value, index + 1);
			}
		}
	}

}