using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionResult
{
    public class ReturnResult
    {
        public object? Data { get; set; }

        public int StatusCode { get; set; } = 200;

        public List<string>? Messages { get; set; }

        public int? Total { get; set; }

        public bool IsSuccess { get; set; } = true;

        public bool IsAuthorized { get; set; } = true;

        public void BadRequest(List<string> messages)
        {
            this.IsSuccess = false;
            this.StatusCode = 400;
            this.Messages = messages;
        }

        public void Ok(object? data)
        {
            this.StatusCode = 200;
            this.Data = data;
        }

        public void CreatedSuccess(object? data)
        {
            this.StatusCode = 201;
            this.Data = data;
        }

        public void InternalServer(List<string> messages)
        {
            this.IsSuccess = false;
            this.StatusCode = 500;
            this.Messages = messages;
        }

        public void Unauthorized(List<string> messages)
        {
            this.IsSuccess = false;
            this.StatusCode = 401;
            this.Messages = messages;
            this.IsAuthorized = false;
        }
    }
}
