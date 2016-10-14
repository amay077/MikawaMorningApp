using System;
using Xamarin.Forms.Pages;

namespace DataPagesDemo
{
	public class MapDataPage : DataPage
	{
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
					var lat = (double)item["lat"];
					var lon = (double)item["lon"];
				}
				catch (Exception ex)
				{

				}
			}
		}
	}
}
