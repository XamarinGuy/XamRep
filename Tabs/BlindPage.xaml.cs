using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Plugin.Media;
using Plugin.TextToSpeech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BlindPage : ContentPage
    {
        public BlindPage()
        {
            InitializeComponent();
        }
        private async Task<AnalysisResult> GetImageDescription(Stream imageStream)
        {
            VisionServiceClient visionClient = new VisionServiceClient("4d673d0f18bc44d3b519400de1a7e76d", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
            VisualFeature[] features = { VisualFeature.Tags };
            return await visionClient.AnalyzeImageAsync(imageStream, features.ToList(), null);
        }

        private async Task SelectPicture()
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                var image = await CrossMedia.Current.PickPhotoAsync();
                MyImage.Source = ImageSource.FromStream(() =>
                {
                    return image.GetStream();
                });
                MyActivityIndicator.IsRunning = true;
                try
                {
                    var result = await GetImageDescription(image.GetStream());
                    foreach (var tag in result.Tags)
                    {
                        MyLabel.Text = MyLabel.Text + tag.Name + "\n";
                    }
                }
                catch (ClientException ex)
                {
                    MyLabel.Text = ex.Message;
                }
                await CrossTextToSpeech.Current.Speak(MyLabel.Text);
                MyActivityIndicator.IsRunning = false;
            }
        }

        async void Handle_Click(object sender, EventArgs e)
        {
            await SelectPicture();
        }
    }
}
