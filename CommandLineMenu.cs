using System;

namespace RFCredit
{
    public class CommandLineMenu
    {
        private RandomForestClassifier randomForest;
        private double[][] features;
        private double[] labels;
        private string filePath = "german_credit_data.csv"; // Path to the CSV file

        public CommandLineMenu()
        {
            randomForest = new RandomForestClassifier(); // Initialize Random Forest
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Random Forest CLI Menu");
                Console.WriteLine("1. Load Data");
                Console.WriteLine("2. Train Model");
                Console.WriteLine("3. Evaluate Model");
                Console.WriteLine("4. Adjust Hyperparameters");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        LoadData();
                        break;
                    case "2":
                        TrainModel();
                        break;
                    case "3":
                        EvaluateModel();
                        break;
                    case "4":
                        AdjustHyperparameters();
                        break;
                    case "5":
                        Console.WriteLine("Exiting program...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void LoadData()
        {
            Console.WriteLine("Loading data...");
            try
            {
                CsvReader csvReader = new CsvReader(filePath);
                var data = csvReader.LoadData();

                DatasetManager datasetManager = new DatasetManager();
                datasetManager.LoadData(data);

                features = datasetManager.Features.ToArray();
                labels = datasetManager.Labels.ToArray();

                Console.WriteLine($"Data loaded successfully. Rows: {features.Length}, Features per row: {features[0].Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
            Pause();
        }

        private void TrainModel()
        {
            if (features == null || labels == null)
            {
                Console.WriteLine("No data loaded. Please load data before training.");
                Pause();
                return;
            }

            Console.WriteLine("Training the random forest...");
            try
            {
                randomForest.Train(features, labels);
                Console.WriteLine("Training completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during training: {ex.Message}");
            }
            Pause();
        }

        private void EvaluateModel()
        {
            if (features == null || labels == null)
            {
                Console.WriteLine("No data loaded. Please load data before evaluation.");
                Pause();
                return;
            }

            Console.WriteLine("Evaluating the random forest...");
            try
            {
                int correct = 0;
                for (int i = 0; i < features.Length; i++)
                {
                    // Get raw prediction from the Random Forest
                    double rawPrediction = randomForest.Predict(features[i]);

                    // Classify the raw prediction into either 1 or 2
                    double classifiedPrediction = rawPrediction < 1.5 ? 1.0 : 2.0;

                    // Check if the classification matches the actual label
                    bool isCorrect = Math.Abs(labels[i] - classifiedPrediction) < 0.001;
                    if (isCorrect) correct++;

                    // Print the result for this row
                    Console.WriteLine($"Row {i + 1}: Raw Predicted = {rawPrediction:F4}, Classified = {classifiedPrediction}, Actual = {labels[i]}, Result = {(isCorrect ? "Correct" : "Incorrect")}");
                }

                // Calculate accuracy
                double accuracy = (correct / (double)features.Length) * 100.0;
                Console.WriteLine($"Accuracy: {accuracy:F2}%");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during evaluation: {ex.Message}");
            }
            Pause();
        }

        private void AdjustHyperparameters()
        {
            try
            {
                Console.Write("Enter number of trees (default 100): ");
                int nTrees = int.Parse(Console.ReadLine());

                Console.Write("Enter maximum tree depth (default 5): ");
                int maxDepth = int.Parse(Console.ReadLine());

                Console.Write("Enter maximum features per tree (-1 for all features): ");
                int maxFeatures = int.Parse(Console.ReadLine());

                randomForest = new RandomForestClassifier(nTrees, maxDepth, maxFeatures);
                Console.WriteLine("Hyperparameters updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid input: {ex.Message}");
            }
            Pause();
        }

        private void Pause()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
