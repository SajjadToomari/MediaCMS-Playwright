using Microsoft.Playwright;
using PlaywrightDemo;
using Serilog;
using System.Diagnostics;
using System.Text.Json;

//Initilize console logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

//Read config from json
var configPath = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory), "config.json");
StreamReader streamReader = new(configPath);
var settings = await JsonSerializer.DeserializeAsync<Settings>(streamReader.BaseStream);

//Playwright initializer
using var playwright = await Playwright.CreateAsync();

Console.WriteLine("Playwright initilized");

//Launch chromium browser and save user data (cookies, etc)
await using var browser = await playwright.Chromium.LaunchPersistentContextAsync("data1", new BrowserTypeLaunchPersistentContextOptions
{
    Headless = settings.Headless,
    IgnoreHTTPSErrors = true
});

Console.WriteLine("Chromium launched");

var page = await browser.NewPageAsync();

await Login(page);
await GoToUploadPage(page);
await UploadVideos(page);

async Task Login(IPage page)
{
    await page.GotoAsync(settings.CmsLoginUrl);
    if (page.Url.Contains("accounts/login"))
    {
        //Fill username input
        await page.Locator(settings.LoginInputUsernameSelector).FillAsync(settings.CmsAdminUser);

        //Wait one second
        await Task.Delay(1000);

        //Fill password input
        await page.Locator(settings.LoginInputPasswordSelector).FillAsync(settings.CmsAdminPassword);

        //Wait one second
        await Task.Delay(1000);

        //Click login button
        await page.Locator(settings.LoginButtonSelector).ClickAsync();

        Console.WriteLine("Logged in");
    }
    else
    {
        Console.WriteLine("No need to login in");
    }
}

async Task GoToUploadPage(IPage page)
{
    //Go to upload page
    await page.GotoAsync(settings.CmsUploadUrl);

    //Wait one second
    await Task.Delay(1000);

    Console.WriteLine("Opened upload page");
}

async Task UploadVideos(IPage page)
{
    //Get videos path
    var videosPath = Directory.GetFiles(settings.UploadVideosDirectory, "*.mp4", SearchOption.TopDirectoryOnly);
    foreach (var item in videosPath)
    {
        Console.WriteLine(item);
    }

    Console.WriteLine("Videos Count: " + videosPath.Length);

    int i = 0;

    foreach (var videoPath in videosPath)
    {
        i++;

        Console.WriteLine("Uploading video : " + i);
        if (!File.Exists(videoPath + ".done"))
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            await page.SetInputFilesAsync(settings.UploadButtonSelector, videoPath);

            while (true)
            {
                if (stopWatch.ElapsedMilliseconds / 1000 > 360)
                {
                    Console.WriteLine("video skipped : " + i);
                    await page.ReloadAsync();
                    break;
                }

                await Task.Delay(2000);

                var isUploadSuccess = (await page.Locator("li.qq-file-id-0.qq-upload-success").CountAsync()) > 0;

                if (isUploadSuccess)
                {
                    Console.WriteLine("video completed : " + i);
                    File.WriteAllText(videoPath + ".done", "done");
                    await page.ReloadAsync();
                    break;
                }
            }
        }
    }
}

Console.WriteLine("End");

await Task.Delay(int.MaxValue);