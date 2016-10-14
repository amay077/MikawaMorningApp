using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DataPagesDemo
{
	public partial class StartupPage : ContentPage
	{
		public StartupPage()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			
			InitializeComponent();
			image.Source = ImageSource.FromResource("DataPagesDemo.startup.jpg");
			image2.Source = ImageSource.FromResource("DataPagesDemo.startup2.png");

			buttonShops.Clicked += (sender, e) => this.Navigation.PushAsync(new SessionDataPage()); ;
			buttonMaps.Clicked += (sender, e) => this.Navigation.PushAsync(new CafesMapDataPage()); ;
		}
	}
}
