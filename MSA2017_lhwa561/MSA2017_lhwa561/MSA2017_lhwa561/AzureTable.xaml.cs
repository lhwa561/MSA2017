using Microsoft.WindowsAzure.MobileServices;
using MSA2017_lhwa561.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSA2017_lhwa561
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzureTable : ContentPage
    {
        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;

        public AzureTable()
        {
            InitializeComponent();
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            //Debug.WriteLine("BUTTON PRESSED");
            List<MSA2017lhwa561Table> FaceInformation = await AzureManager.AzureManagerInstance.GetFaceInformation();
            FaceList.ItemsSource = FaceInformation;
            // Debug.WriteLine("FACE LIST GOT");
        }

        async void Handle_ClickedDelete(object sender, System.EventArgs e)
        {
            List<MSA2017lhwa561Table> FaceInformation = await AzureManager.AzureManagerInstance.GetFaceInformation();
            for (int i = 0; i < FaceInformation.Count; i++)
            {
                await AzureManager.AzureManagerInstance.DeleteFaceInformation(FaceInformation[i]);
            }
            FaceInformation.Clear();
            FaceList.ItemsSource = FaceInformation;
        }
    }
}