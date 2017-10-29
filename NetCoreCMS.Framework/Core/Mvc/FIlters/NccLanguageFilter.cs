﻿/*************************************************************
 *          Project: NetCoreCMS                              *
 *              Web: http://dotnetcorecms.org                *
 *           Author: OnnoRokom Software Ltd.                 *
 *          Website: www.onnorokomsoftware.com               *
 *            Email: info@onnorokomsoftware.com              *
 *        Copyright: OnnoRokom Software Ltd.                 *
 *          License: BSD-3-Clause                            *
 *************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NetCoreCMS.Framework.i18n;
using NetCoreCMS.Framework.Setup;
using NetCoreCMS.Framework.Utility;
using System.Linq;

namespace NetCoreCMS.Framework.Core.Mvc.FIlters
{
    public class NccLanguageFilter : IActionFilter
    {
        private HttpContext _httpContext;

        public NccLanguageFilter()
        {
            
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _httpContext = context.HttpContext;
            var language = GetCurrentLanguage();
            if (context.HttpContext.Items.ContainsKey("NCC_RAZOR_PAGE_PROPERTY_CURRENT_LANGUAGE"))
            {
                context.HttpContext.Items["NCC_RAZOR_PAGE_PROPERTY_CURRENT_LANGUAGE"] = language;
            }
            else
            {
                context.HttpContext.Items.Add("NCC_RAZOR_PAGE_PROPERTY_CURRENT_LANGUAGE", language);
            }

            //NCC_RAZOR_PAGE_PROPERTY_TRANSLATOR
            var controller = context.Controller;
            var translator = new NccTranslator<SharedResource>(context.Controller.GetType(), language);
            if(translator != null)
            {
                context.HttpContext.Items.Add("NCC_RAZOR_PAGE_PROPERTY_TRANSLATOR", translator);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public string GetCurrentLanguage()
        {
            if (GlobalContext.WebSite != null && GlobalContext.WebSite.IsMultiLangual)
            {
                var lang = (string)_httpContext.GetRouteValue("lang");
                if (string.IsNullOrEmpty(lang))
                {
                    lang = GetLanguageFromCookie();
                }

                if (string.IsNullOrEmpty(lang))
                {
                    lang = GlobalContext.WebSite.Language;
                }

                if (string.IsNullOrEmpty(lang))
                {
                    lang = SetupHelper.Language;
                }

                return lang;
            }
            else
            {
                return SetupHelper.Language;
            }
        }

        private string GetLanguageFromCookie()
        {
            if (_httpContext.Request.Cookies.ContainsKey(CookieRequestCultureProvider.DefaultCookieName))
            {
                var cultures = (string)_httpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
                if (string.IsNullOrEmpty(cultures) == false)
                {
                    var parsedValue = CookieRequestCultureProvider.ParseCookieValue(cultures);
                    var culture = parsedValue.Cultures.FirstOrDefault();
                    if (culture != null)
                    {
                        return culture.Value;
                    }
                }
            }
            return string.Empty;
        }
    }
}