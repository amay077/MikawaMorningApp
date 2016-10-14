using System;
using System.Collections.Generic;
using Plugin.ExternalMaps;
using Plugin.ExternalMaps.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Pages;

namespace DataPagesDemo
{
	public partial class MapDataPage : DataPage
	{
		public static readonly BindableProperty DetailTemplateProperty = BindableProperty.Create(nameof(DetailTemplate), typeof(DataTemplate), typeof(MapDataPage), null);

		public DataTemplate DetailTemplate
		{
			get { return (DataTemplate)GetValue(DetailTemplateProperty); }
			set
			{
				SetValue(DetailTemplateProperty, value);
			}
		}

		public MapDataPage()
		{
			InitializeComponent();

			map.HasZoomEnabled = true;
			map.HasScrollEnabled = true;
			map.IsShowingUser = true;
		}

		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (DataPage.DataSourceProperty.PropertyName == propertyName)
			{
				OnDataSourceChanged();
			}
		}

		async void OnDataSourceChanged()
		{
			var dataSource = DataSource as JsonDataSource;
			await dataSource.ParseJson();
			var positions = new List<Position>();
			foreach (var dataItem in DataSource.Data)
			{
				try
				{
					var item = dataItem.Value as IDataSource;
					var title = item["title"].ToString();
					var room = item["presenter"].ToString();
					var lat = (float)item["lat"];
					var lon = (float)item["lon"];

					positions.Add(new Position(lat, lon));

					var pin = new Pin
					{
						Icon = BitmapDescriptorFactory.FromBundle("coffee"),
						Type = PinType.Place,
						Label = room,
						Address = title,
						Position = new Position(lat, lon),
						Tag = dataItem
					};

					pin.Clicked += Pin_Clicked;

					map.Pins.Add(pin);
				}
				catch (Exception ex)
				{

				}
			}

			map.MoveToRegion(MapSpan.FromPositions(positions), true);
		}

		async void Pin_Clicked(object sender, EventArgs e)
		{
			var pin = sender as Pin;

			if (await DisplayAlert(pin.Label, "ここへ行く", "GO!", "No"))
			{
				await CrossExternalMaps.Current.NavigateTo(pin.Label, pin.Position.Latitude, pin.Position.Longitude, NavigationType.Default);
			}
		}

		async void Map_SelectedPinChanged(object sender, SelectedPinChangedEventArgs e)
		{
			DataTemplate template = DetailTemplate;
			if (e.SelectedPin == null)
				return;

			Page detailPage;
			if (template == null)
			{
				detailPage = new DataPage();
			}
			else
			{
				detailPage = (Page)CreateContent2(template, e.SelectedPin.Tag, this);
			}

			detailPage.BindingContext = e.SelectedPin.Tag;
			await Navigation.PushAsync(detailPage);
		}

		static object CreateContent2(DataTemplate self, object item, BindableObject container)
		{
			var selector = self as DataTemplateSelector;
			if (selector != null)
			{
				self = selector.SelectTemplate(item, container);
			}
			return self.CreateContent();
		}
	}
}
