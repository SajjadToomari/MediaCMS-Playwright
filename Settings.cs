using System.Text.Json.Serialization;

namespace PlaywrightDemo;

public class Settings
{
    [JsonPropertyName("headless")]
    public bool Headless { get; set; }

    [JsonPropertyName("cms_admin_user")]
    public string CmsAdminUser { get; set; }

    [JsonPropertyName("cms_admin_password")]
    public string CmsAdminPassword { get; set; }

    [JsonPropertyName("cms_login_url")]
    public string CmsLoginUrl { get; set; }

    [JsonPropertyName("cms_upload_url")]
    public string CmsUploadUrl { get; set; }

    [JsonPropertyName("login_input_username_selector")]
    public string LoginInputUsernameSelector { get; set; }

    [JsonPropertyName("login_input_password_selector")]
    public string LoginInputPasswordSelector { get; set; }

    [JsonPropertyName("login_button_selector")]
    public string LoginButtonSelector { get; set; }

    [JsonPropertyName("upload_button_selector")]
    public string UploadButtonSelector { get; set; }

    [JsonPropertyName("upload_videos_directory")]
    public string UploadVideosDirectory { get; set; }
}

