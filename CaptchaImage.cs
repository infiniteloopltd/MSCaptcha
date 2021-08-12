// Decompiled with JetBrains decompiler
// Type: MSCaptcha.CaptchaImage
// Assembly: MSCaptcha, Version=2.0.1.36094, Culture=neutral, PublicKeyToken=b9ff12f28cdcf412
// MVID: B7FCC64A-562A-425F-B726-EB56ABFF0C77
// Assembly location: C:\Users\fiach\Downloads\mscaptcha\MSCaptcha.dll

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace MSCaptcha
{
  [Serializable]
  public class CaptchaImage
  {
    private int _height;
    private int _width;
    private Random _rand;
    private DateTime _generatedAt;
    private bool _arithmetic;
    private int _arithmeticSum;
    private string _randomText;
    private int _randomTextLength;
    private string _randomTextChars;
    private string _fontFamilyName;
    private CaptchaImage.FontWarpFactor _fontWarp;
    private CaptchaImage.BackgroundNoiseLevel _backgroundNoise;
    private CaptchaImage.LineNoiseLevel _lineNoise;
    private string _guid;
    private string _fontWhitelist;
    private CaptchaImage.ArithmeticOperation _mathMethod = CaptchaImage.ArithmeticOperation.Addition;
    private Color _backColor = Color.White;
    private Color _fontColor = Color.Black;
    private Color _noiseColor = Color.Black;
    private Color _lineColor = Color.Black;

    /// <summary>Returns a GUID that uniquely identifies this Captcha</summary>
    public string UniqueId => this._guid;

    /// <summary>
    /// Returns the date and time this image was last rendered
    /// </summary>
    public DateTime RenderedAt => this._generatedAt;

    /// <summary>
    /// Font family to use when drawing the Captcha text. If no font is provided, a random font will be chosen from the font whitelist for each character.
    /// </summary>
    public string Font
    {
      get => this._fontFamilyName;
      set
      {
        System.Drawing.Font font = (System.Drawing.Font) null;
        try
        {
          font = new System.Drawing.Font(value, 12f);
          this._fontFamilyName = value;
        }
        catch (Exception ex)
        {
          this._fontFamilyName = FontFamily.GenericSerif.Name;
        }
        finally
        {
          font.Dispose();
        }
      }
    }

    /// <summary>
    /// Amount of random warping to apply to the Captcha text.
    /// </summary>
    public CaptchaImage.FontWarpFactor FontWarp
    {
      get => this._fontWarp;
      set => this._fontWarp = value;
    }

    /// <summary>
    /// Amount of background noise to apply to the Captcha image.
    /// </summary>
    public CaptchaImage.BackgroundNoiseLevel BackgroundNoise
    {
      get => this._backgroundNoise;
      set => this._backgroundNoise = value;
    }

    public CaptchaImage.LineNoiseLevel LineNoise
    {
      get => this._lineNoise;
      set => this._lineNoise = value;
    }

    /// <summary>
    /// A string of valid characters to use in the Captcha text.
    /// A random character will be selected from this string for each character.
    /// </summary>
    public string TextChars
    {
      get => this._randomTextChars;
      set
      {
        this._randomTextChars = value;
        this._randomText = this.GenerateRandomText();
      }
    }

    public bool Arithmetic
    {
      get => this._arithmetic;
      set
      {
        this._arithmetic = value;
        this._randomTextLength = 7;
        this._randomText = this.GenerateRandomText();
      }
    }

    public int ArithmeticSum => this._arithmeticSum;

    /// <summary>Number of characters to use in the Captcha text.</summary>
    public int TextLength
    {
      get => this._randomTextLength;
      set
      {
        this._randomTextLength = value;
        this._randomText = this.GenerateRandomText();
      }
    }

    /// <summary>Returns the randomly generated Captcha text.</summary>
    public string Text => this._randomText;

    /// <summary>Width of Captcha image to generate, in pixels</summary>
    public int Width
    {
      get => this._width;
      set => this._width = value > 60 ? value : throw new ArgumentOutOfRangeException("width", (object) value, "width must be greater than 60.");
    }

    /// <summary>Height of Captcha image to generate, in pixels</summary>
    public int Height
    {
      get => this._height;
      set => this._height = value > 30 ? value : throw new ArgumentOutOfRangeException("height", (object) value, "height must be greater than 30.");
    }

    /// <summary>
    /// A semicolon-delimited list of valid fonts to use when no font is provided.
    /// </summary>
    public string FontWhitelist
    {
      get => this._fontWhitelist;
      set => this._fontWhitelist = value;
    }

    /// <summary>Background color for the captcha image</summary>
    public Color BackColor
    {
      get => this._backColor;
      set => this._backColor = value;
    }

    /// <summary>Color of captcha text</summary>
    public Color FontColor
    {
      get => this._fontColor;
      set => this._fontColor = value;
    }

    /// <summary>Color for dots in the background noise</summary>
    public Color NoiseColor
    {
      get => this._noiseColor;
      set => this._noiseColor = value;
    }

    /// <summary>Color for the background lines of the captcha image</summary>
    public Color LineColor
    {
      get => this._lineColor;
      set => this._lineColor = value;
    }

    /// <summary>
    /// Specifies arithmetic operation to use when generating a new captcha
    /// </summary>
    public CaptchaImage.ArithmeticOperation ArithmeticFunction
    {
      get => this._mathMethod;
      set => this._mathMethod = value;
    }

    public CaptchaImage()
    {
      this._rand = new Random();
      this._fontWarp = CaptchaImage.FontWarpFactor.Low;
      this._backgroundNoise = CaptchaImage.BackgroundNoiseLevel.Low;
      this._lineNoise = CaptchaImage.LineNoiseLevel.None;
      this._width = 180;
      this._height = 50;
      this._randomTextLength = 5;
      this._randomTextChars = "ACDEFGHJKLNPQRTUVXYZ2346789";
      this._fontFamilyName = "";
      this._fontWhitelist = "arial;arial black;comic sans ms;courier new;estrangelo edessa;franklin gothic medium;georgia;lucida console;lucida sans unicode;mangal;microsoft sans serif;palatino linotype;sylfaen;tahoma;times new roman;trebuchet ms;verdana";
      this._randomText = this.GenerateRandomText();
      this._generatedAt = DateTime.Now;
      this._guid = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Forces a new Captcha image to be generated using current property value settings.
    /// </summary>
    public Bitmap RenderImage() => this.GenerateImagePrivate();

    /// <summary>Returns a random font family from the font whitelist</summary>
    private string RandomFontFamily()
    {
      string[] strArray = (string[]) null;
      if (strArray == null)
        strArray = this._fontWhitelist.Split(';');
      return strArray[this._rand.Next(0, strArray.Length)];
    }

    /// <summary>generate random text for the CAPTCHA</summary>
    private string GenerateRandomText()
    {
      StringBuilder stringBuilder = new StringBuilder(this._randomTextLength);
      int length = this._randomTextChars.Length;
      if (!this._arithmetic)
      {
        for (int index = 0; index <= this._randomTextLength - 1; ++index)
          stringBuilder.Append(this._randomTextChars.Substring(this._rand.Next(length), 1));
      }
      else
      {
        switch (this._mathMethod)
        {
          case CaptchaImage.ArithmeticOperation.Addition:
            int num1 = this._rand.Next(2, 99);
            int num2 = this._rand.Next(2, 99);
            stringBuilder.Append(string.Format("{0}+{1}", (object) num1, (object) num2));
            this._arithmeticSum = num1 + num2;
            break;
          case CaptchaImage.ArithmeticOperation.Substraction:
            int maxValue = this._rand.Next(3, 99);
            int num3 = this._rand.Next(1, maxValue);
            stringBuilder.Append(string.Format("{0}-{1}", (object) maxValue, (object) num3));
            this._arithmeticSum = maxValue - num3;
            break;
        }
      }
      return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns a random point within the specified x and y ranges
    /// </summary>
    private PointF RandomPoint(int xmin, int xmax, int ymin, int ymax) => new PointF((float) this._rand.Next(xmin, xmax), (float) this._rand.Next(ymin, ymax));

    /// <summary>Returns a random point within the specified rectangle</summary>
    private PointF RandomPoint(Rectangle rect) => this.RandomPoint(rect.Left, rect.Width, rect.Top, rect.Bottom);

    /// <summary>
    /// Returns a GraphicsPath containing the specified string and font
    /// </summary>
    private GraphicsPath TextPath(string s, System.Drawing.Font f, Rectangle r)
    {
      StringFormat format = new StringFormat();
      format.Alignment = StringAlignment.Near;
      format.LineAlignment = StringAlignment.Near;
      GraphicsPath graphicsPath = new GraphicsPath();
      graphicsPath.AddString(s, f.FontFamily, (int) f.Style, f.Size, r, format);
      return graphicsPath;
    }

    /// <summary>Returns the CAPTCHA font in an appropriate size</summary>
    private System.Drawing.Font GetFont()
    {
      float emSize = 0.0f;
      string familyName = this._fontFamilyName;
      if (familyName == "")
        familyName = this.RandomFontFamily();
      switch (this.FontWarp)
      {
        case CaptchaImage.FontWarpFactor.None:
          emSize = (float) Convert.ToInt32((double) this._height * 0.7);
          break;
        case CaptchaImage.FontWarpFactor.Low:
          emSize = (float) Convert.ToInt32((double) this._height * 0.8);
          break;
        case CaptchaImage.FontWarpFactor.Medium:
          emSize = (float) Convert.ToInt32((double) this._height * 0.85);
          break;
        case CaptchaImage.FontWarpFactor.High:
          emSize = (float) Convert.ToInt32((double) this._height * 0.9);
          break;
        case CaptchaImage.FontWarpFactor.Extreme:
          emSize = (float) Convert.ToInt32((double) this._height * 0.95);
          break;
      }
      return new System.Drawing.Font(familyName, emSize, FontStyle.Bold);
    }

    /// <summary>Renders the CAPTCHA image</summary>
    private Bitmap GenerateImagePrivate()
    {
      Bitmap bitmap = new Bitmap(this._width, this._height, PixelFormat.Format32bppArgb);
      using (Graphics graphics1 = Graphics.FromImage((Image) bitmap))
      {
        graphics1.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle rect = new Rectangle(0, 0, this._width, this._height);
        Brush brush1;
        using (brush1 = (Brush) new SolidBrush(this._backColor))
          graphics1.FillRectangle(brush1, rect);
        int num1 = 0;
        double num2 = (double) (this._width / this._randomTextLength);
        Brush brush2;
        using (brush2 = (Brush) new SolidBrush(this._fontColor))
        {
          foreach (char ch in this._randomText)
          {
            System.Drawing.Font font;
            using (font = this.GetFont())
            {
              Rectangle rectangle = new Rectangle(Convert.ToInt32((double) num1 * num2), 0, Convert.ToInt32(num2), this._height);
              using (GraphicsPath graphicsPath = this.TextPath(ch.ToString(), font, rectangle))
              {
                this.WarpText(graphicsPath, rectangle);
                graphics1.FillPath(brush2, graphicsPath);
              }
            }
            ++num1;
          }
        }
        this.AddNoise(graphics1, rect);
        this.AddLine(graphics1, rect);
      }
      return bitmap;
    }

    /// <summary>
    /// Warp the provided text GraphicsPath by a variable amount
    /// </summary>
    private void WarpText(GraphicsPath textPath, Rectangle rect)
    {
      float num1 = 1f;
      float num2 = 1f;
      switch (this._fontWarp)
      {
        case CaptchaImage.FontWarpFactor.None:
          return;
        case CaptchaImage.FontWarpFactor.Low:
          num1 = 6f;
          num2 = 1f;
          break;
        case CaptchaImage.FontWarpFactor.Medium:
          num1 = 5f;
          num2 = 1.3f;
          break;
        case CaptchaImage.FontWarpFactor.High:
          num1 = 4.5f;
          num2 = 1.4f;
          break;
        case CaptchaImage.FontWarpFactor.Extreme:
          num1 = 4f;
          num2 = 1.5f;
          break;
      }
      RectangleF srcRect = new RectangleF(Convert.ToSingle(rect.Left), 0.0f, Convert.ToSingle(rect.Width), (float) rect.Height);
      int int32_1 = Convert.ToInt32((float) rect.Height / num1);
      int int32_2 = Convert.ToInt32((float) rect.Width / num1);
      int xmin = rect.Left - Convert.ToInt32((float) int32_2 * num2);
      int ymin = rect.Top - Convert.ToInt32((float) int32_1 * num2);
      int xmax = rect.Left + rect.Width + Convert.ToInt32((float) int32_2 * num2);
      int ymax = rect.Top + rect.Height + Convert.ToInt32((float) int32_1 * num2);
      if (xmin < 0)
        xmin = 0;
      if (ymin < 0)
        ymin = 0;
      if (xmax > this.Width)
        xmax = this.Width;
      if (ymax > this.Height)
        ymax = this.Height;
      PointF[] destPoints = new PointF[4]
      {
        this.RandomPoint(xmin, xmin + int32_2, ymin, ymin + int32_1),
        this.RandomPoint(xmax - int32_2, xmax, ymin, ymin + int32_1),
        this.RandomPoint(xmin, xmin + int32_2, ymax - int32_1, ymax),
        this.RandomPoint(xmax - int32_2, xmax, ymax - int32_1, ymax)
      };
      Matrix matrix = new Matrix();
      matrix.Translate(0.0f, 0.0f);
      textPath.Warp(destPoints, srcRect, matrix, WarpMode.Perspective, 0.0f);
    }

    /// <summary>Add a variable level of graphic noise to the image</summary>
    private void AddNoise(Graphics graphics1, Rectangle rect)
    {
      int num1 = 0;
      int num2 = 0;
      switch (this._backgroundNoise)
      {
        case CaptchaImage.BackgroundNoiseLevel.None:
          return;
        case CaptchaImage.BackgroundNoiseLevel.Low:
          num1 = 30;
          num2 = 40;
          break;
        case CaptchaImage.BackgroundNoiseLevel.Medium:
          num1 = 18;
          num2 = 40;
          break;
        case CaptchaImage.BackgroundNoiseLevel.High:
          num1 = 16;
          num2 = 39;
          break;
        case CaptchaImage.BackgroundNoiseLevel.Extreme:
          num1 = 12;
          num2 = 38;
          break;
      }
      using (SolidBrush solidBrush = new SolidBrush(this._noiseColor))
      {
        int int32 = Convert.ToInt32(Math.Max(rect.Width, rect.Height) / num2);
        for (int index = 0; index <= Convert.ToInt32(rect.Width * rect.Height / num1); ++index)
          graphics1.FillEllipse((Brush) solidBrush, this._rand.Next(rect.Width), this._rand.Next(rect.Height), this._rand.Next(int32), this._rand.Next(int32));
      }
    }

    /// <summary>Add variable level of curved lines to the image</summary>
    private void AddLine(Graphics graphics1, Rectangle rect)
    {
      int num1 = 0;
      float width = 1f;
      int num2 = 0;
      switch (this._lineNoise)
      {
        case CaptchaImage.LineNoiseLevel.None:
          return;
        case CaptchaImage.LineNoiseLevel.Low:
          num1 = 4;
          width = Convert.ToSingle((double) this._height / 31.25);
          num2 = 1;
          break;
        case CaptchaImage.LineNoiseLevel.Medium:
          num1 = 5;
          width = Convert.ToSingle((double) this._height / 27.7777);
          num2 = 1;
          break;
        case CaptchaImage.LineNoiseLevel.High:
          num1 = 3;
          width = Convert.ToSingle(this._height / 25);
          num2 = 2;
          break;
        case CaptchaImage.LineNoiseLevel.Extreme:
          num1 = 3;
          width = Convert.ToSingle((double) this._height / 22.7272);
          num2 = 3;
          break;
      }
      PointF[] points = new PointF[num1 + 1];
      using (Pen pen = new Pen(this._lineColor, width))
      {
        for (int index1 = 1; index1 <= num2; ++index1)
        {
          for (int index2 = 0; index2 <= num1; ++index2)
            points[index2] = this.RandomPoint(rect);
          graphics1.DrawCurve(pen, points, 1.75f);
        }
      }
    }

    /// <summary>
    /// Amount of random font warping to apply to rendered text
    /// </summary>
    public enum FontWarpFactor
    {
      None,
      Low,
      Medium,
      High,
      Extreme,
    }

    /// <summary>Amount of background noise to add to rendered image</summary>
    public enum BackgroundNoiseLevel
    {
      None,
      Low,
      Medium,
      High,
      Extreme,
    }

    /// <summary>Amount of curved line noise to add to rendered image</summary>
    public enum LineNoiseLevel
    {
      None,
      Low,
      Medium,
      High,
      Extreme,
    }

    /// <summary>Arithmetic operation to perform in formula</summary>
    public enum ArithmeticOperation
    {
      Random,
      Addition,
      Substraction,
    }
  }
}
