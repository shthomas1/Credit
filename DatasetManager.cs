using System;
using System.Collections.Generic;
using System.Linq;

namespace RFCredit
{
    public class DatasetManager
    {
        public List<double[]> Features { get; private set; }
        public List<double> Labels { get; private set; }

        private Dictionary<string, double> categoricalEncodings;
        private double[] minValues;
        private double[] maxValues;

        public DatasetManager()
        {
            Features = new List<double[]>();
            Labels = new List<double>();
            categoricalEncodings = new Dictionary<string, double>();
        }

        public void LoadData(List<string[]> data)
        {
            if (data == null || data.Count == 0)
                throw new ArgumentException("Data cannot be null or empty.");

            int featureCount = data[0].Length - 1;

            // Debugging output
            Console.WriteLine($"Number of columns (features + label): {data[0].Length}");
            Console.WriteLine($"Assumed number of features: {featureCount}");

            minValues = new double[featureCount];
            maxValues = new double[featureCount];
            Array.Fill(minValues, double.MaxValue);
            Array.Fill(maxValues, double.MinValue);

            foreach (var row in data)
            {
                if (row.Length != featureCount + 1)
                {
                    Console.WriteLine($"Warning: Skipping row with mismatched column count: {string.Join(", ", row)}");
                    continue;
                }

                for (int i = 0; i < featureCount; i++)
                {
                    if (IsNumeric(row[i]))
                    {
                        double value = double.Parse(row[i]);
                        minValues[i] = Math.Min(minValues[i], value);
                        maxValues[i] = Math.Max(maxValues[i], value);
                    }
                }
            }

            foreach (var row in data)
            {
                if (row.Length != featureCount + 1) continue;

                double[] featureVector = new double[featureCount];
                for (int i = 0; i < featureCount; i++)
                {
                    if (IsNumeric(row[i]))
                    {
                        double value = double.Parse(row[i]);
                        featureVector[i] = Normalize(value, minValues[i], maxValues[i]);
                    }
                    else
                    {
                        featureVector[i] = EncodeFeature(row[i]);
                    }
                }

                Features.Add(featureVector);

                if (double.TryParse(row[^1], out double label))
                {
                    Labels.Add(label);
                }
                else
                {
                    Console.WriteLine($"Warning: Skipping row with invalid label: {string.Join(", ", row)}");
                }
            }

            Console.WriteLine($"Data processed successfully. Total valid rows: {Features.Count}");
        }

        private double Normalize(double value, double min, double max)
        {
            return min == max ? 0.0 : (value - min) / (max - min);
        }

        private double EncodeFeature(string feature)
        {
            if (!categoricalEncodings.ContainsKey(feature))
            {
                categoricalEncodings[feature] = categoricalEncodings.Count + 1;
                Console.WriteLine($"Encoded '{feature}' as {categoricalEncodings[feature]}");
            }
            return categoricalEncodings[feature];
        }

        private bool IsNumeric(string value)
        {
            return double.TryParse(value, out _);
        }
    }
}
