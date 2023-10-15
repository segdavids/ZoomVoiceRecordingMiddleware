using Eleveo_EFCX_Connector_API.Contracts;
using Eleveo_EFCX_Connector_API.Data;
using Eleveo_EFCX_Connector_API.Dto;
using Eleveo_EFCX_Connector_API.Exceptions;
using Eleveo_EFCX_Connector_API.Response;
using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using RestSharp;
using System.Net;

namespace Eleveo_EFCX_Connector_API.Repository
{
    public class EleveoConnector : IEleveoConnector
    {
        private readonly EleveoEFCXDBContext _context;
        private readonly IHelper _helper;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EleveoConnector(EleveoEFCXDBContext context, IHelper helper, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _helper = helper;
            _hostEnvironment = hostEnvironment;
        }
        public Task<RecordingFile> DecodeRecordingAsync(RecordingFile recording)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ConversationDataDto>> GetAllConversationDataAsync()
        {
            return await _context.Set<ConversationDataDto>().ToListAsync();
        }

        public string GetDownloadToken(string jsessionid, string sid)
        {
            string? downloadtoekn = string.Empty;
            try
            {
                var env = _helper.GetEnv();
                if ((env.eleveoAuthURL is not null) && (!string.IsNullOrEmpty(jsessionid)) && (!string.IsNullOrEmpty(sid)))
                {
                    var resourceurl = $"callrec/downloadtoken;jsessionid={jsessionid}?sid={sid}&type=1&action=download";
                    //var resourceurl = $"/callrec/downloadtoken;jsessionid={jsessionid}?sid={sid}:metadata&type=1&action=download";
                    RestClient client = new RestClient(env.eleveoSearchURL);
                    var baseurl = client.Options.BaseUrl;
                    var restrequest = new RestRequest(baseurl + resourceurl, RestSharp.Method.Get)
                    {
                        RequestFormat = DataFormat.Json,
                    };
                    var response = client.Execute<string>(restrequest);
                    if ((response.StatusCode == System.Net.HttpStatusCode.OK) && (response.Content is not null))
                    {
                        var respcontent = response.Content;
                        if (respcontent.Contains("<reply>"))
                        {
                            downloadtoekn = response.Content.Substring(47, respcontent.Length - 56);
                            _helper.Logit(downloadtoekn);
                        }
                    }
                }
                else
                {
                    throw new NoConfigException($"Configuration files are missing : Auth:{env.eleveoAuthURL} , Jsession:{jsessionid} , sid{sid}");
                }
            }
            catch (NoConfigException ex)
            {
                _helper.Logit($"{ex.Message}");
            }
            catch (Exception e)
            {
                _helper.Logit($"{e.Message}");
            }
            return downloadtoekn;
        }

        public async Task<string> GetJSessionIdAsync()
        {
            string jsessionid = null;
            try
            {
                //GET RECORDINGS FROM ELEVEO              

                var eleveouser = _helper.GetEleveoUser();
                var env = _helper.GetEnv();
                var eleveosearchURL = $"{env.eleveoSearchURL}";
                var resourceurl = $"callrec/loginservlet?loginname={eleveouser.username}&password={eleveouser.password}";
                RestClient client = new RestClient(env.eleveoSearchURL);
                var baseurl = client.Options.BaseUrl;
                var restrequest = new RestRequest(baseurl + resourceurl, RestSharp.Method.Get)
                {
                    RequestFormat = DataFormat.Json,
                };
                _helper.Logit($"Going to get JsessionId");
                var jsresp = await client.ExecuteAsync<string>(restrequest);
                if (jsresp == null)
                {
                    _helper.Logit($"JSessionID resp is null");
                    return null;
                }
                if ((jsresp.StatusCode == System.Net.HttpStatusCode.OK) && (jsresp.Content is not null))
                {
                    var respcontent = jsresp.Content.ToString().Trim();
                    if (respcontent.Contains("<ok sessionid"))
                    {
                        jsessionid = respcontent.Substring(55, respcontent.Length - 94);
                        _helper.Logit($"{jsessionid}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return jsessionid ?? string.Empty;
        }

        public async Task<List<ConversationDataDto>> GetConversationDataAsync(string? JtapiId)
        {
            var convobject = _context.Conversations.Where(conv => conv.JtapiId.ToString() == JtapiId).FirstOrDefault();
            if (convobject == null)
            {
                return null;
            }
            var conversatnId = convobject.conversation_Id;
            return await _context.
                Conversation_Data.
                Where(conversation => conversation.conversation_Id == conversatnId).
                ToListAsync();
        }

        public async Task<bool> RecordingExists(string JtapiId)
        {
            return await GetConversationDataAsync(JtapiId) != null;
        }

        public Stream GetRecordingWavByDToken(string jsessionId, string downloadtoken)
        {
            //GET ZIP CALL EVENT FILES FROM RECORDING SEERVER
            //1. GET JSESSION ID
            //2. GET DOWNLOAD TOKEN
            //3. GET ZIP FFILE

            Stream receiveStream = null;
            try
            {

                var env = _helper.GetEnv();
                // var path = "C:\\EF\\";// Path.Combine(_hostEnvironment.ContentRootPath, "Files");
                //var fullpath = path + $"{DateTime.Now.ToUniversalTime():dd_MM_yyyy}.zip";

                if ((env is not null) && (jsessionId is not null) && (downloadtoken is not null))
                {
                    var resourceurl = $"/callrec/sendcallfile.mp3;jsessionid={jsessionId}?token={downloadtoken}";
                    RestClient client = new RestClient(env.eleveoSearchURL);
                    var baseurl = client.Options.BaseUrl;
                    var request = WebRequest.CreateHttp(baseurl + resourceurl);
                    HttpWebResponse rps = (HttpWebResponse)request.GetResponse();
                    receiveStream = rps.GetResponseStream();

                    //IF YOU WANT TO SAVE THE RECORDINGS TO A FILE

                    //byte[] buffer = new byte[32768];
                    //using (FileStream fileStream = File.Create(fullpath))
                    //{
                    //    while (true)
                    //    {
                    //        int read = receiveStream.Read(buffer, 0, buffer.Length);
                    //        if (read <= 0)
                    //            break;
                    //        fileStream.Write(buffer, 0, read);
                    //    }
                    //}
                }
                else
                {
                    throw new NoConfigException("Configuration files are missing");
                }
            }
            catch (NoConfigException ex)
            {
                _helper.Logit($"{ex.Message}");
            }
            catch (Exception ex)
            {
                _helper.Logit($"{ex.Message}");
            }
            return receiveStream;
        }

        public Stream GetRecordingWavByJTapi(string jsessionId, string JtapiId)
        {
            //GET wav FILES FROM RECORDING SEERVER
            Stream receiveStream = null;
            try
            {
                var env = _helper.GetEnv();
                //var path = "C:\\EF\\";// Path.Combine(_hostEnvironment.ContentRootPath, "Files");
                //var fullpath = path + $"{DateTime.Now.ToUniversalTime():dd_MM_yyyy}.wav";

                if ((env is not null) && (jsessionId is not null))
                {
                    var resourceurl = $"/callrec/audiodata;jsessionid={jsessionId}?externalData=JTAPI_CISCO_ID$!${JtapiId}";
                    //var resourceurl = $"/callrec/sendcallfile.mp3;jsessionid={jsessionId}?token={JtapiId}";

                    RestClient client = new RestClient(env.eleveoSearchURL);
                    var baseurl = client.Options.BaseUrl;
                    var request = WebRequest.CreateHttp(baseurl + resourceurl);
                    HttpWebResponse rps = (HttpWebResponse)request.GetResponse();
                    receiveStream = rps.GetResponseStream();
                    //IF YOU WANT TO SAVE THE RECORDINGS TO A FILE

                    //byte[] buffer = new byte[32768];
                    //using (FileStream fileStream = File.Create(fullpath))
                    //{
                    //    while (true)
                    //    {
                    //        int read = receiveStream.Read(buffer, 0, buffer.Length);
                    //        if (read <= 0)
                    //            break;
                    //        fileStream.Write(buffer, 0, read);
                    //    }
                    //}
                }
                else
                {
                    throw new NoConfigException("Configuration files are missing");
                }
            }
            catch (NoConfigException ex)
            {
                _helper.Logit($"{ex.Message}");
            }
            return receiveStream;
        }

        public Stream MergeAudioParts(List<string> filewavparts)
        {
            var templocation = _hostEnvironment.ContentRootPath;
            List<AudioFileReader> audioreaderliust = new();
            _helper.Logit($"File Path => {templocation}");
            var mergedaudiofilename = $"{templocation}/{DateTime.Now:yyyyMMddHHmm}_Merged.mp3";
            var stream = new MemoryStream();
            try
            {
                using (var fs = File.OpenWrite(mergedaudiofilename))
                {
                    filewavparts.ForEach(part =>
                {
                    //var reader = new AudioFileReader($"{part}");
                    //audioreaderliust.Add(reader);


                    //USING BUFFER
                    var buffer = File.ReadAllBytes(part);
                    fs.Write(buffer, 0, buffer.Length);
                });
                }
                //var playlist = new ConcatenatingSampleProvider(audioreaderliust);               
                //WaveFileWriter.CreateWaveFile16(mergedaudiofilename,playlist);
                _helper.Logit($"Files merged successfully. Converting to Audio Stream...");

                var audiostream = GetMP3Stream(mergedaudiofilename);
                stream = (MemoryStream)audiostream;
                _helper.Logit($"Running clean up...");
                Task.Run(() => DeleteUsedFiles(templocation));
                //_helper.Logit($"Sending to EFCX...");

            }
            catch (Exception ex)
            {
                _helper.Logit(ex.Message);
                return stream;

            }
            return stream;
        }

        public void DeleteUsedFiles(string filepath)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(filepath);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
        }

        public Stream GetStream4RecordingList(List<ConversationDataDto> conversatonDataList, string Jsessionid)
        {
            throw new NotImplementedException();
        }


        public List<string> ConvertStreamList(List<Stream> streamlist)
        {
            List<string> AudioFilePaths = new();
            var filedirecroty = _hostEnvironment.ContentRootPath;
            var filecount = 0;
            try
            {
                //IF YOU WANT TO SAVE THE RECORDINGS TO A FILE

                streamlist.ForEach(audiopartstream =>
                {
                    var filefullpath = $"{filedirecroty}/{DateTime.Now:yyyyMMddHHmm}_Audiopart_{filecount}.mp3";
                    byte[] buffer = new byte[32768];
                    using (FileStream fileStream = File.Create(filefullpath))
                    {
                        while (true)
                        {
                            int read = audiopartstream.Read(buffer, 0, buffer.Length);
                            if (read <= 0)
                                break;
                            fileStream.Write(buffer, 0, read);
                        }
                    }
                    AudioFilePaths.Add(filefullpath);
                    filecount++;
                });
            }
            catch (Exception ex)
            {
                _helper.Logit(ex.Message);
            }
            return AudioFilePaths;
        }

        //public File GetAudio()
        //{
        //    var bytes = new byte[0];


        //    using (var fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read))
        //    {
        //        var br = new BinaryReader(fs);
        //        long numBytes = new FileInfo(fileLocation).Length;
        //        buff = br.ReadBytes((int)numBytes);


        //        return File(buff, "audio/mpeg", "callrecording.mp3");
        //    }
        //}
        public  Stream GetMP3Stream(string filepath)
        {
            byte[] fileData;
          
            using (FileStream fs = System.IO.File.OpenRead(filepath))
            {
                using (BinaryReader binaryReader = new BinaryReader(fs))
                {
                    fileData = binaryReader.ReadBytes((int)(fs.Length));
                }
            }
            MemoryStream stream = new MemoryStream(fileData);
            return stream;
            //return new FileStreamResult(stream, contentType: new MediaTypeHeaderValue("audio/mpeg").MediaType)
            //{
            //    EnableRangeProcessing = true
            //};
        }
    }
}
