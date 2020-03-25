using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomVisionApiClassification.Complete
{

    class PredictionConsole
    {
        // Set your Custom Vision service details here...
        static string Endpoint = "";
        static string PredictionKey = "";

        // Set your Custom Vision project details here...
        static string ProjetId = "";
        static string PublishedName = "";

        // Set the paths to your test images and output prediction folder here...
        static string TestImageFolder = @"C:\AIData\Cars\Test";
        static string PredictionImageFolder = @"C:\AIData\Cars\Prediction";


        static async Task Main(string[] args)
        {
            Console.WriteLine("Custom Vision Api - Classification - Prediction");

            var predictionResults = await PredictTestImageFolderAsync();

            foreach (var result in predictionResults)
            {
                Console.WriteLine($"{ result.Key } - { result.Value * 100 }");
            }

            // Test for a sinfle image...
            //var imagePrediction = await GetImagePredictionsAsync(@"C:\AIData\Images\ActiveSolution\Alan\scene00382.png");

            //foreach (var prediction in imagePrediction.Predictions)
            //{
            //    Console.WriteLine($"{ prediction.TagName } - { (int)(prediction.Probability * 100) }%");
            //}


        }

        static async Task<Dictionary<string, double>> PredictTestImageFolderAsync()
        {
            var results = new Dictionary<string, double>();

            int overallPredicted = 0;
            int overallCorrect = 0;

            // Iterate through the test folders
            var testImageFolders = Directory.EnumerateDirectories(TestImageFolder);

            // Create the prediction folders
            foreach (var testFolder in testImageFolders)
            {
                var testTagName = Path.GetFileName(testFolder);
                var predictionsFolder = Path.Combine(PredictionImageFolder, testTagName);
                if (!Directory.Exists(predictionsFolder))
                {
                    Directory.CreateDirectory(predictionsFolder);
                }
            }

            foreach (var testFolder in testImageFolders)
            {
                int tagPredicted = 0;
                int tagCorrect = 0;

                // Get the name of the tag
                var testTagName = Path.GetFileName(testFolder);
                Console.WriteLine($"Processing { testTagName }...");

                // Iterate through the test images
                var testImages = Directory.GetFiles(testFolder);
                foreach (var testImage in testImages)
                {
                    var testImageFileName = Path.GetFileName(testImage);
                    //Console.WriteLine($"    Processing { testImageFileName }...");

                    // Get the top prediction
                    var predictions = await GetImagePredictionsAsync(testImage);
                    var topPredicton = predictions.Predictions.OrderByDescending(q => q.Probability).FirstOrDefault();

                    // Fix the statistics
                    overallPredicted++;
                    tagPredicted++;

                    bool isCorrect = testTagName == topPredicton.TagName;
                    if (isCorrect)
                    {
                        overallCorrect++;
                        tagCorrect++;
                    }

                    // Display the results
                    var temp = Console.ForegroundColor;
                    Console.ForegroundColor = isCorrect ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($"    { testImageFileName } predicted as { topPredicton.TagName } with { (int)(topPredicton.Probability * 100) }");
                    Console.ForegroundColor = temp;


                    // Copy the image to the predicted folder
                    File.Copy(testImage, Path.Combine(PredictionImageFolder, topPredicton.TagName, $"{ testTagName }_{ testImageFileName }"));
                }

                // Add the tag percentage correct to the results
                results.Add(testTagName, (double)tagCorrect / (double)tagPredicted);


            }

            // Add the overall percentage correct to the results
            results.Add("Overall", (double)overallCorrect / (double)overallPredicted);
            return results;
        }

        static async Task<ImagePrediction> GetImagePredictionsAsync(string imageFile)
        {
            // Create a prediction client
            var predictionClient = new CustomVisionPredictionClient()
            {
                Endpoint = Endpoint,
                ApiKey = PredictionKey
            };

            // Get predictions from the image
            using (var imageStream = new FileStream(imageFile, FileMode.Open))
            {

                var predictions = await predictionClient.ClassifyImageAsync
                    (new Guid(ProjetId), PublishedName, imageStream);
                return predictions;
            };
        }


    }

}
