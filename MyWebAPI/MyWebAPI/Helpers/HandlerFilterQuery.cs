using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyWebAPI.Helpers
{
    public class HandlerFilterQuery
    {
        public static dynamic Handler(dynamic filter)
        {
            dynamic filterObj = new object();
            try
            {
                if (filter != null)
                {
                    filterObj = JsonConvert.DeserializeObject<dynamic>(filter);
                }
                else
                {
                    filterObj = null;
                }
            }
            catch (Exception)
            {
                throw new CatchError
                {
                    status = HttpStatusCode.BadRequest,
                    code = "0002",
                    name = "invalid_argument",
                    message = "tham số filter truyền vào không đúng"
                }.Exception();
            }
            return filterObj;
        }
    }
}
