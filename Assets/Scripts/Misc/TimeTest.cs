using UnityEngine;
using System;
using System.Threading.Tasks;

public class TimeTest : MonoBehaviour {

    public bool getTime = false;

    // никого не ждем
    private async void Start() {
        if (getTime) {
            await FetchAndLogWorldTimeAsync();
        }
    }

    private async void Update() {
        if(getTime) {
            getTime = false;
            await FetchAndLogWorldTimeAsync();
        }
    }

    private async Task FetchAndLogWorldTimeAsync() {
        DateTime currentTime = await GlobalTimeFetcher.FetchWorldTime();
        Debug.Log($"Net time: {currentTime}, PC time: {DateTime.Now}");
    }
}

/* Дожидаемся ответа от фетчера

    private async void Start() {
        await FetchAndLogWorldTime();
    }

    private async Task FetchAndLogWorldTime() {
        DateTime currentTime = await GlobalTimeFetcher.FetchWorldTime();
        Debug.Log("Current time: " + currentTime);
    }
*/