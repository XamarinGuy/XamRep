using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.TextToSpeech;
using Tabs.Model;
using Xamarin.Forms;

namespace Tabs
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
            this.AnimationView.IsVisible = true;
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

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            TagLabel.Text = "";

            await MakePredictionRequest(file);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "4acc8969a66e4e6a8566c55658bbe9d2");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.1/Prediction/ce6463c9-a4b5-4af6-aed2-18babd9f8483/image?iterationId=1ddec163-8098-4387-b095-fa55183c512e";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    double max = responseModel.Predictions.Max(m => m.Probability);


                    if(max >= 0.5)
                    {
                        TagLabel.Text = "There is a heavy traffic in TinFactory";
                        await CrossTextToSpeech.Current.Speak(TagLabel.Text);
                        await Navigation.PushAsync(new redtrafficlightpage());
                        CrossLocalNotifications.Current.Show("Alert", "We have send the informaton to local concen autheroties");

                    }

                    else
                    {
                        TagLabel.Text = " There is no heavy Traffic in Tin Factory";
                        await CrossTextToSpeech.Current.Speak(TagLabel.Text);
                        await Navigation.PushAsync(new normaltrafficlightpage());
                    }


                  

                  
                }
               
                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }
    }
}
