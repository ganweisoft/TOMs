package gwdatacenter;

import gwdatacenter.Interface.PropertyChangedEventHandler;
import gwdatacenter.args.PropertyChangedEventArgs;

import javax.xml.stream.*;
import javax.xml.transform.*;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardOpenOption;
import java.util.*;
import java.io.*;

public class Properties
{
	/**  Needed for support of late deserialization 
	*/
	private static class SerializedValue
	{
		private String content;

		public final String getContent()
		{
			return content;
		}

		public final <T> T Deserialize(Class<T> valueType)
		{
			return null;
		}

		public SerializedValue(String content)
		{
			this.content = content;
		}
	}

	private HashMap<String, Object> properties = new HashMap<String, Object>();

	public final String get(String property)
	{
		return String.valueOf(Get(property));
	}
	public final void set(String property, String value)
	{
		Set(property, value);
	}

	public final String[] getElements()
	{
		synchronized (properties)
		{
			ArrayList<String> ret = new ArrayList<String>();
			for (Map.Entry<String, Object> property : properties.entrySet())
			{
				ret.add(property.getKey());
			}
			return ret.toArray(new String[0]);
		}
	}

	public final Object Get(String property)
	{
		synchronized (properties)
		{
			Object val;
			val = properties.get(property);
			return val;
		}
	}

	public final <T> void Set(String property, T value)
	{
		if (property == null)
		{
			throw new NullPointerException("property");
		}
		if (value == null)
		{
			throw new NullPointerException("value");
		}
		T oldValue = null;
		synchronized (properties)
		{
			if (!properties.containsKey(property))
			{
				properties.put(property, value);
			}
			else
			{
				oldValue = this.<T>Get(property, value);
				properties.put(property, value);
			}
		}
		OnPropertyChanged(new PropertyChangedEventArgs(this, property, oldValue, value));
	}

	public final boolean Contains(String property)
	{
		synchronized (properties)
		{
			return properties.containsKey(property);
		}
	}

	public final int getCount()
	{
		synchronized (properties)
		{
			return properties.size();
		}
	}

	public final boolean Remove(String property)
	{
		synchronized (properties)
		{
			properties.remove(property);
			return true;
		}
	}

	@Override
	public String toString()
	{
		synchronized (properties)
		{
			StringBuilder sb = new StringBuilder();
			sb.append("[Properties:{");
			for (Map.Entry<String, Object> entry : properties.entrySet())
			{
				sb.append(entry.getKey());
				sb.append("=");
				sb.append(entry.getValue());
				sb.append(",");
			}
			sb.append("}]");
			return sb.toString();
		}
	}

	public static Properties ReadFromAttributes(XMLStreamReader reader)
	{
		Properties properties = new Properties();
        try {
            if (reader.hasNext())
            {
                for (int i = 0; i < reader.getAttributeCount(); i++)
                {
                    var name = reader.getAttributeLocalName(i);
					var value = reader.getAttributeValue(i);
                    properties.set(name, value);
                }
            }
        } catch (XMLStreamException e) {
            e.printStackTrace();
        }
        return properties;
	}

	public void ReadProperties(XMLStreamReader reader, String endElement) throws XMLStreamException {
		if (reader.isStartElement()) {
			return;
		}

		while (reader.hasNext()) {
			int eventType = reader.next();
			switch (eventType) {
				case XMLStreamConstants.END_ELEMENT:
					if (reader.getLocalName().equals(endElement)) {
						return;
					}
					break;

				case XMLStreamConstants.START_ELEMENT:
					String propertyName = reader.getLocalName();
					if ("Properties".equals(propertyName)) {
						propertyName = reader.getAttributeValue(0);
						Properties p = new Properties();
						p.ReadProperties(reader, "Properties");
						properties.put(propertyName, p);
					} else if ("Array".equals(propertyName)) {
						propertyName = reader.getAttributeValue(0);
						properties.put(propertyName, ReadArray(reader));
					} else if ("SerializedValue".equals(propertyName)) {
						propertyName = reader.getAttributeValue(0);
						StringBuilder content = new StringBuilder();
						while (reader.hasNext()) {
							int innerEvent = reader.next();
							if (innerEvent == XMLStreamConstants.END_ELEMENT) {
								break;
							}
							if (innerEvent == XMLStreamConstants.CHARACTERS) {
								content.append(reader.getText());
							}
						}
						properties.put(propertyName, new SerializedValue(content.toString()));
					} else {
						String value = reader.getAttributeCount() > 0 ?
								reader.getAttributeValue(0) : null;
						properties.put(propertyName, value);
					}
					break;
			}
		}
	}

	private ArrayList ReadArray(XMLStreamReader reader)
	{
		ArrayList l = new ArrayList();
        try {
			for (int i = 0; i < reader.getAttributeCount(); i++)
			{
				int event = reader.next();
				switch (event)
				{
					case XMLStreamConstants.END_ELEMENT:
						if (reader.getLocalName().equals("Array"))
						{
							return l;
						}
						break;
					case XMLStreamConstants.ATTRIBUTE:
						l.add(reader.hasText() ? reader.getText() : null);
						break;
				}
			}
        } catch (XMLStreamException e) {

        }

		return l;
	}

	public final void WriteProperties(XMLStreamWriter writer)
	{
		synchronized (properties)
		{
			var sortedProperties = new ArrayList<Map.Entry<String, Object>>((Collection<? extends Map.Entry<String, Object>>) properties);
			sortedProperties.sort(Map.Entry.comparingByKey());
			for (Map.Entry<String, Object> entry : sortedProperties) {
				try {
					Object val = entry.getValue();
					if (val instanceof Properties) {
						writer.writeStartElement("Properties");
						writer.writeAttribute("name", entry.getKey());
						((Properties) val).WriteProperties(writer);
						writer.writeEndElement();
					} else if (val instanceof ArrayList) {
						writer.writeStartElement("Array");
						writer.writeAttribute("name", entry.getKey());
						for (Object o : (Iterable) val) {
							writer.writeStartElement("Element");
							WriteValue(writer, o);
							writer.writeEndElement();
						}
						writer.writeEndElement();
					} else if (val instanceof SerializedValue) {
						writer.writeStartElement("SerializedValue");
						writer.writeAttribute("name", entry.getKey());
						writer.writeCharacters(((SerializedValue) val).getContent());
						writer.writeEndElement();
					} else {
						writer.writeStartElement("SerializedValue");
						writer.writeAttribute("name", entry.getKey());
						writer.writeCharacters(val.toString());
						writer.writeEndElement();
					}
				} catch (XMLStreamException e) {
					throw new RuntimeException(e);
				}
			}
		}
	}

	private void WriteValue(XMLStreamWriter writer, Object val) throws XMLStreamException {
		if (val != null)
		{
			writer.writeAttribute("value", val.toString());
		}
	}

	public final void Save(String fileName) throws TransformerConfigurationException {
        try {

			TransformerFactory formerFactory = TransformerFactory.newInstance();
			Transformer transformer  = formerFactory.newTransformer();

			// 换行
			transformer.setOutputProperty(OutputKeys.INDENT, "YES");
			// 文档字符编码
			transformer.setOutputProperty(OutputKeys.ENCODING, "utf-8");

			StringWriter stringOut = new StringWriter();
			XMLStreamWriter writer = XMLOutputFactory.newDefaultFactory().createXMLStreamWriter(stringOut);
			this.WriteProperties(writer);
			var result = stringOut.toString();

			Files.writeString(Path.of(fileName),
					result, StandardCharsets.UTF_8, StandardOpenOption.CREATE);

        } catch (XMLStreamException | IOException | TransformerException e) {
            e.printStackTrace();
        }
    }


	public static Properties Load(String fileName) {
		if (!(new File(fileName)).isFile()) {
			return null;
		}

		XMLInputFactory factory = XMLInputFactory.newFactory();
		InputStream input = null;
		try {
			input = new FileInputStream(fileName);
			var reader = factory.createXMLStreamReader(input);
			var rootName = reader.getLocalName();
			if ("Properties".equals(rootName)) {
				Properties properties = new Properties();
				properties.ReadProperties(reader, "Properties");
			}
		} catch (FileNotFoundException | XMLStreamException e) {
			throw new RuntimeException(e);
		}

		return null;
	}

	public final <T> T Get(String property, T defaultValue)
	{
		synchronized (properties)
		{
			Object o;
			if (!(properties.containsKey(property) && (o = properties.get(property)) == o))
			{
				properties.put(property, defaultValue);
				return defaultValue;
			}

			try
			{
				return (T)o;
			}
			catch (NullPointerException e)
			{
				// can happen when configuration is invalid -> o is null and a value type is expected
				return defaultValue;
			}
		}
	}

	protected void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		if (PropertyChanged != null)
		{
			for (PropertyChangedEventHandler listener : PropertyChanged.listeners())
			{
				listener.invoke(this, e);
			}
		}
	}

	public sharpSystem.Event<PropertyChangedEventHandler> PropertyChanged = new sharpSystem.Event<PropertyChangedEventHandler>();
}