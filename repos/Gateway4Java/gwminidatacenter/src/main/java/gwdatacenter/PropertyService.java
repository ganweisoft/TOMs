package gwdatacenter;

import gwdatacenter.Interface.PropertyChangedEventHandler;
import gwdatacenter.args.PropertyChangedEventArgs;

import javax.xml.stream.XMLInputFactory;
import javax.xml.stream.XMLOutputFactory;
import javax.xml.stream.XMLStreamException;
import javax.xml.stream.XMLStreamWriter;
import javax.xml.transform.*;
import java.io.*;
import java.nio.charset.StandardCharsets;
import java.nio.file.*;
import java.util.concurrent.Semaphore;

public final class PropertyService
{
	private static String propertyFileName;
	private static String propertyXmlRootNodeName;

	private static String configDirectory;
	private static String dataDirectory;

	private static Properties properties;

	public static boolean getInitialized()
	{
		return properties != null;
	}
	private static Semaphore mutex = new Semaphore(1);

	public static void InitializeServiceForUnitTests()
	{
		properties = null;
		InitializeService(null, null, null);
	}

	public static void InitializeService(String configDirectory, String dataDirectory, String propertiesName)
	{
		if (properties != null)
		{
			throw new IllegalStateException("Service is already initialized.");
		}
		properties = new Properties();
		PropertyService.configDirectory = configDirectory;
		PropertyService.dataDirectory = dataDirectory;
		propertyXmlRootNodeName = propertiesName;
		propertyFileName = propertiesName + ".xml";
		properties.PropertyChanged.addListener("PropertiesPropertyChanged", (Object sender, PropertyChangedEventArgs e) -> PropertiesPropertyChanged(sender, e));
	}

	public static String getConfigDirectory()
	{
		return configDirectory;
	}

	public static String getDataDirectory()
	{
		return dataDirectory;
	}

	public static String Get(String property)
	{
		return properties.get(property);
	}

	public static <T> T Get(String property, T defaultValue)
	{
		return properties.Get(property, defaultValue);
	}

	public static <T> void Set(String property, T value)
	{
		properties.Set(property, value);
	}

	public static void Load()
	{
		if (properties == null)
		{
			throw new IllegalStateException("Service is not initialized.");
		}
		if (sharpSystem.StringHelper.isNullOrEmpty(configDirectory) || sharpSystem.StringHelper.isNullOrEmpty(propertyXmlRootNodeName))
		{
			throw new IllegalStateException("No file name was specified on service creation");
		}
		if (!(new File(configDirectory)).isDirectory())
		{
			(new File(configDirectory)).mkdirs();
		}

		if (!LoadPropertiesFromStream(Paths.get(configDirectory).resolve(propertyFileName).toString()))
		{
			LoadPropertiesFromStream(Path.of(getDataDirectory(), "options", propertyFileName).toString());
		}
	}

	public static boolean LoadPropertiesFromStream(String fileName)
	{
		if (!(new File(fileName)).isFile())
		{
			return false;
		}

		try {
			mutex.acquire();
			try (InputStream input = new FileInputStream(fileName))
			{
				XMLInputFactory factory = XMLInputFactory.newFactory();
				var reader =  factory.createXMLStreamReader(input);
				var rootName = reader.getLocalName();
				if(rootName == propertyXmlRootNodeName){
					properties.ReadProperties(reader, propertyXmlRootNodeName);
				}
			}
		} catch (IOException | InterruptedException | XMLStreamException e ) {
			DataCenter.WriteLogFile("Error loading properties: " + e.getMessage() + "\nSettings have been restored to default values.");
		}

		return false;
	}

	public static void Save()
	{
		if (sharpSystem.StringHelper.isNullOrEmpty(configDirectory) || sharpSystem.StringHelper.isNullOrEmpty(propertyXmlRootNodeName))
		{
			throw new IllegalStateException("No file name was specified on service creation");
		}

		try {
			TransformerFactory formerFactory = TransformerFactory.newInstance();
			Transformer transformer = formerFactory.newTransformer();
			// 换行
			transformer.setOutputProperty(OutputKeys.INDENT, "YES");
			// 文档字符编码
			transformer.setOutputProperty(OutputKeys.ENCODING, "utf-8");

			// 可随意指定文件的后缀,效果一样,但xml比较好解析,比如: E:\\person.txt等
			var fileName = Paths.get(configDirectory).resolve(propertyFileName);

			StringWriter stringOut = new StringWriter();
			XMLStreamWriter writer = XMLOutputFactory.newDefaultFactory().createXMLStreamWriter(stringOut);
			properties.WriteProperties(writer);
			var result = stringOut.toString();

			Files.writeString(fileName,
					result, StandardCharsets.UTF_8, StandardOpenOption.CREATE);

		} catch (TransformerException | XMLStreamException | IOException e) {
            throw new RuntimeException(e);
        }
    }

	private static void PropertiesPropertyChanged(Object sender, PropertyChangedEventArgs e)
	{
		if (PropertyChanged != null)
		{
			for (PropertyChangedEventHandler listener : PropertyChanged.listeners())
			{
				listener.invoke(null, e);
			}
		}
	}

	public static sharpSystem.Event<PropertyChangedEventHandler> PropertyChanged = new sharpSystem.Event<PropertyChangedEventHandler>();
}