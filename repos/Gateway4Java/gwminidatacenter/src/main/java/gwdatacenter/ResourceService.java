package gwdatacenter;

import org.w3c.dom.Document;
import org.xml.sax.SAXException;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import java.util.*;
import java.io.*;
import java.nio.file.*;

public final class ResourceService
{
	private static class ResourceItem
	{
		private String StringID;
		public final String getStringID()
		{
			return StringID;
		}
		public final void setStringID(String value)
		{
			StringID = value;
		}
		private String zh;
		public final String getZh()
		{
			return zh;
		}
		public final void setZh(String value)
		{
			zh = value;
		}
		private String ft;
		public final String getFt()
		{
			return ft;
		}
		public final void setFt(String value)
		{
			ft = value;
		}
		private String en;
		public final String getEn()
		{
			return en;
		}
		public final void setEn(String value)
		{
			en = value;
		}
		public final String GetString(String Language)
		{
			if (Language.equals("zh-CN") || Language.equals("zh"))
			{
				return getZh();
			}
			if (Language.equals("zh-HK") || Language.equals("ft"))
			{
				return getFt();
			}
			if (Language.equals("en-US") || Language.equals("en"))
			{
				return getEn();
			}

			return null;
		}
	}
	private static final String uiLanguageProperty = "CoreProperties.UILanguage";
	private static HashMap<String, ResourceItem> ResourceDict = new HashMap<String, ResourceItem>();
	public static String GetApplicationRootPath()
	{
		String applicationRootPath = null;
		try
		{
			String classPath = General.class.getProtectionDomain().getCodeSource().getLocation().getPath();
			File file = new File(classPath);
			applicationRootPath = file.getParentFile().getParent();
		}
		catch (RuntimeException e)
		{
		}
		return applicationRootPath;
	}

	public static String getLanguage()
	{
		return PropertyService.Get(uiLanguageProperty, Locale.getDefault().getLanguage());
	}
	public static void setLanguage(String value)
	{
		if (!getLanguage().equals(value))
		{
			PropertyService.Set(uiLanguageProperty, value);
		}
	}

	public static void InitializeService()
	{
		try {
			var fileName = Paths.get(GetApplicationRootPath()).resolve("bin/GWRES1.dll").toString();

			try (InputStream input = new FileInputStream(fileName))
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();
				Document doc = db.parse(input);
				String rootName = doc.getDocumentElement().getTagName();
				if(rootName == "NewDataSet"){
					var nodeList = doc.getChildNodes();
					for (int temp = 0; temp < nodeList.getLength(); temp++)
					{
						var oon = nodeList.item(temp);
						ResourceItem Item = new ResourceItem();
						Item.setStringID(oon.getUserData("StringID").toString());
						try
						{
							Item.setZh(oon.getUserData("zh-CN").toString());
						}
						catch (RuntimeException e)
						{
							Item.setZh(oon.getUserData("zh").toString());
						}
						try
						{
							Item.setFt(oon.getUserData("zh-HK").toString());
						}
						catch (RuntimeException e)
						{
							Item.setFt(oon.getUserData("ft").toString());
						}
						try
						{
							Item.setEn(oon.getUserData("en-US").toString());
						}
						catch (RuntimeException e)
						{
							Item.setEn(oon.getUserData("en").toString());
						}
						ResourceDict.put(DataCenter.DecodeBase64(Item.getStringID()).strip(), Item);
					}
				}
			}
		} catch (IOException | ParserConfigurationException | SAXException e ) {
			DataCenter.WriteLogFile("Error loading properties: " + e.getMessage() + "\nSettings have been restored to default values.");
		}
	}

	/** 
	 Returns a string from the resource database, it handles localization
	 transparent for the user.
	 
	 @return 
	 The string in the (localized) resource database.
	 
	 @param name
	 The name of the requested resource.
	 
	*/
	public static String GetString(String name)
	{
		String s = "";
		try
		{
			if (ResourceDict.containsKey(name.strip()))
			{
				s = DataCenter.DecodeBase64(ResourceDict.get(name).GetString(ResourceService.getLanguage()));
			}

		}
		catch (RuntimeException e)
		{
			s = name;
		}
		if (sharpSystem.StringHelper.isNullOrEmpty(s))
		{
			s = name;
		}
		return s;
	}

	public static String GetString(String name, String defaultStr)
	{
		String s = "";
		try
		{
			if (ResourceDict.containsKey(name.strip()))
			{
				s = DataCenter.DecodeBase64(ResourceDict.get(name).GetString(ResourceService.getLanguage()));
			}
			else
			{
				s = defaultStr;
			}
		}
		catch (RuntimeException e)
		{
			s = defaultStr;
		}
		if (sharpSystem.StringHelper.isNullOrEmpty(s))
		{
			s = defaultStr;
		}
		return s;

	}

}