﻿using AutoMapper;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TXHRM.Model.Models;
using TXHRM.Service;
using TXHRM.WebAPI.Infrastructure.Core;
using TXHRM.WebAPI.Infrastructure.Extensions;
using TXHRM.WebAPI.Models;

namespace TXHRM.WebAPI.Controllers
{
    
    [RoutePrefix("api/function")]
    public class FunctionController : ApiControllerBase
    {
        private IFunctionService _functionService;

        public FunctionController(IErrorService errorService, IFunctionService functionService) : base(errorService)
        {
            this._functionService = functionService;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string filter = "")
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _functionService.GetAll(filter);

                IEnumerable<FunctionViewModel> modelVm = Mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(model);

                response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }

        [Route("getlisthierachy")]
        [HttpGet]
        public HttpResponseMessage GetAllHierachy(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                IEnumerable<Function> model;
                if (User.IsInRole("Admin"))
                {
                    model = _functionService.GetAll(string.Empty);
                }
                else
                {
                    model = _functionService.GetAllWithPermission(User.Identity.GetUserId());
                }

                IEnumerable<FunctionViewModel> modelVm = Mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(model);
                //IEnumerable<FunctionViewModel> parents = modelVm.Where(x => x.Parent == null);
                //foreach (var parent in parents)
                //{
                //    parent.ChildFunctions = (IEnumerable<FunctionViewModel>)modelVm.Where(x => x.ParentId == parent.ID);
                //}
                //var jsonString = JsonConvert.SerializeObject(parents);
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }


        [Route("detail/{id}")]
        [HttpGet]
        public HttpResponseMessage Details(HttpRequestMessage request, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            var function = _functionService.Get(id);
            if (function == null)
            {
                return request.CreateErrorResponse(HttpStatusCode.NoContent, "No data");
            }
            var modelVm = Mapper.Map<Function, FunctionViewModel>(function);

            return request.CreateResponse(HttpStatusCode.OK, modelVm);
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Create(HttpRequestMessage request, FunctionViewModel functionViewModel)
        {
            if (ModelState.IsValid)
            {
                var newFunction = new Function();
                try
                {
                    if (_functionService.CheckExistedId(functionViewModel.Id))
                    {
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID đã tồn tại");
                    }
                    else
                    {
                        if (functionViewModel.ParentId == "") functionViewModel.ParentId = null;
                        newFunction.UpdateFunction(functionViewModel);

                        _functionService.Create(newFunction);
                        _functionService.Save();
                        return request.CreateResponse(HttpStatusCode.OK, functionViewModel);
                    }

                }
                catch (Exception dex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, dex.Message);
                }
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [HttpPut]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, FunctionViewModel functionViewModel)
        {
            if (ModelState.IsValid)
            {
                var function = _functionService.Get(functionViewModel.Id);
                try
                {
                    if (functionViewModel.ParentId == "") functionViewModel.ParentId = null;
                    function.UpdateFunction(functionViewModel);
                    _functionService.Update(function);
                    _functionService.Save();

                    return request.CreateResponse(HttpStatusCode.OK, function);
                }
                catch (Exception dex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, dex.Message);
                }
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, string id)
        {
            _functionService.Delete(id);
            _functionService.Save();

            return request.CreateResponse(HttpStatusCode.OK, id);
        }
    }
}