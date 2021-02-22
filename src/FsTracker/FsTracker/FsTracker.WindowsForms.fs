namespace FsTracker

open System.Drawing
open System.Runtime.InteropServices
open System.Text
open System.Windows.Forms

module WindowsForms =
  open Common

  open type FsTracker.OpenGL.Native

  let vertexShader = """#version 460

#define SCREEN_LOADER

layout (location=0) in vec2 inVer;
out vec2 p;
out vec2 q;

out gl_PerVertex
{
vec4 gl_Position;
};

void main()
{
gl_Position=vec4(inVer,0.0,1.0);
p=inVer;
q=0.5*inVer+0.5;
}
"""

  let pixelFormatDescriptor =
    let mutable pfd = PIXELFORMATDESCRIPTOR.Create ()
    pfd.dwFlags               <- PFD_DRAW_TO_WINDOW|||PFD_SUPPORT_OPENGL|||PFD_DOUBLEBUFFER
    pfd.iPixelType            <- PFD_TYPE_RGBA
    pfd.cColorBits            <- 24uy
    pfd.cRedBits              <- 0uy
    pfd.cRedShift             <- 0uy
    pfd.cGreenBits            <- 0uy
    pfd.cGreenShift           <- 0uy
    pfd.cBlueBits             <- 0uy
    pfd.cBlueShift            <- 0uy
    pfd.cAlphaBits            <- 8uy
    pfd.cAlphaShift           <- 0uy
    pfd.cAccumBits            <- 0uy
    pfd.cAccumRedBits         <- 0uy
    pfd.cAccumGreenBits       <- 0uy
    pfd.cAccumBlueBits        <- 0uy
    pfd.cAccumAlphaBits       <- 0uy
    pfd.cDepthBits            <- 32uy
    pfd.cStencilBits          <- 0uy
    pfd.cAuxBuffers           <- 0uy
    pfd.iLayerType            <- PFD_MAIN_PLANE
    pfd.bReserved             <- 0uy
    pfd.dwLayerMask           <- 0u
    pfd.dwVisibleMask         <- 0u
    pfd.dwDamageMask          <- 0u
    pfd

  let check nm res =
    if res  = Unchecked.defaultof<_> then
      let e = Marshal.GetLastWin32Error ()
      failwithf "%s failed with 0x%x" nm e
    else
      res

  let check_link_status (opengl : OpenGlFunctions) nm id =
    let mutable res = -1
    opengl.glGetProgramiv.Invoke (id, GL_LINK_STATUS, &res)
    if res = 0 then
      let buf = Array.zeroCreate 1024
      let mutable bufLen = 0
      opengl.glGetProgramInfoLog.Invoke (id, 1024, &bufLen, buf)
      let err = Encoding.ASCII.GetString (buf, 0, bufLen)
      failwithf "Link status failed for '%s' with: %s" nm err
    else
      id

  type Invoker = delegate of unit -> unit

  let run (m : Module) =
    Application.SetHighDpiMode HighDpiMode.SystemAware |> ignore
    Application.EnableVisualStyles ()
    Application.SetCompatibleTextRenderingDefault false

    use form = new Form ()
    form.ClientSize <- Size (1920, 1200)
    form.Text       <- "FsShaderTracker"

    let hwnd        = form.Handle
    let hdc         = GetDC hwnd

    let mutable pfd = pixelFormatDescriptor
    let pf = 
      ChoosePixelFormat (hdc, &pfd) 
      |> check "ChoosePixelFormat"

    SetPixelFormat (hdc, pf, &pfd) 
    |> check "SetPixelFormat" 
    |> ignore

    let hglrc = 
      wglCreateContext hdc 
      |> check "wglCreateContext"

    wglMakeCurrent (hdc, hglrc) 
    |> check "wglMakeCurrent" 
    |> ignore

    let opengl = new OpenGlFunctions ()
    let check_link_status = check_link_status opengl

    let vsh = Marshal.StringToCoTaskMemAnsi vertexShader
    use freeVsh = onExitFreeCoTaskMem vsh

    let fsh = Marshal.StringToCoTaskMemAnsi m.FragmentShader
    use freeFsh = onExitFreeCoTaskMem fsh

    let vsid = 
      opengl.glCreateShaderProgramv.Invoke (GL_VERTEX_SHADER, 1, [|vsh|]) 
      |> check_link_status "glCreateShaderProgramv"

    let fsid = 
      opengl.glCreateShaderProgramv.Invoke (GL_FRAGMENT_SHADER, 1, [|fsh|]) 
      |> check_link_status "glCreateShaderProgramv"

    let mutable pid = 0u
    opengl.glGenProgramPipelines.Invoke (1, &pid)
    opengl.glBindProgramPipeline.Invoke pid
    opengl.glUseProgramStages.Invoke (pid, GL_VERTEX_SHADER_BIT, vsid)
    opengl.glUseProgramStages.Invoke (pid, GL_FRAGMENT_SHADER_BIT, fsid)

    let start = now () |> float

    let invalidator = Invoker (fun () -> InvalidateRect (hwnd, 0n, false) |> ignore)

    let delayedInvoke (i : Invoker) = form.BeginInvoke i |> ignore
    let delayedInvalidate () = delayedInvoke invalidator

    let mutable localInSec = floor start
    let mutable localFrameCount = 0
    let mutable totalFrameCount = 0

    let onPaint o e =
      let size = form.ClientSize
      glViewport (0, 0, size.Width, size.Height)
      opengl.glBindFramebuffer.Invoke (GL_DRAW_FRAMEBUFFER, 0u)
      let now = now () |> float
      let nowInSec = floor now
      let time = now - start
      localFrameCount <- localFrameCount + 1
      totalFrameCount <- totalFrameCount + 1
      if nowInSec > localInSec then
        let sustainedFps = float totalFrameCount / time |> round |> int
        form.Text <- sprintf "FsShaderTracker - FPS: %d/%d" localFrameCount sustainedFps
        localInSec <- nowInSec
        localFrameCount <- 0
      opengl.glProgramUniform1f.Invoke (fsid, 0, float32 time)
      opengl.glProgramUniform2f.Invoke (fsid, 1, float32 size.Width, float32 size.Height)
      glRects (-1s, -1s, 1s, 1s)
      SwapBuffers hdc |> ignore
      delayedInvalidate ()

    form.Paint.AddHandler (PaintEventHandler onPaint)

    Application.Run form


