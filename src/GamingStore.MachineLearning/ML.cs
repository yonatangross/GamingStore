using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingStore.Contracts.ML;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace GamingStore.MachineLearning
{
    public class ML
    {
        private const string TrainingDataLocation = @"D:\Compare.txt";
        public static async Task<Dictionary<int, double>> Run(Request request)
        {
            CreateFile(request.ItemCustomersList);
            //STEP 1: Create MLContext to be shared across the model creation workflow objects 
            var mlContext = new MLContext();

            //STEP 2: Read the trained data using TextLoader by defining the schema for reading the product co-purchase dataset
            //        Do remember to replace amazon0302.txt with dataset from https://snap.stanford.edu/data/amazon0302.html
            IDataView dataView = mlContext.Data.LoadFromTextFile(path: TrainingDataLocation,
                                                      new[]
                                                                {
                                                                    new TextLoader.Column("Label", DataKind.Single, 0),
                                                                    new TextLoader.Column(nameof(ProductEntry.CustomerNumber), DataKind.UInt32, new [] { new TextLoader.Range(0) }, new KeyCount(10000)),
                                                                    new TextLoader.Column(nameof(ProductEntry.RelatedItemId), DataKind.UInt32, new [] { new TextLoader.Range(1) }, new KeyCount(10000))
                                                                },
                                                      hasHeader: true);

            //STEP 3: Your data is already encoded so all you need to do is specify options for MatrxiFactorizationTrainer with a few extra hyperparameters
            //        LossFunction, Alpa, Lambda and a few others like K and C as shown below and call the trainer. 
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(ProductEntry.CustomerNumber),
                MatrixRowIndexColumnName = nameof(ProductEntry.RelatedItemId),
                LabelColumnName = "Label",
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                Alpha = 0.01,
                Lambda = 0.025
            };

            // For better results use the following parameters
            //options.K = 100;
            //options.C = 0.00001;

            //Step 4: Call the MatrixFactorization trainer by passing options.
            MatrixFactorizationTrainer est = mlContext.Recommendation().Trainers.MatrixFactorization(options);

            //STEP 5: Train the model fitting to the DataSet
            //Please add Amazon0302.txt dataset from https://snap.stanford.edu/data/amazon0302.html to Data folder if FileNotFoundException is thrown.
            ITransformer model = est.Fit(dataView);

            //STEP 6: Create prediction engine and predict the score for Product 63 being co-purchased with Product 3.
            //        The higher the score the higher the probability for this particular productID being co-purchased 
            PredictionEngine<ProductEntry, PredictionScore> predictionEngine = mlContext.Model.CreatePredictionEngine<ProductEntry, PredictionScore>(model);
            var scores = new Dictionary<int, double>();

            foreach (int itemId in request.AllItemsIds)
            {
                var entry = new ProductEntry
                {
                    CustomerNumber = (uint)request.CustomerNumber,
                    RelatedItemId = (uint)itemId
                };

                PredictionScore predictionScore = predictionEngine.Predict(entry);
                double finalScore = Math.Round(predictionScore.Score, 3);
                scores.Add(itemId, finalScore);
            }

            scores = scores.ToDictionary(pair => pair.Key, pair => pair.Value);

            return scores;
        }

        private static void CreateFile(List<ItemsCustomers> customersItems)
        {
            File.Delete(@"D:\Compare.txt");
            File.AppendAllText(@"D:\Compare.txt", $"ProductID	ProductID_Copurchased{Environment.NewLine}");

            foreach (var customerItem in customersItems)
            {
                File.AppendAllText(@"D:\Compare.txt", $"{customerItem.CustomerNumber}	{customerItem.ItemId}{Environment.NewLine}");
            }
        }

        //public static string GetAbsolutePath(string relativeDatasetPath)
        //{
        //    FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
        //    string assemblyFolderPath = _dataRoot.Directory.FullName;

        //    string fullPath = Path.Combine(assemblyFolderPath, relativeDatasetPath);

        //    return fullPath;
        //}
    }
}
