using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
            public bool getContext() { if (this.contextcode == null) { return false; } return true; }

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
        public string replacespecialchars(string StrinInit)
        {
            string newstring = StrinInit;
            newstring = newstring.Replace("?", "");
            return newstring;
        }

            ////////////////////////////////////////////////////
            ////////////////////////////////////////////////////
            /// Download Images If Not Exists
            ////////////////////////////////////////////////////
            public bool downloadAnImage(string filename, string imageUrl, bool HiresBool)
            {
                string[] tmpformatstring = imageUrl.Split('.');
                ImageFormat format;

            if (imageUrl.Substring(0, 4) != "http") { return false; }

                if (File.Exists(filename + "." + tmpformatstring[tmpformatstring.Length - 1])) { return true; }
                WebClient client = new WebClient();

            Stream stream;
            try
            {
                stream = client.OpenRead(imageUrl);
            }
            catch (WebException) { return false; }
            catch (Exception) { return false; }

            Bitmap bitmap;
            try
            {
                bitmap = new Bitmap(stream);
            } catch(ArgumentException)
            {
                stream.Flush();
                stream.Close();
                client.Dispose();
                return false;
            }

            string tmpendingforfile = tmpformatstring[tmpformatstring.Length - 1];

            bool foundendinginstring = tmpendingforfile.IndexOf("jpg") != -1;
            if(foundendinginstring) { tmpendingforfile = "jpg"; }

             foundendinginstring = tmpendingforfile.IndexOf("jpeg") != -1;
            if (foundendinginstring) { tmpendingforfile = "jpeg"; }

            foundendinginstring = tmpendingforfile.IndexOf("bmp") != -1;
            if (foundendinginstring) { tmpendingforfile = "bmp"; }

            foundendinginstring = tmpendingforfile.IndexOf("gif") != -1;
            if (foundendinginstring) { tmpendingforfile = "gif"; }

            foundendinginstring = tmpendingforfile.IndexOf("ico") != -1;
            if (foundendinginstring) { tmpendingforfile = "ico"; }

            foundendinginstring = tmpendingforfile.IndexOf("JPG") != -1;
            if (foundendinginstring) { tmpendingforfile = "jpg"; }

            foundendinginstring = tmpendingforfile.IndexOf("JPEG") != -1;
            if (foundendinginstring) { tmpendingforfile = "jpeg"; }

            foundendinginstring = tmpendingforfile.IndexOf("BMP") != -1;
            if (foundendinginstring) { tmpendingforfile = "bmp"; }

            foundendinginstring = tmpendingforfile.IndexOf("GIF") != -1;
            if (foundendinginstring) { tmpendingforfile = "gif"; }

            foundendinginstring = tmpendingforfile.IndexOf("ICO") != -1;
            if (foundendinginstring) { tmpendingforfile = "ico"; }

            if (bitmap != null) { 
                    if(HiresBool)
                        {
                            if(bitmap.Height > 400 || bitmap.Width > 400)
                            {
                                bitmap.Save(filename + "." + tmpendingforfile, bitmap.RawFormat);
                            } else {
                                stream.Flush();
                                stream.Close();
                                client.Dispose();
                                return false;
                            }
                        } else  {
                            bitmap.Save(filename + "." + tmpendingforfile, bitmap.RawFormat);
                        }
                   
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<string> getImageLinks(String URLDomain)
        {
            List<string> tmpsitearray = new List<string>();

            string linksearchvar = "src=\"";
            string linkendvar = "\"";

            string currentstring = this.contextcode;

            while (currentstring.IndexOf(linksearchvar) != -1)
            {
                currentstring = currentstring.Substring(currentstring.IndexOf(linksearchvar) + linksearchvar.Length);

                string tmplinkstring = currentstring;
                tmplinkstring = tmplinkstring.Substring(0, tmplinkstring.IndexOf(linkendvar) - linkendvar.Length);
                currentstring = currentstring.Substring(currentstring.IndexOf(linkendvar));


                if (tmplinkstring.Substring(0, 4) == "http")
                {
                    tmpsitearray.Add(tmplinkstring);
                }
                else
                {
                    if (tmplinkstring.Length > 4)
                    {
                        tmpsitearray.Add(URLDomain + tmplinkstring);
                    }
                }
            }


            return tmpsitearray;
        }

        ////////////////////////////////////////////////////
        ////////////////////////////////////////////////////
        /// GET SITE LINKS FROM CONTEXT
        ////////////////////////////////////////////////////
        public List<string> getSiteLinks(String LastSearchString, String URLDomain)
        {
            List<string> tmpsitearray = new List<string>();
            tmpsitearray.Add(LastSearchString);
            
            string linksearchvar = "<a href=\"";
            string linkendvar    = "\"";

            string currentstring = this.contextcode;

            while (currentstring.IndexOf(linksearchvar) != -1) {
               currentstring = currentstring.Substring(currentstring.IndexOf(linksearchvar) + linksearchvar.Length);

               string tmplinkstring = currentstring;
               tmplinkstring = tmplinkstring.Substring(0, tmplinkstring.IndexOf(linkendvar) - linkendvar.Length);
               currentstring = currentstring.Substring(currentstring.IndexOf(linkendvar));

                if (tmplinkstring != null && tmplinkstring.Length > 5)
                {
                    if (tmplinkstring.Substring(0, 4) == "http")
                    {
                        tmpsitearray.Add(tmplinkstring);
                    }
                    else
                    {
                        if (tmplinkstring.Length > 4)
                        {
                            tmpsitearray.Add(URLDomain + tmplinkstring);
                        }
                    }
                }
            }
            return tmpsitearray;
        }

    }
}