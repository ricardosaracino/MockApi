using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MockApi.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Policy = "MockApi")]
    public class MockController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        public ActionResult<IEnumerable<string>> GetFacilities()
        {
            return new JsonResult(new[]
            {
                new
                {
                    facilityCode = "52000",
                    nameEnglish = "SASKATCHEWAN PENITENTIARY",
                    nameFrench = "ETABL. SASKATCHEWAN",
                },
                new
                {
                    facilityCode = "42000",
                    nameEnglish = "MILLHAVEN ASSESSMENT UNIT",
                    nameFrench = "MILLHAVEN UNITE TRANSFERT",
                },
                new
                {
                    facilityCode = "43000",
                    nameEnglish = "PRISON FOR WOMEN",
                    nameFrench = "PRISON DES FEMMES"
                }
            });
        }

        /// <summary>
        /// Only Returns if facilityCode is 52000
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("Offenders")]
        public ActionResult<IEnumerable<string>> GetOffenders()
        {
            if (Request.Query["facilityCode"] != "52000")
            {
                return new JsonResult(new Object[] { });
            }

            var offenders = new List<Object>();

            for (int i = 0; i < 10; i++)
            {
                offenders.Add(new
                {
                    oid = $"OID {i}",
                    fpsNumber = $"FPS {i}",
                    surname = $"Last {i}",
                    firstname = $"First {i}",
                    facility = new
                    {
                        code = "52000"
                    }
                });
            }

            return new JsonResult(offenders);
        }
    }
}