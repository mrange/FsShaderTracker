namespace FsTracker.OpenGL
{
  using System;
  using System.Runtime.InteropServices;

  public static partial class Native
  {
    public const UInt32 PFD_DOUBLEBUFFER            = 0x00000001;
    public const UInt32 PFD_STEREO                  = 0x00000002;
    public const UInt32 PFD_DRAW_TO_WINDOW          = 0x00000004;
    public const UInt32 PFD_DRAW_TO_BITMAP          = 0x00000008;
    public const UInt32 PFD_SUPPORT_GDI             = 0x00000010;
    public const UInt32 PFD_SUPPORT_OPENGL          = 0x00000020;
    public const UInt32 PFD_GENERIC_FORMAT          = 0x00000040;
    public const UInt32 PFD_NEED_PALETTE            = 0x00000080;
    public const UInt32 PFD_NEED_SYSTEM_PALETTE     = 0x00000100;
    public const UInt32 PFD_SWAP_EXCHANGE           = 0x00000200;
    public const UInt32 PFD_SWAP_COPY               = 0x00000400;
    public const UInt32 PFD_SWAP_LAYER_BUFFERS      = 0x00000800;
    public const UInt32 PFD_GENERIC_ACCELERATED     = 0x00001000;
    public const UInt32 PFD_SUPPORT_DIRECTDRAW      = 0x00002000;
    public const UInt32 PFD_DIRECT3D_ACCELERATED    = 0x00004000;
    public const UInt32 PFD_SUPPORT_COMPOSITION     = 0x00008000;

    public const Byte   PFD_TYPE_RGBA               = 0;
    public const Byte   PFD_TYPE_COLORINDEX         = 1;

    public const Byte   PFD_MAIN_PLANE              = 0   ;
    public const Byte   PFD_OVERLAY_PLANE           = 1   ;
    public const Byte   PFD_UNDERLAY_PLANE          = 0xFF;

    public const UInt32 GL_FRAGMENT_SHADER          = 0x8B30;
    public const UInt32 GL_VERTEX_SHADER            = 0x8B31;
    public const UInt32 GL_LINK_STATUS              = 0x8B82;
    public const UInt32 GL_DRAW_FRAMEBUFFER         = 0x8CA9;

    public const UInt32 GL_VERTEX_SHADER_BIT        = 0x00000001;
    public const UInt32 GL_FRAGMENT_SHADER_BIT      = 0x00000002;


    [StructLayout(LayoutKind.Sequential)]
    public struct PIXELFORMATDESCRIPTOR
    {
        public const            UInt16 CurrentVersion = 1;
        public readonly static  UInt16 CurrentSize    = (UInt16)Marshal.SizeOf(typeof(PIXELFORMATDESCRIPTOR));

        public static PIXELFORMATDESCRIPTOR Create ()
        {
          var v = new PIXELFORMATDESCRIPTOR
          {
            nSize     = CurrentSize   ,
            nVersion  = CurrentVersion,
          };
          return v;
        }

        public UInt16  nSize;
        public UInt16  nVersion;
        public UInt32  dwFlags;
        public Byte    iPixelType;
        public Byte    cColorBits;
        public Byte    cRedBits;
        public Byte    cRedShift;
        public Byte    cGreenBits;
        public Byte    cGreenShift;
        public Byte    cBlueBits;
        public Byte    cBlueShift;
        public Byte    cAlphaBits;
        public Byte    cAlphaShift;
        public Byte    cAccumBits;
        public Byte    cAccumRedBits;
        public Byte    cAccumGreenBits;
        public Byte    cAccumBlueBits;
        public Byte    cAccumAlphaBits;
        public Byte    cDepthBits;
        public Byte    cStencilBits;
        public Byte    cAuxBuffers;
        public Byte    iLayerType;
        public Byte    bReserved;
        public UInt32  dwLayerMask;
        public UInt32  dwVisibleMask;
        public UInt32  dwDamageMask;
    }

    [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern Int32 ChoosePixelFormat(IntPtr hdc, [In] ref PIXELFORMATDESCRIPTOR ppfd);

    [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern Int32 SetPixelFormat(IntPtr hdc, [In] Int32 format, [In] ref PIXELFORMATDESCRIPTOR ppfd);

    [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern bool SwapBuffers(IntPtr hdc);

    [DllImport("opengl32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern IntPtr wglCreateContext(IntPtr hdc);

    [DllImport("opengl32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern bool wglMakeCurrent(IntPtr hdc, IntPtr hglrc);

    [DllImport("opengl32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern IntPtr wglGetProcAddress(string name);

    [DllImport("opengl32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern void glViewport(Int32 x, Int32 y, Int32 width, Int32 height);

    [DllImport("opengl32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern void glRects(Int16 x1, Int16 y1, Int16 x2, Int16 y2);


    public static TDelegate LoadOpenGlFunction<TDelegate>(string glFunc)
    {
      IntPtr glPtr = wglGetProcAddress(glFunc);
      if (glPtr == IntPtr.Zero)
      {
        throw new Exception($"OpenGL function '{glFunc}' not found in current context");
      }

      return Marshal.GetDelegateForFunctionPointer<TDelegate>(glPtr);
    }

    public delegate UInt32 Delegate_glCreateShaderProgramv    (UInt32 type, Int32 count, IntPtr[] programs);
    public delegate void   Delegate_glGenProgramPipelines     (Int32 n, ref UInt32 pipeline);
    public delegate void   Delegate_glBindProgramPipeline     (UInt32 pipeline);
    public delegate void   Delegate_glUseProgramStages        (UInt32 pipeline, UInt32 stages, UInt32 program);
    public delegate void   Delegate_glGetProgramiv            (UInt32 program, UInt32 pname, ref Int32 parameter);
    public delegate void   Delegate_glGetProgramInfoLog       (UInt32 program, Int32 bufSize, ref Int32 length, byte[] buffer);
    public delegate void   Delegate_glProgramUniform1f        (UInt32 program, Int32 location, Single v0);
    public delegate void   Delegate_glProgramUniform2f        (UInt32 program, Int32 location, Single v0, Single v1);
    public delegate void   Delegate_glActiveTexture           (UInt32 texture);
    public delegate void   Delegate_glBindSampler             (UInt32 unit, UInt32 sampler);
    public delegate void   Delegate_glProgramUniform1i        (UInt32 program, Int32 location, Int32 v0);
    public delegate void   Delegate_glGenerateMipmap          (UInt32 target);
    public delegate void   Delegate_glGenFramebuffers         (Int32 n, UInt32[] framebuffers);
    public delegate void   Delegate_glBindFramebuffer         (UInt32 target, UInt32 framebuffer);
    public delegate void   Delegate_glFramebufferRenderbuffer (UInt32 target, UInt32 attachment, UInt32 renderbuffertarget, UInt32 renderbuffer);
    public delegate void   Delegate_glGenRenderbuffers        (UInt32 n, UInt32[] renderbuffers);
    public delegate void   Delegate_glBindRenderbuffer        (UInt32 target, UInt32 renderbuffer);
    public delegate void   Delegate_glRenderbufferStorage     (UInt32 target, UInt32 internalformat, Int32 width, Int32 height);
    public delegate void   Delegate_glBlitFramebuffer         (Int32 srcX0, Int32 srcY0, Int32 srcX1, Int32 srcY1, Int32 dstX0, Int32 dstY0, Int32 dstX1, Int32 dstY1, UInt32 mask, UInt32 filter);

  }
}
