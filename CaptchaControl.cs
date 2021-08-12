// Decompiled with JetBrains decompiler
// Type: MSCaptcha.CaptchaControl
// Assembly: MSCaptcha, Version=2.0.1.36094, Culture=neutral, PublicKeyToken=b9ff12f28cdcf412
// MVID: B7FCC64A-562A-425F-B726-EB56ABFF0C77
// Assembly location: C:\Users\fiach\Downloads\mscaptcha\MSCaptcha.dll

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MSCaptcha
{
  [DefaultProperty("Text")]
  public class CaptchaControl : WebControl, INamingContainer, IPostBackDataHandler, IValidator
  {
    private int _timeoutSecondsMax = 90;
    private int _timeoutSecondsMin = 3;
    private bool _userValidated = true;
    private string _text = "Enter the code shown:";
    private string _font = "";
    private CaptchaImage _captcha = new CaptchaImage();
    private string _prevguid;
    private string _errorMessage = "";
    private CaptchaControl.CacheType _cacheStrategy;
    private bool m_Arithmetic;
    private CaptchaControl.ArithmeticOperation _mathMethod = CaptchaControl.ArithmeticOperation.Addition;
    private string m_ValidationGroup;
    private string m_CustomValidatorErrorMessage = "The text you typed does not match the text in the image.";
    private string m_ImageTag = "border=\"0\"";
    private string m_ClientKey = DateTime.Now.ToShortDateString();
    private CaptchaControl.ClientHashingAlgorithm m_Hash;
    private string m_EnKey = "";
    private Color _backColor = Color.White;
    private Color _fontColor = Color.Black;
    private Color _noiseColor = Color.Black;
    private Color _lineColor = Color.Black;

    public string ErrorInputTooFast { get; set; }

    public string ErrorInputTooSlow { get; set; }

    public string ClientKey => this.m_ClientKey;

    public CaptchaControl.ClientHashingAlgorithm ClientHashMethod
    {
      get => this.m_Hash;
      set => this.m_Hash = value;
    }

    public string ImageTag
    {
      get => this.m_ImageTag;
      set => this.m_ImageTag = value;
    }

    public string CustomValidatorErrorMessage
    {
      get => this.m_CustomValidatorErrorMessage;
      set => this.m_CustomValidatorErrorMessage = value;
    }

    public string ValidationGroup
    {
      get => this.m_ValidationGroup;
      set => this.m_ValidationGroup = value;
    }

    public bool Arithmetic
    {
      get => this.m_Arithmetic;
      set
      {
        this.m_Arithmetic = value;
        this._captcha.Arithmetic = value;
        this._captcha.RenderImage();
      }
    }

    [DefaultValue("The text you typed does not match the text in the image.")]
    [Description("Message to display in a Validation Summary when the CAPTCHA fails to validate.")]
    [Bindable(true)]
    [Category("Appearance")]
    [Browsable(false)]
    string IValidator.ErrorMessage
    {
      get => !this._userValidated ? this.m_CustomValidatorErrorMessage : "";
      set => this.m_CustomValidatorErrorMessage = value;
    }

    bool IValidator.IsValid
    {
      get => this._userValidated;
      set
      {
      }
    }

    public override bool Enabled
    {
      get => base.Enabled;
      set
      {
        base.Enabled = value;
        if (value)
          return;
        this._userValidated = true;
      }
    }

    [DefaultValue(typeof (CaptchaControl.CacheType), "HttpRuntime")]
    [Description("Determines if CAPTCHA codes are stored in HttpRuntime (fast, but local to current server) or Session (more portable across web farms).")]
    [Category("Captcha")]
    public CaptchaControl.CacheType CacheStrategy
    {
      get => this._cacheStrategy;
      set => this._cacheStrategy = value;
    }

    [Category("Captcha")]
    [Description("Returns True if the user was CAPTCHA validated after a postback.")]
    public bool UserValidated => this._userValidated;

    [DefaultValue("")]
    [Category("Captcha")]
    [Description("Font used to render CAPTCHA text. If font name is blank, a random font will be chosen.")]
    public string CaptchaFont
    {
      get => this._font;
      set
      {
        this._font = value;
        this._captcha.Font = this._font;
      }
    }

    [DefaultValue("ABCDEFGHJKLMNPQRSTUVWXYZ23456789")]
    [Category("Captcha")]
    [Description("Characters used to render CAPTCHA text. A character will be picked randomly from the string.")]
    public string CaptchaChars
    {
      get => !this.m_Arithmetic ? this._captcha.TextChars : "0123456789";
      set => this._captcha.TextChars = value;
    }

    [Category("Captcha")]
    [DefaultValue(5)]
    [Description("Number of CaptchaChars used in the CAPTCHA text")]
    public int CaptchaLength
    {
      get => this._captcha.TextLength;
      set => this._captcha.TextLength = value;
    }

    [DefaultValue(2)]
    [Description("Minimum number of seconds CAPTCHA must be displayed before it is valid. If you're too fast, you must be a robot. Set to zero to disable.")]
    [Category("Captcha")]
    public int CaptchaMinTimeout
    {
      get => this._timeoutSecondsMin;
      set => this._timeoutSecondsMin = value <= 15 ? value : throw new ArgumentOutOfRangeException("CaptchaTimeout", "Timeout must be less than 15 seconds. Humans aren't that slow!");
    }

    [DefaultValue(90)]
    [Description("Maximum number of seconds CAPTCHA will be cached and valid. If you're too slow, you may be a CAPTCHA hack attempt. Set to zero to disable.")]
    [Category("Captcha")]
    public int CaptchaMaxTimeout
    {
      get => this._timeoutSecondsMax;
      set => this._timeoutSecondsMax = !(value < 15 & value != 0) ? value : throw new ArgumentOutOfRangeException("CaptchaTimeout", "Timeout must be greater than 15 seconds. Humans can't type that fast!");
    }

    [Category("Captcha")]
    [Description("Height of generated CAPTCHA image.")]
    [DefaultValue(50)]
    public int CaptchaHeight
    {
      get => this._captcha.Height;
      set => this._captcha.Height = value;
    }

    [Description("Width of generated CAPTCHA image.")]
    [DefaultValue(180)]
    [Category("Captcha")]
    public int CaptchaWidth
    {
      get => this._captcha.Width;
      set => this._captcha.Width = value;
    }

    [DefaultValue(typeof (CaptchaImage.FontWarpFactor), "Low")]
    [Category("Captcha")]
    [Description("Amount of random font warping used on the CAPTCHA text")]
    public CaptchaImage.FontWarpFactor CaptchaFontWarping
    {
      get => this._captcha.FontWarp;
      set => this._captcha.FontWarp = value;
    }

    [Description("Amount of background noise to generate in the CAPTCHA image")]
    [Category("Captcha")]
    [DefaultValue(typeof (CaptchaImage.BackgroundNoiseLevel), "Low")]
    public CaptchaImage.BackgroundNoiseLevel CaptchaBackgroundNoise
    {
      get => this._captcha.BackgroundNoise;
      set => this._captcha.BackgroundNoise = value;
    }

    [DefaultValue(typeof (CaptchaImage.LineNoiseLevel), "None")]
    [Description("Add line noise to the CAPTCHA image")]
    [Category("Captcha")]
    public CaptchaImage.LineNoiseLevel CaptchaLineNoise
    {
      get => this._captcha.LineNoise;
      set => this._captcha.LineNoise = value;
    }

    /// <summary>
    /// Specifies arithmetic operation to use when generating a new captcha
    /// </summary>
    public CaptchaControl.ArithmeticOperation ArithmeticFunction
    {
      get => this._mathMethod;
      set
      {
        this._mathMethod = value;
        this._captcha.ArithmeticFunction = (CaptchaImage.ArithmeticOperation) value;
        this._captcha.RenderImage();
      }
    }

    /// <summary>Background color for the captcha image</summary>
    public new Color BackColor
    {
      get => this._backColor;
      set
      {
        this._backColor = value;
        this._captcha.BackColor = this._backColor;
      }
    }

    /// <summary>Color of captcha text</summary>
    public Color FontColor
    {
      get => this._fontColor;
      set
      {
        this._fontColor = value;
        this._captcha.FontColor = this._fontColor;
      }
    }

    /// <summary>Color for dots in the background noise</summary>
    public Color NoiseColor
    {
      get => this._noiseColor;
      set
      {
        this._noiseColor = value;
        this._captcha.NoiseColor = this._noiseColor;
      }
    }

    /// <summary>Color for the background lines of the captcha image</summary>
    public Color LineColor
    {
      get => this._lineColor;
      set
      {
        this._lineColor = value;
        this._captcha.LineColor = this._lineColor;
      }
    }

    void IValidator.Validate()
    {
    }

    private CaptchaImage GetCachedCaptcha(string guid) => this._cacheStrategy == CaptchaControl.CacheType.HttpRuntime || this._cacheStrategy == CaptchaControl.CacheType.ClientSide ? (CaptchaImage) HttpRuntime.Cache.Get(guid) : (CaptchaImage) HttpContext.Current.Session[guid];

    private void RemoveCachedCaptcha(string guid)
    {
      if (this._cacheStrategy == CaptchaControl.CacheType.HttpRuntime || this._cacheStrategy == CaptchaControl.CacheType.ClientSide)
        HttpRuntime.Cache.Remove(guid);
      else
        HttpContext.Current.Session.Remove(guid);
    }

    /// <summary>are we in design mode?</summary>
    private bool IsDesignMode => HttpContext.Current == null;

    [Description("Validate Captcha and return IsValid property")]
    public bool CheckCaptcha(string userEntry)
    {
      this.ValidateCaptcha(userEntry);
      return this._userValidated;
    }

    /// <summary>Validate the user's text against the CAPTCHA text</summary>
    public void ValidateCaptcha(string userEntry)
    {
      if (!this.Visible | !this.Enabled)
        this._userValidated = true;
      else if (this._cacheStrategy == CaptchaControl.CacheType.ClientSide)
      {
        string str = this.ViewState["MSCaptcha"].ToString();
        if (this.EncryptKey(userEntry) != str)
        {
          this._userValidated = false;
          this.ViewState.Remove("MSCaptcha");
          ((IValidator) this).ErrorMessage = this.m_CustomValidatorErrorMessage;
        }
        else
          this._userValidated = true;
      }
      else
      {
        CaptchaImage cachedCaptcha = this.GetCachedCaptcha(this._prevguid);
        if (cachedCaptcha == null)
        {
          if (this.ErrorInputTooSlow.Length == 0)
            ((IValidator) this).ErrorMessage = "The code you typed has expired after " + (object) this.CaptchaMaxTimeout + " seconds.";
          else
            ((IValidator) this).ErrorMessage = this.ErrorInputTooSlow;
          this._userValidated = false;
        }
        else if (this.CaptchaMinTimeout > 0 && cachedCaptcha.RenderedAt.AddSeconds((double) this.CaptchaMinTimeout) > DateTime.Now)
        {
          this._userValidated = false;
          if (this.ErrorInputTooFast.Length == 0)
            ((IValidator) this).ErrorMessage = "Code was typed too quickly. Wait at least " + (object) this.CaptchaMinTimeout + " seconds.";
          else
            ((IValidator) this).ErrorMessage = this.ErrorInputTooFast;
          this.RemoveCachedCaptcha(this._prevguid);
        }
        else
        {
          if (!this.m_Arithmetic)
          {
            if (string.Compare(userEntry, cachedCaptcha.Text, true) != 0)
            {
              ((IValidator) this).ErrorMessage = this.m_CustomValidatorErrorMessage;
              this._userValidated = false;
              this.RemoveCachedCaptcha(this._prevguid);
              return;
            }
          }
          else if (string.Compare(userEntry, cachedCaptcha.ArithmeticSum.ToString(), true) != 0)
          {
            ((IValidator) this).ErrorMessage = "The code you typed does not match the result of the formula in the image.";
            this._userValidated = false;
            this.RemoveCachedCaptcha(this._prevguid);
            return;
          }
          this._userValidated = true;
          this.RemoveCachedCaptcha(this._prevguid);
        }
      }
    }

    /// <summary>returns HTML-ized color strings</summary>
    private string HtmlColor(Color color)
    {
      if (color.IsEmpty)
        return "";
      if (color.IsNamedColor)
        return color.ToKnownColor().ToString();
      return color.IsSystemColor ? color.ToString() : "#" + color.ToArgb().ToString("x").Substring(2);
    }

    /// <summary>
    /// returns css "style=" tag for this control
    /// based on standard control visual properties
    /// </summary>
    private string CssStyle()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(" style='");
      if (this.BorderWidth.ToString().Length > 0)
      {
        stringBuilder.Append("border-width:");
        stringBuilder.Append(this.BorderWidth.ToString());
        stringBuilder.Append(";");
      }
      if (this.BorderStyle != BorderStyle.NotSet)
      {
        stringBuilder.Append("border-style:");
        stringBuilder.Append(this.BorderStyle.ToString());
        stringBuilder.Append(";");
      }
      string str1 = this.HtmlColor(this.BorderColor);
      if (str1.Length > 0)
      {
        stringBuilder.Append("border-color:");
        stringBuilder.Append(str1);
        stringBuilder.Append(";");
      }
      string str2 = this.HtmlColor(this.BackColor);
      if (str2.Length > 0)
        stringBuilder.Append("background-color:" + str2 + ";");
      string str3 = this.HtmlColor(this.ForeColor);
      if (str3.Length > 0)
        stringBuilder.Append("color:" + str3 + ";");
      if (this.Font.Bold)
        stringBuilder.Append("font-weight:bold;");
      if (this.Font.Italic)
        stringBuilder.Append("font-style:italic;");
      if (this.Font.Underline)
        stringBuilder.Append("text-decoration:underline;");
      if (this.Font.Strikeout)
        stringBuilder.Append("text-decoration:line-through;");
      if (this.Font.Overline)
        stringBuilder.Append("text-decoration:overline;");
      if (this.Font.Size.ToString().Length > 0)
        stringBuilder.Append("font-size:" + this.Font.Size.ToString() + ";");
      if (this.Font.Names.Length > 0)
      {
        stringBuilder.Append("font-family:");
        foreach (string name in this.Font.Names)
        {
          stringBuilder.Append(name);
          stringBuilder.Append(",");
        }
        --stringBuilder.Length;
        stringBuilder.Append(";");
      }
      if (this.Height.ToString() != "")
        stringBuilder.Append("height:" + this.Height.ToString() + ";");
      if (this.Width.ToString() != "")
        stringBuilder.Append("width:" + this.Width.ToString() + ";");
      stringBuilder.Append("'");
      return stringBuilder.ToString() == " style=''" ? "" : stringBuilder.ToString();
    }

    /// <summary>render raw control HTML to the page</summary>
    protected override void Render(HtmlTextWriter Output)
    {
      Output.Write("<div");
      if (this.CssClass != "")
        Output.Write(" class='" + this.CssClass + "'");
      Output.Write(this.CssStyle());
      Output.Write(" >");
      Output.Write("<img src=\"CaptchaImage.axd");
      if (!this.IsDesignMode)
        Output.Write("?guid=" + Convert.ToString(this._captcha.UniqueId));
      if (this.CacheStrategy == CaptchaControl.CacheType.Session)
        Output.Write("&s=1");
      if (this.m_ImageTag.Length > 0)
        Output.Write("\" " + this.m_ImageTag);
      if (this.ToolTip.Length > 0)
        Output.Write(" alt='" + this.ToolTip + "'");
      else
        Output.Write(" alt=\"Captcha\"");
      Output.Write(" width=\"" + this._captcha.Width.ToString() + "\"");
      Output.Write(" height=\"" + this._captcha.Height.ToString() + "\"");
      Output.Write(" />");
      Output.Write("</div>");
    }

    /// <summary>
    /// generate a new captcha and store it in the ASP.NET Cache by unique GUID
    /// </summary>
    private void GenerateNewCaptcha()
    {
      if (this.IsDesignMode)
        return;
      switch (this._cacheStrategy)
      {
        case CaptchaControl.CacheType.HttpRuntime:
          HttpRuntime.Cache.Add(this._captcha.UniqueId, (object) this._captcha, (CacheDependency) null, DateTime.Now.AddSeconds(Convert.ToDouble(this.CaptchaMaxTimeout == 0 ? 90 : this.CaptchaMaxTimeout)), Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, (CacheItemRemovedCallback) null);
          break;
        case CaptchaControl.CacheType.Session:
          HttpContext.Current.Session.Add(this._captcha.UniqueId, (object) this._captcha);
          break;
        case CaptchaControl.CacheType.ClientSide:
          this.m_EnKey = !this._captcha.Arithmetic ? this.EncryptKey(this._captcha.Text) : this.EncryptKey(this._captcha.ArithmeticSum.ToString());
          HttpRuntime.Cache.Add(this._captcha.UniqueId, (object) this._captcha, (CacheDependency) null, DateTime.Now.AddSeconds(5.0), Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, (CacheItemRemovedCallback) null);
          this.ViewState.Add("MSCaptcha", (object) this.m_EnKey);
          break;
      }
    }

    /// <summary>
    /// Retrieve the user's CAPTCHA input from the posted data
    /// </summary>
    bool IPostBackDataHandler.LoadPostData(
      string PostDataKey,
      NameValueCollection Values)
    {
      this.ValidateCaptcha(Convert.ToString(Values[this.UniqueID]));
      return false;
    }

    void IPostBackDataHandler.RaisePostDataChangedEvent()
    {
    }

    protected override object SaveControlState() => (object) this._captcha.UniqueId;

    protected override void LoadControlState(object state)
    {
      if (state == null)
        return;
      this._prevguid = (string) state;
    }

    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);
      this.Page.RegisterRequiresControlState((Control) this);
      this.Page.Validators.Add((IValidator) this);
    }

    protected override void OnUnload(EventArgs e)
    {
      if (this.Page != null)
        this.Page.Validators.Remove((IValidator) this);
      base.OnUnload(e);
    }

    protected override void OnPreRender(EventArgs e)
    {
      if (this.Visible)
        this.GenerateNewCaptcha();
      base.OnPreRender(e);
    }

    private string EncryptKey(string OriginalValue)
    {
      string input = this.m_ClientKey + OriginalValue;
      switch (this.m_Hash)
      {
        case CaptchaControl.ClientHashingAlgorithm.SHA1:
          return this.CalculateSHA1Hash(input);
        case CaptchaControl.ClientHashingAlgorithm.SHA256:
          return this.CalculateSHA256Hash(input);
        default:
          return this.CalculateMD5Hash(input);
      }
    }

    private string CalculateMD5Hash(string input)
    {
      byte[] hash = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("X2"));
      return stringBuilder.ToString();
    }

    private string CalculateSHA1Hash(string input)
    {
      byte[] hash = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(input));
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("X2"));
      return stringBuilder.ToString();
    }

    private string CalculateSHA256Hash(string input)
    {
      byte[] hash = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(input));
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("X2"));
      return stringBuilder.ToString();
    }

    public enum CacheType
    {
      HttpRuntime,
      Session,
      ClientSide,
    }

    /// <summary>Arithmetic operation to perform in formula</summary>
    public enum ArithmeticOperation
    {
      Random,
      Addition,
      Substraction,
    }

    public enum ClientHashingAlgorithm
    {
      MD5,
      SHA1,
      SHA256,
    }
  }
}
