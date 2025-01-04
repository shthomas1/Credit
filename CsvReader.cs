using System;
using System.Collections.Generic;
using System.IO;

namespace RFCredit
{
    public class CsvReader
    {
        private readonly string filePath;

        public CsvReader(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public List<string[]> LoadData()
        {
            var data = new List<string[]>();

            try
            {
                using var reader = new StreamReader(filePath);
                bool isFirstLine = true;

                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (isFirstLine)
                    {
                        isFirstLine = false; // Skip the header row
                        continue;
                    }

                    string[] columns = line.Split(',');

                    // Ensure all rows have consistent column counts
                    if (columns.Length == 0)
                    {
                        Console.WriteLine($"Warning: Skipping a blank or malformed line: '{line}'");
                        continue;
                    }

                    data.Add(columns);
                }

                Console.WriteLine($"Data loaded successfully. Total rows (excluding header): {data.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                throw;
            }

            return data;
        }
    }
}
