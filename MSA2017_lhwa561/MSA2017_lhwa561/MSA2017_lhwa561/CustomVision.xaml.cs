using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//using MSA2017_lhwa561.Model;

using Newtonsoft.Json;

using Plugin.Media;
using Plugin.Media.Abstractions;
using MSA2017_lhwa561.Model;

namespace MSA2017_lhwa561
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent(); 
        }
        private async void loadCamera(object sender, EventArgs e)
        {
            
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

           
            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

           
            if (file == null)
                return;


            GenderLabel.Text = "Analysing...";
            AgeLabel.Text = "";
            EmotionLabel.Text = "";

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            //file.Dispose();
            MakeAnalysisRequest(file);
        }

        const string subscriptionKey = "0c9ce465dfb349f4bd8c71302edf60f7";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect";

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }
        
        private async void MakeAnalysisRequest(MediaFile file)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
               
                JsonToList(contentString);
            }
        }

        async void JsonToList(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");
            int jsonsize = json.Length;
            //bool endofvalue = false;
            int firstofGender = json.IndexOf("gender");
            int firstofAge = json.IndexOf("age");
            int firstofEmotion = json.IndexOf("anger");
            Dictionary<int, double> emotions = new Dictionary<int, double>();
            char ch = ' ';
            bool numstate = false;

            StringBuilder temp = new StringBuilder();
            double tempVal = 0;

            string g = "Male";
            double a = 0;
            string e = "Neutral";

            for (int i = firstofGender; i < jsonsize; i++)
            {
                ch = json[i];
                if (numstate == false)
                {
                    if (ch == ':')
                    {
                        numstate = true;
                    }
                }
                else
                {
                    if (ch == ',')
                    {
                        g = temp.ToString();
                        GenderLabel.Text = "Gender: " + g;
                        temp.Clear();
                        numstate = false;
                        break;
                    }
                    else if (ch != '"' && ch != ' ')
                    {
                        temp.Append(ch);
                    }
                }
            }

            for (int i = firstofAge; i < jsonsize; i++)
            {
                ch = json[i];
                if (numstate == false)
                {
                    if (ch == ':')
                    {
                        numstate = true;
                    }
                }
                else
                {
                    if (ch == ',')
                    {
                        AgeLabel.Text = "Age: " + temp.ToString();
                        a = double.Parse(temp.ToString());
                        temp.Clear();
                        numstate = false;
                        break;
                    }
                    else if (ch != '"' && ch != ' ')
                    {
                        temp.Append(ch);
                    }
                }
            }
            //bool secondQuote = false;
            int itr = 0;
            for (int j = firstofEmotion; j < jsonsize; j++)
            {
                ch = json[j];
                if (numstate == false)
                {

                    if (ch == ':')
                    {
                        numstate = true;
                    }
                }
                else
                {
                    if (ch == ',' || ch == '}') 
                    {

                        tempVal = double.Parse(temp.ToString());
                        emotions.Add(itr, tempVal);

                        temp.Clear();
                        numstate = false;
                        if (itr < 7)
                        {
                            itr++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (ch != '"' && ch != ' ')
                    {
                       temp.Append(ch);
                    }
                }
            }

            List<string> emotionString = new List<string>();
            emotionString.Add("Anger");
            emotionString.Add("Contempt");
            emotionString.Add("Disgust");
            emotionString.Add("Fear");
            emotionString.Add("Happiness");
            emotionString.Add("Neutral");
            emotionString.Add("Sadness");
            emotionString.Add("Surprise");

            int MaxIndex = 0;
            //Debug.WriteLine("Determining Max Index");
            for (int i = 1; i < 8; i++)
            {
                if (emotions[MaxIndex] < emotions[i])
                    MaxIndex = i;
            }
            e = emotionString[MaxIndex];
            EmotionLabel.Text = "Emotion: " + e;

            await postFaceAsync(g, a, e);

            return;
        }

        async private Task postFaceAsync(string g, double a, string e)
        {
            MSA2017lhwa561Table model = new MSA2017lhwa561Table()
            {
                Gender = g,
                Age = a,
                Emotion = e
            };
            await AzureManager.AzureManagerInstance.PostFaceInformation(model);
        }

    }
}