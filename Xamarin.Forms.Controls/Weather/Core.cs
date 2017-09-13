using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.Controls
{
    public class Core
    {
        public static async Task<Weather> GetWeather(string zipCode)
        {
            // Sign up for a free API key at http://openweathermap.org/appid
            string key = "fc9f6c524fc093759cd28d41fda89a1b";
            string queryString = "http://api.openweathermap.org/data/2.5/weather?zip="
                + zipCode + "&appid=" + key;

            var results = await DataService.getDataFromService(queryString).ConfigureAwait(false);

            if (results["weather"] != null)
            {
                Weather weather = new Weather();
                weather.Title = (string)results["name"];
                weather.Temperature = (string)results["main"]["temp"] + " F";
                weather.Wind = (string)results["wind"]["speed"] + " mph";
                weather.Humidity = (string)results["main"]["humidity"] + " %";
                weather.Visibility = (string)results["weather"][0]["main"];

                DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                DateTime sunrise = time.AddSeconds((double)results["sys"]["sunrise"]);
                DateTime sunset = time.AddSeconds((double)results["sys"]["sunset"]);
                weather.Sunrise = sunrise.ToString() + " UTC";
                weather.Sunset = sunset.ToString() + " UTC";
                return weather;
            }
            else
            {
                return null;
            }
        }
    }
}