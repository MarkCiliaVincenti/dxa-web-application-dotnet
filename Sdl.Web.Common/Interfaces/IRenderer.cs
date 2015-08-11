﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sdl.Web.Common.Models;

namespace Sdl.Web.Common.Interfaces
{
#pragma warning disable 618
    [Obsolete("Renderers are deprecated in DXA 1.1. Rendering should be done using DXA 1.1 HtmlHelper extension methods.")]
    public interface IRenderer
    {
        MvcHtmlString RenderEntity(object item, HtmlHelper helper, int containerSize = 0, List<string> excludedItems = null);
        MvcHtmlString RenderRegion(IRegion region, HtmlHelper helper, int containerSize = 0, List<string> excludedItems = null);
        MvcHtmlString RenderPageData(IPage page, HtmlHelper helper);
    }
#pragma warning restore 618
}