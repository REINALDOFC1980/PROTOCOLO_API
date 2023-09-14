using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CustomAuthorizeAttribute : TypeFilterAttribute
{
    public CustomAuthorizeAttribute(params string[] roles) : base(typeof(CustomAuthorizeFilter))
    {
        Arguments = new object[] { roles };
    }
}
