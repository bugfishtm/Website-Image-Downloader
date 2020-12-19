using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace wib
{
    class ImageDownloader
    {
        ////////////////////////////////////////////////////
        ////////////////////////////////////////////////////
        /// Reset Search Vars
        ////////////////////////////////////////////////////
            string contextcode = null;
            public void ResetVars() { this.contextcode = null;  }
            public bool getContext() { if (this.contextcode.Equals(null)) { return false; } return true; }

        ////////////////////////////////////////////////////
        ////////////////////////////////////////////////////
        /// Get a Complete HTML Website as String
        ////////////////////////////////////////////////////
            public void getHtmlUrlAsString(String URL)
            {
                if (URL == null) { return; }
                HttpWebRequest request;
                HttpWebResponse response;
                    try
                    {
                        request = (HttpWebRequest)WebRequest.Create(URL);
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    catch (WebException) { return; }
                    catch (Exception) { return; }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream;
                    if (String.IsNullOrWhiteSpace(response.CharacterSet))
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    this.contextcode = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                    return;
                }

                response.Close();
                return;
            }

        ////////////////////////////////////////////////////
        ////////////////////////////////////////////////////
        /// Download Images If Not Exists
        ////////////////////////////////////////////////////
            public bool downloadAnImage(string filename, string imageUrl)
            {
                string[] tmpformatstring = imageUrl.Split('.');
                ImageFormat format;

            switch (tmpformatstring[tmpformatstring.Length - 1])
            {
                case "jpg": case "JPG": format = ImageFormat.Jpeg; break;
                case "png": case "PNG": format = ImageFormat.Png; break;
                case "jpeg": case "JPEG": format = ImageFormat.Jpeg; break;
                case "gif": case "GIF": format = ImageFormat.Gif; break;
                case "bmp": case "BMP": format = ImageFormat.Bmp; break;
                case "tiff": case "TIFF": format = ImageFormat.Tiff; break;
                case "ico": case "ICO": format = ImageFormat.Icon; break;
                default: return false;
            };

            if (imageUrl.Substring(0, 4) != "http") { return false; }

                if (File.Exists(filename + "." + tmpformatstring[tmpformatstring.Length - 1])) { return true; }
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(imageUrl);
                Bitmap bitmap; bitmap = new Bitmap(stream);

                if (bitmap != null) { 
                    bitmap.Save(filename + "." + tmpformatstring[tmpformatstring.Length - 1], format);
                    stream.Flush();
                    stream.Close();
                    client.Dispose();
                    return true; }
                else {
                    stream.Flush();
                    stream.Close();
                    client.Dispose();
                    return false; }
            }

        ////////////////////////////////////////////////////
        ////////////////////////////////////////////////////
        /// GET IMAGE LINKS FROM CONTEXT
        ////////////////////////////////////////////////////
        public List<string> getImageLinks()
        {
            return null;
        }

        ////////////////////////////////////////////////////
        ////////////////////////////////////////////////////
        /// GET SITE LINKS FROM CONTEXT
        ////////////////////////////////////////////////////
        public List<string> getSiteLinks(String LastSearchString)
        {
            return null;
        }

    }
}