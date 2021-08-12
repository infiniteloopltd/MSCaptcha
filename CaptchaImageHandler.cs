// Decompiled with JetBrains decompiler
// Type: MSCaptcha.CaptchaImageHandler
// Assembly: MSCaptcha, Version=2.0.1.36094, Culture=neutral, PublicKeyToken=b9ff12f28cdcf412
// MVID: B7FCC64A-562A-425F-B726-EB56ABFF0C77
// Assembly location: C:\Users\fiach\Downloads\mscaptcha\MSCaptcha.dll

using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;

namespace MSCaptcha
{
  public class CaptchaImageHandler : IHttpHandler, IRequiresSessionState
  {
    void IHttpHandler.ProcessRequest(HttpContext context)
    {
      HttpApplication applicationInstance = context.ApplicationInstance;
      string str = applicationInstance.Request.QueryString["guid"];
      CaptchaImage captchaImage = (CaptchaImage) null;
      if (str != "")
        captchaImage = !string.IsNullOrEmpty(applicationInstance.Request.QueryString["s"]) ? (CaptchaImage) HttpContext.Current.Session[str] : (CaptchaImage) HttpRuntime.Cache.Get(str);
      if (captchaImage == null)
      {
        applicationInstance.Response.StatusCode = 404;
        context.ApplicationInstance.CompleteRequest();
      }
      else
      {
        using (Bitmap bitmap = captchaImage.RenderImage())
          bitmap.Save(applicationInstance.Context.Response.OutputStream, ImageFormat.Jpeg);
        applicationInstance.Response.ContentType = "image/jpeg";
        applicationInstance.Response.StatusCode = 200;
        context.ApplicationInstance.CompleteRequest();
      }
    }

    bool IHttpHandler.IsReusable => true;
  }
}
