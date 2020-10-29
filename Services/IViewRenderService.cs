using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToString(string viewName, object model);
        string Content { get; set; }
    }

}
