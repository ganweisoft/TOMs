package sharpSystem;

//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2022 Tangible Software Solutions, Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class is used to replicate some .NET string methods in Java.
//----------------------------------------------------------------------------------------
public final class StringHelper
{
	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'Substring' when 'start' is a method
	//	call or calculated value to ensure that 'start' is obtained just once.
	//------------------------------------------------------------------------------------
	public static String substring(String string, int start, int length)
	{
		if (length < 0)
			throw new IndexOutOfBoundsException("Parameter length cannot be negative.");

		return string.substring(start, start + length);
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET static string method 'IsNullOrEmpty'.
	//------------------------------------------------------------------------------------
	public static boolean isNullOrEmpty(String string)
	{
		return string == null || string.length() == 0;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET static string method 'IsNullOrWhiteSpace'.
	//------------------------------------------------------------------------------------
	public static boolean isNullOrWhiteSpace(String string)
	{
		if (string == null)
			return true;

		for (int index = 0; index < string.length(); index++)
		{
			if ( ! Character.isWhitespace(string.charAt(index)))
				return false;
		}

		return true;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET static string method 'Join' (2 parameter version).
	//------------------------------------------------------------------------------------
	public static String join(String separator, String[] stringArray)
	{
		if (stringArray == null)
			return null;
		else
			return join(separator, stringArray, 0, stringArray.length);
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET static string method 'Join' (4 parameter version).
	//------------------------------------------------------------------------------------
	public static String join(String separator, String[] stringArray, int startIndex, int count)
	{
		if (stringArray == null)
			return null;

		StringBuilder sb = new StringBuilder();

		for (int index = startIndex; index < stringArray.length && index - startIndex < count; index++)
		{
			if (separator != null && index > startIndex)
				sb.append(separator);

			if (stringArray[index] != null)
				sb.append(stringArray[index]);
		}

		return sb.toString();
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'Remove' (1 parameter version).
	//------------------------------------------------------------------------------------
	public static String remove(String string, int start)
	{
		return string.substring(0, start);
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'Remove' (2 parameter version).
	//------------------------------------------------------------------------------------
	public static String remove(String string, int start, int count)
	{
		return string.substring(0, start) + string.substring(start + count);
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'TrimEnd'.
	//------------------------------------------------------------------------------------
	public static String trimEnd(String string, Character... charsToTrim)
	{
		if (string == null || charsToTrim == null)
			return string;

		int lengthToKeep = string.length();
		for (int index = string.length() - 1; index >= 0; index--)
		{
			boolean removeChar = false;
			if (charsToTrim.length == 0)
			{
				if (Character.isWhitespace(string.charAt(index)))
				{
					lengthToKeep = index;
					removeChar = true;
				}
			}
			else
			{
				for (int trimCharIndex = 0; trimCharIndex < charsToTrim.length; trimCharIndex++)
				{
					if (string.charAt(index) == charsToTrim[trimCharIndex])
					{
						lengthToKeep = index;
						removeChar = true;
						break;
					}
				}
			}
			if ( ! removeChar)
				break;
		}
		return string.substring(0, lengthToKeep);
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'TrimStart'.
	//------------------------------------------------------------------------------------
	public static String trimStart(String string, Character... charsToTrim)
	{
		if (string == null || charsToTrim == null)
			return string;

		int startingIndex = 0;
		for (int index = 0; index < string.length(); index++)
		{
			boolean removeChar = false;
			if (charsToTrim.length == 0)
			{
				if (Character.isWhitespace(string.charAt(index)))
				{
					startingIndex = index + 1;
					removeChar = true;
				}
			}
			else
			{
				for (int trimCharIndex = 0; trimCharIndex < charsToTrim.length; trimCharIndex++)
				{
					if (string.charAt(index) == charsToTrim[trimCharIndex])
					{
						startingIndex = index + 1;
						removeChar = true;
						break;
					}
				}
			}
			if ( ! removeChar)
				break;
		}
		return string.substring(startingIndex);
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'Trim' when arguments are used.
	//------------------------------------------------------------------------------------
	public static String trim(String string, Character... charsToTrim)
	{
		return trimEnd(trimStart(string, charsToTrim), charsToTrim);
	}

	//------------------------------------------------------------------------------------
	//	This method is used for string equality comparisons when the option
	//	'Use helper 'stringsEqual' method to handle null strings' is selected
	//	(The Java String 'equals' method can't be called on a null instance).
	//------------------------------------------------------------------------------------
	public static boolean stringsEqual(String s1, String s2)
	{
		if (s1 == null && s2 == null)
			return true;
		else
			return s1 != null && s1.equals(s2);
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'PadRight' (1 parameter version).
	//------------------------------------------------------------------------------------
	public static String padRight(String string, int totalWidth)
	{
		return padRight(string, totalWidth, ' ');
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'PadRight' (2 parameter version).
	//------------------------------------------------------------------------------------
	public static String padRight(String string, int totalWidth, char paddingChar)
	{
		StringBuilder sb = new StringBuilder(string);

		while (sb.length() < totalWidth)
		{
			sb.append(paddingChar);
		}

		return sb.toString();
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'PadLeft' (1 parameter version).
	//------------------------------------------------------------------------------------
	public static String padLeft(String string, int totalWidth)
	{
		return padLeft(string, totalWidth, ' ');
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'PadLeft' (2 parameter version).
	//------------------------------------------------------------------------------------
	public static String padLeft(String string, int totalWidth, char paddingChar)
	{
		StringBuilder sb = new StringBuilder("");

		while (sb.length() + string.length() < totalWidth)
		{
			sb.append(paddingChar);
		}

		sb.append(string);
		return sb.toString();
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string constructor which repeats a character.
	//------------------------------------------------------------------------------------
	public static String repeatChar(char charToRepeat, int count)
	{
		String newString = "";
		for (int i = 1; i <= count; i++)
		{
			newString += charToRepeat;
		}
		return newString;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'LastIndexOf' (char version).
	//------------------------------------------------------------------------------------
	public static int lastIndexOf(String string, char value, int startIndex, int count)
	{
		int leftMost = startIndex + 1 - count;
		int rightMost = startIndex + 1;
		String substring = string.substring(leftMost, rightMost);
		int lastIndexInSubstring = substring.lastIndexOf(value);
		if (lastIndexInSubstring < 0)
			return -1;
		else
			return lastIndexInSubstring + leftMost;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'LastIndexOf' (string version).
	//------------------------------------------------------------------------------------
	public static int lastIndexOf(String string, String value, int startIndex, int count)
	{
		int leftMost = startIndex + 1 - count;
		int rightMost = startIndex + 1;
		String substring = string.substring(leftMost, rightMost);
		int lastIndexInSubstring = substring.lastIndexOf(value);
		if (lastIndexInSubstring < 0)
			return -1;
		else
			return lastIndexInSubstring + leftMost;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'IndexOfAny' (1 parameter version).
	//------------------------------------------------------------------------------------
	public static int indexOfAny(String string, char[] anyOf)
	{
		int lowestIndex = -1;
		for (char c : anyOf)
		{
			int index = string.indexOf(c);
			if (index > -1)
			{
				if (lowestIndex == -1 || index < lowestIndex)
				{
					lowestIndex = index;

					if (index == 0)
						break;
				}
			}
		}

		return lowestIndex;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'IndexOfAny' (2 parameter version).
	//------------------------------------------------------------------------------------
	public static int indexOfAny(String string, char[] anyOf, int startIndex)
	{
		int indexInSubstring = indexOfAny(string.substring(startIndex), anyOf);
		if (indexInSubstring == -1)
			return -1;
		else
			return indexInSubstring + startIndex;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'IndexOfAny' (3 parameter version).
	//------------------------------------------------------------------------------------
	public static int indexOfAny(String string, char[] anyOf, int startIndex, int count)
	{
		int endIndex = startIndex + count;
		int indexInSubstring = indexOfAny(string.substring(startIndex, endIndex), anyOf);
		if (indexInSubstring == -1)
			return -1;
		else
			return indexInSubstring + startIndex;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'LastIndexOfAny' (1 parameter version).
	//------------------------------------------------------------------------------------
	public static int lastIndexOfAny(String string, char[] anyOf)
	{
		int highestIndex = -1;
		for (char c : anyOf)
		{
			int index = string.lastIndexOf(c);
			if (index > highestIndex)
			{
				highestIndex = index;

				if (index == string.length() - 1)
					break;
			}
		}

		return highestIndex;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'LastIndexOfAny' (2 parameter version).
	//------------------------------------------------------------------------------------
	public static int lastIndexOfAny(String string, char[] anyOf, int startIndex)
	{
		String substring = string.substring(0, startIndex + 1);
		int lastIndexInSubstring = lastIndexOfAny(substring, anyOf);
		if (lastIndexInSubstring < 0)
			return -1;
		else
			return lastIndexInSubstring;
	}

	//------------------------------------------------------------------------------------
	//	This method replaces the .NET string method 'LastIndexOfAny' (3 parameter version).
	//------------------------------------------------------------------------------------
	public static int lastIndexOfAny(String string, char[] anyOf, int startIndex, int count)
	{
		int leftMost = startIndex + 1 - count;
		int rightMost = startIndex + 1;
		String substring = string.substring(leftMost, rightMost);
		int lastIndexInSubstring = lastIndexOfAny(substring, anyOf);
		if (lastIndexInSubstring < 0)
			return -1;
		else
			return lastIndexInSubstring + leftMost;
	}

}