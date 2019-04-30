using MyTools.Desktop.Updater;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace MyTools.Desktop.App.Helpers
{
    public static class UpdateHelper
    {
        public static bool HasUpdates(Assembly assembly, string updateCheckUrl)
        {
            try
            {
                var updateVersion = GetUpdateInformation(updateCheckUrl);
                if (updateVersion == null)
                {
                    return false;
                }

                int latestVerstion = Convert.ToInt32(updateVersion.Version.Replace(".", string.Empty));

                var version = assembly.GetName().Version;

                int currentVersion = Convert.ToInt32($"{version.Major}{version.Minor}{version.Build}{version.Revision}");

                if (currentVersion < latestVerstion)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public static UpdateInformation GetUpdateInformation(string updateCheckUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(updateCheckUrl))
                {
                    throw new ArgumentNullException(nameof(updateCheckUrl));
                }

                string json = CreateHttpRequest(updateCheckUrl);

                var updateVersion = !string.IsNullOrWhiteSpace(json) ? JsonConvert.DeserializeObject<UpdateInformation>(json) : null;
                if (updateVersion == null)
                {
                    return null;
                }

                return updateVersion;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool HasDownloadedFile(UpdateInformation updateInformation, UpdateFile file)
        {
            try
            {
                if(updateInformation == null)
                {
                    throw new ArgumentNullException(nameof(updateInformation));
                }

                if(file == null)
                {
                    throw new ArgumentNullException(nameof(file));
                }

                var url = $"{updateInformation.DownloadUrl}{updateInformation.Version}/{file.Name}";

                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri(url), file.Name);

                    File.SetAttributes(file.Name, FileAttributes.Normal);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static string CreateHttpRequest(string url)
        {
            var request = WebRequest.CreateHttp(url);

            var timeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

            request.Timeout = timeout;
            request.ContinueTimeout = timeout;
            request.ReadWriteTimeout = timeout;

            var response = request.GetResponse();

            using (var data = response.GetResponseStream())
            {
                if (data == null)
                {
                    return null;
                }

                using (var reader = new StreamReader(data, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
