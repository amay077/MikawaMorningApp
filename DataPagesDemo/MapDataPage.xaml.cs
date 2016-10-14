using System;
using System.Collections.Generic;

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
			foreach (var dataItem in DataSource.Data)
			{
				try
				{
					var item = dataItem.Value as IDataSource;
					var title = item["title"].ToString();
					var room = item["room"].ToString();
					var lat = (float)item["lat"];
					var lon = (float)item["lon"];

					var pin = new Pin
					{
						Type = PinType.Place,
						Label = title,
						Address = room,
						Position = new Position(lat, lon),
						Tag = dataItem
					};

					map.Pins.Add(pin);
				}
				catch (Exception ex)
				{

				}
			}

			map.SelectedPinChanged += Map_SelectedPinChanged;

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
