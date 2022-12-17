using Microsoft.Playwright;
using PlaywrightDemo;
using Serilog;
using System.Text.Json;

//Initilize console logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

//Read config from json
var configPath = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory), "config.json");
StreamReader streamReader = new (configPath);
var settings = await JsonSerializer.DeserializeAsync<Settings>(streamReader.BaseStream);

//Playwright initializer
using var playwright = await Playwright.CreateAsync();

Log.Information("Playwright initilized");

//Launch chromium browser and save user data (cookies, etc)
await using var browser = await playwright.Chromium.LaunchPersistentContextAsync("data1", new BrowserTypeLaunchPersistentContextOptions
{
    Headless = settings.Headless,
});

Log.Information("Chromium launched");

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

        Log.Information("Logged in");
    }
    else
    {
        Log.Information("No need to login in");
    }
}

async Task GoToUploadPage(IPage page)
{
    //Go to upload page
    await page.GotoAsync(settings.CmsUploadUrl);

    //Wait one second
    await Task.Delay(1000);

    Log.Information("Opened upload page");
}

async Task UploadVideos(IPage page)
{
    //Get videos path
    var videosPath = Directory.GetFiles(settings.UploadVideosDirectory, "*.mp4", SearchOption.TopDirectoryOnly);

    foreach (var videoPath in videosPath)
    {
        await page.SetInputFilesAsync(settings.UploadButtonSelector, videoPath);

        await Task.Delay(2000);
    }

}

await Task.Delay(int.MaxValue);