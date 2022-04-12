using RemedioApp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RemedioApp
{
    public partial class MainPage : ContentPage
    {
        public readonly MainPageViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();
            this.viewModel = new MainPageViewModel();
            this.BindingContext = this.viewModel;
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            this.viewModel.GetLeitura();
        }
        private void OnUpdateList(object sender, EventArgs e)
        {
            this.viewModel.GetLeitura();
        }
    }
}
