using System;
using System.Globalization;

namespace Morph.Server.Sdk.Model
{
    public sealed class DateOnly
    {
        const string dateFormat = "yyyy-MM-dd";
        

        public int Year { get; }
        public int Month { get; }
        public int Day { get; }
        public DateOnly(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;            
        }

        public string ToIsoDate()
        {
            return string.Format("{0:D4}-{1:D2}-{2:D2}", Year, Month, Day);
        }
        public static DateOnly FromIsoDate(string isoDate)
        {
            if (string.IsNullOrWhiteSpace(isoDate))
                return null;
            if (DateTime.TryParseExact(isoDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime date))
            {
                var year = int.Parse(isoDate.Substring(0, 4));
                var month = int.Parse(isoDate.Substring(5, 2));
                var day = int.Parse(isoDate.Substring(8, 2));
                return new DateOnly(year, month, day);

            }
            else
            {
                throw
                new ArgumentException($"Date parameter expected to be formatted like {dateFormat} and have a valid date");
            }
        
        }
    }
}
