using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using DddsUtils.Logging.NetStandard;
using NLog;
using emri_service.Managers;
using emri_models.Dto;
using emri_repository;

namespace emri_service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CDIAlertController : ControllerBase
    {
        private readonly IBusControl _busControl;
        private readonly ILogger _logger;
        private readonly ICDIAlertManager _cdiAlertManager;

        public CDIAlertController(IBusControl busControl
            ,ILogFactory logFactory
            ,ICDIAlertManager cdiAlertManager
            )
        {
            _busControl = busControl;
            _logger = logFactory.GetLogger("CDIAlertController");
            _cdiAlertManager = cdiAlertManager;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(CDIAlertResponseDTO alertResponseDTO)
        {
            var cdi = _cdiAlertManager.ValidateCDIAlertEntityID(alertResponseDTO.CDIAlertEntityID).Result;
            if(cdi.Count > 0 )
            {
                return BadRequest(cdi.FirstOrDefault().Value);
            }

            int trackingID = _cdiAlertManager.InsertEMRTrackingRecord(alertResponseDTO);
            if(trackingID == 0)
            {
                return BadRequest("Something went wrong");
            }

            return Created(string.Empty, new { id = trackingID });
        }
    }
}
