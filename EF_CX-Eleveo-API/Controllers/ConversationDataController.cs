using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eleveo_EFCX_Connector_API.Data;
using Eleveo_EFCX_Connector_API.Dto;
using Eleveo_EFCX_Connector_API.Contracts;
using Eleveo_EFCX_Connector_API.Exceptions;
using Eleveo_EFCX_Connector_API.Response;

namespace Eleveo_EFCX_Connector_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationDataController : ControllerBase
    {
        private readonly IEleveoConnector _eleveoConnector;
        private readonly IHelper _helper;

        public ConversationDataController(IEleveoConnector eleveoConnector, IHelper helper)
        {
            _eleveoConnector = eleveoConnector;
            _helper = helper;
        }

        // GET: api/ConversationData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConversationDataDto>>> GetConversation_Data()
        {
            var allconversationdata = await _eleveoConnector.GetAllConversationDataAsync();
          if (allconversationdata == null)
          {
              return NotFound();
          }
            return Ok(allconversationdata);
        }

        // GET: api/ConversationData/5
        [HttpGet("{JtapiId}")]
        public async Task<ActionResult<Stream>> GetConversationDataDto(string JtapiId)
        {
            int value;
            if(!int.TryParse(JtapiId,out value))
            {
                var notfoundobj = new NotFoundObj()
                {
                    Description = $"DialogID is not in correct format: {JtapiId}",
                    status = "WRONG_INPUT_FORMAT"
                };
                return BadRequest(notfoundobj);
            }
            var conversationdata = await _eleveoConnector.GetConversationDataAsync(JtapiId);


            Stream finalaudiostream = null;
            Stream singlemerged = null;
            List<Stream> ListofStream = new List<Stream>();
            try
            {
                if (conversationdata == null)
                {
                    var notfoundobj = new NotFoundObj()
                    {
                        DialogId = Convert.ToInt32(JtapiId),
                        Description = $"Conversation data not found for DialogID: {JtapiId}",
                        status = "NOT_FOUND"
                    };
                    return NotFound(notfoundobj);
                }
                //GO TO GET JSESSION ID FROM ELEVEO USING LOGIN PARAMS
                var jsessionid = await _eleveoConnector.GetJSessionIdAsync();
                if (string.IsNullOrEmpty(jsessionid))
                {
                    throw new ApiError($"JSessionId could not be fetched for DialogID: {JtapiId}.");
                }
                _helper.Logit($"JSessionId fetched successfully for DialogID: {JtapiId}");


                //THIS METHOD RETURNS AUDIO STREAM FOR A LIST OF RECORDING EVENTS FOR ONE CONVERSATION
                conversationdata.ForEach(SidObj =>
                {
                    string? Sid = SidObj.CallRec_Sid;
                    _helper.Logit($"Fetching download token for SID: {Sid}");
                    var downlloadtoekn = _eleveoConnector.GetDownloadToken(jsessionid, Sid);
                    if (downlloadtoekn == string.Empty)
                    {                      
                        throw new ApiError($"Eleveo download token could not be fetched for DialogID: {JtapiId}");
                    }
                    _helper.Logit($"Download token fetched successfully for audio part SID: {Sid}");


                    //GET THE AUDIO USING JTAPI OR DOWNLOAD TOKEN
                    _helper.Logit($"Fetching Audio part for SID: {Sid}");
                    var audiofile = _eleveoConnector.GetRecordingWavByDToken(jsessionid, downlloadtoekn);
                    if (audiofile is null)
                    {
                        throw new ApiError($"Empty Audio recording returned for SID: {Sid}. Exiting...");
                    }
                    ListofStream.Add(audiofile);
                    _helper.Logit($"Audio part fetched successfully for part SID: {Sid}");
                });
               

                _helper.Logit($"All audio parts fetched successfully.");

               
                _helper.Logit($"Converting audio parts to stream.");
                //CONVERT AUDIO STREAM 
                var listoOfAudioFiles =_eleveoConnector.ConvertStreamList(ListofStream);
                if (listoOfAudioFiles.Count == 0)
                {
                    throw new ApiError($"Could not convert the Audio streams to playable files. Check Logs for more information. Exiting...");
                }
                _helper.Logit($"Each audio part converted to playable files successfully. Going to merge all audio party");
                //MERGING AUDIO TO ONE
                singlemerged = _eleveoConnector.MergeAudioParts(listoOfAudioFiles);
                if (singlemerged.Length == 0)
                {
                    throw new ApiError($"Error occured while merging the audio parts. Check the log for details");
                }
                _helper.Logit($"Audio parts merged successfully. Sending to EFCX");

                finalaudiostream = singlemerged;
                if (finalaudiostream.Length==0)
                {
                    var notfoundobj = new NotFoundObj()
                    {
                        DialogId = Convert.ToInt32(JtapiId),
                        Description = $"Merged Audio Stream returned empty for DialogID: {JtapiId}. Audio File not found",
                        status = "NOT_FOUND"
                    };
                    return NotFound(notfoundobj);
                }
                _helper.Logit($"Audio Stream fetched successfully for DialogID: {JtapiId}");

            }
            catch (NoConfigException ex)
            {
                _helper.Logit($"{ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (ApiError apierr)
            {
                var notfoundobj = new NotFoundObj()
                {
                    DialogId = Convert.ToInt32(JtapiId),
                    Description = apierr.Message,
                    status = "NOT_FOUND"
                };
                return NotFound(notfoundobj);
            }
            catch (Exception ex)
            {
                _helper.Logit($"{ex.Message}");
            }
            return Ok(finalaudiostream);
        }

        //// PUT: api/ConversationData/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutConversationDataDto(int id, ConversationDataDto conversationDataDto)
        //{
        //    if (id != conversationDataDto.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(conversationDataDto).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ConversationDataDtoExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/ConversationData
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<ConversationDataDto>> PostConversationDataDto(ConversationDataDto conversationDataDto)
        //{
        //  if (_context.Conversation_Data == null)
        //  {
        //      return Problem("Entity set 'EleveoEFCXDBContext.Conversation_Data'  is null.");
        //  }
        //    _context.Conversation_Data.Add(conversationDataDto);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetConversationDataDto", new { id = conversationDataDto.Id }, conversationDataDto);
        //}

        //// DELETE: api/ConversationData/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteConversationDataDto(int id)
        //{
        //    if (_context.Conversation_Data == null)
        //    {
        //        return NotFound();
        //    }
        //    var conversationDataDto = await _context.Conversation_Data.FindAsync(id);
        //    if (conversationDataDto == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Conversation_Data.Remove(conversationDataDto);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private async Task<bool> ConversationDataDtoExists(string JtapiId)
        {
            return await _eleveoConnector.RecordingExists(JtapiId);
        }
    }
}
