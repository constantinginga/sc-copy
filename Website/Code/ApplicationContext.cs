using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ApplicationContext
{

    private static ApplicationContext _this = new ApplicationContext();

    public static ApplicationContext Current { get { return _this; } }

    public ApplicationContext Services { get { return _this; } }

    public Umbraco.Core.Services.IContentService ContentService
    {
        get
        {
            return Umbraco.Core.Composing.Current.Services.ContentService;
        }
    }

    public Umbraco.Core.Services.IMemberService MemberService
    {
        get
        {
            return Umbraco.Core.Composing.Current.Services.MemberService;
        }
    }

    public Umbraco.Core.Services.IMediaService MediaService
    {
        get
        {
            return Umbraco.Core.Composing.Current.Services.MediaService;
        }
    }

    public Umbraco.Core.Services.IEntityService EntityService { 
        get 
        {
            return Umbraco.Core.Composing.Current.Services.EntityService;
        }
    }

    /*
    public Umbraco.Core.Services.IContentTypeBaseServiceProvider ContentTypeBaseServices
    {
        get
        {
            return Umbraco.Core.Composing.Current.Services.ContentTypeBaseServices;
        }
    }
    */
}