﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareX.UploadersLib.FileUploaders
{
    public class Pomf : FileUploader
    {
        public static List<PomfUploader> Uploaders = new List<PomfUploader>()
        {
            new PomfUploader("http://1339.cf/upload.php", "http://b.1339.cf"),
            // new PomfUploader("https://bucket.pw/upload.php", "https://dl.bucket.pw"),
            new PomfUploader("http://catgirlsare.sexy/upload.php"),
            new PomfUploader("http://cuntflaps.me/upload.php", "http://a.cuntflaps.me"),
            // new PomfUploader("https://fuwa.se/api/upload"),
            new PomfUploader("http://g.zxq.co/upload.php", "http://y.zxq.co"),
            new PomfUploader("http://kyaa.sg/upload.php", "https://r.kyaa.sg"),
            // new PomfUploader("https://madokami.com/upload"),
            // new PomfUploader("http://matu.red/upload.php", "http://x.matu.red"),
            new PomfUploader("https://maxfile.ro/static/upload.php", "https://d.maxfile.ro"),
            new PomfUploader("https://mixtape.moe/upload.php"),
            new PomfUploader("http://nigger.cat/upload.php"),
            // new PomfUploader("http://nyanimg.com/upload.php"),
            new PomfUploader("http://openhost.xyz/upload.php"),
            // new PomfUploader("https://pantsu.cat/upload.php"),
            new PomfUploader("https://pomf.cat/upload.php", "http://a.pomf.cat"),
            new PomfUploader("http://pomf.hummingbird.moe/upload.php", "http://a.pomf.hummingbird.moe"),
            new PomfUploader("http://pomf.io/upload.php"),
            new PomfUploader("http://pomf.pl/upload.php"),
            // new PomfUploader("https://pomf.se/upload.php", "https://a.pomf.se"),
            // new PomfUploader("https://sheesh.in/upload.php"),
            new PomfUploader("http://up.che.moe/upload.php", "http://cdn.che.moe")
        };

        public PomfUploader Uploader { get; private set; }

        public Pomf(PomfUploader uploader)
        {
            Uploader = uploader;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (Uploader == null || string.IsNullOrEmpty(Uploader.UploadURL))
            {
                Errors.Add(Resources.Pomf_Upload_Please_select_one_of_the_Pomf_uploaders_from__Destination_settings_window____Pomf_tab__);
                return null;
            }

            UploadResult result = UploadData(stream, Uploader.UploadURL, fileName, "files[]");

            if (result.IsSuccess)
            {
                PomfResponse response = JsonConvert.DeserializeObject<PomfResponse>(result.Response);

                if (response.success && response.files != null && response.files.Count > 0)
                {
                    string url = response.files[0].url;

                    if (!string.IsNullOrEmpty(Uploader.ResultURL))
                    {
                        url = URLHelpers.CombineURL(Uploader.ResultURL, url);
                    }

                    result.URL = url;
                }
            }

            return result;
        }

        public static string TestClones()
        {
            List<string> successful = new List<string>();
            List<string> failed = new List<string>();

            foreach (PomfUploader uploader in Uploaders)
            {
                try
                {
                    Pomf pomf = new Pomf(uploader);

                    byte[] bytes = Encoding.UTF8.GetBytes("Test");
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        UploadResult result = pomf.Upload(ms, "Test.txt");

                        if (result != null && result.IsSuccess && !string.IsNullOrEmpty(result.URL))
                        {
                            successful.Add(uploader.ToString());
                        }
                        else
                        {
                            failed.Add(uploader.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                    failed.Add(uploader.ToString());
                }
            }

            return string.Format("Successful uploads ({0}):\r\n\r\n{1}\r\n\r\nFailed uploads ({2}):\r\n\r\n{3}",
                successful.Count, string.Join("\r\n", successful), failed.Count, string.Join("\r\n", failed));
        }

        private class PomfResponse
        {
            public bool success { get; set; }
            public object error { get; set; }
            public List<PomfFile> files { get; set; }
        }

        private class PomfFile
        {
            public string hash { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string size { get; set; }
        }
    }
}