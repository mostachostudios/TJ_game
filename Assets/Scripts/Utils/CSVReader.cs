using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CSVReader
{
	// splits a CSV file into a 2D string array
	static public string[,] SplitCsvGrid(string csvText)
	{
		// TODO escape characters
		string[] lines = csvText.Split('\n');

		if(lines.Length == 0)
		{
			return new string[0, 0];
		}

		int rows = lines.Length - 1;

		string[] firstRowColumns = lines[0].Split(';');

		int cols = firstRowColumns.Length;

		string[,] outputGrid = new string[rows, cols];

		for(int i = 0; i < cols; i++)
		{
			outputGrid[0, i] = firstRowColumns[i];
		}

		for(int i = 1; i < rows; i++)
		{
			string[] thisRowColumns = lines[i].Split(';');
			for (int j = 0; j < cols; j++)
			{
				outputGrid[i, j] = thisRowColumns[j];
			}
		}

		return outputGrid;
	}

	// splits a CSV row 
	static public string[] SplitCsvLine(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
		@"(((?<x>(?=[;\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^;\r\n]+));?)",
		System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
				select m.Groups[1].Value).ToArray();
	}

	static public string[] GetRow(string csvText, int index)
	{
		int currentIndex = 0;
		int currentLineStart = 0;

		for(int i = 0; i < csvText.Length; i++)
		{
			if(csvText[i] == '\n')
			{
				if(index == currentIndex)
				{
					return csvText.Substring(currentLineStart, i - currentLineStart).Split(';');
				}
				else
				{
					currentIndex++;
					currentLineStart = i + 1;
				}
			}
		}

		return new string[0];
	}

	static public string[] GetColumn(string csvText, int index)
	{
		List<string> column = new List<string>();

		int currentColumnIndex = 0;
		int currentCellStart = 0;

		for (int i = 0; i < csvText.Length; i++)
		{
			if(csvText[i] == ';') // delimiter
			{
				if(currentColumnIndex == index)
				{
					column.Add(csvText.Substring(currentCellStart, i - currentCellStart));
				}
				currentCellStart = i + 1;
				currentColumnIndex++;
			}
			else if (csvText[i] == '\n')
			{
				if (currentColumnIndex == index)
				{
					column.Add(csvText.Substring(currentCellStart, i - currentCellStart));
				}
				currentCellStart = i + 1;
				currentColumnIndex = 0;
			}
		}

		return column.ToArray();
	}

}