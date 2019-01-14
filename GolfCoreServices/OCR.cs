using System;
using Cloudmersive.APIClient.NETCore.OCR.Api;
using Cloudmersive.APIClient.NETCore.OCR.Client;
using Cloudmersive.APIClient.NETCore.OCR.Model;

namespace GolfCoreServices
{
    
    public class OCR
    {
        private const string ocrkey = "35c14606-1d5c-40ad-b32f-b9b1c8f27da8";

        public void Test()
        {
            // Configure API key authorization: Apikey
            Configuration.Default.AddApiKey("Apikey", ocrkey);

            var apiInstance = new ImageOcrApi();
            var imageFile = new System.IO.FileStream("C:\\temp\\inputfile", System.IO.FileMode.Open); // System.IO.Stream | Image file to perform OCR on.  Common file formats such as PNG, JPEG are supported.

            try
            {
                // Converts an uploaded image in common formats such as JPEG, PNG into text via Optical Character Recognition.
                ImageToTextResponse result = apiInstance.ImageOcrPost(imageFile);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ImageOcrApi.ImageOcrPost: " + e.Message);
            }
        }

    }
}
