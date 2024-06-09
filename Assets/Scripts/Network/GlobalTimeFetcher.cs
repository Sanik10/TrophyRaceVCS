using UnityEngine;
using System;
using System.Net.Http;
using System.Globalization;
using System.Threading.Tasks;

public static class GlobalTimeFetcher {

    private static DateTime lastFetchedTime;
    private static bool isFetching;

    public static async Task<DateTime> FetchWorldTime() {
        if (!isFetching && InternetChecker.IsConnected) {
            isFetching = true;
            string apiUrl = "http://worldtimeapi.org/api/ip";

            using (HttpClient client = new HttpClient()) {
                try {
                    string response = await client.GetStringAsync(apiUrl);
                    lastFetchedTime = ParseWorldTime(response);
                    // Debug.Log("Current World Time:  " + lastFetchedTime);
                } catch (HttpRequestException e) {
                    Debug.LogError("HTTP request error: " + e.Message);
                } catch (Exception e) {
                    Debug.LogError("Error fetching time: " + e.Message);
                }
            }

            isFetching = false;
        }

        return lastFetchedTime;
    }

    private static DateTime ParseWorldTime(string timeString) {
        DateTime result;
        if(DateTime.TryParseExact(timeString, "yyyy-MM-ddTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result)) {
            return result;
        } else {
            try {
                var json = JsonUtility.FromJson<WorldTimeResponse>(timeString);
                result = DateTime.Parse(json.utc_datetime);
                return result;
            } catch (Exception ex) {
                Debug.LogError("Error parsing world time: " + ex.Message);
                return DateTime.MinValue;
            }
        }
    }
}

[Serializable]
public class WorldTimeResponse {
    public string abbreviation;
    public string client_ip;
    public string datetime;
    public int day_of_week;
    public int day_of_year;
    public bool dst;
    public string dst_from;
    public int dst_offset;
    public string dst_until;
    public int raw_offset;
    public string timezone;
    public int unixtime;
    public string utc_datetime;
    public string utc_offset;
    public int week_number;
}