using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("Enter the name of the file from where to extract: ");
string filePath = Console.ReadLine();

Console.WriteLine();
Console.WriteLine("Enter the RegEx string to search for: ");
string regEx = Console.ReadLine();
Regex regex = new Regex(regEx);

Console.WriteLine();
Console.WriteLine("The higher the matchable string length, the longer the string matched with RegEx can be. However, the more RAM will be used to find strings.");
Console.WriteLine("Enter no value if you wish to read the entire file at once. (This is not recommended for very large files!)");
Console.Write("Enter the maximum length of the matching string: ");
string bufferSizeStr = Console.ReadLine();
long bufferSize = 0;
if(bufferSizeStr != null && bufferSizeStr != "")
{
	bufferSize = int.Parse(bufferSizeStr);
}
else
{
	bufferSize = new FileInfo(filePath).Length;
}

Dictionary<long, string> results = new Dictionary<long, string>();
using (Stream s = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
{
	long totalBytesRead = 0;
	byte[] readBlock = new byte[bufferSize];
	while(true)
	{
		long readBytes = s.Read(readBlock, 0, readBlock.Length);
		if(readBytes > 0)
		{
			string readStr = Encoding.Default.GetString(readBlock);
			MatchCollection matches = regex.Matches(readStr);
			foreach(Match match in matches)
			{
				if(match.Success)
				{
					results.Add(match.Index + totalBytesRead, match.Value);
					Console.WriteLine();
					Console.WriteLine(match.Value + " found at index " + (match.Index + totalBytesRead));
				}
			}

			totalBytesRead += readBytes;
		}

		if (bufferSize > readBytes)
		{
			break;
		}
	}
}

Console.WriteLine();
Console.WriteLine(results.Count + " results found!");
foreach(long key in results.Keys)
{
	Console.WriteLine(results[key] + " found at index " + key);
}

while (true)
{
	Console.WriteLine();
	Console.WriteLine("Do you wish to 1. Save the results or 2. Refine the results with further RegEx?");
	string choiceStr = Console.ReadLine();
	int choice = int.Parse(choiceStr);

	if (choice == 1)
	{
		Console.WriteLine();
		Console.WriteLine("Please enter the path and name of the file to store the results to:");
		string saveFilePath = Console.ReadLine();

		using (StreamWriter sw = new StreamWriter(saveFilePath))
		{
			foreach (long key in results.Keys)
			{
				sw.WriteLine(results[key] + " found at index " + key);
			}
		}

		break;
	}
	else if (choice == 2)
	{
		Console.WriteLine();
		Console.WriteLine("Please enter the RegEx string:");
		string refineRegExStr = Console.ReadLine();

		Dictionary<long, string> tempResults = new Dictionary<long, string>();

		Regex refineRegEx = new Regex(refineRegExStr);
		foreach(long key in results.Keys)
		{
			string value = results[key];
			Match newMatch = refineRegEx.Match(value);
			if(newMatch.Success)
			{
				tempResults.Add(key, newMatch.Value);
			}
		}

		results = new Dictionary<long, string>();
		foreach(long key in tempResults.Keys)
		{
			results.Add(key, tempResults[key]);
		}
	}
}