﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodinovaAssignment.Helper
{
    public class ReturnModelValue
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public object Errors { get; set; }
        public object Data { get; set; }


        public ReturnModelValue(enumReturnStatus status, string code, string message, object data = null)
        {
            Status = status.ToString().ToLower();
            Code = code;
            Message = message;
            Data = data;
            Errors = string.Empty;
        }
        public ReturnModelValue(enumReturnStatus status, string code, string message, object data, string error)
        {
            Status = status.ToString().ToLower();
            Code = code;
            Message = message;
            Data = data;
            Errors = error.ToString();
        }
        public ReturnModelValue(enumReturnStatus status, string code, string message, object data, ModelStateDictionary error)
        {
            Status = status.ToString().ToLower();
            Code = code;
            Message = message;
            Data = data;
            Errors = GetStateError((error as ModelStateDictionary));
        }

        private Dictionary<string, string[]> GetStateError(ModelStateDictionary modelstate)
        {
            var errorList = modelstate.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            return errorList;
        }
    }
}

public enum enumReturnStatus
{
    Success,
    Failed,
    Warning
}


