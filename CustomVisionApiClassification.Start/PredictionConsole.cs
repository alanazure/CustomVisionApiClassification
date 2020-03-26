using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CustomVisionApiClassification.Start
{
    class PredictionConsole
    {


        static string TestImageFolder = @"C:\AIData\Cars\Test";
        static string PredictionImageFolder = @"C:\AIData\Cars\Prediction";




        static async Task Main(string[] args)
        {
            Console.WriteLine("Custom Vision Api - Classification - Prediction");

            var predictionResults = await PredictTestImageFolder();

            foreach (var result in predictionResults)
            {
                Console.WriteLine($"{ result.Key } - { result.Value * 100 }");
            }



        }

        static async Task<Dictionary<string, double>> PredictTestImageFolder()
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
                    Console.WriteLine($"    Processing { testImageFileName }...");

                    // ToDo: Get the top prediction


                    // Fix the statistics
                    overallPredicted++;
                    tagPredicted++;


                }

                // Add the tag percentage correct to the results
                results.Add(testTagName, (double)tagCorrect / (double)tagPredicted);


            }

            // Add the overall percentage correct to the results
            results.Add("Overall", (double)overallCorrect / (double)overallPredicted);
            return results;
        }


    }
}
