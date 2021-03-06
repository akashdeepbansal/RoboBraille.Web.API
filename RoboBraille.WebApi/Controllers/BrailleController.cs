﻿using RoboBraille.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RoboBraille.WebApi.Controllers
{
    public class BrailleController : ApiController
    {
      private readonly IRoboBrailleJob<BrailleJob> _repository;

        public BrailleController()
        {
            _repository = new BrailleRepository();
        }

        public BrailleController(IRoboBrailleJob<BrailleJob> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get a list of translation tables used to brute force the web api to use liblouis
        /// </summary>
        /// <returns>A list of translation tables.</returns>
        [AllowAnonymous]
        [Route("api/braille/gettranslationtables")]
        public IEnumerable<string> GetTranslationTables()
        {
            return LouisFacade.GetTranslationTables();
        }

        /// <summary>
        /// GET the installed braille languages
        /// </summary>
        /// <returns>A list of installed languages</returns>
        [AllowAnonymous]
        [Route("api/braille/getlangs")]
        public IEnumerable<string> GetLangs()
        {
            //current supported langs are:
            IEnumerable<string> supportedLangs = new List<string>()
            {
                Enum.GetName(typeof(Language),Language.enUS),
                Enum.GetName(typeof(Language),Language.enGB),
                Enum.GetName(typeof(Language),Language.bgBG),
                Enum.GetName(typeof(Language),Language.czCZ),
                Enum.GetName(typeof(Language),Language.daDK),
                Enum.GetName(typeof(Language),Language.frFR),
                Enum.GetName(typeof(Language),Language.deDE),
                Enum.GetName(typeof(Language),Language.elGR),
                Enum.GetName(typeof(Language),Language.huHU),
                Enum.GetName(typeof(Language),Language.isIS),
                Enum.GetName(typeof(Language),Language.itIT),
                Enum.GetName(typeof(Language),Language.nbNO),
                Enum.GetName(typeof(Language),Language.plPL),
                Enum.GetName(typeof(Language),Language.ptPT),
                Enum.GetName(typeof(Language),Language.roRO),
                Enum.GetName(typeof(Language),Language.skSK),
                Enum.GetName(typeof(Language),Language.slSI),
                Enum.GetName(typeof(Language),Language.esES)
            };

            return supportedLangs;
        }

        [AllowAnonymous]
        [Route("api/braille/getcontractions")]
        public IEnumerable<string> GetContractions()
        {
            return Enum.GetNames(typeof(BrailleContraction));
        }

        /// <summary>
        /// POST a new BrailleJob.
        /// </summary>
        /// <param name="job">The BrailleJob class parameter</param>
        /// <returns>A unique job ID</returns>
        [Authorize]
        [Route("api/braille")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Post(BrailleJob job)
        {
            try
            {
                Guid userId = RoboBrailleProcessor.getUserIdFromJob(this.Request.Headers.Authorization.Parameter);
                job.UserId = userId;
                Guid jobId = await _repository.SubmitWorkItem(job);
                return Ok(jobId.ToString("D"));
            }
            catch (Exception e)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(string.Format("Internal error: {0}", e)),
                    ReasonPhrase = e.Message
                };
                throw new HttpResponseException(resp);
            }
        }

        /// <summary>
        /// GET braille job status
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>A status code representing the job's state</returns>
        [Route("api/braille/getstatus")]
        [ResponseType(typeof(string))]
        public Task<string> GetJobStatus([FromUri] Guid jobId)
        {
            int status = _repository.GetWorkStatus(jobId);
            return Task.FromResult(Convert.ToString(status));
        }

        /// <summary>
        /// GET conversion result
        /// </summary>
        /// <param name="jobId">The GUID job ID</param>
        /// <returns>The file result</returns>
        [Route("api/braille/getresult")]
        [ResponseType(typeof(FileResult))]
        public Task<IHttpActionResult> GetJobResult([FromUri] Guid jobId)
        {
            var fr = _repository.GetResultContents(jobId);
            if (fr == null)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            return Task.FromResult<IHttpActionResult>(fr);
        }

        /// <summary>
        /// Delete a job that you published. You must be the owner of the job.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/braille")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete([FromUri] Guid jobId)
        {
            try
            {
                Guid userId = RoboBrailleProcessor.getUserIdFromJob(this.Request.Headers.Authorization.Parameter);
                jobId = await RoboBrailleProcessor.DeleteJobFromDb(jobId, userId, _repository.GetDataContext());
                return Ok(jobId.ToString("D"));
            }
            catch (Exception e)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(string.Format("Internal error: {0}", e)),
                    ReasonPhrase = e.Message
                };
                throw new HttpResponseException(resp);
            }
        }
    }
}