namespace UMS.Features;

public static class ConfigUtil
{
    public static (string Username, string Password, string Host, int Port) ParseSmtpRelayConfig(string smtpRelayConfig)
    {
        if (Uri.TryCreate(smtpRelayConfig, UriKind.Absolute, out var uri))
        {
            string username = string.Empty;
            string password = string.Empty;
            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                string[] userInfoParts = uri.UserInfo.Split(':');
                username = Uri.UnescapeDataString(userInfoParts[0]);
                password = Uri.UnescapeDataString(userInfoParts[1]);
            }

            int port = uri.Port;
            string host = uri.Host;

            return (username, password, host, port);
        }

        return (string.Empty, string.Empty, string.Empty, 0);
    }
}